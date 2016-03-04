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
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace App3
{
    public class FicheContexte : INotifyPropertyChanged
    {
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

        private ObservableCollection<saison> saisons;
        public ObservableCollection<saison> Saisons
        {
            get
            {
                return saisons;
            }
            set
            {
                if (value == saisons)
                    return;
                saisons = value;
                NotifyPropertyChanged("Saisons");
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
    public sealed partial class Fiche : Page
    {
        private FicheContexte contexte;

        public Fiche()
        {
            this.InitializeComponent();
            contexte = new FicheContexte();
            DataContext = contexte;
        }

        private async void GetSeasons(int serieId)
        {
            var baseAddress = new Uri("http://api.themoviedb.org/3/");

            using (var httpClient = new HttpClient { BaseAddress = baseAddress })
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");

                using (var response = await httpClient.GetAsync("tv/"+ serieId + "?api_key=889054f234c4c6cc4e4ec479fb26fe58&language=fr"))
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

                    contexte.Saisons = saisons;
                    GetSeasonDetails();
                }
            }
        }

        private async void GetSeasonDetails(/*int serieId, int saisonNumero*/)
        {
            var baseAddress = new Uri("http://api.themoviedb.org/3/");

            var saisonsUp = new ObservableCollection<saison>();

            foreach (saison saison in contexte.Saisons)
            {

                using (var httpClient = new HttpClient { BaseAddress = baseAddress })
                {
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");

                    using (var response = await httpClient.GetAsync("tv/" + contexte.SerieId + "/season/" + saison.NumeroDeSaison + "?api_key=889054f234c4c6cc4e4ec479fb26fe58&language=fr"))
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


                        JToken token = JObject.Parse(responseData);
                        
                        saison.Name = (string)token.SelectToken("name");
                        saisonsUp.Add(saison);
                        //contexte.Saisons.Add(saison);

                        //return saison;
                    }
                }
            }
            contexte.Saisons = saisonsUp;
        }


        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Parameter.ToString()))
            {
                serie newSerie = e.Parameter as serie;
                //var serieId = ((Windows.UI.Xaml.Controls.ItemClickEventArgs)e.Parameter).ClickedItem as serie;

                contexte.SerieId = newSerie.Id;
                contexte.Saisons = newSerie.saisons;
//                GetSeasons(newSerie.Id);
            }
        }

        private void OnPostItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(FicheSaison), e.ClickedItem);
        }

        private void HyperlinkButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
