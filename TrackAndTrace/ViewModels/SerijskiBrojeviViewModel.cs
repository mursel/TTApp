using DbProvider.DAL;
using DbProvider.Model;
using FastMember;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ZXing;
using ZXing.Datamatrix;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using ZXing.Mobile;
using System.IO;

namespace TrackAndTrace.ViewModels
{
    public class SerijskiBrojeviViewModel : INotifyPropertyChanged
    {

        private string BrojKupca = string.Empty;
        private string DatumKreiranja = string.Empty;
        private string OznakaTipa = string.Empty;

        private int PadSize = 0;

        BarcodeWriter writer = null;

        public SerijskiBrojeviViewModel(PlanPakovanjaViewModel planPakovanjaViewModel = null)   // suvisno = null?! Plan mora postojati!
        {
            Model = planPakovanjaViewModel ?? new PlanPakovanjaViewModel();                     // suvisno = null?! Plan mora postojati!

            IsSaveEnabled = false;

            ListaSerijskih = new ObservableCollection<dynamic>();
            ListaSerijskihRaw = new List<dynamic>();
            
            BrojKupca = App.ViewModel.ListaKupaca.Where(k => k.Id == Model.KupacId).First().BrojProizvodnogMjesta;
            PadSize = Model.Kolicina.ToString().Length;
            DatumKreiranja = DateTime.Now.ToString("ddMMyyyy");

            writer = new BarcodeWriter();
            writer.Format = BarcodeFormat.DATA_MATRIX;
            writer.Options = new DatamatrixEncodingOptions
            {
                SymbolShape = ZXing.Datamatrix.Encoder.SymbolShapeHint.FORCE_SQUARE,
                Height = 96,    // 41px cca 1,1 cm
                Width = 96,
                MaxSize = new Dimension(64, 64),
                MinSize = new Dimension(32, 32),
                GS1Format = true
            };
        }

        public async Task<bool> UcitajJedinacne(bool jeSkenirana, bool jeStampana, int odabranaKolicina = 0)
        {
            if (odabranaKolicina == 0) odabranaKolicina = (int)planPakovanjaViewModel.JedinacnihUOmot;

            var jedinacne = await App.SbRepository.GetJedinacneAsync(jeSkenirana, jeStampana, odabranaKolicina, planPakovanjaViewModel.Id);

            if (jedinacne != null)
            {
                ListaSerijskih.Clear();
                ListaSerijskihRaw.Clear();

                foreach (var item in jedinacne)
                {
                    tipSerijskog = new JedinacnaViewModel(item);
                    ListaSerijskih.Add(tipSerijskog);
                    ListaSerijskihRaw.Add(tipSerijskog.Model);
                    item.JeRezervisana = 1;
                }
                await App.SbRepository.UpdateJedinacneAsync(jedinacne);
                return true;
            }
            return false;
        }

        public async Task<bool> UcitajOmotne(bool jeSkenirana, bool jeStampana, int odabranaKolicina = 0)
        {
            if (odabranaKolicina == 0) odabranaKolicina = (int)planPakovanjaViewModel.OmotnihUPaket;

            var omotne = await App.SbRepository.GetOmotneAsync(jeSkenirana, jeStampana, odabranaKolicina, planPakovanjaViewModel.Id);

            if (omotne != null)
            {
                ListaSerijskih.Clear();
                ListaSerijskihRaw.Clear();

                foreach (var item in omotne)
                {
                    tipSerijskog = new OmotnaViewModel(item);
                    ListaSerijskih.Add(tipSerijskog);
                    ListaSerijskihRaw.Add(tipSerijskog.Model);
                    item.JeRezervisana = 1;
                }
                await App.SbRepository.UpdateOmotneAsync(omotne);
                return true;
            }
            return false;
        }

        public async Task<bool> UcitajPaketne(bool jeSkenirana, bool jeStampana, int odabranaKolicina = 0)
        {
            if (odabranaKolicina == 0) odabranaKolicina = (int)planPakovanjaViewModel.PaketaNaPaletu;

            var paketne = await App.SbRepository.GetPaketneAsync(jeSkenirana, jeStampana, odabranaKolicina, planPakovanjaViewModel.Id);

            if (paketne != null)
            {
                ListaSerijskih.Clear();
                ListaSerijskihRaw.Clear();

                foreach (var item in paketne)
                {
                    tipSerijskog = new PaketnaViewModel(item);
                    ListaSerijskih.Add(tipSerijskog);
                    ListaSerijskihRaw.Add(tipSerijskog.Model);
                    item.JeRezervisana = 1;
                }
                await App.SbRepository.UpdatePaketneAsync(paketne);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Enumeracija pakovanja po nivoima (EU direktiva)
        /// </summary>
        public enum TipSerijskogBroja
        {
            Item = 0,                   // jedinacna
            InnerPackaging = 1,         // omotna
            IntermediatePackaging = 2,  
            OuterPackaging = 3,         // paketna
            Pallet = 4,                 
            Container = 5,
            AdHoc = 9                   // paletna - mjesovita
        }

        public TipSerijskogBroja OdabraniTip { get; set; }

        private PlanPakovanjaViewModel planPakovanjaViewModel;
        public PlanPakovanjaViewModel Model
        {
            get { return planPakovanjaViewModel; }
            set
            {
                if (planPakovanjaViewModel != value)
                {
                    planPakovanjaViewModel = value;
                    Set(string.Empty);
                }
            }
        }

        public async Task<bool> PostaviRangoveBrojevaAsync()
        {
            switch (OdabraniTip)
            {
                case TipSerijskogBroja.Item:
                    {
                        // kreirati nedostajuci niz kao observable ili nesto drugo za odBroja doBroja
                        var jedinacne = await App.SbRepository.GetJedinacneAsync(planPakovanjaViewModel.Id);
                        var ukupniBrojevi = Enumerable.Range(1, (int)Model.Kolicina);
                        var postojeciBrojevi = jedinacne.Select(j => j.SerijskiBrojInt).ToList();
                        var nedostajuciBrojevi = ukupniBrojevi.Except(postojeciBrojevi);
                        NedostajuciBrojevi = new ObservableCollection<int>(nedostajuciBrojevi);
                    }
                    break;
                case TipSerijskogBroja.InnerPackaging:
                    {
                        var omotne = await App.SbRepository.GetOmotneAsync(planPakovanjaViewModel.Id);
                        var ukupniBrojevi = Enumerable.Range(1, (int)Model.UkupnoOmotnih);
                        var postojeciBrojevi = omotne.Select(j => j.SerijskiBrojInt).ToList();
                        var nedostajuciBrojevi = ukupniBrojevi.Except(postojeciBrojevi);
                        NedostajuciBrojevi = new ObservableCollection<int>(nedostajuciBrojevi);
                    }
                    break;
                case TipSerijskogBroja.IntermediatePackaging:
                    break;
                case TipSerijskogBroja.OuterPackaging:
                    {
                        var paketne = await App.SbRepository.GetPaketneAsync(planPakovanjaViewModel.Id);
                        var ukupniBrojevi = Enumerable.Range(1, (int)Model.UkupnoPaketa);
                        var postojeciBrojevi = paketne.Select(j => j.SerijskiBrojInt).ToList();
                        var nedostajuciBrojevi = ukupniBrojevi.Except(postojeciBrojevi);
                        NedostajuciBrojevi = new ObservableCollection<int>(nedostajuciBrojevi);
                    }
                    break;
                case TipSerijskogBroja.Pallet:
                    {
                        var paletne = await App.SbRepository.GetPaletneAsync(planPakovanjaViewModel.Id);
                        var ukupniBrojevi = Enumerable.Range(1, (int)Model.UkupnoPaleta);
                        var postojeciBrojevi = paletne.Select(j => j.SerijskiBrojInt).ToList();
                        var nedostajuciBrojevi = ukupniBrojevi.Except(postojeciBrojevi);
                        NedostajuciBrojevi = new ObservableCollection<int>(nedostajuciBrojevi);
                    }
                    break;
                case TipSerijskogBroja.Container:
                    break;
                case TipSerijskogBroja.AdHoc:
                    break;
                default:
                    return false;
            }
            return true;
        }

        public async Task<OmotnaViewModel> AddNewInnerPackageByItemsList()
        {
            var zadnjaOmotna = await App.SbRepository.GetLastInnerPackageByPlanIdAsync(planPakovanjaViewModel.Id);

            var nextIndex = (zadnjaOmotna != null) ? (zadnjaOmotna.SerijskiBrojInt + 1) : 1;

            OmotnaViewModel omotnaViewModel = null;

            var omot = new Omotna()
            {
                DatumPrintanja = DateTime.Now,
                DatumSkeniranja = DateTime.Now,
                JeSkenirana = 0,
                JeStampana = 1,
                KupacId = App.ViewModel.OdabraniPlan.KupacId,
                PlanPakovanjaId = App.ViewModel.OdabraniPlan.Id,
                ProizvodId = App.ViewModel.OdabraniPlan.ProizvodId,
                UposlenikId = App.ViewModel.AutorizovaniKorisnik.Model.Id,
                SerijskiBroj = "(90)" + BrojKupca + "(250)SN01" + DatumKreiranja + nextIndex.ToString("D" + PadSize),
                SerijskiBrojInt = nextIndex,
                Code = await GetDMCode("90" + BrojKupca + "250SN01" + DatumKreiranja + nextIndex.ToString("D" + PadSize))
            };

            //var task = Task.Run(()=>App.SbRepository.AddNewInnerPackageAsync(omot));
            //await task.ContinueWith(async t1 =>
            //{
            //    if (t1.Status == TaskStatus.RanToCompletion)
            //    {
            //        await VeziJedinacneZaOmotnu(t1.Result.Id);
            //        omotnaViewModel = new OmotnaViewModel(t1.Result);

            //    }
            //});

            var omotna = await App.SbRepository.AddNewInnerPackageAsync(omot);
            var result = await VeziJedinacneZaOmotnu(omotna.Id);
            if (result)
                omotnaViewModel = new OmotnaViewModel(omotna);

            if (omotnaViewModel != null)
                return omotnaViewModel;

            return null;
        }

        public async Task<PaketnaViewModel> AddNewOuterPackageByItemsList()
        {
            var zadnjaPaketna = await App.SbRepository.GetLastOuterPackageByPlanIdAsync(planPakovanjaViewModel.Id);

            var nextIndex = (zadnjaPaketna != null) ? (zadnjaPaketna.SerijskiBrojInt + 1) : 1;

            PaketnaViewModel paketnaViewModel = null;

            var paket = new Paketna()
            {
                DatumPrintanja = DateTime.Now,
                DatumSkeniranja = DateTime.Now,
                JeSkenirana = 0,
                JeStampana = 1,
                KupacId = App.ViewModel.OdabraniPlan.KupacId,
                PlanPakovanjaId = App.ViewModel.OdabraniPlan.Id,
                ProizvodId = App.ViewModel.OdabraniPlan.ProizvodId,
                UposlenikId = App.ViewModel.AutorizovaniKorisnik.Model.Id,
                SerijskiBroj = "(90)" + BrojKupca + "(250)SN03" + DatumKreiranja + nextIndex.ToString("D" + PadSize),
                SerijskiBrojInt = nextIndex,
                Code = await GetDMCode("90" + BrojKupca + "250SN03" + DatumKreiranja + nextIndex.ToString("D" + PadSize))
            };

            //var task = Task.Run(()=>App.SbRepository.AddNewInnerPackageAsync(omot));
            //await task.ContinueWith(async t1 =>
            //{
            //    if (t1.Status == TaskStatus.RanToCompletion)
            //    {
            //        await VeziJedinacneZaOmotnu(t1.Result.Id);
            //        omotnaViewModel = new OmotnaViewModel(t1.Result);

            //    }
            //});

            var paketna = await App.SbRepository.AddNewOuterPackageAsync(paket);
            var result = await VeziOmotneZaPaketnu(paketna.PaketnaId);
            if (result)
                paketnaViewModel = new PaketnaViewModel(paketna);

            if (paketnaViewModel != null)
                return paketnaViewModel;

            return null;
        }

        public async Task<PaletnaViewModel> AddNewPalletByItemsList()
        {
            var zadnjaPaletna = await App.SbRepository.GetLastPalletByPlanIdAsync(planPakovanjaViewModel.Id);

            var nextIndex = (zadnjaPaletna != null) ? (zadnjaPaletna.SerijskiBrojInt + 1) : 1;

            PaletnaViewModel paletnaViewModel = null;

            var paleta = new Paletna()
            {
                DatumPrintanja = DateTime.Now,
                DatumSkeniranja = DateTime.Now,
                JeSkenirana = 0,
                JeStampana = 1,
                KupacId = App.ViewModel.OdabraniPlan.KupacId,
                PlanPakovanjaId = App.ViewModel.OdabraniPlan.Id,
                ProizvodId = App.ViewModel.OdabraniPlan.ProizvodId,
                UposlenikId = App.ViewModel.AutorizovaniKorisnik.Model.Id,
                SerijskiBroj = "(90)" + BrojKupca + "(250)SN04" + DatumKreiranja + nextIndex.ToString("D" + PadSize),
                SerijskiBrojInt = nextIndex,
                Code = await GetDMCode("90" + BrojKupca + "250SN04" + DatumKreiranja + nextIndex.ToString("D" + PadSize))
            };

            //var task = Task.Run(()=>App.SbRepository.AddNewInnerPackageAsync(omot));
            //await task.ContinueWith(async t1 =>
            //{
            //    if (t1.Status == TaskStatus.RanToCompletion)
            //    {
            //        await VeziJedinacneZaOmotnu(t1.Result.Id);
            //        omotnaViewModel = new OmotnaViewModel(t1.Result);

            //    }
            //});

            var paletna = await App.SbRepository.AddNewPallet(paleta);
            var result = await VeziPaketneZaPaletnu(paletna.Id);
            if (result)
                paletnaViewModel = new PaletnaViewModel(paletna);

            if (paletnaViewModel != null)
                return paletnaViewModel;

            return null;
        }

        public async Task<bool> DodajPaketUPaletu(PaketnaViewModel izabraniPaket, object izabranaPaleta)
        {
            var task = Task.Run(()=> App.SbRepository.GetPaletnaByNameAsync(izabranaPaleta.ToString()));
            await task.ContinueWith(async t1 =>
            {
                if (t1.Status == TaskStatus.RanToCompletion)
                {
                    izabraniPaket.Model.PaletnaId = t1.Result.Id;
                    var rezultat = await App.SbRepository.UpdatePaketnaAsync(izabraniPaket.Model);
                    if (!rezultat) return false;
                }
                return true;
            });

            return false;
        }

        /// <summary>
        /// Ucitaj paketne serijske brojeve kojima nije dodijeljena paleta
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UcitajPaketeBezPalete()
        {
            var serijski = await App.SbRepository.GetPaketneAsync(planPakovanjaViewModel.Id);
            var listaSerijskih = serijski.Where(p => p.PaletnaId == 0);
            foreach (var item in listaSerijskih)
            {
                tipSerijskog = new PaketnaViewModel(item);
                ListaSerijskih.Add(tipSerijskog);
                ListaSerijskihRaw.Add(tipSerijskog.Model);
            }
            return true;
        }


        #region Atributi

        private string _saveStatus;

        public string SaveStatus
        {
            get { return _saveStatus; }
            set { if (_saveStatus != value) {
                    _saveStatus = value; Set(); } }
        }


        private int odBroja;
        public int OdBroja
        {
            get { return odBroja; }
            set
            {
                if (odBroja != value)
                {
                    odBroja = value;
                    Set();
                }
            }
        }

        private int doBroja;
        public int DoBroja
        {
            get { return doBroja; }
            set
            {
                if (doBroja != value)
                {
                    doBroja = value;
                    Set();
                }
            }
        }

        public double OdabranaKolicina {
            get
            {
                if (OdabraniTip== TipSerijskogBroja.Item)
                    return Model.Kolicina;
                if (OdabraniTip == TipSerijskogBroja.InnerPackaging)
                    return Model.UkupnoOmotnih;
                if (OdabraniTip == TipSerijskogBroja.OuterPackaging)
                    return Model.UkupnoPaketa;
                if (OdabraniTip == TipSerijskogBroja.Pallet)
                    return Model.UkupnoPaleta;
                return 0;
            }
        }



        private bool _isSaveEnabled = false;
        public bool IsSaveEnabled
        {
            get { return _isSaveEnabled; }
            set { _isSaveEnabled = value; Set(); }
        }

        #endregion

        public List<dynamic> ListaSerijskihRaw { get; set; }
        private List<Identifikator> Identifikatori { get; set; }

        public ObservableCollection<int> _nedostajuciBrojevi;
        public ObservableCollection<int> NedostajuciBrojevi {
            get { return _nedostajuciBrojevi; }
            set
            {
                if (_nedostajuciBrojevi != value)
                {
                    _nedostajuciBrojevi = value;
                    Set();
                }
            }
        }


        private ObservableCollection<dynamic> serijski;
        public ObservableCollection<dynamic> ListaSerijskih
        {
            get => serijski;
            set
            {
                if (serijski != value)
                {
                    serijski = value;
                    Set();
                }
            }
        }

        dynamic tipSerijskog;
                
        public bool GenerirajAsync(int start, int kraj)
        {
            if (kraj == 0)
                kraj = (int)OdabranaKolicina;

            if (start > 1)
            {
                ListaSerijskih.Clear();
                ListaSerijskihRaw.Clear();
            }else if (start == 0)
                start = 1;

            for (int i = start; i < kraj + 1; i++)
            {
                switch (OdabraniTip)
                {
                    case TipSerijskogBroja.Item:
                        tipSerijskog = new JedinacnaViewModel(); OznakaTipa = "SN00";
                        break;
                    case TipSerijskogBroja.InnerPackaging:
                        tipSerijskog = new OmotnaViewModel(); OznakaTipa = "SN01";
                        break;
                    case TipSerijskogBroja.IntermediatePackaging:
                        break;
                    case TipSerijskogBroja.OuterPackaging:
                        tipSerijskog = new PaketnaViewModel(); OznakaTipa = "SN03";
                        break;
                    case TipSerijskogBroja.Pallet:
                        tipSerijskog = new PaletnaViewModel(jeAdHoc: false); OznakaTipa = "SN04";
                        break;
                    case TipSerijskogBroja.Container:
                        break;
                    case TipSerijskogBroja.AdHoc:
                        tipSerijskog = new PaletnaViewModel(jeAdHoc: true); OznakaTipa = "SN09";
                        break;
                }

                tipSerijskog.SerijskiBroj = "(90)" + BrojKupca + "(250)" + OznakaTipa + DatumKreiranja + i.ToString("D" + PadSize);
                tipSerijskog.DatumPrintanja = DateTime.Now;
                tipSerijskog.DatumSkeniranja = DateTime.Now;
#if DEBUG
                tipSerijskog.Model.UposlenikId = 1;
#endif
                tipSerijskog.Model.ProizvodId = App.ViewModel.OdabraniPlan.ProizvodId;
                tipSerijskog.Model.KupacId = App.ViewModel.OdabraniPlan.KupacId;
                tipSerijskog.Model.PlanPakovanjaId = planPakovanjaViewModel.Id;
                tipSerijskog.Model.SerijskiBrojInt = i;
                tipSerijskog.Model.UposlenikId = App.ViewModel.AutorizovaniKorisnik.Model.Id;

                try
                {
                    ListaSerijskih.Add(tipSerijskog);
                    ListaSerijskihRaw.Add(tipSerijskog.Model);
                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                }
            }

            IsSaveEnabled = true;
            

            return true;
        }

        private IEnumerable<Jedinacna> jedinacne;
        private IEnumerable<Omotna> omotne;
        private IEnumerable<Paketna> paketne;
        private IEnumerable<Paletna> paletne;

        public async Task<bool> VeziJedinacneZaOmotnu(int omotnaId)
        {
            jedinacne = ListaSerijskihRaw.Cast<Jedinacna>();

            try
            {
                    foreach (var item in jedinacne)
                    {
                        item.OmotnaId = omotnaId;
                    }
                    await App.SbRepository.UpdateJedinacneAsync(jedinacne);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return false;
            }

            return true;
        }

        public async Task<bool> VeziOmotneZaPaketnu(int paketnaId)
        {
            omotne = ListaSerijskihRaw.Cast<Omotna>();

            try
            {
                foreach (var item in omotne)
                {
                    item.PaketnaId = paketnaId;
                }
                await App.SbRepository.UpdateOmotneAsync(omotne);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return false;
            }

            return true;
        }

        public async Task<bool> VeziPaketneZaPaletnu(int paletnaId)
        {
            paketne = ListaSerijskihRaw.Cast<Paketna>();

            try
            {
                foreach (var item in paketne)
                {
                    item.PaketnaId = paletnaId;
                }
                await App.SbRepository.UpdatePaketneAsync(paketne);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return false;
            }

            return true;
        }

        public async Task<bool> VeziJedinacneZaOmotne()
        {
            if (OdabraniTip == TipSerijskogBroja.InnerPackaging)
            {
                // vrati omotne koje su pohranjene u bazi podataka po planu pakovanja
                omotne = await App.SbRepository.GetOmotneAsync(planPakovanjaViewModel.Id);

                // vrati jedinacne koje su pohranjene u bazi podataka po planu pakovanja
                jedinacne = await App.SbRepository.GetJedinacneAsync(planPakovanjaViewModel.Id);

                int step = 0;
                
                var kolicina = (int)planPakovanjaViewModel.JedinacnihUOmot;

                try
                {
                    foreach (var omotna in omotne)
                    {
                        var partList = jedinacne.Skip(kolicina * step).Take(kolicina);
                        foreach (var item in partList)
                        {
                            item.OmotnaId = omotna.Id;
                        }
                        await App.SbRepository.UpdateJedinacneAsync(partList);
                        step++;
                    }
                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                }


                return true;
            }

            return false;
        }

        public async Task<bool> VeziOmotneZaPaketne()
        {
            if (OdabraniTip == TipSerijskogBroja.OuterPackaging)
            {
                // vrati paketne koje su pohranjene u bazi podataka po planu pakovanja
                paketne = await App.SbRepository.GetPaketneAsync(planPakovanjaViewModel.Id);

                // vrati omotne koje su pohranjene u bazi podataka po planu pakovanja
                omotne = await App.SbRepository.GetOmotneAsync(planPakovanjaViewModel.Id);

                int step = 0;

                var kolicina = (int)planPakovanjaViewModel.OmotnihUPaket;

                try
                {
                    foreach (var paketna in paketne)
                    {
                        var partList = omotne.Skip(kolicina * step).Take(kolicina);
                        foreach (var item in partList)
                        {
                            item.PaketnaId = paketna.PaketnaId;
                        }
                        Task t = Task.Run(() => App.SbRepository.UpdateOmotneAsync(partList));
                        await t.ContinueWith(t1 => step++);                        
                    }
                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                }

                return true;
            }

            return false;
        }

        public async Task<bool> VeziPaketneZaPaletne()
        {
            if (OdabraniTip == TipSerijskogBroja.Pallet)
            {
                // vrati paletne koje su pohranjene u bazi podataka po planu pakovanja
                paletne = await App.SbRepository.GetPaletneAsync(planPakovanjaViewModel.Id);

                // vrati paketne koje su pohranjene u bazi podataka po planu pakovanja
                paketne = await App.SbRepository.GetPaketneAsync(planPakovanjaViewModel.Id);

                int step = 0;

                var kolicina = (int)planPakovanjaViewModel.PaketaNaPaletu;

                try
                {
                    foreach (var paletna in paletne)
                    {
                        var partList = paketne.Skip(kolicina * step).Take(kolicina);
                        foreach (var item in partList)
                        {
                            item.PaletnaId = paletna.Id;
                        }
                        Task t = Task.Run(() => App.SbRepository.UpdatePaketneAsync(partList));
                        await t.ContinueWith(t1 => step++);
                    }
                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                }

                return true;
            }

            return false;
        }

        public async Task<bool> SnimiSerijskeAsync()
        {
            switch (OdabraniTip)
            {
                case TipSerijskogBroja.Item:
                        return await App.SbRepository.AddBulkJedinacneAsync(ListaSerijskihRaw.Cast<Jedinacna>().ToList().Where(i=>i.OmotnaId == 0));
                case TipSerijskogBroja.InnerPackaging:
                        return await App.SbRepository.AddBulkOmotneAsync(ListaSerijskihRaw.Cast<Omotna>().ToList().Where(i => i.PaketnaId == 0));
                case TipSerijskogBroja.IntermediatePackaging:
                    break;
                case TipSerijskogBroja.OuterPackaging:
                    return await App.SbRepository.AddBulkPaketneAsync(ListaSerijskihRaw.Cast<Paketna>().ToList().Where(i => i.PaletnaId == 0));
                case TipSerijskogBroja.Pallet:
                    return await App.SbRepository.AddBulkPaletneAsync(ListaSerijskihRaw.Cast<Paletna>().ToList());
                case TipSerijskogBroja.Container:
                    break;
                case TipSerijskogBroja.AdHoc:
                    return await App.SbRepository.AddBulkPaletneAsync(ListaSerijskihRaw.Cast<Paletna>().ToList());
            }

            return false;
        }
                
        public async Task<bool> UcitajPostojeceSerijske(bool jePostojeci = false)
        {
            switch (OdabraniTip)
            {
                case TipSerijskogBroja.Item:
                    {
                        var serijski = await App.SbRepository.GetJedinacneAsync(planPakovanjaViewModel.Id);
                        foreach (var item in serijski)
                        {
                            tipSerijskog = new JedinacnaViewModel(item);
                            ListaSerijskih.Add(tipSerijskog);
                            ListaSerijskihRaw.Add(tipSerijskog.Model);
                        }
                        return true;
                    }
                case TipSerijskogBroja.InnerPackaging:
                    {
                        var serijski = await App.SbRepository.GetOmotneAsync(planPakovanjaViewModel.Id);
                        foreach (var item in serijski)
                        {
                            tipSerijskog = new OmotnaViewModel(item);
                            ListaSerijskih.Add(tipSerijskog);
                            ListaSerijskihRaw.Add(tipSerijskog.Model);
                        }
                        return true;
                    }
                case TipSerijskogBroja.IntermediatePackaging:
                    break;
                case TipSerijskogBroja.OuterPackaging:
                    {
                        var serijski = await App.SbRepository.GetPaketneAsync(planPakovanjaViewModel.Id);
                        foreach (var item in serijski)
                        {
                            tipSerijskog = new PaketnaViewModel(item);
                            ListaSerijskih.Add(tipSerijskog);
                            ListaSerijskihRaw.Add(tipSerijskog.Model);
                        }
                        return true;
                    }
                case TipSerijskogBroja.Pallet:
                    {
                        var serijski = await App.SbRepository.GetPaletneAsync(planPakovanjaViewModel.Id);
                        foreach (var item in serijski)
                        {
                            tipSerijskog = new PaletnaViewModel(item);
                            ListaSerijskih.Add(tipSerijskog);
                            ListaSerijskihRaw.Add(tipSerijskog.Model);
                        }
                        return true;
                    }
                case TipSerijskogBroja.Container:
                    break;
                case TipSerijskogBroja.AdHoc:
                    {
                        var serijski = await App.SbRepository.GetPaletneAsync(planPakovanjaViewModel.Id, true);
                        foreach (var item in serijski)
                        {
                            tipSerijskog = new PaletnaViewModel(item);
                            ListaSerijskih.Add(tipSerijskog);
                            ListaSerijskihRaw.Add(tipSerijskog.Model);
                        }
                        return true;
                    }
            }
            return false;
        }

        public async void UcitajIdentifikatore()
        {
            var identifikators = await App.IdentifikatoriRepository.GetList(k => k.KupacId == Model.KupacId);
            Identifikatori = identifikators.ToList();            
        }

        public async Task<string> AddNewPallet()
        {
            OznakaTipa = "SN04";

            int i = await SlijedeciSerijskiBrojPalete();

            var novaPaletna = new Paletna()
            {
                DatumPrintanja = DateTime.Now,
                DatumSkeniranja = DateTime.Now,
                JeAdHoc = 0,
                JeSkenirana = 0,
                JeStampana = 0,
                KupacId = Model.KupacId,
                ProizvodId = Model.ProizvodId,
                SerijskiBroj = "(90)" + BrojKupca + "(250)" + OznakaTipa + DatumKreiranja + i.ToString("D" + PadSize),
                SerijskiBrojInt = i,
                PlanPakovanjaId = Model.Id,
                UposlenikId = App.ViewModel.AutorizovaniKorisnik.Model.Id
            };

            var result = await App.SbRepository.AddNewPallet(novaPaletna);
            if (result != null)
                return result.SerijskiBroj;

            return string.Empty;
        }

        public async Task<IEnumerable<PaletnaViewModel>> VratiPaletneSaPaketima()
        {
            List<PaletnaViewModel> palete = new List<PaletnaViewModel>();

            var paletne = await App.SbRepository.GetPaletneAndPaketneAsync(Model.Id);

            if (paletne != null)
            {
                foreach (var item in paletne)
                {
                    palete.Add(new PaletnaViewModel(item));
                }
            }
            else
            {
                paletne = await App.SbRepository.GetPaletneAsync(Model.Id);

                if (paletne != null)
                {
                    foreach (var item in paletne)
                    {
                        palete.Add(new PaletnaViewModel(item));
                    }
                }
            }

            return palete;
        }

        private async Task<int> SlijedeciSerijskiBrojPalete()
        {
            int broj = 0;
            var value = await App.SbRepository.GetPaletneAsync(Model.Id);
            if (value.Count() > 0)
            {
                var slijedBroj = value.OrderBy(o => o.SerijskiBroj).Last().SerijskiBroj;
                int m = slijedBroj.IndexOf("SN04");     // <== obavezno ukloniti ovu nebulozu od hardcodinga cim stignem!!!
                var datum = slijedBroj.Substring(m + 4, 8);
                var zadnji = slijedBroj.Substring(m + 4 + 8, slijedBroj.Length - (m + 4 + 8));
                broj = int.Parse(zadnji) + 1;
            }
            return broj;
        }

        private async Task<byte[]> GetDMCode(string serBroj)
        {
            WriteableBitmap bitmap = writer.Write(serBroj);
            byte[] bytes;

            using (InMemoryRandomAccessStream ms = new InMemoryRandomAccessStream())
            {
                BitmapEncoder bitmapEncoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, ms);
                bitmapEncoder.SetSoftwareBitmap(SoftwareBitmap.CreateCopyFromBuffer(
                    bitmap.PixelBuffer, BitmapPixelFormat.Bgra8, bitmap.PixelWidth, bitmap.PixelHeight));
                await bitmapEncoder.FlushAsync();
                bitmap = null;
                bytes = new byte[ms.Size];
                await ms.AsStream().ReadAsync(bytes, 0, bytes.Length);
            }
            return bytes;
        }


        //public delegate void OnSerialSave(bool rezultat);
        //public event OnSerialSave OnSave;

        //public delegate void OnSerialsLoaded(bool rezultat);
        //public event OnSerialsLoaded OnLoaded;

        public event PropertyChangedEventHandler PropertyChanged;
        private void Set([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
