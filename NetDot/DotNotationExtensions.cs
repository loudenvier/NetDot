namespace NetDot
{
    public static class DotNotationExtensions {

        public static string AsQueryString(this object o, DotNotationSettings? settings = null) {
            settings ??= DotNotationSettings.Default.Clone();
            settings.UrlEncode = true;
            settings.EntrySeparator = "&";
            return DotNotation.Serialize(o, settings: settings);
        }

    }
}
