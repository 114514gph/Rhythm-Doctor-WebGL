using System.IO;

public class RDDirectory_Default : RDDirectory
{
	protected override bool InternalExists(string path)
	{
		return Directory.Exists(path);
	}

	protected override void InternalCreateDirectory(string path)
	{
		Directory.CreateDirectory(path);
	}
}
