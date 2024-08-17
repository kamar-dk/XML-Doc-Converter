using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Xml.Linq;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Diagnostics;
using XmlDocConverterLibary.Utilities.DocumentationParser;
using System.Windows.Controls;
using log4net;
using log4net.Config;

namespace XmlDocConverter
{
    public partial class MainWindow : Window
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(MainWindow));

        public MainWindow() 
        {
            XmlConfigurator.Configure(new FileInfo("app.config"));
            log.Info("Program Started");

            InitializeComponent();
            FileSelectionFrame.Navigate(new FileSelectionPage());
        }

        public void SetDocumentationViewerPage(DocumentationViwerPage page)
        {
            DocumentationViewerFrame.Navigate(page);
        }
    }
}