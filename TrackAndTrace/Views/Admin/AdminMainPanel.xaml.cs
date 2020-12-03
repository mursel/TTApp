using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TrackAndTrace.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TrackAndTrace.Views.Admin
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AdminMainPanel : Page
    {
        public AdminMainPanel()
        {
            this.InitializeComponent();
        }

        public GlobalViewModel ViewModel => App.ViewModel;

        public Frame AppFrame => adminContent;

        private void GlavniNavMeni_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                //adminContent.Navigate(typeof(mojaSettingsStranica));
            }
            else
            {
                var odabir = (NavigationViewItem)args.SelectedItem;
                string nazivStranice = "TrackAndTrace.Views.Admin." + ((string)odabir.Tag);
                Type type = Type.GetType(nazivStranice);
                AppFrame.Navigate(type);
            }
        }

        private void GlavniNavMeni_Loaded(object sender, RoutedEventArgs e)
        {
            //GlavniNavMeni.SelectedItem = UposleniciMenuItem;
            AppFrame.Navigate(typeof(Status));
        }

        private void GlavniNavMeni_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if (AppFrame.CanGoBack)
            {
                AppFrame.GoBack();
            }
        }

        private void adminContent_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                if (e.SourcePageType == typeof(UposleniciPage))
                {
                    GlavniNavMeni.SelectedItem = UposleniciMenuItem;
                }
                else if (e.SourcePageType == typeof(KupciPage))
                {
                    GlavniNavMeni.SelectedItem = KupciMenuItem;
                }
                else if (e.SourcePageType == typeof(ProizvodiPage))
                {
                    GlavniNavMeni.SelectedItem = ProizvodiMenuItem;
                }
            }

        }

        private void GlavniNavMeni_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            // imeplemntirati posle nekad navigaciju izmedju stranica
        }
    }
}
