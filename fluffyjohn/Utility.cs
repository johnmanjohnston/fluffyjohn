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

            foreach (FileInfo fileInfo in dirInfo.GetFiles("*.*", SearchOption.AllDirectories))
            {
                bytes += (long)fileInfo.Length;
            }

            return bytes;
        }

        public static int GetFilesCount(DirectoryInfo dirInfo)
        {
            int count = 0;

            foreach (FileInfo fileInfo in dirInfo.GetFiles("*.*", SearchOption.AllDirectories))
            {
                count++;
            }
                
            return count;
        }

        public static int GetDirectoryCount(DirectoryInfo dirInfo)
        {
            int count = 0;

            foreach (DirectoryInfo subDirInfo in dirInfo.GetDirectories("*.*", SearchOption.AllDirectories))
            {
                count++;
            }

            return count;
        }
    }
}
