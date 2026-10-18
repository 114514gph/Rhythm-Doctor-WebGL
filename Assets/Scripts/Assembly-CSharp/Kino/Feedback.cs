using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Kino
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	public class Feedback : MonoBehaviour
	{
		[SerializeField]
		private Color _color = Color.white;

		[SerializeField]
		[Range(-1f, 1f)]
		private float _offsetX;

		[SerializeField]
		[Range(-1f, 1f)]
		private float _offsetY;

		[SerializeField]
		[Range(-5f, 5f)]
		private float _rotation;

		[SerializeField]
		[Range(0.95f, 1.05f)]
		private float _scale = 1f;

		public bool _jaggies;

		public Shader _feedbackShader;

		private Material _feedbackMaterial;

		private RenderTexture _delayBuffer;

		private CommandBuffer _feedbackCommand;

		public Color color
		{
			get
			{
				return _color;
			}
			set
			{
				_color = value;
			}
		}

		public float offsetX
		{
			get
			{
				return _offsetX;
			}
			set
			{
				_offsetX = value;
			}
		}

		public float offsetY
		{
			get
			{
				return _offsetY;
			}
			set
			{
				_offsetY = value;
			}
		}

		public float rotation
		{
			get
			{
				return _rotation;
			}
			set
			{
				_rotation = value;
			}
		}

		public float scale
		{
			get
			{
				return _scale;
			}
			set
			{
				_scale = value;
			}
		}

		public bool jaggies
		{
			get
			{
				return _jaggies;
			}
			set
			{
				_jaggies = value;
			}
		}

		private Shader feedbackShader
		{
			get
			{
				if (!_feedbackShader)
				{
					return Shader.Find("Hidden/Kino/Feedback");
				}
				return _feedbackShader;
			}
		}

		private Material feedbackMaterial
		{
			get
			{
				if (_feedbackMaterial == null)
				{
					_feedbackMaterial = new Material(feedbackShader);
					_feedbackMaterial.hideFlags = HideFlags.DontSave;
				}
				return _feedbackMaterial;
			}
		}

		private Camera targetCamera => GetComponent<Camera>();

		private RenderTexture delayBuffer
		{
			get
			{
				if (_delayBuffer == null)
				{
					Camera camera = targetCamera;
					_delayBuffer = new RenderTexture(camera.pixelWidth, camera.pixelHeight, 0, RenderTextureFormat.DefaultHDR);
					_delayBuffer.hideFlags = HideFlags.DontSave;
				}
				return _delayBuffer;
			}
		}

		private CommandBuffer feedbackCommand
		{
			get
			{
				if (_feedbackCommand == null)
				{
					_feedbackCommand = new CommandBuffer();
					_feedbackCommand.name = "Kino.Feedback";
					_feedbackCommand.Blit(delayBuffer, BuiltinRenderTextureType.CameraTarget, feedbackMaterial, 0);
				}
				return _feedbackCommand;
			}
		}

		private Vector4 rotationMatrixAsVector
		{
			get
			{
				float f = -MathF.PI / 180f * _rotation;
				float num = Mathf.Sin(f);
				float num2 = Mathf.Cos(f);
				return new Vector4(num2, num, 0f - num, num2);
			}
		}

		private void OnDisable()
		{
			if (_feedbackCommand != null)
			{
				targetCamera.RemoveCommandBuffer(CameraEvent.BeforeForwardAlpha, _feedbackCommand);
			}
			_feedbackCommand = null;
			if (_delayBuffer != null)
			{
				_delayBuffer.Release();
				UnityEngine.Object.DestroyImmediate(_delayBuffer);
			}
			_delayBuffer = null;
			if (_feedbackMaterial != null)
			{
				UnityEngine.Object.DestroyImmediate(_feedbackMaterial);
			}
			_feedbackMaterial = null;
		}

		private void Update()
		{
			if (!(_delayBuffer == null))
			{
				if (_feedbackCommand == null)
				{
					targetCamera.AddCommandBuffer(CameraEvent.BeforeForwardAlpha, feedbackCommand);
				}
				Camera camera = targetCamera;
				if (camera.pixelWidth != _delayBuffer.width || camera.pixelHeight != _delayBuffer.height)
				{
					_delayBuffer.Release();
					_delayBuffer.width = camera.pixelWidth;
					_delayBuffer.height = camera.pixelHeight;
				}
				Material material = feedbackMaterial;
				material.SetColor("_Color", _color);
				material.SetVector("_Offset", new Vector2(_offsetX, _offsetY) * -0.05f);
				material.SetVector("_Rotation", rotationMatrixAsVector);
				material.SetFloat("_Scale", 2f - _scale);
				delayBuffer.filterMode = ((!_jaggies) ? FilterMode.Bilinear : FilterMode.Point);
			}
		}

		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			Graphics.Blit(source, delayBuffer);
			Graphics.Blit(source, destination);
		}
	}
}
