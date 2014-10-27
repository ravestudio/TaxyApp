using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxyApp.Core.Managers
{
    public class ManagerFactory : TaxyApp.Core.Singleton<ManagerFactory>
    {
        private LocationManager locationMG = null;
        private MapPainter mapPainter = null;

        public LocationManager GetLocationManager()
        {
            if (this.locationMG == null)
            {
                this.locationMG = new LocationManager();
            }

            return this.locationMG;
        }

        public MapPainter GetMapPainter()
        {
            if (this.mapPainter == null)
            {
                this.mapPainter = new MapPainter();
            }

            return this.mapPainter;
        }
    }
}
