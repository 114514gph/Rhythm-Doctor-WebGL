using UnityEngine;

public class scrThingShake : RDBase
{
	private float thingJitterTime;

	private int thingJitterAmountX = 20;

	private int thingJitterAmountY = 20;

	private float saveThingX;

	private float saveThingY;

	public void StopShake()
	{
		if (thingJitterTime > 0f)
		{
			thingJitterTime = 0f;
			base.x = saveThingX;
			base.y = saveThingY;
		}
	}

	public void activateThingJitter(int jitterCount, int jitterAmount)
	{
		if (thingJitterTime <= 0f)
		{
			thingJitterTime = (float)jitterCount / 60f;
			thingJitterAmountX = jitterAmount;
			thingJitterAmountY = jitterAmount;
			saveThingX = base.x;
			saveThingY = base.y;
		}
		else if (jitterAmount > thingJitterAmountX)
		{
			thingJitterTime = (float)jitterCount / 60f;
			thingJitterAmountX = jitterAmount;
			thingJitterAmountY = jitterAmount;
		}
	}

	public void activateThingJitter(float duration, int amountX, int amountY)
	{
		if (thingJitterTime <= 0f)
		{
			thingJitterTime = duration;
			thingJitterAmountX = amountX;
			thingJitterAmountY = amountY;
			saveThingX = base.x;
			saveThingY = base.y;
		}
		else if (amountX > thingJitterAmountX || amountY > thingJitterAmountY)
		{
			thingJitterTime = duration;
			thingJitterAmountX = amountX;
			thingJitterAmountY = amountY;
		}
	}

	private void Update()
	{
		if (!(thingJitterTime > 0f))
		{
			return;
		}
		thingJitterTime -= Time.unscaledDeltaTime;
		if (thingJitterTime <= 0f)
		{
			base.x = saveThingX;
			base.y = saveThingY;
			return;
		}
		float num = Random.value * (float)thingJitterAmountX;
		if ((double)Random.value < 0.5)
		{
			num = 0f - num;
		}
		float num2 = Random.value * (float)thingJitterAmountY;
		if ((double)Random.value < 0.5)
		{
			num2 = 0f - num2;
		}
		base.x = saveThingX + num;
		base.y = saveThingY + num2;
	}
}
