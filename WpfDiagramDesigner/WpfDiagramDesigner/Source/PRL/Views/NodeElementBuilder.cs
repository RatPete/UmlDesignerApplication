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
        public static TextBox FunctionBuilder(MetaDslx.Languages.Uml.Model.OperationBuilder item, Objects.Node element, ViewModel.IRefreshable model)
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
                element.RemoveFunction(tb,item);
                ((ClassBuilder)item.Owner).OwnedOperation.Remove(item);
                model.Refresh();
            };
            tb.ContextMenu.Items.Add(menuitem);
            menuitem = new MenuItem();
            menuitem.Header = "Add";
            menuitem.Click += (e, er) =>
            {
                ((ClassBuilder)item.Owner).OwnedAttribute.Add(UMLReader.UmlReader.UmlFactory.Property());
                element.AddFunction();
                model.Refresh();
            };
            tb.ContextMenu.Items.Add(menuitem);


            return tb;
        }
        public static TextBox AttributeBuilder(PropertyBuilder item, Objects.Node element, ViewModel.IRefreshable model)
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
                element.RemoveAttribute(tb,item);
                model.Refresh();
            };
            tb.ContextMenu.Items.Add(menuitem);
            menuitem = new MenuItem();
            menuitem.Header = "Add";
            menuitem.Click += (e, er) =>
            {
                ((ClassBuilder)item.Owner).OwnedAttribute.Add(UMLReader.UmlReader.UmlFactory.Property());
                element.AddAttribute();
                //tb.Text= UMLReader.UmlReader.CreateAttributeText( item);
                model.Refresh();
            };
            tb.ContextMenu.Items.Add(menuitem);
            return tb;
        }

        public static TextBox EnumBuilder(EnumerationLiteralBuilder enumerationLiteral,Objects.Node element, ViewModel.IRefreshable model)
        {
            var tb = new TextBox();
            StyleTexbox(tb);
            tb.Text= CreateEnumText( enumerationLiteral);
            tb.LostFocus += (e, er) =>
            {
                InlineParser.NameParser(tb.Text, enumerationLiteral);
                tb.Text=CreateEnumText( enumerationLiteral);
                model.Refresh();
            };
            tb.ContextMenu = new ContextMenu();

            var menuitem = new MenuItem { Header = "Remove" };
            menuitem.Click += (e, er) =>
            {
                element.RemoveLiteral(tb,enumerationLiteral);
                ((EnumerationBuilder)(enumerationLiteral.Owner)).OwnedLiteral.Remove(enumerationLiteral);
                model.Refresh();
            };
            tb.ContextMenu.Items.Add(menuitem);
            menuitem.Header = "Add";
            menuitem.Click += (e, er) =>
            {
                element.AddLiteral();
                //tb.Text= UMLReader.UmlReader.CreateAttributeText( item);
                model.Refresh();
            };
            tb.ContextMenu.Items.Add(menuitem);

            return tb;
        }

        private static string CreateEnumText( EnumerationLiteralBuilder enumerationLiteral)
        {
            return enumerationLiteral.Name;
        }
    }
}