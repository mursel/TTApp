using Dapper.Contrib.Extensions;
using System;

namespace DbProvider.Model
{
    [Table("Role")]
    public class Rola
    {
        [Key]
        public int Id { get; set; }
        public string Naziv { get; set; }
    }
}
