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
    /// Interaction logic for InfoPopup.xaml
    /// </summary>
    public partial class InfoPopup : Window
    {

        public InfoPopupViewModel ViewModel { get; set; }
        public InfoPopup(string errorMessage, Point pos)
        {


            InitializeComponent();
            ViewModel = new InfoPopupViewModel
            {
                ErrorDescription = errorMessage
            };

            this.DataContext = ViewModel;
            this.Top = pos.Y;
            this.Left = pos.X;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
