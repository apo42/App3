using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace App3
{
    public class FicheSaisonContexte : INotifyPropertyChanged
    {
        public saison Saison;

        private int serieId;
        public int SerieId
        {
            get
            {
                return serieId;
            }
            set
            {
                if (value == serieId)
                    return;
                serieId = value;
                NotifyPropertyChanged("Valeur");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<episode> episodes;
        public ObservableCollection<episode> Episodes
        {
            get
            {
                return episodes;
            }
            set
            {
                if (value == episodes)
                    return;
                episodes = value;
                NotifyPropertyChanged("Episodes");
            }
        }

        public void NotifyPropertyChanged(string nomPropriete)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(nomPropriete));
        }
    }


    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FicheSaison : Page
    {
        private FicheSaisonContexte contexte;
        Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        ObservableCollection<serie> Series;

        public FicheSaison()
        {
            this.InitializeComponent();
            contexte = new FicheSaisonContexte();
            DataContext = contexte;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ReadInLocalFolder();
            if (!string.IsNullOrEmpty(e.Parameter.ToString()))
            {
                saison saison = e.Parameter as saison;
                contexte.Saison = saison;
                contexte.Episodes = saison.episodes;
            }
        }

        private void OnPostItemClick(object sender, ItemClickEventArgs e)
        {

        }

        async void WriteInLocalFolder()
        {
            StorageFile sampleFile = await localFolder.CreateFileAsync("dataFile.txt",
                CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(sampleFile, JToken.FromObject(Series).ToString());

            Frame.Navigate(typeof(MainPage));
        }
        async Task ReadInLocalFolder()
        {
            try
            {
                StorageFile sampleFile = await localFolder.GetFileAsync("dataFile.txt");
                String datajson = await FileIO.ReadTextAsync(sampleFile);
                // Data is contained in datajson
                if (!String.IsNullOrWhiteSpace(datajson))
                {
                    Series = JToken.Parse(datajson).ToObject<ObservableCollection<serie>>();
                }
            }
            catch (Exception)
            {
                // Timestamp not found
            }
        }


        private void HyperlinkButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // test save checked here

            var serie = Series.FirstOrDefault(s => s.Id == contexte.Saison.SerieId);
            var saison = serie.saisons.FirstOrDefault(ss => ss.Id == contexte.Saison.Id);
            saison.episodes = contexte.Episodes;
            //            Series.FirstOrDefault(s => s.Id == contexte.Saison.SerieId).saisons.FirstOrDefault(ss => ss.Id == contexte.Saison.Id).episodes = contexte.Episodes;

            // number_of_episodes_checked
            serie.number_of_episodes_checked = 0;
            foreach (saison s in serie.saisons)
            {
                serie.number_of_episodes_checked += s.episodes.Where(ep => ep.IsChecked).Count();
            }
            WriteInLocalFolder();

            
        }
    }
}
