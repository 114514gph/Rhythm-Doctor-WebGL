using UnityEngine;

public struct BackgroundData
{
	public tk2dSpriteCollectionData spriteCollectionData;

	public string spriteName;

	public Vector3 scale;

	public Vector2 velocity;

	public int renderQueue;

	public bool fixedPosition;

	public bool forceTiling;

	public TileType tileType;

	public bool followCamera;

	public int room;

	public bool animatedFrames;

	public float fps;

	public FollowBeatType followBeatType;

	public bool isForeground;
}
