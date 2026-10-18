using System;

namespace RDLevelEditor
{
	[AttributeUsage(AttributeTargets.Property)]
	public class CharacterPickerAttribute : ControlAttribute
	{
		public readonly Character[] availableCharacters;

		public readonly bool portraitPicker;

		public readonly bool returnsExpression;

		public readonly bool includesCustomChars;

		public readonly bool includesNew;

		public readonly bool includesNone;

		public readonly bool emptyIsIgnore;

		public readonly bool showFilePicker;

		public CharacterPickerAttribute(Character[] availableCharacters, bool useDefaultCharacters = false, bool portraitPicker = false, bool returnsExpression = false, bool includesCustomChars = false, bool includesNew = false, bool includesNone = false, bool emptyIsIgnore = false, bool showFilePicker = false, bool multiline = false)
			: base(multiline)
		{
			this.availableCharacters = (useDefaultCharacters ? RDEditorConstants.AvailableCharacters : availableCharacters);
			this.portraitPicker = portraitPicker;
			this.returnsExpression = returnsExpression;
			this.includesCustomChars = includesCustomChars;
			this.includesNew = includesNew;
			this.includesNone = includesNone;
			this.emptyIsIgnore = emptyIsIgnore;
			this.showFilePicker = showFilePicker;
		}
	}
}
