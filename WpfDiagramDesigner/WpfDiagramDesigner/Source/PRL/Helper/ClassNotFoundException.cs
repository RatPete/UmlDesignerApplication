using System;
using System.Collections.Generic;
using System.Text;

namespace WpfDiagramDesigner.Source.PRL.Helper
{
    class ClassNotFoundException:Exception
    {
        public ClassNotFoundException() { }
        public ClassNotFoundException(string message) : base(message) { }
        public ClassNotFoundException(string message, Exception inner) : base(message, inner) { }
    }
}
