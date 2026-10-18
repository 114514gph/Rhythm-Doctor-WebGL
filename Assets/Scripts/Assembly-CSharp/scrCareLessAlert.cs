using UnityEngine;
using UnityEngine.UI;

public class scrCareLessAlert : MonoBehaviour
{
	public RectTransform canvasRect;

	public Image boxImg;

	public Image buttonsImg;

	public Sprite[] boxSpr;

	public Sprite[] buttonsNeutSpr;

	public Sprite[] buttonsYesSpr;

	public Sprite[] buttonsNoSpr;

	public Text ok1Text;

	public Text ok2Text;

	public Text cancelText;

	public Color unselectedColor;

	public Color selectedColor;

	public void SetButtons(int mode, int type)
	{
		switch (mode)
		{
		case 0:
			buttonsImg.sprite = buttonsNeutSpr[type];
			ok1Text.color = unselectedColor;
			ok2Text.color = unselectedColor;
			cancelText.color = unselectedColor;
			break;
		case 1:
			buttonsImg.sprite = buttonsYesSpr[type];
			ok1Text.color = selectedColor;
			ok2Text.color = selectedColor;
			cancelText.color = unselectedColor;
			break;
		case 2:
			buttonsImg.sprite = buttonsNoSpr[type];
			ok1Text.color = unselectedColor;
			ok2Text.color = unselectedColor;
			cancelText.color = selectedColor;
			break;
		}
	}

	public void SetBox(int type)
	{
		boxImg.sprite = boxSpr[type];
		ok1Text.gameObject.SetActive(type == 0);
		ok2Text.gameObject.SetActive(type == 1);
		cancelText.gameObject.SetActive(type == 0);
	}
}
