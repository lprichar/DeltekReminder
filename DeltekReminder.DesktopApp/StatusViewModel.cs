using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using DeltekReminder.DesktopApp.Annotations;

namespace DeltekReminder.DesktopApp
{
    public class StatusViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<CheckTime> _checkTimes;
        private CheckTime _selectedCheckTime;
        private Visibility _nextCheckTextBlockVisible;
        private Visibility _nextCheckComboBoxVisible;
        private string _nextCheckDay;
        private bool _nextCheckComboBoxOpen;

        public StatusViewModel()
        {
            CheckTimes = new ObservableCollection<CheckTime>
                {
                    new CheckTime { TextValue = "4:00 PM" },
                    new CheckTime { TextValue = "4:15 PM" },
                    new CheckTime { TextValue = "4:30 PM" },
                    new CheckTime { TextValue = "4:45 PM" },
                    new CheckTime { TextValue = "5:00 PM" },
                    new CheckTime { TextValue = "5:15 PM" },
                    new CheckTime { TextValue = "5:30 PM" },
                    new CheckTime { TextValue = "5:45 PM" },
                    new CheckTime { TextValue = "6:00 PM" },
                    new CheckTime { TextValue = "6:15 PM" },
                    new CheckTime { TextValue = "6:30 PM" },
                };
            SelectedCheckTime = CheckTimes.Single(i => i.TextValue == "5:00 PM");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<CheckTime> CheckTimes
        {
            get { return _checkTimes; }
            set
            {
                _checkTimes = value;
                OnPropertyChanged();
            }
        }

        public bool NextCheckComboBoxOpen
        {
            get { return _nextCheckComboBoxOpen; }
            set
            {
                _nextCheckComboBoxOpen = value;
                OnPropertyChanged();
            }
        }

        public CheckTime SelectedCheckTime
        {
            get { return _selectedCheckTime; }
            set
            {
                _selectedCheckTime = value;
                SetNextCheckTimeEditable(false);
                OnPropertyChanged();
            }
        }

        public Visibility NextCheckTextBlockVisible
        {
            get { return _nextCheckTextBlockVisible; }
            set
            {
                _nextCheckTextBlockVisible = value;
                OnPropertyChanged();
            }
        }

        public Visibility NextCheckComboBoxVisible
        {
            get { return _nextCheckComboBoxVisible; }
            set
            {
                _nextCheckComboBoxVisible = value;
                OnPropertyChanged();
            }
        }

        public string NextCheckDay
        {
            get { return _nextCheckDay; }
            set
            {
                _nextCheckDay = value;
                OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetNextCheckTimeEditable(bool isEditable)
        {
            NextCheckComboBoxVisible = isEditable ? Visibility.Visible : Visibility.Collapsed;
            NextCheckTextBlockVisible = isEditable ? Visibility.Collapsed : Visibility.Visible;
            NextCheckComboBoxOpen = isEditable;
        }
    }
}