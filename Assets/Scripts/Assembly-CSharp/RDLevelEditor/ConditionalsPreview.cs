using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class ConditionalsPreview : RDEditorBase
	{
		public RectTransform container;

		public RectTransform box;

		public RectTransform triangle;

		public int conditionalsCounter;

		private void Awake()
		{
			base.gameObject.SetActive(value: false);
		}

		public void Move(Vector2 position, bool topToBottom, float pivotX = 0f)
		{
			int num = ((!topToBottom) ? 1 : (-1));
			RectTransform component = GetComponent<RectTransform>();
			component.pivot = component.pivot.WithY(topToBottom ? 1 : 0);
			component.position = position;
			box.pivot = new Vector2(pivotX, topToBottom ? 1 : 0);
			box.anchorMin = box.anchorMin.WithY((!topToBottom) ? 1 : 0);
			box.anchorMax = box.anchorMax.WithY((!topToBottom) ? 1 : 0);
			triangle.eulerAngles = Vector3.forward * 90f * num;
			Shadow component2 = triangle.GetComponent<Shadow>();
			component2.effectDistance = new Vector2(Mathf.Abs(component2.effectDistance.x), Mathf.Abs(component2.effectDistance.y)) * -num;
			if (topToBottom)
			{
				triangle.SetAsFirstSibling();
			}
			else
			{
				triangle.SetAsLastSibling();
			}
		}

		public void Show(LevelEvent_Base levelEvent, Vector2 position, bool topToBottom, float pivotX = 0f)
		{
			foreach (Transform item in container)
			{
				Object.Destroy(item.gameObject);
			}
			List<Conditional> list = new List<Conditional>();
			List<bool> list2 = new List<bool>();
			foreach (Conditional globalConditional in base.editor.globalConditionals)
			{
				foreach (string globalConditional2 in levelEvent.globalConditionals)
				{
					if (!(globalConditional.gid != globalConditional2) || !("~" + globalConditional.gid != globalConditional2))
					{
						list.Add(globalConditional);
						list2.Add(globalConditional2.StartsWith("~"));
					}
				}
			}
			foreach (Conditional conditional in base.editor.conditionals)
			{
				foreach (int conditional2 in levelEvent.conditionals)
				{
					if (conditional.id == conditional2 || conditional.id == -conditional2 - 1)
					{
						list.Add(conditional);
						list2.Add(conditional2 < 0);
					}
				}
			}
			int num = ((!topToBottom) ? 1 : (-1));
			float num2 = 4f * (float)num;
			float y = base.editor.conditionalsPanel.conditionalButtonPrefab.GetComponent<RectTransform>().sizeDelta.y;
			conditionalsCounter = 0;
			foreach (Conditional item2 in list)
			{
				GameObject obj = Object.Instantiate(base.editor.conditionalsPanel.conditionalButtonPrefab, container);
				RectTransform component = obj.GetComponent<RectTransform>();
				ConditionalButton component2 = obj.GetComponent<ConditionalButton>();
				bool flag = list2[conditionalsCounter];
				component.anchorMin = new Vector2(0f, topToBottom ? 1 : 0);
				component.anchorMax = new Vector2(1f, topToBottom ? 1 : 0);
				component.anchoredPosition = new Vector2(0f, num2);
				component.pivot = new Vector2(0.5f, topToBottom ? 1 : 0);
				component.sizeDelta = component.sizeDelta.WithX(-4f);
				component2.eventTrigger.enabled = false;
				component2.Selected = false;
				component2.Negated = false;
				component2.conditional.enabled = false;
				component2.background.color = Color.clear;
				if (conditionalsCounter == 7)
				{
					component2.conditionalName.text = string.Format("+ {0} {1}", list.Count - conditionalsCounter, RDString.Get("editor.Conditionals.more"));
					component2.conditionalName.color = Color.white;
					component2.icon.gameObject.SetActive(value: false);
					conditionalsCounter++;
					break;
				}
				if (item2.IsGlobal())
				{
					component2.conditionalName.color = (flag ? new Color(1f, 0.5f, 0.5f) : Color.cyan);
					component2.conditionalName.text = (flag ? Conditionals.globalNegatedDescriptions[item2.gid] : item2.description);
				}
				else
				{
					component2.conditionalName.color = Color.white;
					component2.conditionalName.text = item2.description;
				}
				component2.icon.sprite = item2.GetSprite();
				component2.icon.color = Color.white;
				component2.negativeIcon.SetActive(flag);
				num2 += component.sizeDelta.y * (float)num;
				conditionalsCounter++;
			}
			bool flag2 = conditionalsCounter > 0;
			base.gameObject.SetActive(flag2);
			if (flag2)
			{
				float y2 = (float)conditionalsCounter * y + 8f;
				box.sizeDelta = box.sizeDelta.WithY(y2);
				Move(position, topToBottom, pivotX);
			}
		}

		public void Hide()
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
