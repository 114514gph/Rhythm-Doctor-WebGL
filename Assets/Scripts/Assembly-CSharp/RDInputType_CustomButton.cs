using UnityEngine;

public class RDInputType_CustomButton : RDInputType_Keyboard
{
	private const string CustomButtonNameP1 = "P1";

	private const string CustomButtonNameP2 = "P2";

	public float p1DownTimestamp;

	public float p2DownTimestamp;

	private string currentButtonName
	{
		get
		{
			if (schemeIndex != 0)
			{
				return "P2";
			}
			return "P1";
		}
	}

	public RDInputType_CustomButton(int schemeIndex)
		: base(schemeIndex)
	{
	}

	public override bool Main(RDButtonState state)
	{
		return state switch
		{
			RDButtonState.IsDown => Input.GetButton(currentButtonName), 
			RDButtonState.WentDown => Input.GetButtonDown(currentButtonName), 
			RDButtonState.WentUp => Input.GetButtonUp(currentButtonName), 
			_ => false, 
		};
	}

	public override bool Quit()
	{
		return base.Quit();
	}
}
