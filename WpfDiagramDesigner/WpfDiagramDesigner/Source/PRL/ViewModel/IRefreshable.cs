using MetaDslx.Languages.Uml.Model;

namespace WpfDiagramDesigner.ViewModel
{
    public interface IRefreshable
    {
        public void Refresh();
        public void RemoveNode(ElementBuilder el);
    }
}