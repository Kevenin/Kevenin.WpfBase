using Kevenin.LiveClock.Enums;
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
		private TimeSpan CountDownInitialTime;
		private CountDownCommands countdownState;
		private TimeSpan currentTime;
		private string displayFormat = "hh:mm";
		private FontFamily font;
		private SolidColorBrush foregroundColor;
		private bool isCountDownMode = false;
		private WindowState state = WindowState.Normal;
		private Timer timer = new Timer(10);

		public LiveClockViewModel()
		{
			timer.Elapsed += Timer_Elapsed;
			timer.Start();
			Mediator.Register("FullScreen", ShowFullScreen);
			Mediator.Register("BackgroundColor", ChangeBackgroundColor);
			Mediator.Register("ForegroundColor", ChangeForegroundColor);
			Mediator.Register("Font", ChangeFont);
			Mediator.Register("Mode", ChangeMode);
			Mediator.Register("CountdownCommand", ChangeState);
			Mediator.Register("CountDownTimeChanged", ChangeCountDownTime);
			Mediator.Register("DisplayFormatChanged", ChangeDisplayFormat);
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

		public string DisplayFormat
		{
			get { return displayFormat; }
			set { displayFormat = value; this.OnPropertyChanged(); }
		}

		public FontFamily Font
		{
			get { return font; }
			set { font = value; this.OnPropertyChanged(); }
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

		private void ChangeCountDownTime(object obj)
		{
			CountDownInitialTime = (TimeSpan)obj;
		}

		private void ChangeDisplayFormat(object obj)
		{
			DisplayFormat = obj.ToString();
		}

		private void ChangeFont(object obj)
		{
			Font = (FontFamily)obj;
		}

		private void ChangeForegroundColor(object value)
		{
			ForegroundColor = new SolidColorBrush((Color)value);
		}

		private void ChangeMode(object obj)
		{
			isCountDownMode = (bool)obj;
			CurrentTime = CountDownInitialTime;
		}

		private void ChangeState(object obj)
		{
			switch ((CountDownCommands)obj)
			{
				case CountDownCommands.Play:
				case CountDownCommands.Pause:
					if (countdownState == CountDownCommands.None)
						CurrentTime = CountDownInitialTime;
					countdownState = (CountDownCommands)obj;
					break;

				case CountDownCommands.Reset:
					var currentState = countdownState;
					CurrentTime = CountDownInitialTime;
					countdownState = currentState;
					break;

				case CountDownCommands.Stop:
					CurrentTime = CountDownInitialTime;
					countdownState = (CountDownCommands)obj;
					break;
			}
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
			if (isCountDownMode)
			{
				if (countdownState == CountDownCommands.Play)
				{
					var timeToRemove = new TimeSpan(0, 0, 0, 0, Convert.ToInt32(timer.Interval));
					CurrentTime = CurrentTime.Subtract(timeToRemove);
				}
			}
			else
			{
				CurrentTime = DateTime.Now.TimeOfDay;
			}
		}
	}
}