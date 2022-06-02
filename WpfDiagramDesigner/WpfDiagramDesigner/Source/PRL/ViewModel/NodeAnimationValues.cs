using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace WpfDiagramDesigner.Source.PRL.ViewModel
{
    public class NodeAnimationValues:AnimationValues
    {
        public Point TargetPosition { get; set; }
        public Size TargetSize { get; set; }
    }
}
