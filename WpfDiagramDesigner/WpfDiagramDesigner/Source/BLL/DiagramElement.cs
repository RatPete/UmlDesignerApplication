using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using WpfDiagramDesigner.Source.PRL.ViewModel;

namespace WpfDiagramDesigner.Objects
{
    interface DiagramElement
    {
        string Id { get; }
        void Draw(Canvas canvas);
        void EnableTextBoxes();
        void DisableTextBoxes();
        void AnimateObject(AnimationValues a, Storyboard storyboard);
        void RemoveFromCanvas(Canvas canvas);
    }
}
