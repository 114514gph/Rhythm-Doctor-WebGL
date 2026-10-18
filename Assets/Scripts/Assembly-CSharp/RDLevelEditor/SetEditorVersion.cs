using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class SetEditorVersion : MonoBehaviour
	{
		private const string keyword = "[editor_version]";

		public void Start()
		{
			Text component = GetComponent<Text>();
			TextMesh component2 = GetComponent<TextMesh>();
			string displayVersion = RDEditorConstants.DisplayVersion;
			if (component != null)
			{
				component.text = component.text.Replace("[editor_version]", displayVersion);
			}
			else if (component2 != null)
			{
				component2.text = component2.text.Replace("[editor_version]", displayVersion);
			}
			else
			{
				MonoBehaviour.print("couldn't find either UI.Text or TextMesh to replace text!");
			}
		}
	}
}
