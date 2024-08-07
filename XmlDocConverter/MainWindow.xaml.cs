using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Xml.Linq;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Diagnostics;
using XmlDocConverterLibary.Utilities.DocumentationParser;
using System.Windows.Controls;

namespace XmlDocConverter
{
    public partial class MainWindow : Window
    {
        

        public MainWindow() 
        {
            InitializeComponent();
            FileSelectionFrame.Navigate(new FileSelectionPage());
        }

        public void SetDocumentationViewerPage(DocumentationViwerPage page)
        {
            DocumentationViewerFrame.Navigate(page);

        }
    }
}