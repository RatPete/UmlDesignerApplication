using System;
using System.Collections.Generic;
using System.Text;

namespace WpfDiagramDesigner.Source.PRL.Helper
{
    public static class RelationshipCreator
    {
        private static ClickType currentClickType;
        private static string startNode="";
        private static string endNode="";
        public static ClickType CurrentClickType { set { if (currentClickType != value)
                {
                    currentClickType = value;startNode = "";endNode = "";
                } } }
        public static bool NodeClicked(string clickedNode)
        {
            if (currentClickType == ClickType.NORMAL)
                return false;
            if (startNode == "")
            {
                startNode = clickedNode;
                return true;
            }
            else
            {
                endNode = clickedNode;
                //TODO switch case with different clickTypes-->UMLReader class creationMethods
                switch (currentClickType)
                {
                    case ClickType.AGGREGATION: UMLReader.UmlReader.CreateAggregation(startNode, endNode);break;
                    case ClickType.ASSOCIATION: UMLReader.UmlReader.CreateAssociation(startNode, endNode);break;
                    case ClickType.COMPOSITION:UMLReader.UmlReader.CreateComposition(startNode, endNode);break;
                    case ClickType.DEPENDENCY: UMLReader.UmlReader.CreateDependency(startNode, endNode);break;
                    case ClickType.INHERITANCE: UMLReader.UmlReader.CreateInheritance(startNode, endNode);break;
                    case ClickType.REALIZATION: UMLReader.UmlReader.CreateRealization(startNode, endNode);break;
                }
                startNode = "";endNode = "";
                return false;
            }
        }
    }
}
