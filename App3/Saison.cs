using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App3
{
    public class saison
    {
        public string Name { get; set; }
        public string ImageUri { get; set; }
        public string Description { get; set; }
        public int NumeroDeSaison { get; set; }
        public int Id { get; set; }
        public int SerieId { get; set; }

        public ObservableCollection<episode> episodes { get; set; }
    }
}
