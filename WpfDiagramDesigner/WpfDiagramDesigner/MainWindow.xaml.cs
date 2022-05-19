using MetaDslx.GraphViz;
using MetaDslx.Languages.Uml.Model;
using MetaDslx.Languages.Uml.Serialization;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfDiagramDesigner.Objects;
using WpfDiagramDesigner.Source.PRL.Helper;
using WpfDiagramDesigner.UMLReader;
using WpfDiagramDesigner.ViewModel;

namespace WpfDiagramDesigner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel viewModel;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "UML file (*.uml)|*.uml|XMI file (*.xmi)|*.xmi";
            if (openFileDialog.ShowDialog() == true)
            {
                viewModel = new MainViewModel(canvas);
                viewModel.InitDiagram(openFileDialog.FileName);
                viewModel.DrawAll();
            }
                
        }
        private void New_Diagram_Click(object sender,RoutedEventArgs e)
        {
            viewModel = new MainViewModel(canvas);
            viewModel.InitDiagram("");
            viewModel.DrawAll();
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XMI file (*.xmi)|*.xmi";
            if (saveFileDialog.ShowDialog() == true)
                UMLReader.UmlReader.WriteOut(saveFileDialog.FileName);
        }

        private void Aggregation_Click(object sender, RoutedEventArgs e)
        {
            RelationshipCreator.CurrentClickType = ClickType.AGGREGATION;
            DisableElements();
            viewModel.CancelLineDraw();
        }

        private void Association_Click(object sender, RoutedEventArgs e)
        {
            RelationshipCreator.CurrentClickType = ClickType.ASSOCIATION;
            DisableElements();
            viewModel.CancelLineDraw();
        }

        private void Composition_Click(object sender, RoutedEventArgs e)
        {
            RelationshipCreator.CurrentClickType = ClickType.COMPOSITION;
            DisableElements();
            viewModel.CancelLineDraw();
        }

        private void Inheritance_Click(object sender, RoutedEventArgs e)
        {
            RelationshipCreator.CurrentClickType = ClickType.INHERITANCE;
            DisableElements();
            viewModel.CancelLineDraw();
        }

        private void Dependency_Click(object sender, RoutedEventArgs e)
        {
            RelationshipCreator.CurrentClickType = ClickType.DEPENDENCY;
            DisableElements();
            viewModel.CancelLineDraw();
        }

        private void Realization_Click(object sender, RoutedEventArgs e)
        {
            RelationshipCreator.CurrentClickType = ClickType.REALIZATION;
            DisableElements();
            viewModel.CancelLineDraw();
        }
        private void OneWayAssociation_Click(object sender, RoutedEventArgs e)
        {
            RelationshipCreator.CurrentClickType = ClickType.ONEWAYASSOCIATION;
            DisableElements();
            viewModel.CancelLineDraw();
        }

        private void DisableElements()
        {
            viewModel.DisableAll();
        }

        private void Normal_Click(object sender, RoutedEventArgs e)
        {
            RelationshipCreator.CurrentClickType = ClickType.NORMAL;
            EnableElements();
            viewModel.CancelLineDraw();

        }

        private void EnableElements()
        {
            viewModel.EnableAll();
        }
    }
}
