﻿using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using XmlDocConverterLibary.Utilities;
using XmlDocConverterLibary.Utilities.DocumentationParser;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Diagnostics;
using log4net;

namespace XmlDocConverter
{
    /// <summary>
    /// Interaction logic for FileSelection.xaml
    /// </summary>
    public partial class FileSelectionPage : Page
    {
        private string selectedFilePath;
        private string selectedFileName;
        private string outputDirectory;

        private static readonly ILog log = LogManager.GetLogger(typeof(FileSelectionPage));

        public FileSelectionPage()
        {
            InitializeComponent();
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "XML Files (*.xml)|*.xml|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == true)
                {
                    selectedFilePath = openFileDialog.FileName;
                    selectedFileName = openFileDialog.SafeFileName;
                    outputDirectory = Path.GetDirectoryName(selectedFilePath); // Set the output directory to the file's directory
                    txtSelectedFilePath.Text = selectedFileName; // Display the selected file path
                    txtOutputDirectory.Text = outputDirectory; // Display the output directory

                    MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                    mainWindow.SetDocumentationViewerPage(new DocumentationViwerPage(selectedFilePath));
                }
            } catch (Exception ex)
            {
                log.Error($"An error occurred while opening the file: {ex.Message}");
                MessageBox.Show($"An error occurred while opening the file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);    
            }
        }

        private void btnConvertFileToMarkdown_Click(object sender, RoutedEventArgs e)
        {

            if (!string.IsNullOrEmpty(selectedFilePath))
            {
                try
                {
                    XDocument xmlDoc = XDocument.Load(selectedFilePath);
                    var classDocs = XmlParser.ParseDocumentation(xmlDoc);
                    MarkdownParser markdownParser = new MarkdownParser();
                    string markdownContent = markdownParser.GenerateMarkdown(classDocs);

                    string markdownPath = Path.Combine(outputDirectory, "documentation.md");
                    File.WriteAllText(markdownPath, markdownContent);

                    MessageBox.Show($"Markdown file created at {markdownPath}", "Conversion Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                } catch (Exception ex)
                {
                    log.Error($"An error occurred while converting the file to Markdown: {ex.Message}");
                    MessageBox.Show($"An error occurred while converting the file to Markdown: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select an XML file first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnConvertFileToBitbucketMarkdown_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedFilePath))
            {
                try
                {
                    XDocument xmlDoc = XDocument.Load(selectedFilePath);
                    var classDocs = XmlParser.ParseDocumentation(xmlDoc);
                    BitBucketMarkdownParser bitBucketMarkdownParser = new BitBucketMarkdownParser();
                    string markdownContent = bitBucketMarkdownParser.GenerateMarkdown(classDocs);

                    string markdownPath = Path.Combine(outputDirectory, "BitbucketDocumentation.md");
                    File.WriteAllText(markdownPath, markdownContent);

                    MessageBox.Show($"Markdown file created at {markdownPath}", "Conversion Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                } catch (Exception ex)
                {
                    log.Error($"An error occurred while converting the file to Bitbucket Markdown: {ex.Message}");
                    MessageBox.Show($"An error occurred while converting the file to Bitbucket Markdown: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }  
            }
            else
            {
                MessageBox.Show("Please select an XML file first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnConvertFileToHtml_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedFilePath) && !string.IsNullOrEmpty(outputDirectory))
            {
                try
                {
                    XDocument xmlDoc = XDocument.Load(selectedFilePath);
                    var classDocs = XmlParser.ParseDocumentation(xmlDoc);
                    string htmlContent = HtmlParser.GenerateHtml(classDocs);

                    string htmlPath = Path.Combine(outputDirectory, "documentation.html");
                    File.WriteAllText(htmlPath, htmlContent);

                    // Copy the CSS file to the output directory
                    string cssSourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "styles.css");
                    string cssDestinationPath = Path.Combine(outputDirectory, "styles.css");
                    File.Copy(cssSourcePath, cssDestinationPath, true);

                    MessageBox.Show($"HTML file created at {htmlPath}", "Conversion Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                } catch (Exception ex)
                {
                    log.Error($"An error occurred while converting the file to HTML: {ex.Message}");
                    MessageBox.Show($"An error occurred while converting the file to HTML: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select an XML file and set the output directory first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSetOutputDirectory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new CommonOpenFileDialog();
                dialog.IsFolderPicker = true;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    outputDirectory = dialog.FileName;
                    txtOutputDirectory.Text = outputDirectory; // Display the selected output directory
                    MessageBox.Show($"Output directory set to: {outputDirectory}", "Output Directory", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            } catch (Exception ex)
            {
                log.Error($"An error occurred while setting the output directory: {ex.Message}");
                MessageBox.Show($"An error occurred while setting the output directory: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private void btnOpenOutputFolder_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(outputDirectory))
            {
                try
                {
                    Process.Start(new ProcessStartInfo()
                    {
                        FileName = outputDirectory,
                        UseShellExecute = true,
                        Verb = "open"
                    });
                } catch (Exception ex)
                {
                    log.Error($"An error occurred while opening the output directory: {ex.Message}");
                    MessageBox.Show($"An error occurred while opening the output directory: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                
            }
            else
            {
                MessageBox.Show("Please set the output directory first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
