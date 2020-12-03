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
    public class RolaViewModel : INotifyPropertyChanged
    {
        public RolaViewModel(Rola rola = null)
        {
            Model = rola ?? new Rola();
        }

        private Rola rola;
        public Rola Model
        {
            get { return rola; }
            set
            {
                if (rola != value)
                {
                    rola = value;
                    Set();
                }
            }
        }

        #region Atributi

        public int Id { get { return Model.Id; } }

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
