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
using System.Windows.Input;
using TrackAndTrace.Helpers;
using Windows.UI.Xaml.Controls;

namespace TrackAndTrace.ViewModels
{
    public class UposlenikViewModel : INotifyPropertyChanged
    {

        public UposlenikViewModel(Uposlenik uposlenik = null)
        {
            Model = uposlenik ?? new Uposlenik();
            IsNewUserMode = (uposlenik != null) ? false : true;
            IsEditMode = (uposlenik == null) ? false : true;
            Naslov = (IsNewUserMode) ? "Novi uposlenik" : uposlenik.ImePrezime;
            SearchString_Grad = (uposlenik != null) ? uposlenik.Grad : "";
            SearchString_Drzava = (uposlenik != null) ? uposlenik.Drzava : "";
        }

        private Uposlenik _model;
        public Uposlenik Model
        {
            get { return _model; }
            set
            {
                if (_model != value)
                {
                    _model = value;
                    
                    Set(string.Empty);
                }
            }
        }

        #region Atributi

        public int RolaId
        {
            get { return Model.RolaId; }
            set
            {
                if (Model.RolaId != value)
                {
                    Model.RolaId = value;
                    Set();
                    Set(nameof(RolaId));
                }
            }
        }

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
        
        public string Rola
        {
            get { return Model.Rola;   }
        }
        
        public string Ime
        {
            get { return Model.Ime; }
            set
            {
                if (Model.Ime != value)
                {
                    Model.Ime = value;
                    Set();
                    Set(nameof(Ime));
                }
            }
        }

        public string ImeRoditelja
        {
            get { return Model.ImeRoditelja; }
            set
            {
                if (Model.ImeRoditelja != value)
                {
                    Model.ImeRoditelja = value;
                    Set();
                    Set(nameof(ImeRoditelja));
                }
            }
        }

        public string Prezime
        {
            get { return Model.Prezime; }
            set
            {
                if (Model.Prezime != value)
                {
                    Model.Prezime = value;
                    Set();
                    Set(nameof(Prezime));
                }
            }
        }

        public string Username
        {
            get { return Model.Username; }
            set
            {
                if (Model.Username != value)
                {
                    Model.Username = value;
                    Set();
                    Set(nameof(Username));
                }
            }
        }

        public string Password
        {
            get { return Model.Password; }
            set
            {
                if (Model.Password != value)
                {
                    Model.Password = value;
                    Set();
                    Set(nameof(Password));
                }
            }
        }

        public string Email
        {
            get { return Model.Email; }
            set
            {
                if (Model.Email != value)
                {
                    Model.Email = value;
                    Set();
                    Set(nameof(Email));
                }
            }
        }

        #endregion

        public delegate void OnSaveUser(bool rezultat);
        public event OnSaveUser OnSave;

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

        private bool isNewUserMode;
        public bool IsNewUserMode
        {
            get { return isNewUserMode; }
            set { if (isNewUserMode != value) { isNewUserMode = value; Set(); } }
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

        public async Task SnimiUposlenikaAsync()
        {
            // ako je novi dodaj uposlenika na listu
            if(IsNewUserMode)
            {
                // novi grad??
                if(GradId == 0)
                {
                    Grad noviGrad = new Grad()
                    {
                        Naziv = SearchString_Grad
                    };
                    var grad = await App.GradoviRepository.Add(noviGrad);
                    GradId = grad.Id;
                }

                // nova drzava??
                if (DrzavaId == 0)
                {
                    Drzava novaDrzava = new Drzava()
                    {
                        Naziv = SearchString_Drzava
                    };
                    var drzava = await App.DrzaveRepository.Add(novaDrzava);
                    DrzavaId = drzava.Id;
                }

                IsNewUserMode = false;
                Password = AuthViewModel.Instance.EncodePassword(Password);
                App.ViewModel.ListaUposlenika.Add(this);
                var noviUposlenik = await App.UposleniciRepository.Add(Model);
                if (noviUposlenik != null)
                    OnSave(true);
            }

            // postojeci? Izmjeni uspolenika
            if (IsEditMode)
            {
                IsEditMode = false;
                var rezultat = await App.UposleniciRepository.Update(Model);
                OnSave(rezultat);
            }
        }

        // *******************************************************************
        // AB_SuggestionChosen i AB_TextChanged dogadjaji krse pravila MVVM-a
        // Naknadno ovo prebaciti u view-behind klasu
        // *******************************************************************

        public async void AB_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (args.SelectedItem is GradViewModel)
                GradId = ((GradViewModel)args.SelectedItem).Model.Id;

            if (args.SelectedItem is DrzavaViewModel)
                DrzavaId = ((DrzavaViewModel)args.SelectedItem).Model.Id;

            // ako grad ne postoji smatramo da je novi i automatski ga unosimo u bazu
            if (args.SelectedItem == null && sender.Name == "grad_s")
            {
                await App.GradoviRepository.Add(new Grad() { Naziv = SearchString_Grad });
            }

            // isto vazi i za drzavu
            if (args.SelectedItem == null && sender.Name == "drzava_s")
            {
                await App.DrzaveRepository.Add(new Drzava() { Naziv = SearchString_Drzava });
            }
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
        // *******************************************************************
        
        public event PropertyChangedEventHandler PropertyChanged;
        private void Set([CallerMemberName]string PropName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropName));

    }
}
