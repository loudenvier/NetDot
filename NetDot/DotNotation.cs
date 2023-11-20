﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

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

        static readonly JsonSerializerSettings defaultSettings = new() {
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            NullValueHandling = NullValueHandling.Ignore,
        };
        public static T? Deserialize<T>(this string text, JsonSerializerSettings? settings = null) {
            settings ??= defaultSettings;
            var parsed = Parse(text);
            var json = JsonConvert.SerializeObject(parsed, settings);
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        private static void ParseInternal(string singleLine, IDictionary<string, object> property) {
            var (members, value) = SplitMembersFromValue(singleLine);
            for (var i = 0; i < members.Length; i++) {
                var isLast = i == members.Length - 1;
                var member = new Member(members[i]);
                var memberProp = CreateMemberProp(property, member);
                if (member.IsArray) {
                    StoreArrayItem((List<object?>)memberProp, member, isLast ? value : property);
                } else {
                    if (isLast) {
                        property[member.Name] = value;
                    } else {
                        property = (IDictionary<string, object>)memberProp;
                    }
                }
            }
        }

        static char[] separator = { '=' };
        private static (string[], object) SplitMembersFromValue(string text) {
            // TODO: Allow caller to customize member/value separator
            var membersAndValue = text.Split(separator, 2);
            if (membersAndValue.Length != 2)
                throw new FormatException($"Text is not in dot notation format: {text}");
            //TODO: Process "value" to handle quoted strings, boolean and numeric values, etc.
            return (membersAndValue[0].Split('.'), membersAndValue[1]);
        }

        private static object CreateMemberProp(IDictionary<string, object> property, Member member) {
            if (!property.ContainsKey(member.Name))
                property[member.Name] = member.IsArray
                    ? new List<object?>()
                    : new Dictionary<string, object>();
            return property[member.Name];
        }
        
        private static void StoreArrayItem(List<object?> list, Member member, object item) {
            // the list must contain at least index + 1 items!
            while (list.Count < member.Index + 1)
                list.Add(null);
            list[member.Index ?? 0] = item;
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
            public int? Index { get; }
            public bool IsArray { get; }
            public override string ToString() => $"{Name}{(IsArray ? $"[{Index}]" : "")}";
        }
    }
}
