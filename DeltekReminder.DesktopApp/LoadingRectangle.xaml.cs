using System;
using System.Windows;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace DeltekReminder.DesktopApp
{
    public sealed partial class LoadingRectangle
    {
        private double _offsetSecondsValue;

        public LoadingRectangle()
        {
            InitializeComponent();
            SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Math.Abs(e.NewSize.Width - e.PreviousSize.Width) > .1)
            {
                KeyFrame2.Value = ActualWidth / 3;
                KeyFrame3.Value = KeyFrame2.Value * 2;
                KeyFrame4.Value = ActualWidth + 10;
                KeyFrame5.Value = ActualWidth + 10;
            }
        }

        public double OffsetSeconds
        {

            get
            {
                return _offsetSecondsValue;
            }
            set
            {
                _offsetSecondsValue = value;
                OffsetKeyFrameKeyTimes();

            }

        }

        private void OffsetKeyFrameKeyTimes()
        {
            TimeSpan offset = TimeSpan.FromSeconds(OffsetSeconds);
            KeyFrame1.KeyTime = KeyFrame1.KeyTime.TimeSpan.Add(offset);
            KeyFrame2.KeyTime = KeyFrame2.KeyTime.TimeSpan.Add(offset);
            KeyFrame3.KeyTime = KeyFrame3.KeyTime.TimeSpan.Add(offset);
            KeyFrame4.KeyTime = KeyFrame4.KeyTime.TimeSpan.Add(offset);
        }
    }
}
