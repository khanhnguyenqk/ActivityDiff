using System;
using System.Windows;
using System.Windows.Forms;
using XmlItemDiffTool;

namespace XmlDocumentWrapper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            fileDialog.RestoreDirectory = true;
        }

        private void ButtonSearch1_OnClick(object sender, RoutedEventArgs e)
        {
            if(DataContext is MainWindowViewModel)
            {
                MainWindowViewModel viewModel = (MainWindowViewModel) DataContext;
                viewModel.FilePath1 = SearchFile();
            }
        }

        private void ButtonSearch2_OnClick(object sender, RoutedEventArgs e)
        {
            if(DataContext is MainWindowViewModel)
            {
                MainWindowViewModel viewModel = (MainWindowViewModel)DataContext;
                viewModel.FilePath2 = SearchFile();
            }
        }

        private readonly FileDialog fileDialog = new OpenFileDialog();

        private string SearchFile()
        {
            if(fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return fileDialog.FileName;
            }
            return String.Empty;
        }
    }
}
