using Prism.Mvvm;
using Prism.Regions;

namespace UDPMonitor.ViewModels.Base
{
    public abstract class ViewModelBase : BindableBase, INavigationAware, IRegionMemberLifetime
    {
        public bool KeepAlive { get => ShouldKeepAlive(); }

        protected ViewModelBase()
        {
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            OnInitializeCommands();
        }

        protected virtual void OnInitializeCommands()
        {
        }

        protected virtual bool ShouldKeepAlive()
        {
            return true;
        }

        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return KeepAlive;
        }

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
        }
    }
}