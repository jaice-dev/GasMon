using System.Collections.Generic;
using System.Linq;
using GasMon.Models;

namespace GasMon
{
    public class LocationChecker
    {
        private readonly IEnumerable<string> _validLocationsIds;

        public LocationChecker(IEnumerable<Location> validLocations)
        {
            _validLocationsIds = validLocations.Select(location => location.id);
        }

        
        
        public bool LocationIsValid(Notification notification)
        {
            return _validLocationsIds.Contains(notification.locationId);
        }
    }
}