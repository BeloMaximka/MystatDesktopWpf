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

namespace MystatDesktopWpf
{
    /// <summary>
    /// Interaction logic for PlaceholderUserControl.xaml
    /// </summary>
    public partial class ScheduleUserControl : UserControl
    {
        Dictionary<int, List<DaySchedule>> groupedSchedules = new();
        Button lastButtonHover;
        DateTime currentDate = DateTime.Now;
        public ScheduleUserControl()
        {
            InitializeComponent();
            GenerateButtons();
            dateTextBlock.Text = $"{currentDate.ToString("MMMM", CultureInfo.CurrentCulture)} {currentDate.Year}";
            Task.Run(() => LoadSchedule(currentDate));
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;
            popup.Child = CreateScheduleCard((int)button.Content);
            lastButtonHover = button;
            popup.IsOpen = true;
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

        StackPanel CreateScheduleLine(PackIconKind kind, string text)
        {
            StackPanel stackPanel = new();
            stackPanel.Orientation = Orientation.Horizontal;

            Thickness margin = new(8, 0, 8, 0);
            PackIcon icon = new();
            icon.Kind = kind;
            icon.Margin = margin;
            TextBlock textBlock = new();
            textBlock.Text = text;

            stackPanel.Children.Add(icon);
            stackPanel.Children.Add(textBlock);

            return stackPanel;
        }
        Card CreateScheduleCard(int day)
        {
            Card card = new();
            card.Padding = new Thickness(0, 8, 8, 8);
            StackPanel mainStackPanel = new();
            card.Content = mainStackPanel;

            foreach (var item in groupedSchedules[day])
            {
                StackPanel stackPanel = new();
                stackPanel.Orientation = Orientation.Horizontal;

                var children = mainStackPanel.Children;
                children.Add(CreateScheduleLine(PackIconKind.Book, item.SubjectName));
                children.Add(CreateScheduleLine(PackIconKind.Account, item.TeacherFullName));
                children.Add(CreateScheduleLine(PackIconKind.Door, $"Аудитория {item.RoomName}"));
                children.Add(CreateScheduleLine(PackIconKind.Clock, $"{item.StartedAt} - {item.FinishedAt}"));
                children.Add(new TextBlock());
            }
            mainStackPanel.Children.RemoveAt(mainStackPanel.Children.Count - 1);
            return card;
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
                        Style normal = (Style)this.FindResource("MaterialDesignRaisedButton");

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
            currentDate = currentDate.AddMonths(1);
            Task.Run(() => LoadSchedule(currentDate));
        }

        private void Button_PrevMonth_Click(object sender, RoutedEventArgs e)
        {
            currentDate = currentDate.AddMonths(-1);
            Task.Run(() => LoadSchedule(currentDate));
        }
    }
}
