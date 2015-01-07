using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.Geolocation;
using Windows.Services.Maps;

namespace TaxiApp.Core.Managers
{
    public class LocationManager
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
            //this.Init();
        }

        public async Task InitCurrentLocation()
        {
            this.LocationReady = false;

            Geopoint currentGeopoint = null;

            Task<Geopoint> task = this.GetCurrentGeopoint();
            currentGeopoint = await task;

            MapLocationFinderResult result = null;

            Windows.Foundation.IAsyncOperation<MapLocationFinderResult> findLocationTask = null;

            
            
            findLocationTask = MapLocationFinder.FindLocationsAtAsync(currentGeopoint);
            result = await findLocationTask;
            

            if (result.Status == MapLocationFinderStatus.Success)
            {
                this._currentLocation = result.Locations[0];
            }

            this.LocationReady = true;
        }



        public async Task<Geopoint> GetCurrentGeopoint()
        {
            Geoposition pos = null;

            //locator.DesiredAccuracy = PositionAccuracy.High;
            
            Windows.Foundation.IAsyncOperation<Geoposition> task = locator.GetGeopositionAsync();

            pos = await locator.GetGeopositionAsync();

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

    }
}
