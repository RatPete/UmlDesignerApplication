using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfDiagramDesigner.Source.PRL.Helper;
using WpfDiagramDesigner.Source.PRL.Views;

namespace WpfDiagramDesigner.Source.PRL.ViewModel
{
    public class EditPrimitiveListViewModel
    {
        public EditPrimitiveListViewModel()
        {

            var allPrimitives = UMLReader.UmlReader.GetAllPrimitives();
            foreach (var item in allPrimitives)
            {
                var textBox = new TextBox { Text = item.Name };
                textBox.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                textBox.LostFocus += TextBox_LostFocus;
                Primitives.Add(textBox);

                var grid = new Grid();
                var coldef1 = new ColumnDefinition();
                var coldef2 = new ColumnDefinition();
                coldef2.Width = GridLength.Auto;
                grid.ColumnDefinitions.Add(coldef1);
                grid.ColumnDefinitions.Add(coldef2);
                var button = new Button();
                button.Content = "Remove";
                button.Click += (e, er) =>
                {
                    Primitives.Remove(textBox);
                    PrimitivePanel.Remove(grid);
                };

                Grid.SetColumn(button, 1);
                Grid.SetColumn(textBox, 0);
                grid.Children.Add(button);
                grid.Children.Add(textBox);


                PrimitivePanel.Add(grid);
            }
        }
        public ObservableCollection<Grid> PrimitivePanel { get; set; } = new ObservableCollection<Grid>();
        public ObservableCollection<TextBox> Primitives { get; set; } = new ObservableCollection<TextBox>();
        public void AddButtonClicked(object sender, RoutedEventArgs e)
        {

            var textBox = new TextBox { Text = "" };
            textBox.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            textBox.LostFocus += TextBox_LostFocus;
            Primitives.Add(textBox);

            var grid = new Grid();
            var coldef1 = new ColumnDefinition();
            var coldef2 = new ColumnDefinition();
            coldef2.Width = GridLength.Auto;
            grid.ColumnDefinitions.Add(coldef1);
            grid.ColumnDefinitions.Add(coldef2);
            var button = new Button();
            button.Content = "Törlés";
            button.Click += (e, er) =>
            {
                Primitives.Remove(textBox);
                PrimitivePanel.Remove(grid);
            };

            Grid.SetColumn(button, 1);
            Grid.SetColumn(textBox, 0);
            grid.Children.Add(button);
            grid.Children.Add(textBox);


            PrimitivePanel.Add(grid);


        }

        public bool SaveObject()
        {
            if (ValidateCheckboxes(true))
                return false;
            else
            {
                var allPrimitives = UMLReader.UmlReader.GetAllPrimitives();
                var allNewPrimitives = Primitives.Where(item => !allPrimitives.Any(primitive => primitive.Name != null && primitive.Name != "" && primitive.Name.ToLower() == item.Text?.ToLower()));
                var allRemovedPrimitives = allPrimitives.Where(primitive => !Primitives.Any(item => item.Text != null && item.Text != "" && item.Text.ToLower() == primitive.Name?.ToLower()));
                foreach(var item in allRemovedPrimitives)
                {
                    var deplist=UMLReader.UmlReader.ListDependecies(item);
                    if (deplist != null && deplist.Count > 0)
                    {
                        var depString = "";
                        foreach (var dep in deplist)
                        {
                            depString += dep + "\n";
                        }

                        var conDelete = new ConfirmDelete(depString, PopupGlobalPosition.Position);
                        var res = conDelete.ShowDialog();
                        if (res.HasValue && res.Value)
                        {
                            UMLReader.UmlReader.RemoveElementFromModel(item);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        UMLReader.UmlReader.RemoveElementFromModel(item);
                    }
                }
                foreach (var item in allNewPrimitives)
                {
                    UMLReader.UmlReader.CreatePrimitive(item.Text);
                }
                return true;
            }
        }

        public void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateCheckboxes();
        }
        private bool ValidateCheckboxes(bool createdClick = false)
        {
            bool hasInvalid = false;
            string exceptionLog = "";
            var nonUniques = Primitives.GroupBy(item => item.Text.ToLower()).Where(group => group.Count() > 1).Select(x => x.Key).ToList();
            foreach (var element in Primitives)
            {

                element.Background = Brushes.Transparent;
            }
            if (nonUniques.Any())
            {
                var allFaulty = Primitives.Where(item => nonUniques.Contains(item.Text.ToLower()));
                foreach (var item in allFaulty)
                {
                    item.Background = Brushes.Red;
                }
                hasInvalid = true;
            }
            return hasInvalid;
        }
    }
}
