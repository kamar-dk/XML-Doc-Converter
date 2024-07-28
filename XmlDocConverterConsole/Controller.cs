using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using XmlDocConverterLibary.Utilities.DocumentationParser;

namespace XmlDocConverterConsole
{
    public class Controller
    {
        private string selectedFilePath;
        private string selectedFileName;
        private string outputDirectory;

        public void Menu()
        {
            Console.Clear();
            Console.WriteLine("Menu");
            Console.WriteLine("1. Insert Path To XML Documentation File");
            Console.WriteLine("2. Convert XML Documentation to Markdown");
            Console.WriteLine("3. Convert XML Documentation to Bitbucket Markdown");
            Console.WriteLine("4. Convert XML Documentation to HTML");
            Console.WriteLine("5. Exit");
            ReadUserInput();
        }

        public void ReadUserInput()
        {
            Console.WriteLine("Enter your choice:");
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    ReadXmlDocument();
                    break;
                case "2":
                    ConvertToMarkdown();
                    break;
                case "3":
                    ConvertToBitbucketMarkdown();
                    break;
                case "4":
                    ConvertToHtml();
                    break;
                case "5":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
            Console.WriteLine("Press any key to return to the menu.");
            Console.ReadKey();
            Menu();
        }

        public void ReadXmlDocument()
        {
            Console.Clear();
            Console.WriteLine("Enter the path to the XML documentation file:");
            string inputFilePath = Console.ReadLine();
            if (!File.Exists(inputFilePath))
            {
                Console.WriteLine($"Input file not found: {inputFilePath}");
                return;
            }
            else
            {
                selectedFilePath = inputFilePath;
                outputDirectory = Path.GetDirectoryName(selectedFilePath);
            }
        }



        public void ConvertToMarkdown()
        {
            try
            {
                // Check if the file exists
                if (!File.Exists(selectedFilePath))
                {
                    Console.WriteLine($"File not found: {selectedFilePath}");
                    return;
                }

                XDocument xmlDoc = XDocument.Load(selectedFilePath);
                var classDocs = XmlParser.ParseDocumentation(xmlDoc);
                string markdownContent = MarkdownParser.GenerateMarkdown(classDocs);
                Directory.CreateDirectory(outputDirectory);
                string markdownPath = Path.Combine(outputDirectory, "documentation.md");
                File.WriteAllText(markdownPath, markdownContent);

                Console.WriteLine($"Markdown file created at {markdownPath}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Access to the path is denied: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public void ConvertToBitbucketMarkdown()
        {
            try
            {
                if (!File.Exists(selectedFilePath))
                {
                    Console.WriteLine($"File not found: {selectedFilePath}");
                    return;
                }

                XDocument xmlDoc = XDocument.Load(selectedFilePath);
                var classDocs = XmlParser.ParseDocumentation(xmlDoc);
                string markdownContent = BitBucketMarkdownParser.GenerateMarkdown(classDocs);

                string markdownPath = Path.Combine(outputDirectory, "BitbucketDocumentation.md");
                File.WriteAllText(markdownPath, markdownContent);

                Console.WriteLine($"Markdown file created at {markdownPath}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Access to the path is denied: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public void ConvertToHtml()
        {
            try
            {
                if (!File.Exists(selectedFilePath))
                {
                    Console.WriteLine($"File not found: {selectedFilePath}");
                    return;
                }
                XDocument xmlDoc = XDocument.Load(selectedFilePath);
                var classDocs = XmlParser.ParseDocumentation(xmlDoc);
                string htmlContent = HtmlParser.GenerateHtml(classDocs);

                string htmlPath = Path.Combine(outputDirectory, "documentation.html");
                File.WriteAllText(htmlPath, htmlContent);

                // Add the styles.css file from XmlDocConvertLibary.Resources to the output directory
                string cssSourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "styles.css");
                string cssDestinationPath = Path.Combine(outputDirectory, "styles.css");
                File.Copy(cssSourcePath, cssDestinationPath, true);

                Console.WriteLine($"HTML file created at {htmlPath}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Access to the path is denied: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

        }
    }
}
