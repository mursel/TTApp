using DbProvider.DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TrackAndTrace.ViewModels;
using TrackAndTrace.Views.Admin;
using TrackAndTrace.Views.Planer;
using TrackAndTrace.Views.Scan;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
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
    sealed partial class App : Application
    {

        // repozitoriji kao db provideri
        public static GradoviRepository GradoviRepository { get; private set; }
        public static DrzaveRepository DrzaveRepository { get; private set; }
        public static RoleRepository RoleRepository { get; private set; }
        public static UposleniciRepository UposleniciRepository { get; private set; }
        public static KupciRepository KupciRepository { get; private set; }
        public static ProizvodiRepository ProizvodiRepository { get; private set; }
        public static PlanPakDefaultRepository PlanPakDefaultRepository { get; private set; }
        public static PlanPakovanjaRepository PlanPakovanjaRepository { get; private set; }
        public static IdentifikatoriRepository IdentifikatoriRepository { get; private set; }
        public static SbRepository SbRepository { get; private set; }

        // glavni view model
        public static GlobalViewModel ViewModel { get; } = new GlobalViewModel();

        public App() => this.InitializeComponent();


        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            // inicijaliziraj repozitorije
            RoleRepository = new RoleRepository();
            GradoviRepository = new GradoviRepository();
            DrzaveRepository = new DrzaveRepository();
            UposleniciRepository = new UposleniciRepository();
            KupciRepository = new KupciRepository();
            ProizvodiRepository = new ProizvodiRepository();
            PlanPakDefaultRepository = new PlanPakDefaultRepository();
            PlanPakovanjaRepository = new PlanPakovanjaRepository();
            IdentifikatoriRepository = new IdentifikatoriRepository();
            SbRepository = new SbRepository();

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                                
                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }
    }
}
