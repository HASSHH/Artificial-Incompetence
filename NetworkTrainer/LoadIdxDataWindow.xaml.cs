using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NetworkTrainer
{
    /// <summary>
    /// Interaction logic for LoadIdxDataWindow.xaml
    /// </summary>
    public partial class LoadIdxDataWindow : Window
    {
        public LoadIdxDataWindow()
        {
            InitializeComponent();
        }

        private void BrowseTrainingDataButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog().GetValueOrDefault())
            {
                (DataContext as LoadIdxDataWindowModel).TrainingData = ofd.FileName;
            }
        }

        private void BrowseTrainingLabelsButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog().GetValueOrDefault())
            {
                (DataContext as LoadIdxDataWindowModel).TrainingLabels = ofd.FileName;
            }
        }

        private void BrowseTestingDataButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog().GetValueOrDefault())
            {
                (DataContext as LoadIdxDataWindowModel).TestingData = ofd.FileName;
            }
        }

        private void BrowseTestingLabelsButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog().GetValueOrDefault())
            {
                (DataContext as LoadIdxDataWindowModel).TestingLabels = ofd.FileName;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            try { DialogResult = true; } catch { }
            Close();
        }
    }
}
