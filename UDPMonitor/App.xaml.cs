using Prism.Ioc;
using Prism.Regions;
using System;
using System.Windows;
using UDPMonitor.Business;
using UDPMonitor.Business.Config;
using UDPMonitor.Business.Interfaces;
using UDPMonitor.Core;
using UDPMonitor.Core.Configuration;
using UDPMonitor.ViewModels;
using UDPMonitor.Views;

namespace UDPMonitor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public static class RegisteredViews
        {
            public const string Inbound_View = "Inbound_View";
            public const string Outbound_View = "Outbound_View";
            public const string DialogDetail_View = "DialogDetail_View";
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow_View>();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            var regionManager = Container.Resolve<IRegionManager>();

            regionManager.RequestNavigate(RegionNames.InboundRegion, RegisteredViews.Inbound_View);
            regionManager.RequestNavigate(RegionNames.OutboundRegion, RegisteredViews.Outbound_View);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            RegisterConfigManager(containerRegistry);
            RegisterPages(containerRegistry);
            RegisterDialogs(containerRegistry);
            RegisterServices(containerRegistry);
        }

        private void RegisterConfigManager(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IConfigurationManager<UDPManagerConfiguration>>(ConfigurationFactory);
        }

        private void RegisterDialogs(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<MessageDetail_View, MessageDetail_ViewModel>(RegisteredViews.DialogDetail_View);
        }

        private static void RegisterServices(IContainerRegistry containerRegistry)
        {
          
            containerRegistry.RegisterSingleton<IInboundService, InboundService>();
            containerRegistry.RegisterSingleton<IOutboundService, OutboundService>();
        }

        private static IConfigurationManager<UDPManagerConfiguration> ConfigurationFactory(IContainerProvider containerProvider)
        {
            var configManager = new ConfigFile_JsonConfigManager(@$"{ApplicationService.ConfigFolderPath}\config.json");

            configManager.ReadFromFile();

            return configManager;
        }

        private static void RegisterPages(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<Inbound_View, Inbound_ViewModel>(RegisteredViews.Inbound_View);
            containerRegistry.RegisterForNavigation<Outbound_View, Outbound_ViewModel>(RegisteredViews.Outbound_View);
        }
    }
}