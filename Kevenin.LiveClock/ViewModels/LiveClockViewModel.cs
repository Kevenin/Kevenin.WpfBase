using Kevenin.WpfBase;
using Kevenin.WpfBase.ViewModelBases;
using System;
using System.Timers;
using System.Windows;
using System.Windows.Media;

namespace Kevenin.LiveClock.ViewModels
{
    public class LiveClockViewModel : ViewModelBase
    {
        private SolidColorBrush backgroundColor;
        private TimeSpan currentTime;
        private SolidColorBrush foregroundColor;
        private WindowState state = WindowState.Normal;
        private Timer timer = new Timer(10);

        public LiveClockViewModel()
        {
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
            Mediator.Register("FullScreen", ShowFullScreen);
            Mediator.Register("BackgroundColor", ChangeBackgroundColor);
            Mediator.Register("ForegroundColor", ChangeForegroundColor);
        }

        public SolidColorBrush BackgroundColor
        {
            get { return backgroundColor; }
            set { backgroundColor = value; this.OnPropertyChanged(); }
        }

        public TimeSpan CurrentTime
        {
            get { return currentTime; }
            set { currentTime = value; this.OnPropertyChanged(); }
        }

        public SolidColorBrush ForegroundColor
        {
            get { return foregroundColor; }
            set { foregroundColor = value; this.OnPropertyChanged(); }
        }

        public WindowState State
        {
            get { return state; }
            set { state = value; this.OnPropertyChanged(); }
        }

        private void ChangeBackgroundColor(object value)
        {
            BackgroundColor = new SolidColorBrush((Color)value);
        }

        private void ChangeForegroundColor(object value)
        {
            ForegroundColor = new SolidColorBrush((Color)value);
        }

        private void ShowFullScreen(object value)
        {
            bool isFullSCreen = (bool)value;
            if (isFullSCreen)
            {
                State = WindowState.Maximized;
            }
            else
            {
                State = WindowState.Normal;
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            CurrentTime = DateTime.Now.TimeOfDay;
        }
    }
}