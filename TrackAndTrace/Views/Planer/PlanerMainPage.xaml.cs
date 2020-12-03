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

namespace TrackAndTrace.Views.Planer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlanerMainPage : Page
    {
        public PlanerMainPage()
        {
            this.InitializeComponent();
        }

        public GlobalViewModel ViewModel => App.ViewModel;

        public Frame AppFrame => planoviContent;

        private void planoviContent_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                if (e.SourcePageType == typeof(PlanoviPage))
                {
                    GlavniNavMeni.SelectedItem = PlanoviMenuItem;
                }
                else if (e.SourcePageType == typeof(SerijskiBrojeviPage))
                {
                    GlavniNavMeni.SelectedItem = SerijskiMenuItem;
                }
                else if (e.SourcePageType == typeof(PaletiranjePage))
                {
                    GlavniNavMeni.SelectedItem = PaleteMenuItem;
                }
                else if (e.SourcePageType == typeof(IzvozPodatakaXml))
                {
                    GlavniNavMeni.SelectedItem = XMLMenuItem;
                }
            }
        }

        private void GlavniNavMeni_Loaded(object sender, RoutedEventArgs e)
        {
            AppFrame.Navigate(typeof(Status));
        }

        private void GlavniNavMeni_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            // imeplemntirati posle nekad navigaciju izmedju stranica
        }

        private void GlavniNavMeni_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                // nista sad za sad
            }
            else
            {
                var odabir = (NavigationViewItem)args.SelectedItem;
                string nazivStranice = "TrackAndTrace.Views.Planer." + ((string)odabir.Tag);
                Type type = Type.GetType(nazivStranice);
                AppFrame.Navigate(type);
            }
        }

        private void GlavniNavMeni_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if (AppFrame.CanGoBack)
            {
                AppFrame.GoBack();
            }
        }
    }
}
