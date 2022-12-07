using MetaDslx.GraphViz;
using MetaDslx.Languages.Uml.Model;
using MetaDslx.Languages.Uml.Serialization;
using MetaDslx.Modeling;
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
        private static int objectCounter = 0;
        private static List<string> InitialPrimitives { get; set; } = new List<string>(new string[] { "int", "bool", "string", "char", "double", "long", "float", "void" });
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
            if (item.OwnedParameter.Where(item => item.Direction == ParameterDirectionKind.Return).ToList().Count == 0)
            {
                elementText += "void";
            }
            else
            {
                var enumerator = item.OwnedParameter.FirstOrDefault(item => item.Direction == ParameterDirectionKind.Return);
                elementText += enumerator?.Type?.Name;
            }
            return elementText;
        }

        public static List<PrimitiveTypeBuilder> GetAllPrimitives()
        {
            return model.Objects.OfType<PrimitiveTypeBuilder>().Where(item => item.Name != null && item.Name != "").ToList();
        }
        public static void CreatePrimitive(string primitiveName)
        {
            if (FindClass(primitiveName) != null || FindEnum(primitiveName) != null || FindInterface(primitiveName) != null || FindPrimitive(primitiveName) != null)
            {
                throw new ObjectNotParsableException("Ez a név már foglalt");
            }
            var newPrimitive = UmlFactory.PrimitiveType();

            newPrimitive.Name = primitiveName;
        }
        public static void CreateClass(string className = "")
        {
            var newClass = UmlFactory.Class();
            if (className == "")
            {
                newClass.Name = "NewClass" + objectCounter++;
            }
            else
            {
                if (model.Objects.OfType<NamedElementBuilder>().Any(item => item.Name == className))
                {
                    throw new ObjectNameAlreadyTakenException("A megadott név már foglalt: " + className);
                }
                newClass.Name = className;
            }
        }
        public static bool CreateClass(string className, List<string> Attributes, List<string> Functions)
        {
            if (FindClass(className) != null || FindEnum(className) != null || FindInterface(className) != null || FindPrimitive(className) != null)
            {
                throw new ObjectNotParsableException("Ez a név már foglalt");
            }
            foreach (var item in Attributes)
            {
                try
                {
                    InlineParser.CanParseAttribute(item);


                }
                catch (ObjectNotParsableException ex)
                {
                    throw new ObjectNotParsableException("Az alábbi attribútum feldolgozása közben hiba következett be:\n" + item + "\nBelső hibaüzenet:\n" + ex.Message);
                }
            }
            foreach (var item in Functions)
            {
                try
                {
                    InlineParser.CanParseFunction(item);
                }
                catch (ObjectNotParsableException ex)
                {
                    throw new ObjectNotParsableException("Az alábbi függvény feldolgozása közben hiba következett be:\n" + item + "\nBelső hibaüzenet:\n" + ex.Message);
                }
            }
            var newClass = UmlFactory.Class();

            newClass.Name = className;
            foreach (var item in Attributes)
            {
                PropertyBuilder property = CreateAttribute();
                if (!InlineParser.AttributeParse(item, property))
                {
                    RemoveElementFromModel(newClass);
                    throw new ObjectNotParsableException("Kérlek javítsd az elutasított osztálynevű property-t majd ments újra");
                }
                newClass.OwnedAttribute.Add(property);
            }
            foreach (var item in Functions)
            {
                OperationBuilder function = CreateFuntion();
                if (!InlineParser.FunctionParse(item, function))
                {

                    RemoveElementFromModel(newClass);
                    throw new ObjectNotParsableException("Kérlek javítsd az elutasított osztálynevű paramétert majd ments újra");
                }
                newClass.OwnedOperation.Add(function);
            }
            return true;
        }
        public static bool CreateInterface(string interfaceName, List<string> Functions)
        {
            if (FindClass(interfaceName) != null || FindEnum(interfaceName) != null || FindInterface(interfaceName) != null || FindPrimitive(interfaceName) != null)
            {
                throw new ObjectNotParsableException("Ez a név már foglalt");
            }
            foreach (var item in Functions)
            {
                try
                {
                    InlineParser.CanParseFunction(item);
                }
                catch (ObjectNotParsableException ex)
                {
                    throw new ObjectNotParsableException("Az alábbi függvény feldolgozása közben hiba következett be:\n" + item + "\nBelső hibaüzenet:\n" + ex.Message);
                }
            }
            var newInterace = UmlFactory.Interface();
            newInterace.Name = interfaceName;
            foreach (var item in Functions)
            {
                OperationBuilder function = CreateFuntion();
                if (!InlineParser.FunctionParse(item, function))
                {

                    RemoveElementFromModel(newInterace);
                    throw new ObjectNotParsableException("Kérlek javítsd az elutasított osztálynevű paramétert majd ments újra");
                }
                newInterace.OwnedOperation.Add(function);
            }
            return true;
        }
        public static bool CreateEnumeration(string enumerationName, List<string> Enumerations)
        {
            if (FindClass(enumerationName) != null || FindEnum(enumerationName) != null || FindInterface(enumerationName) != null || FindPrimitive(enumerationName) != null)
            {
                throw new ObjectNotParsableException("Ez a név már foglalt");
            }
            foreach (var item in Enumerations)
            {
                try
                {
                    InlineParser.CanParseEnum(item);
                }
                catch (ObjectNotParsableException ex)
                {
                    throw new ObjectNotParsableException("Az alábbi enumerációs literál feldolgozása közben hiba következett be:\n" + item + "\nBelső hibaüzenet:\n" + ex.Message);
                }
            }
            var enumeration = UmlFactory.Enumeration();
            enumeration.Name = enumerationName;
            foreach (var item in Enumerations)
            {
                EnumerationLiteralBuilder literal = CreateEnumerationLiteral();
                InlineParser.EnumParser(item, literal);
                enumeration.OwnedLiteral.Add(literal);
            }
            return true;
        }

        public static List<string> ListDependecies(ElementBuilder el)
        {
            List<string> allDependencies = new List<string>();
            var allInts = model.Objects.OfType<InterfaceRealizationBuilder>().Where(i => i.Client.Contains(el) || i.Supplier.Contains(el));
            if (allInts.Any())
            {
                allDependencies.Add("Interfész realizáció:");
            }
            foreach (var item in allInts)
            {
                allDependencies.Add("\t" + item?.Supplier.FirstOrDefault()?.Name + "<--" + item?.Client?.FirstOrDefault()?.Name);
            }
            var everyAssocInExistenceEVER = model.Objects.OfType<AssociationBuilder>().ToList();
            var allAssoc = model.Objects.OfType<AssociationBuilder>().Where(i =>(i.MemberEnd.Any(end=>(end as PropertyBuilder)?.Type==el ))|| i.Member.Contains(el) || el.OwnedElement.Any(element => i.Member.Contains(element))|| i.MemberEnd.Contains(el) || el.OwnedElement.Any(element => i.MemberEnd.Contains(element)) || i.OwnedEnd.Any(item => item.Type == el) || el.OwnedElement.Any(element => i.OwnedEnd.Contains(element)));
            if (allAssoc.Any())
            {
                allDependencies.Add("Asszociáció");
            }
            foreach (var item in allAssoc)
            {
                if (item?.MemberEnd.Count == 2)
                {
                    allDependencies.Add("\t" + item?.MemberEnd[0]?.Type?.Name + "<-->" + item?.MemberEnd[1].Type?.Name);
                }
                else
                {
                    if (item.MemberEnd.Count == 1 && item.Member.Count == 1)
                    {
                        allDependencies.Add("\t" + item?.MemberEnd[0]?.Type?.Name + "<-->" + (item?.Member.ToArray()[0]?.Owner as NamedElementBuilder)?.Name);
                    }
                }
            }
            var allGen = model.Objects.OfType<GeneralizationBuilder>().Where(i => i.General == el || i.Specific == el);
            if (allGen.Any())
            {
                allDependencies.Add("Öröklés:");
            }
            foreach (var item in allGen)
            {
                allDependencies.Add("\t" + item?.General?.Name + "<--" + item?.Specific?.Name);
            }
            var alldeps = model.Objects.OfType<DependencyBuilder>().Where(i => !(i.GetType() is AssociationBuilder) && !(i.GetType() is InterfaceRealizationBuilder) && !(i.GetType() is GeneralizationBuilder) && (i.Client.Contains(el) || i.Supplier.Contains(el)));
            if (alldeps.Any())
            {
                allDependencies.Add("Dependencia:");
            }
            foreach (var item in alldeps)
            {
                allDependencies.Add("\t" + item?.Supplier.FirstOrDefault()?.Name + "<--" + item?.Client?.FirstOrDefault()?.Name);
            }
            var allProperties = model.Objects.OfType<PropertyBuilder>().Where(i => i.Type?.Name == (el as NamedElementBuilder)?.Name);
            if (allProperties.Any(item =>item.Owner!=null&& item.Owner.MName != null && item.Owner.MName != ""))
            {
                allDependencies.Add("Attribútumok:");
            }
            foreach (var item in allProperties)
            {
                if (item.Owner!=null&&item.Owner.MName != null && item.Owner.MName != "")
                    allDependencies.Add("\t" + "Az " + item.Owner.MName + " objektum attribútuma:\n\t\t" + item.Name + ":" + item.Type.Name);
            }
            var allFunctions = model.Objects.OfType<ParameterBuilder>().Where(i => i.Type?.Name == (el as NamedElementBuilder)?.Name);
            if (allFunctions.Any())
            {
                allDependencies.Add("Függvények:");
            }
            foreach (var item in allFunctions)
            {

                allDependencies.Add("\t" + "Az " + item.Operation?.Owner?.MName + " objektum függvénye:\n\t\t" + item.Operation?.Name);
            }
            return allDependencies;
        }

        public static void CreateEnum()
        {
            var newEnum = UmlFactory.Enumeration();
            newEnum.Name = "NewEnum" + objectCounter++;
        }

        public static void CreateInterface()
        {

            var inter = UmlFactory.Interface();
            inter.Name = "NewInterface" + objectCounter++;
        }

        public static string CreateAttributeText(PropertyBuilder item)
        {
            if (item == null)
                return "";
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
            var masterNode = g.AddNode("MasterNode");
            foreach (var cls in model.Objects.OfType<ClassBuilder>())
            {
                if (cls.Name == null)
                    continue;
                var temp = g.AddNode(cls);
                string longest = cls.Name == null ? "temp" : cls.Name;
                int maxLength = longest.Length;
                if (cls.OwnedAttribute.Contains(null))
                {
                    cls.OwnedAttribute.Remove(null);
                }
                foreach (var att in cls.OwnedAttribute)
                {
                    if (att == null)
                        continue;
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
                oneElementSize.Height += 3;
                oneElementSize.Height += 3;
                oneElementSize.Height += 3;
                Point2D prefSize = new Point2D(oneElementSize.Width, oneElementSize.Height);
                temp.PreferredSize = prefSize;
                if (!model.Objects.OfType<RelationshipBuilder>().Any(item => item.RelatedElement != null && item.RelatedElement.Count != 0 && item.RelatedElement.Contains(cls)))
                {
                    g.AddEdge(cls, masterNode.NodeObject, "Master" + cls.Name + "shadow");
                }
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
                oneElementSize.Height += 6;
                Point2D prefSize = new Point2D(oneElementSize.Width, oneElementSize.Height);
                temp.PreferredSize = prefSize;
                if (!model.Objects.OfType<RelationshipBuilder>().Any(item => item.RelatedElement.Contains(en)))
                {
                    g.AddEdge(en, masterNode.NodeObject, "Master" + en.Name + "shadow");
                }
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
                oneElementSize.Height += 6;
                Point2D prefSize = new Point2D(oneElementSize.Width, oneElementSize.Height);
                temp.PreferredSize = prefSize;
                if (!model.Objects.OfType<RelationshipBuilder>().Any(item => item.RelatedElement.Contains(intf)))
                {
                    g.AddEdge(intf, masterNode.NodeObject, "Master" + intf.Name + "shadow");
                }
            }
            foreach (var ir in model.Objects.OfType<InterfaceRealizationBuilder>())
            {
                var first = ir.Client.FirstOrDefault()?.MName;
                var second = ir.Supplier.FirstOrDefault()?.MName;
                var resFirst = g.AllNodes.Where(i => (i.NodeObject as NamedElementBuilder)?.Name?.ToString() == first).FirstOrDefault()?.NodeObject as NamedElementBuilder;
                var resSecond = g.AllNodes.Where(i => (i.NodeObject as NamedElementBuilder)?.Name?.ToString() == second).FirstOrDefault()?.NodeObject as NamedElementBuilder;
                if (resFirst != null && resSecond != null && resFirst?.Name?.ToString() == first && resSecond?.ToString() == second)
                {
                    g.AddEdge(resFirst, resSecond, ir);
                }
            }
            foreach (var gen in model.Objects.OfType<GeneralizationBuilder>())
            {
                var first = gen.Specific?.Name;
                var second = gen.General?.Name;
                var resFirst = g.AllNodes.Where(i => (i.NodeObject as NamedElementBuilder)?.Name?.ToString() == first).FirstOrDefault()?.NodeObject as NamedElementBuilder;
                var resSecond = g.AllNodes.Where(i => (i.NodeObject as NamedElementBuilder)?.Name?.ToString() == second).FirstOrDefault()?.NodeObject as NamedElementBuilder;
                if (resFirst != null && resSecond != null && resFirst?.Name.ToString() == first && resSecond?.Name?.ToString() == second)
                    g.AddEdge(resFirst, resSecond, gen);

            }
            foreach (var assoc in model.Objects.OfType<AssociationBuilder>().Where(item => item.MemberEnd != null && item.MemberEnd.Count > 1))
            {

                var first = "";
                var second = "";
                if (assoc.MemberEnd.Count == 0)
                    continue;
                first = assoc.MemberEnd[0]?.Type?.Name;
                second = assoc.MemberEnd[1]?.Type?.Name;


                if (g.AllNodes.Any(i => (i.NodeObject as NamedElementBuilder)?.Name.ToString() == second) && g.AllNodes.Any(i => (i.NodeObject as NamedElementBuilder)?.Name.ToString() == first))
                    if (!g.AllEdges.Any(i => (i.Source.NodeObject as NamedElementBuilder)?.Name.ToString() == first && (i.Source.NodeObject as NamedElementBuilder)?.Name.ToString() == second && i.EdgeObject is AssociationBuilder))
                        g.AddEdge(g.AllNodes.Where(i => (i.NodeObject as NamedElementBuilder)?.Name.ToString() == second).FirstOrDefault().NodeObject, g.AllNodes.Where(i => (i.NodeObject as NamedElementBuilder)?.Name.ToString() == first).FirstOrDefault().NodeObject, assoc);
            }
            foreach (var dep in model.Objects.OfType<DependencyBuilder>())
            {
                var first = dep.Supplier.FirstOrDefault();
                var second = dep.Client.FirstOrDefault();
                if (first == null || second == null)
                {
                    continue;

                }
                if (!g.AllEdges.Any(i => (i.Source.NodeObject as NamedElementBuilder)?.Name.ToString() == second?.Name && (i.Target.NodeObject as NamedElementBuilder)?.Name.ToString() == first?.Name && (i.EdgeObject is DependencyBuilder)))
                    if (!g.AllEdges.Any(i => (i.Source.NodeObject as NamedElementBuilder)?.Name.ToString() == second?.Name && (i.Target.NodeObject as NamedElementBuilder)?.Name.ToString() == first?.Name && (i.EdgeObject is InterfaceRealizationBuilder)))
                    {
                        var edge = g.AddEdge(g.AllNodes.Where(i => (i.NodeObject as NamedElementBuilder)?.Name.ToString() == second?.Name).FirstOrDefault().NodeObject, g.AllNodes.Where(i => (i.NodeObject as NamedElementBuilder)?.Name.ToString() == first?.Name).FirstOrDefault().NodeObject, dep);
                    }
            }
            g.NodeSeparation = 30;
            g.RankSeparation = 20;
            g.NodeMargin = 10;
            g.EdgeLength = 10;

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
            List<MutableObject> objectsToBeRemoved = new List<MutableObject>();
            objectsToBeRemoved.Add(el);
            var allInts = model.Objects.OfType<InterfaceRealizationBuilder>().Where(i => i.Client.Contains(el) || i.Supplier.Contains(el));
            foreach (var item in allInts)
            {
                objectsToBeRemoved.Add(item);
                //model.RemoveObject(item);
            }
            var allAssoc = model.Objects.OfType<AssociationBuilder>().Where(i => (i.MemberEnd.Any(end => (end as PropertyBuilder)?.Type == el)) || i.Member.Contains(el)|| el.OwnedElement.Any(element=>i.Member.Contains(element))|| i.MemberEnd.Contains(el) || el.OwnedElement.Any(element => i.MemberEnd.Contains(element)) || i.OwnedEnd.Any(item => item.Type == el) || el.OwnedElement.Any(element => i.OwnedEnd.Contains(element)));
            foreach (var item in allAssoc)
            {
                objectsToBeRemoved.Add(item);
                //model.RemoveObject(item);
            }
            var allGen = model.Objects.OfType<GeneralizationBuilder>().Where(i => i.General == el || i.Specific == el);
            foreach (var item in allGen)
            {
                objectsToBeRemoved.Add(item);
                //model.RemoveObject(item);
            }
            var alldeps = model.Objects.OfType<DependencyBuilder>().Where(i => i.Client.Contains(el) || i.Supplier.Contains(el));
            foreach (var item in alldeps)
            {
                objectsToBeRemoved.Add(item);
                //model.RemoveObject(item);
            }
            var allProperties = model.Objects.OfType<PropertyBuilder>().Where(i => i.Type ==el);
            foreach (var item in allProperties)
            {
                objectsToBeRemoved.Add(item);
                (item.Owner as ClassBuilder)?.OwnedAttribute.Remove(item);
                //model.RemoveObject(item);
            }
            var allFunctions = model.Objects.OfType<ParameterBuilder>().Where(i => i.Type?.Name == (el as NamedElementBuilder)?.Name);
            foreach (var item in allFunctions)
            {
                objectsToBeRemoved.Add(item);
                //model.RemoveObject(item);
            }
            foreach (var item in objectsToBeRemoved)
            {
                model.RemoveObject(item);
            }

            model.PurgeWeakObjects();

        }

        public static GraphLayout LayoutReader(string inputURL)
        {

            UmlDescriptor.Initialize();
            string[] parts = inputURL.Split(".");
            if (inputURL == "")
            {
                model = new MetaDslx.Modeling.MutableModel();
                UmlFactory = new UmlFactory(model);
                var newPrimitives = model.Objects.OfType<PrimitiveTypeBuilder>();
                if (!newPrimitives.Any())
                {
                    foreach (var item in InitialPrimitives)
                    {
                        var primitive = UmlFactory.PrimitiveType();
                        primitive.Name = item;
                    }

                }
                return null;
            }
            if (parts[1].ToLower() == "uml")
            {
                var umlSerializer = new WhiteStarUmlSerializer();
                try
                {
                    model = umlSerializer.ReadModelFromFile(inputURL, out var diagnostics).ToMutable();
                }
                catch (System.Xml.XmlException ex)
                {
                    throw ex;
                }
            }
            else
            {
                var umlSerializer = new UmlXmiSerializer();
                try
                {
                    model = umlSerializer.ReadModelFromFile(inputURL).ToMutable();
                }
                catch (System.Xml.XmlException ex)
                {
                    throw ex;
                }
            }
            UmlFactory = new UmlFactory(model);
            var currentPrimitives = model.Objects.OfType<PrimitiveTypeBuilder>();
            if (currentPrimitives.Any())
            {
                foreach (var item in InitialPrimitives)
                {
                    var primitive = UmlFactory.PrimitiveType();
                    primitive.Name = item;
                }

            }
            return RefreshLayout();
        }
        public static void WriteOut(string output)
        {
            var xmiSerializer = new UmlXmiSerializer();
            xmiSerializer.WriteModelToFile(output, model, new UmlXmiWriteOptions { RequireXmiRoot = true });
        }

        public static TypeBuilder FindClassByName(string name, bool createnew = false)
        {
            if (name == null || name == "") return null;
            var cls = FindClass(name);
            if (cls != null)
                return cls;
            var enu = FindEnum(name);
            if (enu != null)
                return enu;
            var inte = FindInterface(name);
            if (inte != null)
                return inte;
            var primitive = FindPrimitive(name);
            if (primitive != null)
                return primitive;
            if (!createnew)
            {
                throw new ClassNotFoundException("A megadott névvel nem található objektum: " + name);
            }
            else
            {

                var type = UmlFactory.Class();
                type.Name = name;



                return type;
            }
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
                if (cls != null && cls?.Name?.ToLower() == name.ToLower())
                    return cls;
            return null;
        }
        private static EnumerationBuilder FindEnum(string name)
        {
            foreach (var enu in model.Objects.OfType<EnumerationBuilder>())
            {
                if (enu != null && enu?.Name?.ToLower() == name.ToLower())
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
                if (inte != null && inte?.Name?.ToLower() == name.ToLower())
                {
                    return inte;
                }
            }
            return null;
        }
        private static TypeBuilder FindPrimitive(string name)
        {
            foreach (var primitive in model.Objects.OfType<PrimitiveTypeBuilder>())
            {
                if (primitive != null && primitive?.Name?.ToLower() == name.ToLower())
                {
                    return primitive;
                }
            }
            return null;
        }
        public static void CreateOneWayAssociation(string startNode, string endNode)
        {
            CreateOneWayAssocWithEndReturn(startNode, endNode);
        }

        public static void CreateAssociation(string startNode, string endNode)
        {
            var assoc = UmlFactory.Association();

            var cls1 = FindClass(startNode);
            var cls2 = FindClass(endNode);
            var any1 = FindClassByName(startNode);
            var any2 = FindClassByName(endNode);
            if (cls1 == null)
                if (any1 != null)
                    throw new ClassNotFoundException("Első csomópontnak osztálynak kellene lennie");
                else
                    throw new ClassNotFoundException("Az első csomópont nem létezik a modellben");
            if (cls2 == null)
                if (any2 != null)
                    throw new ClassNotFoundException("Második csomópontnak osztálynak kellene lennie");
                else
                    throw new ClassNotFoundException("A második csomópont nem létezik a modellben");
            var end1 = UmlFactory.Property();
            end1.Type = cls1;
            var end2 = UmlFactory.Property();
            end2.Type = cls2;

            assoc.MemberEnd.Add(end1);
            assoc.MemberEnd.Add(end2);
            cls1.OwnedAttribute.Add(end2);
            cls2.OwnedAttribute.Add(end1);
        }
        public static void CreateAggregation(string startNode, string endNode)
        {
            var end2 = CreateOneWayAssocWithEndReturn(startNode, endNode);
            end2.Aggregation = AggregationKind.Shared;
        }
        public static PropertyBuilder CreateOneWayAssocWithEndReturn(string startNode, string endNode)
        {
            var assoc = UmlFactory.Association();
            var cls2 = FindClass(endNode);
            var any1 = FindClassByName(startNode);
            var any2 = FindClassByName(endNode);
            if (cls2 == null)
                if (any2 != null)
                    throw new ClassNotFoundException("A második csomópontnak osztálynak kell lennie");
                else
                    throw new ClassNotFoundException("A második csomópont nem létezik");
            if (any1 == null)
                throw new ClassNotFoundException("Az első csomópont nem létezik");
            var end1 = UmlFactory.Property();
            end1.Type = any1;
            var end2 = UmlFactory.Property();
            end2.Type = cls2;
            assoc.OwnedEnd.Add(end2);
            assoc.MemberEnd.Add(end1);
            cls2.OwnedAttribute.Add(end1);
            return end2;
        }
        public static void CreateComposition(string startNode, string endNode)
        {
            var end2 = CreateOneWayAssocWithEndReturn(startNode, endNode);
            end2.Aggregation = AggregationKind.Composite;
        }
        public static void CreateRealization(string startNode, string endNode)
        {
            var cls1 = FindClassByName(startNode);
            var cls2 = FindInterface(endNode);
            if (cls1 == null || cls2 == null)
            {
                throw new ClassNotFoundException("Az inheritence nem jött létre mert valamelyik végpont nem felel meg az elvárásnak");
            }
            var rel = UmlFactory.InterfaceRealization();
            rel.Client.Add(cls1);
            rel.Supplier.Add(cls2);
        }
        public static void CreateInheritance(string startNode, string endNode)
        {
            var cls1 = FindClass(startNode);
            var cls2 = FindClass(endNode);
            var any1 = FindClassByName(startNode);
            var any2 = FindClassByName(endNode);
            if (cls1 == null)
                if (any1 != null)
                    throw new ClassNotFoundException("Első csomópontnak osztálynak kellene lennie");
                else
                    throw new ClassNotFoundException("Az első csomópont nem létezik a modellben");
            if (cls2 == null)
                if (any2 != null)
                    throw new ClassNotFoundException("Második csomópontnak osztálynak kellene lennie");
                else
                    throw new ClassNotFoundException("A második csomópont nem létezik a modellben");

            var rel = UmlFactory.Generalization();
            cls1.Generalization.Add(rel);
            rel.Specific = cls1;
            rel.General = cls2;
        }
        public static void CreateDependency(string startNode, string endNode)
        {
            var cls1 = FindClassByName(startNode);
            var cls2 = FindClassByName(endNode);
            if (cls1 == null || cls2 == null)
            {
                throw new ClassNotFoundException("Dependencia létrehozása nem sikerült mert valamelyik csomópont nem található");
            }
            var dep = UmlFactory.Dependency();
            dep.Client.Add(cls1);
            dep.Supplier.Add(cls2);
        }
    }
}
