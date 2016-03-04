using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App3
{
    public class serie
    {
        public string Name { get; set; }
        public string ImageUri { get; set; }
        public string Description { get; set; }
        public int Id { get; set; }

        public int number_of_episodes { get; set; }
        public int number_of_seasons { get; set; }
        public DateTime last_air_date { get; set; }

        public int number_of_episodes_checked { get; set; }
        public string Percent { get { return number_of_episodes_checked.ToString() + "/" + number_of_episodes.ToString(); } }
        

        public ObservableCollection<saison> saisons { get; set; }
    }
}
