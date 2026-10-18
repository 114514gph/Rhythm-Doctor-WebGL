public class RDAbandonedWard : RDEnvironment
{
	public tk2dSprite[] voidItems;

	public tk2dSprite light;

	public tk2dSprite foreground;

	public tk2dSpriteAnimator elevator;

	public tk2dSpriteAnimator door;

	public void SetElevatorOpened(bool open, float speed = 1f)
	{
		elevator.Play("AbandonedWard_Elevator_" + (open ? "Open" : "Close"), speed, shouldreset: false);
	}

	public void SetDoorOpened(bool open, float speed = 1f)
	{
		door.Play("AbandonedWard_Door_" + (open ? "Open" : "Close"), speed, shouldreset: false);
	}
}
