namespace RDLevelEditor
{
	public class TabSection_Song : TabSection
	{
		public override void Setup()
		{
			tab = Tab.Song;
			base.Setup();
		}

		public override string DefaultControlName()
		{
			return "Song";
		}
	}
}
