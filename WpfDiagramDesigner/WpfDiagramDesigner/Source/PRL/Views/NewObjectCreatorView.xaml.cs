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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class NewObjectCreatorView : Window
    {
        private NewObjectCreatorViewModel ViewModel { get; set; }
        public NewObjectCreatorView(ObjectType type,double width)
        {
            ViewModel = new NewObjectCreatorViewModel(type);
            InitializeComponent();
            this.Width = width;
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
                else
                {
                    InfoPopup popup = new InfoPopup("Ez a név már foglalt:" + ViewModel.NewObjectName, PopupGlobalPosition.Position);
                    popup.ShowDialog();

                }
            }
            catch(ObjectNotParsableException ex)
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

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ViewModel.TextBox_LostFocus(sender, e);
            if (!InlineParser.CanParseEnum(((TextBox)sender).Text))
            {
                ((TextBox)sender).Background = Brushes.Red;
            }
            else
            {
                ((TextBox)sender).Background = Brushes.Transparent;
            }
        }
    }
}
