using Kevenin.LiveClock.Enums;
using Kevenin.WpfBase;
using Kevenin.WpfBase.ViewModelBases;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private DateTime countdownTime;
        private Color foregroundColor;
        private bool isFullScreen;
        private string isFullScreenText = "Windowed";
        private string liveClockButtonText = "Hidden";
        private bool mode;
        private bool resetTimer;
        private string selectedDisplayFormat;
        private FontFamily selectedFont = new FontFamily("SegoeUI");
        private Screen selectedScreen;
        private bool showLiveClock;
        private bool startPauseTimer;
        private bool stopTimer;

        public MainViewModel()
        {
            AvailableScreens = Screen.AllScreens;

            BackgroundColor = Properties.Settings.Default.Background;
            ForegroundColor = Properties.Settings.Default.Foregound;
            SelectedFont = Properties.Settings.Default.Font;

            SelectedScreen = AvailableScreens.FirstOrDefault(x => x.DeviceName == Properties.Settings.Default.Display);
            if (SelectedScreen == null)
                SelectedScreen = AvailableScreens.First();

            CountdownTime = new DateTime(2000, 1, 1, 0, 5, 0);
            StartPauseTimer = true;
            SelectedDisplayFormat = "hh:mm";
            Mode = false;
        }

        public string[] AvailableDisplayFormats { get; set; } = new string[]
        {
            "hh:mm",
            "hh:mm:ss",
            "hh:mm:ss.fff",
            "mm:ss",
            "mm:ss.fff",
            "ss.fff"
        };

        public IEnumerable<Screen> AvailableScreens
        {
            get { return availableScreens; }
            set { availableScreens = value; this.OnPropertyChanged(); }
        }

        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set { backgroundColor = value; SetBackground(value); this.OnPropertyChanged(); }
        }

        public DateTime CountdownTime
        {
            get { return countdownTime; }
            set { countdownTime = value; Mediator.Notify("CountDownTimeChanged", value.TimeOfDay); this.OnPropertyChanged(); }
        }

        public Color ForegroundColor
        {
            get { return foregroundColor; }
            set { foregroundColor = value; SetForeground(value); this.OnPropertyChanged(); }
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

        public bool Mode
        {
            get { return mode; }
            set
            {
                mode = value;
                Mediator.Notify("CountdownCommand", CountDownCommands.None);
                Mediator.Notify("Mode", value);
                if (!value)
                    StartPauseTimer = true;
                this.OnPropertyChanged();
            }
        }

        public bool ResetTimer
        {
            get { return resetTimer; }
            set { resetTimer = value; Mediator.Notify("CountdownCommand", CountDownCommands.Reset); this.OnPropertyChanged(); }
        }

        public string SelectedDisplayFormat
        {
            get { return selectedDisplayFormat; }
            set { selectedDisplayFormat = value; Mediator.Notify("DisplayFormatChanged", value); this.OnPropertyChanged(); }
        }

        public FontFamily SelectedFont
        {
            get { return selectedFont; }
            set { selectedFont = value; SetFont(value); this.OnPropertyChanged(); }
        }

        public Screen SelectedScreen
        {
            get { return selectedScreen; }
            set
            {
                selectedScreen = value;
                if (value != null && clock.IsLoaded)
                    SetClockScreen(value);
                this.OnPropertyChanged();
            }
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

        public bool StartPauseTimer
        {
            get { return startPauseTimer; }
            set
            {
                startPauseTimer = value;
                if (value)
                    Mediator.Notify("CountdownCommand", CountDownCommands.Pause);
                else
                    Mediator.Notify("CountdownCommand", CountDownCommands.Play);
                this.OnPropertyChanged();
            }
        }

        public bool StopTimer
        {
            get { return stopTimer; }
            set
            {
                stopTimer = value;
                StartPauseTimer = true;
                Mediator.Notify("CountdownCommand", CountDownCommands.Stop);
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
                IsFullScreenText = "FullScreen";
            else
                IsFullScreenText = "Windowed";
            Properties.Settings.Default.FullScreen = isFullScreenValue;
            Properties.Settings.Default.Save();
        }

        private void OnShowLiveClockChanged(bool showLiveClockValue)
        {
            if (showLiveClockValue)
            {
                LiveClockButtonText = "Show";
                clock.Show();
                SetClockScreen(SelectedScreen);
            }
            else
            {
                LiveClockButtonText = "Hidden";
                clock.Hide();
            }
        }

        private void SetBackground(Color value)
        {
            Mediator.Notify("BackgroundColor", value);
            Properties.Settings.Default.Background = value;
            Properties.Settings.Default.Save();
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

            Properties.Settings.Default.Display = selectedScreen.DeviceName;
            Properties.Settings.Default.Save();
        }

        private void SetFont(FontFamily value)
        {
            Mediator.Notify("Font", value);
            Properties.Settings.Default.Font = value;
            Properties.Settings.Default.Save();
        }

        private void SetForeground(Color value)
        {
            Mediator.Notify("ForegroundColor", value);
            Properties.Settings.Default.Foregound = value;
            Properties.Settings.Default.Save();
        }
    }
}