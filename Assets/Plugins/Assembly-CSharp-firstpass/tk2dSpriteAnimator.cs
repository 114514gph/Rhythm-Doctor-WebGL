using System;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("2D Toolkit/Sprite/tk2dSpriteAnimator")]
public class tk2dSpriteAnimator : MonoBehaviour
{
	public enum DeltaTimeStyle
	{
		Normal = 0,
		AlwaysOneExceptWhenMusicPaused = 1,
		AlwaysOneAlways = 2,
		PreventFasterThanOne = 3
	}

	private enum State
	{
		Init = 0,
		Playing = 1,
		Paused = 2
	}

	[SerializeField]
	private tk2dSpriteAnimation library;

	[SerializeField]
	private int defaultClipId;

	[SerializeField]
	private string defaultClipName = string.Empty;

	public bool playAutomatically;

	private static State globalState;

	public DeltaTimeStyle deltaTimeStyle;

	public int currentStaticFrame;

	[NonSerialized]
	public tk2dSpriteAnimationClip currentClip;

	private float clipTime;

	private float clipFps = -1f;

	private int previousFrame = -1;

	public Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip> AnimationCompleted;

	public Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int> AnimationEventTriggered;

	private State state;

	protected tk2dBaseSprite _sprite;

	protected RawImage _rawImage;

	private string lastClip;

	private int lastFrame;

	public static bool g_Paused
	{
		get
		{
			return (globalState & State.Paused) != 0;
		}
		set
		{
			globalState = (value ? State.Paused : State.Init);
		}
	}

	public bool Paused
	{
		get
		{
			return (state & State.Paused) != 0;
		}
		set
		{
			if (value)
			{
				state |= State.Paused;
			}
			else
			{
				state &= (State)(-3);
			}
		}
	}

	public bool isLoopOnBeat
	{
		get
		{
			if (CurrentClip != null)
			{
				return CurrentClip.loopsOnBeat;
			}
			return false;
		}
	}

	public tk2dSpriteAnimation Library
	{
		get
		{
			return library;
		}
		set
		{
			library = value;
		}
	}

	public int DefaultClipId
	{
		get
		{
			return defaultClipId;
		}
		set
		{
			defaultClipId = value;
		}
	}

	public string DefaultClipName
	{
		get
		{
			return defaultClipName;
		}
		set
		{
			defaultClipName = value;
		}
	}

	public tk2dSpriteAnimationClip DefaultClip => GetClipById(defaultClipId);

	public virtual tk2dBaseSprite Sprite
	{
		get
		{
			if (_sprite == null)
			{
				_sprite = GetComponent<tk2dBaseSprite>();
			}
			return _sprite;
		}
	}

	public virtual RawImage RawImage
	{
		get
		{
			if (_rawImage == null)
			{
				_rawImage = GetComponent<RawImage>();
			}
			return _rawImage;
		}
	}

	public bool Playing => (state & State.Playing) != 0;

	public tk2dSpriteAnimationClip CurrentClip => currentClip;

	public float ClipTimeSeconds
	{
		get
		{
			if (!(clipFps > 0f))
			{
				return clipTime / currentClip.fps;
			}
			return clipTime / clipFps;
		}
	}

	public float ClipFps
	{
		get
		{
			return clipFps;
		}
		set
		{
			if (currentClip != null)
			{
				clipFps = ((value > 0f) ? value : currentClip.fps);
			}
		}
	}

	public static float DefaultFps => 0f;

	public int CurrentFrame
	{
		get
		{
			switch (currentClip.wrapMode)
			{
			case tk2dSpriteAnimationClip.WrapMode.Once:
				return Mathf.Min((int)clipTime, currentClip.frames.Length);
			case tk2dSpriteAnimationClip.WrapMode.Loop:
			case tk2dSpriteAnimationClip.WrapMode.RandomLoop:
				return (int)clipTime % currentClip.frames.Length;
			case tk2dSpriteAnimationClip.WrapMode.LoopSection:
			{
				int num2 = (int)clipTime;
				int result = currentClip.loopStart + (num2 - currentClip.loopStart) % (currentClip.frames.Length - currentClip.loopStart);
				if (num2 >= currentClip.loopStart)
				{
					return result;
				}
				return num2;
			}
			case tk2dSpriteAnimationClip.WrapMode.PingPong:
			{
				int num = ((currentClip.frames.Length > 1) ? ((int)clipTime % (currentClip.frames.Length + currentClip.frames.Length - 2)) : 0);
				if (num >= currentClip.frames.Length)
				{
					num = 2 * currentClip.frames.Length - 2 - num;
				}
				return num;
			}
			case tk2dSpriteAnimationClip.WrapMode.Single:
				return 0;
			default:
				Debug.LogError("Unhandled clip wrap mode");
				goto case tk2dSpriteAnimationClip.WrapMode.Loop;
			}
		}
	}

	public bool IsCurrentClip(string clipname)
	{
		tk2dSpriteAnimationClip clipByName = GetClipByName(clipname);
		return currentClip == clipByName;
	}

	private void OnEnable()
	{
		if (RawImage == null && Sprite == null)
		{
			base.enabled = false;
			MonoBehaviour.print("RawImage or Sprite not found.");
		}
	}

	private void Awake()
	{
		if (!string.IsNullOrEmpty(DefaultClipName))
		{
			DefaultClipId = GetClipIdByName(DefaultClipName);
		}
	}

	private void Start()
	{
		if (playAutomatically)
		{
			Play(DefaultClip);
		}
	}

	public static tk2dSpriteAnimator AddComponent(GameObject go, tk2dSpriteAnimation anim, int clipId)
	{
		tk2dSpriteAnimationClip tk2dSpriteAnimationClip2 = anim.clips[clipId];
		tk2dSpriteAnimator obj = go.AddComponent<tk2dSpriteAnimator>();
		obj.Library = anim;
		obj.SetSprite(tk2dSpriteAnimationClip2.frames[0].spriteCollection, tk2dSpriteAnimationClip2.frames[0].spriteId);
		return obj;
	}

	private tk2dSpriteAnimationClip GetClipByNameVerbose(string name)
	{
		if (library == null)
		{
			Debug.LogError("Library not set");
			return null;
		}
		tk2dSpriteAnimationClip clipByName = library.GetClipByName(name);
		if (clipByName == null)
		{
			Debug.LogError("Unable to find clip '" + name + "' in library");
			return null;
		}
		return clipByName;
	}

	public void Play()
	{
		if (currentClip == null)
		{
			currentClip = DefaultClip;
		}
		Play(currentClip);
	}

	public void Play(tk2dSpriteAnimationClip clip)
	{
		Play(clip, 0f, DefaultFps);
	}

	public void Play(string clip, bool shouldreset = true)
	{
		Play(GetClipByNameVerbose(clip), 0f, DefaultFps, shouldreset);
	}

	public void Play(string clip, float speed, bool shouldreset = true)
	{
		tk2dSpriteAnimationClip clipByNameVerbose = GetClipByNameVerbose(clip);
		Play(clipByNameVerbose, 0f, clipByNameVerbose.fps * speed, shouldreset);
	}

	public void PlayFromFrame(int frame)
	{
		if (currentClip == null)
		{
			currentClip = DefaultClip;
		}
		PlayFromFrame(currentClip, frame);
	}

	public void PlayFromFrame(string name, int frame)
	{
		PlayFromFrame(GetClipByNameVerbose(name), frame);
	}

	public void PlayFromFrame(tk2dSpriteAnimationClip clip, int frame)
	{
		PlayFrom(clip, ((float)frame + 0.001f) / clip.fps);
	}

	public void PlayFrom(float clipStartTime)
	{
		if (currentClip == null)
		{
			currentClip = DefaultClip;
		}
		PlayFrom(currentClip, clipStartTime);
	}

	public void PlayFrom(string name, float clipStartTime)
	{
		tk2dSpriteAnimationClip tk2dSpriteAnimationClip2 = (library ? library.GetClipByName(name) : null);
		if (tk2dSpriteAnimationClip2 == null)
		{
			ClipNameError(name);
		}
		else
		{
			PlayFrom(tk2dSpriteAnimationClip2, clipStartTime);
		}
	}

	public void PlayFrom(tk2dSpriteAnimationClip clip, float clipStartTime)
	{
		Play(clip, clipStartTime, DefaultFps);
	}

	public void Play(tk2dSpriteAnimationClip clip, float clipStartTime, float overrideFps, bool forceReset = false)
	{
		if (clip == null)
		{
			Debug.LogError(base.name + ": Calling clip.Play() with a null clip");
			OnAnimationCompleted();
			state &= (State)(-2);
			return;
		}
		float num = ((overrideFps > 0f) ? overrideFps : clip.fps);
		if (clipStartTime == 0f && IsPlaying(clip) && !forceReset)
		{
			clipFps = num;
			return;
		}
		state |= State.Playing;
		currentClip = clip;
		clipFps = num;
		if (currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.Single || currentClip.frames == null)
		{
			WarpClipToLocalTime(currentClip, 0f);
			state &= (State)(-2);
			return;
		}
		if (currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.RandomFrame || currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.RandomLoop)
		{
			int num2 = UnityEngine.Random.Range(0, currentClip.frames.Length);
			WarpClipToLocalTime(currentClip, num2);
			if (currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.RandomFrame)
			{
				previousFrame = -1;
				state &= (State)(-2);
			}
			return;
		}
		float num3 = clipStartTime * clipFps;
		if (currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.Once && num3 >= clipFps * (float)currentClip.frames.Length)
		{
			WarpClipToLocalTime(currentClip, currentClip.frames.Length - 1);
			OnAnimationCompleted();
			state &= (State)(-2);
		}
		else
		{
			WarpClipToLocalTime(currentClip, num3);
			clipTime = num3;
		}
	}

	public void Stop()
	{
		state &= (State)(-2);
	}

	public void StopAndResetFrame()
	{
		if (currentClip != null)
		{
			SetSprite(currentClip.frames[0].spriteCollection, currentClip.frames[0].spriteId);
		}
		Stop();
	}

	public bool IsPlaying(string name)
	{
		if (Playing && CurrentClip != null)
		{
			return CurrentClip.name == name;
		}
		return false;
	}

	public bool IsPlaying(tk2dSpriteAnimationClip clip)
	{
		if (Playing && CurrentClip != null)
		{
			return CurrentClip == clip;
		}
		return false;
	}

	public tk2dSpriteAnimationClip GetClipById(int id)
	{
		if (library == null)
		{
			return null;
		}
		return library.GetClipById(id);
	}

	public int GetClipIdByName(string name)
	{
		if (!library)
		{
			return -1;
		}
		return library.GetClipIdByName(name);
	}

	public tk2dSpriteAnimationClip GetClipByName(string name, bool anyCase = false)
	{
		if (!library)
		{
			return null;
		}
		return library.GetClipByName(name, anyCase);
	}

	public void Pause()
	{
		state |= State.Paused;
	}

	public void Resume()
	{
		state &= (State)(-3);
	}

	public void SetFrame(int currFrame)
	{
		SetFrame(currFrame, triggerEvent: true);
	}

	public void SetFrame(int currFrame, bool triggerEvent)
	{
		if (currentClip == null)
		{
			currentClip = DefaultClip;
		}
		if (currentClip != null)
		{
			int num = currFrame % currentClip.frames.Length;
			SetFrameInternal(num);
			if (triggerEvent && currentClip.frames.Length != 0 && currFrame >= 0)
			{
				ProcessEvents(num - 1, num, 1);
			}
		}
	}

	public void UpdateAnimation(float deltaTime)
	{
		if ((state | globalState) != State.Playing)
		{
			return;
		}
		clipTime += deltaTime * clipFps;
		int num = previousFrame;
		switch (currentClip.wrapMode)
		{
		case tk2dSpriteAnimationClip.WrapMode.Loop:
		case tk2dSpriteAnimationClip.WrapMode.RandomLoop:
		{
			int num5 = (int)clipTime % currentClip.frames.Length;
			SetFrameInternal(num5);
			if (num5 < num)
			{
				ProcessEvents(num, currentClip.frames.Length - 1, 1);
				ProcessEvents(-1, num5, 1);
			}
			else
			{
				ProcessEvents(num, num5, 1);
			}
			break;
		}
		case tk2dSpriteAnimationClip.WrapMode.LoopSection:
		{
			int num3 = (int)clipTime;
			int num4 = currentClip.loopStart + (num3 - currentClip.loopStart) % (currentClip.frames.Length - currentClip.loopStart);
			if (num3 >= currentClip.loopStart)
			{
				SetFrameInternal(num4);
				num3 = num4;
				if (num < currentClip.loopStart)
				{
					ProcessEvents(num, currentClip.loopStart - 1, 1);
					ProcessEvents(currentClip.loopStart - 1, num3, 1);
				}
				else if (num3 < num)
				{
					ProcessEvents(num, currentClip.frames.Length - 1, 1);
					ProcessEvents(currentClip.loopStart - 1, num3, 1);
				}
				else
				{
					ProcessEvents(num, num3, 1);
				}
			}
			else
			{
				SetFrameInternal(num3);
				ProcessEvents(num, num3, 1);
			}
			break;
		}
		case tk2dSpriteAnimationClip.WrapMode.PingPong:
		{
			int num6 = ((currentClip.frames.Length > 1) ? ((int)clipTime % (currentClip.frames.Length + currentClip.frames.Length - 2)) : 0);
			int direction = 1;
			if (num6 >= currentClip.frames.Length)
			{
				num6 = 2 * currentClip.frames.Length - 2 - num6;
				direction = -1;
			}
			if (num6 < num)
			{
				direction = -1;
			}
			SetFrameInternal(num6);
			ProcessEvents(num, num6, direction);
			break;
		}
		case tk2dSpriteAnimationClip.WrapMode.Once:
		{
			int num2 = (int)clipTime;
			if (num2 >= currentClip.frames.Length)
			{
				SetFrameInternal(currentClip.frames.Length - 1);
				state &= (State)(-2);
				ProcessEvents(num, currentClip.frames.Length - 1, 1);
				OnAnimationCompleted();
			}
			else
			{
				SetFrameInternal(num2);
				ProcessEvents(num, num2, 1);
			}
			break;
		}
		case tk2dSpriteAnimationClip.WrapMode.RandomFrame:
			break;
		}
	}

	private void ClipNameError(string name)
	{
		Debug.LogError("Unable to find clip named '" + name + "' in library");
	}

	private void ClipIdError(int id)
	{
		Debug.LogError("Play - Invalid clip id '" + id + "' in library");
	}

	private void WarpClipToLocalTime(tk2dSpriteAnimationClip clip, float time)
	{
		clipTime = time;
		int num = (int)clipTime % clip.frames.Length;
		tk2dSpriteAnimationFrame tk2dSpriteAnimationFrame2 = clip.frames[num];
		SetSprite(tk2dSpriteAnimationFrame2.spriteCollection, tk2dSpriteAnimationFrame2.spriteId);
		if (tk2dSpriteAnimationFrame2.triggerEvent && AnimationEventTriggered != null)
		{
			AnimationEventTriggered(this, clip, num);
		}
		previousFrame = num;
	}

	private void SetFrameInternal(int currFrame)
	{
		if (previousFrame != currFrame)
		{
			SetSprite(currentClip.frames[currFrame].spriteCollection, currentClip.frames[currFrame].spriteId);
			previousFrame = currFrame;
		}
	}

	private void ProcessEvents(int start, int last, int direction)
	{
		if (AnimationEventTriggered == null || start == last || Mathf.Sign(last - start) != Mathf.Sign(direction))
		{
			return;
		}
		int num = last + direction;
		tk2dSpriteAnimationFrame[] frames = currentClip.frames;
		for (int i = start + direction; i != num; i += direction)
		{
			if (frames[i].triggerEvent && AnimationEventTriggered != null)
			{
				AnimationEventTriggered(this, currentClip, i);
			}
		}
	}

	private void OnAnimationCompleted()
	{
		previousFrame = -1;
		if (AnimationCompleted != null)
		{
			AnimationCompleted(this, currentClip);
		}
	}

	public virtual void LateUpdate()
	{
		float deltaTime = 1f;
		if (deltaTimeStyle == DeltaTimeStyle.AlwaysOneAlways)
		{
			deltaTime = RDTime.unscaledDeltaTime;
		}
		else if (deltaTimeStyle == DeltaTimeStyle.AlwaysOneExceptWhenMusicPaused)
		{
			deltaTime = RDTime.deltaTimeIndependentPauseAffected;
		}
		else if (deltaTimeStyle == DeltaTimeStyle.Normal)
		{
			deltaTime = Time.deltaTime;
		}
		else if (deltaTimeStyle == DeltaTimeStyle.PreventFasterThanOne)
		{
			deltaTime = RDTime.deltaTimeWithMaximumSpeed;
		}
		UpdateAnimation(deltaTime);
	}

	public virtual void SetSprite(tk2dSpriteCollectionData spriteCollection, int spriteId)
	{
		if (RawImage == null)
		{
			Sprite.SetSprite(spriteCollection, spriteId);
		}
		else
		{
			RawImage.SetTk2dSprite(spriteCollection, spriteId);
		}
	}
}
