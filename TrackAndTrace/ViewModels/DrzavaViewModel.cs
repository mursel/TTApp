using DbProvider.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TrackAndTrace.ViewModels
{
    public class DrzavaViewModel : INotifyPropertyChanged
    {
        public DrzavaViewModel(Drzava drzava = null)
        {
            Model = drzava ?? new Drzava();
        }

        private Drzava drzava;
        public Drzava Model
        {
            get { return drzava; }
            set
            {
                if (drzava != value)
                {
                    drzava = value;
                    Set();
                }
            }
        }

        #region Atributi

        public string Naziv
        {
            get { return Model.Naziv; }
            set
            {
                if (Model.Naziv != value)
                {
                    Model.Naziv = value;
                    Set();
                }
            }
        }                

        #endregion


        public event PropertyChangedEventHandler PropertyChanged;
        private void Set([CallerMemberName] string propName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
