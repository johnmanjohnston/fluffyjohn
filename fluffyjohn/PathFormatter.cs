using fluffyjohn.Utility;

namespace fluffyjohn
{
    public static class PathFormatter
    {
        private static void StrRemove(ref string orgStr, string match) { orgStr = orgStr.Replace(match, string.Empty); }

        public static string[] FormatEntries(
            ref string[] entries, string reqPath, string rootUserDir)
        {
            string[] formattedEntries = entries;
            
            for (int i = 0; i < formattedEntries.Length; i++)
            {
                StrRemove(ref formattedEntries[i], rootUserDir);

                formattedEntries[i] = formattedEntries[i].Replace("//", "/");
                formattedEntries[i] = formattedEntries[i].Replace("\\", "/");

                try { StrRemove(ref formattedEntries[i], reqPath); }
                catch (ArgumentException) { }
            }

            return formattedEntries;
        }
    }
}
