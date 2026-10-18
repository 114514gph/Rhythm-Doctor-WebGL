using UnityEngine;

public abstract class RDInputType
{
	public int schemeIndex;

	protected bool gameplay
	{
		get
		{
			if ((!(scnGame.instance != null) || scnGame.instance.paused) && !(scnCalibration.instance != null))
			{
				return scnRhythmWeightlifter.gameInstance != null;
			}
			return true;
		}
	}

	protected bool gameplayCancel
	{
		get
		{
			if (!(scnGame.instance != null) || scnGame.instance.paused)
			{
				return scnCalibration.instance != null;
			}
			return true;
		}
	}

	protected virtual Vector2 Axis => Vector2.zero;

	public RDInputType(int schemeIndex)
	{
		this.schemeIndex = schemeIndex;
	}

	public abstract bool Main(RDButtonState state);

	public abstract bool Restart();

	public abstract bool Skip();

	public abstract bool Cancel(RDButtonState state);

	public abstract bool Quit();

	public abstract bool Left(RDButtonState state);

	public abstract bool Right(RDButtonState state);

	public abstract bool Up(RDButtonState state);

	public abstract bool Down(RDButtonState state);

	public abstract bool FinerControl(RDButtonState state);

	public abstract bool FaceLeft(RDButtonState state);

	public abstract bool FaceUp(RDButtonState state);

	public abstract bool Select(RDButtonState state);

	public virtual void Update()
	{
	}

	public bool Get(RDInputAction action, RDButtonState state = RDButtonState.WentDown)
	{
		return action switch
		{
			RDInputAction.Main => Main(state), 
			RDInputAction.Cancel => Cancel(state), 
			RDInputAction.Quit => Quit(), 
			RDInputAction.Skip => Skip(), 
			RDInputAction.Restart => Restart(), 
			RDInputAction.Left => Left(state), 
			RDInputAction.Right => Right(state), 
			RDInputAction.Up => Up(state), 
			RDInputAction.Down => Down(state), 
			RDInputAction.FinerControl => FinerControl(state), 
			RDInputAction.FaceLeft => FaceLeft(state), 
			RDInputAction.FaceUp => FaceUp(state), 
			RDInputAction.Select => Select(state), 
			_ => false, 
		};
	}

	public virtual Vector2 GetPosition()
	{
		if (Axis != Vector2.zero)
		{
			return Vector2.ClampMagnitude(Axis, 1f);
		}
		Vector2 zero = Vector2.zero;
		if (Left(RDButtonState.IsDown))
		{
			zero += Vector2.left;
		}
		if (Right(RDButtonState.IsDown))
		{
			zero += Vector2.right;
		}
		if (Up(RDButtonState.IsDown))
		{
			zero += Vector2.up;
		}
		if (Down(RDButtonState.IsDown))
		{
			zero += Vector2.down;
		}
		return Vector2.ClampMagnitude(zero, 1f);
	}
}
