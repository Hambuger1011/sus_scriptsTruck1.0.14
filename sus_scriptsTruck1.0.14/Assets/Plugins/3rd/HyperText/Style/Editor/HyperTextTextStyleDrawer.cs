/*
 * Text Style Editor
 */

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Candlelight.UI
{
	/// <summary>
	/// HyperText text style drawer.
	/// </summary>
	[CustomPropertyDrawer(typeof(HyperTextStyles.Text))]
	public class HyperTextTextStyleDrawer : HyperTextStyleDrawer
	{
		#region Labels
		private static readonly GUIContent s_ColorizationGuiContent = new GUIContent("Color", "Enable if instances of this style should wrap text in <color> tags.");
		private static readonly GUIContent s_FontStyleGuiContent = new GUIContent("Style", "Style to apply to the font face.");
		private static readonly GUIContent s_TagGuiContent = new GUIContent("Tag", "Unique name in the collection of styles used to reference style.");
        private static readonly GUIContent s_UnderlineContent = new GUIContent("Underline", "Underline used to reference style.");
        #endregion

        #region Serialized Properties
        private readonly Dictionary<string, SerializedProperty> m_FontStyle = new Dictionary<string, SerializedProperty>();
		private readonly Dictionary<string, SerializedProperty> m_ReplacementColor = new Dictionary<string, SerializedProperty>();
		private readonly Dictionary<string, SerializedProperty> m_ShouldReplaceColor = new Dictionary<string, SerializedProperty>();
		private readonly Dictionary<string, SerializedProperty> m_Tag = new Dictionary<string, SerializedProperty>();

        private readonly Dictionary<string, SerializedProperty> m_Underline = new Dictionary<string, SerializedProperty>();
        private readonly Dictionary<string, SerializedProperty> m_ShouldReplaceUnderlineColor = new Dictionary<string, SerializedProperty>();
        private readonly Dictionary<string, SerializedProperty> m_UnderlineColor = new Dictionary<string, SerializedProperty>();
        #endregion

        /// <summary>
        /// Gets the height of the property.
        /// </summary>
        /// <value>The height of the property.</value>
        protected override float PropertyHeight { get { return propertyHeight; } }
		/// <summary>
		/// Gets the size property name prefix.
		/// </summary>
		/// <value>The size property name prefix.</value>
		protected override string SizePropertyNamePrefix { get { return "m_TextStyle."; } }


		/// <summary>
		/// Displays the identifier field for this style.
		/// </summary>
		/// <param name="position">Position.</param>
		/// <param name="property">Property.</param>
		protected override void DisplayIdentifierField(Rect position, SerializedProperty property)
		{
			EditorGUI.PropertyField(position, m_Tag[property.propertyPath], s_TagGuiContent);
		}

		/// <summary>
		/// Initialize this instance.
		/// </summary>
		/// <param name="property">Property.</param>
		protected override void Initialize(SerializedProperty property)
		{
			base.Initialize(property);
			if (m_FontStyle.ContainsKey(property.propertyPath))
			{
				return;
            }
            m_Tag.Add(property.propertyPath, property.FindPropertyRelative("m_Tag"));
            m_FontStyle.Add(
				property.propertyPath,
				property.FindPropertyRelative(string.Format("{0}m_FontStyle", this.SizePropertyNamePrefix))
			);
			m_ReplacementColor.Add(
				property.propertyPath, 
				property.FindPropertyRelative(string.Format("{0}m_ReplacementColor", this.SizePropertyNamePrefix))
			);
			m_ShouldReplaceColor.Add(
				property.propertyPath, 
				property.FindPropertyRelative(string.Format("{0}m_ShouldReplaceColor", this.SizePropertyNamePrefix))
			);


            m_Underline.Add(
                property.propertyPath,
                property.FindPropertyRelative(string.Format("{0}m_Underline", this.SizePropertyNamePrefix))
            );
            m_ShouldReplaceUnderlineColor.Add(
                property.propertyPath,
                property.FindPropertyRelative(string.Format("{0}m_ShouldReplaceUnderlineColor", this.SizePropertyNamePrefix))
            );
            m_UnderlineColor.Add(
                property.propertyPath,
                property.FindPropertyRelative(string.Format("{0}m_UnderlineColor", this.SizePropertyNamePrefix))
            );
        }



        /// <summary>
        /// The height of the property.
        /// </summary>
        public static readonly float propertyHeight = 7f * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
        /// <summary>
        /// Displays the custom fields.
        /// </summary>
        protected override int DisplayCustomFields(Rect firstLinePosition, SerializedProperty property)
        {
            int numLines = 4;
            //font style
            float entireWidth = firstLinePosition.width;
            float entireX = firstLinePosition.x;
            EditorGUI.PropertyField(firstLinePosition, m_FontStyle[property.propertyPath], s_FontStyleGuiContent);

            //color
            firstLinePosition.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            firstLinePosition.width = EditorGUIUtility.labelWidth + 14f;
            EditorGUI.PropertyField(firstLinePosition, m_ShouldReplaceColor[property.propertyPath], s_ColorizationGuiContent);

            var tmpPos = firstLinePosition;
            tmpPos.x += EditorGUIUtility.labelWidth + EditorGUIX.StandardHorizontalSpacing + 14f;
            tmpPos.width = entireWidth - (firstLinePosition.x - entireX);
            EditorGUI.BeginDisabledGroup(!m_ShouldReplaceColor[property.propertyPath].boolValue);
            {
                EditorGUI.PropertyField(tmpPos, m_ReplacementColor[property.propertyPath], GUIContent.none);
            }
            EditorGUI.EndDisabledGroup();


            //Underline
            firstLinePosition.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(firstLinePosition, m_Underline[property.propertyPath], s_UnderlineContent);

            EditorGUI.BeginDisabledGroup(!m_Underline[property.propertyPath].boolValue);
            {
                firstLinePosition.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                tmpPos.y = firstLinePosition.y;
                EditorGUI.PropertyField(firstLinePosition, m_ShouldReplaceUnderlineColor[property.propertyPath], s_ColorizationGuiContent);
                EditorGUI.BeginDisabledGroup(!m_ShouldReplaceUnderlineColor[property.propertyPath].boolValue);
                {
                    EditorGUI.PropertyField(tmpPos, m_UnderlineColor[property.propertyPath], GUIContent.none);
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUI.EndDisabledGroup();

            return numLines;
        }
    }
}