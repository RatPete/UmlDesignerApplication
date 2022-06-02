using MetaDslx.Languages.Uml.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace WpfDiagramDesigner
{
    static class InlineParser
    {
        public static bool FunctionParse(string input,OperationBuilder item)
        {
            VisibilityKind vis=item.Visibility;
            var oldItem = item;
            input = input.Trim();
            switch (input[0])
            {
                case '+':item.Visibility=VisibilityKind.Public; break;
                case '-':item.Visibility=VisibilityKind.Private; break;
                case '~':item.Visibility=VisibilityKind.Package; break;
                case '#':item.Visibility=VisibilityKind.Protected; break;
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
            string returnType = splits[1].Trim(' ', ':');
            if (returnType == "")
            {
                item.Visibility = vis;
                return false;
            }
            if (UMLReader.UmlReader.FindClassByName(returnType.Trim()) != null)
            {
                var parameter = UMLReader.UmlReader.UmlFactory.Parameter();
                parameter.Direction = ParameterDirectionKind.Return;
                parameter.Operation = item;
                parameter.Type = UMLReader.UmlReader.FindClassByName(returnType.Trim());
            }
            string[] inputParams = splits[0].Split(',');
            item.OwnedParameter.Clear();
            foreach (var inputParam in inputParams)
            {
                var parts = inputParam.Split(":");
                if (parts.Length != 2)
                {
                    item.Visibility = vis;
                    return false;
                }
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
                }
            }
            return true;
        }
        public static bool AttributeParse(string input,PropertyBuilder item)
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
            var parts = input.Split(":");
            if (parts.Length != 2)
            {
                item.Visibility=vis;
                return false;
            }
            if ((!ValidName(parts[0].Trim())&&parts[0].Length!=0) || !ValidName(parts[1].Trim()))
            {
                item.Visibility = vis;
                return false;
            }
            if (UMLReader.UmlReader.FindClassByName(parts[1].Trim())!=null)
            {
                item.Name = parts[0].Trim();
                item.Type = UMLReader.UmlReader.FindClassByName(parts[1].Trim());
            }
            return true;
        }
        public static void NameParser(string name, NamedElementBuilder element)
        {
            if (ValidName(name))
            {
                element.Name = name;
                
            }
        }
        private static bool ValidName(string name)
        {
            name = name.Trim();
            if (name.Length == 0)
                return false;
            char c = name[0];

            if (!((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_'))
                return false;
            var name2 = name.Substring(1);
            foreach (char c_name in name2)
            {
                if (!((c_name >= '0' && c_name <= '9') || (c_name >= 'A' && c_name <= 'Z') || (c_name >= 'a' && c_name <= 'z') || c_name == '_'))
                {
                    return false;
                }
            }
            return true;
        }
    }
   


}
