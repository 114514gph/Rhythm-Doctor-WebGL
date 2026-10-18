#if UNITY_WEBGL
using UnityEngine;
using System;

// RDTools 桩
namespace RDTools
{
    public static class RD
    {
        public static T Find<T>() where T : class { return null; }
        public static T Find<T>(string name) where T : class { return null; }
    }
    public class Singleton<T> where T : class, new()
    {
        public static T instance { get; } = new T();
    }
    public class NonMonoSingleton<T> where T : class, new()
    {
        public static T instance { get; } = new T();
    }
}

// Steamworks 桩
namespace Steamworks
{
    public struct PublishedFileId_t { public ulong m_PublishedFileId; public PublishedFileId_t(ulong value) { m_PublishedFileId = value; } public static implicit operator ulong(PublishedFileId_t id) => id.m_PublishedFileId; public static implicit operator PublishedFileId_t(ulong value) => new PublishedFileId_t(value); }
    public struct AppId_t { public uint m_AppId; }
    public struct SteamId { public ulong value; }
    public static class SteamManager { public static bool initialized => false; }
    public static class SteamUser { public static SteamId GetSteamID() => new SteamId(); }
    public static class SteamUGC
    {
        public static void CreateItem(AppId_t appId, EWorkshopFileType fileType) { }
        public static void SubmitItemUpdate(PublishedFileId_t fileId, string changeNote) { }
        public static void SetItemTitle(PublishedFileId_t fileId, string title) { }
        public static void SetItemDescription(PublishedFileId_t fileId, string description) { }
        public static void SetItemContent(PublishedFileId_t fileId, string contentFolder) { }
        public static void SetItemPreview(PublishedFileId_t fileId, string previewFile) { }
    }
    public enum EWorkshopFileType { Community = 0, Microtransaction = 1, Collection = 2, Art = 3, Video = 4, Screenshot = 5, Game = 6, Software = 7, Concept = 8, WebGuide = 9, IntegratedGuide = 10, Merch = 11, ControllerBinding = 12, SteamworksAccessInvite = 13, SteamVideo = 14, GameManagedItem = 15, Max = 16 }
    public enum EResult { k_EResultOK = 1, k_EResultFail = 2, k_EResultNoConnection = 3 }
    public class CallResult<T> { public static CallResult<T> Create() => new CallResult<T>(); public void Set(SteamAPICall_t call, Action<T, bool> callback) { } }
    public struct SteamAPICall_t { public ulong value; }
    public class CreateItemResult_t { public EResult result; public PublishedFileId_t publishedFileId; public bool userNeedsToAcceptWorkshopLegalAgreement; }
    public class SubmitItemUpdateResult_t { public EResult result; public bool userNeedsToAcceptWorkshopLegalAgreement; }
}

// MultiWindow 桩
namespace MultiWindow
{
    public static class MultiWindowManager
    {
        public static bool isSupported => false;
        public static int windowCount => 1;
        public static void CreateWindow(string title, int width, int height) { }
        public static void CloseWindow(int index) { }
    }
}

// UnityFileDialog 桩
namespace UnityFileDialog
{
    public static class FileDialog
    {
        public static string[] OpenFilePanel(string title, string directory, string extension, bool multiselect) => new string[0];
        public static string[] OpenFolderPanel(string title, string directory, bool multiselect) => new string[0];
        public static string SaveFilePanel(string title, string directory, string defaultName, string extension) => null;
    }
}

// RewiredConsts 桩
namespace RewiredConsts
{
    public static class Action
    {
        public const int MoveHorizontal = 0;
        public const int MoveVertical = 1;
        public const int UIHorizontal = 2;
        public const int UIVertical = 3;
        public const int UISubmit = 4;
        public const int UICancel = 5;
        public const int Select = 6;
        public const int Back = 7;
        public const int Pause = 8;
    }
    public static class Category
    {
        public const int Default = 0;
        public const int UI = 1;
    }
    public static class Layout
    {
        public const int Default = 0;
    }
    public static class Player
    {
        public const int System = 0;
        public const int Player1 = 1;
    }
}

// SteamIntegration 相关桩
public static class SteamIntegration
{
    public static bool initialized => false;
    public static void Init() { }
    public static void Shutdown() { }
    public static void UnlockAchievement(string id) { }
    public static void StoreStats() { }
}

// SteamWorkshop 相关桩
public static class SteamWorkshop
{
    public static bool initialized => false;
    public static void UploadLevel(string path, string title, string description) { }
}

#endif

namespace SA
{
    namespace GoogleDoc
    {
        public class API { }
        public class Cell { }
        public class CellRange { }
        public class DataType { }
        public class DocTemplate { }
        public enum LangCode { English = 0, ChineseSimplified = 1, ChineseTraditional = 2, Spanish = 3, Korean = 4, Portuguese = 5, Polish = 6, Japanese = 7, French = 8, German = 9 }
        public class LangSection { public const int Levels = 0; public const int Translations = 1; public const int CustomMethods = 2; }
        public class Localization { }
        public class LocalizationClient { }
        public class LocalizationConfig { }
        public class RangeDirection { }
        public class Reader { }
        public class Settings { 
        public static Settings Instance { get { return null; } }
        public object GetDocByName(string name) { return null; }
    }
        public class SpreadsheetData { }
        public class VariableType { }
        public class WorksheetTemplate { }
    }
}

