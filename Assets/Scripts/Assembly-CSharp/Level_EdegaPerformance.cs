using System;
using DG.Tweening;
using RDLevelEditor;

public class Level_EdegaPerformance : LevelBase
{
	public Level_EdegaPerformance(RDLevelData data)
		: base(data)
	{
	}

	public RDArm GetArm(int room, bool leftArm)
	{
		scrHandController handControllerFromInt = base.game.GetHandControllerFromInt(room);
		if (!leftArm)
		{
			return handControllerFromInt.rightArm;
		}
		return handControllerFromInt.leftArm;
	}

	public void ShowButton(int room, bool leftArm, bool show)
	{
		RDArm arm = GetArm(room, leftArm);
		arm.button.gameObject.SetActive(show);
		arm.moveButtonWithArm = show;
	}

	public void MoveArmX(int room, bool leftArm, float x, float duration, string easeString)
	{
		Ease ease = Enum.Parse<Ease>(easeString);
		RDArm arm = GetArm(room, leftArm);
		arm.SetVisible(visible: true, force: true);
		arm.transform.DOKill();
		arm.transform.DOLocalMoveX(x, duration).SetEase(ease).Done();
	}

	public void MoveArmY(int room, bool leftArm, float y, float duration, string easeString)
	{
		Ease ease = Enum.Parse<Ease>(easeString);
		RDArm arm = GetArm(room, leftArm);
		arm.SetVisible(visible: true, force: true);
		arm.transform.DOKill();
		arm.transform.DOLocalMoveY(y, duration).SetEase(ease).Done();
	}

	public void MoveButtonX(int room, bool leftArm, int x, float duration, string easeString)
	{
		Ease ease = Enum.Parse<Ease>(easeString);
		RDArm arm = GetArm(room, leftArm);
		arm.SetVisible(visible: true, force: true);
		arm.button.transform.DOKill();
		arm.button.transform.DOLocalMoveX(x, duration).SetEase(ease).Done();
	}

	public void MoveButtonY(int room, bool leftArm, int y, float duration, string easeString)
	{
		Ease ease = Enum.Parse<Ease>(easeString);
		RDArm arm = GetArm(room, leftArm);
		arm.SetVisible(visible: true, force: true);
		arm.button.transform.DOKill();
		arm.button.transform.DOLocalMoveY(y, duration).SetEase(ease).Done();
	}

	public void PlayArmAnim(int room, bool leftArm, string anim)
	{
		GetArm(room, leftArm).hand.PlayAnim(anim);
	}

	public void ToggleNervous(int room, bool leftArm, bool on)
	{
		GetArm(room, leftArm).ToggleNervous(on);
	}

	public void FlipDialog(bool flip)
	{
		base.game.ink.SetPortraitSide(flip ? RDInk.PortraitSide.Right : RDInk.PortraitSide.Left);
	}
}
