using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace WpfDiagramDesigner.Objects
{
    interface DiagramElement
    {
        void Draw(Canvas canvas);
        void EnableTextBoxes();
        void DisableTextBoxes();
    }
}
