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

        public Order.OrderPoint SelectedPoint { get; set; }

        public Windows.UI.Core.CoreDispatcher Dispatcher { get; set; }

        public bool LocationReady
        {
            get
            {
                return this.locationMg.LocationReady;
            }
        }

        public SearchModel()
        {
            int thread = Environment.CurrentManagedThreadId;

            this.search_cmd = new SearchCommand(this);

            this.locationMg = Managers.ManagerFactory.Instance.GetLocationManager();


            Task initTask = this.locationMg.InitCurrentLocation().ContinueWith((task) =>
                {
                    if (task.Exception == null)
                    {
                        this.NotifyPropertyChanged("LocationReady");
                    }

                });

            this._searchText = string.Empty;

            this.Locations = new System.Collections.ObjectModel.ObservableCollection<LocationItem>();
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

        public System.Collections.ObjectModel.ObservableCollection<LocationItem> Locations { get; set; }

        public async void Search()
        {
            Geopoint hintPoint = await this.locationMg.GetCurrentGeopoint();

            MapLocation currentLocation = this.locationMg.GetCurrentLocation();

            string town = currentLocation.Address.Town;

            string searchQuery = string.Format("{0} {1}", town, this.SearchText);

            MapLocationFinderResult SearchResults = await this.locationMg.GetLocations(hintPoint, searchQuery);

            //lock (this.thisLock)
            //{
            //    this.FillLocations(SearchResults);

            //}

            this.FillLocations(SearchResults);
        }

        private void FillLocations(MapLocationFinderResult SearchResults)
        {
            this.Locations.Clear();

            if (SearchResults.Status == MapLocationFinderStatus.Success)
            {
                foreach (MapLocation location in SearchResults.Locations)
                {
                    this.Locations.Add(new LocationItem(location));
                }
            }
        }


        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                Windows.Foundation.IAsyncAction action =
                this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
                });
            }
        }
    }

    public class LocationItem
    {
        public LocationItem()
        {
            this.Ready = false;
        }

        public LocationItem(MapLocation location)
        {
            this.Address = string.Format("{0}, {1} {2} {3}", location.Address.Town, location.Address.Street, location.Address.StreetNumber, location.Address.BuildingName);

            this.Latitude = location.Point.Position.Latitude;
            this.Longitude = location.Point.Position.Longitude;

            this.MapLocation = location;

            this.Ready = true;
        }

        public string Address { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public MapLocation MapLocation { get; set; }

        public bool Ready { get; set; }

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
