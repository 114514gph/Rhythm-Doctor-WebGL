using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class ShatteredGlass : MonoBehaviour
{
	[HideInInspector]
	public List<MeshRenderer> shards;

	public List<Vector3> shardPositions;

	private bool shattered;

	public void Shatter(RDRoom room)
	{
		base.gameObject.SetActive(value: true);
		RenderTexture renderTexture = room.renderTexture;
		Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, renderTexture.graphicsFormat, TextureCreationFlags.None);
		Graphics.CopyTexture(room.renderTexture, texture2D);
		ApplyTexture(texture2D);
		System.Random random = new System.Random();
		int num = 0;
		foreach (MeshRenderer shard in shards)
		{
			Transform transform = shard.transform;
			if (shattered)
			{
				transform.DOKill();
				transform.localPosition = shardPositions[num];
				transform.localRotation = Quaternion.identity;
				shard.material.color = Color.white;
			}
			else
			{
				shardPositions.Add(transform.localPosition);
			}
			float num2 = Vector2.Distance(transform.localPosition - new Vector3(176f, 99f), Vector3.zero) / 176f;
			float num3 = Mathf.Abs((transform.localPosition.x - 176f) / 352f * 2f);
			Vector3 endValue = transform.localPosition + new Vector3(num3 * 0.25f, -325 + random.Next(0, 100));
			Vector3 endValue2 = new Vector3(random.Next(-720, 720), random.Next(-720, 720), random.Next(-720, 720));
			float num4 = (float)random.NextDouble() * num3 * 1f;
			float duration = 0.75f + num4;
			float delay = num2 * 0.2f;
			Material cachedMaterial = shard.material;
			transform.DOLocalJump(endValue, transform.localPosition.y + (float)random.Next(5, 20) + (1f - num3) * 40f, 1, duration).SetEase(Ease.Linear).SetDelay(delay);
			transform.DOLocalRotate(endValue2, duration).SetRelative(isRelative: true).SetEase(Ease.Linear)
				.SetDelay(delay)
				.OnUpdate(() =>
				{
					float t = Vector3.Dot(transform.up, Vector3.down);
					float num5 = Mathf.Lerp(1f, 0.5f, t);
					cachedMaterial.color = new Color(num5, num5, num5);
				});
			random.Next();
			num++;
		}
		shattered = true;
	}

	public void ApplyTexture(Texture texture)
	{
		foreach (MeshRenderer shard in shards)
		{
			shard.material.mainTexture = texture;
		}
	}
}
