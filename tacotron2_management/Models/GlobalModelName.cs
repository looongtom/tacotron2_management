namespace tacotron2_management.Models
{
    public static class GlobalModelName
    {
        static GlobalModelName()
        {
            // Default values
            GlobalModel = "None";
        }
        public static string GlobalModel { get; private set; }

        public static void SetGlobalModel(string newStr)
        {
            GlobalModel = newStr;
        }
    }
}
