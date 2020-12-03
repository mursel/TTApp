using Dapper.Contrib.Extensions;
using DbProvider.Pomocna;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbProvider.Model
{
    [Table("Identifikatori")]
    public class Identifikator
    {
        [Key]
        public int Id { get; set; }
        public string Naziv { get; set; }
        public int KupacId { get; set; }
    }
}
