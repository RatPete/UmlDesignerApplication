using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using WpfDiagramDesigner.Objects;

namespace WpfDiagramDesigner.Source.PRL.Helper
{
    public static class RelationshipCreator
    {
        private static ClickType currentClickType;
        private static string startNode = "";
        private static string endNode = "";
        public static ClickType CurrentClickType
        {
            set
            {
                if (currentClickType != value)
                {
                    currentClickType = value; startNode = ""; endNode = "";
                }
            }
        }
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
                try
                {
                    switch (currentClickType)
                    {
                        case ClickType.AGGREGATION: UMLReader.UmlReader.CreateAggregation(startNode, endNode); break;
                        case ClickType.ASSOCIATION: UMLReader.UmlReader.CreateAssociation(startNode, endNode); break;
                        case ClickType.COMPOSITION: UMLReader.UmlReader.CreateComposition(startNode, endNode); break;
                        case ClickType.DEPENDENCY: UMLReader.UmlReader.CreateDependency(startNode, endNode); break;
                        case ClickType.INHERITANCE: UMLReader.UmlReader.CreateInheritance(startNode, endNode); break;
                        case ClickType.REALIZATION: UMLReader.UmlReader.CreateRealization(startNode, endNode); break;
                        case ClickType.ONEWAYASSOCIATION: UMLReader.UmlReader.CreateOneWayAssociation(startNode, endNode); break;
                    }
                    startNode = ""; endNode = "";
                }
                catch (ClassNotFoundException)
                {

                    startNode = "";
                }
                return false;
            }
        }
        public static void GenerateArrow(Point start, Point end, Path body, Path head)
        {
            Path temp = new Path(); ;
            switch (currentClickType)
            {
                case ClickType.AGGREGATION: LineBuilder.NonDashedLine(body); temp = HeadBuilder.CreateEmptyDiamondHead(start, end); head.Data = temp.Data; head.Stroke = temp.Stroke; head.Fill = temp.Fill; break;
                case ClickType.ASSOCIATION: LineBuilder.NonDashedLine(body); temp = HeadBuilder.CreateNoHead(start, end); head.Data = temp.Data; head.Stroke = temp.Stroke; break;
                case ClickType.COMPOSITION: LineBuilder.NonDashedLine(body); temp = HeadBuilder.CreateFullDiamondHead(start, end); head.Data = temp.Data; head.Stroke = temp.Stroke; head.Fill = temp.Fill; break;
                case ClickType.DEPENDENCY: LineBuilder.DashedLine(body); temp = HeadBuilder.CreateArrowHead(start, end); head.Data = temp.Data; head.Stroke = temp.Stroke; break;
                case ClickType.INHERITANCE: LineBuilder.NonDashedLine(body); temp = HeadBuilder.CreateTriangleHead(start, end); head.Data = temp.Data; head.Stroke = temp.Stroke; break;
                case ClickType.REALIZATION: LineBuilder.DashedLine(body); temp = HeadBuilder.CreateTriangleHead(start, end); head.Data = temp.Data; head.Stroke = temp.Stroke; break;
                case ClickType.ONEWAYASSOCIATION:LineBuilder.NonDashedLine(body); temp = HeadBuilder.CreateArrowHead(start, end); head.Data = temp.Data; head.Stroke = temp.Stroke; break;

            }

        }
    }
}
