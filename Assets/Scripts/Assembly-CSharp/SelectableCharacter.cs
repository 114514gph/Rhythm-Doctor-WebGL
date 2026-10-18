using System.Collections.Generic;

public class SelectableCharacter : SelectableEntity
{
	public Character[] charactersNormal = new Character[1] { Character.None };

	public Character[] charactersHard = new Character[1] { Character.None };

	public Character[] portraitsNormal = new Character[1] { Character.None };

	public Character[] portraitsHard = new Character[1] { Character.None };

	public List<Level> unlocks;

	public Dictionary<Difficulty, Level> levels;

	public scrChar animation;

	public scrChar[] childAnimations = new scrChar[0];

	public string levelTier;

	public int bpmNormal;

	public int bpmHard;
}
