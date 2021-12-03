using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace WpfDiagramDesigner.Objects
{
    interface DiagramElement
    {
        void InitCanvasPosition(Canvas canvas);
        void AnimateElementOnCanvas(System.Windows.Point endPoint);
    }
}
