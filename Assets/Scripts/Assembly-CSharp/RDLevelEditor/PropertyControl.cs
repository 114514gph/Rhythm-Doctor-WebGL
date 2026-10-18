using System;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public abstract class PropertyControl : RDBase
	{
		internal InspectorPanel inspectorPanel;

		private Property parentProperty { get; set; }

		internal BasePropertyInfo propertyInfo => parentProperty.propertyInfo;

		public abstract GameObject controlObject { get; }

		internal Text label => parentProperty?.label;

		public abstract void AddListeners(InspectorPanel.ChangeAction action);

		public abstract void Setup();

		public abstract void UpdateUI(LevelEvent_Base levelEvent);

		public abstract void Save(LevelEvent_Base levelEvent);

		internal object GetEventValue(LevelEvent_Base levelEvent)
		{
			return parentProperty.propertyInfo.propertyInfo.GetValue(levelEvent);
		}

		internal void SetEventValue(LevelEvent_Base levelEvent, object value)
		{
			parentProperty.propertyInfo.propertyInfo.SetValue(levelEvent, value);
		}

		internal void SetPropertyControlHeight(float height, bool updateControlObject = true, bool updateProperty = false)
		{
			RectTransform component = GetComponent<RectTransform>();
			component.sizeDelta = new Vector2(component.sizeDelta.x, height);
			if (updateControlObject)
			{
				RectTransform component2 = controlObject.GetComponent<RectTransform>();
				component2.sizeDelta = new Vector2(component2.sizeDelta.x, height);
			}
			if (updateProperty)
			{
				Property.UpdateProperty(parentProperty, propertyInfo);
				VerticalLayoutGroup component3 = parentProperty.transform.parent.GetComponent<VerticalLayoutGroup>();
				component3.enabled = false;
				component3.enabled = true;
			}
		}

		internal void SetToggleOnValue(object val)
		{
			if (parentProperty.propertyInfo is NullablePropertyInfo nullablePropertyInfo)
			{
				nullablePropertyInfo.UpdateToggleOnValue(val);
			}
		}

		public static PropertyControl Create(Property property)
		{
			RDConstants data = RDConstants.data;
			ControlAttribute controlAttribute = property.propertyInfo.controlAttribute;
			GameObject original;
			if (!(controlAttribute is DescriptionAttribute))
			{
				if (!(controlAttribute is ButtonAttribute))
				{
					if (!(controlAttribute is InputFieldAttribute))
					{
						if (!(controlAttribute is ColorAttribute))
						{
							if (!(controlAttribute is DropdownAttribute))
							{
								if (!(controlAttribute is RowAttribute))
								{
									if (!(controlAttribute is RoomAttribute))
									{
										if (!(controlAttribute is CheckboxAttribute))
										{
											if (!(controlAttribute is ToggleGroupAttribute))
											{
												if (!(controlAttribute is SliderAttribute))
												{
													if (!(controlAttribute is SliderAlphaAttribute))
													{
														if (!(controlAttribute is SliderPercentAttribute))
														{
															if (!(controlAttribute is PositionPickerAttribute))
															{
																if (!(controlAttribute is ExpPositionPickerAttribute))
																{
																	if (!(controlAttribute is CharacterPickerAttribute))
																	{
																		if (!(controlAttribute is HandAttribute))
																		{
																			if (!(controlAttribute is SetCountingSound))
																			{
																				if (!(controlAttribute is SetGameSoundAttribute))
																				{
																					if (!(controlAttribute is SoundAttribute))
																					{
																						if (!(controlAttribute is ImageAttribute))
																						{
																							if (!(controlAttribute is ReorderRoomsAttribute))
																							{
																								if (!(controlAttribute is ShowRoomsAttribute))
																								{
																									if (!(controlAttribute is SetRoomPerspectiveAttribute))
																									{
																										if (!(controlAttribute is FlipScreenAttribute))
																										{
																											if (!(controlAttribute is PulsePickerAttribute))
																											{
																												if (!(controlAttribute is BPMCalculatorAttribute))
																												{
																													if (!(controlAttribute is BeatModifiersAttribute))
																													{
																														if (!(controlAttribute is ShowDialogueAttribute))
																														{
																															throw new ArgumentOutOfRangeException();
																														}
																														original = data.showDialogueControlPrefab;
																													}
																													else
																													{
																														original = data.beatModifiersControlPrefab;
																													}
																												}
																												else
																												{
																													original = data.bpmCalculatorControlPrefab;
																												}
																											}
																											else
																											{
																												original = data.pulsePickerControlPrefab;
																											}
																										}
																										else
																										{
																											original = data.flipScreenControlPrefab;
																										}
																									}
																									else
																									{
																										original = data.setRoomPerspectiveControlPrefab;
																									}
																								}
																								else
																								{
																									original = data.showRoomsControlPrefab;
																								}
																							}
																							else
																							{
																								original = data.reorderRoomsControlPrefab;
																							}
																						}
																						else
																						{
																							original = data.imageControlPrefab;
																						}
																					}
																					else
																					{
																						original = data.soundControlPrefab;
																					}
																				}
																				else
																				{
																					original = data.setGameSoundControlPrefab;
																				}
																			}
																			else
																			{
																				original = data.setCountingSoundControlPrefab;
																			}
																		}
																		else
																		{
																			original = data.handControlPrefab;
																		}
																	}
																	else
																	{
																		original = data.characterPickerControlPrefab;
																	}
																}
																else
																{
																	original = data.expPositionPickerControlPrefab;
																}
															}
															else
															{
																original = data.positionPickerControlPrefab;
															}
														}
														else
														{
															original = data.sliderPercentControlPrefab;
														}
													}
													else
													{
														original = data.sliderAlphaControlPrefab;
													}
												}
												else
												{
													original = data.sliderControlPrefab;
												}
											}
											else
											{
												original = data.toggleGroupControlPrefab;
											}
										}
										else
										{
											original = data.checkboxControlPrefab;
										}
									}
									else
									{
										original = data.roomControlPrefab;
									}
								}
								else
								{
									original = data.rowControlPrefab;
								}
							}
							else
							{
								original = data.dropdownControlPrefab;
							}
						}
						else
						{
							original = data.colorControlPrefab;
						}
					}
					else
					{
						original = data.inputFieldControlPrefab;
					}
				}
				else
				{
					original = data.buttonControlPrefab;
				}
			}
			else
			{
				original = data.descriptionControlPrefab;
			}
			PropertyControl component = UnityEngine.Object.Instantiate(original, property.controlContainer, worldPositionStays: false).GetComponent<PropertyControl>();
			component.parentProperty = property;
			component.Setup();
			return component;
		}
	}
}
