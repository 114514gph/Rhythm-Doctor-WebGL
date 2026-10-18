using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomAnimation : RDBase
{
	public enum RenderMode
	{
		MeshRenderer = 0,
		RawImage = 1
	}

	public TextAsset jsonData;

	public RenderMode renderMode;

	public CustomAnimationData data;

	public CustomAnimationClip currentClip;

	[NonSerialized]
	public float time;

	[NonSerialized]
	public int clipFrame;

	[NonSerialized]
	public bool ignoreTimescale;

	[NonSerialized]
	public float pivotX = 0.5f;

	[NonSerialized]
	public float pivotY = 0.5f;

	private RawImage rawImage;

	private MeshFilter filter;

	private MeshRenderer renderer;

	private Vector2 uvSize;

	private float fps;

	private bool paused;

	private int lastClipFrame = int.MaxValue;

	private CustomAnimationClip lastClip;

	public bool isNewData;

	private bool setup;

	public Action<CustomAnimation, CustomAnimationClip> onAnimationCompleted;

	public Action<CustomAnimation, CustomAnimationClip, int> onClipFrameUpdate;

	public const float DefaultFramesPerBeat = 4f;

	public bool isLoopOnBeat => currentClip.type == AnimationType.LoopOnBeat;

	private void Awake()
	{
		Setup();
	}

	private void Setup()
	{
		if (!setup)
		{
			setup = true;
			if (renderMode == RenderMode.MeshRenderer)
			{
				filter = GetComponent<MeshFilter>();
				renderer = GetComponent<MeshRenderer>();
			}
			else
			{
				rawImage = GetComponent<RawImage>();
				rawImage.material = new Material(rawImage.material);
			}
		}
	}

	private void LateUpdate()
	{
		if (!data.mainTexture || data.mainTexture.width == 0 || data.mainTexture.height == 0)
		{
			RDBaseDllDummy.printesw($"{base.name}, {data.mainTexture == null}");
			if (data.mainTexture != null)
			{
				RDBaseDllDummy.printesw($"width: {data.mainTexture.width} || height: {data.mainTexture.height}");
			}
			return;
		}
		UpdateTexture();
		if (currentClip == null)
		{
			return;
		}
		if ((Time.timeScale > 0f || ignoreTimescale) && !paused)
		{
			time += Time.unscaledDeltaTime;
		}
		clipFrame = Mathf.FloorToInt(fps * time);
		if (currentClip.type == AnimationType.Once || currentClip.type == AnimationType.LoopOnBeat)
		{
			if (clipFrame >= currentClip.frames.Length && lastClipFrame < currentClip.frames.Length && onAnimationCompleted != null)
			{
				onAnimationCompleted(this, currentClip);
			}
		}
		else if (currentClip.type == AnimationType.Loop)
		{
			float num = (float)currentClip.frames.Length * 1f / fps;
			if (time > num)
			{
				int num2 = currentClip.frames.Length - currentClip.loopStart;
				clipFrame = (clipFrame - currentClip.frames.Length) % num2 + currentClip.loopStart;
			}
		}
		if (clipFrame == lastClipFrame && currentClip == lastClip && !isNewData)
		{
			return;
		}
		if (onClipFrameUpdate != null)
		{
			for (int i = lastClipFrame; i <= Mathf.Min(clipFrame, currentClip.frames.Length - 1); i++)
			{
				onClipFrameUpdate(this, currentClip, i);
			}
		}
		lastClipFrame = clipFrame;
		lastClip = currentClip;
		isNewData = false;
		UpdateMesh();
	}

	private void UpdateTexture()
	{
		Material obj = ((renderMode == RenderMode.MeshRenderer) ? renderer.material : rawImage.material);
		if (renderMode == RenderMode.RawImage)
		{
			rawImage.texture = data.mainTexture;
		}
		obj.mainTexture = data.mainTexture;
		obj.SetTexture(RDShaderProperties.GlowTexID, data.glowTexture);
		obj.SetTexture(RDShaderProperties.OutlineTexID, data.outlineTexture);
	}

	public void UpdateMesh()
	{
		if (currentClip != null && clipFrame >= 0)
		{
			UpdateTexture();
			Material material = ((renderMode == RenderMode.MeshRenderer) ? renderer.material : rawImage.material);
			Texture mainTexture = material.mainTexture;
			int num = Mathf.Min(clipFrame, currentClip.frames.Length - 1);
			int sheetFrame = currentClip.frames[num];
			List<Vector2> uVsForSheetFrame = data.GetUVsForSheetFrame(sheetFrame);
			if (renderMode == RenderMode.MeshRenderer)
			{
				Mesh mesh = filter.mesh;
				Vector3 vector = new Vector3(currentClip.pivotOffset.x * 1f / (float)data.spriteSize.x, currentClip.pivotOffset.y * 1f / (float)data.spriteSize.y, 0f);
				vector += new Vector3(pivotX, pivotY, 0f);
				Vector3 b = new Vector3(data.spriteSize.x, data.spriteSize.y, 1f);
				mesh.vertices = new Vector3[4]
				{
					Vector3.Scale(new Vector3(0f, 0f, 0f) - vector, b),
					Vector3.Scale(new Vector3(1f, 0f, 0f) - vector, b),
					Vector3.Scale(new Vector3(0f, 1f, 0f) - vector, b),
					Vector3.Scale(new Vector3(1f, 1f, 0f) - vector, b)
				};
				mesh.RecalculateBounds();
				int[] indices = new int[6] { 0, 3, 1, 0, 2, 3 };
				mesh.SetIndices(indices, MeshTopology.Triangles, 0);
				mesh.colors = new Color[4]
				{
					Color.white,
					Color.white,
					Color.white,
					Color.white
				};
				mesh.SetUVs(0, uVsForSheetFrame);
				material.SetVector(RDShaderProperties.UVBoundsID, new Vector4(uVsForSheetFrame[0].x, uVsForSheetFrame[0].y, uVsForSheetFrame[3].x, uVsForSheetFrame[3].y));
			}
			else
			{
				float num2 = currentClip.portraitOffset.x * 1f / (float)mainTexture.width;
				float num3 = currentClip.portraitOffset.y * 1f / (float)mainTexture.height;
				float width = (float)currentClip.portraitSize.x * 1f / (float)mainTexture.width;
				float height = (float)currentClip.portraitSize.y * 1f / (float)mainTexture.height;
				rawImage.uvRect = new Rect(uVsForSheetFrame[0].x + num2, uVsForSheetFrame[0].y + num3, width, height);
			}
		}
	}

	public void Play(string animationName, float timeStart = 0f, float fps = -1f)
	{
		if (!data.clips.TryGetValue(animationName, out var value))
		{
			Debug.LogError("Animation name not found: " + animationName);
			return;
		}
		Setup();
		currentClip = value;
		this.fps = ((fps == -1f) ? currentClip.fps : fps);
		this.fps = Mathf.Max(0f, this.fps);
		time = timeStart;
		clipFrame = (clipFrame = Mathf.FloorToInt(fps * time));
		lastClipFrame = clipFrame;
		UpdateMesh();
		if (onClipFrameUpdate != null)
		{
			onClipFrameUpdate(this, currentClip, 0);
		}
	}

	public void PlayFromClip(CustomAnimationClip clip, float speedMultiplier = 1f, float customCrotchet = 0f, float timeStart = 0f)
	{
		bool flag = clip.type == AnimationType.LoopOnBeat;
		bool num = flag || clip.fps <= 0f;
		float num2 = ((clip.fps <= 0f) ? 4f : clip.fps);
		float num3 = ((customCrotchet > 0f) ? customCrotchet : (base.conductor ? base.conductor.visualCrotchet : 0.6f));
		float num4 = (num ? (num2 / num3) : (num2 * speedMultiplier));
		timeStart += (flag ? ((float)clip.loopStart * 1f / num4) : 0f);
		Play(clip.name, timeStart, num4);
	}

	public void AttemptLoopOnBeat(float customCrotchet = 0f)
	{
		if (clipFrame >= currentClip.frames.Length - 1)
		{
			PlayFromClip(currentClip, 1f, customCrotchet);
		}
	}

	public void Pause()
	{
		paused = true;
	}

	public void Resume()
	{
		paused = false;
	}
}
