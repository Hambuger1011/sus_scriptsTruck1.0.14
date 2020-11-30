using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace Candlelight.UI
{
	[System.Serializable]
	public class HyperTextProcessor : System.IDisposable
	{
        #region regex

        /// <summary>
		/// Gets the name of the capture group used for a close tag in a piece of text.
		/// </summary>
		/// <value>The name of the capture group used for a close tag in a piece of text.</value>
		public static string CloseTagCaptureGroup { get { return "closeTag"; } }
        /// <summary>
        /// Gets the name of the capture group used for an open tag in a piece of text.
        /// </summary>
        /// <value>The name of the capture group used for an open tag in a piece of text.</value>
        public static string OpenTagCaptureGroup { get { return "openTag"; } }
        /// <summary>
        /// Gets the reserved tags.
        /// </summary>
        /// <value>The reserved tags.</value>
        public static ReadOnlyCollection<string> ReservedTags { get { return s_ReservedTags; } }
        /// <summary>
        /// Gets the name of the capture group used for text enclosed in a tag.
        /// </summary>
        /// <value>The name of the capture group used for text enclosed in a tag.</value>
        public static string TextCaptureGroup { get { return "text"; } }


        /// <summary>
        /// Gets the name of the capture group used for a tag's attribute value of interest.
        /// </summary>
        /// <value>The name of the capture group used for a tag's attribute value of interest.</value>
        private static string AttributeValueCaptureGroup { get { return "attributeValue"; } }
        /// <summary>
        /// Gets the name of the capture group used for a tag's class attribute value.
        /// </summary>
        /// <value>The name of the capture group used for a tag's class attribute value.</value>
        private static string ClassNameCaptureGroup { get { return "className"; } }


        /// <summary>
        /// A table of replacement regular expressions for keywords.
        /// </summary>
        private static readonly Dictionary<int, Regex> s_KeywordRegexTable = new Dictionary<int, Regex>();

        /*
         * 超连接格式
         * <a href="attributeValue" class="className">text</>
         */
        private static readonly Regex s_PostprocessedLinkTagRegex = new Regex(
            string.Format(
                "<a href\\s*=\\s*\"(?<{0}>.*?)\"(\\s+class\\s*=\\s*\"(?<{1}>.*?)\")?>(?<{2}>.*?)(?<{3}></a>)",
                AttributeValueCaptureGroup, ClassNameCaptureGroup, TextCaptureGroup, CloseTagCaptureGroup
            ),
            RegexOptions.Singleline | RegexOptions.IgnoreCase
        );
        /// <summary>
        /// A regular expression to extract the attribute value of a &lt;size&gt; tag or the size attribute of a
        /// &lt;quad&gt; tag.
        /// </summary>
        private static readonly Regex s_PostProcessedSizeAttributeRegex = new Regex(
            string.Format(
                @"(?<{0}><size\s*=\s*)(?<{1}>\d+)(?<{2}>>)|(?<{0}><quad\b[^>]*?\bsize=)(?<{1}>\d+)(?<{2}>[^>]*?>)",
                OpenTagCaptureGroup, AttributeValueCaptureGroup, CloseTagCaptureGroup
            ),
            RegexOptions.Singleline | RegexOptions.IgnoreCase
        );
        /// <summary>
        /// The base match pattern for any rich text tag in preprocessed text (used when supportRichText = true).
        /// </summary>
        private static readonly string s_PreprocessedAnyTagMatchPattern =
            "</?a\b.*?>|" +
            "<quad\b.*?>|" +
            "</?color\b.*?>|" +
            "</?i>|" +
            "</?b>|" +
            "</?size\b.*?>|" +
            "</?material\b.*?>";
        /// <summary>
        /// A regular expression to match only &lt;a&gt; tags in preprocessed text (used when supportRichText = false).
        /// </summary>
        private static readonly Regex s_PreprocessedLinkTagRegex =
            new Regex("</?a\b.*?>", RegexOptions.Singleline | RegexOptions.IgnoreCase);

#if USE_FONT_SCALE
        /*
         * <size=100%></size>
         */
        private static readonly Regex s_PreprocessedSizeTagRegex = new Regex(
            string.Format(
                "<size\\s*=\\s*(?<{0}>\\d*\\.?\\d+%?)>(?<{1}>.+?)</size>", AttributeValueCaptureGroup, TextCaptureGroup
            ),
            RegexOptions.Singleline | RegexOptions.IgnoreCase
        );
#endif

        /// <summary>
        /// A regular expression to extract a &lt;quad&gt; tag in text.
        /// </summary>
        private static readonly Regex s_QuadTagRegex = new Regex(
            string.Format("<quad class\\s*=\\s*\"(?<{0}>.+?)\"\\s*.*?/?>", ClassNameCaptureGroup),
            RegexOptions.IgnoreCase
        );
        /// <summary>
        /// Regular expressions for each custom tag.
        /// </summary>
        private static readonly Dictionary<string, Regex> s_TagRegexes = new Dictionary<string, Regex>();

        #endregion
        #region Data Types
        /// <summary>
        /// A class for storing information about a custom tag indicated in the text.
        /// </summary>
        public class CustomTag : TagCharacterData
		{
			/// <summary>
			/// Gets the style.
			/// </summary>
			/// <value>The style.</value>
			public HyperTextStyles.Text Style { get; private set; }

			/// <summary>
			/// Initializes a new instance of the <see cref="HyperTextProcessor.CustomTag"/> class.
			/// </summary>
			/// <param name="indexRange">Index range.</param>
			/// <param name="style">Style.</param>
			public CustomTag(IndexRange indexRange, HyperTextStyles.Text style) : base(indexRange)
			{
				this.Style = style;
			}

			/// <summary>
			/// Clone this instance.
			/// </summary>
			/// <returns>A clone of this instance.</returns>
			public override object Clone()
			{
				return new CustomTag((IndexRange)this.CharacterIndices.Clone(), this.Style);
			}
		}
		
		/// <summary>
		/// A structure for storing a keyword collection and its associated class. It is used to create associations
		/// between keyword collections and styles specified in the style sheet.
		/// </summary>
		[System.Serializable]
		public struct KeywordCollectionClass : IPropertyBackingFieldCompatible<KeywordCollectionClass>
		{

			#region Backing Fields
			[SerializeField]
			private string m_ClassName;

			#endregion
			
			/// <summary>
			/// Gets the name of the class.
			/// </summary>
			/// <value>The name of the class.</value>
			public string ClassName { get { return m_ClassName = m_ClassName ?? string.Empty; } }

            
			
			/// <summary>
			/// Clone this instance.
			/// </summary>
			/// <returns>A clone of this instance.</returns>
			public object Clone()
			{
				return this;
			}
            
			public override bool Equals(object obj)
			{
				return ObjectX.Equals(ref this, obj);
			}
            
			public bool Equals(KeywordCollectionClass other)
			{
				return GetHashCode() == other.GetHashCode();
			}
            
			
			/// <summary>
			/// Gets a hash value that is based on the values of the serialized properties of this instance.
			/// </summary>
			/// <returns>A hash value based on the values of the serialized properties on this instance.</returns>
			public int GetSerializedPropertiesHash()
			{
				return GetHashCode();
			}
		}

		/// <summary>
		/// A custom <see cref="UnityEngine.PropertyAttribute"/> to specify information for inspector labels on a
		/// <see cref="HyperTextProcessor.KeywordCollectionClass"/>.
		/// </summary>
		public class KeywordCollectionClassAttribute : PropertyAttribute
		{
			/// <summary>
			/// Gets the label.
			/// </summary>
			/// <value>The label.</value>
			public GUIContent Label { get; private set; }

			/// <summary>
			/// Initializes a new instance of the <see cref="HyperTextProcessor.KeywordCollectionClassAttribute"/>
			/// class.
			/// </summary>
			/// <param name="identifierLabel">Identifier label.</param>
			/// <param name="identifierDescription">Identifier description.</param>
			public KeywordCollectionClassAttribute(string identifierLabel, string identifierDescription)
			{
				this.Label = new GUIContent(
					identifierLabel, string.Format("{0} with which the collection is associated", identifierDescription)
				);
			}
		}
		
		/// <summary>
		/// A class for storing information about a link indicated in the text.
		/// </summary>
		public class Link : TagCharacterData
		{
			/// <summary>
			/// Gets the name of the class.
			/// </summary>
			/// <value>The name of the class.</value>
			public string ClassName { get; private set; }
			/// <summary>
			/// Gets the value of the <c>name</c> attribute.
			/// </summary>
			/// <value>The value of the <c>name</c> attribute.</value>
			public string Name { get; private set; }
			/// <summary>
			/// Gets or sets the style.
			/// </summary>
			/// <value>The style.</value>
			public HyperTextStyles.Link Style { get; private set; }

			/// <summary>
			/// Initializes a new instance of the <see cref="HyperTextProcessor.Link"/> class.
			/// </summary>
			/// <param name="linkName">Value of the link's <c>name</c> attribute.</param>
			/// <param name="className">Class name.</param>
			/// <param name="characterIndices">Character indices.</param>
			/// <param name="style">Style.</param>
			public Link(
				string linkName, string className, IndexRange characterIndices, HyperTextStyles.Link style
			) : base(characterIndices)
			{
				this.Name = linkName;
				this.ClassName = className;
				this.Style = style;
			}

			/// <summary>
			/// Clone this instance.
			/// </summary>
			/// <returns>A clone of this instance.</returns>
			public override object Clone()
			{
				return new Link(this.Name, this.ClassName, (IndexRange)this.CharacterIndices.Clone(), this.Style);
			}
		}
		
		/// <summary>
		/// A class for storing information about a quad indicated in the text.
		/// </summary>
		public class Quad : TagCharacterData
		{
			/// <summary>
			/// Gets the style.
			/// </summary>
			/// <value>The style.</value>
			public HyperTextStyles.Quad Style { get; private set; }

			/// <summary>
			/// Initializes a new instance of the <see cref="HyperTextProcessor.Quad"/> class.
			/// </summary>
			/// <param name="indexRange">Index range.</param>
			/// <param name="style">Style.</param>
			public Quad(IndexRange indexRange, HyperTextStyles.Quad style) : base(indexRange)
			{
				this.Style = style;
			}

			/// <summary>
			/// Clone this instance.
			/// </summary>
			/// <returns>A clone of this instance.</returns>
			public override object Clone()
			{
				return new Quad((IndexRange)this.CharacterIndices.Clone(), this.Style);
			}
		}
		
		/// <summary>
		/// A base class for storing data about the characters for a tag appearing in the text.
		/// </summary>
		public abstract class TagCharacterData : System.ICloneable
		{
			/// <summary>
			/// Gets or sets the character indices.
			/// </summary>
			/// <value>The character indices.</value>
			public IndexRange CharacterIndices { get; set; }

			/// <summary>
			/// Initializes a new instance of the <see cref="HyperTextProcessor.TagCharacterData"/>
			/// class.
			/// </summary>
			/// <param name="indexRange">Index range.</param>
			protected TagCharacterData(IndexRange indexRange)
			{
				this.CharacterIndices = indexRange;
			}

			/// <summary>
			/// Clone this instance.
			/// </summary>
			/// <returns>A clone of this instance.</returns>
			public abstract object Clone();
		}
		#endregion


		#region Backing Fields
		private static readonly ReadOnlyCollection<string> s_ReservedTags = new ReadOnlyCollection<string>(
			new [] { "a", "b", "color", "i", "material", "quad", "size" }
		);
		#endregion
        


		/// <summary>
		/// A value indicating whether or not m_ProcessedText is currently dirty.
		/// </summary>
		[System.NonSerialized]
		private bool m_IsDirty = true;

		#region Backing Fields
		[System.NonSerialized]
		private List<CustomTag> m_CustomTags = new List<CustomTag>();

		
		[System.NonSerialized]
		private List<Link> m_Links = new List<Link>();
		[SerializeField, PropertyBackingField]
		private string m_InputText = string.Empty;
		[SerializeField, PropertyBackingField]
		private Object m_InputTextSourceObject = null;

		[SerializeField, PropertyBackingField]
		private bool m_IsDynamicFontDesired = true;
		[SerializeField, PropertyBackingField]
		private bool m_IsRichTextDesired = true;
		[SerializeField, HideInInspector] // serialize this so editor undo/redo bypasses lazy evaluation
		private string m_OutputText = string.Empty;

		
		[System.NonSerialized]
		private List<Quad> m_Quads = new List<Quad>();
		[SerializeField, PropertyBackingField]
		private int m_ReferenceFontSize = 14;
		private float m_ScaleFactor = 1f;

		[SerializeField, PropertyBackingField]
		private HyperTextStyles m_Styles = null;

		
		#endregion

		#region Event Handlers
		

		/// <summary>
		/// Raises the styles changed event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		private void OnStylesChanged(HyperTextStyles sender)
		{
			SetDirty();
		}
		#endregion



		#region Public Properties

		/// <summary>
		/// Gets or sets the input text.
		/// </summary>
		/// <value>The input text.</value>
		public string InputText
		{
			get { return m_InputText; }
			set
			{
				if (m_InputText != value)
				{
					m_InputText = value;
					SetDirty();
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether dynamic font output is desired on this instance.
		/// </summary>
		/// <value>
		/// <see langword="true"/> if this dynamic font output is desired on this instance; otherwise,
		/// <see langword="false"/>.
		/// </value>
		public bool IsDynamicFontDesired
		{
			get { return m_IsDynamicFontDesired; }
			set
			{
				if (m_IsDynamicFontDesired != value)
				{
					m_IsDynamicFontDesired = value;
					SetDirty();
				}
			}
		}
		/// <summary>
		/// Gets a value indicating whether dynamic font output is enabled.
		/// </summary>
		/// <value><see langword="true"/> if dynamic font output is enabled; otherwise, <see langword="false"/>.</value>
		public bool IsDynamicFontEnabled { get { return m_IsDynamicFontDesired && m_IsRichTextDesired; } }
		/// <summary>
		/// Gets or sets a value indicating whether rich text is desired on this instance.
		/// </summary>
		/// <value>
		/// <see langword="true"/> if rich text is desired on this instance; otherwise, <see langword="false"/>.
		/// </value>
		public bool IsRichTextDesired
		{
			get { return m_IsRichTextDesired; }
			set
			{
				if (m_IsRichTextDesired != value)
				{
					m_IsRichTextDesired = value;
					SetDirty();
				}
			}
		}
		/// <summary>
		/// Gets a value indicating whether rich text is enabled on this instance.
		/// </summary>
		/// <value><see langword="true"/> if rich text is enabled; otherwise, <see langword="false"/>.</value>
		public bool IsRichTextEnabled { get { return m_IsRichTextDesired && m_Styles != null; } }
		/// <summary>
		/// Gets the output text.
		/// </summary>
		/// <value>The output text.</value>
		public string OutputText
		{
			get
			{
				ProcessInputText();
				return m_OutputText;
			}
		}
		/// <summary>
		/// Gets or sets the reference font size. It should correspond to the font size where OutputText will be sent.
		/// </summary>
		/// <value>The reference font size.</value>
		public int ReferenceFontSize
		{
			get { return m_ReferenceFontSize; }
			set
			{
				value = Mathf.Max(value, 0);
				if (m_ReferenceFontSize != value)
				{
					m_ReferenceFontSize = value;
					SetDirty();
				}
			}
		}
		/// <summary>
		/// Gets or sets the scale factor.
		/// </summary>
		/// <value>The scale factor.</value>
		public float ScaleFactor
		{
			get { return m_ScaleFactor; }
			set
			{
				if (m_ScaleFactor != value)
				{
					m_ScaleFactor = value;
					SetDirty();
				}
			}
		}

		/// <summary>
		/// Gets or sets the styles.
		/// </summary>
		/// <value>The styles.</value>
		public HyperTextStyles Styles
		{
			get { return m_Styles; }
			set
			{
				if (m_Styles == value)
				{
					return;
				}
				if (m_Styles != null)
				{
					m_Styles.Changed -= OnStylesChanged;
				}
				m_Styles = value;
				if (m_Styles != null)
				{
					m_Styles.Changed += OnStylesChanged;
				}
				SetDirty();
			}
		}

		/// <summary>
		/// Gets the custom tags extracted from the text.
		/// </summary>
		/// <param name="tags">Tags.</param>
		public void GetCustomTags(List<CustomTag> tags)
		{
			ProcessInputText();
			tags.Clear();
            //tags.AddRange(from customTag in m_CustomTags select (CustomTag)customTag.Clone());
            tags.AddRange(m_CustomTags);
		}
        

		/// <summary>
		/// Gets the links extracted from the text.
		/// </summary>
		/// <param name="links">Links.</param>
		public void GetLinks(List<Link> links)
		{
			ProcessInputText();
			links.Clear();
			links.AddRange(m_Links);
		}



		/// <summary>
		/// Gets the quads extracted from the text.
		/// </summary>
		/// <param name="quads">Quads.</param>
		public void GetQuads(List<Quad> quads)
		{
			ProcessInputText();
			quads.Clear();
			quads.AddRange(m_Quads);
		}

		/// <summary>
		/// Initializes this instance. Call this method when the provider is enabled, or this instance is otherwise
		/// first initialized.
		/// </summary>
		public void OnEnable()
		{
			if (m_Styles != null)
			{
				m_Styles.Changed -= OnStylesChanged;
				m_Styles.Changed += OnStylesChanged;
			}

			SetDirty();
		}
        

		#endregion

		#region System.IDisposable
		/// <summary>
		/// Releases all resource used by the <see cref="HyperTextProcessor"/> object.
		/// </summary>
		/// <remarks>
		/// Call <see cref="Dispose"/> when you are finished using the <see cref="HyperTextProcessor"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="HyperTextProcessor"/> in an unusable state. After calling 
		/// <see cref="Dispose"/>, you must release all references to the <see cref="HyperTextProcessor"/> so the
		/// garbage collector can reclaim the memory that the <see cref="HyperTextProcessor"/> was occupying.
		/// </remarks>
		public void Dispose()
		{

		}
		#endregion

		/// <summary>
		/// Gets the default link style.
		/// </summary>
		/// <value>The default link style.</value>
		private HyperTextStyles.Link DefaultLinkStyle
		{
			get { return m_Styles == null ? HyperTextStyles.Link.DefaultStyle : m_Styles.DefaultLinkStyle; }
		}
		/// <summary>
		/// Gets the font size to use.
		/// </summary>
		/// <value>The font size to use.</value>
		private int FontSizeToUse
		{
			get
			{
				return this.ReferenceFontSize;
			}
		}
		
		/// <summary>
		/// Gets the input text to use.
		/// </summary>
		/// <value>The input text to use.</value>
		private string InputTextToUse
		{
			get { return m_InputText ?? string.Empty; }
		}
		/// <summary>
		/// Gets the size of the font multiplied by the DPI.
		/// </summary>
		/// <value>The size of the font multiplied by the DPI.</value>
		private int ScaledFontSize { get { return (int)(this.FontSizeToUse * this.ScaleFactor); } }
		
		/// <summary>
		/// Gets a version of the quad tag corresponding to the supplied Match with all of its arguments injected.
		/// </summary>
		/// <returns>The postprocessed quad tag corresponding to the supplied Match.</returns>
		/// <param name="quadTagMatch">Quad tag match.</param>
		/// <param name="quadTemplates">The list of quad styles specified on the styles object.</param>
		private string GetPostprocessedQuadTag(Match quadTagMatch)
		{
			string quadName = quadTagMatch.Groups[ClassNameCaptureGroup].Value;
			string linkOpenTag = "";
			float sizeScalar = 1f;
			float width, height;
			Vector4 padding;
			float aspect = 1f;

            var quad = HyperTextUtils.LoadStyleQuad(quadName);
			//if (templateIndex >= 0)
			{
				if (!string.IsNullOrEmpty(quad.LinkId))
				{
					linkOpenTag = string.Format(
                        "<a href=\"{0}\"{1}>",
                        quad.LinkId,
						string.IsNullOrEmpty(quad.LinkClassName) ?
							"" : string.Format(" class=\"{0}\"", quad.LinkClassName)
					);
				}
				if (quad.Sprite != null)
				{
					padding = UnityEngine.Sprites.DataUtility.GetPadding(quad.Sprite);
					Rect rect = quad.Sprite.rect;
					width = rect.width - padding.z - padding.x;
					height = rect.height - padding.w - padding.y;
					aspect = height == 0f ? 0f : width / height;
				}
				sizeScalar = quad.SizeScalar;
			}
			return string.Format(
				"{0}<size={1}><quad class=\"{2}\" width={3}></size>{4}",
				linkOpenTag,
				sizeScalar * this.ScaledFontSize,
				quadName,
				aspect,
				string.IsNullOrEmpty(linkOpenTag) ? "" : "</a>"
			);
		}
        
		/// <summary>
		/// Sets this instance dirty in order to force a became dirty callback.
		/// </summary>
		private void SetDirty()
		{
			m_IsDirty = true;
		}
        
        
        /// <summary>
        /// Processes the input text.
        /// </summary>
        private void ProcessInputText()
        {
            // early out if already up to date
            if (!m_IsDirty)
            {
                return;
            }
            //using (var cascadedQuadStyles = new ListPool<HyperTextStyles.Quad>.Scope())
            {
                using (var processedIndexRangesAndScalars = new DictPool<IndexRange, float>.Scope())
                {
                    string textCache;
                    using (StringX.StringBuilderScope sb = new StringX.StringBuilderScope())
                    {
                        // initialize variables used throughout this method
                        int indexInRawString = 0;
                        using (var linkStyles = new DictPool<string, HyperTextStyles.Link>.Scope())
                        {
                            using (var customTags = new DictPool<string, HyperTextStyles.Text>.Scope())
                            {
                                if (m_Styles != null)
                                {
                                    using (var styles = new ListPool<HyperTextStyles.Text>.Scope())
                                    {
                                        m_Styles.GetCascadedCustomTextStyles(styles.List);
                                        for (int i = 0; i < styles.List.Count; ++i)
                                        {
                                            if (
                                                !string.IsNullOrEmpty(styles.List[i].Tag) &&
                                                !customTags.Dict.ContainsKey(styles.List[i].Tag)
                                            )
                                            {
                                                customTags.Dict.Add(styles.List[i].Tag, styles.List[i]);
                                            }
                                        }
                                    }
                                }
                                // insert tags in text for words present in keyword collections
                                textCache = this.InputTextToUse;
                                // if rich text is enabled, substitute quad arguments, discrete sizes, and custom tag styles into text
                                //if (m_Styles != null)
                                //{
                                //    m_Styles.GetCascadedQuadStyles(cascadedQuadStyles.List);
                                //}
                                m_CustomTags.Clear();
                                m_Quads.Clear();
                                if (this.IsRichTextEnabled)
                                {
                                    textCache = ProcessInputText_Quad(textCache);
                                    textCache = ProcessInputText_CustomTag(textCache, customTags.Dict, processedIndexRangesAndScalars.Dict);
                                    textCache = ProcessInputText_Link(textCache, linkStyles.Dict);
                                }
                            }

#region 移除 <a />
                            // remove <a> tags from processed text and record the link character indices
                            string className;
                            string textCapture;
                            m_Links.Clear();
                            foreach (Match match in s_PostprocessedLinkTagRegex.Matches(textCache))
                            {
                                // append everything since last append
                                sb.StringBuilder.Append(
                                    textCache.Substring(indexInRawString, match.Index - indexInRawString)
                                );
                                // get link class and style from match
                                HyperTextStyles.Link linkStyle = this.DefaultLinkStyle;
                                className = match.Groups[ClassNameCaptureGroup].Value;
                                if (
                                    match.Groups[ClassNameCaptureGroup].Success &&
                                    linkStyles.Dict.ContainsKey(match.Groups[ClassNameCaptureGroup].Value)
                                )
                                {
                                    linkStyle = linkStyles.Dict[className];
                                }
                                // create the result for the substitution
                                RichTextStyle textStyle = this.IsDynamicFontEnabled ?
                                    linkStyle.TextStyle : linkStyle.TextStyle.NonDynamicVersion;
                                string openTag = textStyle.ToStartTag(this.ScaledFontSize);
                                string closeTag = textStyle.ToEndTag();
                                textCapture = match.Groups[TextCaptureGroup].Value;
                                string result = textCapture;
                                if (this.IsRichTextEnabled)
                                {
                                    result = string.Format("{0}{1}{2}", openTag, textCapture, closeTag);
                                }
                                // append substitution
                                sb.StringBuilder.Append(result);
                                indexInRawString = match.Index + match.Length;
                                // store the data for the link
                                int startPosition = sb.StringBuilder.Length -
                                    (this.IsRichTextEnabled ? closeTag.Length : 0) -
                                    textCapture.Length;
                                Link newLink = new Link(
                                    match.Groups[AttributeValueCaptureGroup].Value,
                                    className,
                                    new IndexRange(startPosition, startPosition + textCapture.Length - 1),
                                    linkStyle
                                );
                                m_Links.Add(newLink);
                                // offset existing index ranges as needed
                                using (DictPool<IndexRange, int>.Scope offsets = new DictPool<IndexRange, int>.Scope())
                                {
                                    // add close tag first in case it shifts range backward
                                    offsets.Dict.Add(
                                        new IndexRange(match.Groups[CloseTagCaptureGroup].Index, textCache.Length),
                                        closeTag.Length - match.Groups[CloseTagCaptureGroup].Length
                                    );
                                    offsets.Dict.Add(
                                        // start range one after match so that start indices of enclosing tags aren't affected
                                        new IndexRange(match.Index + 1, match.Groups[CloseTagCaptureGroup].Index - 1),
                                        openTag.Length - (match.Groups[TextCaptureGroup].Index - match.Index)
                                    );
                                    foreach (IndexRange range in processedIndexRangesAndScalars.Dict.Keys)
                                    {
                                        range.Offset(offsets.Dict);
                                    }
                                }
                            }
#endregion
                        }
                        sb.StringBuilder.Append(
                            textCache.Substring(indexInRawString, textCache.Length - indexInRawString)
                        );
                        m_OutputText = sb.StringBuilder.ToString();
                    }
                    // pull out data for quads and finalize sizes if rich text is enabled
                    if (this.IsRichTextEnabled)
                    {
                        // multiply out overlapping sizes if dynamic font is enabled
                        if (this.IsDynamicFontEnabled)
                        {
                            foreach (Link link in m_Links)
                            {
                                processedIndexRangesAndScalars.Dict.Add(
                                    link.CharacterIndices, link.Style.TextStyle.SizeScalar
                                );
                            }
                            foreach (KeyValuePair<IndexRange, float> rangeScalar in processedIndexRangesAndScalars.Dict)
                            {
                                if (rangeScalar.Value <= 0f || rangeScalar.Value == 1f)
                                {
                                    continue;
                                }
                                string segment =
                                    m_OutputText.Substring(rangeScalar.Key.StartIndex, rangeScalar.Key.Count);
                                int oldLength = segment.Length;
                                if (s_PostProcessedSizeAttributeRegex.IsMatch(segment))
                                {
                                    using (StringX.StringBuilderScope sb = new StringX.StringBuilderScope())
                                    {
                                        sb.StringBuilder.Append(m_OutputText.Substring(0, rangeScalar.Key.StartIndex));
                                        segment = s_PostProcessedSizeAttributeRegex.Replace(
                                            segment,
                                            match => string.Format(
                                                "{0}{1}{2}",
                                                match.Groups[OpenTagCaptureGroup].Value,
                                                (int)(
                                                    rangeScalar.Value *
                                                    float.Parse(match.Groups[AttributeValueCaptureGroup].Value)
                                                ),
                                                match.Groups[CloseTagCaptureGroup].Value
                                            )
                                        );
                                        sb.StringBuilder.Append(segment);
                                        sb.StringBuilder.Append(m_OutputText.Substring(rangeScalar.Key.EndIndex + 1));
                                        m_OutputText = sb.StringBuilder.ToString();
                                    }
                                    int delta = segment.Length - oldLength;
                                    if (delta != 0)
                                    {
                                        using (
                                            DictPool<IndexRange, int>.Scope offsets =
                                            new DictPool<IndexRange, int>.Scope()
                                        )
                                        {
                                            offsets.Dict.Add(rangeScalar.Key, delta);
                                            foreach (IndexRange range in processedIndexRangesAndScalars.Dict.Keys)
                                            {
                                                if (range == rangeScalar.Key)
                                                {
                                                    continue;
                                                }
                                                range.Offset(offsets.Dict);
                                            }
                                        }
                                        rangeScalar.Key.EndIndex += delta;
                                    }
                                }
                            }
                        }
                        // pull out quad data
                        string quadName;
                        bool isQuadGeomAtEndOfTag = UnityVersion.Current < new UnityVersion(5, 3, 0);
                        foreach (Match match in s_QuadTagRegex.Matches(m_OutputText))
                        {
                            // add new quad data to list if its class is known
                            quadName = match.Groups[ClassNameCaptureGroup].Value;
                            var quad = HyperTextUtils.LoadStyleQuad(quadName);
                            int quadGeomIndex = match.Index;
                            if (isQuadGeomAtEndOfTag)
                            {
                                quadGeomIndex += match.Length - 1;
                            }

                            m_Quads.Add(
                                new Quad(
                                    new IndexRange(quadGeomIndex, quadGeomIndex),
                                    quad
                                )
                            );
                        }
                    }
                }
            }
            m_CustomTags.Sort((x, y) => x.CharacterIndices.StartIndex.CompareTo(y.CharacterIndices.StartIndex));
            m_Links.Sort((x, y) => x.CharacterIndices.StartIndex.CompareTo(y.CharacterIndices.StartIndex));
            m_Quads.Sort((x, y) => x.CharacterIndices.StartIndex.CompareTo(y.CharacterIndices.StartIndex));
            m_IsDirty = false;
        }



        private string ProcessInputText_CustomTag(string textCache, Dictionary<string, HyperTextStyles.Text> customTags, Dictionary<IndexRange, float> processedIndexRangesAndScalars)
        {
            // substitute text styles in for custom tags
            string tag;
            foreach (HyperTextStyles.Text style in customTags.Values)
            {
                RichTextStyle textStyle = this.IsDynamicFontEnabled ?
                    style.TextStyle : style.TextStyle.NonDynamicVersion;
                tag = style.Tag;
                if (!s_TagRegexes.ContainsKey(tag))
                {
                    s_TagRegexes[tag] = new Regex(
                        string.Format(
                            "(?<{0}><{1}>)(?<{2}>.+?)(?<{3}></{1}>)",
                            OpenTagCaptureGroup,
                            Regex.Escape(tag),
                            TextCaptureGroup,
                            CloseTagCaptureGroup
                        ), RegexOptions.Singleline | RegexOptions.IgnoreCase
                    );
                }
                while (s_TagRegexes[tag].IsMatch(textCache))
                {
                    textCache = s_TagRegexes[tag].Replace(
                        textCache,
                        delegate (Match match)
                        {
                            string openTag = textStyle.ToStartTag(ScaledFontSize);
                            string segment = match.Groups[TextCaptureGroup].Value;
                            string closeTag = textStyle.ToEndTag();
                            IndexRange characterIndices = new IndexRange(
                                match.Index + openTag.Length,
                                match.Index + openTag.Length + segment.Length - 1
                            );
                            using (var offsets = new DictPool<IndexRange, int>.Scope())
                            {
                                int openTagDelta = openTag.Length - match.Groups[OpenTagCaptureGroup].Length;
                                offsets.Dict.Add(
                                    // start one after match so start indices of enclosing tags aren't affected
                                    new IndexRange(
                                        match.Index + 1,
                                        match.Groups[CloseTagCaptureGroup].Index - 1
                                    ),
                                    openTagDelta
                                );
                                offsets.Dict.Add(
                                    new IndexRange(
                                        openTagDelta + match.Groups[CloseTagCaptureGroup].Index,
                                        openTagDelta + textCache.Length
                                    ),
                                    closeTag.Length - match.Groups[CloseTagCaptureGroup].Length
                                );
                                foreach (IndexRange range in processedIndexRangesAndScalars.Keys)
                                {
                                    range.Offset(offsets.Dict);
                                }
                            }
                            processedIndexRangesAndScalars.Add(
                                characterIndices, style.TextStyle.SizeScalar
                            );
                            m_CustomTags.Add(
                                new CustomTag(characterIndices, customTags[style.Tag])
                            );
                            return string.Format("{0}{1}{2}", openTag, segment, closeTag);
                        },
                        1 // only replace first instance so indices are properly set for any subsequent matches
                    );
                }
            }
            return textCache;
        }

        private string ProcessInputText_Link(string textCache, Dictionary<string, HyperTextStyles.Link> linkStyles)
        {
            // collect link styles
            using (var styles = new ListPool<HyperTextStyles.LinkSubclass>.Scope())
            {
                this.Styles.GetCascadedLinkStyles(styles.List);
                for (int i = 0; i < styles.List.Count; ++i)
                {
                    if (
                        !string.IsNullOrEmpty(styles.List[i].ClassName) &&
                        !linkStyles.ContainsKey(styles.List[i].ClassName)
                    )
                    {
                        linkStyles.Add(styles.List[i].ClassName, styles.List[i].Style);
                    }
                }
            }
            return textCache;
        }

        private string ProcessInputText_Quad(string textCache)
        {
            // 替换<quad>内容
            textCache = s_QuadTagRegex.Replace(
                textCache, 
                match => {
                    return GetPostprocessedQuadTag(match);
            });

#if USE_FONT_SCALE
            // 替换<size>内容
            textCache = s_PreprocessedSizeTagRegex.Replace(
                textCache,
                match => string.Format(
                    "<size={0}>{1}</size>",
                    match.Groups[AttributeValueCaptureGroup].Value.EndsWith("%") ?
                        (int)(
                            float.Parse(
                                match.Groups[AttributeValueCaptureGroup].Value.Substring(
                                    0, match.Groups[AttributeValueCaptureGroup].Value.Length - 1
                                )
                            ) * this.ScaledFontSize * 0.01f
                        ) : (
                            (int)float.Parse(
                                match.Groups[AttributeValueCaptureGroup].Value
                            ) > 0 ?
                                (int)float.Parse(
                                    match.Groups[AttributeValueCaptureGroup].Value
                                ) : this.ScaledFontSize
                        ),
                        match.Groups[TextCaptureGroup].Value
                )
            );
#endif

            return textCache;
        }
#region Obsolete
        [System.Obsolete("Use HyperTextProcessor.GetCustomTags(List<CustomTag>", true)]
		public void GetCustomTags(ref List<CustomTag> tags) {}
		[System.Obsolete("Use HyperTextProcessor.GetLinkKeywordCollections(List<KeywordCollectionClass>", true)]
		public void GetLinkKeywordCollections(ref List<KeywordCollectionClass> collections) {}
		[System.Obsolete("Use HyperTextProcessor.GetLinks(List<Link>", true)]
		public void GetLinks(ref List<Link> links) {}
		[System.Obsolete("Use HyperTextProcessor.GetQuadKeywordCollections(List<KeywordCollectionClass>", true)]
		public void GetQuadKeywordCollections(ref List<KeywordCollectionClass> collections) {}
		[System.Obsolete("Use HyperTextProcessor.GetQuads(List<Quad>", true)]
		public void GetQuads(ref List<Quad> quads) {}
		[System.Obsolete("Use HyperTextProcessor.GetTagKeywordCollections(List<KeywordCollectionClass>", true)]
		public void GetTagKeywordCollections(ref List<KeywordCollectionClass> collections) {}
		[System.Obsolete("Use HyperTextProcessor.SetLinkKeywordCollections(IList<KeywordCollectionClass>", true)]
		public void SetLinkKeywordCollections(IEnumerable<KeywordCollectionClass> value) {}
		[System.Obsolete("Use HyperTextProcessor.SetQuadKeywordCollections(IList<KeywordCollectionClass>", true)]
		public void SetQuadKeywordCollections(IEnumerable<KeywordCollectionClass> value) {}
		[System.Obsolete("Use HyperTextProcessor.SetTagKeywordCollections(IList<KeywordCollectionClass>", true)]
		public void SetTagKeywordCollections(IEnumerable<KeywordCollectionClass> value) {}
#endregion
	}
}