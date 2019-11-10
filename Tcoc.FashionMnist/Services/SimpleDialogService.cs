using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tcoc.FashionMnist.Views.Dialogs;

namespace Tcoc.FashionMnist.Services
{
    class SimpleDialogService : IDialogService
    {
        private Lazy<MainWindow> _lmainWindow;

        public SimpleDialogService()
        {
            _lmainWindow = new Lazy<MainWindow>(() =>
                (MainWindow)Application.Current.MainWindow);
        }

        public async Task<string> ShowModelNameDialog(string suggestedfileName)
        {
            var nameDialog = new FileNameDialog();
            var tcs = new TaskCompletionSource<bool>();
            nameDialog.NameTextBox.Text = suggestedfileName;

            void SaveButtonClicked(object s, RoutedEventArgs a)
            {
                _lmainWindow.Value.DialogRegion.Content = null;
                tcs.TrySetResult(true);
            }
            void CancelButtonClicked(object s, RoutedEventArgs a)
            {
                _lmainWindow.Value.DialogRegion.Content = null;
                tcs.TrySetResult(false);
            }

            nameDialog.SaveButton.Click += SaveButtonClicked;
            nameDialog.CancelButton.Click += CancelButtonClicked;
            _lmainWindow.Value.DialogRegion.Content = nameDialog;
            bool okButtonClicked = await tcs.Task;
            nameDialog.SaveButton.Click -= SaveButtonClicked;
            nameDialog.CancelButton.Click -= CancelButtonClicked;
            if(okButtonClicked)
            {
                return nameDialog.NameTextBox.Text;
            } else
            {
                return String.Empty;
            }
        }

        public async Task ShowProgressDialog(string header, Action<Action<string>> actionDelegate)
        {
            var progressDialog = new ProgressDialog();
            progressDialog.HeaderTb.Text = header;

            void Report(string step)
            {
                Application.Current.Dispatcher.Invoke(() => progressDialog.StepTb.Text = step);
            }

            _lmainWindow.Value.DialogRegion.Content = progressDialog;

            await Task.Run(() => actionDelegate(Report));

            _lmainWindow.Value.DialogRegion.Content = null;
        }
    }
}
