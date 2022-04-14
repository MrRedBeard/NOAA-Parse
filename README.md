# NOAA-Parse
c# Console App to parse NOAA Forecast &amp; Tornado Watch Warnings (Alerts)

See Program.cs to review code

## Json Forecast

Parses out forecast in DB friendly class format

https://forecast.weather.gov/MapClick.php?lat=34.8241&lon=-92.2794&unit=0&lg=english&FcstType=json

## Hazards Alerts 

Parses H3 and PRE html elements. Grabs title and expiration time

https://forecast.weather.gov/showsigwx.php?warnzone=ARZ044&warncounty=ARC119&firewxzone=ARZ044&local_place1=North+Little+Rock+Airport+AR&product1=Tornado+Watch
