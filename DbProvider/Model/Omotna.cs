using Dapper.Contrib.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DbProvider.Model
{
    [Table("Omotne")]
    public class Omotna
    {
        [Key]
        public int Id { get; set; }
        public int KupacId { get; set; }
        public int ProizvodId { get; set; }
        public int UposlenikId { get; set; }
        public string SerijskiBroj { get; set; }
        public int SerijskiBrojInt { get; set; }
        public double UkupnaKolicina { get; set; }
        public byte[] Code { get; set; }
        public DateTime DatumPrintanja { get; set; }
        public DateTime DatumSkeniranja { get; set; }
        public int PaketnaId { get; set; }
        public int PlanPakovanjaId { get; set; }
        public int JeStampana { get; set; }
        public int JeSkenirana { get; set; }
        public int JeRezervisana { get; set; }
        [Computed]
        public IList<Jedinacna> Jedinacne { get; set; }
    }
}
