using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace WpfDiagramDesigner.Source.PRL.ViewModel
{
    public class NewTypeSelectorPopUpViewModel : INotifyPropertyChanged
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
        private List<string> types = new List<string>(new string[] { "Osztály", "Interfész", "Enumeráció","Primitív típus" });
        public List<string> Types
        {
            get
            {
                return types;
            }
            set
            {
                if (types != value)
                {
                    types = value;
                    RaisePropertyChanged();
                }
            }
        }
        private string selectedItem;
        public string SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                if (selectedItem != value)
                {
                    selectedItem = value;
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
