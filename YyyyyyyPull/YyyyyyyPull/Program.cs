using HtmlAgilityPack;

namespace YyyyyyyPull
{
    public static class Program
    {
        internal static HttpClient httpClient = new();
        internal static string outputDir = Path.Combine(Environment.CurrentDirectory, "yyyyyyy");

        static async Task Main()
        {
            Console.WriteLine("--- YyyyyyyPull ---");
            EnsureDirectoryExists(outputDir);
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("YyyyyyyPullBot/1.0");

            Console.WriteLine("Fetching HTML...");
            string html = await httpClient.GetStringAsync(Constants.URL);

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var assetUrls = doc.DocumentNode
                .SelectNodes("//img[@src] | //audio[@src] | //video[@src] | //source[@src] | //embed[@src]")
                ?.Select(n => n.GetAttributeValue("src", null))
                .Where(src => !string.IsNullOrWhiteSpace(src))
                .Distinct()
                .ToList();

            if (assetUrls == null || assetUrls.Count == 0)
            {
                Console.WriteLine("No assets found.");
                return;
            }

            Console.WriteLine($"Found {assetUrls.Count} assets. Downloading...");

            foreach (var src in assetUrls)
            {
                try
                {
                    var absoluteUrl = src.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                        ? src
                        : new Uri(new Uri(url), src).ToString();

                    string fileName = Path.GetFileName(new Uri(absoluteUrl).LocalPath);
                    if (string.IsNullOrWhiteSpace(fileName))
                        fileName = Guid.NewGuid().ToString();

                    string filePath = Path.Combine(outputDir, fileName);

                    Console.WriteLine($"Downloading {fileName}...");
                    var data = await httpClient.GetByteArrayAsync(absoluteUrl);
                    await File.WriteAllBytesAsync(filePath, data);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to download {src}: {ex.Message}");
                }
            }

            Console.WriteLine("Done! All assets saved to: " + outputDir);
        }

        public static void EnsureDirectoryExists(string dirName, string internalDirName = "")
        {
            string dir;
            if (string.IsNullOrEmpty(internalDirName))
               dir = Path.Combine(Environment.CurrentDirectory, dirName);
            else
                dir = Path.Combine(Environment.CurrentDirectory, internalDirName, dirName);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }
    }
}
