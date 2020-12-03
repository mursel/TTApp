using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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

    public sealed partial class PlanDetalji : Page
    {
        public PlanPakovanjaViewModel ViewModel { get; set; }
        public GlobalViewModel AppViewModel => App.ViewModel;

        public PlanDetalji()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter == null)
            {
                ViewModel = new PlanPakovanjaViewModel()
                {
                    IsEditMode = false,
                    IsNewPlanMode = true
                };
            }
            else
            {
                var index = int.Parse(e.Parameter.ToString());
                ViewModel = App.ViewModel.ListaPlanova.Where(u => u.Model.Id == index).First();
                if (ViewModel != null)
                {
                    ViewModel.IsEditMode = true;
                    ViewModel.IsNewPlanMode = false;
                }
            }

            ViewModel.OnSave += ViewModel_OnSave;

            cbKupci.ItemsSource = App.ViewModel.ListaKupaca;
            cbProizvodi.ItemsSource = App.ViewModel.ListaProizvoda;

        }

        private void ViewModel_OnSave(bool rezultat)
        {
            if (rezultat)
            {
                if (Frame.CanGoBack)
                {
                    Frame.GoBack();
                }
            }
        }

        private async void cbProizvodi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await ViewModel.UcitajProizvodPlanPak();
        }
    }
}
