using UnityEngine.Networking;

public static class UnityWebRequestExtensions
{
    public static bool AnyErrors(this UnityWebRequest uwr)
    {
        UnityWebRequest.Result result = uwr.result;
        return result == UnityWebRequest.Result.ConnectionError 
            || result == UnityWebRequest.Result.DataProcessingError 
            || result == UnityWebRequest.Result.ProtocolError;
    }
}
