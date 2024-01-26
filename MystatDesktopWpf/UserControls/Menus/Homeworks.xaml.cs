using MaterialDesignThemes.Wpf;
using MystatAPI.Entity;
using MystatAPI.Exceptions;
using MystatDesktopWpf.Domain;
using MystatDesktopWpf.Extensions;
using MystatDesktopWpf.Services;
using MystatDesktopWpf.UserControls.DialogContent;
using MystatDesktopWpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MystatDesktopWpf.UserControls.Menus
{
	/// <summary>
	/// Interaction logic for Homeworks.xaml
	/// </summary>
	public partial class Homeworks : UserControl, IRefreshable
	{
		private HomeworksViewModel viewModel;
		private static readonly HttpClient httpClient = new();
		private readonly UploadHomework uploadContent = new();
		private readonly DeleteHomework deleteContent = new();
		private readonly DonwloadHomeworkPreview downloadContent = new();
		private readonly HomeworkList[] homeworkSections;
		public static bool IsInstantiated { get; private set; } = false;

		public Homeworks()
		{
			InitializeComponent();
			IsInstantiated = true;

            homeworkSections = new HomeworkList[] { OverdueList, DeletedList, ActiveList, UploadedList, CheckedList };

			for (int i = 0; i < homeworkSections.Length; i++)
			{
				homeworkSections[i].SectionNumber = i;
			}
			App.LanguageChanged += App_LanguageChanged;
			App.GroupChanged += (_, _) => Dispatcher.Invoke(Refresh);
		}

		// Updating "All subjects" item in ComboBox
		private void App_LanguageChanged(object? sender, EventArgs e)
		{
			SpecsComboBox.SelectionChanged -= SpecsComboBox_SelectionChanged;
			var temp = SpecsComboBox.SelectedIndex;
			SpecsComboBox.SelectedIndex = -1;
			SpecsComboBox.Items.Refresh();
			SpecsComboBox.SelectedIndex = temp;
			SpecsComboBox.SelectionChanged += SpecsComboBox_SelectionChanged;
		}

		private void ScheduleAutoUpdate()
		{
			TaskService.CancelTask("auto-homework-refresh");
			TaskService.ScheduleTask("auto-homework-refresh", TimeSpan.FromHours(1), () =>
			{
				Refresh(true);
				ScheduleAutoUpdate();
			});
		}

		#region Homework interactions
		private void OpenFileInExplorer(string filePath)
		{
			try
			{
				string argument = "/select, \"" + filePath + "\"";
				Process.Start("explorer.exe", argument);
			}
			catch (Exception)
			{
				string folderOpenError = (string)FindResource("m_FolderOpenError");
				snackbar.MessageQueue?.Enqueue(folderOpenError);
			}
		}

		public async void DownloadHomework(string filePath)
		{
			try
			{
				if (homeworkDialog.IsOpen) return;

				var res = await httpClient.GetAsync(filePath);
				string fileName = res.Content.Headers.ContentDisposition?.FileName.Trim('\"');
				string extension = fileName.Remove(0, fileName.LastIndexOf('.'));

				if (extension == ".txt")
				{
					downloadContent.Header = (string)FindResource("m_TxtPreview");
					downloadContent.TextBoxHint = (string)FindResource("m_TxtContent");

					downloadContent.Text = await res.Content.ReadAsStringAsync();
					downloadContent.IsFileMissing = filePath == null;
					bool? result = (bool?)await homeworkDialog.ShowDialog(downloadContent);

					if (!result.HasValue || result == false)
						return;
				}

				System.Windows.Forms.SaveFileDialog dialog = new()
				{
					FileName = fileName,
					Filter = $"(*{extension})|*{extension}"
				};

				if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					var buffer = await res.Content.ReadAsByteArrayAsync();
					File.WriteAllBytes(dialog.FileName, buffer);

					string homeworkDownloaded = (string)FindResource("m_HomeworkDownloaded");
					string openInExplorer = (string)FindResource("m_OpenInExplorer");
					snackbar.MessageQueue?.Enqueue(homeworkDownloaded, openInExplorer, OpenFileInExplorer, dialog.FileName);
					homeworkDialog.IsOpen = false;
				}
			}
			catch (Exception)
			{
				string homeworkDownloadError = (string)FindResource("m_HomeworkDownloadError");
				snackbar.MessageQueue?.Enqueue(homeworkDownloadError);
			}
		}

		public async void OpenDownloadUploadedDialog(Homework homework)
		{
			if (homeworkDialog.IsOpen) return;

			downloadContent.Header = (string)FindResource("m_UploadedWork");
			downloadContent.TextBoxHint = (string)FindResource("m_YourComment");

			downloadContent.Text = homework.UploadedHomework.StudentAnswer;
			downloadContent.IsFileMissing = homework.UploadedHomework.FilePath == null;
			bool? result = (bool?)await homeworkDialog.ShowDialog(downloadContent);

			if (result.HasValue && result == true)
				DownloadHomework(homework.UploadedHomework.FilePath);
		}
		public async void OpenUploadDialog(Homework homework, ICollection<Homework> source, Button progress, Button upload, string[]? files = null)
		{
			if (homeworkDialog.IsOpen) return;

			uploadContent.ResetContent();
			uploadContent.Homework = homework;
			uploadContent.HomeworkSource = source;
			uploadContent.Files = files;

			if (homework.Status == HomeworkStatus.Checked)
			{
				uploadContent.Header = (string)FindResource("m_RedoRequest");
				uploadContent.SendButtonName = (string)FindResource("m_RedoSend");
			}
			else
			{
				uploadContent.Header = (string)FindResource("m_UploadWork");
				uploadContent.SendButtonName = (string)FindResource("m_Send");
			}
			bool? result = (bool?)await homeworkDialog.ShowDialog(uploadContent);

			if (result.HasValue && result == true)
			{
				// Делаем кнопку загрузки крутиться
				ButtonProgressAssist.SetIsIndicatorVisible(progress, true);
				upload.IsHitTestVisible = false;
				try
				{
					UploadedHomeworkInfo info;
					if (uploadContent.Files != null && uploadContent.Archive)
					{
						// Архивация
						using MemoryStream stream = new();
						using (ZipArchive zip = new(stream, ZipArchiveMode.Create, true))
						{
							foreach (var path in uploadContent.Files)
								zip.CreateEntryFromAny(path);
						}
						HomeworkFile file = new(uploadContent.ArchiveName, ".zip", stream.ToArray());

						info = await MystatAPISingleton.Client.UploadHomeworkFile(uploadContent.Homework.Id, file, uploadContent.Comment);
					}
					else
						info = await MystatAPISingleton.Client.UploadHomework(uploadContent.Homework.Id, uploadContent.Files?[0], uploadContent.Comment);

					uploadContent.Homework.UploadedHomework = info;
					viewModel.MoveHomework(uploadContent.Homework, HomeworkStatus.Uploaded);

					string workUploaded = (string)FindResource("m_WorkUploaded");
					snackbar.MessageQueue?.Enqueue(workUploaded);
				}
				catch
				{
					string workUploadError = (string)FindResource("m_WorkUploadError");
					snackbar.MessageQueue?.Enqueue(workUploadError);
				}
				ButtonProgressAssist.SetIsIndicatorVisible(progress, false);
				upload.IsHitTestVisible = true;
			}
		}

		public async void OpenDeleteDialog(Homework homework)
		{
			if (homeworkDialog.IsOpen) return;

			bool? result = (bool?)await homeworkDialog.ShowDialog(deleteContent);

			if (result.HasValue && result == true)
			{
				try
				{
					if (await MystatAPISingleton.Client.RemoveHomework(homework.UploadedHomework.Id) == false)
						throw new HttpRequestException("Error deleting homework");

					viewModel.DeleteHomework(homework);
					homework.Status = DateTime.Parse(homework.OverdueTime) < DateTime.Now ?
									  HomeworkStatus.Overdue : HomeworkStatus.Active;
					viewModel.AddHomework(homework);

					string workDeleted = (string)FindResource("m_WorkDeleted");
					snackbar.MessageQueue?.Enqueue(workDeleted);
				}
				catch (Exception)
				{
					string deleteWorkError = (string)FindResource("m_DeleteWorkError");
					snackbar.MessageQueue?.Enqueue(deleteWorkError);
				}
			}
		}
		#endregion

		private void ShowHomeworkSlide()
		{
			transitioner.SelectedIndex = 1;
			foreach (HomeworkList section in homeworkSections)
			{
				section.UpdateNextPageButtonVisibility();
			}
		}

		bool loading = false;

		public async void Refresh() => await Refresh(false); // Because of interface
		public async Task Refresh(bool silent)
		{

			if (loading) return;
			loading = true;
			if (!silent)
			{
				transitioner.SelectedIndex = 0;
			}

			SpecsComboBox.SelectionChanged -= SpecsComboBox_SelectionChanged;
			if (await viewModel.LoadSpecs() && await viewModel.LoadHomework())
			{
				ShowHomeworkSlide();
			}
			else if (!silent)
			{
				transitioner.SelectedIndex = 2; // Switch to error slider
			}
			SpecsComboBox.SelectionChanged += SpecsComboBox_SelectionChanged;
			loading = false;
		}

        // When view model is assigned outside this class
        private async void Control_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue is HomeworksViewModel vm)
			{
				viewModel = vm;
				uploadContent.Host = homeworkDialog;

				OverdueList.Collection = viewModel.Homework[HomeworkStatus.Overdue];
				DeletedList.Collection = viewModel.Homework[HomeworkStatus.Deleted];
				ActiveList.Collection = viewModel.Homework[HomeworkStatus.Active];
				UploadedList.Collection = viewModel.Homework[HomeworkStatus.Uploaded];
				CheckedList.Collection = viewModel.Homework[HomeworkStatus.Checked];

				if (viewModel.LoadedOnce == true)
				{
					SpecsComboBox.SelectionChanged -= SpecsComboBox_SelectionChanged;
					await viewModel.LoadSpecs();
					SpecsComboBox.SelectionChanged += SpecsComboBox_SelectionChanged;
					ShowHomeworkSlide();
				}
				else
				{
					Refresh();
				}

				ScheduleAutoUpdate();
			}
		}

		private async void SpecsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (sender is ComboBox comboBox && comboBox.SelectedItem is Spec spec)
			{
				transitioner.SelectedIndex = 0;
				if (await viewModel.LoadHomework(spec))
				{
					ShowHomeworkSlide();
				}
				else
				{
					transitioner.SelectedIndex = 2; // Switch to error slider
				}
			}
		}

        private async void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is HomeworkTypeListEntry type)
            {
                transitioner.SelectedIndex = 0;
                if (await viewModel.LoadHomework(type: type))
                {
                    ShowHomeworkSlide();
                }
                else
                {
                    transitioner.SelectedIndex = 2; // Switch to error slider
                }
            }
        }
    }
}
