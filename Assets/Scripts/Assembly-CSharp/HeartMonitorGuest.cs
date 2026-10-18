using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartMonitorGuest : MonoBehaviour
{
	[Header("Components")]
	public Image icon;

	public Text nameText;

	public Button button;

	public void Setup(GuestData data)
	{
		string text = RDString.Get("enum.GuestType." + data.type) + " " + data.name;
		icon.sprite = Resources.Load<Sprite>("CLS/MediaIcons/" + data.linkType);
		if (icon.sprite == null)
		{
			icon.sprite = Resources.Load<Sprite>("CLS/MediaIcons/Other");
		}
		nameText.text = text;
		RDStringToUIText.Apply(nameText);
		if (RDString.isCJK || Persistence.GetReducePixelFontUsage())
		{
			nameText.fontSize = 6;
		}
		Canvas.ForceUpdateCanvases();
		if (!data.link.IsNullOrEmpty())
		{
			button.onClick.AddListener(() =>
			{
				Application.OpenURL(data.link);
			});
			TextGenerator cachedTextGenerator = nameText.cachedTextGenerator;
			cachedTextGenerator.Populate(text, nameText.GetGenerationSettings(nameText.rectTransform.rect.size));
			IList<UIVertex> verts = cachedTextGenerator.verts;
			float num = float.MinValue;
			foreach (UIVertex item in verts)
			{
				num = Mathf.Max(num, item.position.x);
			}
			float num2 = (nameText.font.dynamic ? nameText.canvas.scaleFactor : 1f);
			float num3 = num / num2;
			icon.transform.MoveX(nameText.transform.position.x + num3 + 9f);
		}
		else
		{
			icon.gameObject.SetActive(value: false);
		}
	}
}
