using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using UDPMonitor.ViewModels.Base;

namespace UDPMonitor.ViewModels
{
    public class MessageDetail_ViewModel : DialogViewModelBase
    {
        private DateTime _timeStamp;

        public DateTime TimeStamp
        {
            get => _timeStamp;
            set => SetProperty(ref _timeStamp, value);
        }

        private string _text;

        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }

        public MessageDetail_ViewModel()
        {
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            Title = "Message Details";

            var message = parameters.GetValue<ODTMessage>("message");
            if (message == null) return;

            TimeStamp = message.TimeStamp;
            Text = message.Text;
        }
    }
}