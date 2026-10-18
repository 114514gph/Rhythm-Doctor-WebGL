using UnityEngine;

public class BlinkingText : InvisibleEntity
{
	public override void OnBeat()
	{
		MeshRenderer component = base.gameObject.GetComponent<MeshRenderer>();
		component.enabled = !component.enabled;
	}
}
