#if UNITY_EDITOR || !UNITY_WEBGL
using Ink;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.Networking;

public static class RDPluginsUtils
{
	public static string DataPath => Application.persistentDataPath;

	public static bool AnyErrors(this UnityWebRequest uwr)
	{
		UnityWebRequest.Result result = uwr.result;
		return result == UnityWebRequest.Result.ConnectionError || result == UnityWebRequest.Result.DataProcessingError || result == UnityWebRequest.Result.ProtocolError;
	}

	public static Story CompileInk(string ink)
	{
		Debug.Log("Compiling ink:\n\n" + ink);
		return new Compiler(ink, new Compiler.Options
		{
			errorHandler = (string message, ErrorType type) =>
			{
				Debug.LogWarning("Couldn't parse log " + message);
			}
		}).Compile();
	}
}

#endif
