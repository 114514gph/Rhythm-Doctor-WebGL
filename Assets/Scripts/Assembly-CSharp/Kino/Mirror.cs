using System;
using UnityEngine;

namespace Kino
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	[AddComponentMenu("Kino Image Effects/Mirror")]
	public class Mirror : MonoBehaviour
	{
		public int _repeat;

		public float _offset;

		public float _roll;

		public bool _symmetry;

		public Shader _shader;

		private Material _material;

		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (_material == null)
			{
				_material = new Material(_shader);
				_material.hideFlags = HideFlags.DontSave;
			}
			float value = MathF.PI * 2f / (float)Mathf.Max(1, _repeat);
			_material.SetFloat("_Divisor", value);
			_material.SetFloat("_Offset", _offset * (MathF.PI / 180f));
			_material.SetFloat("_Roll", _roll * (MathF.PI / 180f));
			if (_symmetry)
			{
				_material.EnableKeyword("SYMMETRY_ON");
			}
			else
			{
				_material.DisableKeyword("SYMMETRY_ON");
			}
			Graphics.Blit(source, destination, _material);
		}
	}
}
