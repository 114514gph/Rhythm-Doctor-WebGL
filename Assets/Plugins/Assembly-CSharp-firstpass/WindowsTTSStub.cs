#if UNITY_WEBGL
using UnityEngine;

public class WindowsTTS : MonoBehaviour
{
    public static WindowsTTS instance { get; private set; }
    public bool isInitialized => false;
    
    void Awake() { instance = this; }
    
    public static bool IsNVDADetected() { return false; }
    public static bool IsSpeaking() { return false; }
    public static void Initialize() { }
    public static void Speak(string text) { }
    public static void Stop() { }
    public void SetVolume(float volume) { }
    public void SetRate(int rate) { }
}
#endif
