using System;

namespace Rewired.Utils.Interfaces { }
namespace Rewired.Utils.Platforms { }
namespace Rewired.Utils.Libraries.TinyJson { }
namespace Rewired.Dev { }
namespace Rewired.Interfaces
{
    public interface ILocalizedStringProvider
    {
        bool TryGetLocalizedString(string key, out string value);
    }
}
namespace Rewired.Data { }
namespace Rewired.Data.Mapping { }
namespace Rewired.Platforms { }

namespace TempoStudio
{
    public class TempoSound
    {
        public string name;
        public float volume;
        public void Play() { }
        public void Stop() { }
    }
}

namespace Steamworks
{
    public class UserAchievementStored_t
    {
        public ulong steamIDUser;
        public bool groupAchievement;
        public string achievementName;
        public uint curProgress;
        public uint maxProgress;
    }
    public struct CGameID { public ulong value; }
    public class Callback<T> where T : class
    {
        public static Callback<T> Create(Action<T> callback) { return null; }
        public void Dispose() { }
    }
    public class DownloadItemResult_t
    {
        public AppId_t appID;
        public PublishedFileId_t publishedFileId;
        public EResult result;
    }
    public class GameOverlayActivated_t { public byte active; }
    public class RemoteStoragePublishedFileSubscribed_t
    {
        public PublishedFileId_t publishedFileId;
        public AppId_t appID;
    }
    public class RemoteStoragePublishedFileUnsubscribed_t
    {
        public PublishedFileId_t publishedFileId;
        public AppId_t appID;
    }
    public class ItemInstalled_t
    {
        public AppId_t appID;
        public PublishedFileId_t publishedFileId;
    }
    public struct UGCUpdateHandle_t { public ulong value; }
    public class DeleteItemResult_t
    {
        public EResult result;
        public PublishedFileId_t publishedFileId;
    }
    public class SteamUGCQueryCompleted_t
    {
        public EResult result;
        public ulong handle;
        public uint numResultsReturned;
        public uint totalMatchingResults;
        public bool cachedData;
    }
    public class RemoteStorageUnsubscribePublishedFileResult_t
    {
        public EResult result;
        public PublishedFileId_t publishedFileId;
    }
    public class RemoteStorageDownloadUGCResult_t
    {
        public EResult result;
        public PublishedFileId_t publishedFileId;
        public AppId_t appID;
        public string fileName;
        public int fileSize;
    }
    public enum EWorkshopVote { Unvoted = 0, For = 1, Against = 2, Later = 3 }
}

namespace UnityPlayerWindow
{
    public class PlayerWindow { }
}

namespace Steamworks
{
    public static partial class SteamApps
    {
        public static uint GetCurrentBetaName(out string betaName, int bufferSize) { betaName = ""; return 0; }
    }
}
