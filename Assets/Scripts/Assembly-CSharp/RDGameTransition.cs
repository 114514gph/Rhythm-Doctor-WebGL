using System;
using UnityEngine;

public class RDGameTransition : RDBase
{
	private string _spriteName;

	public GameObject transitionPiecePrefab;

	public RDTransitionPiece[,] transitionPieceArray;

	public float layerTimeDelay;

	public float pieceTransitionDuration;

	public bool isReversed;

	private int yCorePos;

	private int xCorePos;

	private float width;

	private float height;

	[NonSerialized]
	public float transitionDuration;

	public string spriteName
	{
		get
		{
			return _spriteName;
		}
		set
		{
			_spriteName = value;
			for (int i = 0; i < transitionPieceArray.GetLength(0); i++)
			{
				for (int j = 0; j < transitionPieceArray.GetLength(1); j++)
				{
					transitionPieceArray[i, j].SetSpriteSheet(_spriteName);
				}
			}
		}
	}

	private void Start()
	{
		Camera componentInParent = GetComponentInParent<Camera>();
		height = 2f * componentInParent.orthographicSize;
		width = height * componentInParent.aspect;
		int num = Mathf.FloorToInt(width / 16f) + 1;
		int num2 = Mathf.FloorToInt(height / 16f) + 1;
		transitionPieceArray = new RDTransitionPiece[num, num2];
		base.transform.position = new Vector3(-1f * ((float)transitionPieceArray.GetLength(0) * 16f / 2f), -1f * ((float)transitionPieceArray.GetLength(1) * 16f / 2f), base.transform.position.z);
		CalculateTransitionPieces();
	}

	public void CalculateTransitionPieces()
	{
		Vector3 vector = new Vector3(0f, 0f, base.transform.position.z);
		for (int i = 0; i < transitionPieceArray.GetLength(0); i++)
		{
			vector.x = i * 16;
			for (int j = 0; j < transitionPieceArray.GetLength(1); j++)
			{
				vector.y = j * 16;
				if (transitionPieceArray[i, j] == null)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(transitionPiecePrefab, base.transform.localPosition + vector, base.transform.rotation, base.transform);
					transitionPieceArray[i, j] = gameObject.GetComponent<RDTransitionPiece>();
					transitionPieceArray[i, j].SetSpriteSheet(_spriteName);
				}
			}
		}
	}

	public void PlayTransition(SelectableEntity selectable, bool isReverting = false)
	{
		if (selectable == null || selectable.gameObject == null)
		{
			return;
		}
		bool flag = false;
		Transform obj = selectable.gameObject.transform;
		float num = obj.position.x;
		float num2 = obj.position.y;
		scnLevelSelect obj2 = base.scnCurrent as scnLevelSelect;
		float num3 = obj2.wrldCamera.transform.position.x;
		float num4 = obj2.wrldCamera.transform.position.y;
		float num5 = width / 2f - (num3 - num);
		float num6 = height / 2f - (num4 - num2);
		xCorePos = Mathf.RoundToInt(num5 / 16f);
		xCorePos = Mathf.Clamp(xCorePos, 0, transitionPieceArray.GetUpperBound(0));
		yCorePos = Mathf.RoundToInt(num6 / 16f);
		yCorePos = Mathf.Clamp(yCorePos, 0, transitionPieceArray.GetUpperBound(0));
		int num7 = transitionPieceArray.GetLength(0) - xCorePos;
		int num8 = transitionPieceArray.GetLength(1) - yCorePos;
		if (yCorePos > xCorePos && yCorePos > num8 && yCorePos > num7)
		{
			transitionDuration = (float)yCorePos * layerTimeDelay;
		}
		else if (xCorePos > yCorePos && xCorePos > num8 && xCorePos > num7)
		{
			transitionDuration = (float)xCorePos * layerTimeDelay;
		}
		else if (num8 > xCorePos && num8 > yCorePos && num8 > num7)
		{
			transitionDuration = (float)num8 * layerTimeDelay;
		}
		else
		{
			transitionDuration = (float)num7 * layerTimeDelay;
		}
		float num9 = (isReversed ? transitionDuration : 0f);
		transitionDuration = transitionDuration - layerTimeDelay + pieceTransitionDuration;
		transitionPieceArray[xCorePos, yCorePos].AnimateLineal(pieceTransitionDuration, num9, isReverting);
		int num11;
		int num10 = (num11 = yCorePos);
		int num13;
		int num12 = (num13 = xCorePos);
		do
		{
			num10--;
			num12--;
			num11++;
			num13++;
			flag = false;
			num9 = (isReversed ? (num9 - layerTimeDelay) : (num9 + layerTimeDelay));
			for (int i = num10; i <= num11; i++)
			{
				if (i >= 0 && i < transitionPieceArray.GetLength(1))
				{
					if (num12 >= 0 && num12 < transitionPieceArray.GetLength(0))
					{
						flag = true;
						transitionPieceArray[num12, i].AnimateLineal(pieceTransitionDuration, num9, isReverting);
					}
					if (num13 >= 0 && num13 < transitionPieceArray.GetLength(0))
					{
						flag = true;
						transitionPieceArray[num13, i].AnimateLineal(pieceTransitionDuration, num9, isReverting);
					}
				}
			}
			for (int j = num12 + 1; j <= num13 - 1; j++)
			{
				if (j >= 0 && j < transitionPieceArray.GetLength(0))
				{
					if (num10 >= 0 && num10 < transitionPieceArray.GetLength(1))
					{
						flag = true;
						transitionPieceArray[j, num10].AnimateLineal(pieceTransitionDuration, num9, isReverting);
					}
					if (num11 >= 0 && num11 < transitionPieceArray.GetLength(1))
					{
						flag = true;
						transitionPieceArray[j, num11].AnimateLineal(pieceTransitionDuration, num9, isReverting);
					}
				}
			}
		}
		while (flag);
		transitionDuration += 0.5f;
	}
}
