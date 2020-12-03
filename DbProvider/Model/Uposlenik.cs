using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DbProvider.Model
{
    [Table("Uposlenici")]
    public class Uposlenik
    {
        [Key]
        public int Id { get; set; }

        private int rolaId;

        public int RolaId
        {
            get { return rolaId; }
            set { rolaId = value; }
        }
        
        private int gradId;

        public int GradId
        {
            get { return gradId; }
            set { gradId = value; }
        }


        private int drzavaId;

        public int DrzavaId
        {
            get { return drzavaId; }
            set { drzavaId = value; }
        }

        private string ime;

        public string Ime
        {
            get { return ime; }
            set { ime = value; }
        }

        private string imeRoditelja;

        public string ImeRoditelja
        {
            get { return imeRoditelja; }
            set { imeRoditelja = value; }
        }

        private string prezime;

        public string Prezime
        {
            get { return prezime; }
            set { prezime = value; }
        }

        private string username;

        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        private string password;

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        private string email;

        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        [Computed]
        public string ImePrezime
        {
            get
            {
                return Ime + ' ' + Prezime;
            }
        }
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
        [Computed]
        public string Rola
        {
            get
            {
                var nazivRole = Task.Run(() => DAL.RoleRepository.Instance.GetById(RolaId));
                return nazivRole.Result.Naziv;
            }
        }

    }
}
