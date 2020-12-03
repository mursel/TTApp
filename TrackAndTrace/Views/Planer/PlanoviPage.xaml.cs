using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TrackAndTrace.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TrackAndTrace.Views.Planer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlanoviPage : Page
    {
        public PlanoviPage()
        {
            this.InitializeComponent();
        }

        public GlobalViewModel ViewModel => App.ViewModel;

        private void abrNovi_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PlanDetalji));
        }

        private void abrIzmjena_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PlanDetalji), ViewModel.OdabraniPlan.Model.Id);
        }

        private void dataGridPlanovi_LoadingRow(object sender, Microsoft.Toolkit.Uwp.UI.Controls.DataGridRowEventArgs e)
        {
            var planPakovanjaVM = (PlanPakovanjaViewModel)e.Row.DataContext;
            if (planPakovanjaVM.TrajanjePlana < 60 && planPakovanjaVM.StatusPlana == 1)
                e.Row.Background = new SolidColorBrush(Colors.IndianRed);
        }
    }
}
