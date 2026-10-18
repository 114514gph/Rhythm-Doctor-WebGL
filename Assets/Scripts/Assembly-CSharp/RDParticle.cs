using UnityEngine;

public class RDParticle : Entity
{
	public RDParticleSystem mSystem;

	public float startingTime;

	public float lifetime;

	public bool diesWhenPassingScreen;

	public bool isDead;

	public float speed;

	private float mTimeElapsed;

	public float angularSpeed;

	public string spriteName
	{
		set
		{
			GetComponent<tk2dSprite>().SetSprite(RDBase.Vfx.scLevelElements, value);
		}
	}

	public void Setup(string animSheetName, RDParticleSystem system)
	{
		if (animSheetName != "")
		{
			tk2dSpriteAnimation library = Resources.Load(animSheetName, typeof(tk2dSpriteAnimation)) as tk2dSpriteAnimation;
			base.animator.Library = library;
		}
		mSystem = system;
	}

	private void Start()
	{
		isDead = false;
		startingTime = Time.timeSinceLevelLoad;
	}

	private void Update()
	{
		base.x += Time.deltaTime * speed * mSystem.speedMultiplier;
		mTimeElapsed += Time.deltaTime;
		if (diesWhenPassingScreen)
		{
			HasPassedScreen();
		}
		else if (mTimeElapsed > lifetime)
		{
			Kill();
		}
		base.transform.Rotate(Vector3.forward * (angularSpeed * Time.deltaTime));
	}

	private void HasPassedScreen()
	{
		if (speed > 0f)
		{
			if (base.transform.localPosition.x > RDBase.Vfx.RDWidth + base.sprite.width)
			{
				Kill();
			}
		}
		else if (base.transform.localPosition.x < 0f - base.sprite.width)
		{
			Kill();
		}
	}

	public void Kill()
	{
		isDead = true;
		base.gameObject.SetActive(value: false);
	}

	public void SetColor(Color color)
	{
		shaderDataSource = this;
		shaderData.overlayColor = color;
	}
}
