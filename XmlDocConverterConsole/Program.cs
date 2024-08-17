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
        }
    }
}