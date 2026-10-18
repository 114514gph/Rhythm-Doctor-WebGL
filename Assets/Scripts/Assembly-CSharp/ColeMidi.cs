using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ColeMidi : RDBase
{
	[Serializable]
	public class Pattern
	{
		public Color color;

		public float noteSpeed;

		public List<Note> notes;

		public Transform container;
	}

	[Serializable]
	public class Note
	{
		public float bar;

		public float duration;

		public int pitch;

		public SpriteRenderer renderer;

		[NonSerialized]
		public bool spawned;

		[NonSerialized]
		public bool started;

		[NonSerialized]
		public bool ended;
	}

	[Header("Runtime")]
	public float startTime;

	public float bpm;

	public Transform coleTransform;

	[Header("Generation")]
	public GameObject notePrefab;

	public float beatHeight;

	public float beatWidth;

	public List<Pattern> patterns;

	private void OnEnable()
	{
		bpm = base.conductor.bpm;
		startTime = base.conductor.songFileTimeVisual;
		foreach (Pattern pattern in patterns)
		{
			foreach (Note note in pattern.notes)
			{
				note.renderer.gameObject.SetActive(value: false);
			}
		}
		Row[] rows = base.game.rows;
		foreach (Row row in rows)
		{
			Character? character = row.ent?.character?.character;
			if (!character.HasValue || character != Character.HoodieBoy)
			{
				character = row.ent?.character?.character;
				if (!character.HasValue || character != Character.Ian)
				{
					continue;
				}
			}
			if (row.ent.isActiveAndEnabled)
			{
				coleTransform = row.ent.character.transform;
				break;
			}
		}
	}

	private void LateUpdate()
	{
		scrConductor obj = base.conductor;
		if ((object)obj != null && obj.IsSongPlaying())
		{
			float num = base.conductor.songFileTimeVisual - startTime;
			float num2 = 60f / bpm;
			float beat = num / num2;
			UpdateSongPos(beat);
		}
	}

	private void UpdateSongPos(float beat)
	{
		float duration = 0.2f;
		foreach (Pattern pattern in patterns)
		{
			Transform container = pattern.container;
			float num = beatWidth * pattern.noteSpeed * beat;
			container.localPosition = new Vector3(0f - num, 0f);
			foreach (Note note in pattern.notes)
			{
				if (note.ended)
				{
					continue;
				}
				float num2 = (note.bar - 1f) * 4f;
				float num3 = 24f * (1f / pattern.noteSpeed);
				float num4 = num2 - num3;
				if (beat < num4)
				{
					continue;
				}
				if (!note.spawned)
				{
					note.spawned = true;
					note.renderer.gameObject.SetActive(value: true);
					if (beat > 8f)
					{
						float endValue = note.renderer.size.x;
						note.renderer.size = note.renderer.size.WithX(beatWidth);
						DOTween.To(() => note.renderer.size.x, (float x) =>
						{
							note.renderer.size = note.renderer.size.WithX(x);
						}, endValue, 0.75f).SetDelay(0.75f).SetEase(Ease.OutSine);
						Vector3 localPosition = note.renderer.transform.localPosition;
						note.renderer.transform.position = coleTransform.position;
						note.renderer.transform.DOLocalMove(localPosition, 0.75f).SetEase(Ease.OutQuad);
					}
				}
				else
				{
					if (beat < num2)
					{
						continue;
					}
					if (!note.started)
					{
						note.started = true;
						note.renderer.material.DOFloat(0.25f, RDShaderProperties.FlashID, duration).SetEase(Ease.OutSine);
						note.renderer.transform.DOKill();
						note.renderer.transform.DOLocalMoveY((0f - beatHeight) * 0.5f, duration).SetRelative(isRelative: true).SetEase(Ease.OutSine);
						continue;
					}
					float num5 = num2 + note.duration;
					if (!(beat < num5))
					{
						note.ended = true;
						note.renderer.material.DOFloat(0f, RDShaderProperties.FlashID, duration).SetEase(Ease.OutSine);
						note.renderer.transform.DOKill();
						note.renderer.transform.DOLocalMoveY(beatHeight * 0.5f, duration).SetRelative(isRelative: true).SetEase(Ease.OutSine);
					}
				}
			}
		}
	}

	[ContextMenu("Generate Notes")]
	public void GenerateNotes()
	{
		foreach (Pattern pattern in patterns)
		{
			Transform transform = new GameObject("Pattern").transform;
			transform.SetParent(base.transform);
			float noteSpeed = pattern.noteSpeed;
			transform.localPosition = Vector3.zero.WithZ(noteSpeed);
			if (pattern.container != null)
			{
				UnityEngine.Object.DestroyImmediate(pattern.container.gameObject);
			}
			pattern.container = transform;
			foreach (Note note in pattern.notes)
			{
				GameObject obj = UnityEngine.Object.Instantiate(notePrefab);
				SpriteRenderer component = obj.GetComponent<SpriteRenderer>();
				obj.transform.SetParent(transform);
				component.color = pattern.color;
				float num = (note.bar - 1f) * 4f;
				int num2 = note.pitch - 1;
				float num3 = num * beatWidth * noteSpeed;
				float num4 = (float)num2 * beatHeight;
				float num5 = note.duration * beatWidth * noteSpeed;
				float num6 = beatHeight;
				obj.transform.localPosition = new Vector3(num3, num4);
				component.size = new Vector3(num5, num6);
				if (note.renderer != null)
				{
					UnityEngine.Object.DestroyImmediate(note.renderer.gameObject);
				}
				note.renderer = component;
			}
		}
	}
}
