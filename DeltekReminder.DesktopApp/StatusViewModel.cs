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
        private ObservableCollection<string> _checkTimes;
        private string _selectedCheckTime;
        private Visibility _nextCheckTextBlockVisible;
        private Visibility _nextCheckComboBoxVisible;
        private string _nextCheckDay;
        private bool _nextCheckComboBoxOpen;
        private readonly DeltekReminderUiContext _ctx;
        public event CheckTimeChanged CheckTimeChanged;

        protected virtual void InvokeCheckTimeChanged(string value)
        {
            var handler = CheckTimeChanged;
            if (handler != null) handler(this, new CheckTimeChangedArgs { NewCheckTime = value });
        }

        public StatusViewModel(DeltekReminderUiContext ctx)
        {
            _ctx = ctx;
            CheckTimes = new ObservableCollection<string>
                {
                    "4:00 PM",
                    "4:15 PM",
                    "4:30 PM",
                    "4:45 PM",
                    "5:00 PM",
                    "5:15 PM",
                    "5:30 PM",
                    "5:45 PM",
                    "6:00 PM",
                    "6:15 PM",
                    "6:30 PM",
                };
            SelectedCheckTime = CheckTimes.Single(i => i == ctx.Settings.CheckTime);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> CheckTimes
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

        public string SelectedCheckTime
        {
            get { return _selectedCheckTime; }
            set
            {
                _selectedCheckTime = value;
                SetNextCheckTimeEditable(false);
                if (_ctx.Settings.CheckTime != value)
                {
                    InvokeCheckTimeChanged(value);
                };
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

    public delegate void CheckTimeChanged(object sender, CheckTimeChangedArgs args);

    public class CheckTimeChangedArgs
    {
        public string NewCheckTime { get; set; }
    }
}