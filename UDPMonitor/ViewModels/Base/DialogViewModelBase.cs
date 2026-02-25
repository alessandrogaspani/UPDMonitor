using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;

namespace UDPMonitor.ViewModels.Base
{
    public abstract class DialogViewModelBase : BindableBase, IDialogAware
    {
        public string Title { get; protected set; }

        public event Action<IDialogResult> RequestClose;
        public DelegateCommand CloseCommand { get; }
        protected DialogViewModelBase()
        {
            CloseCommand = new DelegateCommand(OnDialogClosed);
        }

        public abstract void OnDialogOpened(IDialogParameters parameters);

        public virtual bool CanCloseDialog()
        {
            return true;
        }

        public void CloseDialog(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }

        public virtual void OnDialogClosed()
        {
            CloseDialog(new DialogResult(ButtonResult.OK));
        }
    }
}