using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Transitions;
using MystatAPI.Entity;
using MystatDesktopWpf.Domain;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MystatDesktopWpf.UserControls
{
    /// <summary>
    /// Interaction logic for PlaceholderUserControl.xaml
    /// </summary>
    public partial class Schedule : UserControl
    {
        Dictionary<int, List<DaySchedule>> groupedSchedules = new();
        Button lastButtonHover;
        DateTime selectedDate = DateTime.Now;
        public Schedule()
        {
            InitializeComponent();
            GenerateButtons();
            dateTextBlock.Text = $"{selectedDate.ToString("MMMM", CultureInfo.CurrentCulture)} {selectedDate.Year}";
            Task.Run(() => LoadSchedule(selectedDate));
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;
            List<DaySchedule> schedules = groupedSchedules[(int)button.Content];
            popup.Child = ScheduleControlCreator.CreateScheduleCard(schedules);
            lastButtonHover = button;
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
            if (popup.IsMouseOver) return;
            popup.IsOpen = false;
        }

        private void popup_MouseMove(object sender, MouseEventArgs e)
        {
            Point relative = lastButtonHover.TransformToAncestor(this).Transform(new Point(0, 0));
            Point mousePos = e.GetPosition(this);
            if (mousePos.X > relative.X && mousePos.Y > relative.Y &&
                mousePos.X < relative.X + lastButtonHover.ActualWidth &&
                mousePos.Y < relative.Y + lastButtonHover.ActualHeight)
                return;
            popup.IsOpen = false;
        }
        
        void GenerateButtons()
        {
            int column = 0;
            int row = 0;

            Border border = new();
            border.Margin = new Thickness(8);
            Grid.SetRow(border, row);
            Grid.SetColumn(border, column);

            Binding binding = new("Height");
            binding.Source = border;
            binding.Mode = BindingMode.OneWay;

            for (int i = 1; i <= 42; i++)
            {
                if (column == 7)
                {
                    column = 0;
                    row++;
                }

                Button button = new();
                button.Margin = border.Margin;
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
        async void LoadSchedule(DateTime date)
        {
            while (true)
            {
                try
                {
                    DaySchedule[] schedules = await MystatAPISingleton.mystatAPIClient.GetMonthSchedule(date);
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

                    this.Dispatcher.Invoke((Action)delegate
                    {
                        dateTextBlock.Text = date.ToString("MMMM", CultureInfo.CurrentCulture) + " " + date.Year;
                        Style outlined = (Style)this.FindResource("MaterialDesignOutlinedButton");
                        Style normal = (Style)this.FindResource("CalendarButton");

                        DateTime prevDate = date.AddMonths(-1);
                        int prevMonthDays = DateTime.DaysInMonth(prevDate.Year, prevDate.Month);
                        for (int i = 0; i < dayOfWeek; i++) // prev month buttons
                        {
                            Button button = (Button)gridCalendar.Children[i];
                            button.IsEnabled = false;
                            button.Style = outlined;
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
                            button.Content = i - offset + 1;
                        }

                        foreach (var item in groupedSchedules) // enable buttons with schedule
                        {
                            Button button = (Button)gridCalendar.Children[item.Key + dayOfWeek - 1];
                            button.IsEnabled = true;
                        }

                        Transitioner.MoveNextCommand.Execute(null, transitioner);
                    });

                    break;
                }
                catch (Exception e)
                {
                    //this.Dispatcher.Invoke((Action)delegate { snackbar.MessageQueue?.Enqueue(e.Message); });
                    Task.Delay(500).Wait();
                    continue;
                }
            }
        }

        private void Button_NextMonth_Click(object sender, RoutedEventArgs e)
        {
            selectedDate = selectedDate.AddMonths(1);
            Task.Run(() => LoadSchedule(selectedDate));
        }

        private void Button_PrevMonth_Click(object sender, RoutedEventArgs e)
        {
            selectedDate = selectedDate.AddMonths(-1);
            Task.Run(() => LoadSchedule(selectedDate));
        }
    }
}
