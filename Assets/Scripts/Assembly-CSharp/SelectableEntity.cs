using UnityEngine;

public class SelectableEntity : RDClass
{
	public GameObject gameObject;

	public string id;

	public int group;

	public Vector2 gamePosition;

	public Vector2 cameraOffset;

	public Vector2 descriptionOffset;

	public Vector2 downArrowOffset;

	public Vector2 upArrowOffset;

	public LevelType levelType;

	public bool normalEnabled;

	public bool hardEnabled;

	public bool leftPager;

	public bool skipCamAnimOnSelect;

	public bool isBossLevel => levelType == LevelType.Boss;

	public bool isBonus => levelType == LevelType.Bonus;

	public bool isIntermission => levelType == LevelType.Intermission;

	public bool isCollab => levelType == LevelType.Collab;
}
