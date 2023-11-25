namespace NetDot
{
    public static class DotNotationExtensions {

        public static string AsQueryString(this object o, DotNotationSettings? settings = null) {
            settings ??= new();
            settings.UrlEncode = true;
            settings.EntrySeparator = "&";
            return DotNotation.Serialize(o, settings: settings);
        }

    }
}
