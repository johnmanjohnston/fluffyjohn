namespace fluffyjohn
{
    public static class Utility
    {
        public static void StrRemove(ref string orgStr, string match) 
        { 
            if (match.Length > orgStr.Length) { throw new ArgumentException("Match cannot be bigger than orginal string"); }
            orgStr = orgStr.Replace(match, string.Empty);
        }

        public static string ConfigureDirHref(string href)
        {
            int dirSplits = href.Split("/").Length;

            for (int i = 0; i < dirSplits; i++)
            {
                href = href.Replace("//", "/");
            }

            return href;
        }
    }
}
