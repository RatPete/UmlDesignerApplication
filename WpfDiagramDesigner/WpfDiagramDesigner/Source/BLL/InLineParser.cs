using MetaDslx.Languages.Uml.Model;
using System.Linq;
using WpfDiagramDesigner.Source.PRL.Helper;
using WpfDiagramDesigner.Source.PRL.Views;

namespace WpfDiagramDesigner
{
    public static class InlineParser
    {
        public static bool FunctionParse(string input, OperationBuilder item)
        {
            VisibilityKind vis = item.Visibility;
            var oldItem = item;
            input = input.Trim();
            switch (input[0])
            {
                case '+': item.Visibility = VisibilityKind.Public; break;
                case '-': item.Visibility = VisibilityKind.Private; break;
                case '~': item.Visibility = VisibilityKind.Package; break;
                case '#': item.Visibility = VisibilityKind.Protected; break;
                default: item = oldItem; return false;

            }
            input = input.Substring(1).TrimStart();
            string[] splits = input.Split("(");
            if (splits.Length != 2)
            {
                item.Visibility = vis;
                return false;
            }
            item.Name = splits[0];
            splits = splits[1].Split(")");
            if (splits.Length != 2)
            {
                item.Visibility = vis;
                return false;
            }
            item.OwnedParameter.Clear();
            string returnType = splits[1].Trim(' ', ':');
            if (returnType == "")
            {
                item.Visibility = vis;
                return false;
            }
            try
            {
                if (UMLReader.UmlReader.FindClassByName(returnType.Trim()) != null)
                {
                    var parameter = UMLReader.UmlReader.UmlFactory.Parameter();
                    parameter.Direction = ParameterDirectionKind.Return;
                    parameter.Operation = item;
                    parameter.Type = UMLReader.UmlReader.FindClassByName(returnType.Trim());

                    item.OwnedParameter.Add(parameter);
                }
            }
            catch (ClassNotFoundException ex)
            {
                var popup = new NewTypeSelectorPopup(ex.Message, PopupGlobalPosition.Position, returnType.Trim());
                var returnResult = popup.ShowDialog();
                if (returnResult.HasValue && returnResult.Value)
                {
                    var parameter = UMLReader.UmlReader.UmlFactory.Parameter();
                    parameter.Direction = ParameterDirectionKind.Return;
                    parameter.Operation = item;

                    parameter.Type = UMLReader.UmlReader.FindClassByName(returnType.Trim());

                    item.OwnedParameter.Add(parameter);
                }
                else
                {
                    return false;
                }
            }
            string[] inputParams = splits[0].Split(',');
            foreach (var inputParam in inputParams)
            {
                var parts = inputParam.Split(":");
                if (parts.Length != 2)
                {
                    item.Visibility = vis;
                    return false;
                }
                try
                {
                    if (UMLReader.UmlReader.FindClassByName(parts[1].Trim()) != null)
                    {
                        ParameterBuilder param = UMLReader.UmlReader.UmlFactory.Parameter();



                        if (!ValidName(parts[0].Trim()) || !ValidName(parts[1].Trim()))
                        {
                            item.Visibility = vis;
                            return false;
                        }

                        param.Name = parts[0];
                        param.Direction = ParameterDirectionKind.In;
                        param.Operation = item;
                        param.Type = UMLReader.UmlReader.FindClassByName(parts[1].Trim());

                        item.OwnedParameter.Add(param);
                    }
                }
                catch(ClassNotFoundException ex)
                {
                    var popup = new NewTypeSelectorPopup(ex.Message, PopupGlobalPosition.Position, parts[1].Trim());
                    var returnResult = popup.ShowDialog();
                    if (returnResult.HasValue && returnResult.Value)
                    {
                        ParameterBuilder param = UMLReader.UmlReader.UmlFactory.Parameter();



                        if (!ValidName(parts[0].Trim()) || !ValidName(parts[1].Trim()))
                        {
                            item.Visibility = vis;
                            return false;
                        }

                        param.Name = parts[0];
                        param.Direction = ParameterDirectionKind.In;
                        param.Operation = item;
                        param.Type = UMLReader.UmlReader.FindClassByName(parts[1].Trim());

                        item.OwnedParameter.Add(param);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public static bool AttributeParse(string input, PropertyBuilder item, bool createNew = false)
        {
            if (item == null)
                return false;
            string originalInput = input;
            if (input == null || input.Length == 0)
                return false;
            VisibilityKind vis = item.Visibility;
            var oldItem = item;
            input = input.Trim();
            switch (input[0])
            {
                case '+': item.Visibility = VisibilityKind.Public; break;
                case '-': item.Visibility = VisibilityKind.Private; break;
                case '~': item.Visibility = VisibilityKind.Package; break;
                case '#': item.Visibility = VisibilityKind.Protected; break;
                default: item = oldItem; return false;
            }
            input = input.Substring(1).TrimStart();
            var parts = input.Split(":");
            if (parts.Length != 2)
            {
                item.Visibility = vis;
                return false;
            }
            if ((!ValidName(parts[0].Trim()) && parts[0].Length != 0) || !ValidName(parts[1].Trim()))
            {
                item.Visibility = vis;
                return false;
            }
            try
            {
                if (UMLReader.UmlReader.FindClassByName(parts[1].Trim()) != null)
                {
                    item.Name = parts[0].Trim();
                    item.Type = UMLReader.UmlReader.FindClassByName(parts[1].Trim());
                }
            }
            catch (ClassNotFoundException ex)
            {
                var popup = new NewTypeSelectorPopup(ex.Message, PopupGlobalPosition.Position, parts[1].Trim());
                var returnResult = popup.ShowDialog();
                if (returnResult.HasValue && returnResult.Value)
                {
                    AttributeParse(originalInput, item);
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        public static bool CanParseAttribute(string input)
        {
            input = input.Trim();
            if (input.Length == 0)
                throw new ObjectNotParsableException("Az attribútum üres így nem parseolható");
            string chars = "+-#~";
            if (!chars.Contains(input[0]))
            {
                throw new ObjectNotParsableException("Az attribútum feldolgozása sikertelen, nem volt megtalálható a láthatóság jelző");
                return false;
            }
            input = input.Substring(1).TrimStart();
            var parts = input.Split(":");
            if (parts.Length != 2)
            {

                throw new ObjectNotParsableException("Az attribútum feldolgozása sikertelen, nem pontosan egy \":\" karakter taláható meg az inputfieldben");
                return false;
            }
            try
            {
                ValidName(parts[0].Trim());
                ValidName(parts[1].Trim());
            }
            catch (ObjectNotParsableException e)
            {
                throw new ObjectNotParsableException("Az attribútum feldolgozása sikertelen,nem felel meg az attribútum neve vagy megadott osztálya az általános névszabályoknak. Belső hibaüzenet:" + e.Message);
                return false;
            }

            return true;
        }
        public static bool CanParseFunction(string input)
        {
            input = input.Trim();
            string chars = "+-#~";
            if (input == null || input.Length == 0)
                throw new ObjectNotParsableException("A beviteli mező üres");
            if (!chars.Contains(input[0]))
                throw new ObjectNotParsableException ("A kezdeti karakter nem tartalmazza a láthatóságot");
            input = input.Substring(1).TrimStart();
            string[] splits = input.Split("(");
            if (splits.Length != 2)
            {
                throw new ObjectNotParsableException("A függvény feldolgozása sikertelen,nem pontosan egy nyitó zárójel (\"(\") található meg a beviteli mezőben");
                return false;
            }
            splits = splits[1].Split(")");
            if (splits.Length != 2)
            {
                throw new ObjectNotParsableException("A függvény feldolgozása sikertelen,nem pontosan egy csukó zárójel (\")\") található meg a beviteli mezőben");
                return false;
            }
            string returnType = splits[1].Trim(' ', ':');
            if (returnType == "")
            {
                throw new ObjectNotParsableException("A függvény feldolgozása sikertelen, nem rendelkezik visszatérési értékkel, ha nincs visszatérési értéke akkor azt jelezd külön pl a \"void\" kulcsszó segítségével");
                return false;
            }
            var inputparams = splits[0].Trim();
            if (inputparams == null || inputparams.Length == 0)
                return true;
            string[] inputParams = inputparams.Split(',');

            foreach (var inputParam in inputParams)
            {
                var parts = inputParam.Split(":");
                if (parts.Length != 2)
                {
                    throw new ObjectNotParsableException($"A függvény feldolgozása sikertelen,a {inputParam} attribútum feldolgozása során nem sikerült a feldolgozást végrehajtani.");
                    return false;
                }

            }
            return true;
        }
        public static bool CanParseEnum(string input)
        {
            return ValidName(input);
        }
        public static void EnumParser(string name, EnumerationLiteralBuilder literal){
            try
            {
                if (ValidName(name))
                {
                    literal.Name = name;
                }
            }
            catch(ObjectNotParsableException e)
            {

            }

        }
        public static void NameParser(string name, NamedElementBuilder element)
        {
            try
            {
                if (ValidName(name))
                {
                    if (element.Name != null)
                        element.Name = name;

                }
            }
            catch (ObjectNotParsableException e)
            {

            }
        }
        private static bool ValidName(string name)
        {
            name = name.Trim();
            if (name.Length == 0)
                return false;
            char c = name[0];

            if (!(char.IsLetter(c)|| c == '_'))
            {
                throw new ObjectNotParsableException("A név első karaktere nem megfelelő");
                return false;
            }
            var name2 = name.ToLower().Substring(1);
            if (!name2.All(item => char.IsLetterOrDigit(item) || item == '_'))
            {

                throw new ObjectNotParsableException("A név illegális karaktert tartalmaz");
                return false;
            }
            //foreach (char c_name in name2)
            //{
            //    if (!((c_name >= '0' && c_name <= '9') || (c_name >= 'A' && c_name <= 'Z') || (c_name >= 'a' && c_name <= 'z') || c_name == '_'||c_name='é'||c_name=='á'||c_name=='ű'|| c_name == 'ú'||))
            //    {
            //        throw new ObjectNotParsableException("A név illegális karaktert tartalmaz");
            //        return false;
            //    }
            //}
            return true;
        }
    }



}
