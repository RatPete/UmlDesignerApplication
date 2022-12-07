using System;
using System.Collections.Generic;
using System.Text;

namespace WpfDiagramDesigner.Source.PRL.Helper
{
    public class ObjectNotParsableException : Exception
    {
        public ObjectNotParsableException(string message) : base(message)
        {
        }
    }
}
