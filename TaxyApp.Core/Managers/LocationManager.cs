using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.Geolocation;
using Windows.Services.Maps;

namespace TaxyApp.Core.Managers
{
    public class LocationManager : System.ComponentModel.INotifyPropertyChanged
    {

        private Geolocator locator = new Geolocator();

        private MapLocation _currentLocation = null;

        //private Object thisLock = new Object();

        public bool LocationReady
        {
            get;
            set;
        }

        public LocationManager()
        {
            this.LocationReady = false;
            this.Init();
        }

        public async void Init()
        {
            Geopoint currentGeopoint = await this.GetCurrentGeopoint();

            MapLocationFinderResult result = null;

            result = await MapLocationFinder.FindLocationsAtAsync(currentGeopoint);

            if (result.Status == MapLocationFinderStatus.Success)
            {
                this._currentLocation = result.Locations[0];
            }

            this.LocationReady = true;

            NotifyPropertyChanged("LocationReady");
        }



        public async Task<Geopoint> GetCurrentGeopoint()
        {

            //locator.DesiredAccuracy = PositionAccuracy.High;
            Geoposition pos = await locator.GetGeopositionAsync();

            Geopoint myGeopoint = new Geopoint(new BasicGeoposition()
            {
                Latitude = pos.Coordinate.Point.Position.Latitude,
                Longitude = pos.Coordinate.Point.Position.Longitude
            });

            return myGeopoint;
        }


        public MapLocation GetCurrentLocation()
        {
            return this._currentLocation;
        }

        public async Task<MapLocationFinderResult> GetLocations(Geopoint hintPoint, string searchQuery)
        {
            MapLocationFinderResult result = await MapLocationFinder.FindLocationsAsync(searchQuery, hintPoint, 3);

            return result;
        }

        public async Task<MapRouteFinderResult> GetRoute(IEnumerable<Geopoint> points)
        {
            MapRouteFinderResult routeResult =
                await MapRouteFinder.GetDrivingRouteFromWaypointsAsync(
                points,
                MapRouteOptimization.Time,
                MapRouteRestrictions.None);

            return routeResult;
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
}
