using System.ComponentModel;
using System.Runtime.CompilerServices;
using DeltekReminder.DesktopApp.Annotations;

namespace DeltekReminder.DesktopApp
{
    public class CheckTime : INotifyPropertyChanged
    {
        private string _textValue;
        public string TextValue
        {
            get { return _textValue; }
            set
            {
                _textValue = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}