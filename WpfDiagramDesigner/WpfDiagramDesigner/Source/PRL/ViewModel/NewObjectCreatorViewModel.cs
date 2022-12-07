
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfDiagramDesigner.Source.PRL.Helper;
using System.Linq;

namespace WpfDiagramDesigner.Source.PRL.ViewModel
{
    public class NewObjectCreatorViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<TextBox> Attributes { get; set; } = new ObservableCollection<TextBox>();
        public ObservableCollection<TextBox> Functions { get; set; } = new ObservableCollection<TextBox>();
        public ObservableCollection<TextBox> Enumerations { get; set; } = new ObservableCollection<TextBox>();
        public ObservableCollection<Grid> AttribPanel { get; set; } = new ObservableCollection<Grid>();
        public ObservableCollection<Grid> FunPanel { get; set; } = new ObservableCollection<Grid>();
        public ObservableCollection<Grid> EnumPanel { get; set; } = new ObservableCollection<Grid>();
        private ObjectType creatorType = ObjectType.ENUMERATION;


        public event PropertyChangedEventHandler PropertyChanged;
        public string NewObjectName { get; set; } = "";
        public ObjectType CreatorType
        {
            get { return creatorType; }
            set
            {
                if (creatorType != value)
                {
                    creatorType = value;
                    RaisePropertyChanged();
                }
            }
        }
        public NewObjectCreatorViewModel(ObjectType type)
        {
            CreatorType = type;
        }
        public bool SaveObject()
        {
            if (ValidateCheckboxes(true))
                return false;
            else
            {
                switch (CreatorType)
                {
                    case ObjectType.CLASS: return UMLReader.UmlReader.CreateClass(NewObjectName, Attributes.Select(item => item.Text).ToList(), Functions.Select(item => item.Text).ToList());
                    case ObjectType.INTERFACE: return UMLReader.UmlReader.CreateInterface(NewObjectName, Functions.Select(item => item.Text).ToList());
                    case ObjectType.ENUMERATION: return UMLReader.UmlReader.CreateEnumeration(NewObjectName, Enumerations.Select(item => item.Text).ToList());
                    default: throw new Exception("Hiba történt az alkalmazásban és nem került beállításra a CreatorType a modellben");
                }

            }
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public void AddButtonClicked(object sender, RoutedEventArgs e)
        {
            var senderButton = sender as System.Windows.Controls.Button;
            if (senderButton != null)
            {

                if (senderButton.Name == "FunctionAdd")
                {
                    var textBox = new TextBox { Text = "" };
                    textBox.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                    textBox.LostFocus += TextBox_LostFocus;
                    Functions.Add(textBox);

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
                        Functions.Remove(textBox);
                        FunPanel.Remove(grid);
                    };

                    Grid.SetColumn(button, 1);
                    Grid.SetColumn(textBox, 0);
                    grid.Children.Add(button);
                    grid.Children.Add(textBox);


                    FunPanel.Add(grid);

                }
                if (senderButton.Name == "AttributeAdd")
                {

                    var textBox = new TextBox { Text = "" };

                    textBox.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                    textBox.LostFocus += TextBox_LostFocus;
                    Attributes.Add(textBox);


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
                        Attributes.Remove(textBox);
                        AttribPanel.Remove(grid);
                    };

                    Grid.SetColumn(button, 1);
                    Grid.SetColumn(textBox, 0);
                    grid.Children.Add(button);
                    grid.Children.Add(textBox);

                    AttribPanel.Add(grid);
                }
                if (senderButton.Name == "LiteralAdd")
                {
                    var textBox = new TextBox { Text = "" };

                    textBox.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                    textBox.LostFocus += TextBox_LostFocus;
                    Enumerations.Add(textBox);

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
                        Enumerations.Remove(textBox);
                        EnumPanel.Remove(grid);
                    };

                    Grid.SetColumn(button, 1);
                    Grid.SetColumn(textBox, 0);
                    grid.Children.Add(button);
                    grid.Children.Add(textBox);
                    EnumPanel.Add(grid);
                }
            }

        }

        public void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateCheckboxes();
        }

        private bool ValidateCheckboxes(bool createdClick=false)
        {
            bool hasInvalid = false;
            string exceptionLog = "";
            foreach (var element in Attributes)
            {
                try
                {
                    InlineParser.CanParseAttribute(element.Text);
                    element.Background = Brushes.Transparent;
                }
                catch (ObjectNotParsableException e)
                {
                    exceptionLog += e.Message+"\n";
                    element.Background = Brushes.Red;
                    hasInvalid = true;
                }

            }
            foreach (var element in Functions)
            {
                try
                {
                    InlineParser.CanParseFunction(element.Text);
                    element.Background = Brushes.Transparent;

                }
                catch (ObjectNotParsableException e)
                {
                    exceptionLog += e.Message + "\n";
                    element.Background = Brushes.Red;
                    hasInvalid = true;
                }

            }
            foreach (var element in Enumerations)
            {
                try
                {
                    InlineParser.CanParseEnum(element.Text);
                    element.Background = Brushes.Transparent;
                }
                catch (ObjectNotParsableException e)
                {
                    exceptionLog += e.Message + "\n";
                    element.Background = Brushes.Red;
                    hasInvalid = true;
                }

            }
            try
            {
                if (!InlineParser.CanParseEnum(NewObjectName))
                {
                    hasInvalid = true;
                }
            }
            catch (ObjectNotParsableException e)
            {
                hasInvalid = true;

                exceptionLog += e.Message + "\n";
            }
            if (exceptionLog != ""&&createdClick)
                throw new ObjectNotParsableException(exceptionLog);
            return hasInvalid;
        }
    }
}
