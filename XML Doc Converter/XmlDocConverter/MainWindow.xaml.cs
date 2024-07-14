using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Xml.Linq;
using XmlDocConverter.Utilities;

namespace XmlDocConverter
{
    public partial class MainWindow : Window
    {
        private string selectedFilePath;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XML Files (*.xml)|*.xml|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                selectedFilePath = openFileDialog.FileName;
                txtEditor.Text = File.ReadAllText(selectedFilePath);
            }
        }

        private void btnConvertFileToMarkdown_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedFilePath))
            {
                XDocument xmlDoc = XDocument.Load(selectedFilePath);
                var classDocs = DocumentationParser.ParseDocumentation(xmlDoc);
                string markdownContent = DocumentationParser.GenerateMarkdown(classDocs);

                string markdownPath = Path.Combine(Path.GetDirectoryName(selectedFilePath), "documentation.md");
                File.WriteAllText(markdownPath, markdownContent);

                MessageBox.Show($"Markdown file created at {markdownPath}", "Conversion Complete", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Please select an XML file first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnConvertFileToHtml_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedFilePath))
            {
                XDocument xmlDoc = XDocument.Load(selectedFilePath);
                var classDocs = DocumentationParser.ParseDocumentation(xmlDoc);
                string htmlContent = DocumentationParser.GenerateHtml(classDocs);

                string htmlPath = Path.Combine(Path.GetDirectoryName(selectedFilePath), "documentation.html");
                File.WriteAllText(htmlPath, htmlContent);

                MessageBox.Show($"HTML file created at {htmlPath}", "Conversion Complete", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Please select an XML file first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}