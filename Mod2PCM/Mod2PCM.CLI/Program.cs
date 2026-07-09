namespace Mod2PCM.CLI
{
    public class Program
    {
        public static void Main()
        {
            BassHandler handler = new();

            handler.Load(@"D:\aryx.s3m");
            handler.Write();
        }
    }
}
