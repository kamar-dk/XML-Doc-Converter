using System.Security.Cryptography.X509Certificates;


namespace XmlDocConverterConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to XML Documentation Converter!");

            Menu();

            if (args.Length < 2)
            {
                Console.WriteLine("Usage: XmlDocConverterConsole <inputFilePath> <outputFilePath>");
                return;
            }

            string inputFilePath = args[0];
            string outputFilePath = args[1];

            if (!File.Exists(inputFilePath))
            {
                Console.WriteLine($"Input file not found: {inputFilePath}");
                return;
            }

            try
            {
                ConvertXmlDocumentation(inputFilePath, outputFilePath);
                Console.WriteLine($"Conversion successful! Output file created at: {outputFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public static void Menu()
        {
            Console.Clear();
            Console.WriteLine("Menu");
            Console.WriteLine("1. Insert Path To XML Documentation File");
            Console.WriteLine("2. Convert XML Documentation to Markdown");
            Console.WriteLine("3. Convert XML Documentation to Bitbucket Markdown");
            Console.WriteLine("4. Convert XML Documentation to HTML");
            Console.WriteLine("5. Exit");
        }

        public static void ConvertXmlDocumentation(string inputFilePath, string outputFilePath)
        {
            X509Certificate2 cert = new X509Certificate2();
            var xmlDoc = new X509Certificate2(inputFilePath);
            var classDocs = XmlParser.ParseDocumentation(xmlDoc);
            string markdownContent = MarkdownParser.GenerateMarkdown(classDocs);

            File.WriteAllText(outputFilePath, markdownContent);
        }
    }
}