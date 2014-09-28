using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.Geolocation;
using Windows.Services.Maps;

namespace TaxyApp.Core.DataModel
{
    public class SearchModel : System.ComponentModel.INotifyPropertyChanged
    {
        private TaxyApp.Core.Managers.LocationManager locationMg = null;
        private SearchCommand search_cmd = null;
        private string _searchText = null;

        private Object thisLock = new Object();

        public bool LocationReady
        {
            get
            {
                return this.locationMg.LocationReady;
            }
        }

        public SearchModel()
        {
            this.search_cmd = new SearchCommand(this);

            this.locationMg = new Managers.LocationManager();

            this.locationMg.PropertyChanged += locationMg_PropertyChanged;

            this._searchText = string.Empty;

            this.Locations = new System.Collections.ObjectModel.ObservableCollection<string>();
        }

        void locationMg_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LocationReady")
            {
                NotifyPropertyChanged("LocationReady");
            }
        }

        public string SearchText {
            get { return this._searchText; }
            set
            {
                this._searchText = value;

                if (this.SearchChanged.CanExecute(null))
                {
                    this.SearchChanged.Execute(null);
                }
                else
                {
                    this.Locations.Clear();
                }
            }
        }

        public SearchCommand SearchChanged { get { return this.search_cmd; } }

        //public Windows.Services.Maps.MapLocationFinderResult SearchResults
        //{
        //    get;
        //    set;
        //}

        public System.Collections.ObjectModel.ObservableCollection<string> Locations { get; set; }

        public async void Search()
        {
            Geopoint hintPoint = await this.locationMg.GetCurrentGeopoint();

            MapLocation currentLocation = this.locationMg.GetCurrentLocation();

            string town = currentLocation.Address.Town;

            string searchQuery = string.Format("{0} {1}", town, this.SearchText);

            MapLocationFinderResult SearchResults = await this.locationMg.GetLocations(hintPoint, searchQuery);

            lock (this.thisLock)
            {
                this.FillLocations(SearchResults);

            }
        }

        private void FillLocations(MapLocationFinderResult SearchResults)
        {
            this.Locations.Clear();

            if (SearchResults.Status == MapLocationFinderStatus.Success)
            {
                foreach (MapLocation location in SearchResults.Locations)
                {
                    this.Locations.Add(location.Address.ToString());

                }
            }
        }


        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                    new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class SearchCommand : System.Windows.Input.ICommand
    {
        private SearchModel _model = null;

        public SearchCommand(SearchModel model)
        {
            this._model = model;
        }

        public bool CanExecute(object parameter)
        {
            bool res = false;
            
            if (this._model.SearchText.Length > 5)
            {
                res = true;
            }

            return res;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            this._model.Search();
        }
    }
}
