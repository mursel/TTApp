using Dapper.Contrib.Extensions;
using System;

namespace DbProvider.Model
{
    [Table("Proizvodi")]
    public class Proizvod
    {
        [Key]
        public int Id { get; set; }
        public string Naziv { get; set; }
        public string JedinicaMjere { get; set; }
    }
}
