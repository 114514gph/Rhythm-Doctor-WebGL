using DG.Tweening;
using UnityEngine;

public class RDBGWalkingChar : MonoBehaviour
{
	private const float endpointLeftX = -30f;

	private const float endpointRightX = 390f;

	public Level unlockLevel;

	public float speed;

	public float rotation;

	public bool bird;

	public string runRightAnimName;

	public string runLeftAnimName;

	public bool dontAppear;

	private scrChar character;

	private Transform charTrans;

	private bool hasLeftRunAnim => !runLeftAnimName.IsNullOrEmpty();

	private bool hasRightRunAnim => !runRightAnimName.IsNullOrEmpty();

	public float walkingDuration => 15f / speed;

	public bool eligibleToWalk
	{
		get
		{
			if (!dontAppear)
			{
				if (unlockLevel != Level.None)
				{
					return Persistence.GetLevelClearedByAnySlot(unlockLevel);
				}
				return true;
			}
			return false;
		}
	}

	private void Awake()
	{
		character = GetComponent<scrChar>();
		character.Setup(character.character);
		charTrans = base.transform;
	}

	public void StartRunning(TweenCallback onComplete)
	{
		bool flag = Random.Range(0, 2) == 1;
		float x = 1f;
		if (hasRightRunAnim)
		{
			if (hasLeftRunAnim)
			{
				character.PlayExpression(flag ? runRightAnimName : runLeftAnimName, disableOverride: true);
			}
			else
			{
				character.PlayExpression(runRightAnimName, disableOverride: true);
				x = (flag ? 1 : (-1));
			}
		}
		else
		{
			character.PlayExpression(runLeftAnimName, disableOverride: true);
			x = ((!flag) ? 1 : (-1));
		}
		charTrans.ScaleX(x);
		charTrans.MoveX(flag ? (-30f) : 390f);
		charTrans.LocalMoveY(Random.Range(-10, 3) + (bird ? 65 : 0));
		charTrans.Rotate(Vector3.zero);
		charTrans.DOMoveX(flag ? 390f : (-30f), walkingDuration).SetEase(Ease.Linear).OnComplete(onComplete);
		charTrans.DORotate(new Vector3(0f, 0f, (float)((!flag) ? 1 : (-1)) * rotation), walkingDuration, RotateMode.LocalAxisAdd).SetEase(Ease.Linear);
	}
}
