using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Steamworks;
using UnityEngine;

public static class SteamWorkshop
{
	public struct ResultItem
	{
		public PublishedFileId_t id;

		public string title;

		public string path;

		public string previewImagePath;

		public ResultItem(PublishedFileId_t id, string title, string path, string previewImagePath)
		{
			this.id = id;
			this.title = title;
			this.path = path;
			this.previewImagePath = previewImagePath;
		}
	}

	public enum Error
	{
		None = -1,
		LevelNameNullOrEmpty = 1,
		PreviewImageNotFound = 2,
		PreviewImageInvalidSize = 7,
		ItemDirectoryNotFound = 3,
		CreateItemFailed = 4,
		InvalidUpdateHandle = 5,
		UpdateItemFailed = 6,
		DeleteItemFailed = 8,
		UnsubscribeFailed = 12,
		GetItemTitleQueryFailed = 13,
		GetItemTitleFailed = 14,
		GetPublishedItemsQueryFailed = 16,
		GetPublishedItemInfo = 17,
		GetItemPreviewFile = 18,
		ItemNotReadyToUse = 15
	}

	public enum Warnings
	{
		TitleTruncated = 0,
		DescriptionTruncated = 1,
		TagsTruncated = 2
	}

	public const int MaxItemsPerPage = 50;

	public static bool isDownloadingItem;

	public static int itemsUnsubscribed;

	public static List<Error> errors;

	public static HashSet<Warnings> warnings;

	public static List<ResultItem> resultItems;

	public static int totalPublishedItems;

	private static bool lastCallResultEnded;

	private static PublishedFileId_t lastUploadId;

	private static PublishedFileId_t lastDownloadId;

	private static PublishedFileId_t[] subscribedItemsId;

	private static Dictionary<PublishedFileId_t, string> newSubscribedItemsDict;

	private static UGCUpdateHandle_t updateHandle;

	private static ulong lastBytesProcessed;

	private static float itemUploadProgress;

	private static bool unsubscribedWithinGame;

	private static CallResult<CreateItemResult_t> CreateItemResult;

	private static CallResult<DeleteItemResult_t> DeleteItemResult;

	private static CallResult<SubmitItemUpdateResult_t> SubmitItemUpdateResult;

	private static CallResult<SteamUGCQueryCompleted_t> SteamUGCQueryCompleted;

	private static CallResult<RemoteStorageUnsubscribePublishedFileResult_t> RSUnsubscribePublishedFileResult;

	private static CallResult<RemoteStorageDownloadUGCResult_t> RemoteStorageDownloadUGCResult;

	private static Callback<DownloadItemResult_t> DownloadItemResult;

	private static Callback<GameOverlayActivated_t> GameOverlayActivated;

	private static Callback<RemoteStoragePublishedFileSubscribed_t> RSPublishedFileSubscribed;

	private static Callback<RemoteStoragePublishedFileUnsubscribed_t> RSPublishedFileUnsubscribed;

	private static Callback<ItemInstalled_t> ItemInstalled;

	private static scnCLS cls => scnCLS.instance;

	public static int ItemsSubscribed
	{
		get
		{
			if (newSubscribedItemsDict != null)
			{
				return newSubscribedItemsDict.Count;
			}
			return 0;
		}
	}

	public static bool OperationSucceeded => errors.Count == 0;

	public static string GetCurrentDownloadingItemTitle
	{
		get
		{
			string result = string.Empty;
			foreach (KeyValuePair<PublishedFileId_t, string> item in newSubscribedItemsDict)
			{
				EItemState itemState = (EItemState)SteamUGC.GetItemState(item.Key);
				if (itemState.HasFlag(EItemState.k_EItemStateDownloading))
				{
					result = item.Value;
					break;
				}
			}
			return result;
		}
	}

	public static int ItemsInstalled
	{
		get
		{
			int num = 0;
			foreach (KeyValuePair<PublishedFileId_t, string> item in newSubscribedItemsDict)
			{
				if (item.Key.ReadyToUse())
				{
					num++;
				}
			}
			return num;
		}
	}

	public static void Initialize()
	{
		DownloadItemResult = Callback<DownloadItemResult_t>.Create(OnDownloadItemResult);
		GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnToggleGameOverlay);
		RSPublishedFileSubscribed = Callback<RemoteStoragePublishedFileSubscribed_t>.Create(OnPublishedFileSubscribed);
		RSPublishedFileUnsubscribed = Callback<RemoteStoragePublishedFileUnsubscribed_t>.Create(OnPublishedFileUnsubscribed);
		ItemInstalled = Callback<ItemInstalled_t>.Create(OnItemInstalled);
		UpdateSubscribedItemsArray();
		itemsUnsubscribed = 0;
	}

	public static void OpenWorkshop()
	{
		SteamFriends.ActivateGameOverlayToWebPage("https://steamcommunity.com//workshop/browse?appid=774181", EActivateGameOverlayToWebPageMode.k_EActivateGameOverlayToWebPageMode_Modal);
	}

	public static void OnToggleGameOverlay(GameOverlayActivated_t param)
	{
		bool flag = !Convert.ToBoolean(param.m_bActive);
		cls.CanReceiveInput = flag;
		cls.levelDetail.canReceiveInputs = flag;
	}

	public static void OpenAgreement()
	{
		SteamFriends.ActivateGameOverlayToWebPage("https://steamcommunity.com//sharedfiles/workshoplegalagreement", EActivateGameOverlayToWebPageMode.k_EActivateGameOverlayToWebPageMode_Modal);
	}

	public static bool OverlayEnabled()
	{
		if (SteamIntegration.initialized)
		{
			return SteamUtils.IsOverlayEnabled();
		}
		return false;
	}

	public static void ClearItemsInfoCache()
	{
		newSubscribedItemsDict = new Dictionary<PublishedFileId_t, string>();
		itemsUnsubscribed = 0;
	}

	public static void OnPublishedFileSubscribed(RemoteStoragePublishedFileSubscribed_t param)
	{
		if (!(param.m_nAppID != SteamIntegration.gameID.AppID()))
		{
			newSubscribedItemsDict.Add(param.m_nPublishedFileId, string.Empty);
			cls.StartCoroutine(GetItemTitle(param.m_nPublishedFileId));
		}
	}

	private static IEnumerator GetItemTitle(PublishedFileId_t publishedFileId)
	{
		errors = new List<Error>();
		lastCallResultEnded = false;
		_ = string.Empty;
		PublishedFileId_t[] obj = new PublishedFileId_t[1] { publishedFileId };
		SteamAPICall_t hAPICall = SteamUGC.SendQueryUGCRequest(SteamUGC.CreateQueryUGCDetailsRequest(obj, (uint)obj.Length));
		SteamUGCQueryCompleted_t queryResult = default;
		SteamUGCQueryCompleted = CallResult<SteamUGCQueryCompleted_t>.Create();
		SteamUGCQueryCompleted.Set(hAPICall, (SteamUGCQueryCompleted_t param, bool bIOFailure) =>
		{
			if (!bIOFailure && param.m_eResult == EResult.k_EResultOK)
			{
				queryResult = param;
			}
			else
			{
				errors.Add(Error.GetItemTitleQueryFailed);
			}
			lastCallResultEnded = true;
		});
		yield return new WaitUntil(() => lastCallResultEnded);
		if (OperationSucceeded)
		{
			if (SteamUGC.GetQueryUGCResult(queryResult.m_handle, 0u, out var pDetails))
			{
				newSubscribedItemsDict[publishedFileId] = pDetails.m_rgchTitle;
			}
			else
			{
				errors.Add(Error.GetItemTitleFailed);
			}
		}
	}

	public static void OnPublishedFileUnsubscribed(RemoteStoragePublishedFileUnsubscribed_t param)
	{
		if (!(param.m_nAppID != SteamIntegration.gameID.AppID()))
		{
			RDBaseDllDummy.printem($"item {param.m_nPublishedFileId} unsubscribed from game: {unsubscribedWithinGame}");
			bool flag = newSubscribedItemsDict.Remove(param.m_nPublishedFileId);
			if (!unsubscribedWithinGame && !scnCLS.instance.ShowingWard && !flag)
			{
				itemsUnsubscribed++;
			}
			unsubscribedWithinGame = false;
		}
	}

	private static void UpdateSubscribedItemsArray()
	{
		uint numSubscribedItems = SteamUGC.GetNumSubscribedItems();
		subscribedItemsId = new PublishedFileId_t[numSubscribedItems];
		SteamUGC.GetSubscribedItems(subscribedItemsId, numSubscribedItems);
		newSubscribedItemsDict = new Dictionary<PublishedFileId_t, string>();
	}

	private static void OnItemInstalled(ItemInstalled_t param)
	{
		if (!(param.m_unAppID != SteamIntegration.gameID.AppID()))
		{
			if (Array.IndexOf(subscribedItemsId, param.m_nPublishedFileId) == -1)
			{
				RDBaseDllDummy.printem($"New item {param.m_nPublishedFileId} installed");
			}
			else
			{
				RDBaseDllDummy.printem($"Item {param.m_nPublishedFileId} updated");
			}
		}
	}

	public static IEnumerator GetPublishedItems(int pageNumber = 1, Action<int> OnLoadingLevel = null)
	{
		Debug.Log($"pageNumber: {pageNumber}");
		totalPublishedItems = 0;
		resultItems = new List<ResultItem>();
		errors = new List<Error>();
		int itemsNumber = 0;
		UGCQueryHandle_t queryHandle = default;
		try
		{
			queryHandle = SteamUGC.CreateQueryUserUGCRequest(SteamUser.GetSteamID().GetAccountID(), EUserUGCList.k_EUserUGCList_Published, EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items, EUserUGCListSortOrder.k_EUserUGCListSortOrder_LastUpdatedDesc, AppId_t.Invalid, SteamUtils.GetAppID(), (uint)pageNumber);
			SteamAPICall_t hAPICall = SteamUGC.SendQueryUGCRequest(queryHandle);
			SteamUGCQueryCompleted = CallResult<SteamUGCQueryCompleted_t>.Create();
			lastCallResultEnded = false;
			SteamUGCQueryCompleted.Set(hAPICall, (SteamUGCQueryCompleted_t param, bool bIOFailure) =>
			{
				if (bIOFailure || param.m_eResult != EResult.k_EResultOK)
				{
					Debug.Log($"GetPublishedItems query failed: bIOFailure = {bIOFailure}, result = {param.m_eResult}");
					errors.Add(Error.GetPublishedItemsQueryFailed);
				}
				else
				{
					Debug.Log($"itemsNumber = {param.m_unNumResultsReturned}, itemsTotal = {param.m_unTotalMatchingResults}, cachedData = {param.m_bCachedData}, nextPageCursor = {param.m_rgchNextCursor}");
					itemsNumber = (int)param.m_unNumResultsReturned;
					totalPublishedItems = (int)param.m_unTotalMatchingResults;
				}
				lastCallResultEnded = true;
			});
		}
		catch (Exception ex)
		{
			Debug.Log("Error getting published items: " + ex.Message);
		}
		yield return new WaitUntil(() => lastCallResultEnded);
		if (!OperationSucceeded)
		{
			SteamUGC.ReleaseQueryUGCRequest(queryHandle);
			yield break;
		}
		for (uint i = 0u; i < itemsNumber; i++)
		{
			OnLoadingLevel((int)i);
			if (SteamUGC.GetQueryUGCResult(queryHandle, i, out var itemDetails))
			{
				lastCallResultEnded = false;
				string previewImagePath = Path.Combine(LevelValidation.TempLevelsFolder, $"{itemDetails.m_nPublishedFileId}.png");
				SteamAPICall_t hAPICall2 = SteamRemoteStorage.UGCDownloadToLocation(itemDetails.m_hPreviewFile, previewImagePath, 0u);
				RemoteStorageDownloadUGCResult = CallResult<RemoteStorageDownloadUGCResult_t>.Create();
				RemoteStorageDownloadUGCResult.Set(hAPICall2, (RemoteStorageDownloadUGCResult_t param, bool bIOFailure) =>
				{
					if (bIOFailure || param.m_eResult != EResult.k_EResultOK)
					{
						Debug.Log($"UGCDownloadToLocation failed for item {i}: bIOFailure = {bIOFailure}, result = {param.m_eResult}");
						errors.Add(Error.GetItemPreviewFile);
					}
					else
					{
						Debug.Log($"item {i}: id = {itemDetails.m_nPublishedFileId}, title = {itemDetails.m_rgchTitle}, previewImagePath = {previewImagePath}");
						resultItems.Add(new ResultItem(itemDetails.m_nPublishedFileId, itemDetails.m_rgchTitle, string.Empty, previewImagePath));
					}
					lastCallResultEnded = true;
				});
				yield return new WaitUntil(() => lastCallResultEnded);
			}
			else
			{
				Debug.Log($"GetQueryUGCResult failed for index {i}: query handle is invalid or the index is out of bounds");
				errors.Add(Error.GetPublishedItemInfo);
			}
		}
		SteamUGC.ReleaseQueryUGCRequest(queryHandle);
		errors = errors.Distinct().ToList();
		Debug.Log($"completed: {resultItems.Count} items, {errors.Count} errors");
	}

	public static bool ReadyToUse(this PublishedFileId_t publishedFileId)
	{
		EItemState itemState = (EItemState)SteamUGC.GetItemState(publishedFileId);
		if (!itemState.HasFlag(EItemState.k_EItemStateDownloading) && !itemState.HasFlag(EItemState.k_EItemStateDownloadPending))
		{
			return !itemState.HasFlag(EItemState.k_EItemStateNeedsUpdate);
		}
		return false;
	}

	public static IEnumerator GetLocalItems()
	{
		errors = new List<Error>();
		UpdateSubscribedItemsArray();
		PublishedFileId_t[] array = subscribedItemsId;
		foreach (PublishedFileId_t publishedFileId_t in array)
		{
			if (!publishedFileId_t.ReadyToUse() && SteamUGC.DownloadItem(publishedFileId_t, bHighPriority: true))
			{
				errors.Add(Error.ItemNotReadyToUse);
				lastCallResultEnded = false;
				isDownloadingItem = true;
				lastDownloadId = publishedFileId_t;
				yield return new WaitUntil(() => lastCallResultEnded);
				continue;
			}
			SteamUGC.GetItemInstallInfo(publishedFileId_t, out var _, out var pchFolder, 1000u, out var _);
			if (!string.IsNullOrEmpty(pchFolder) && Directory.Exists(pchFolder))
			{
				int count = cls.levelsData.Count;
				DesktopLevelLoader.LoadLevelPreview(pchFolder, fromSteam: true);
				int count2 = cls.levelsData.Count;
				if (count < count2)
				{
					cls.levelsData[count2 - 1].publishedId = publishedFileId_t;
				}
			}
		}
		errors = errors.Distinct().ToList();
		DesktopLevelLoader.LoadLevelsPreviewFromPath(LevelValidation.CustomLevelsPath);
	}

	private static void OnDownloadItemResult(DownloadItemResult_t param)
	{
		if (!(param.m_unAppID != SteamIntegration.gameID.AppID()))
		{
			if (param.m_eResult == EResult.k_EResultOK)
			{
				RDBaseDllDummy.printesw($"ERROR OnDownloadItemResult: {param.m_eResult}");
			}
			isDownloadingItem = false;
			lastCallResultEnded = true;
		}
	}

	public static void CheckDownloadInfo()
	{
		bool itemDownloadInfo = SteamUGC.GetItemDownloadInfo(lastDownloadId, out var punBytesDownloaded, out var punBytesTotal);
		if ((float)punBytesDownloaded > 0f && (float)punBytesTotal > 0f && punBytesTotal > punBytesDownloaded)
		{
			RDBaseDllDummy.printem($"Download info: {lastDownloadId}, {punBytesDownloaded}, {punBytesTotal}, {itemDownloadInfo}");
		}
	}

	public static void ShowItemOnWorkshop(PublishedFileId_t publishedFileId)
	{
		string empty = string.Empty;
		empty = ((!DebugSettings.instance.RunningOnSteamDeck) ? "steam://url/CommunityFilePage/" : "https://steamcommunity.com/sharedfiles/filedetails/?id=");
		empty += publishedFileId.ToString();
		SteamFriends.ActivateGameOverlayToWebPage(empty);
	}

	public static void UnsubscribeItem(PublishedFileId_t publishedFileId)
	{
		errors = new List<Error>();
		SteamAPICall_t hAPICall = SteamUGC.UnsubscribeItem(publishedFileId);
		RSUnsubscribePublishedFileResult = CallResult<RemoteStorageUnsubscribePublishedFileResult_t>.Create();
		RSUnsubscribePublishedFileResult.Set(hAPICall, (RemoteStorageUnsubscribePublishedFileResult_t param, bool bIOFailure) =>
		{
			unsubscribedWithinGame = true;
			if (!bIOFailure && param.m_eResult == EResult.k_EResultOK)
			{
				cls.levelDetail.StartCoroutine(cls.levelDetail.RemoveCurrentLevel());
			}
			else
			{
				errors.Add(Error.UnsubscribeFailed);
			}
		});
	}

	private static IEnumerator DeleteItem(PublishedFileId_t publishedFileId)
	{
		SteamAPICall_t hAPICall = SteamUGC.DeleteItem(publishedFileId);
		DeleteItemResult = CallResult<DeleteItemResult_t>.Create();
		lastCallResultEnded = false;
		DeleteItemResult.Set(hAPICall, (DeleteItemResult_t param, bool bIOFailure) =>
		{
			if (bIOFailure || param.m_eResult != EResult.k_EResultOK)
			{
				errors.Add(Error.DeleteItemFailed);
				RDBaseDllDummy.printem($"Couldn't delete the item in the workshop: bIOFailure = {bIOFailure}, result = {param.m_eResult}, for item = {param.m_nPublishedFileId}");
			}
			lastCallResultEnded = true;
		});
		yield return new WaitUntil(() => lastCallResultEnded);
	}

	public static IEnumerator UploadToWorkshop(string title, string contentPath, string previewPath, string description, string[] tags, PublishedFileId_t updateId = default(PublishedFileId_t))
	{
		RDBaseDllDummy.printem("UploadToWorkshop: " + contentPath + " --- " + previewPath);
		errors = new List<Error>();
		warnings = new HashSet<Warnings>();
		if (string.IsNullOrEmpty(title))
		{
			errors.Add(Error.LevelNameNullOrEmpty);
		}
		else
		{
			title = title.TruncateAnyEncoding(127, () =>
			{
				warnings.Add(Warnings.TitleTruncated);
			});
		}
		if (string.IsNullOrEmpty(previewPath) || !RDFile.Exists(previewPath))
		{
			errors.Add(Error.PreviewImageNotFound);
		}
		else
		{
			long length = new FileInfo(previewPath).Length;
			if (length <= 16 || length >= 1000000)
			{
				errors.Add(Error.PreviewImageInvalidSize);
				RDBaseDllDummy.printem($"Preview Image invalid size {length}");
			}
		}
		if (string.IsNullOrEmpty(contentPath) || !RDDirectory.Exists(contentPath))
		{
			errors.Add(Error.ItemDirectoryNotFound);
		}
		if (!OperationSucceeded)
		{
			yield break;
		}
		for (int num = 0; num < tags.Length; num++)
		{
			tags[num] = tags[num].Trim(' ');
			tags[num] = tags[num].TruncateAnyEncoding(64, () =>
			{
				warnings.Add(Warnings.TagsTruncated);
			});
		}
		if (!string.IsNullOrEmpty(description) && description.Length > 1000)
		{
			description = description.TruncateAnyEncoding(1000, () =>
			{
				warnings.Add(Warnings.DescriptionTruncated);
			});
		}
		lastUploadId = updateId;
		SteamAPICall_t hAPICall;
		if (lastUploadId == default(PublishedFileId_t))
		{
			hAPICall = SteamUGC.CreateItem(SteamUtils.GetAppID(), EWorkshopFileType.k_EWorkshopFileTypeFirst);
			CreateItemResult = CallResult<CreateItemResult_t>.Create();
			lastCallResultEnded = false;
			CreateItemResult.Set(hAPICall, (CreateItemResult_t param, bool bIOFailure) =>
			{
				if (!bIOFailure && param.m_eResult == EResult.k_EResultOK)
				{
					lastUploadId = param.m_nPublishedFileId;
				}
				else
				{
					errors.Add(Error.CreateItemFailed);
					RDBaseDllDummy.printem($"Create item failed: bIOFailure = {bIOFailure}, result = {param.m_eResult}, must accept WLA = {param.m_bUserNeedsToAcceptWorkshopLegalAgreement}");
				}
				lastCallResultEnded = true;
			});
			yield return new WaitUntil(() => lastCallResultEnded);
			if (!OperationSucceeded)
			{
				yield break;
			}
		}
		updateHandle = SteamUGC.StartItemUpdate(SteamIntegration.gameID.AppID(), lastUploadId);
		if (!SteamUGC.SetItemTitle(updateHandle, title) || !SteamUGC.SetItemContent(updateHandle, contentPath) || !SteamUGC.SetItemPreview(updateHandle, previewPath) || (!string.IsNullOrEmpty(description) && !SteamUGC.SetItemDescription(updateHandle, description)) || (tags != null && tags.Length >= 1 && !SteamUGC.SetItemTags(updateHandle, tags)) || !SteamUGC.SetItemVisibility(updateHandle, ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPublic))
		{
			errors.Add(Error.InvalidUpdateHandle);
			yield return DeleteItem(lastUploadId);
			yield break;
		}
		RDBaseDllDummy.printem($"Uploading to workshop with id {lastUploadId}");
		hAPICall = SteamUGC.SubmitItemUpdate(updateHandle, null);
		SubmitItemUpdateResult = CallResult<SubmitItemUpdateResult_t>.Create();
		lastCallResultEnded = false;
		SubmitItemUpdateResult.Set(hAPICall, (SubmitItemUpdateResult_t param, bool bIOFailure) =>
		{
			if (bIOFailure || param.m_eResult != EResult.k_EResultOK)
			{
				errors.Add(Error.UpdateItemFailed);
				RDBaseDllDummy.printem($"SubmitItemUpdateResult - any failure: {bIOFailure}, result: {param.m_eResult}," + $" needs to accept legal agreement: {param.m_bUserNeedsToAcceptWorkshopLegalAgreement}");
			}
			lastCallResultEnded = true;
		});
		yield return new WaitUntil(() => lastCallResultEnded);
		if (!OperationSucceeded)
		{
			yield return DeleteItem(lastUploadId);
		}
		else
		{
			RDBaseDllDummy.printem($"UploadToWorkshop success: {OperationSucceeded}, errors: {errors.Count}");
		}
	}

	public static void CheckUploadInfo()
	{
		EItemUpdateStatus itemUpdateProgress = SteamUGC.GetItemUpdateProgress(updateHandle, out var punBytesProcessed, out var punBytesTotal);
		if ((float)punBytesProcessed > 0f && (float)punBytesTotal > 0f && punBytesTotal > punBytesProcessed && lastBytesProcessed != punBytesProcessed)
		{
			lastBytesProcessed = punBytesProcessed;
			itemUploadProgress = (float)punBytesProcessed / ((float)punBytesTotal * 1f);
			RDBaseDllDummy.printem(string.Format("{0} upload info: status = {1}, progress = {2}%", updateHandle, itemUpdateProgress, (itemUploadProgress * 100f).ToString("00.00")));
		}
	}

	public static void ShowLastUploadedItemOnWorkshop()
	{
		SteamFriends.ActivateGameOverlayToWebPage("steam://url/CommunityFilePage/" + lastUploadId.m_PublishedFileId);
	}
}
