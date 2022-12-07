using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfDiagramDesigner.Source.PRL.ViewModel;

namespace WpfDiagramDesigner.Source.PRL.Views
{
    /// <summary>
    /// Interaction logic for ErrorPopup.xaml
    /// </summary>
    public partial class ErrorPopup : Window
    {
        public ErrorPopupViewModel ViewModel { get; set; }
        public ErrorPopup(string errorMessage,Point pos)
        {
            

            InitializeComponent();
            ViewModel = new ErrorPopupViewModel
            {
                ErrorDescription = errorMessage
            };

            this.DataContext = ViewModel;
            this.Top = pos.Y;
            this.Left = pos.X;
        }

        private void RejectButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void FixButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }
    }
}
