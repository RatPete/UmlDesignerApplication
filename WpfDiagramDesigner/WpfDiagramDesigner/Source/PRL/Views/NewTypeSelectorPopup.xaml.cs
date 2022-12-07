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
    /// Interaction logic for NewTypeSelectorPopup.xaml
    /// </summary>
    public partial class NewTypeSelectorPopup : Window
    {
        public NewTypeSelectorPopUpViewModel ViewModel { get; set; }
        private string newObjectName;
        public NewTypeSelectorPopup(string errorMessage, Point pos,string name)
        {

            newObjectName = name;
            InitializeComponent();
            ViewModel = new NewTypeSelectorPopUpViewModel
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
            switch (ViewModel.SelectedItem)
            {
                case "Osztály": UMLReader.UmlReader.CreateClass(newObjectName, new List<string>(), new List<string>()); break;
                case "Interfész": UMLReader.UmlReader.CreateInterface(newObjectName, new List<string>()); break;
                case "Enumeráció": UMLReader.UmlReader.CreateEnumeration(newObjectName, new List<string>()); break;
                case "Primitív típus":UMLReader.UmlReader.CreatePrimitive(newObjectName);break;
            }
            DialogResult = true;
            this.Close();
        }
    }
}
