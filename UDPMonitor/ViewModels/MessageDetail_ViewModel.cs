using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;

namespace UDPMonitor.ViewModels
{
    public class MessageDetail_ViewModel : BindableBase, IDialogAware
    {
        private string _timeStamp;

        public string TimeStamp
        {
            get { return _timeStamp; }
            set { SetProperty(ref _timeStamp, value); }
        }

        private string _text;

        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }

        public string Title => "Details";

        public DelegateCommand CloseCommand { get; private set; }

        public MessageDetail_ViewModel()
        {
            CloseCommand = new DelegateCommand(OnDialogClosed);
        }

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            var message = parameters.GetValue<ODTMessage>("message");

            TimeStamp = message.TimeStamp;
            Text = message.Text;
        }
    }
}