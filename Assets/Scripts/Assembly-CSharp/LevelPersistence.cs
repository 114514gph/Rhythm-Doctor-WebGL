using System.IO;

public class LevelPersistence : RDClass
{
	public string levelDir;

	public bool inEditor;

	public bool startImmediately;

	public MistakesManager mistakes;

	public string restartLevelPath;

	public MistakesManager restartLevelMistakes;

	public string rootLevelName;

	public LevelPersistence Setup(string path)
	{
		string text = Path.GetDirectoryName(path);
		if (text.IsNullOrEmpty())
		{
			text = path;
		}
		if (text != levelDir)
		{
			Clear();
		}
		levelDir = text;
		inEditor = base.editorMode;
		return this;
	}

	public LevelPersistence Clear()
	{
		startImmediately = false;
		mistakes = null;
		return this;
	}

	public bool ShouldLoad()
	{
		if (inEditor == base.editorMode && !levelDir.IsNullOrEmpty())
		{
			return LevelDirMatches();
		}
		return false;
	}

	public bool LevelDirMatches()
	{
		string levelDirectory = GetLevelDirectory();
		if (levelDir == levelDirectory)
		{
			return true;
		}
		if (scnGame.levelToLoadSource != LevelSource.ExternalPath && levelDir == scnGame.internalIdentifier)
		{
			return true;
		}
		return false;
	}

	public void PrintDebug()
	{
	}

	public string GetLevelPath()
	{
		if (!base.editorMode)
		{
			return scnGame.currentLevelPath;
		}
		return base.editor.openedFilePath;
	}

	public string GetLevelDirectory()
	{
		return Path.GetDirectoryName(GetLevelPath());
	}
}
