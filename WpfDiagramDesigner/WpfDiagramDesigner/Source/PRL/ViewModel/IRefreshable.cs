using MetaDslx.Languages.Uml.Model;
using System.Collections.Generic;

namespace WpfDiagramDesigner.ViewModel
{
    public interface IRefreshable
    {
        public void Refresh();
        public void RemoveElement(ElementBuilder el);
        void StartDrawingLine(System.Windows.Input.MouseButtonEventArgs e);
        void EndDrawingLine(System.Windows.Input.MouseButtonEventArgs e);
        void RefocusElement(System.Windows.Controls.TextBox tb);
    }
}