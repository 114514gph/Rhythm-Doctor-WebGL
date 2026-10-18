using System.Collections;
using UnityEngine;

public class scrHoldBar : Entity
{
	private const float InitialWidth = 1f;

	public tk2dSprite border;

	public Color defaultColor;

	public float maxWidth;

	private bool mBarValueLocked;

	private bool mVisibleHold;

	private float mValue;

	private Renderer borderRenderer;

	private Color barFullColor = new Color(188f / 255f, 0f, 1f, 1f);

	private Color barBlinkColor = new Color(220f / 255f, 0f, 1f, 1f);

	private Color barReleasedEarlyColor = new Color(230f / 255f, 40f / 255f, 40f / 255f, 1f);

	public bool visibleHold
	{
		get
		{
			return mVisibleHold;
		}
		set
		{
			mVisibleHold = value;
			base.visible = value;
		}
	}

	private void Start()
	{
		visibleHold = false;
		borderRenderer = border.GetComponent<Renderer>();
		UpdateVisibility();
		maxWidth = base.sprite.scale.x - 1f;
		defaultColor = base.sprite.color;
	}

	private void Update()
	{
		if (!mBarValueLocked)
		{
			UpdateVisibility();
		}
		if (mValue == 1f)
		{
			bool flag = Time.timeSinceLevelLoad % 0.2f < 0.1f;
			base.sprite.color = (flag ? barFullColor : barBlinkColor);
		}
	}

	private void UpdateVisibility()
	{
		borderRenderer.enabled = mVisibleHold;
	}

	public void SetBarPercentage(float value)
	{
		if (!mBarValueLocked)
		{
			InternalSetBarPercentage(value);
		}
	}

	private void InternalSetBarPercentage(float value)
	{
		mValue = value;
		base.sprite.scale = base.sprite.scale.WithX(value * maxWidth + 1f);
		bool flag = mValue == 1f;
		base.sprite.color = (flag ? barFullColor : defaultColor);
	}

	public void PlayerReleasedEarly()
	{
		visibleHold = false;
		mBarValueLocked = true;
		Shake(20, 3);
		base.sprite.color = barReleasedEarlyColor;
		StartCoroutine(Blink());
	}

	private IEnumerator Blink()
	{
		for (float i = 0f; i < 1f; i += Time.deltaTime)
		{
			bool flag = i % 0.16f < 0.08f;
			border.color = (flag ? Color.gray : Color.white);
			yield return null;
		}
		border.color = Color.white;
		mBarValueLocked = false;
	}
}
