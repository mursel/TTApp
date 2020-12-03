using DbProvider.DAL;
using DbProvider.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TrackAndTrace.Helpers;
using Windows.UI.Xaml.Controls;

namespace TrackAndTrace.ViewModels
{
    public class GlobalViewModel : INotifyPropertyChanged
    {

        public GlobalViewModel() => Task.Run(() => InicijalizirajPodatke());

        #region Odabrane stavke

        private UposlenikViewModel _uposlenik;
        public UposlenikViewModel OdabraniUposlenik
        {
            get { return _uposlenik; }
            set
            {
                if (_uposlenik != value)
                {
                    _uposlenik = value;
                    Set();
                }
            }
        }

        private UposlenikViewModel _autorizovaniKorisnik;
        public UposlenikViewModel AutorizovaniKorisnik
        {
            get { return _autorizovaniKorisnik; }
            set
            {
                if (_autorizovaniKorisnik != value)
                {
                    _autorizovaniKorisnik = value;
                    Set();
                }
            }
        }

        private KupacViewModel _kupac;
        public KupacViewModel OdabraniKupac
        {
            get { return _kupac; }
            set {
                if (_kupac != value)
                {
                    _kupac = value;
                    Set();
                }
            }
        }

        private ProizvodiViewModel _proizvod;
        public ProizvodiViewModel OdabraniProizvod
        {
            get { return _proizvod; }
            set
            {
                if (_proizvod!=value)
                {
                    _proizvod = value;
                    Set();
                }
            }
        }

        private PlanPakovanjaViewModel planPakovanja;
        public PlanPakovanjaViewModel OdabraniPlan
        {
            get { return planPakovanja; }
            set
            {
                if (planPakovanja!=value)
                {
                    planPakovanja = value;
                    Set();
                }
            }
        }

        private dynamic odabraniSerijskiBroj;
        public dynamic OdabraniSerijskiBroj 
        {
            get { return odabraniSerijskiBroj; }
            set
            {
                if (odabraniSerijskiBroj != value)
                {
                    odabraniSerijskiBroj = value;
                    Set();
                }
            }
        }

        #endregion

        #region Posmatrane kolekcije podataka

        public ObservableCollection<RolaViewModel> ListaRola { get; private set; }

        public ObservableCollection<GradViewModel> ListaGradova { get; private set; }

        public ObservableCollection<DrzavaViewModel> ListaDrzava { get; private set; }

        public ObservableCollection<UposlenikViewModel> ListaUposlenika { get; private set; }

        public ObservableCollection<KupacViewModel> ListaKupaca { get; private set; }

        public ObservableCollection<ProizvodiViewModel> ListaProizvoda { get; private set; }

        public ObservableCollection<PlanPakovanjaViewModel> ListaPlanova { get; private set; }

        public List<Tuple<int, string>> TipoviEtiketa = new List<Tuple<int, string>>
        {
                new Tuple<int, string>(0, "Jedinačne"),
                new Tuple<int, string>(1, "Omotne"),
                new Tuple<int, string>(3, "Paketne"),
                new Tuple<int, string>(4, "Paletne")
        };

        public List<Tuple<int, string>> TipoviPaleta => TipoviEtiketa.Where(t => t.Item1 == 4 && t.Item1 == 9).ToList();


        #endregion

        #region Properties

        private bool ucitavaSe;
        public bool UcitavaSe
        {
            get { return ucitavaSe; }
            set { ucitavaSe = value; Set(); }
        }

        private bool _isMenuEnabled = false;

        public bool IsMenuEnabled
        {
            get { return _isMenuEnabled; }
            set { _isMenuEnabled = value; Set(); }
        }

        public delegate void OnDataLoaded(bool rezultat);
        public event OnDataLoaded OnLoaded = delegate { };

        #endregion

        public async Task InicijalizirajPodatke()
        {
            ListaRola = new ObservableCollection<RolaViewModel>();
            ListaGradova = new ObservableCollection<GradViewModel>();
            ListaDrzava = new ObservableCollection<DrzavaViewModel>();
            ListaUposlenika = new ObservableCollection<UposlenikViewModel>();
            ListaKupaca = new ObservableCollection<KupacViewModel>();
            ListaProizvoda = new ObservableCollection<ProizvodiViewModel>();
            ListaPlanova = new ObservableCollection<PlanPakovanjaViewModel>();

            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                UcitavaSe = true;
            });

            var role = await App.RoleRepository.GetList();

            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                ListaRola.Clear();
                foreach (var item in role)
                {
                    ListaRola.Add(new RolaViewModel(item));
                }
            });

            var gradovi = await App.GradoviRepository.GetList();

            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                ListaGradova.Clear();
                foreach (var item in gradovi)
                {
                    ListaGradova.Add(new GradViewModel(item));
                }
            });

            var drzave = await App.DrzaveRepository.GetList();

            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                ListaDrzava.Clear();
                foreach (var item in drzave)
                {
                    ListaDrzava.Add(new DrzavaViewModel(item));
                }
            });

            var uposlenici = await App.UposleniciRepository.GetList();

            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                ListaUposlenika.Clear();
                foreach (var u in uposlenici)
                {
                    if (u.Username != "admin")
                        ListaUposlenika.Add(new UposlenikViewModel(u));
                }

            });

            var kupci = await App.KupciRepository.GetList();

            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                ListaKupaca.Clear();
                foreach (var item in kupci)
                {
                    ListaKupaca.Add(new KupacViewModel(item));
                }


            });

            var proizvodi = await App.ProizvodiRepository.GetList();

            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                ListaProizvoda.Clear();
                foreach (var item in proizvodi)
                {
                    ProizvodiViewModel proizvodiViewModel = new ProizvodiViewModel(item);
                    Task.Run(() => proizvodiViewModel.UcitajPlanPakovanjaDefault());
                    ListaProizvoda.Add(proizvodiViewModel);
                }

            });

            var planovi = await App.PlanPakovanjaRepository.GetList();

            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                ListaPlanova.Clear();
                foreach (var item in planovi)
                {
                    ListaPlanova.Add(new PlanPakovanjaViewModel(item));                    
                }

                UcitavaSe = false;
                IsMenuEnabled = true;

            });

            try
            {
                OnLoaded(true);
            }
            catch (Exception ex)
            {
                var eg = ex.Message;
            }
        }

        private void GlobalViewModel_OnLoaded(bool rezultat)
        {
            throw new NotImplementedException();
        }

        #region Metode

        public async Task UkloniUposlenikaAsync()
        {
            if (_uposlenik != null && _uposlenik.Model != null)
            {
                await App.UposleniciRepository.Remove(_uposlenik.Model);
                ListaUposlenika.Remove(_uposlenik);
                OdabraniUposlenik = null;
                //await InicijalizirajPodatke();    // nije potrebno, jer je ListaUposlenika observable lista
            }
        }

        public async Task UkloniKupcaAsync()
        {
            if (_kupac!=null && _kupac.Model!=null)
            {
                var rezultat = await App.KupciRepository.Remove(_kupac.Model);
                if (rezultat)
                {
                    ListaKupaca.Remove(_kupac);
                    OdabraniKupac = null;
                }
                else
                {
                    var errDlg = new ContentDialog();
                    errDlg.Title = "Greška";
                    errDlg.Content = "Kupca nije moguće ukloniti, jer postoje transakcije vezane za kupca!";
                    errDlg.CloseButtonText = "Zatvori";
                    errDlg.DefaultButton = ContentDialogButton.Close;
                    await errDlg.ShowAsync();
                }
            }
        }

        public async Task UkloniProizvodAsync()
        {
            if (_proizvod!=null && _proizvod.Model!=null)
            {
                // prvo ukloni plan pakovanja za proizvod
                var planPakProizvod = await App.PlanPakDefaultRepository.GetByProductId(_proizvod.Model.Id);

                if (planPakProizvod != null)
                    await App.PlanPakDefaultRepository.Remove(planPakProizvod);

                // ukloni proizvod
                await App.ProizvodiRepository.Remove(_proizvod.Model);
                ListaProizvoda.Remove(_proizvod);
                OdabraniProizvod = null;
            }
        }

        public async Task UkloniPlanAsync()
        {
            if (planPakovanja!=null && planPakovanja.Model != null)
            {
                await App.PlanPakovanjaRepository.Remove(planPakovanja.Model);
                ListaPlanova.Remove(planPakovanja);
                OdabraniPlan = null;
            }
        }


        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        private void Set([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
