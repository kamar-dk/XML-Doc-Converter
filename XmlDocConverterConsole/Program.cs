using System.Security.Cryptography.X509Certificates;
using XmlDocConverterLibary.Utilities.DocumentationParser;

namespace XmlDocConverterConsole
{
    class Program
    {
        

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to XML Documentation Converter!");
            System.Threading.Thread.Sleep(2000);
            Controller controller = new Controller();
            controller.Menu();
            

            /*if (args.Length < 2)
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
            }*/
        }

        
    }
}