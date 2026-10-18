using System.Text;

public abstract class RDFile
{
	private static readonly RDFile Instance = new RDFile_Default();

	protected abstract Encoding DefaultEncoding { get; }

	public static void WriteAllText(string path, string data, Encoding encoding = null)
	{
		Instance.InternalWriteAllText(path, data, encoding);
	}

	public static void WriteAllBytes(string path, byte[] bytes)
	{
		Instance.InternalWriteAllBytes(path, bytes);
	}

	public static string ReadAllText(string path, Encoding encoding = null)
	{
		return Instance.InternalReadAllText(path, encoding);
	}

	public static byte[] ReadAllBytes(string path)
	{
		return Instance.InternalReadAllBytes(path);
	}

	public static bool Exists(string path)
	{
		return Instance.InternalExists(path);
	}

	public static void Copy(string sourceFileName, string destFileName, bool overwrite = false)
	{
		Instance.InternalCopy(sourceFileName, destFileName, overwrite);
	}

	public static void Delete(string path)
	{
		Instance.InternalDelete(path);
	}

	public static void Move(string sourceFileName, string destFileName)
	{
		Instance.InternalMove(sourceFileName, destFileName);
	}

	public static void Create(string path)
	{
		Instance.InternalCreate(path);
	}

	protected abstract void InternalWriteAllText(string path, string data, Encoding encoding = null);

	protected abstract void InternalWriteAllBytes(string path, byte[] bytes);

	protected abstract string InternalReadAllText(string path, Encoding encoding = null);

	protected abstract byte[] InternalReadAllBytes(string path);

	protected abstract bool InternalExists(string path);

	protected abstract void InternalCopy(string sourceFileName, string destFileName, bool overwrite = false);

	protected abstract void InternalDelete(string path);

	protected abstract void InternalMove(string sourceFileName, string destFileName);

	protected abstract void InternalCreate(string path);
}
