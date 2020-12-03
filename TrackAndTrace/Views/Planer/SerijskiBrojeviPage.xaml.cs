using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
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
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SerijskiBrojeviPage : Page
    {
        public SerijskiBrojeviViewModel ViewModel { get; set; }
        public GlobalViewModel AppViewModel => App.ViewModel;

        public SerijskiBrojeviPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var aktivniPlanovi = App.ViewModel.ListaPlanova.Where(p => p.StatusPlana == 1);
            cbPlanovi.ItemsSource = aktivniPlanovi.ToList();
        }

        private void cbTipovi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                switch (cbTipovi.SelectedValue)
                {
                    case 0:
                        ViewModel.OdabraniTip = SerijskiBrojeviViewModel.TipSerijskogBroja.Item;            // jedinance
                        break;
                    case 1:
                        ViewModel.OdabraniTip = SerijskiBrojeviViewModel.TipSerijskogBroja.InnerPackaging;  // omotne
                        break;
                    case 3:
                        ViewModel.OdabraniTip = SerijskiBrojeviViewModel.TipSerijskogBroja.OuterPackaging;  // paketne
                        break;
                    case 4:
                        ViewModel.OdabraniTip = SerijskiBrojeviViewModel.TipSerijskogBroja.Pallet;          // paletne
                        break;
                }

                //await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                //{
                    dataGridSerijski.ItemsSource = null;
                    ViewModel.ListaSerijskih.Clear();
                    ViewModel.ListaSerijskihRaw.Clear();
                    App.ViewModel.UcitavaSe = true;
                //});

                UcitajRangoveAsync();

                UcitajPostojeceSerijskeBrojeve();
            }
        }

        private  void UcitajRangoveAsync()
        {


            Task task = Task.Run(() => ViewModel.PostaviRangoveBrojevaAsync());

            task.Wait();

            task.ContinueWith(async t1 =>
            {
#if DEBUG
                //Thread.Sleep(1500);
#endif
                if (t1.Status == TaskStatus.RanToCompletion)
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        odBroja.IsEnabled = true;
                        doBroja.IsEnabled = true;

                        odBroja.ItemsSource = ViewModel.NedostajuciBrojevi;
                        doBroja.ItemsSource = ViewModel.NedostajuciBrojevi;

                        btnGenerisi.IsEnabled = true;
                        tbPretraga.IsEnabled = true;

                        App.ViewModel.UcitavaSe = false;
                    });
                }
            });

        }

        private void btnGenerisi_Click(object sender, RoutedEventArgs e)
        {
            App.ViewModel.UcitavaSe = true;

            if(dataGridSerijski.ItemsSource!= null)
                dataGridSerijski.ItemsSource = null;

            if (ViewModel.ListaSerijskih.Count > 0)
                ViewModel.ListaSerijskih.Clear();

            if (ViewModel.ListaSerijskihRaw.Count > 0)
                ViewModel.ListaSerijskihRaw.Clear();

            int start = (odBroja.SelectedItem != null) ? (int)odBroja.SelectedItem : 0;
            int stop = (doBroja.SelectedItem != null) ? (int)doBroja.SelectedItem : 0;
            
            btnGenerisi.IsEnabled = false;

            //Task t = Task.Run(() => ViewModel.GenerirajAsync(start, stop));
            Task t = Task.Factory.StartNew(() => ViewModel.GenerirajAsync(start, stop), 
                CancellationToken.None, 
                TaskCreationOptions.LongRunning, 
                TaskScheduler.FromCurrentSynchronizationContext());

            t.ContinueWith(async t1 =>
            {
                if (t1.Status == TaskStatus.RanToCompletion)
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        dataGridSerijski.ItemsSource = ViewModel.ListaSerijskih;
                        btnSnimi.IsEnabled = true;
                        tbPretraga.IsEnabled = true;
                        btnGenerisi.IsEnabled = true;
                        AppViewModel.UcitavaSe = false;
                        
                    });
                }
            });
        }

        private void btnSnimi_Click(object sender, RoutedEventArgs e)
        {
            btnSnimi.IsEnabled = false;
            btnGenerisi.IsEnabled = false;
            tbPretraga.IsEnabled = false;
            cbTipovi.IsEnabled = false;
            cbPlanovi.IsEnabled = false;
            App.ViewModel.UcitavaSe = true;

            Task t = Task.Run(() => ViewModel.SnimiSerijskeAsync());

            t.ContinueWith(async t1 =>
            {
                if (t1.Status == TaskStatus.RanToCompletion)
                {

                    await ViewModel.VeziJedinacneZaOmotne();

                    await ViewModel.VeziOmotneZaPaketne();

                    await ViewModel.VeziPaketneZaPaletne();

                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        btnGenerisi.IsEnabled = true;
                        tbPretraga.IsEnabled = true;
                        cbTipovi.IsEnabled = true;
                        cbPlanovi.IsEnabled = true;
                        
                        AppViewModel.UcitavaSe = false;

                    });                    
                }
            });

            // nakon što smo pohranili serijske brojeve u bazu ponovo ucitaj rangove brojeva
            // ako nisu svi iskoristeni vec
            //UcitajRangoveAsync();
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var rezultatPretrage = ViewModel.ListaSerijskih.Where(j => j.SerijskiBroj.IndexOf(((AutoSuggestBox)sender).Text, StringComparison.CurrentCultureIgnoreCase) >= 0);
                dataGridSerijski.ItemsSource = rezultatPretrage;
            }
        }
        
        private void cbPlanovi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel = new SerijskiBrojeviViewModel(App.ViewModel.OdabraniPlan);
            ViewModel.UcitajIdentifikatore();
        }

        private void UcitajPostojeceSerijskeBrojeve()
        {
            Task task = Task.Run(() => ViewModel.UcitajPostojeceSerijske());

            task.ContinueWith(async t1 =>
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    if (t1.Status == TaskStatus.RanToCompletion)
                    {
                        dataGridSerijski.ItemsSource = ViewModel.ListaSerijskih;
                        btnPrint.IsEnabled = true;
                        AppViewModel.UcitavaSe = false;
                    }
                });
            });
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PrintPage), cbTipovi.SelectedValue);
        }
    }
}
