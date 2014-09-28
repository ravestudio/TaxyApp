﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.Geolocation;
using Windows.Services.Maps;

namespace TaxyApp.Core.Managers
{
    public class LocationManager
    {

        private Geolocator locator = new Geolocator();

        private MapLocation _currentLocation = null;

        //private Object thisLock = new Object();

        public async void Init()
        {
            Geopoint hintPoint = await this.GetCurrentGeopoint();

            MapLocationFinderResult result = await MapLocationFinder.FindLocationsAtAsync(hintPoint);

            if (result.Status == MapLocationFinderStatus.Success)
            {
                this._currentLocation = result.Locations[0];
            }
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
    }
}
