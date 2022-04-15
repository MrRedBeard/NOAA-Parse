**Status:** Initial working version. Not wired to a database yet.

# NOAA-Parse
c# Console App to parse NOAA Forecast & Hazard Alerts

See Program.cs to review code

## Want to get data for your location?

Change Program.cs line 444 lat & lon. Keep decimal to 4 or less.

"https://forecast.weather.gov/MapClick.php?lat=34.8241&lon=-92.2794&unit=0&lg=english&FcstType=json";

## Debug Mode

Change Program.cs line 29

public bool debug = true;

## Json Forecast

Parses out forecast in DB friendly class format

https://forecast.weather.gov/MapClick.php?lat=34.8241&lon=-92.2794&unit=0&lg=english&FcstType=json

## Hazards Alerts 

Parses H3 and PRE html elements. Grabs title and expiration time

https://forecast.weather.gov/showsigwx.php?warnzone=ARZ044&warncounty=ARC119&firewxzone=ARZ044&local_place1=North+Little+Rock+Airport+AR&product1=Tornado+Watch
