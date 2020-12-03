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
    public sealed partial class KupciPage : Page
    {
        

        public KupciPage()
        {
            this.InitializeComponent();
        }

        public GlobalViewModel ViewModel => App.ViewModel;

        private void abrNovi_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(KupacDetalji));
        }

        private void abrIzmjena_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(KupacDetalji), ViewModel.OdabraniKupac.Model.Id);
        }                
    }
}
