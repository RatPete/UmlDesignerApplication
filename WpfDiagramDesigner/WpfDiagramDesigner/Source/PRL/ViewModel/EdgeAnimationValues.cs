using MetaDslx.GraphViz;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Text;

namespace WpfDiagramDesigner.Source.PRL.ViewModel
{
    class EdgeAnimationValues:AnimationValues
    {
        public ImmutableArray<ImmutableArray<Point2D>> TargetPosition { get; set; }
    }
}
