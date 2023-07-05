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

        public static long GetDirectorySize(DirectoryInfo dirInfo)
        {
            long bytes = 0;

            FileInfo[] filesInfo = dirInfo.GetFiles("*.*", SearchOption.AllDirectories);
            for (int i = 0; i  < filesInfo.Length; i++)
            {
                bytes += filesInfo[i].Length;
            }

            return bytes;
        }

        public static int GetFilesCount(DirectoryInfo dirInfo)
        {
            FileInfo[] filesInfo = dirInfo.GetFiles("*.*", SearchOption.AllDirectories);
            int count = filesInfo.Length;
                
            return count;
        }

        public static int GetDirectoryCount(DirectoryInfo dirInfo)
        {
            DirectoryInfo[] dirsInfo = dirInfo.GetDirectories("*.*", SearchOption.AllDirectories);
            int count = dirsInfo.Length;

            return count;
        }
    }
}
