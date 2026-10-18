using UnityEngine;

public class VirtualTouchControls : MonoBehaviour
{
	private static int StampFrame
	{
		get
		{
			if (RDInput.lastUpdateFrame != Time.frameCount)
			{
				return Time.frameCount;
			}
			return Time.frameCount + 1;
		}
	}

	public void Left_WentDown()
	{
		RDInputType_Touch.virtualButtonLeftFrame_WentDown = StampFrame;
	}

	public void Left_WentUp()
	{
		RDInputType_Touch.virtualButtonLeftFrame_WentUp = StampFrame;
	}

	public void Right_WentDown()
	{
		RDInputType_Touch.virtualButtonRightFrame_WentDown = StampFrame;
	}

	public void Right_WentUp()
	{
		RDInputType_Touch.virtualButtonRightFrame_WentUp = StampFrame;
	}

	public void Up_WentDown()
	{
		RDInputType_Touch.virtualButtonUpFrame_WentDown = StampFrame;
	}

	public void Up_WentUp()
	{
		RDInputType_Touch.virtualButtonUpFrame_WentUp = StampFrame;
	}

	public void Down_WentDown()
	{
		RDInputType_Touch.virtualButtonDownFrame_WentDown = StampFrame;
	}

	public void Down_WentUp()
	{
		RDInputType_Touch.virtualButtonDownFrame_WentUp = StampFrame;
	}

	public void Cancel_WentDown()
	{
		RDInputType_Touch.virtualButtonCancelFrame_WentDown = StampFrame;
	}

	public void Cancel_WentUp()
	{
		RDInputType_Touch.virtualButtonCancelFrame_WentUp = StampFrame;
	}

	public void FaceLeft_WentDown()
	{
		RDInputType_Touch.virtualButtonFaceLeftFrame_WentDown = StampFrame;
	}

	public void FaceLeft_WentUp()
	{
		RDInputType_Touch.virtualButtonFaceLeftFrame_WentUp = StampFrame;
	}

	public void FaceUp_WentDown()
	{
		RDInputType_Touch.virtualButtonFaceUpFrame_WentDown = StampFrame;
	}

	public void FaceUp_WentUp()
	{
		RDInputType_Touch.virtualButtonFaceUpFrame_WentUp = StampFrame;
	}

	public void Select_WentDown()
	{
		RDInputType_Touch.virtualButtonSelectFrame_WentDown = StampFrame;
	}

	public void Select_WentUp()
	{
		RDInputType_Touch.virtualButtonSelectFrame_WentUp = StampFrame;
	}
}
