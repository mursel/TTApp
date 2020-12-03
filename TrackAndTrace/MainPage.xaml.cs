using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using TrackAndTrace.ViewModels;
using TrackAndTrace.Views.Admin;
using TrackAndTrace.Views.Planer;
using TrackAndTrace.Views.Scan;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace TrackAndTrace
{
    public sealed partial class MainPage : Page
    {

        public GlobalViewModel AppViewModel => App.ViewModel;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void BtnValidate_Click(object sender, RoutedEventArgs e)
        {
            var user = tbUsername.Text;
            var pass = tbPassword.Password;

            var task = Task.Run(()=>AuthViewModel.Instance.Authorize(user, pass));

            task.ContinueWith(async t1 =>
            {
                var authUser = App.ViewModel.AutorizovaniKorisnik;

                if (authUser != null)
                {

                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        if (authUser.Rola == "Administrator")
                            Frame.Navigate(typeof(AdminMainPanel));

                        if (authUser.Rola == "Planer")
                            Frame.Navigate(typeof(PlanerMainPage));

                        if (authUser.Rola == "Uposlenik")
                            Frame.Navigate(typeof(JedinacnePage));

                        if (authUser.Rola == "SkladistarPaketi")
                            Frame.Navigate(typeof(OmotnePage));

                        if (authUser.Rola == "SkladistarPalete")
                            Frame.Navigate(typeof(PaketnePage));

                    });
                }
                else
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                    {
                        var errDlg = new ContentDialog();
                        errDlg.Title = "Greška";
                        errDlg.Content = "Greška prilikom prijave na sistem! Provjerite korisničko ime i lozinku!";
                        errDlg.CloseButtonText = "Zatvori";
                        errDlg.DefaultButton = ContentDialogButton.Close;
                        await errDlg.ShowAsync();
                    });
                }
            });
        }
    }
}
