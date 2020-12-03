using DbProvider.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TrackAndTrace.ViewModels
{
    public class ProizvodiViewModel : INotifyPropertyChanged
    {

        public ProizvodiViewModel(Proizvod proizvod = null)
        {
            Model = proizvod ?? new Proizvod();
            PlanPakProizvod = PlanPakProizvod ?? new PlanPakProizvod();
            IsNewProizvod = (proizvod != null) ? false : true;
            IsEditMode = (proizvod == null) ? false : true;
            Naslov = (IsNewProizvod) ? "Novi proizvod" : proizvod.Naziv;
        }

        private Proizvod proizvod;

        public Proizvod Model
        {
            get { return proizvod; }
            set
            {
                if (proizvod != value)
                {
                    proizvod = value;
                    Set(string.Empty);
                }
            }
        }

        public PlanPakProizvod PlanPakProizvod { get; private set; }

        public async Task UcitajPlanPakovanjaDefault()
        {
            var planPakDefault = await App.PlanPakDefaultRepository.GetByProductId(Model.Id);
            PlanPakProizvod = planPakDefault;
        }


        #region Atributi

        public int Id { get { return Model.Id; } }

        public string Naziv
        {
            get { return Model.Naziv; }
            set
            {
                if (Model.Naziv != value)
                {
                    Model.Naziv = value;
                    Set();
                }
            }
        }

        public string JedinicaMjere
        {
            get { return Model.JedinicaMjere; }
            set
            {
                if (Model.JedinicaMjere != value)
                {
                    Model.JedinicaMjere = value;
                    Set();
                }
            }
        }

        public double JedinacnihZaOmot
        {
            get { return PlanPakProizvod.JedinacnihZaOmot; }
            set
            {
                if (PlanPakProizvod.JedinacnihZaOmot != value)
                {
                    PlanPakProizvod.JedinacnihZaOmot = value;
                    Set();
                }
            }
        }

        public double OmotnihZaPaket
        {
            get { return PlanPakProizvod.OmotnihZaPaket; }
            set
            {
                if (PlanPakProizvod.OmotnihZaPaket != value)
                {
                    PlanPakProizvod.OmotnihZaPaket = value;
                    Set();
                }
            }
        }

        public double PaketaNaPaletu
        {
            get { return PlanPakProizvod.PaketaNaPaletu; }
            set
            {
                if (PlanPakProizvod.PaketaNaPaletu != value)
                {
                    PlanPakProizvod.PaketaNaPaletu = value;
                    Set();
                }
            }
        }

        #endregion

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

        private bool isNewProizvod;
        public bool IsNewProizvod
        {
            get { return isNewProizvod; }
            set { if (isNewProizvod != value) { isNewProizvod = value; Set(); } }
        }
        
        #endregion

        public delegate void OnSaveProizvod(bool rezultat);
        public event OnSaveProizvod OnSave;

        public async Task SnimiProizvodAsync()
        {
            // novi proizvod??
            if (IsNewProizvod)
            {
                IsNewProizvod = false;
                App.ViewModel.ListaProizvoda.Add(this);
                var noviProizvod = await App.ProizvodiRepository.Add(Model);
                if (noviProizvod != null)
                {
                    var proizvodId = noviProizvod.Id;
                    var planPakDefault = new PlanPakProizvod()
                    {
                        ProizvodId = proizvodId,
                        JedinacnihZaOmot = this.JedinacnihZaOmot,
                        OmotnihZaPaket = this.OmotnihZaPaket,
                        PaketaNaPaletu = this.PaketaNaPaletu
                    };
                    var noviPlanPakDefault = await App.PlanPakDefaultRepository.Add(planPakDefault);

                    if (noviPlanPakDefault != null)
                        OnSave(true);
                }                
            }

            // postojeci? 
            if (IsEditMode)
            {
                IsEditMode = false;
                var rezultat = await App.ProizvodiRepository.Update(Model);
                rezultat = await App.PlanPakDefaultRepository.Update(PlanPakProizvod);

                OnSave(rezultat);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void Set([CallerMemberName] string propName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
