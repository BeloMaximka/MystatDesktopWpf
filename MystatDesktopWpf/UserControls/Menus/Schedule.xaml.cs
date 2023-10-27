using MaterialDesignThemes.Wpf.Transitions;
using MystatAPI.Entity;
using MystatDesktopWpf.Domain;
using MystatDesktopWpf.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using MaterialDesignThemes.Wpf;

namespace MystatDesktopWpf.UserControls
{
    /// <summary>
    /// Interaction logic for PlaceholderUserControl.xaml
    /// </summary>
    public partial class Schedule : UserControl, IRefreshable
    {
        private readonly Dictionary<int, List<DaySchedule>> groupedSchedules = new();
        private DateTime selectedDate = DateTime.Now;
        
        Popup popup;
        readonly MultiBinding alignmentBinding;

        public Schedule()
        {
            InitializeComponent();
            GenerateButtons();
            dateTextBlock.Text = GetMonthName(selectedDate);
            App.LanguageChanged += HandleLangChange;
            App.GroupChanged += (_, _) => Dispatcher.Invoke(Refresh);
            LoadSchedule(selectedDate);
            ScheduleAutoUpdate();

            alignmentBinding = (MultiBinding)FindResource("PopupBinding");
        }

        private void ScheduleAutoUpdate()
        {
            TaskService.CancelTask("daily-schedule-refresh");
            TaskService.ScheduleTask("daily-schedule-refresh", DateTime.Now.AddDays(1), new TimeOnly(0, 5), () =>
            {
                selectedDate = DateTime.Now;
                Refresh();
                ScheduleAutoUpdate();
            });
        }

        private void HandleLangChange(object? sender, EventArgs e)
        {
            dateTextBlock.Text = GetMonthName(selectedDate);
        }

        private static string GetMonthName(DateTime date)
        {
            StringBuilder builder = new();
            builder.Append($"{date.ToString("MMMM", CultureInfo.CurrentUICulture)} {date.Year}");
            builder[0] = builder[0].ToString().ToUpper()[0];
            return builder.ToString();
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;
            List<DaySchedule> schedules = groupedSchedules[(int)button.Content];
            popup = new()
            {
                Placement = PlacementMode.Bottom,
                Child = ScheduleControlCreator.CreateScheduleCard(schedules),
                PlacementTarget = button,
                AllowsTransparency = true,
                PopupAnimation = PopupAnimation.Fade
            };
            popup.Closed += Popup_Closed;
            grid.Children.Add(popup);
            popup.SetBinding(Popup.HorizontalOffsetProperty, alignmentBinding);
            
            popup.IsOpen = true;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            var schedules = groupedSchedules[(int)button.Content];
            scheduleDialog.DialogContent = ScheduleControlCreator.CreateScheduleCardSelectable(schedules, scheduleDialog);
            scheduleDialog.IsOpen = true;
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            popup.IsOpen = false;
        }

        private void GenerateButtons()
        {
            int column = 0;
            int row = 0;

            Border border = new() { Margin = new Thickness(8) };
            Grid.SetRow(border, row);
            Grid.SetColumn(border, column);

            Binding binding = new("Height")
            {
                Source = border,
                Mode = BindingMode.OneWay
            };

            for (int i = 1; i <= 42; i++)
            {
                if (column == 7)
                {
                    column = 0;
                    row++;
                }

                Button button = new() { Margin = border.Margin };
                button.MouseEnter += Button_MouseEnter;
                button.MouseLeave += Button_MouseLeave;
                button.Click += Button_Click;
                gridCalendar.Children.Add(button);

                Grid.SetRow(button, row);
                Grid.SetColumn(button, column);

                button.SetBinding(Button.HeightProperty, binding);
                column++;
            }
        }

        private bool Loading = false;

        private async void LoadSchedule(DateTime date)
        {
            Loading = true;
            RetryDelayProvider delay = new();
            while (true)
            {
                try
                {
                    DaySchedule[] schedules = await MystatAPISingleton.Client.GetMonthSchedule(date);
                    groupedSchedules.Clear();
                    foreach (DaySchedule schedule in schedules)
                    {
                        int day = DateTime.Parse(schedule.Date).Day;
                        if (!groupedSchedules.ContainsKey(day))
                            groupedSchedules[day] = new();
                        groupedSchedules[day].Add(schedule);
                    }

                    date = new DateTime(date.Year, date.Month, 1);
                    int dayOfWeek = ((int)date.DayOfWeek) - 1;
                    if (dayOfWeek == -1) dayOfWeek = 6; // sunday fix

                    dateTextBlock.Text = GetMonthName(date);
                    Style outlined = (Style)this.FindResource("MaterialDesignOutlinedButton");
                    Style normal = (Style)this.FindResource("CalendarButton");
                    int fontSize = 18;

                    DateTime prevDate = date.AddMonths(-1);
                    int prevMonthDays = DateTime.DaysInMonth(prevDate.Year, prevDate.Month);
                    for (int i = 0; i < dayOfWeek; i++) // prev month buttons
                    {
                        Button button = (Button)gridCalendar.Children[i];
                        button.IsEnabled = false;
                        button.Style = outlined;
                        button.FontSize = fontSize;
                        button.Content = prevMonthDays - dayOfWeek + i + 1;
                    }

                    int monthDays = DateTime.DaysInMonth(date.Year, date.Month);
                    for (int i = dayOfWeek; i < monthDays + dayOfWeek; i++) // current month buttons
                    {
                        Button button = (Button)gridCalendar.Children[i];
                        button.IsEnabled = false;
                        button.Style = normal;
                        button.Content = i + 1 - dayOfWeek;
                    }

                    DateTime currentDate = DateTime.Now;
                    if (date.Month == currentDate.Month && date.Year == currentDate.Year)
                    {
                        // current day button;
                        Button todayButton = (Button)gridCalendar.Children[dayOfWeek + currentDate.Day - 1];
                        todayButton.Style = (Style)this.FindResource("DarkCalendarButton");
                    }

                    DateTime nextDate = date.AddMonths(-1);
                    int nextMonthDays = DateTime.DaysInMonth(nextDate.Year, nextDate.Month);
                    int offset = monthDays + dayOfWeek;
                    for (int i = offset; i < 42; i++) // next month buttons
                    {
                        Button button = ((Button)gridCalendar.Children[i]);
                        button.IsEnabled = false;
                        button.Style = outlined;
                        button.FontSize = fontSize;
                        button.Content = i - offset + 1;
                    }

                    foreach (var item in groupedSchedules) // enable buttons with schedule
                    {
                        Button button = (Button)gridCalendar.Children[item.Key + dayOfWeek - 1];
                        button.IsEnabled = true;
                    }

                    Transitioner.MoveNextCommand.Execute(null, transitioner);
                    Loading = false;
                    break;
                }
                catch (Exception)
                {
                    //this.Dispatcher.Invoke((Action)delegate { snackbar.MessageQueue?.Enqueue(e.Message); });
                    await Task.Delay(delay.ProvideValueMilliseconds());
                    continue;
                }
            }
        }

        private void Button_NextMonth_Click(object sender, RoutedEventArgs e)
        {
            if (Loading) return;
            selectedDate = selectedDate.AddMonths(1);
            LoadSchedule(selectedDate);
        }

        private void Button_PrevMonth_Click(object sender, RoutedEventArgs e)
        {
            if (Loading) return;
            selectedDate = selectedDate.AddMonths(-1);
            LoadSchedule(selectedDate);
        }

        public void Refresh()
        {
            if (Loading) return;
            LoadSchedule(selectedDate);
        }

        // We need to wait for the popup fade out to finish animation before we remove it to avoid visual bugs
        private void Popup_Closed(object sender, EventArgs e)
        {
            grid.Children.Remove((Popup)sender);
        }
    }
}
