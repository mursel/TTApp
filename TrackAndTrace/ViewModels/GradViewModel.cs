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
    public class GradViewModel : INotifyPropertyChanged
    {
        public GradViewModel(Grad grad = null)
        {
            Model = grad ?? new Grad();
        }

        private Grad grad;
        public Grad Model
        {
            get { return grad; }
            set
            {
                if (grad != value)
                {
                    grad = value;
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
