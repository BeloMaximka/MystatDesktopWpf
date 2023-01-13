using MaterialDesignThemes.Wpf;
using MystatAPI.Entity;
using MystatDesktopWpf.UserControls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MystatDesktopWpf.Domain
{
    internal static class ScheduleControlCreator
    {
        private static StackPanel CreateScheduleLine(PackIconKind kind, string text)
        {
            StackPanel stackPanel = new() { Orientation = Orientation.Horizontal };

            Thickness margin = new(8, 0, 8, 0);
            PackIcon icon = new()
            {
                Kind = kind,
                Margin = margin
            };
            TextBlock textBlock = new() { Text = text };

            stackPanel.Children.Add(icon);
            stackPanel.Children.Add(textBlock);

            return stackPanel;
        }

        static public FrameworkElement CreateScheduleCardSelectable(List<DaySchedule> schedules, DialogHost dialog)
        {
            ScheduleCardSelectable card = new() { MinWidth = 220 };

            card.textBox.Text = DateTime.Parse(schedules[0].Date).ToString("d") + "\n\n";
            card.DialogHost = dialog;

            for (int i = 0; i < schedules.Count; i++)
            {
                double size = 18;
                card.iconPanel.Children.Add(new PackIcon { Kind = PackIconKind.Book, Width = size, Height = size, Margin = i == 0 ? new(0, 50, 0, 0) : new(0, 10, 0, 0) });
                card.textBox.Text += schedules[i].SubjectName + '\n';
                card.iconPanel.Children.Add(new PackIcon { Kind = PackIconKind.Account, Width = size, Height = size });
                card.textBox.Text += schedules[i].TeacherFullName + '\n';
                card.iconPanel.Children.Add(new PackIcon { Kind = PackIconKind.Door, Width = size, Height = size });
                string classRoom = (string)App.Current.FindResource("m_Classroom");
                card.textBox.Text += $"{classRoom} {schedules[i].RoomName}" + '\n';
                card.textBox.Text += $"{schedules[i].StartedAt} - {schedules[i].FinishedAt}";

                PackIcon icon = new() { Kind = PackIconKind.Clock, Width = size, Height = size };
                if (i != schedules.Count - 1)
                {
                    icon.Margin = new Thickness(0, 0, 0, 14);
                    card.textBox.Text += "\n\n";
                }
                card.iconPanel.Children.Add(icon);
            }

            return card;
        }
        static public Card CreateScheduleCard(List<DaySchedule> schedules)
        {
            Card card = new();
            if(schedules.Count == 0)
            {
                card.Content = new TextBlock { Text = (string)App.Current.FindResource("m_NoLessons") };
                card.Padding = new Thickness(8);
                return card; 
            }
            card.Padding = new Thickness(0, 8, 8, 8);
            StackPanel mainStackPanel = new();
            card.Content = mainStackPanel;

            foreach (var item in schedules)
            {
                var children = mainStackPanel.Children;
                children.Add(CreateScheduleLine(PackIconKind.Book, item.SubjectName));
                children.Add(CreateScheduleLine(PackIconKind.Account, item.TeacherFullName));
                string classRoom = (string)App.Current.FindResource("m_Classroom");
                children.Add(CreateScheduleLine(PackIconKind.Door, $"{classRoom} {item.RoomName}"));
                children.Add(CreateScheduleLine(PackIconKind.Clock, $"{item.StartedAt} - {item.FinishedAt}"));
                children.Add(new TextBlock());
            }

            mainStackPanel.Children.RemoveAt(mainStackPanel.Children.Count - 1);
            return card;
        }
    }
}
