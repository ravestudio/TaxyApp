using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;

namespace TaxyApp.Core.Managers
{
    public class MapPainter
    {


        public async Task ShowMyPossitionAsync(MapControl mapControl)
        {
            TaxyApp.Core.Managers.LocationManager locationMG = TaxyApp.Core.Managers.ManagerFactory.Instance.GetLocationManager();

            Geopoint myGeopoint = await locationMG.GetCurrentGeopoint();

            mapControl.Center = myGeopoint;

            mapControl.ZoomLevel = 12;
            mapControl.LandmarksVisible = true;

            AddMapIcon(myGeopoint, mapControl);
        }

        private void AddMapIcon(Geopoint point, MapControl mapControl)
        {
            Windows.UI.Xaml.Shapes.Ellipse fence = new Windows.UI.Xaml.Shapes.Ellipse();
            fence.Fill = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 50, 120, 90));

            fence.Width = 15;
            fence.Height = 15;

            //MapIcon MapIcon1 = new MapIcon();
            //MapIcon1.Title = "Space Needle";

            var childObj = new Windows.UI.Xaml.Controls.Image
            {
                Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/point.png"))
            };

            Windows.UI.Xaml.Controls.Maps.MapControl.SetLocation(fence, point);
            Windows.UI.Xaml.Controls.Maps.MapControl.SetNormalizedAnchorPoint(fence, new Windows.Foundation.Point(0.5, 0.5));

            mapControl.Children.Add(fence);
        }

        public async Task ShowRoute(MapControl mapControl, Windows.Services.Maps.MapRoute route)
        {
            if (route != null)
            {
                int thread = Environment.CurrentManagedThreadId;

                Windows.Foundation.IAsyncAction action =
                mapControl.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    Windows.UI.Xaml.Controls.Maps.MapRouteView viewOfRoute = new Windows.UI.Xaml.Controls.Maps.MapRouteView(route);
                    viewOfRoute.RouteColor = Windows.UI.Colors.Yellow;
                    viewOfRoute.OutlineColor = Windows.UI.Colors.Black;

                    // Add the new MapRouteView to the Routes collection
                    // of the MapControl.
                    mapControl.Routes.Add(viewOfRoute);

                    // Fit the MapControl to the route.
                    await mapControl.TrySetViewBoundsAsync(
                        route.BoundingBox,
                        null,
                        Windows.UI.Xaml.Controls.Maps.MapAnimationKind.None);
                });

                await action;
            }
        }
    }
}
