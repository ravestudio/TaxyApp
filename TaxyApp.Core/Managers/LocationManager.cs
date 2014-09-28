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
        public async Task<Geopoint> GetCurrentGeopoint()
        {
            Geolocator geo = new Geolocator();

            Geoposition pos = await geo.GetGeopositionAsync();

            Geopoint myGeopoint = new Geopoint(new BasicGeoposition()
            {
                Latitude = pos.Coordinate.Point.Position.Latitude,
                Longitude = pos.Coordinate.Point.Position.Longitude
            });

            return myGeopoint;
        }

        public async Task<MapLocation> GetCurrentLocation(Geopoint hintPoint)
        {
            MapLocation location = null;

            MapLocationFinderResult result = await MapLocationFinder.FindLocationsAsync(string.Empty, hintPoint);

            if (result.Status == MapLocationFinderStatus.Success)
            {
                location = result.Locations[0];
            }

            return location;
        }

        public async Task<MapLocationFinderResult> GetLocations(Geopoint hintPoint, string searchQuery)
        {
            MapLocationFinderResult result = await MapLocationFinder.FindLocationsAsync(searchQuery, hintPoint);

            return result;
        }
    }
}
