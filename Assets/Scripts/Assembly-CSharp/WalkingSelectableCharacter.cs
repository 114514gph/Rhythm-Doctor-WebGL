using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class WalkingSelectableCharacter : MonoBehaviour
{
	[Serializable]
	public class Destination
	{
		public string id;

		public CharacterExpressionGroup expressions;

		public bool flipSprite;

		public bool keepSpriteVisible;

		public bool fadeSprite;

		public bool moveSprite;

		public int priority;

		public Vector2 offset;

		public bool preferred;

		private SelectableEntity _destEnt;

		public SelectableEntity destEnt
		{
			get
			{
				if (_destEnt == null)
				{
					if ((bool)scnLevelSelect.instance)
					{
						foreach (SelectableEntity item in scnLevelSelect.instance?.selectableEntities)
						{
							if (item.id == id)
							{
								_destEnt = item;
								break;
							}
						}
					}
					else
					{
						GameObject gameObject = GameObject.Find(id);
						if ((bool)gameObject)
						{
							Dictionary<Difficulty, Level> dictionary = new Dictionary<Difficulty, Level>();
							dictionary[Difficulty.Normal] = Level.Freezeshot;
							_destEnt = new SelectableCharacter
							{
								gameObject = gameObject,
								animation = gameObject.GetComponent<scrChar>(),
								levels = dictionary
							};
						}
					}
				}
				return _destEnt;
			}
		}
	}

	[Header("Components")]
	private scrChar entity;

	[Header("Settings")]
	public Character character;

	public float walkDuration;

	public string walkingExpression;

	public string walkingExpressionLeftOnly;

	public string tiredExpression;

	public List<Destination> dayDestinations = new List<Destination>();

	public List<Destination> nightDestinations = new List<Destination>();

	[NonSerialized]
	[Header("Private")]
	public Destination currentDestination;

	private Destination previousDestination;

	private Tween walkTween;

	private float walkTime;

	private bool isWalking;

	private bool isTired;

	private void Awake()
	{
		scnLevelSelect.instance?.walkingCharacters.Add(this);
		entity = base.gameObject.GetOrAddComponent<scrChar>();
		entity.customAnimation = base.gameObject.AddComponent<CustomAnimation>();
		entity.Setup(character);
	}

	public void UpdateCharacter(string destinationId, bool instant, bool day)
	{
		Difficulty key = (day ? Difficulty.Normal : Difficulty.Hard);
		List<Destination> list = (day ? dayDestinations : nightDestinations);
		Destination destination = null;
		bool flag = false;
		foreach (Destination dayDestination in dayDestinations)
		{
			if (dayDestination.destEnt is SelectableCharacter selectableCharacter && (bool)selectableCharacter.animation)
			{
				selectableCharacter.animation.visible = true;
			}
		}
		foreach (Destination nightDestination in nightDestinations)
		{
			if (nightDestination.destEnt is SelectableCharacter selectableCharacter2 && (bool)selectableCharacter2.animation)
			{
				selectableCharacter2.animation.visible = true;
			}
		}
		foreach (Destination item in list)
		{
			if (!(item.id != destinationId) && (!(item.destEnt is SelectableCharacter selectableCharacter3) || !selectableCharacter3.levels[key].Locked()))
			{
				flag = true;
				destination = item;
				break;
			}
		}
		if (destination == null)
		{
			int num = -1;
			foreach (Destination item2 in list)
			{
				if (item2.priority > num && (num <= -1 || !(item2.destEnt is SelectableCharacter selectableCharacter4) || !selectableCharacter4.levels[key].Locked()))
				{
					num = item2.priority;
					destination = item2;
					flag = item2.preferred;
				}
			}
		}
		if (instant)
		{
			isTired = false;
		}
		if ((flag && currentDestination != destination) | instant)
		{
			MoveCharacter(destination, instant);
		}
	}

	public void MoveCharacter(Destination destination, bool instant)
	{
		previousDestination = currentDestination;
		currentDestination = destination;
		if (destination == null)
		{
			return;
		}
		isWalking = !instant;
		if (isTired)
		{
			return;
		}
		SelectableEntity destEnt = destination.destEnt;
		Vector2 vector = destEnt.gamePosition + destination.offset;
		Vector3 endValue = entity.transform.parent.InverseTransformPoint(vector);
		float duration = (instant ? 0f : walkDuration);
		if (walkTween != null)
		{
			walkTween.Kill();
		}
		if (instant)
		{
			entity.transform.position = vector;
		}
		else
		{
			walkTween = entity.transform.DOLocalMove(endValue, duration).SetEase(Ease.OutSine);
		}
		Destination destination2 = previousDestination;
		if (destination2 != null && destination2.moveSprite && previousDestination.destEnt is SelectableCharacter selectableCharacter)
		{
			selectableCharacter.animation.transform.DOKill();
			selectableCharacter.animation.transform.DOMoveY(selectableCharacter.gamePosition.y, instant ? 0f : 1f);
		}
		if (destEnt is SelectableCharacter selectableCharacter2 && (bool)selectableCharacter2.animation)
		{
			if (instant)
			{
				entity.shaderDataSource = selectableCharacter2.animation;
				entity.shaderData = selectableCharacter2.animation.shaderData;
			}
			else
			{
				RDShaderProperties rDShaderProperties = new RDShaderProperties();
				rDShaderProperties.borderColor = Color.black;
				rDShaderProperties.outline = 1;
				entity.shaderDataSource = entity;
				entity.shaderData = rDShaderProperties;
				entity.shaderData.SetPropertiesInRenderer(entity.shaderRenderer);
			}
			entity.shaderData.SetPropertiesInRenderer(entity.shaderRenderer);
			if (destination.moveSprite)
			{
				selectableCharacter2.animation.transform.DOKill();
				selectableCharacter2.animation.transform.DOMoveY(selectableCharacter2.gamePosition.y + 40f, instant ? 0f : 2f);
			}
		}
		Vector3 position = entity.transform.position;
		Vector2 vector2 = (Vector2)destination.destEnt.gameObject.transform.position + currentDestination.offset;
		string expression = ((walkingExpressionLeftOnly.IsNullOrEmpty() || vector2.x - position.x > 0f) ? walkingExpression : walkingExpressionLeftOnly);
		if (!entity.IsCurrentExpression(expression))
		{
			entity.PlayExpression(expression);
		}
	}

	private void LateUpdate()
	{
		Difficulty difficulty = Difficulty.Normal;
		if ((bool)scnLevelSelect.instance)
		{
			difficulty = scnLevelSelect.instance.currentDifficulty;
		}
		if (difficulty != Difficulty.Normal)
		{
			_ = nightDestinations;
		}
		else
		{
			_ = dayDestinations;
		}
		entity.visible = currentDestination != null;
		UpdateTiredness();
		if (!entity.visible)
		{
			isWalking = false;
			return;
		}
		Vector3 position = entity.transform.position;
		Vector2 b = (currentDestination.moveSprite ? currentDestination.destEnt.gamePosition : ((Vector2)currentDestination.destEnt.gameObject.transform.position)) + currentDestination.offset;
		float num = Vector2.Distance(position, b);
		bool flag = num < 14f;
		bool flag2 = (flag ? currentDestination.flipSprite : (walkingExpressionLeftOnly.IsNullOrEmpty() && b.x - position.x < 0f));
		if (!isTired)
		{
			entity.transform.ScaleX((!flag2) ? 1 : (-1));
		}
		Rank rank = 4;
		LevelState levelState = LevelState.Passed;
		if (currentDestination.destEnt is SelectableCharacter selectableCharacter && (bool)selectableCharacter.animation)
		{
			Level level = selectableCharacter.levels[difficulty];
			levelState = Persistence.GetStateFromLevel(level);
			rank = Persistence.GetLevelRank(level);
			if (!currentDestination.keepSpriteVisible)
			{
				selectableCharacter.animation.visible = !flag;
			}
			else if (currentDestination.fadeSprite)
			{
				Material material = selectableCharacter.animation.shaderRenderer.material;
				float t = Mathf.InverseLerp(4f, 25f, num);
				float num2 = Mathf.Lerp(0.5f, 1f, t);
				material.SetFloat("_Opacity", num2);
				material.SetColor("_GlowColor", material.GetColor("_GlowColor").WithAlpha(num2));
			}
			if (currentDestination.id == "5-1" && Persistence.GetTriesToClearLevel(Level.Freezeshot.ToString()) <= 0 && !Level.Freezeshot.Passed())
			{
				selectableCharacter.animation.visible = false;
			}
		}
		string expression = currentDestination.expressions.GetExpression(levelState, rank);
		if (flag && !entity.IsCurrentExpression(expression))
		{
			entity.PlayExpression(expression);
			isWalking = false;
		}
	}

	private void UpdateTiredness()
	{
		if (!tiredExpression.IsNullOrEmpty())
		{
			if (walkTime >= 6f)
			{
				if (isTired)
				{
					isTired = false;
					MoveCharacter(currentDestination, instant: false);
				}
			}
			else if (walkTime >= 4f)
			{
				if (!entity.IsCurrentExpression(tiredExpression))
				{
					entity.PlayExpression(tiredExpression);
				}
				isTired = true;
				walkTween.Kill();
			}
		}
		walkTime = (isWalking ? (walkTime + Time.deltaTime) : 0f);
	}
}
