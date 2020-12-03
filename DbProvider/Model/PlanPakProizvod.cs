using Dapper.Contrib.Extensions;
using System;
using System.Threading.Tasks;

namespace DbProvider.Model
{
    [Table("PlanPakProizvod")]
    public class PlanPakProizvod
    {
        [Key]
        public int Id { get; set; }
        public int ProizvodId { get; set; }
        public double JedinacnihZaOmot { get; set; }
        public double OmotnihZaPaket { get; set; }
        public double PaketaNaPaletu { get; set; }
        [Computed]
        public string J_Opis
        {
            get
            {
                return JedinacnihZaOmot + " jedinacnih dolazi u jednom omotu";
            }
        }
        [Computed]
        public string O_Opis
        {
            get
            {
                return OmotnihZaPaket + " omotnih dolazi u jednom paketu";
            }
        }
        [Computed]
        public string P_Opis
        {
            get
            {
                return PaketaNaPaletu + " paketa dolazi na jednoj paleti";
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
    }
}
