using Dapper.Contrib.Extensions;
using DbProvider.Pomocna;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbProvider.Model
{
    [Table("Drzave")]
    public class Drzava
    {
        [Key]
        public int Id { get; set; }
        public string Naziv { get; set; }
    }
}
