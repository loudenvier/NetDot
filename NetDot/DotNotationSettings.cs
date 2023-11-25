using System;
using System.Globalization;

namespace NetDot
{
    public class DotNotationSettings {
        public static DotNotationSettings Default { get; } = new();
        /// <summary>
        /// Specifies the connector to use between properties. Defaults to a dot ('.')
        /// </summary>
        public string DotConnector { get; set; } = ".";
        /// <summary>
        /// The separator to use between the key (full property path) and value. Defaults to "="
        /// </summary>
        public string KeyValueSeparator { get; set; } = "=";
        /// <summary>
        /// The quote character to use if either <see cref="QuoteStrings"/> or <see cref="QuoteValues"/> are set
        /// </summary>
        public char QuoteChar { get; set; } = '"';
        /// <summary>
        /// Adds it's content after the key (full properti name) and before the <see cref="KeyValueSeparator"/>
        /// </summary>
        public string SpacingAfterKey { get; set; } = "";
        /// <summary>
        /// Adds it's content after the <see cref="KeyValueSeparator"/> and before the actual value
        /// </summary>
        public string SpacingBeforeValue { get; set; } = "";
        /// <summary>
        /// The value used to separate each entrye. Defaults to a new line
        /// </summary>
        public string EntrySeparator { get; set; } = Environment.NewLine;
        /// <summary>
        /// Indicates that strings should be quoted
        /// </summary>
        public bool QuoteStrings { get; set; } = false;
        /// <summary>
        /// Indicates that all values should be quoted (overrides <see cref="QuoteStrings"/>
        /// </summary>
        public bool QuoteValues { get; set; } = false;
        /// <summary>
        /// Indicates that values should be trimmed before being written
        /// </summary>
        public bool TrimValues { get; set; } = false;
        /// <summary>
        /// The set of characters that will be trimmed if <see cref="TrimValues"/> is true
        /// </summary>
        public char[] TrimChars { get; set; } = { ' ' };
        /// <summary>
        /// Text to add around the entry
        /// </summary>
        public (string opening, string closing) SurroundingTexts { get; set; } = ("", "");
        /// <summary>
        /// Indicates that keys and values should be URL Encoded
        /// </summary>
        public bool UrlEncode { get; set; } = false;
        /// <summary>
        /// Allows customization of date and time serialization
        /// </summary>
        public string DateFormatString { get; set; } = @"yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";
        /// <summary>
        /// Controls serialization formats/rules for numbers and dates. Defaults to <see cref="CultureInfo.InvariantCulture"/>.
        /// </summary>
        public CultureInfo Culture { get;set; } = CultureInfo.InvariantCulture;

        /// <summary>
        /// Clones this instance copying all properties
        /// </summary>
        /// <returns>a new <see cref="DotNotationSettings"/> with the source properties copied</returns>
        public DotNotationSettings Clone() => new() {
            DotConnector = DotConnector,
            KeyValueSeparator = KeyValueSeparator,
            QuoteChar = QuoteChar,
            SpacingAfterKey = SpacingAfterKey,
            SpacingBeforeValue = SpacingBeforeValue,
            EntrySeparator = EntrySeparator,
            QuoteStrings = QuoteStrings,
            QuoteValues = QuoteValues,
            TrimValues = TrimValues,
            TrimChars = TrimChars,
            SurroundingTexts = SurroundingTexts,
            UrlEncode = UrlEncode,
            DateFormatString = DateFormatString,
            Culture = Culture,
        };

    }
}
