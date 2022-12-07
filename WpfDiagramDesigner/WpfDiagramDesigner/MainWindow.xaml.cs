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
using WpfDiagramDesigner.Source.PRL.Views;
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
            PopupGlobalPosition.Position = new Point(this.Left + this.Width / 2.0, this.Top + this.Height / 4.0);
        }
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            PopupGlobalPosition.Position = new Point(this.Left + sizeInfo.NewSize.Width / 2.0, this.Top + sizeInfo.NewSize.Height / 4.0);
        }
        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            PopupGlobalPosition.Position = new Point(this.Left + this.Width / 2.0, this.Top + this.Height / 4.0);
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "UML file (*.uml)|*.uml|XMI file (*.xmi)|*.xmi";
            if (openFileDialog.ShowDialog() == true)
            {
                canvas.Children.Clear();
                viewModel = new MainViewModel(canvas, this);
                viewModel.InitDiagram(openFileDialog.FileName);
                viewModel.DrawAll();
            }

        }
        private void New_Diagram_Click(object sender, RoutedEventArgs e)
        {
            viewModel = new MainViewModel(canvas, this);
            canvas.Children.Clear();
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
        private void SetRelationShipType(ClickType click)
        {
            if (viewModel != null)
            {
                RelationshipCreator.CurrentClickType = RelationshipCreator.CurrentClickType == click && click != ClickType.NORMAL ? ClickType.NORMAL : click;
                if (RelationshipCreator.CurrentClickType != ClickType.NORMAL)
                {
                    DisableElements();
                    viewModel.CancelLineDraw();
                }
                else
                {
                    EnableElements();
                    viewModel.CancelLineDraw();
                }
            }
            else
            {
                InfoPopup popup = new InfoPopup("Ahhoz hogy kapcsolatot tudj hozzáadni, először nyiss meg egy diagrammot", PopupGlobalPosition.Position);
                popup.ShowDialog();
            }
        }
        private void Aggregation_Click(object sender, RoutedEventArgs e)
        {

            SetRelationShipType(ClickType.AGGREGATION);

        }

        private void Association_Click(object sender, RoutedEventArgs e)
        {
            SetRelationShipType(ClickType.ASSOCIATION);

        }

        private void Composition_Click(object sender, RoutedEventArgs e)
        {
            SetRelationShipType(ClickType.COMPOSITION);

        }

        private void Inheritance_Click(object sender, RoutedEventArgs e)
        {
            SetRelationShipType(ClickType.INHERITANCE);

        }

        private void Dependency_Click(object sender, RoutedEventArgs e)
        {
            SetRelationShipType(ClickType.DEPENDENCY);

        }

        private void Realization_Click(object sender, RoutedEventArgs e)
        {
            SetRelationShipType(ClickType.REALIZATION);

        }
        private void OneWayAssociation_Click(object sender, RoutedEventArgs e)
        {
            SetRelationShipType(ClickType.ONEWAYASSOCIATION);

        }
        private void NewClass_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel != null)
            {
                NewObjectCreatorView view = new NewObjectCreatorView(ObjectType.CLASS,this.ActualWidth);
                view.ShowDialog();

                viewModel.Refresh();
            }
            else
            {
                InfoPopup popup = new InfoPopup("Ahhoz hogy új osztály adj hozzá, először hozz létre vagy nyiss meg egy diagrammot", PopupGlobalPosition.Position);
                popup.ShowDialog();
            }

        }
        private void NewInterface_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel != null)
            {
                NewObjectCreatorView view = new NewObjectCreatorView(ObjectType.INTERFACE, this.ActualWidth);
                view.ShowDialog();
                viewModel.Refresh();
            }

            else
            {
                InfoPopup popup = new InfoPopup("Ahhoz hogy új interfészt adj hozzá, először hozz létre vagy nyiss meg egy diagrammot", PopupGlobalPosition.Position);
                popup.ShowDialog();
            }
        }
        private void NewEnum_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel != null)
            {
                NewObjectCreatorView view = new NewObjectCreatorView(ObjectType.ENUMERATION, this.ActualWidth);
                view.ShowDialog();
                viewModel.Refresh();
            }
            else
            {
                InfoPopup popup = new InfoPopup("Ahhoz hogy új enumerációt adj hozzá, először hozz létre vagy nyiss meg egy diagrammot", PopupGlobalPosition.Position);
                popup.ShowDialog();
            }
        }
        private void Edit_Primitive_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel != null)
            {
                EditPrimitiveList view = new EditPrimitiveList();
                view.ShowDialog();
                viewModel.Refresh();
            }

            else
            {
                InfoPopup popup = new InfoPopup("Ahhoz hogy a primitív típus listát módosítsd, először hozz létre vagy nyiss meg egy diagrammot", PopupGlobalPosition.Position);
                popup.ShowDialog();
            }
        }

        private void DisableElements()
        {
            viewModel.DisableAll();
        }

        private void Normal_Click(object sender, RoutedEventArgs e)
        {
            SetRelationShipType(ClickType.NORMAL);

        }

        private void EnableElements()
        {
            viewModel.EnableAll();
        }
    }
}
