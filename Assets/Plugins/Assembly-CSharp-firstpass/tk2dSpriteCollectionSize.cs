using System;

[Serializable]
public class tk2dSpriteCollectionSize
{
	public enum Type
	{
		Explicit = 0,
		PixelsPerMeter = 1
	}

	public Type type = Type.PixelsPerMeter;

	public float orthoSize = 10f;

	public float pixelsPerMeter = 100f;

	public float width = 960f;

	public float height = 640f;

	public float OrthoSize => type switch
	{
		Type.Explicit => orthoSize, 
		Type.PixelsPerMeter => 0.5f, 
		_ => orthoSize, 
	};

	public float TargetHeight => type switch
	{
		Type.Explicit => height, 
		Type.PixelsPerMeter => pixelsPerMeter, 
		_ => height, 
	};

	public static tk2dSpriteCollectionSize Explicit(float orthoSize, float targetHeight)
	{
		return ForResolution(orthoSize, targetHeight, targetHeight);
	}

	public static tk2dSpriteCollectionSize PixelsPerMeter(float pixelsPerMeter)
	{
		return new tk2dSpriteCollectionSize
		{
			type = Type.PixelsPerMeter,
			pixelsPerMeter = pixelsPerMeter
		};
	}

	public static tk2dSpriteCollectionSize ForResolution(float orthoSize, float width, float height)
	{
		return new tk2dSpriteCollectionSize
		{
			type = Type.Explicit,
			orthoSize = orthoSize,
			width = width,
			height = height
		};
	}

	public static tk2dSpriteCollectionSize ForTk2dCamera()
	{
		return new tk2dSpriteCollectionSize
		{
			type = Type.PixelsPerMeter,
			pixelsPerMeter = 1f
		};
	}

	public static tk2dSpriteCollectionSize Default()
	{
		return PixelsPerMeter(100f);
	}

	public void CopyFromLegacy(bool useTk2dCamera, float orthoSize, float targetHeight)
	{
		if (useTk2dCamera)
		{
			type = Type.PixelsPerMeter;
			pixelsPerMeter = 1f;
		}
		else
		{
			type = Type.Explicit;
			height = targetHeight;
			this.orthoSize = orthoSize;
		}
	}

	public void CopyFrom(tk2dSpriteCollectionSize source)
	{
		type = source.type;
		width = source.width;
		height = source.height;
		orthoSize = source.orthoSize;
		pixelsPerMeter = source.pixelsPerMeter;
	}
}
