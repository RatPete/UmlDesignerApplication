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
using WpfDiagramDesigner.Source.PRL.Helper;
using WpfDiagramDesigner.Source.PRL.ViewModel;

namespace WpfDiagramDesigner.Source.PRL.Views
{
    /// <summary>
    /// Interaction logic for EditPrimitiveList.xaml
    /// </summary>
    public partial class EditPrimitiveList : Window
    {
        private EditPrimitiveListViewModel ViewModel { get; set; }
        public EditPrimitiveList()
        {
            ViewModel = new EditPrimitiveListViewModel();
            InitializeComponent();
            this.Top = PopupGlobalPosition.Position.Y;
            this.Left = PopupGlobalPosition.Position.X;
            this.DataContext = ViewModel;
            Loaded += OnLoaded;
        }
        private void OnLoaded(object sender, RoutedEventArgs args)
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            ViewModel.AddButtonClicked(sender, e);

        }
        public void SaveInstance(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ViewModel.SaveObject())
                {
                    this.DialogResult = true;
                    this.Close();
                }
                
            }
            catch (ObjectNotParsableException ex)
            {
                InfoPopup popup = new InfoPopup(ex.Message, PopupGlobalPosition.Position);
                popup.ShowDialog();
            }


        }
        public void Cancel(object sender,RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
