using Kevenin.WpfBase;
using Kevenin.WpfBase.ViewModelBases;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace Kevenin.LiveClock.ViewModels
{
    public class MainViewModel : ViewModelBase, IDisposable
    {
        private IEnumerable<Screen> availableScreens;
        private Color backgroundColor;
        private LiveClock clock = new LiveClock();
        private Color foregroundColor;
        private bool isFullScreen;
        private string isFullScreenText = "FullScreen";
        private string liveClockButtonText = "Show";
        private Screen selectedScreen;
        private bool showLiveClock;

        public MainViewModel()
        {
            AvailableScreens = Screen.AllScreens;
            BackgroundColor = Colors.Black;
            ForegroundColor = Colors.White;
        }

        public IEnumerable<Screen> AvailableScreens
        {
            get { return availableScreens; }
            set { availableScreens = value; this.OnPropertyChanged(); }
        }

        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set { backgroundColor = value; Mediator.Notify("BackgroundColor", value); this.OnPropertyChanged(); }
        }

        public Color ForegroundColor
        {
            get { return foregroundColor; }
            set { foregroundColor = value; Mediator.Notify("ForegroundColor", value); this.OnPropertyChanged(); }
        }

        public bool IsFullScreen
        {
            get { return isFullScreen; }
            set { isFullScreen = value; OnIsFullScreenChanged(value); this.OnPropertyChanged(); }
        }

        public string IsFullScreenText
        {
            get { return isFullScreenText; }
            set { isFullScreenText = value; this.OnPropertyChanged(); }
        }

        public string LiveClockButtonText
        {
            get { return liveClockButtonText; }
            set { liveClockButtonText = value; this.OnPropertyChanged(); }
        }

        public Screen SelectedScreed
        {
            get { return selectedScreen; }
            set { selectedScreen = value; SetClockScreen(value); this.OnPropertyChanged(); }
        }

        public bool ShowLiveClock
        {
            get { return showLiveClock; }
            set
            {
                showLiveClock = value;
                OnShowLiveClockChanged(value);
                this.OnPropertyChanged();
            }
        }

        public void Dispose()
        {
            clock.Close();
            clock = null;
        }

        private void OnIsFullScreenChanged(bool isFullScreenValue)
        {
            Mediator.Notify("FullScreen", isFullScreenValue);
            if (isFullScreenValue)
                IsFullScreenText = "Windowed";
            else
                IsFullScreenText = "FullScreen";
        }

        private void OnShowLiveClockChanged(bool showLiveClockValue)
        {
            if (showLiveClockValue)
            {
                LiveClockButtonText = "Hide";
                clock.Show();
            }
            else
            {
                LiveClockButtonText = "Show";
                clock.Hide();
            }
        }

        private void SetClockScreen(Screen selectedScreen)
        {
            var transform = PresentationSource.FromVisual(clock).CompositionTarget.TransformFromDevice;
            var corner = transform.Transform(new Point(selectedScreen.WorkingArea.Right, selectedScreen.WorkingArea.Bottom));

            var Left = corner.X - clock.ActualWidth;
            var Top = corner.Y - clock.ActualHeight;

            bool tempFullScreen = IsFullScreen;

            if (IsFullScreen)
                IsFullScreen = false;

            clock.Left = Left < 0 ? 0 : Left;
            clock.Top = Top < 0 ? 0 : Top;

            IsFullScreen = tempFullScreen;
        }
    }
}