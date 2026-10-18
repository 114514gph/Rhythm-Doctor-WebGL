using DG.Tweening;
using RDLevelEditor;
using UnityEngine;

public class LevelSelectRobot : MonoBehaviour
{
	private scrChar entity;

	private scnLevelSelect levelSelect;

	[Header("Animation")]
	public bool faceTowardCamera;

	public float deltaX;

	public float delay;

	public float duration;

	public string animationLeft;

	public string animationRight;

	private Sequence sequence;

	private void Awake()
	{
		entity = GetComponent<scrChar>();
	}

	private void Update()
	{
		if (faceTowardCamera)
		{
			_ = Camera.main;
			bool flag = levelSelect.arrows.position.x - base.transform.position.x >= 0f;
			base.transform.ScaleX(flag ? 1 : (-1));
		}
	}

	private void Start()
	{
		levelSelect = scnLevelSelect.instance;
		if (duration > 0f)
		{
			SpawnTween();
		}
	}

	private void SpawnTween()
	{
		EaseFunction customEase = LevelEvent_MoveRow.VelocityBasedEase(0.5f, 0.5f);
		sequence = DOTween.Sequence().Append(base.transform.DOMoveX(deltaX, duration).SetRelative(isRelative: true).SetEase(customEase)).JoinCallback(() =>
		{
			entity.PlayExpression(animationRight ?? animationLeft);
		})
			.AppendInterval(delay)
			.Append(base.transform.DOMoveX(0f - deltaX, duration).SetRelative(isRelative: true).SetEase(customEase))
			.JoinCallback(() =>
			{
				entity.PlayExpression(animationLeft ?? animationRight);
			})
			.AppendInterval(delay)
			.SetLoops(-1);
	}
}
