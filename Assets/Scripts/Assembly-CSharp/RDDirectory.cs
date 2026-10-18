public abstract class RDDirectory
{
	private static readonly RDDirectory Instance = new RDDirectory_Default();

	public static bool Exists(string path)
	{
		return Instance.InternalExists(path);
	}

	public static void CreateDirectory(string path)
	{
		Instance.InternalCreateDirectory(path);
	}

	protected abstract bool InternalExists(string path);

	protected abstract void InternalCreateDirectory(string path);
}
