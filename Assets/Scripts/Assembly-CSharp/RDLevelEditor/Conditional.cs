using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace RDLevelEditor
{
	public abstract class Conditional : RDClass
	{
		public const string typeKey = "type";

		public const string idKey = "id";

		public const string descriptionKey = "name";

		public const string tagKey = "tag";

		private ConditionalType? _type;

		public string description;

		public string tag;

		public int id;

		public string gid;

		public ConditionalType type
		{
			get
			{
				ConditionalType valueOrDefault = _type.GetValueOrDefault();
				if (!_type.HasValue)
				{
					string text = GetType().ToString();
					int length = "RDLevelEditor.Conditional_".Length;
					valueOrDefault = Enum.Parse<ConditionalType>(text.Substring(length, text.Length - length));
					_type = valueOrDefault;
				}
				return _type.Value;
			}
		}

		public ConditionalInfo info => GC.conditionalsInfo[type.ToString()];

		public abstract bool Check(LevelEvent_Base levelEvent, bool negated);

		public abstract override string ToString();

		public static Conditional Decode(Dictionary<string, object> dict)
		{
			Type obj = Type.GetType(string.Format("RDLevelEditor.Conditional_{0}", dict["type"]));
			if (obj == null)
			{
				throw new Exception(string.Format("Could not find conditional with name '{0}", dict["type"]));
			}
			Conditional conditional = (Conditional)Activator.CreateInstance(obj, 0);
			conditional.description = (string)dict["name"];
			conditional.id = (int)dict["id"];
			conditional.gid = null;
			conditional.tag = ((!dict.ContainsKey("tag") || dict["tag"].ToString().IsNullOrEmpty()) ? conditional.id.ToString() : ((string)dict["tag"]));
			foreach (var (key, basePropertyInfo2) in conditional.info.propertiesInfo)
			{
				if (dict.ContainsKey(key))
				{
					object value = basePropertyInfo2.Decode(dict[key]);
					basePropertyInfo2.propertyInfo.SetValue(conditional, value);
				}
			}
			return conditional;
		}

		public string Encode()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendMany(RDEditorUtils.EncodeString("type", type.ToString()), RDEditorUtils.EncodeString("tag", tag), RDEditorUtils.EncodeString("name", description), RDEditorUtils.EncodeInt("id", id));
			foreach (var (key, basePropertyInfo2) in info.propertiesInfo)
			{
				object value = basePropertyInfo2.propertyInfo.GetValue(this);
				stringBuilder.Append(basePropertyInfo2.Encode(key, value, isLastKey: false));
			}
			while (true)
			{
				StringBuilder eventEncode = RDLevelData.eventEncode;
				char c = eventEncode[eventEncode.Length - 1];
				if (c != ',' && c != ' ')
				{
					break;
				}
				stringBuilder.Length--;
			}
			return stringBuilder.ToString();
		}

		public bool IsGlobal()
		{
			return !gid.IsNullOrEmpty();
		}

		public bool Matches(ConditionalID c)
		{
			if (!c.global)
			{
				return c.numID == id;
			}
			return c.id == gid;
		}

		public virtual Sprite GetSprite()
		{
			return base.editor.conditionalsPanel.typeIconsDict[type];
		}

		protected Conditional(int id)
		{
			this.id = id;
			description = id.ToString();
		}

		protected Conditional(string gid)
		{
			id = 0;
			description = string.Empty;
			this.gid = gid;
		}

		public Conditional CopyWithId(int newId)
		{
			Conditional conditional = (Conditional)Activator.CreateInstance(GetType(), newId);
			conditional.tag = tag;
			conditional.description = description;
			foreach (BasePropertyInfo value in info.propertiesInfo.Values())
			{
				PropertyInfo propertyInfo = value.propertyInfo;
				propertyInfo.SetValue(conditional, value.MakeCopy(propertyInfo.GetValue(this)));
			}
			return conditional;
		}
	}
}
