using DbProvider.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrackAndTrace.ViewModels
{
    public class IzvozPodatakaViewModel
    {
        public IzvozPodatakaViewModel(PlanPakovanjaViewModel odabraniPlan = null)
        {
            Model = odabraniPlan ?? new PlanPakovanjaViewModel();
        }

        private PlanPakovanjaViewModel planPakovanja;
        public PlanPakovanjaViewModel Model
        {
            get { return planPakovanja; }
            set {
                if (planPakovanja != value)
                {
                    planPakovanja = value;
                }
            }
        }

        public List<JedinacnaViewModel> ListaJedinacne { get; set; }
        public List<Jedinacna> Jedinacne { get; set; }

        public List<OmotnaViewModel> ListaOmotne { get; set; }
        public List<Omotna> Omotne { get; set; }

        public List<PaketnaViewModel> ListaPaketne { get; set; }
        public List<Paketna> Paketne { get; set; }

        public List<PaletnaViewModel> ListaPaletne { get; set; }
        public List<Paletna> Paletne { get; set; }

        public async Task<bool> UcitajPlanPakovanja()
        {
            ListaJedinacne = new List<JedinacnaViewModel>();
            ListaOmotne = new List<OmotnaViewModel>();
            ListaPaketne = new List<PaketnaViewModel>();
            ListaPaletne = new List<PaletnaViewModel>();
            Jedinacne = new List<Jedinacna>();
            Omotne = new List<Omotna>();
            Paketne = new List<Paketna>();
            Paletne = new List<Paletna>();

            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                App.ViewModel.UcitavaSe = true;
            });

            var jedinacne = await App.SbRepository.GetJedinacneAsync((int)Model.Kolicina, Model.Id);
            if (jedinacne == null) return false;

            foreach (var item in jedinacne)
            {
                ListaJedinacne.Add(new JedinacnaViewModel(item));
                Jedinacne.Add(item);
            }

            var omotne = await App.SbRepository.GetOmotneAsync((int)Model.UkupnoOmotnih, Model.Id);
            if (omotne == null) return false;

            foreach (var item in omotne)
            {
                ListaOmotne.Add(new OmotnaViewModel(item));
                Omotne.Add(item);
            }

            var paketne = await App.SbRepository.GetPaketneAsync((int)Model.UkupnoPaketa, Model.Id);
            if (paketne == null) return false;

            foreach (var item in paketne)
            {
                ListaPaketne.Add(new PaketnaViewModel(item));
                Paketne.Add(item);
            }

            var paletne = await App.SbRepository.GetPaletneAsync(Model.Id);
            if (paletne == null) return false;

            foreach (var item in paletne)
            {
                ListaPaletne.Add(new PaletnaViewModel(item));
                Paletne.Add(item);
            }

            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                App.ViewModel.UcitavaSe = false;
            });

            return true;
        }

    }
}
