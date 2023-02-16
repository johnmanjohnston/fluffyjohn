namespace fluffyjohn.Utility
{
    public static class Utility
    {
        public static void StrRemove(ref string orgStr, string match) 
        { 
            if (match.Length > orgStr.Length) { throw new ArgumentException("Match cannot be bigger than orginal string"); }
            orgStr = orgStr.Replace(match, string.Empty);
        }
    }
}
