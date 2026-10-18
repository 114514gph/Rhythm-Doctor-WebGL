using System.Linq;
using UnityEngine;

public class Entity : InvisibleEntity
{
	[Header("Entity")]
	private bool hasSetup;

	private tk2dSprite _sprite;

	private tk2dSpriteAnimator _animator;

	public bool updateChildRenderers;

	public Renderer[] childRenderersToIgnore;

	public Renderer[] nonChildRenderersToUpdate;

	public string animPrefixName = "";

	public bool isTintable;

	private bool _visible;

	public tk2dSprite sprite
	{
		get
		{
			if (!hasSetup)
			{
				Setup();
			}
			return _sprite;
		}
		set
		{
			_sprite = value;
		}
	}

	public tk2dSpriteAnimator animator
	{
		get
		{
			if (!hasSetup)
			{
				Setup();
			}
			return _animator;
		}
		set
		{
			_animator = value;
		}
	}

	public bool visible
	{
		get
		{
			return _visible;
		}
		set
		{
			shaderRenderer.enabled = value;
			_visible = value;
		}
	}

	public void Setup()
	{
		isTintable = true;
		_sprite = GetComponent<tk2dSprite>();
		_animator = GetComponent<tk2dSpriteAnimator>();
		shaderRenderer = GetComponent<Renderer>();
		_visible = shaderRenderer.enabled;
		if (nonChildRenderersToUpdate == null)
		{
			nonChildRenderersToUpdate = new Renderer[0];
		}
		hasSetup = true;
		if (base.gm != null)
		{
			base.gm.entityPool.Add(this);
		}
	}

	public override void Awake()
	{
		base.Awake();
		if (!hasSetup)
		{
			Setup();
		}
	}

	public void LateUpdate()
	{
		UpdateShaderDataSource();
		if (!isTintable || !visible || lastFrameUpdatedGraphics >= shaderDataToUse.frameChanged)
		{
			return;
		}
		if (updateChildRenderers)
		{
			foreach (Renderer item in GetComponentsInChildren<Renderer>().Except(childRenderersToIgnore))
			{
				shaderDataToUse.SetPropertiesInRenderer(item);
			}
		}
		else
		{
			shaderDataToUse.SetPropertiesInRenderer(shaderRenderer);
		}
		Renderer[] array = nonChildRenderersToUpdate;
		foreach (Renderer propertiesInRenderer in array)
		{
			shaderDataToUse.SetPropertiesInRenderer(propertiesInRenderer);
		}
		lastFrameUpdatedGraphics = RDTime.currentFrame;
	}

	private string GetAnimPath(string animationName)
	{
		if (animPrefixName.IsNullOrEmpty())
		{
			Debug.LogWarning(base.name + ": No animPrefixName defined on entity");
			return animationName;
		}
		return animPrefixName + "_" + animationName;
	}

	public bool AnimExists(string animationName)
	{
		string animPath = GetAnimPath(animationName);
		return animator.GetClipByName(animPath) != null;
	}

	public void PlayAnim(string animationName, bool shouldReset = true)
	{
		string animPath = GetAnimPath(animationName);
		animator.Play(animPath, shouldReset);
	}

	public bool IsCurrentEntityClip(string animstr)
	{
		string clipname = animPrefixName + "_" + animstr;
		return animator.IsCurrentClip(clipname);
	}

	public void Shake(int jitterCount, int jitterAmount)
	{
		base.gameObject.GetOrAddComponent<scrThingShake>().activateThingJitter(jitterCount, jitterAmount);
	}

	public void Shake(float duration, int amountX, int amountY)
	{
		base.gameObject.GetOrAddComponent<scrThingShake>().activateThingJitter(duration, amountX, amountY);
	}

	public void StopShake()
	{
		base.gameObject.GetOrAddComponent<scrThingShake>().StopShake();
	}

	public new void OnDestroy()
	{
		base.OnDestroy();
		if (base.gm != null)
		{
			base.gm.entityPool.Remove(this);
		}
	}

	public void DisableOutlines()
	{
		shaderDataSource = this;
		shaderData.borderColor = shaderData.borderColor.WithAlpha(0f);
		shaderData.outline = 1;
	}
}
