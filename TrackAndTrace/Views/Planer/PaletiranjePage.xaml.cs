using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using TrackAndTrace.ViewModels;
using Windows.ApplicationModel.DataTransfer;
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
    public sealed partial class PaletiranjePage : Page
    {

        public SerijskiBrojeviViewModel ViewModel { get; set; }
        public GlobalViewModel AppViewModel => App.ViewModel;

        public PaletiranjePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var aktivniPlanovi = App.ViewModel.ListaPlanova.Where(p => p.StatusPlana == 1);
            cbPlanovi.ItemsSource = aktivniPlanovi.ToList();

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            //ViewModel.ListaSerijskih.Clear();
            //ViewModel.ListaSerijskihRaw.Clear();
        }

        private void cbPlanovi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel = new SerijskiBrojeviViewModel(App.ViewModel.OdabraniPlan);

            tvPalete.RootNodes.Clear();

            UcitajPaletePakete();

            UcitajPaketeBezPalete();

        }

        private void UcitajPaletePakete()
        {
            // postavi na pakete kako bi ucitali iz baze paketne za ovaj plan pakovanja
            ViewModel.OdabraniTip = SerijskiBrojeviViewModel.TipSerijskogBroja.OuterPackaging;

            // ucitaj palete i pakete
            Task<IEnumerable<PaletnaViewModel>> task = Task.Run(() => ViewModel.VratiPaletneSaPaketima());

            // nakon sto je operacija ucitavanja zavrsena nastavi dalje
            task.ContinueWith(t1 =>
            {
                if (t1.Status == TaskStatus.RanToCompletion)
                {
                    t1.Result.ToList().ForEach(async p =>
                    {
                        await tvPalete.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            if (tvPalete.RootNodes.Where(r => r.Content.ToString() == p.SerijskiBroj).FirstOrDefault() == null)
                            {
                                tvPalete.RootNodes.Add(new TreeViewNode()
                                {
                                    Content = p.SerijskiBroj
                                });
                                if (p.Model.Paketne != null)
                                {
                                    p.Model.Paketne.ForEach(pak =>
                                    {
                                        tvPalete.RootNodes.Last().Children.Add(new TreeViewNode() { Content = pak.SerijskiBroj });
                                    });
                                }
                            }
                        });
                    });
                }
            });
        }
        
        private async void UcitajPaketeBezPalete()
        {
            // pripremi kolekcije prije ucitavanja paletnih etiketa
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                lvPaketi.ItemsSource = null;
                ViewModel.ListaSerijskih.Clear();
                ViewModel.ListaSerijskihRaw.Clear();
                App.ViewModel.UcitavaSe = true;
            });

            Task task = Task.Run(() => ViewModel.UcitajPaketeBezPalete());

            await task.ContinueWith(async t1 =>
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    if (t1.Status == TaskStatus.RanToCompletion)
                    {
                        var paketne = ViewModel.ListaSerijskih.Cast<PaketnaViewModel>().ToList();
                        lvPaketi.ItemsSource = paketne;
                    }
                });
                AppViewModel.UcitavaSe = false;
            });
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //App.ViewModel.IsMenuEnabled = false;
            tvPalete.RootNodes.Add(new TreeViewNode()
            {
                Content = await ViewModel.AddNewPallet()
            });
        }

        private async void btnRemovePacket_Click(object sender, RoutedEventArgs e)
        {
            if (tvPalete.SelectedNodes.Count > 0)
            {
                foreach (var item in tvPalete.SelectedNodes)
                {
                    if (item.Content.ToString().Contains("SN03"))
                    {
                        try
                        {
                            var tempPaketna = item.Content.ToString();
                            var tempRoot = item.Parent.Content.ToString();

                            Task<bool> task = Task.Run(()=> App.SbRepository.RemovePaketnaFromPaletnaAsync(tempPaketna));
                            await task.ContinueWith(async t1 =>
                            {
                                await tvPalete.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                                {
                                    var root = tvPalete.RootNodes.Where(r => r.Content.ToString() == tempRoot).First();
                                    root.Children.Remove(item);
                                });
                            });

                            Task<DbProvider.Model.Paketna> task2 = Task.Run(()=> App.SbRepository.GetPaketnaByNameAsync(tempPaketna));
                            await task2.ContinueWith(async t1 => {
                                await lvPaketi.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                                {
                                    ViewModel.ListaSerijskih.Add(new PaketnaViewModel(t1.Result));
                                    lvPaketi.ItemsSource = ViewModel.ListaSerijskih;
                                });
                            });
                            
                        }
                        catch (Exception ex)
                        {
                            var exm = ex.Message;
                        }
                    }
                }
                tvPalete.SelectedNodes.Clear();
            }
        }

        private async void btnAddPacket_Click(object sender, RoutedEventArgs e)
        {
            // moramo pohraniti u privremenu varijablu, jer ne mozemo da uklanjamo stavke direktno koristeci listview.Items.remove
            // time modifikujemo itemssource property
            var selectedPackets = lvPaketi.SelectedItems.ToList();

            // potrebno je izabrati paletu??
            if (tvPalete.SelectedNodes.Count > 0 && lvPaketi.SelectedItems.Count > 0)
            {
                var izabranaPaleta = tvPalete.SelectedNodes.First().Content;
                foreach (PaketnaViewModel izabraniPaket in lvPaketi.SelectedItems)
                {
                    Task<bool> task = Task.Run(()=>ViewModel.DodajPaketUPaletu(izabraniPaket, izabranaPaleta));
                    await task.ContinueWith( async t1 =>
                    {
                        if (t1.Status == TaskStatus.RanToCompletion)
                        {
                            await tvPalete.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                            {
                                var rootNode = tvPalete.RootNodes.Where(r => r.Content == izabranaPaleta).First();
                                rootNode.Children.Add(new TreeViewNode()
                                {
                                    Content = izabraniPaket.SerijskiBroj
                                });
                                tvPalete.Expand(rootNode);                                
                            });
                        }
                    });
                }

                foreach (var item in selectedPackets)
                {
                    bool rezultat = ViewModel.ListaSerijskih.Remove(item);
                }

                lvPaketi.ItemsSource = ViewModel.ListaSerijskih;
                // inicijaliziraj nanovo palete i pakete
                //UcitajPaletePakete();
            }
                        
        }
    }

}
