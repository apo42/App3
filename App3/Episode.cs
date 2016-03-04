using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace App3
{
    public class episode
    {
        public string Name { get; set; }
        public string ImageUri { get; set; }
        public string Description { get; set; }
        public int Id { get; set; }

        private bool isChecked;
        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                if (value == isChecked)
                    return;
                isChecked = value;
                NotifyPropertyChanged("IsChecked");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string nomPropriete)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(nomPropriete));
        }


        public static readonly DependencyProperty IsCheckedProperty =
           DependencyProperty.Register("IsChecked", typeof(bool),
             typeof(episode), new PropertyMetadata(false));
    }
}
