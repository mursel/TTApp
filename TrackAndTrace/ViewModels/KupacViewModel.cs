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
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace TrackAndTrace.ViewModels
{
    public class KupacViewModel : INotifyPropertyChanged
    {

        public KupacViewModel(Kupac kupac = null)
        {
            Model = kupac ?? new Kupac();
            IsNewKupacMode = (kupac != null) ? false : true;
            IsEditMode = (kupac == null) ? false : true;
            Naslov = (IsNewKupacMode) ? "Novi kupac" : kupac.Naziv;
            SearchString_Grad = (kupac != null) ? kupac.Grad : "";
            SearchString_Drzava = (kupac != null) ? kupac.Drzava : "";
        }


        private Kupac kupac;
        public Kupac Model
        {
            get { return kupac; }
            set
            {
                if (kupac != value)
                {
                    kupac = value;
                    Set(string.Empty);
                }
            }
        }

        // standardne vrijednosti koje služe za enkodiranje u barcode
        // 90 = oznaka države i proizvodnog mjesta (Broj proizvodnog mjesta)
        // 250 = serijski broj proizvoda 
        private List<Identifikator> identifikatori;
        public List<Identifikator> Identifikatori
        {
            get
            {
                if (identifikatori == null)
                {
                    identifikatori = new List<Identifikator>
                    {
                        new Identifikator() { KupacId = Model.Id, Naziv = "90" },
                        new Identifikator() { KupacId = Model.Id, Naziv = "250" }
                    };                    
                }
                return identifikatori;
            }
        }

        private Identifikator identifikator;
        public Identifikator OdabraniIdentifikator
        {
            get { return identifikator; }
            set
            {
                if (identifikator != value)
                {
                    identifikator = value;
                    Set();
                }
            }
        }

        private IdentifikatorViewModel identifikatorViewModel;
        public IdentifikatorViewModel OdabraniIdentViewModel
        {
            get { return identifikatorViewModel; }
            set
            {
                if (identifikatorViewModel != value)
                {
                    identifikatorViewModel = value;
                    Set();
                }
            }
        }


        // već postojeći identifikatori 
        public ObservableCollection<IdentifikatorViewModel> ListaIdentifikatora { get; private set; } = new ObservableCollection<IdentifikatorViewModel>();

        #region Properties

        private string _naslov;
        public string Naslov
        {
            get { return _naslov; }
            set { if (_naslov != value) { _naslov = value; Set(); } }
        }


        private bool isEditMode = false;
        public bool IsEditMode
        {
            get { return isEditMode; }
            set { if (isEditMode != value) { isEditMode = value; Set(); } }
        }

        private bool isNewKupacMode ;
        public bool IsNewKupacMode
        {
            get { return isNewKupacMode; }
            set { if (isNewKupacMode != value) { isNewKupacMode = value; Set(); } }
        }

        private string searchString_grad;
        public string SearchString_Grad
        {
            get { return searchString_grad; }
            set { if (searchString_grad != value) { searchString_grad = value; Set(); } }
        }

        private string searchString_drzava;
        public string SearchString_Drzava
        {
            get { return searchString_drzava; }
            set { if (searchString_drzava != value) { searchString_drzava = value; Set(); } }
        }

        #endregion

        #region Atributi

        public int Id { get { return Model.Id; } }

        public int GradId
        {
            get { return Model.GradId; }
            set
            {
                if (Model.GradId != value)
                {
                    Model.GradId = value;
                    Set();
                    Set(nameof(GradId));
                }
            }
        }

        public int DrzavaId
        {
            get { return Model.DrzavaId; }
            set
            {
                if (Model.DrzavaId != value)
                {
                    Model.DrzavaId = value;
                    Set();
                    Set(nameof(DrzavaId));
                }
            }
        }

        public string Grad
        {
            get { return Model.Grad; }
        }

        public string Drzava
        {
            get { return Model.Drzava; }
        }

        public string Naziv
        {
            get { return Model.Naziv; }
            set
            {
                if (Model.Naziv != value)
                {
                    Model.Naziv = value;
                    Set();
                    Set(nameof(Naziv));
                }
            }
        }

        public string BrojProizvodnogMjesta
        {
            get { return Model.BrojProizvodnogMjesta; }
            set
            {
                if (Model.BrojProizvodnogMjesta != value)
                {
                    Model.BrojProizvodnogMjesta = value;
                    Set();
                    Set(nameof(BrojProizvodnogMjesta));
                }
            }
        }


        #endregion

        public delegate void OnSaveCustomer(bool rezultat);
        public event OnSaveCustomer OnSave;

        public async Task SnimiKupcaAsync()
        {
            // ako je novi dodaj kupca na listu
            if (IsNewKupacMode)
            {
                IsNewKupacMode = false;
                App.ViewModel.ListaKupaca.Add(this);
                var noviKupac = await App.KupciRepository.Add(Model);
                if (noviKupac != null)
                {
                    var rezultat = await SnimiIdentifikatoreAsync(noviKupac.Id);
                    if (rezultat)
                        OnSave(true);
                    else
                        OnSave(false);
                }
            }

            // postojeci? Izmjeni kupca
            if (IsEditMode)
            {
                IsEditMode = false;
                var rezultat = await App.KupciRepository.Update(Model);
                OnSave(rezultat);
            }
        }

        public async Task UcitajIdentifikatoreAsync()
        {
            var lista = await App.IdentifikatoriRepository.GetList(i => i.KupacId == Model.Id);

            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                ListaIdentifikatora.Clear();
                foreach (var item in lista)
                {
                    ListaIdentifikatora.Add(new IdentifikatorViewModel(item));
                }
            });
        }

        public async Task<bool> SnimiIdentifikatoreAsync(int kupacId)
        {
            if (ListaIdentifikatora.Count > 0)
            {
                foreach (var ident in ListaIdentifikatora)
                {
                    ident.KupacId = kupacId;
                    var noviIdent = await App.IdentifikatoriRepository.Add(ident.Model);
                    if (noviIdent == null)
                        return false;
                }
            }
            return true;
        }

        public void UnosIdentifikatoraAsync()
        {
            // ako nije null
            if (OdabraniIdentifikator != null)
            {
                // ... i ako ne postoji isti identifikator u listi
                if (ListaIdentifikatora.Where(i => i.Naziv == OdabraniIdentifikator.Naziv).Count() == 0)
                {
                    //OdabraniIdentifikator.KupacId = this.Id;
                    // izvrsi unos novog identifikatora
                    //var noviIdentifikator = await App.IdentifikatoriRepository.Add(OdabraniIdentifikator);
                    //if (noviIdentifikator != null)
                    //{
                        ListaIdentifikatora.Add(new IdentifikatorViewModel(OdabraniIdentifikator));
                    //}
                }
            }
        }

        public async Task UkloniIdentifikatorAsync()
        {
            if (OdabraniIdentViewModel != null)
            {
                bool rezultat = await App.IdentifikatoriRepository.Remove(OdabraniIdentViewModel.Model);
                if (rezultat)
                {
                    ListaIdentifikatora.Remove(OdabraniIdentViewModel);
                    OdabraniIdentViewModel = null;
                }
            }
        }
        
        // *******************************************************************
        // AB_SuggestionChosen i AB_TextChanged dogadjaji krse pravila MVVM-a
        // Naknadno ovo prebaciti u view-behind klasu
        // *******************************************************************

        #region AutoSuggestBox metode

        public void AB_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (args.SelectedItem is GradViewModel)
                GradId = ((GradViewModel)args.SelectedItem).Model.Id;

            if (args.SelectedItem is DrzavaViewModel)
                DrzavaId = ((DrzavaViewModel)args.SelectedItem).Model.Id;
        }

        public void AB_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                dynamic prijedlozi = null;

                if (sender.Name == "grad_s")
                    prijedlozi = App.ViewModel.ListaGradova.Where(i => i.Naziv.IndexOf(SearchString_Grad, StringComparison.CurrentCultureIgnoreCase) >= 0).ToList();

                if (sender.Name == "drzava_s")
                    prijedlozi = App.ViewModel.ListaDrzava.Where(i => i.Naziv.IndexOf(SearchString_Drzava, StringComparison.CurrentCultureIgnoreCase) >= 0).ToList();

                if (prijedlozi.Count > 0)
                {
                    sender.ItemsSource = prijedlozi;
                }
                else
                {
                    sender.ItemsSource = null;
                }
            }
        }

        #endregion

        // *******************************************************************

        public event PropertyChangedEventHandler PropertyChanged;
        private void Set([CallerMemberName] string propName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
