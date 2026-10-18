using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace RDLevelEditor
{
	public class OGGConverter : RDEditorBase
	{
		public AudioclipToOggEncoder oggEncoder;

		public static IEnumerator Convert(string filename, AudioClip audioClip, Action<string> onComplete)
		{
			scnEditor editor = scnEditor.instance;
			string oggFilename = Path.ChangeExtension(filename, "ogg");
			string text = Path.Combine(Path.GetDirectoryName(editor.openedFilePath), oggFilename);
			if (!RDFile.Exists(text))
			{
				foreach (DialogButton button in editor.dialog.buttons)
				{
					button.gameObject.SetActive(value: false);
				}
				long t = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
				yield return editor.GetComponent<AudioclipToOggEncoder>().EncodeToOgg(audioClip, 0f, audioClip.length, text, (float percentage, MemoryStream _) =>
				{
					string arg = RDString.Get("editor.PublishLevel.Converting").Replace("[filename]", filename);
					editor.dialog.message.text = $"{arg} {percentage:F0}%";
				});
				MonoBehaviour.print($"Took {DateTimeOffset.UtcNow.ToUnixTimeSeconds() - t} seconds to convert {oggFilename}");
			}
			editor.dialog.Hide();
			onComplete?.Invoke(oggFilename);
			yield return oggFilename;
		}
	}
}
