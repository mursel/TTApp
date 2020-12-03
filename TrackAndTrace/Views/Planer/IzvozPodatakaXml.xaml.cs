using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using TrackAndTrace.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TrackAndTrace.Views.Planer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class IzvozPodatakaXml : Page
    {
        public GlobalViewModel AppViewModel => App.ViewModel;
        public IzvozPodatakaViewModel ViewModel { get; set; }
        
        public IzvozPodatakaXml()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var aktivniPlanovi = App.ViewModel.ListaPlanova.Where(p => p.StatusPlana == 1);
            cbPlanovi.ItemsSource = aktivniPlanovi.ToList();
        }
        
        private void cbPlanovi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel = new IzvozPodatakaViewModel(App.ViewModel.OdabraniPlan);

            var task = Task.Run(() => ViewModel.UcitajPlanPakovanja());

            task.ContinueWith(async t1 =>
            {
                if (t1.Status == TaskStatus.RanToCompletion)
                {
                    await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        lbJedinacne.ItemsSource = ViewModel.ListaJedinacne;
                        lbOmotne.ItemsSource = ViewModel.ListaOmotne;
                        lbPaketne.ItemsSource = ViewModel.ListaPaketne;
                        lbPaletne.ItemsSource = ViewModel.ListaPaletne;
                    });
                }
            });
        }
        /*
        <MessageID>141 </MessageID>
<MessageTime>2018-11-01T23:27:36</MessageTime>
<MessageType>None</MessageType>
<ShipmentNumber>SNR-126</ShipmentNumber>
<DeliveryNoteNumber>DLNR-126</DeliveryNoteNumber>
<PurchaseOrderNumber>PON-</PurchaseOrderNumber>
<ExpectedDeliveryDate>2018-11-01</ExpectedDeliveryDate>
         * 
         * */

        private async void btnExport_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.ViewMode = PickerViewMode.List;
            folderPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            folderPicker.FileTypeFilter.Add(".xml");
            StorageFolder storageFolder = await folderPicker.PickSingleFolderAsync();
            //StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile sampleFile =
                await storageFolder.CreateFileAsync("feem_export.xml",
                    CreationCollisionOption.ReplaceExisting);

            using (IRandomAccessStream writeStream = await sampleFile.OpenAsync(FileAccessMode.ReadWrite, StorageOpenOptions.AllowReadersAndWriters))
            {

                XmlWriterSettings writerSettings = new XmlWriterSettings();
                writerSettings.Async = true;

                using (XmlWriter xmlWriter = XmlWriter.Create(writeStream.AsStreamForWrite()))
                {
                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteStartElement("Shipment");
                    xmlWriter.WriteAttributeString("Hash", "Hash");
                    xmlWriter.WriteAttributeString("FileCreator", "Mursel Musabasic");
                    xmlWriter.WriteAttributeString("FileType", "FEEM-Std");
                    xmlWriter.WriteAttributeString("FileVersion", "1.0");
                    xmlWriter.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
                    xmlWriter.WriteAttributeString("xsi", "noNamespaceSchemaLocation", null, "FEEM-Std.xsd");

                    xmlWriter.WriteStartElement("MessageID");
                    xmlWriter.WriteString(Environment.TickCount.ToString());
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("MessageTime");
                    xmlWriter.WriteString(DateTime.Now.ToString("o"));
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("MessageType");
                    xmlWriter.WriteString("None");
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("DeliveryNoteNumber");
                    xmlWriter.WriteString("DLNR-155");
                    xmlWriter.WriteEndElement();

                    string brojkupca = App.ViewModel.ListaKupaca.Where(k => k.Id == ViewModel.Model.KupacId).First().BrojProizvodnogMjesta;
                    string nazivKupca = App.ViewModel.ListaKupaca.Where(k => k.Id == ViewModel.Model.KupacId).First().Naziv;
                    string gradKupca = App.ViewModel.ListaKupaca.Where(k => k.Id == ViewModel.Model.KupacId).First().Grad;
                    string drzavaKupca = App.ViewModel.ListaKupaca.Where(k => k.Id == ViewModel.Model.KupacId).First().Drzava;

                    xmlWriter.WriteStartElement("Sender");
                    xmlWriter.WriteStartElement("Code");
                    xmlWriter.WriteString("BH001");
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("Name");
                    xmlWriter.WriteString("Moja kompanija");
                    xmlWriter.WriteEndElement();
                    //xmlWriter.WriteStartElement("ZipCode");
                    //xmlWriter.WriteString("73000");
                    //xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("City");
                    xmlWriter.WriteString("Gorazde");
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("Country");
                    xmlWriter.WriteString("Bosnia and Herzegovina");
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Receiver");
                    xmlWriter.WriteStartElement("Code");
                    xmlWriter.WriteString(brojkupca);
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("Name");
                    xmlWriter.WriteString(nazivKupca);
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("City");
                    xmlWriter.WriteString(gradKupca);
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("Country");
                    xmlWriter.WriteString(drzavaKupca);
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Units");       // Palete start



                    // za svaku paletu
                    foreach (var paleta in ViewModel.ListaPaletne)
                    {
                        xmlWriter.WriteStartElement("Unit");        // Paleta start
                        xmlWriter.WriteAttributeString("PSN", brojkupca);
                        xmlWriter.WriteAttributeString("UID", paleta.SerijskiBroj.Replace("(", "").Replace(")", ""));

                        xmlWriter.WriteStartElement("ItemQuantity");
                        xmlWriter.WriteString(((int)ViewModel.Model.Kolicina).ToString());
                        xmlWriter.WriteEndElement();
                        xmlWriter.WriteStartElement("CountOfTradeUnits");
                        xmlWriter.WriteString(((int)ViewModel.Model.UkupnoPaketa).ToString());
                        xmlWriter.WriteEndElement();
                        xmlWriter.WriteStartElement("PackagingLevel");
                        xmlWriter.WriteString(paleta.SerijskiBroj.Substring(16, 2));
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("Units");       // Paketi start

                        var paketne = ViewModel.ListaPaketne.Where(p => p.Model.PaletnaId == paleta.Model.Id);

                        // za svaki paket u pripadajucoj paleti
                        foreach (var paketna in paketne)
                        {
                            xmlWriter.WriteStartElement("Unit");                // Paket start
                            xmlWriter.WriteAttributeString("PSN", brojkupca);
                            xmlWriter.WriteAttributeString("UID", paketna.SerijskiBroj.Replace("(", "").Replace(")", ""));

                            xmlWriter.WriteStartElement("ItemQuantity");
                            xmlWriter.WriteString(((int)ViewModel.Model.OmotnihUPaket * ViewModel.Model.JedinacnihUOmot).ToString());
                            xmlWriter.WriteEndElement();
                            xmlWriter.WriteStartElement("CountOfTradeUnits");
                            xmlWriter.WriteString(((int)ViewModel.Model.OmotnihUPaket).ToString());
                            xmlWriter.WriteEndElement();
                            xmlWriter.WriteStartElement("PackagingLevel");
                            xmlWriter.WriteString(paketna.SerijskiBroj.Substring(16, 2));
                            xmlWriter.WriteEndElement();

                            xmlWriter.WriteStartElement("Units");       // Omotne start

                            var omotne = ViewModel.ListaOmotne.Where(o => o.Model.PaketnaId == paketna.Model.PaketnaId);

                            // za svaki omot u pripadajucem paketu
                            foreach (var omotna in omotne)
                            {
                                xmlWriter.WriteStartElement("Unit");                // Omot start
                                xmlWriter.WriteAttributeString("PSN", brojkupca);
                                xmlWriter.WriteAttributeString("UID", omotna.SerijskiBroj.Replace("(", "").Replace(")", ""));

                                xmlWriter.WriteStartElement("ItemQuantity");
                                xmlWriter.WriteString(((int)ViewModel.Model.JedinacnihUOmot).ToString());
                                xmlWriter.WriteEndElement();
                                xmlWriter.WriteStartElement("CountOfTradeUnits");
                                xmlWriter.WriteString(((int)ViewModel.Model.JedinacnihUOmot).ToString());
                                xmlWriter.WriteEndElement();
                                xmlWriter.WriteStartElement("PackagingLevel");
                                xmlWriter.WriteString(omotna.SerijskiBroj.Substring(16, 2));
                                xmlWriter.WriteEndElement();

                                xmlWriter.WriteStartElement("Items");       // Jedinacne start

                                var jedinacne = ViewModel.ListaJedinacne.Where(j => j.Model.OmotnaId == omotna.Model.Id);

                                // za svaku jedinacnu u pripadajucem omotu
                                foreach (var jedinacna in jedinacne)
                                {
                                    xmlWriter.WriteStartElement("Item");                // Jedinacna start
                                    xmlWriter.WriteAttributeString("PSN", brojkupca);
                                    xmlWriter.WriteAttributeString("UID", jedinacna.SerijskiBroj.Replace("(", "").Replace(")", ""));
                                    xmlWriter.WriteEndElement();                        // Jedinacna end
                                }

                                xmlWriter.WriteEndElement();                 // Jedinacne end

                                xmlWriter.WriteEndElement();                        // Omot end
                            }

                            xmlWriter.WriteEndElement();                // Omotne end

                            xmlWriter.WriteEndElement();                        // Paket end
                        }

                        xmlWriter.WriteEndElement();                // Paketi end
                        xmlWriter.WriteEndElement();                // Paleta end
                    }

                    xmlWriter.WriteEndElement();                    // Palete end

                    xmlWriter.WriteEndDocument();

                    xmlWriter.Flush();
                    xmlWriter.Close();
                }
                
                var dialog = new ContentDialog();
                dialog.Title = "Status izvoza podataka";
                dialog.Content = "XML datoteka uspješno izvezena!";
                dialog.CloseButtonText = "Zatvori";
                dialog.DefaultButton = ContentDialogButton.Close;
                await dialog.ShowAsync();
            }


        }
    }
}
