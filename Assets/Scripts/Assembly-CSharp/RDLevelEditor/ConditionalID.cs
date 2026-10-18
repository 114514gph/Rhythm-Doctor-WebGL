namespace RDLevelEditor
{
	public struct ConditionalID
	{
		public string str;

		public string id;

		public int numID;

		public bool negated;

		public bool global;

		public ConditionalID(string str)
		{
			this.str = str;
			if (int.TryParse(str, out var result))
			{
				global = false;
				negated = result < 0;
				numID = (negated ? (-result - 1) : result);
				id = numID.ToString();
				return;
			}
			global = true;
			negated = str.StartsWith("~");
			numID = 0;
			string text;
			if (!negated)
			{
				text = str;
			}
			else
			{
				text = str.Substring(1, str.Length - 1);
			}
			id = text;
		}
	}
}
