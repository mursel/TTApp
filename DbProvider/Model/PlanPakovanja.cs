using Dapper.Contrib.Extensions;
using System;
using System.Threading.Tasks;

namespace DbProvider.Model
{
    [Table("PlanPakovanja")]
    public class PlanPakovanja
    {
        [Key]
        public int Id { get; set; }
        public int KupacId { get; set; }
        public int ProizvodId { get; set; }
        public int PlanerId { get; set; }
        public DateTime DatumIzradePlana { get; set; }
        public DateTime DatumZavrsetkaPlana { get; set; }
        public int StatusPlana { get; set; }
        public double Kolicina { get; set; }
        public double JedinacnihUOmot { get; set; }
        public double OmotnihUPaket { get; set; }
        public double PaketaNaPaletu { get; set; }
        public double UkupnoOmotnih { get; set; }
        public double UkupnoPaketa { get; set; }
        public double UkupnoPaleta { get; set; }
        [Computed]
        public int TrajanjePlanaUDanima
        {
            get
            {
                return (DatumZavrsetkaPlana - DatumIzradePlana).Days;
            }
        }
        [Computed]
        public string NazivKupca
        {
            get
            {
                var nazivKupca = Task.Run(() => DAL.KupciRepository.Instance.GetById(KupacId));
                return nazivKupca.Result.Naziv;
            }
        }
        [Computed]
        public string NazivProizvoda
        {
            get
            {
                var nazivProizvoda = Task.Run(() => DAL.ProizvodiRepository.Instance.GetById(ProizvodId));
                return nazivProizvoda.Result.Naziv;
            }
        }
        [Computed]
        public string Planer
        {
            get
            {
                var nazivPlanera = Task.Run(() => DAL.UposleniciRepository.Instance.GetById(PlanerId));
                return nazivPlanera.Result.ImePrezime;
            }
        }
        [Computed]
        public string JeAktivan
        {
            get
            {
                if (StatusPlana == 0) return "Neaktivan";
                if (StatusPlana == 1) return "Aktivan";
                return "-";
            }
        }


    }
}
