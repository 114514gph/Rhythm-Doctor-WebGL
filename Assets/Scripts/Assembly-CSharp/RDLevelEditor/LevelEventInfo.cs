using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace RDLevelEditor
{
	public record LevelEventInfo
	{
		public string name
		{
			[return: NotNull]
			get
			{
				return eventTypeEnum.ToString();
			}
		}

		public bool usesRow => attribute.defaultRow != -10;

		public bool showsRowControl
		{
			get
			{
				if (usesRow)
				{
					LevelEventType levelEventType = eventTypeEnum;
					if (levelEventType != LevelEventType.AddClassicBeat && levelEventType != LevelEventType.AddOneshotBeat && levelEventType != LevelEventType.AddFreeTimeBeat && levelEventType != LevelEventType.SetRowXs)
					{
						return levelEventType != LevelEventType.SetOneshotWave;
					}
					return false;
				}
				return false;
			}
		}

		public readonly LevelEventType eventTypeEnum;

		public readonly LevelEventInfoAttribute attribute;

		[NotNull]
		public readonly ImmutableList<BasePropertyInfo> propertiesInfo;

		public LevelEventInfo(Type eventType)
		{
			attribute = eventType.GetCustomAttribute<LevelEventInfoAttribute>();
			if (attribute == null)
			{
				throw new Exception(string.Format("Event '{0}' does not have an '{1}' attribute.", eventType, "LevelEventInfoAttribute"));
			}
			string text = eventType.ToString();
			int length = "RDLevelEditor.LevelEvent_".Length;
			eventTypeEnum = Enum.Parse<LevelEventType>(text.Substring(length, text.Length - length));
			Type type = eventType;
			List<BasePropertyInfo> list = new List<BasePropertyInfo>();
			while (type != null && type != typeof(LevelEvent_Base))
			{
				list.AddRange(from fieldInfo in type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
					where fieldInfo.IsDefined(typeof(JsonPropertyAttribute))
					orderby fieldInfo.MetadataToken
					select BasePropertyInfo.FromProperty(fieldInfo));
				type = type.BaseType;
			}
			propertiesInfo = ImmutableList.CreateRange(list);
		}

		[CompilerGenerated]
		protected virtual bool PrintMembers(StringBuilder builder)
		{
			RuntimeHelpers.EnsureSufficientExecutionStack();
			builder.Append("name = ");
			builder.Append((object)name);
			builder.Append(", eventTypeEnum = ");
			builder.Append(eventTypeEnum.ToString());
			builder.Append(", attribute = ");
			builder.Append(attribute);
			builder.Append(", propertiesInfo = ");
			builder.Append(propertiesInfo);
			builder.Append(", usesRow = ");
			builder.Append(usesRow.ToString());
			builder.Append(", showsRowControl = ");
			builder.Append(showsRowControl.ToString());
			return true;
		}
	}
}
