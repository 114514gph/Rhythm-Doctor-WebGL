namespace OggVorbisEncoder.Setup
{
	public class MappingTemplate : IMappingTemplate
	{
		public Mapping[] Mapping { get; }

		public IResidueTemplate[] ResidueTemplate { get; }

		public MappingTemplate(Mapping[] mapping, IResidueTemplate[] residueTemplate)
		{
			Mapping = mapping;
			ResidueTemplate = residueTemplate;
		}
	}
}
