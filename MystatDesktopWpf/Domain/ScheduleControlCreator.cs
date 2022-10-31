﻿using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using MystatAPI.Entity;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Input;
using MystatDesktopWpf.UserControls;

namespace MystatDesktopWpf.Domain
{
    static class ScheduleControlCreator
    {
        static StackPanel CreateScheduleLine(PackIconKind kind, string text)
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

        static public FrameworkElement CreateScheduleCardSelectable(List<DaySchedule> schedules, DialogHost dialog)
        {
            ScheduleCardSelectable card = new();
            card.textBox.Text = schedules[0].Date + "\n\n";
            card.DialogHost = dialog;

            for (int i = 0; i < schedules.Count; i++)
            {
                card.iconPanel.Children.Add(new PackIcon { Kind = PackIconKind.Book });
                card.textBox.Text += schedules[i].SubjectName + '\n';
                card.iconPanel.Children.Add(new PackIcon { Kind = PackIconKind.Account });
                card.textBox.Text += schedules[i].TeacherFullName + '\n';
                card.iconPanel.Children.Add(new PackIcon { Kind = PackIconKind.Door });
                card.textBox.Text += $"Аудитория {schedules[i].RoomName}" + '\n';
                card.textBox.Text += $"{schedules[i].StartedAt} - {schedules[i].FinishedAt}";

                PackIcon icon = new PackIcon { Kind = PackIconKind.Clock };
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
            card.Padding = new Thickness(0, 8, 8, 8);
            StackPanel mainStackPanel = new();
            card.Content = mainStackPanel;

            foreach (var item in schedules)
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
    }
}
