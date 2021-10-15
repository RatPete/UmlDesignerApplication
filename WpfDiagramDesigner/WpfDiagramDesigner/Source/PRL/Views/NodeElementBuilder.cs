using MetaDslx.Languages.Uml.Model;
using MetaDslx.Modeling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using WpfDiagramDesigner.Objects;

namespace WpfDiagramDesigner.Views
{
    public static class NodeElementBuilder
    {
        
        private static void StyleTexbox(TextBox tb)
        {
            tb.Foreground = Brushes.Black;
            tb.FontSize = 8.0;
            tb.Padding = new System.Windows.Thickness(0);
            tb.Background = Brushes.Transparent;
            tb.BorderBrush = Brushes.Transparent;
            tb.BorderThickness = new System.Windows.Thickness(0);
        }
        public static TextBox FunctionBuilder(MetaDslx.Languages.Uml.Model.OperationBuilder item, ViewModel.IRefreshable model)
        {
            TextBox tb = new TextBox();
            tb.Text=UMLReader.UmlReader.CreateFunctionText(item);
            StyleTexbox(tb);
            tb.LostFocus += (e, er) =>
            {
                InlineParser.FunctionParse(tb.Text, item);
                tb.Text=UMLReader.UmlReader.CreateFunctionText(item);
                StyleTexbox(tb);
                model.Refresh();
            };
            tb.ContextMenu = new ContextMenu();
            tb.ContextMenu.Items.Clear();
            var menuitem = new MenuItem();
            menuitem.Header = "Remove";
            menuitem.Click += (e, er) =>
            {
                ((ClassBuilder)item.Owner).OwnedOperation.Remove(item);
                model.Refresh();
            };
            tb.ContextMenu.Items.Add(menuitem);
            menuitem = new MenuItem();
            menuitem.Header = "Add";
            menuitem.Click += (e, er) =>
            {
                ((ClassBuilder)item.Owner).OwnedAttribute.Add(UMLReader.UmlReader.UmlFactory.Property());
                model.Refresh();
            };
            tb.ContextMenu.Items.Add(menuitem);


            return tb;
        }
        public static TextBox AttributeBuilder(PropertyBuilder item, ViewModel.IRefreshable model)
        {
            var tb = new TextBox();
            tb.Text=UMLReader.UmlReader.CreateAttributeText( item);
            StyleTexbox(tb);
            tb.LostFocus += (e, er) =>
            {
                InlineParser.AttributeParse(tb.Text, item);
                tb.Text=UMLReader.UmlReader.CreateAttributeText( item);

                model.Refresh();
            };
            tb.ContextMenu = new ContextMenu();
            tb.ContextMenu.Items.Clear();
            var menuitem = new MenuItem();
            menuitem.Header = "Remove";
            menuitem.Click += (e, er) =>
            {
                ((ClassBuilder)item.Owner).OwnedAttribute.Remove(item);
                tb.Text=UMLReader.UmlReader.CreateAttributeText( item);

                model.Refresh();
            };
            tb.ContextMenu.Items.Add(menuitem);
            menuitem = new MenuItem();
            menuitem.Header = "Add";
            menuitem.Click += (e, er) =>
            {
                ((ClassBuilder)item.Owner).OwnedAttribute.Add(UMLReader.UmlReader.UmlFactory.Property());
                tb.Text= UMLReader.UmlReader.CreateAttributeText( item);
                model.Refresh();
            };
            tb.ContextMenu.Items.Add(menuitem);
            return tb;
        }

        public static TextBox EnumBuilder(EnumerationLiteralBuilder enumerationLiteral, ViewModel.IRefreshable model)
        {
            var tb = new TextBox();
            CreateEnumText(tb, enumerationLiteral);
            tb.LostFocus += (e, er) =>
            {
                InlineParser.NameParser(tb.Text, enumerationLiteral);
                CreateEnumText(tb, enumerationLiteral);
                model.Refresh();
            };
            tb.ContextMenu = new ContextMenu();
            var menuitem = new MenuItem { Header = "Remove" };
            menuitem.Click += (e, er) =>
            {
                ((EnumerationBuilder)(enumerationLiteral.Owner)).OwnedLiteral.Remove(enumerationLiteral);
                model.Refresh();
            };
            tb.ContextMenu.Items.Add(menuitem);

            return tb;
        }

        private static void CreateEnumText(TextBox tb, EnumerationLiteralBuilder enumerationLiteral)
        {
            tb.Text = enumerationLiteral?.Name;
            StyleTexbox(tb);
        }
    }
}