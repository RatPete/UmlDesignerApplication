using MetaDslx.GraphViz;
using MetaDslx.Languages.Uml.Model;
using MetaDslx.Languages.Uml.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfDiagramDesigner.Source.PRL.Helper;

namespace WpfDiagramDesigner.UMLReader
{
    public static class UmlReader
    {
        private static MetaDslx.Modeling.MutableModel model;
        public static string CreateFunctionText(OperationBuilder item)
        {
            string elementText = "";
            if (item.Name == "")
            {
                return "";
            }
            switch (item.Visibility)
            {
                case VisibilityKind.Private: elementText += "-"; break;
                case VisibilityKind.Protected: elementText += "#"; break;
                case VisibilityKind.Package: elementText += "~"; break;
                case VisibilityKind.Public: elementText += "+"; break;
            }
            elementText += $"{item.Name}({(item.InputParameters().Count == 0 ? "" : (item.InputParameters()[0].Name) + $":{item.InputParameters()[0].Type.Name}")} ";
            ObservableCollection<ParameterBuilder> collection = new ObservableCollection<ParameterBuilder>();

            foreach (var param in item.InputParameters())
            {
                if (param == item.InputParameters()[0])
                    continue;
                elementText += $",{param.Name}:{param.Type.Name}";

            }
            elementText += $") : ";
            if (item.ReturnResult().Count == 0)
            {
                elementText += "void";
            }
            else
            {
                var enumerator = item.ReturnResult().GetEnumerator();
                enumerator.MoveNext();
                elementText += enumerator.Current.Type.Name;
            }
            return elementText;
        }

        

        public static void CreateClass()
        {
           var newClass= UmlFactory.Class();
            newClass.Name = "NewClass";
        }

        public static void CreateEnum()
        {
            var newEnum=UmlFactory.Enumeration();
            newEnum.Name = "NewEnum";
        }

        public static void CreateInterface()
        {
            
            var inter=UmlFactory.Interface();
            inter.Name = "NewInterface";
        }

        public static string CreateAttributeText(PropertyBuilder item)
        {
            string elementText = "";
            switch (item.Visibility)
            {
                case VisibilityKind.Private: elementText += "-"; break;
                case VisibilityKind.Protected: elementText += "#"; break;
                case VisibilityKind.Package: elementText += "~"; break;
                case VisibilityKind.Public: elementText += "+"; break;
            }

            elementText += $"{item.Name}:{item.Type?.Name}";
            return elementText;
        }
        public static GraphLayout RefreshLayout()
        {
            var g = new GraphLayout("dot");
            foreach (var cls in model.Objects.OfType<ClassBuilder>())
            {
                if (cls.Name == null)
                    continue;
                var temp = g.AddNode(cls);
                string longest = cls.Name==null? "temp":cls.Name;
                int maxLength = longest.Length;
                foreach (var att in cls.OwnedAttribute)
                {
                    string attribFull = CreateAttributeText(att);
                    longest = maxLength < attribFull.Length ? attribFull : longest;
                    maxLength = maxLength < attribFull.Length ? attribFull.Length : maxLength;

                }
                foreach (var item in cls.OwnedOperation)
                {
                    string elementText = CreateFunctionText(item);
                    longest = maxLength < elementText.Length ? elementText : longest;
                    maxLength = maxLength < elementText.Length ? elementText.Length : maxLength;
                }

                int preferedSize = cls.OwnedOperation.Count + cls.OwnedAttribute.Count + 1;
                Size oneElementSize = FormatedTextSize(longest);
                oneElementSize.Width += 10;
                oneElementSize.Height *= preferedSize;
                if (cls.OwnedAttribute.Count != 0)
                {
                    oneElementSize.Height += 3;
                }
                if (cls.OwnedOperation.Count != 0)
                {
                    oneElementSize.Height += 3;
                }
                if (cls.OwnedAttribute.Count != 0 || cls.OwnedOperation.Count != 0)
                {
                    oneElementSize.Height += 3;
                }
                Point2D prefSize = new Point2D(oneElementSize.Width, oneElementSize.Height);
                temp.PreferredSize = prefSize;
            }
            foreach (var en in model.Objects.OfType<EnumerationBuilder>())
            {
                if (en.Name == null)
                    continue;
                var temp = g.AddNode(en);
                string longest = en.Name == null ? "temp" : en.Name;
                int maxLength = longest.Length;

                foreach (var att in en.OwnedLiteral)
                {
                    longest = maxLength < att.Name?.Length ? att.Name : longest;
                    maxLength = maxLength < att.Name?.Length ? att.Name.Length : maxLength;
                }
                Size oneElementSize = FormatedTextSize(longest);
                oneElementSize.Width += 10;
                oneElementSize.Height *= (en.OwnedLiteral.Count + 1);
                if (en.OwnedAttribute.Count != 0)
                {
                    oneElementSize.Height += 3;
                }
                if (en.OwnedOperation.Count != 0)
                {
                    oneElementSize.Height += 3;
                }
                if (en.OwnedAttribute.Count != 0 || en.OwnedOperation.Count != 0)
                {
                    oneElementSize.Height += 3;
                }
                oneElementSize.Height += 6;
                Point2D prefSize = new Point2D(oneElementSize.Width, oneElementSize.Height);
                temp.PreferredSize = prefSize;
            }
            foreach (var intf in model.Objects.OfType<InterfaceBuilder>())
            {
                if (intf.Name == null)
                    continue;
                var temp = g.AddNode(intf);
                string longest = intf.Name == null ? "temp" : intf.Name;
                int maxLength = longest.Length;
                foreach (var item in intf.OwnedOperation)
                {
                    string elementText = CreateFunctionText(item);
                    longest = maxLength < elementText.Length ? elementText : longest;
                    maxLength = maxLength < elementText.Length ? elementText.Length : maxLength;
                }
                int prefferedSize = intf.OwnedOperation.Count + 1;

                Size oneElementSize = FormatedTextSize(longest);
                oneElementSize.Width += 10;
                oneElementSize.Height *= prefferedSize;
                if (intf.OwnedAttribute.Count != 0)
                {
                    oneElementSize.Height += 3;
                }
                if (intf.OwnedOperation.Count != 0)
                {
                    oneElementSize.Height += 3;
                }
                if (intf.OwnedAttribute.Count != 0 || intf.OwnedOperation.Count != 0)
                {
                    oneElementSize.Height += 3;
                }
                Point2D prefSize = new Point2D(oneElementSize.Width, oneElementSize.Height);
                temp.PreferredSize = prefSize;
            }
            foreach (var ir in model.Objects.OfType<InterfaceRealizationBuilder>())
            {
                var first = ir.Client.FirstOrDefault()?.MName;
                var second = ir.Supplier.FirstOrDefault()?.MName;
                var resFirst = (NamedElementBuilder)g.AllNodes.Where(i => ((NamedElementBuilder)i.NodeObject)?.Name?.ToString() == first).FirstOrDefault()?.NodeObject;
                var resSecond = (NamedElementBuilder)g.AllNodes.Where(i => ((NamedElementBuilder)i.NodeObject)?.Name?.ToString() == second).FirstOrDefault()?.NodeObject;
                g.AddEdge(resFirst, resSecond, ir);
            }
            foreach (var gen in model.Objects.OfType<GeneralizationBuilder>())
            {
                var first = gen.Specific?.Name;
                var second = gen.General?.Name;
                var resFirst = (NamedElementBuilder)g.AllNodes.Where(i => ((NamedElementBuilder)i.NodeObject)?.Name?.ToString() == first).FirstOrDefault()?.NodeObject;
                var resSecond = (NamedElementBuilder)g.AllNodes.Where(i => ((NamedElementBuilder)i.NodeObject)?.Name?.ToString() == second).FirstOrDefault()?.NodeObject;
                if (resFirst != null && resSecond != null && resFirst?.Name.ToString() == first && resSecond?.Name?.ToString() == second)
                    g.AddEdge(resFirst, resSecond, gen);

            }
            foreach (var assoc in model.Objects.OfType<AssociationBuilder>())
            {
                var first = assoc.MemberEnd[0].Type?.Name;
                var second = assoc.MemberEnd[1].Type?.Name;
                if (g.AllNodes.Any(i => ((NamedElementBuilder)i.NodeObject).Name.ToString() == second) && g.AllNodes.Any(i => ((NamedElementBuilder)i.NodeObject).Name.ToString() == first))
                    if (!(g.AllEdges.Any(i => ((NamedElementBuilder)i.Source.NodeObject).Name.ToString() == first && ((NamedElementBuilder)i.Source.NodeObject).Name.ToString() == second && i.EdgeObject is AssociationBuilder)))
                        g.AddEdge(g.AllNodes.Where(i => ((NamedElementBuilder)i.NodeObject).Name.ToString() == second).FirstOrDefault().NodeObject, g.AllNodes.Where(i => ((NamedElementBuilder)i.NodeObject).Name.ToString() == first).FirstOrDefault().NodeObject, assoc);
            }
            foreach (var dep in model.Objects.OfType<DependencyBuilder>())
            {
                var first = dep.Supplier.FirstOrDefault();
                var second = dep.Client.FirstOrDefault();
                if (first == null || second == null)
                {
                    continue;

                }
                if (!(g.AllEdges.Any(i => ((NamedElementBuilder)i.Source.NodeObject)?.Name.ToString() == second?.Name && ((NamedElementBuilder)i.Target.NodeObject).Name.ToString() == first?.Name && (i.EdgeObject is DependencyBuilder))))
                    if (!(g.AllEdges.Any(i => ((NamedElementBuilder)i.Source.NodeObject)?.Name.ToString() == second?.Name && ((NamedElementBuilder)i.Target.NodeObject).Name.ToString() == first?.Name && (i.EdgeObject is InterfaceRealizationBuilder))))
                        g.AddEdge(g.AllNodes.Where(i => ((NamedElementBuilder)i.NodeObject)?.Name.ToString() == second?.Name).FirstOrDefault().NodeObject, g.AllNodes.Where(i => ((NamedElementBuilder)i.NodeObject)?.Name.ToString() == first?.Name).FirstOrDefault().NodeObject, dep);

            }
            g.NodeSeparation = 30;
            g.RankSeparation = 20;
            g.NodeMargin = 10;
            g.EdgeLength = 5;
            g.ComputeLayout();
            return g;
        }
        private static Size FormatedTextSize(string longest)
        {
            TextBox tb = new TextBox
            {
                Foreground = Brushes.Black,
                FontSize = 8.0,
                Padding = new System.Windows.Thickness(0),
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent,
                BorderThickness = new System.Windows.Thickness(0)
            };
            var formattedText = new FormattedText(
                longest,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(tb.FontFamily, tb.FontStyle, tb.FontWeight, tb.FontStretch),
                tb.FontSize,
                Brushes.Black,
                new NumberSubstitution(),
                1);
            return new Size(formattedText.Width, formattedText.Height);
        }
        public static void RemoveElementFromModel(ElementBuilder el)
        {
            
            var allInts = model.Objects.OfType<InterfaceRealizationBuilder>().Where(i => i.Client.Contains(el) || i.Supplier.Contains(el));
            foreach (var item in allInts)
            {
                model.RemoveObject(item);
            }
            var allAssoc = model.Objects.OfType<AssociationBuilder>().Where(i => i.MemberEnd.Contains(el));
            foreach (var item in allAssoc)
            {
                model.RemoveObject(item);
            }
            var allGen = model.Objects.OfType<GeneralizationBuilder>().Where(i => i.General == el || i.Specific == el);
            foreach (var item in allGen)
            {
                model.RemoveObject(item);
            }
            var alldeps = model.Objects.OfType<DependencyBuilder>().Where(i => i.Client.Contains(el) || i.Supplier.Contains(el));
            foreach (var item in alldeps)
            {
                model.RemoveObject(item);
            }
            var allProperties = model.Objects.OfType<PropertyBuilder>().Where(i => i.Type?.Name == ((NamedElementBuilder)el).Name);
            foreach (var item in allProperties)
            {
                model.RemoveObject(item);
            }
            var allFunctions = model.Objects.OfType<ParameterBuilder>().Where(i => i.Type?.Name == ((NamedElementBuilder)el).Name);
            foreach(var item in allFunctions)
            {
                model.RemoveObject(item);
            }
            model.RemoveObject(el);
        }

        public static GraphLayout LayoutReader(string inputURL)
        {

            UmlDescriptor.Initialize();
            string[] parts = inputURL.Split(".");
            if (inputURL == "")
            {
                model = new MetaDslx.Modeling.MutableModel();
                UmlFactory = new UmlFactory(model);
                return null;
            }
            if (parts[1].ToLower() == "uml")
            {
                var umlSerializer = new WhiteStarUmlSerializer();
                model = umlSerializer.ReadModelFromFile(inputURL,out var diagnostics).ToMutable();
            }
            else
            {
                var umlSerializer = new UmlXmiSerializer();
                model = umlSerializer.ReadModelFromFile(inputURL).ToMutable();
            }
            UmlFactory = new UmlFactory(model);
            return RefreshLayout();
        }
        public static void WriteOut(string output)
        {
            var xmiSerializer = new UmlXmiSerializer();
            xmiSerializer.WriteModelToFile(output, model);
        }
        public static TypeBuilder FindClassByName(string name)
        {
            if (name == null||name.ToLower() == "void" || name == "" ) return null;
            var cls = FindClass(name);
            if (cls != null)
                return cls;
            var enu = FindEnum(name);
            if (enu != null)
                return enu;
            var inte = FindInterface(name);
            if (inte != null)
                return inte;
            
            
            var type = UmlFactory.Class();
            type.Name = name;
            


            return type;
        }
        public static OperationBuilder CreateFuntion()
        {
            return UmlFactory.Operation();
        }
        public static PropertyBuilder CreateAttribute()
        {
            return UmlFactory.Property();
        }
        public static EnumerationLiteralBuilder CreateEnumerationLiteral()
        {
            return UmlFactory.EnumerationLiteral();
        }

        public static UmlFactory UmlFactory { get; private set; }
        private static ClassBuilder FindClass(string name)
        {
            foreach (var cls in model.Objects.OfType<ClassBuilder>())
                if (cls.Name.ToLower() == name.ToLower())
                    return cls;
            return null;
        }
        private static EnumerationBuilder FindEnum(string name)
        {
            foreach (var enu in model.Objects.OfType<EnumerationBuilder>())
            {
                if (enu.Name.ToLower() == name.ToLower())
                {
                    return enu;
                }
            }
            return null;
        }
        private static InterfaceBuilder FindInterface(string name)
        {
            foreach (var inte in model.Objects.OfType<InterfaceBuilder>())
            {
                if (inte.Name.ToLower() == name.ToLower())
                {
                    return inte;
                }
            }
            return null;
        }
        public static void CreateAssociation(string startNode,string endNode)
        {
            var assoc = UmlFactory.Association();
            var end1 = UmlFactory.Property(); // Kirajzoláskor ez van a cls1 oldalon, de valójában a cls2 hivatkozik rá
            var cls1 = FindClass(startNode);
            var cls2 = FindClass(endNode);
            var any1 = FindClassByName(startNode);
            var any2 = FindClassByName(endNode);
            if (cls1 == null)
                if (any1 != null)
                    throw new ClassNotFoundException("Node1 not class");
                else
                    throw new ClassNotFoundException("Node1 does not exist error");
            if (cls2 == null)
                if (any2 != null)
                    throw new ClassNotFoundException("Node2 not class");
                else
                    throw new ClassNotFoundException("Node2 does not exist error");

            end1.Type = cls1;
            var end2 = UmlFactory.Property(); // Kirajzoláskor ez van a cls2 oldalon, de valójában a cls1 hivatkozik rá
            end2.Type = cls2;
            // Ha mindkét irányba navigálható az asszociáció:
            assoc.MemberEnd.Add(end1);
            assoc.MemberEnd.Add(end2);
            cls1.OwnedAttribute.Add(end2); // ********** itt javítottam
            cls2.OwnedAttribute.Add(end1); // ********** itt javítottam
        }
        public static void CreateAggregation(string startNode, string endNode)
        {
            throw new NotImplementedException();
        }
        public static void CreateComposition(string startNode, string endNode)
        {
            throw new NotImplementedException();
        }
        public static void CreateInheritance(string startNode, string endNode)
        {
            var cls1 = FindClassByName(startNode);
            var cls2 = FindClassByName(endNode);
            if (cls1 == null || cls2 == null)
            {
                throw new ClassNotFoundException("Dependency creation not succesfull");
            }
            var rel = UmlFactory.InterfaceRealization();
            rel.Client.Add(cls1);
            rel.Supplier.Add(cls2);
        }
        public static void CreateRealization(string startNode, string endNode)
        {
            var cls1 = FindClass(startNode);
            var cls2 = FindClass(endNode);
            var any1 = FindClassByName(startNode);
            var any2 = FindClassByName(endNode);
            if (cls1 == null)
                if (any1 != null)
                    throw new ClassNotFoundException("Node1 not class");
                else
                    throw new ClassNotFoundException("Node1 does not exist error");
            if (cls2 == null)
                if (any2 != null)
                    throw new ClassNotFoundException("Node2 not class");
                else
                    throw new ClassNotFoundException("Node2 does not exist error");

            var rel = UmlFactory.Generalization();
            cls1    .Generalization.Add(rel);
            rel.Specific=cls1;
            rel.General=cls2;
        }
        public static void CreateDependency(string startNode, string endNode)
        {
            var cls1 = FindClassByName(startNode);
            var cls2 = FindClassByName(endNode);
            if (cls1 == null || cls2 == null)
            {
                throw new ClassNotFoundException("Dependency creation not succesfull");
            }
            var dep = UmlFactory.Dependency();
            dep.Client.Add(cls1);
            dep.Supplier.Add(cls2);
        }
    }
}
