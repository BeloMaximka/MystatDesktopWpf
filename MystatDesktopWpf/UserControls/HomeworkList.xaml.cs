using MaterialDesignThemes.Wpf;
using MystatAPI.Entity;
using MystatDesktopWpf.Extensions;
using MystatDesktopWpf.Services;
using MystatDesktopWpf.UserControls.Menus;
using MystatDesktopWpf.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace MystatDesktopWpf.UserControls
{
	/// <summary>
	/// Interaction logic for HomeworkList.xaml
	/// </summary>
	public partial class HomeworkList : UserControl
	{
		private const int defaultPageSize = 6;

		private FrameworkElement lastExtraInfoCirlce;

		// Чтобы можно было прикрутить Binding
		public static readonly DependencyProperty CollectionProperty =
			DependencyProperty.Register("Collection", typeof(HomeworkCollection), typeof(HomeworkList));
		public HomeworkCollection Collection
		{
			get => (HomeworkCollection)GetValue(CollectionProperty);
			set => SetValue(CollectionProperty, value);
		}

		public static readonly DependencyProperty HomeworkManagerProperty =
			DependencyProperty.Register("HomeworkManager", typeof(Homeworks), typeof(HomeworkList));
		public Homeworks? HomeworkManager
		{
			get => (Homeworks)GetValue(HomeworkManagerProperty);
			set => SetValue(HomeworkManagerProperty, value);
		}

		public static readonly DependencyProperty HeaderProperty =
			DependencyProperty.Register("Header", typeof(string), typeof(HomeworkList));

		private int? sectionNumber;
		public int? SectionNumber
		{
			get => sectionNumber;
			set
			{
				if (value.HasValue && value.Value >= 0)
				{
					sectionNumber = value.Value;
					MainExpander.IsExpanded = SettingsService.Settings.HomeworkSectionExpandedStates[value.Value];
				}
			}
		}
		public string Header
		{
			get => (string)GetValue(HeaderProperty);
			set => SetValue(HeaderProperty, value);
		}

		public HomeworkList()
		{
			InitializeComponent();
			PopupInfo.CustomPopupPlacementCallback += PopupExtraInfoPlacement;
		}

		private void Card_Initialized(object sender, EventArgs e)
		{
			Card card = (Card)sender;
			HomeworkStatus status = (HomeworkStatus)card.Tag;
			if (status == HomeworkStatus.Uploaded || status == HomeworkStatus.Checked)
			{
				Button uploadButton = (Button)card.FindName("uploadButton");
				uploadButton.Click -= UploadButton_Click;
				uploadButton.Click += DownloadUploadedButton_Click;
			}
			if (status == HomeworkStatus.Uploaded)
			{
				Button deleteButton = (Button)card.FindName("deleteButton");
				deleteButton.Click += DeleteButton_Click;
			}
			if (status == HomeworkStatus.Checked)
			{
				Button deleteButton = (Button)card.FindName("deleteButton");
				deleteButton.Click += UploadButton_Click;
			}
		}

		private void DeleteButton_Click(object sender, RoutedEventArgs e)
		{
			HomeworkManager?.OpenDeleteDialog((Homework)((Button)sender).Tag);
		}

		private void DownloadButton_Click(object sender, RoutedEventArgs e)
		{
			Homework homework = (Homework)((Button)sender).Tag;
			HomeworkManager?.DownloadHomework(homework.FilePath);
		}

		private void UploadButton_Click(object sender, RoutedEventArgs e)
		{
			Button uploadButton = (Button)sender;
			Grid grid = (Grid)uploadButton.Parent;
			Button progressButton = (Button)grid.FindName("progressButton");
			HomeworkManager?.OpenUploadDialog((Homework)uploadButton.Tag, Collection.Items, progressButton, uploadButton);
		}

		private void DownloadUploadedButton_Click(object sender, RoutedEventArgs e)
		{
			Button uploadButton = (Button)sender;
			Homework homework = (Homework)uploadButton.Tag;
			if (homework.UploadedHomework.StudentAnswer != null)
				HomeworkManager?.OpenDownloadUploadedDialog(homework);
			else
				HomeworkManager?.DownloadHomework(homework.UploadedHomework.FilePath);
		}

		private void Card_Drop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				Card card = (Card)sender;
				Button uploadButton = (Button)card.FindName("uploadButton");
				Button progressButton = (Button)card.FindName("progressButton");
				var files = (string[])e.Data.GetData(DataFormats.FileDrop);
				HomeworkManager?.OpenUploadDialog((Homework)uploadButton.Tag, Collection.Items, progressButton, uploadButton, files);
			}
		}

		// Sets the content of comment popup and shows it on screen
		private void CommentButton_Click(object sender, RoutedEventArgs e)
		{
			Button button = (Button)sender;
			popupComment.PlacementTarget = button;

			if (button.Tag is string homeworkTaskComment)
			{
				CommentParagraph.Inlines.SetInlinesWithHyperlinksFromText(homeworkTaskComment);
			}
			else if (button.Tag is HomeworkComment teacherComment)
			{
				CommentParagraph.Inlines.Clear();
				if (teacherComment.AttachmentPath == null) // No file attached, just show the text
				{
					CommentParagraph.Inlines.Add(new Run(teacherComment.Text));
				}
				else // Add the ability to download attached file
				{
					var hyperlink = new Hyperlink(new Run(teacherComment.Text))
					{
						// Adress doesn't matter. RequestNavigate won't fire without it
						NavigateUri = new Uri(@"https://127.0.0.1/"),
					};
					hyperlink.RequestNavigate += (sender, args) => HomeworkManager.DownloadHomework(teacherComment.AttachmentPath);
					CommentParagraph.Inlines.Add(hyperlink);
				}
			}
			CommentViewer.AdjustWidthToText();
			popupComment.IsOpen = true;
		}

		private async void LoadPageButton_Click(object sender, RoutedEventArgs e)
		{
			Button button = (Button)sender;

			button.IsHitTestVisible = false;
			ButtonProgressAssist.SetIsIndicatorVisible(progressPageButton, true);
			await Collection.LoadNextPage();
			ButtonProgressAssist.SetIsIndicatorVisible(progressPageButton, false);
			button.IsHitTestVisible = true;

			UpdateNextPageButtonVisibility();
		}

		public void UpdateNextPageButtonVisibility()
		{
			var count = Collection.Items.Count;
			var maxCount = Collection.MaxCount;
			if (count >= maxCount)
			{
				nextPageButton.Visibility = Visibility.Collapsed;
			}
			else if (count >= defaultPageSize)
			{
				nextPageButton.Visibility = Visibility.Visible;
			}
		}

		#region Extra info popup mouse handling
		// MouseOver multibinding caused many problems so I ended up with manual handling

		private CustomPopupPlacement[] PopupExtraInfoPlacement(Size popupSize, Size targetSize, Point offset)
		{
			CustomPopupPlacement right = new(new Point(targetSize.Width / 2 + 3, -4), PopupPrimaryAxis.Horizontal);
			CustomPopupPlacement left = new(new Point((popupSize.Width + targetSize.Width / 2) * -1 + 8, -4), PopupPrimaryAxis.Horizontal);
			return new CustomPopupPlacement[] { right, left };
		}
		private void Info_MouseEnter(object sender, MouseEventArgs e)
		{
			if (sender is FrameworkElement element && element.Tag is Homework homework)
			{
				lastExtraInfoCirlce = element;
				ExtraInfoTextBox.DataContext = homework;
				PopupInfo.PlacementTarget = element;
				PopupInfo.IsOpen = true;
				this.MouseMove += Control_MouseMove;
			}
		}
		private void Control_MouseMove(object sender, MouseEventArgs e)
		{
			Point pt = e.GetPosition(lastExtraInfoCirlce);
			if (!PopupInfoBorder.IsMouseOver && VisualTreeHelper.HitTest(lastExtraInfoCirlce, pt) == null)
			{
				PopupInfo.IsOpen = false;
				this.MouseMove -= Control_MouseMove;
			}
		}
		#endregion

		#region Expander
		private void SetMainExpanderState(bool isExpanded)
		{
			MainExpander.IsExpanded = isExpanded;
			if (sectionNumber.HasValue)
			{
				SettingsService.Settings.HomeworkSectionExpandedStates[sectionNumber.Value] = isExpanded;
				SettingsService.OnSettingsChange();
			}

		}
		private void MainExpander_Collapsed(object sender, RoutedEventArgs e)
		{
			SetMainExpanderState(false);
		}

		private void MainExpander_Expanded(object sender, RoutedEventArgs e)
		{
			SetMainExpanderState(true);
		}
		#endregion
	}
}
