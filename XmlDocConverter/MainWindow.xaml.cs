using System.Windows;

namespace XmlDocConverter
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new FileSelectionPage());
        }
    }
}