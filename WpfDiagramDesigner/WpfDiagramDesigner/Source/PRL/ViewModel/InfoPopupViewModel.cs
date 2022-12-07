using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace WpfDiagramDesigner.Source.PRL.ViewModel
{
    public class InfoPopupViewModel : INotifyPropertyChanged
    {
        private string errorDescription;
        public string ErrorDescription
        {
            get { return errorDescription; }
            set
            {
                if (errorDescription != value)
                {
                    errorDescription = value;
                    RaisePropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
