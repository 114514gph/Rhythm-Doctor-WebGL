using System;
using System.Reflection;
using UnityEngine;

namespace RDLevelEditor
{
	public class PropertyControl_Sound : PropertyControl
	{
		public PropertyControl_SoundInput soundInput;

		public override GameObject controlObject => soundInput.gameObject;

		private SoundAttribute controlAttribute => (SoundAttribute)base.propertyInfo.controlAttribute;

		public override void AddListeners(InspectorPanel.ChangeAction action)
		{
			_ = (SoundDataPropertyInfo)base.propertyInfo.NullableUnderlying;
			bool useOptions = controlAttribute.optionsMethod != null;
			soundInput.AddListeners(this, action, useOptions);
		}

		public override void Setup()
		{
		}

		public override void UpdateUI(LevelEvent_Base levelEvent)
		{
			SoundDataStruct soundData = (SoundDataStruct)GetEventValue(levelEvent);
			soundInput.soundData = soundData;
			soundInput.SetShowOffset(soundInput.soundData.IsExternalClip());
			soundInput.UpdateUI(GetOptions(levelEvent));
			soundInput.soundSettingsPopup.UpdateData(soundInput.soundData);
		}

		private string[] GetOptions(LevelEvent_Base levelEvent)
		{
			string optionsMethod = controlAttribute.optionsMethod;
			string[] result = new string[0];
			if (!optionsMethod.IsNullOrEmpty())
			{
				Func<LevelEvent_Base, string[]> optionsMethod2 = GetOptionsMethod(optionsMethod, base.propertyInfo.propertyInfo.DeclaringType);
				if (optionsMethod2 != null)
				{
					result = optionsMethod2(levelEvent);
				}
			}
			return result;
		}

		public override void Save(LevelEvent_Base levelEvent)
		{
			if (base.gameObject.activeInHierarchy && !(PropertyControl_SoundInput.currentSoundInput != soundInput))
			{
				soundInput.Save();
				SetEventValue(levelEvent, soundInput.soundData);
			}
		}

		private static Func<LevelEvent_Base, string[]> GetOptionsMethod(string methodName, Type containingType)
		{
			if (methodName == null)
			{
				throw new ArgumentNullException("methodName");
			}
			if (containingType == null)
			{
				throw new ArgumentNullException("containingType");
			}
			MethodInfo methodInfo = containingType.GetMethod(methodName);
			if (methodInfo == null)
			{
				throw new Exception($"Cannot find method with name '{methodName}' in type '{containingType}'.");
			}
			if (methodInfo.GetParameters().Length != 0)
			{
				throw new Exception("Method '" + methodName + "' should have no parameters.");
			}
			if (methodInfo.ReturnType != typeof(string[]))
			{
				throw new Exception("Method '" + methodName + "' should return string[].");
			}
			return (LevelEvent_Base e) => (string[])methodInfo.Invoke(e, new object[0]);
		}
	}
}
