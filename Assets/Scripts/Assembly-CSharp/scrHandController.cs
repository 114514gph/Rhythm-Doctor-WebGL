using System;
using DG.Tweening;
using UnityEngine;

public class scrHandController : RDBase
{
	public void DOKill(bool complete = false) { }

	public const int OnTopRoomIndex = 4;

	public const float DefaultVisibleY = 50f;

	public const float HiddenY = 0f;

	public const float MoveTime = 0.75f;

	public const float MoveCrotchets = 4f;

	public const float ChangeYDuration = 0.25f;

	public RDArm leftArm;

	public RDArm rightArm;

	public int room;

	[NonSerialized]
	public bool startingAnimation = true;

	[NonSerialized]
	public bool slidingIn;

	[NonSerialized]
	public bool shouldShow = true;

	[NonSerialized]
	public float currentVisibleY;

	[NonSerialized]
	public bool rightArmIsCPU;

	[NonSerialized]
	public scrExecuteOnUpdate alignWithRoomComp;

	[NonSerialized]
	public HandAction lastHandAction = HandAction.Lower;

	private Tween settleDelayedCall;

	public bool moveButtonsWithArms
	{
		set
		{
			leftArm.moveButtonWithArm = value;
			rightArm.moveButtonWithArm = value;
		}
	}

	private void Awake()
	{
		rightArm.SetSkin(Persistence.p1Skin);
		if (GC.twoPlayerMode)
		{
			leftArm.SetSkin(Persistence.p2Skin);
		}
		currentVisibleY = 50f;
		if (room >= 0 && room < 4)
		{
			base.transform.localPosition = base.transform.localPosition.WithX(3000f * (float)room);
		}
		SetHandsInFront(room == 4);
	}

	public void HideInstant()
	{
		if (base.gameObject.activeSelf)
		{
			this.DOKill();
			leftArm.HideInstant();
			rightArm.HideInstant();
		}
	}

	public void SlideIn(float moveTimeMultiplier = 1f)
	{
		if (base.gameObject.activeSelf)
		{
			HandUpdateType handType = (GC.twoPlayerMode ? HandUpdateType.BothHands : HandUpdateType.RightHand);
			UpdateHandsPositions(handType, moveTimeMultiplier);
		}
	}

	public void ShowHandButtons()
	{
		if (base.gameObject.activeSelf)
		{
			leftArm.button.gameObject.SetActive(GC.twoPlayerMode);
			rightArm.button.gameObject.SetActive(value: true);
			leftArm.ShowHandButtons(GC.twoPlayerMode ? leftArm.twoPlayerX : leftArm.hiddenX);
			rightArm.ShowHandButtons(GC.twoPlayerMode ? rightArm.twoPlayerX : rightArm.onePlayerX);
		}
	}

	public void UpdateHandsPositions(HandUpdateType handType, float moveTimeMultiplier = 1f, bool shakeOnSettle = true)
	{
		float num = 0.75f * moveTimeMultiplier;
		switch (handType)
		{
		case HandUpdateType.BothHands:
			leftArm.Toggle(HandAction.Show, num, togglingBothHands: true);
			rightArm.Toggle(HandAction.Show, num, togglingBothHands: true);
			break;
		case HandUpdateType.LeftHand:
			leftArm.Toggle(HandAction.Show, num);
			rightArm.Toggle(HandAction.Hide, num);
			break;
		case HandUpdateType.LeftHandShort:
			leftArm.Toggle(HandAction.Show, num, togglingBothHands: true);
			rightArm.Toggle(HandAction.Hide, num);
			break;
		case HandUpdateType.RightHand:
			leftArm.Toggle(HandAction.Hide, num);
			rightArm.Toggle(HandAction.Show, num);
			break;
		default:
			leftArm.Toggle(HandAction.Hide, num);
			rightArm.Toggle(HandAction.Show, num, togglingBothHands: true);
			break;
		}
		slidingIn = true;
		if (settleDelayedCall != null)
		{
			settleDelayedCall.Kill();
		}
		settleDelayedCall = DOVirtual.DelayedCall(Math.Max(num - 0.38f, 0f), () =>
		{
			Settle(shakeOnSettle);
		}, ignoreTimeScale: false);
	}

	private void OnDisable()
	{
	}

	public void Show(bool show, bool instant = false, bool deactivateAfterHiding = false, bool updateHandsAfterHiding = false, bool fromCutscene = false)
	{
		if (!base.gameObject.activeSelf || (show && !shouldShow))
		{
			return;
		}
		float num = (instant ? 0f : 0.25f);
		if (show)
		{
			if (base.transform.position.y != currentVisibleY)
			{
				leftArm.Toggle(HandAction.Raise, num);
				rightArm.Toggle(HandAction.Raise, num);
				lastHandAction = HandAction.Raise;
			}
		}
		else
		{
			if (base.transform.position.y == 0f)
			{
				return;
			}
			leftArm.Toggle(HandAction.Lower, num);
			rightArm.Toggle(HandAction.Lower, num);
			if (!fromCutscene)
			{
				lastHandAction = HandAction.Lower;
			}
			DOVirtual.DelayedCall(num, () =>
			{
				leftArm.Depress();
				rightArm.Depress();
				if (deactivateAfterHiding)
				{
					base.gameObject.SetActive(value: false);
				}
				else if (updateHandsAfterHiding)
				{
					OnTopHandForceUpdate();
				}
			}).Done();
		}
	}

	public void cpuHandPress(Character cpuRowOwner = Character.Otto)
	{
		if (base.gameObject.activeSelf)
		{
			TryPressArm(leftArm);
			TryPressArm(rightArm);
		}
		void TryPressArm(RDArm arm)
		{
			if (arm.cpuCanUse && arm.cpuOwner != Character.Otto && arm.cpuOwner == cpuRowOwner)
			{
				float delay = 0.1f;
				if (arm.cpuOwner == Character.Paige)
				{
					delay = 0.3f;
				}
				arm.Press();
				DOVirtual.DelayedCall(delay, () =>
				{
					arm.Depress();
				});
			}
		}
	}

	public void OnTopHandForceUpdate()
	{
		startingAnimation = false;
		base.transform.localPosition = base.transform.localPosition.WithZ(-80f);
		for (int i = 0; i <= room; i++)
		{
			scrHandController handControllerFromInt = base.game.GetHandControllerFromInt(i);
			handControllerFromInt.leftArm.ToggleLast();
			handControllerFromInt.rightArm.ToggleLast();
		}
	}

	public void UnsetLastArmActions()
	{
		leftArm.UnsetLast();
		rightArm.UnsetLast();
	}

	public void cpuHandHoldDown(Character cpuRowOwner = Character.Otto)
	{
		if (base.gameObject.activeSelf)
		{
			TryPressArm(leftArm);
			TryPressArm(rightArm);
		}
		void TryPressArm(RDArm arm)
		{
			if (arm.cpuCanUse && arm.cpuOwner != Character.Otto && arm.cpuOwner == cpuRowOwner)
			{
				arm.Press();
			}
		}
	}

	public void cpuHandRelease(Character cpuRowOwner = Character.Otto)
	{
		TryDepressArm(leftArm);
		TryDepressArm(rightArm);
		void TryDepressArm(RDArm arm)
		{
			if (arm.cpuCanUse && arm.cpuOwner != Character.Otto && arm.cpuOwner == cpuRowOwner)
			{
				arm.Depress();
			}
		}
	}

	public void SetHandsInFront(bool inFront, bool beforeGameStart = false)
	{
		leftArm.SetHandsInFront(inFront, beforeGameStart);
		rightArm.SetHandsInFront(inFront, beforeGameStart);
	}

	private void Settle(bool shake)
	{
		if (base.gameObject.activeSelf)
		{
			slidingIn = false;
			if (shake && base.gm.hudCamera != null && base.gameObject.activeSelf)
			{
				base.gm.hudCamera.Shake(10, 2);
			}
		}
	}

	public void ToggleAlignWithRoomComp(bool enableAlign)
	{
		if (room == 4)
		{
			return;
		}
		if (alignWithRoomComp == null)
		{
			Material quadMaterial = base.game.rooms[room].renderQuad.GetComponent<Renderer>().material;
			RDCamera cam = RDBase.Vfx.GetCamera(room);
			CameraFilterPack_FlipScreen flipComp = cam.GetOrAddComponent<CameraFilterPack_FlipScreen>();
			alignWithRoomComp = scrExecuteOnUpdate.Add(() =>
			{
				float num = base.game.rooms[room].renderQuadPivot.localScale.y * (1f / cam.zoom);
				float num2 = quadMaterial.GetFloat("_PosY") * RDBase.Vfx.RDHeight * (float)((!flipComp.flipY) ? 1 : (-1));
				float num3 = 99f * (1f - num) + num2;
				base.transform.localPosition = base.transform.localPosition.WithY(-50f + num3);
			}, lateUpdate: true);
		}
		alignWithRoomComp.enabled = enableAlign;
		if (!enableAlign)
		{
			base.transform.localPosition = base.transform.localPosition.WithY(-50f);
		}
	}

	public bool DoesEnumTargetLeftHand(Hand handType)
	{
		switch (handType)
		{
		case Hand.Both:
			return true;
		case Hand.Left:
			return true;
		case Hand.Right:
			return false;
		case Hand.p1:
			if (GC.twoPlayerMode)
			{
				return base.game.currentLevel.arePlayerInputsSwapped;
			}
			return false;
		case Hand.p2:
			if (GC.twoPlayerMode)
			{
				return !base.game.currentLevel.arePlayerInputsSwapped;
			}
			return true;
		default:
			return false;
		}
	}

	public bool DoesEnumTargetRightHand(Hand handType)
	{
		if (handType == Hand.Both)
		{
			return true;
		}
		return !DoesEnumTargetLeftHand(handType);
	}
}
