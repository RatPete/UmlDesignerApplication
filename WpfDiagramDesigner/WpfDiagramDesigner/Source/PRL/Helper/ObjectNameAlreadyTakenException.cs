using System;
using System.Collections.Generic;
using System.Text;

namespace WpfDiagramDesigner.Source.PRL.Helper
{
    public class ObjectNameAlreadyTakenException: Exception
    { 
        public ObjectNameAlreadyTakenException(string ex): base(ex)
        {

        }
    }
}
