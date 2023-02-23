namespace fluffyjohn
{
    public static class PathFormatter
    {
        public static string[] FormatEntries(
            ref string[] entries, string reqPath, string rootUserDir)
        {
            string[] formattedEntries = entries;
            
            for (int i = 0; i < formattedEntries.Length; i++)
            {
                Utility.StrRemove(ref formattedEntries[i], rootUserDir);

                formattedEntries[i] = formattedEntries[i].Replace("//", "/");
                formattedEntries[i] = formattedEntries[i].Replace("\\", "/");

                try { Utility.StrRemove(ref formattedEntries[i], reqPath); }
                catch (ArgumentException) { }
            }

            return formattedEntries;
        }

        public static bool ValidFilePath(string? fPath)
        {
            if (fPath == null || fPath == string.Empty)
            {
                return false;
            }

            return true;
        }
    }
}
