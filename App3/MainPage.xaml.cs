using App3.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using System.Threading.Tasks;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace App3
{
    public class MainPageDesign
    {
        public IEnumerable<serie> Series
        {
            get
            {
                return new ObservableCollection<serie>
            {
                new serie { number_of_episodes_checked = 1, Name = "The Walking Dead", Id = 1, ImageUri = "https://image.tmdb.org/t/p/w185/vxuoMW6YBt6UsxvMfRNwRl9LtWS.jpg", Description = "The walking dead..........d........d........d........desc....desc"},
                new serie { number_of_episodes_checked = 5, Name = "2 TWD 2", Id = 2, ImageUri = "https://image.tmdb.org/t/p/w185/vxuoMW6YBt6UsxvMfRNwRl9LtWS.jpg", Description = "The walking dead..........d........d........d........desc....desc"}
            }; 
            }
        }
    }


    public class Contexte : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<serie> series;
        public ObservableCollection<serie> Series
        {
            get
            {
                return series;
            }
            set
            {
                if (value == series)
                    return;
                series = value;
                NotifyPropertyChanged("Series");
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
    public sealed partial class MainPage : Page
    {
        private Contexte contexte;
        private ObservableCollection<serie>  Series;
        Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;


        public MainPage()
        {
            this.InitializeComponent();

            ReadInLocalFolder();

            Series = new ObservableCollection<serie>();
            contexte = new Contexte { Series = Series};
            DataContext = contexte;
            //var serie = new serie();
            //serie.Name = "the walking dead";
            //serie.Id = 1402;
            //serie.ImageUri = "vxuoMW6YBt6UsxvMfRNwRl9LtWS.jpg";
            //contexte.Series.Add(serie);

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        async void WriteInLocalFolder()
        {
            StorageFile sampleFile = await localFolder.CreateFileAsync("dataFile.txt",
                CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(sampleFile, JToken.FromObject(contexte.Series).ToString());
        }

        // Read data from a file

        async Task ReadInLocalFolder()
        {
            try
            {
                StorageFile sampleFile = await localFolder.GetFileAsync("dataFile.txt");
                String datajson = await FileIO.ReadTextAsync(sampleFile);
                // Data is contained in datajson
                if (!String.IsNullOrWhiteSpace(datajson))
                {
                    contexte.Series = JToken.Parse(datajson).ToObject<ObservableCollection<serie>>();
                }
            }
            catch (Exception)
            {
                // Timestamp not found
            }
        }

        private async void GetSeasons(int serieId)
        {
            var baseAddress = new Uri("http://api.themoviedb.org/3/");

            using (var httpClient = new HttpClient { BaseAddress = baseAddress })
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");

                using (var response = await httpClient.GetAsync("tv/" + serieId + "?api_key=889054f234c4c6cc4e4ec479fb26fe58&language=fr"))
                {

                    string responseData = await response.Content.ReadAsStringAsync();

                    var saisons = new ObservableCollection<saison>();
                    JObject jObject = JObject.Parse(responseData);
                    IList<JToken> results = jObject["seasons"].Children().ToList();
                    foreach (JToken ep in results)
                    {


                        var saison = new saison();
                        saison.Name = "test";
                        saison.Id = (int)ep["id"];
                        saison.ImageUri = "https://image.tmdb.org/t/p/w185/" + (string)ep["poster_path"];
                        //saison.Description = (string)ep["overview"];
                        saison.NumeroDeSaison = (int)ep["season_number"];
                        saison.SerieId = serieId;
                        saisons.Add(saison);
                        //GetSeasonDetails(serieId, (int)ep["id"]);
                    }
                    contexte.Series.FirstOrDefault(s => s.Id == serieId).saisons = saisons;
                    GetSeasonDetails(serieId);
                }
            }
        }

        private async void GetSeasonDetails(int serieId)
        {
            var baseAddress = new Uri("http://api.themoviedb.org/3/");

            var saisonsUp = new ObservableCollection<saison>();

            foreach (saison saison in contexte.Series.FirstOrDefault(s => s.Id == serieId).saisons)
            {

                using (var httpClient = new HttpClient { BaseAddress = baseAddress })
                {
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");

                    using (var response = await httpClient.GetAsync("tv/" + serieId + "/season/" + saison.NumeroDeSaison + "?api_key=889054f234c4c6cc4e4ec479fb26fe58&language=fr"))
                    {

                        string responseData = await response.Content.ReadAsStringAsync();

                        //var saison = contexte.Saisons.FirstOrDefault(s => s.NumeroDeSaison == saisonNumero);
                        var episodes = new ObservableCollection<episode>();
                        JObject jObject = JObject.Parse(responseData);
                        IList<JToken> results = jObject["episodes"].Children().ToList();
                        foreach (JToken ep in results)
                        {
                            var episode = new episode();
                            episode.Name = (string)ep["name"];
                            episode.Id = (int)ep["id"];
                            episode.ImageUri = "https://image.tmdb.org/t/p/w185/" + (string)ep["still_path"];
                            episode.Description = (string)ep["overview"];


                            episodes.Add(episode);
                        }
                        saison.episodes = episodes;
                        saison.SerieId = serieId;


                        JToken token = JObject.Parse(responseData);

                        saison.Name = (string)token.SelectToken("name");
                        saisonsUp.Add(saison);
                    }
                }
            }
            contexte.Series.FirstOrDefault(s => s.Id == serieId).saisons = saisonsUp;
            WriteInLocalFolder();
        }


        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null && !string.IsNullOrEmpty(e.Parameter.ToString()))
            {
                serie newSerie = e.Parameter as serie;
                if (contexte.Series.Any(s => s.Id == newSerie.Id))
                {
                    contexte.Series.Remove(contexte.Series.FirstOrDefault(s => s.Id == newSerie.Id));
                }
                else
                {
                    // add serie
                    contexte.Series.Add(newSerie);
                    // get saesons
                    GetSeasons(newSerie.Id);
                }
                WriteInLocalFolder();
            }
            else
            {
                ReadInLocalFolder();
            }
        }


        private void OnPostItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(Fiche), e.ClickedItem);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Search), e);
        }

    }
}
