﻿// 
// HyperTextProcessorDrawer.cs
// 
// Copyright (c) 2014-2016, Candlelight Interactive, LLC
// All rights reserved.
// 
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://download.unity3d.com/assetstore/customer-eula.pdf
// 
// This file contains a custom property drawer for HyperTextProcessor.

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Candlelight.UI
{
	/// <summary>
	/// A custom property drawer for marked <see cref="HyperTextProcessor.KeywordCollectionClass"/> fields.
	/// </summary>
	[CustomPropertyDrawer(typeof(HyperTextProcessor.KeywordCollectionClassAttribute))]
	public class KeywordCollectionClassDrawer : PropertyDrawer
	{
		#region Labels
		private static readonly GUIContent s_CollectionLabel = new GUIContent(
			"Collection", "Specifies the collection of keywords that should have the specified style applied."
		);
		#endregion

		/// <summary>
		/// Gets the <see cref="HyperTextProcessor.KeywordCollectionClassAttribute"/>.
		/// </summary>
		/// <value>The <see cref="HyperTextProcessor.KeywordCollectionClassAttribute"/>.</value>
		private HyperTextProcessor.KeywordCollectionClassAttribute Attribute
		{
			get { return this.attribute as HyperTextProcessor.KeywordCollectionClassAttribute; }
		}

		/// <summary>
		/// Raises the GUI event.
		/// </summary>
		/// <param name="position">Position.</param>
		/// <param name="property">Property.</param>
		/// <param name="label">Label.</param>
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			position.height = EditorGUIUtility.singleLineHeight;
			float collectionWidth = position.width * 0.6f - EditorGUIX.StandardHorizontalSpacing * 0.5f;
			float classWidth = position.width - collectionWidth - EditorGUIX.StandardHorizontalSpacing;
			position.width = collectionWidth;
			float oldLabelWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 60f;
			EditorGUI.PropertyField(position, property.FindPropertyRelative("m_Collection"), s_CollectionLabel);
			position.x += position.width + EditorGUIX.StandardHorizontalSpacing;
			position.width = classWidth;
			EditorGUIUtility.labelWidth = 40f;
			EditorGUI.PropertyField(position, property.FindPropertyRelative("m_ClassName"), this.Attribute.Label);
			EditorGUIUtility.labelWidth = oldLabelWidth;
		}
	}

	/// <summary>
	/// A custom property drawer for <see cref="HyperTextProcessor"/> objects.
	/// </summary>
	[CustomPropertyDrawer(typeof(HyperTextProcessor))]
	public class HyperTextProcessorDrawer : PropertyDrawer
	{
		#region Shared Allocations
		private static List<HyperTextStyles.LinkSubclass> s_CascadedLinkStyles =
			new List<HyperTextStyles.LinkSubclass>(64);
		private static List<HyperTextStyles.Quad> s_CascadedQuadStyles = new List<HyperTextStyles.Quad>(64);
		private static List<HyperTextStyles.Text> s_CascadedTextStyles = new List<HyperTextStyles.Text>(64);
		#endregion
		#region Labels
		private static readonly GUIContent s_DynamicFontLabel = new GUIContent(
			"Dynamic Font",
			"Specifies whether <size> tags should be generated for styles. " +
			"Disable if the destination uses a non-dynamic font"
		);
		private static readonly GUIContent s_LinkKeywordIdentifierGUIContent =
			new GUIContent("Class", "Optional class name for custom <a> style with which collection is associated.");
		private static readonly GUIContent s_QuadKeywordIdentifierGUIContent =
			new GUIContent("Class", "Class name for the <quad> style with which this collection is associated.");
		private static readonly GUIContent s_RichTextLabel =
			new GUIContent("Output Rich Text", "Disable if the destination does not support rendering rich text.");
		private static readonly GUIContent s_TagKeywordIdentifierGUIContent =
			new GUIContent("Tag", "Tag name for the custom text style with which this collection is associated.");
		#endregion
		/// <summary>
		/// An empty idenfier collection.
		/// </summary>
		private static readonly string[] s_EmptyIdentifierCollection = new string[0];
        

		/// <summary>
		/// Raises the draw keyword collection class entry event.
		/// </summary>
		/// <param name="position">Position.</param>
		/// <param name="element">Element in the list.</param>
		/// <param name="identifierGUIContent">Identifier GUI content.</param>
		/// <param name="existingIdentifierNames">Existing identifier names.</param>
		/// <param name="defaultStatus">Default status.</param>
		/// <param name="missingStyleDescriptor">Missing style descriptor.</param>
		/// <param name="styles">Styles assigned to the <see cref="Candlelight.UI.HyperTextProcessor"/>.</param>
		/// <param name="assignedCollections">
		/// All <see cref="Candlelight.KeywordCollection"/> objects assigned to the
		/// <see cref="Candlelight.UI.HyperTextProcessor"/>.
		/// </param>
		private static void OnDrawKeywordCollectionClassEntry(
			Rect position,
			SerializedProperty element,
			GUIContent identifierGUIContent,
			IEnumerable<string> existingIdentifierNames,
			ValidationStatus defaultStatus,
			string missingStyleDescriptor,
			HyperTextStyles styles
			//IEnumerable<KeywordCollection> assignedCollections
		)
		{
			SerializedProperty collectionProperty = element.FindPropertyRelative("m_Collection");
			ValidationStatus collectionStatus = ValidationStatus.Warning;
			//if (collectionProperty.objectReferenceValue != null)
			//{
			//	collectionStatus =
			//		assignedCollections.Count(item => item == collectionProperty.objectReferenceValue) > 1 ?
			//			ValidationStatus.Error : ValidationStatus.Okay;
			//}
			string collectionTooltip;
			switch (collectionStatus)
			{
			case ValidationStatus.Error:
				collectionTooltip = "Specified keyword collection used for multiple different styles on this object.";
				break;
			case ValidationStatus.Warning:
				collectionTooltip = "Assign a keyword collection to automatically apply this style to keywords.";
				break;
			default:
				collectionTooltip = string.Empty;
				break;
			}
			string identifierName = element.FindPropertyRelative("m_ClassName").stringValue;
			ValidationStatus identifierStatus = defaultStatus;
			string identifierTooltip = string.Empty;
			if (!string.IsNullOrEmpty(identifierName))
			{
				if (styles == null)
				{
					identifierTooltip =
						"No styles assigned to this object. Keywords from this collection will use default style";
					identifierStatus = ValidationStatus.Warning;
				}
				else
				{
					int matches = existingIdentifierNames.Count(existingId => existingId == identifierName);
					if (matches == 1)
					{
						identifierStatus = ValidationStatus.Okay;
					}
					else
					{
						identifierStatus = ValidationStatus.Error;
						identifierTooltip = string.Format(
							"No custom {0} {1} found in {2}.", missingStyleDescriptor, identifierName, styles.name
						);
					}
				}
			}
			bool useCollectionStatus = collectionStatus >= identifierStatus;
			EditorGUIX.DisplayPropertyFieldWithStatus(
				position,
				element,
				useCollectionStatus ? collectionStatus : identifierStatus,
				null,
				true,
				useCollectionStatus ? collectionTooltip : identifierTooltip
			);
		}

		/// <summary>
		/// Raises the draw link keyword collections entry event.
		/// </summary>
		/// <param name="position">Position.</param>
		/// <param name="index">Index.</param>
		/// <param name="hyperTextProcessor">
		/// A SerializedProperty representation of a <see cref="Candlelight.UI.HyperTextProcessor"/>.
		/// </param>
		/// <param name="getAllAssignedCollections">
		/// A method to get all <see cref="Candlelight.KeywordCollection"/> objects assigned to the
		/// <see cref="Candlelight.UI.HyperTextProcessor"/>.
		/// </param>
		public static void OnDrawLinkKeywordCollectionsEntry(
			Rect position,
			int index,
			SerializedProperty hyperTextProcessor
			//System.Func<IEnumerable<KeywordCollection>> getAllAssignedCollections
		)
		{
			HyperTextStyles styles =
				hyperTextProcessor.FindPropertyRelative("m_Styles").objectReferenceValue as HyperTextStyles;
			if (styles != null)
			{
				styles.GetCascadedLinkStyles(s_CascadedLinkStyles);
			}
			//OnDrawKeywordCollectionClassEntry(
			//	position,
			//	hyperTextProcessor.FindPropertyRelative("m_LinkKeywordCollections").GetArrayElementAtIndex(index),
			//	s_LinkKeywordIdentifierGUIContent,
			//	styles == null ?
			//		s_EmptyIdentifierCollection : from style in s_CascadedLinkStyles select style.ClassName,
			//	ValidationStatus.Info,
			//	"link style with class name",
			//	styles,
			//	getAllAssignedCollections()
			//);
		}
        
		/// <summary>
		/// Link collections for each inspected property.
		/// </summary>
		private Dictionary<string, ReorderableList> m_LinkCollections = new Dictionary<string, ReorderableList>();
		/// <summary>
		/// Quad collections for each inspected property.
		/// </summary>
		private Dictionary<string, ReorderableList> m_QuadCollections = new Dictionary<string, ReorderableList>();
		/// <summary>
		/// Tag collections for each inspected property.
		/// </summary>
		private Dictionary<string, ReorderableList> m_TagCollections = new Dictionary<string, ReorderableList>();

		/// <summary>
		/// Initialize this instance.
		/// </summary>
		/// <param name="property">Property.</param>
		private void Initialize(SerializedProperty property)
		{

			if (!m_LinkCollections.ContainsKey(property.propertyPath))
			{
				ReorderableList list = new ReorderableList(
					property.serializedObject, property.FindPropertyRelative("m_LinkKeywordCollections")
				);
				
				string displayName = list.serializedProperty.displayName;
				list.drawHeaderCallback = (position) => EditorGUI.LabelField(position, displayName);
				m_LinkCollections.Add(property.propertyPath, list);
			}
			
		}

		/// <summary>
		/// Gets the height of the property.
		/// </summary>
		/// <returns>The property height.</returns>
		/// <param name="property">Property.</param>
		/// <param name="label">Label.</param>
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			Initialize(property);
			return m_LinkCollections[property.propertyPath].GetHeight() + EditorGUIUtility.standardVerticalSpacing +
				m_QuadCollections[property.propertyPath].GetHeight() + EditorGUIUtility.standardVerticalSpacing +
				m_TagCollections[property.propertyPath].GetHeight() + EditorGUIUtility.standardVerticalSpacing +
				(EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 6f;
		}

		/// <summary>
		/// Raises the GUI event.
		/// </summary>
		/// <param name="position">Position.</param>
		/// <param name="property">Property.</param>
		/// <param name="label">Label.</param>
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			Initialize(property);
			SerializedProperty inputText = property.FindPropertyRelative("m_InputText");
			SerializedProperty styles = property.FindPropertyRelative("m_Styles");
			SerializedProperty supportRichText = property.FindPropertyRelative("m_IsRichTextDesired");
			Rect entirePosition = position;
			position.height = (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2f;
			EditorGUI.PropertyField(position, inputText);
			position.x = entirePosition.x;
			position.width = entirePosition.width;
			position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
			position.height = EditorGUIUtility.singleLineHeight;
			EditorGUIX.DisplayScriptableObjectPropertyFieldWithButton<HyperTextStyles>(position, styles);
			position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
			HyperTextEditor.DisplayOverridableProperty(
				position,
				property.FindPropertyRelative("m_ReferenceFontSize"),
				property.FindPropertyRelative("m_ShouldOverrideStylesFontSize"),
				styles
			);
			position.x = entirePosition.x;
			position.width = entirePosition.width;
			position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
			EditorGUI.PropertyField(position, supportRichText, s_RichTextLabel);
			position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
			++EditorGUI.indentLevel;
			EditorGUI.BeginDisabledGroup(!supportRichText.boolValue);
			{
				EditorGUI.PropertyField(
					position, property.FindPropertyRelative("m_IsDynamicFontDesired"), s_DynamicFontLabel
				);
			}
			EditorGUI.EndDisabledGroup();
			--EditorGUI.indentLevel;
			position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
			position.height = m_LinkCollections[property.propertyPath].GetHeight();
			m_LinkCollections[property.propertyPath].DoList(position);
			position.y += position.height;
			position.height = m_TagCollections[property.propertyPath].GetHeight();
			m_TagCollections[property.propertyPath].DoList(position);
			position.y += position.height;
			position.height = m_QuadCollections[property.propertyPath].GetHeight();
			m_QuadCollections[property.propertyPath].DoList(position);
		}
	}
}