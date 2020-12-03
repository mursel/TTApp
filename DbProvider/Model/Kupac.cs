using Dapper;
using Dapper.Contrib.Extensions;
using System.Threading.Tasks;

namespace DbProvider.Model
{
    [Table("Kupac")]
    public class Kupac
    {
        [Key]
        public int Id { get; set; }
        public int GradId { get; set; }
        public int DrzavaId { get; set; }
        public string Naziv { get; set; }
        public string BrojProizvodnogMjesta { get; set; }
        [Computed]
        public string Grad
        {
            get
            {
                var nazivGrada = Task.Run(() => DAL.GradoviRepository.Instance.GetById(GradId));
                return nazivGrada.Result.Naziv;
            }
        }
        [Computed]
        public string Drzava
        {
            get
            {
                var nazivDrzave = Task.Run(() => DAL.DrzaveRepository.Instance.GetById(DrzavaId));
                return nazivDrzave.Result.Naziv;
            }
        }

    }
}
