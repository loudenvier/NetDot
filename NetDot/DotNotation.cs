using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetDot
{
    public static class DotNotation
    {
        public static IDictionary<string, object> Parse(this string text, IDictionary<string, object>? root = null)
            => Parse(EnumerateLines(text), root);

        public static IDictionary<string, object> Parse(this IEnumerable<string> lines, IDictionary<string, object>? root = null) {
            root ??= new Dictionary<string, object>();
            foreach (var line in lines)
                ParseInternal(line, root);
            return root;
        }

        private static void ParseInternal(string singleLine, IDictionary<string, object> property) {
            object current = property;
            int lastArrayIndex = 0;
            var (members, value) = SplitMembersFromValue(singleLine);
            for (var i = 0; i < members.Length; i++) {
                var isLast = i == members.Length - 1;
                var member = new Member(members[i]);
                var dict = current switch {
                    List<object?> list => list[lastArrayIndex]! as IDictionary<string, object>,
                    _ => (IDictionary<string, object>)current
                };
                if (dict is null) continue;
                if (member.IsArray) {
                    lastArrayIndex = member.Index;
                    List<object?> list;
                    if (dict.ContainsKey(member.Name)) {
                        list = (List<object?>)dict[member.Name];
                    } else {
                        dict[member.Name] = list = new List<object?>();
                    }
                    while (list.Count < member.Index + 1) list.Add(null);
                    if (isLast) {
                        list[member.Index] = value;
                    } else if (list[member.Index] is null) {
                        list[member.Index] = new Dictionary<string, object>();
                    }
                    current = list;
                } else {
                    if (isLast) {
                        dict[member.Name] = value;
                    } else {
                        if (!dict.ContainsKey(member.Name))
                            dict[member.Name] = new Dictionary<string, object>();
                        current = dict[member.Name];
                    }
                }
            }
        }

        static readonly char[] separator = { '=' };
        private static (string[], object) SplitMembersFromValue(string text) {
            // TODO: Allow caller to customize member/value separator
            var membersAndValue = text.Split(separator, 2);
            if (membersAndValue.Length != 2)
                throw new FormatException($"Text is not in dot notation format: {text}");
            //TODO: Process "value" to handle quoted strings, boolean and numeric values, etc.
            return (membersAndValue[0].Split('.'), membersAndValue[1]);
        }

        static readonly JsonSerializerSettings defaultSettings = new() {
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            NullValueHandling = NullValueHandling.Ignore,
        };
        public static T? Deserialize<T>(string text, JsonSerializerSettings? settings = null) {
            settings ??= defaultSettings;
            var parsed = Parse(text);
            return Deserialize<T>(parsed, settings);
        }
        public static T? Deserialize<T>(IDictionary<string, object> dict, JsonSerializerSettings? settings = null) {
            settings ??= defaultSettings;
            var json = JsonConvert.SerializeObject(dict, settings);
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        public static string Serialize(object? o, string prefix = "", DotNotationSettings? settings = null) {
            settings ??= DotNotationSettings.Default;
            string dot(string? s) => string.IsNullOrEmpty(s) ? "" : s + settings.DotConnector;
            if (o is null) return "";
            var sb = new StringBuilder();
            if (typeof(IDictionary).IsAssignableFrom(o.GetType())) {
                var dict = o as IDictionary;
                if (dict is not null) {
                    foreach (DictionaryEntry kvp in dict) {
                        if (kvp.Value is null) continue;
                        sb.Append(Serialize(kvp.Value, $"{prefix}[{kvp.Key}]", settings));
                    }
                }
            } else if (o.GetType().IsArray) {
                var arr = o as Array;
                for (int i = 0; i < arr?.Length; i++) {
                    var value = arr.GetValue(i);
                    if (value is null) continue;
                    sb.Append(Serialize(value, $"{prefix}[{i}]", settings));
                }
            } else if (o.GetType().IsClass && o is not string) {
                foreach (var prop in o.GetType().GetProperties()) {
                    var v = prop.GetValue(o);
                    if (v is not null)
                        sb.Append(Serialize(v, $"{dot(prefix)}{prop.Name}", settings));
                }
            } else {
                sb.Append(WriteEntry(prefix, o, settings)); // $"{prefix}={o}");
            }
            return sb.ToString();
        }

        private static string WriteEntry(string key, object value, DotNotationSettings s) =>
            $"{s.SurroundingTexts.opening}{WriteKey(key, s)}{s.SpacingAfterKey}{s.KeyValueSeparator}{s.SpacingBeforeValue}{WriteValue(value, s)}{s.SurroundingTexts.closing}{s.EntrySeparator}";
        private static string WriteKey(string key, DotNotationSettings s) 
            => s.UrlEncode ? Uri.EscapeDataString(key) : key;
        private static string WriteValue(object value, DotNotationSettings s) {
            var quote = s.QuoteValues || s.QuoteStrings && value is string ? $"{s.QuoteChar}" : "";
            value = value switch {
                bool b => b.ToString().ToLower().ToLower(),
                DateTime dt => dt.ToString(s.DateFormatString, s.Culture),
                DateTimeOffset dto => dto.ToString(s.DateFormatString, s.Culture),
                _ => string.Format(s.Culture, "{0}", value),
            };
            var textValue = s.TrimValues ? $"{value}".Trim(s.TrimChars) : $"{value}";
            var text = $"{quote}{textValue}{quote}";
            return s.UrlEncode ? Uri.EscapeDataString(text) : text ;
        }

        private static IEnumerable<string> EnumerateLines(string text) {
            if (text == null) yield break;
            using var reader = new StringReader(text);
            string? line;
            while ((line = reader.ReadLine()) != null)
                yield return line;
            yield break;
        }

        class Member
        {
            public Member(string text) {
                var parts = text.Trim(']').Split('[');
                Name = parts[0];
                IsArray = parts.Length == 2;
                if (IsArray)
                    Index = Convert.ToInt32(parts[1]);
            }
            public string Name { get; }
            public int Index { get; }
            public bool IsArray { get; }
            public override string ToString() => $"{Name}{(IsArray ? $"[{Index}]" : "")}";
        }
    }
}
