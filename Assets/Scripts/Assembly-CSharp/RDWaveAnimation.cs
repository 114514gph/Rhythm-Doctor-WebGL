using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "RhythmDoctor/Wave Animation", order = 1)]
public class RDWaveAnimation : ScriptableObject
{
	public new string name;

	public float duration;

	public float timeScale;

	public AnimationCurve waveTime;

	public AnimationCurve waveSpeed;

	public AnimationCurve sineWidth;

	public AnimationCurve sineHeight;

	public AnimationCurve triangleOffset;

	public AnimationCurve triangleFrequency;

	[Range(0f, 1f)]
	public float triangleLock;

	public bool heightDecays;

	public float decayConstant;

	public float angularFrequency;
}
