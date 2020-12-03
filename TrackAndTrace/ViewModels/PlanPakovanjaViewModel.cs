using DbProvider.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace TrackAndTrace.ViewModels
{
    public class PlanPakovanjaViewModel : INotifyPropertyChanged
    {
        public PlanPakovanjaViewModel(PlanPakovanja planPakovanja = null)
        {
            Model = planPakovanja ?? new PlanPakovanja();
            IsNewPlanMode = (planPakovanja != null) ? false : true;
            IsEditMode = (planPakovanja == null) ? false : true;
            Naslov = (IsNewPlanMode) ? "Novi plan pakovanja" : "Broj plana: " + planPakovanja.Id.ToString();
            DatumIzradePlana = IsNewPlanMode ? DateTime.Now : Model.DatumIzradePlana;
            DatumZavrsetkaPlana = IsNewPlanMode ? DateTime.Now : Model.DatumZavrsetkaPlana;

            if (App.ViewModel.AutorizovaniKorisnik != null)
                PlanerId = App.ViewModel.AutorizovaniKorisnik.Model.Id;
        }


        private PlanPakovanja planPakovanja;
        public PlanPakovanja Model
        {
            get { return planPakovanja; }
            set
            {
                if (planPakovanja != value)
                {
                    planPakovanja = value;
                    Set(string.Empty);
                }
            }
        }


#region Atributi

        public int Id { get { return Model.Id; } }

        public int KupacId
        {
            get => Model.KupacId;
            set
            {
                if (Model.KupacId != value)
                {
                    Model.KupacId = value;
                    Set();
                    Set(nameof(KupacId));
                }
            }
        }

        public int ProizvodId
        {
            get => Model.ProizvodId;
            set
            {
                if (Model.ProizvodId != value)
                {
                    Model.ProizvodId = value;
                    Set();
                    Set(nameof(ProizvodId));
                }
            }
        }

        public int PlanerId
        {
            get => Model.PlanerId;
            set
            {
                if (Model.PlanerId != value)
                {
                    Model.PlanerId = value;
                    Set();
                    Set(nameof(PlanerId));
                }
            }
        }

        public DateTime DatumIzradePlana
        {
            get => Model.DatumIzradePlana;
            set
            {
                if (Model.DatumIzradePlana != value)
                {
                    Model.DatumIzradePlana = value;
                    Set();
                    Set(nameof(DatumIzradePlana));
                }
            }
        }

        public DateTime DatumZavrsetkaPlana
        {
            get => Model.DatumZavrsetkaPlana;
            set
            {
                if (Model.DatumZavrsetkaPlana != value)
                {
                    Model.DatumZavrsetkaPlana = value;
                    Set();
                    Set(nameof(DatumZavrsetkaPlana));
                }
            }
        }

        public int StatusPlana
        {
            get => Model.StatusPlana;
            set
            {
                if (Model.StatusPlana != value)
                {
                    Model.StatusPlana = value;
                    Set();
                    Set(nameof(StatusPlana));
                }
            }
        }

        public string JeAktivan
        {
            get => Model.JeAktivan;
        }

        public double Kolicina
        {
            get => Model.Kolicina;
            set
            {
                if (Model.Kolicina != value)
                {
                    Model.Kolicina = value;
                    Set();
                    Set(nameof(Kolicina));
                    Recalculate();
                }
            }
        }

        public double JedinacnihUOmot
        {
            get => Model.JedinacnihUOmot;
            set
            {
                if (Model.JedinacnihUOmot != value)
                {
                    Model.JedinacnihUOmot = value;
                    Set();
                    Set(nameof(JedinacnihUOmot));
                    Recalculate();
                }
            }
        }

        public double OmotnihUPaket
        {
            get => Model.OmotnihUPaket;
            set
            {
                if (Model.OmotnihUPaket != value)
                {
                    Model.OmotnihUPaket = value;
                    Set();
                    Set(nameof(OmotnihUPaket));
                    Recalculate();
                }
            }
        }

        public double PaketaNaPaletu
        {
            get => Model.PaketaNaPaletu;
            set
            {
                if (Model.PaketaNaPaletu != value)
                {
                    Model.PaketaNaPaletu = value;
                    Set();
                    Set(nameof(PaketaNaPaletu));
                    Recalculate();
                }
            }
        }
               
        public double UkupnoOmotnih
        {
            get => Model.UkupnoOmotnih;
            set
            {
                if (Model.UkupnoOmotnih != value)
                {
                    Model.UkupnoOmotnih = value;
                    Set();
                    Set(nameof(UkupnoOmotnih));
                }
            }
        }

        public double UkupnoPaketa
        {
            get => Model.UkupnoPaketa;
            set
            {
                if (Model.UkupnoPaketa != value)
                {
                    Model.UkupnoPaketa = value;
                    Set();
                    Set(nameof(UkupnoPaketa));
                }
            }
        }

        public double UkupnoPaleta
        {
            get => Model.UkupnoPaleta;
            set
            {
                if (Model.UkupnoPaleta != value)
                {
                    Model.UkupnoPaleta = value;
                    Set();
                    Set(nameof(UkupnoPaleta));
                }
            }
        }

        public string Kupac { get { return Model.NazivKupca; } }

        public string Proizvod { get { return Model.NazivProizvoda; } }

        public string NazivProizvoda { get { return Model.NazivProizvoda; } }

        public string Planer { get { return Model.Planer; } }

        public int TrajanjePlana { get { return Model.TrajanjePlanaUDanima; } }

        public string NazivPlana
        {
            get
            {
                var nazivPlana = Model.Id + "-" + Model.NazivProizvoda + "-" + Model.NazivKupca;
                return nazivPlana;
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

        private bool isNewPlanMode;


        public bool IsNewPlanMode
        {
            get { return isNewPlanMode; }
            set { if (isNewPlanMode != value) { isNewPlanMode = value; Set(); } }
        }

#endregion
        
        public async Task UcitajProizvodPlanPak()
        {
            // samo ako se radi o novom planu pakovanja povuci inicijelne podatke o kolicinama za omote i pakete
            if (IsNewPlanMode)
            {
                var planPakProizvod = await App.PlanPakDefaultRepository.GetByProductId(ProizvodId);
                JedinacnihUOmot = planPakProizvod.JedinacnihZaOmot;
                OmotnihUPaket = planPakProizvod.OmotnihZaPaket;
                PaketaNaPaletu = planPakProizvod.PaketaNaPaletu;
            }
        }

        public async Task SnimiPlanAsync()
        {
            // ako je novi dodaj !
            if (IsNewPlanMode)
            {
                IsNewPlanMode = false;
                App.ViewModel.ListaPlanova.Add(this);
                var planPakovanja = await App.PlanPakovanjaRepository.Add(Model);
                if (planPakovanja != null)
                    OnSave(true);
            }

            // postojeci? Izmjeni!
            if (IsEditMode)
            {
                IsEditMode = false;
                var rezultat = await App.PlanPakovanjaRepository.Update(Model);
                OnSave(rezultat);
            }
        }
        
        private void Recalculate()
        {
            var kolicina = Kolicina;
            var jed = JedinacnihUOmot;
            var omo = OmotnihUPaket;
            var pak = PaketaNaPaletu;
            var totalO = Math.Ceiling(kolicina / jed);
            var totalP = Math.Ceiling(totalO / omo);
            var totalPl = Math.Ceiling(totalP / pak);
            UkupnoOmotnih = totalO;
            UkupnoPaketa = totalP;
            UkupnoPaleta = totalPl;
        }
        
        public delegate void OnSavePlan(bool rezultat);
        public event OnSavePlan OnSave;

        public event PropertyChangedEventHandler PropertyChanged;
        private void Set([CallerMemberName] string propName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
