using Dapper.Contrib.Extensions;
using System;

namespace DbProvider.Model
{
    [Table("Gradovi")]
    public class Grad
    {
        [Key]
        public int Id { get; set; }
        public string Naziv { get; set; }                
    }
}
