using System.Collections.Generic;
using System.Linq;
using B83.Win32;
using UnityEngine;

public class FileDragAndDrop : MonoBehaviour
{
	public delegate void SendStringList(string[] aFiles);

	private UnityDragAndDropHook hook;

	public SendStringList OnFilesDropped;

	private void OnEnable()
	{
		hook = new UnityDragAndDropHook();
		hook.InstallHook();
		hook.OnDroppedFiles += OnFiles;
	}

	private void OnDisable()
	{
		hook.UninstallHook();
	}

	private void OnFiles(List<string> aFiles, POINT aPos)
	{
		string[] obj = new string[6]
		{
			"Dropped ",
			aFiles.Count.ToString(),
			" files at: ",
			null,
			null,
			null
		};
		POINT pOINT = aPos;
		obj[3] = pOINT.ToString();
		obj[4] = "\n";
		obj[5] = aFiles.Aggregate((string a, string b) => a + "\n" + b);
		Debug.LogError(string.Concat(obj));
		if (OnFilesDropped != null)
		{
			OnFilesDropped(aFiles.ToArray());
		}
	}
}
