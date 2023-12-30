namespace tacotron2_management.Models
{
    public static class GlobalMessage
    {
        static GlobalMessage()
        {
            // Default values
            GlobalStr = "None";
        }
        public static string GlobalStr { get; private set; }

        public static void SetGlobalStr(string newStr)
        {
            GlobalStr = newStr;
        }
    }
}
