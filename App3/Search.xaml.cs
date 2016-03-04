using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
    public class SearchContexte : INotifyPropertyChanged
    {
        private string valeur;
        public string Valeur
        {
            get
            {
                return valeur;
            }
            set
            {
                if (value == valeur)
                    return;
                valeur = value;
                NotifyPropertyChanged("Valeur");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private List<serie> series;
        public List<serie> Series
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
    public sealed partial class Search : Page
    {
        private string responseData;
        private SearchContexte contexte;

        private async void Searching(string q)
        {
            var baseAddress = new Uri("http://api.themoviedb.org/3/");
            ////https://api.themoviedb.org/3/search/tv?api_key=889054f234c4c6cc4e4ec479fb26fe58&query=
            using (var httpClient = new HttpClient { BaseAddress = baseAddress })
            {

                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");

                using (var response = await httpClient.GetAsync("search/tv?api_key=889054f234c4c6cc4e4ec479fb26fe58&language=fr&query=" + q))
                {

                    responseData = await response.Content.ReadAsStringAsync();

                    var series = new List<serie>();
                    JObject jObject = JObject.Parse(responseData);
                    IList<JToken> results = jObject["results"].Children().ToList();
                    foreach (JToken ep in results)
                    {
                        var serie = new serie();
                        serie.Name = (string)ep["name"];
                        serie.Id = (int)ep["id"];
                        serie.ImageUri = "https://image.tmdb.org/t/p/w185/" + (string)ep["poster_path"];
                        serie.Description = (string)ep["overview"];

                        //serie.number_of_episodes = (int)ep["number_of_episodes"];
                        //serie.number_of_seasons = (int)ep["number_of_seasons"];
                        //serie.last_air_date = (DateTime)ep["last_air_date"];

                        series.Add(serie);
                    }

                    contexte.Series = series;
                }
            }
        }

        public Search()
        {
            this.InitializeComponent();
            contexte = new SearchContexte { Valeur = "The Walking Dead" };
            DataContext = contexte;

        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Searching(searchBox.Text.ToString());
            //            contexte.Valeur = responseData;
        }

        // clic item vers fiche
        private void OnPostItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(FicheDetail), e);
        }

        private void HyperlinkButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
