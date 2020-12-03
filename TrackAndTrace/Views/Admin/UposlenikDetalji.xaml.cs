using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using TrackAndTrace.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TrackAndTrace.Views.Admin
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UposlenikDetalji : Page
    {
        public UposlenikViewModel ViewModel { get; set; }
        public GlobalViewModel AppViewModel => App.ViewModel;

        public UposlenikDetalji()
        {
            this.InitializeComponent();
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter == null)
            {
                ViewModel = new UposlenikViewModel()
                {
                    IsEditMode = false,
                    IsNewUserMode = true
                };
            }
            else
            {
                var index = int.Parse(e.Parameter.ToString());
                ViewModel = App.ViewModel.ListaUposlenika.Where(u => u.Model.Id == index).First();
            }

            ViewModel.OnSave += ViewModel_OnSave;

            // inicijaliziraj combox 
            cbRole.ItemsSource = App.ViewModel.ListaRola;
        }
        
        private async void ViewModel_OnSave(bool rezultat)
        {
            if (rezultat)
            {
                Task task = App.ViewModel.InicijalizirajPodatke();
                await task.ContinueWith( async t =>
                {
                    if (t.Status == TaskStatus.RanToCompletion)
                    {
                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            if (Frame.CanGoBack)
                            {
                                Frame.GoBack();
                            }
                        });
                    }
                });
            }
        }
    }
}
