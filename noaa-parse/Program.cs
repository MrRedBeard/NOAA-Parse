using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Globalization;

namespace noaa_parse
{
    class Program
    {
        static void Main(string[] args)
        {
            NOAA noaa = new NOAA();
        }
    }

    class NOAA //Handling NOAA Format
    {
        public NOAAForecastJson Forecast { get;set; }
        public List<NOAAAlerts> Alerts { get; set; } //DB Friendly
        public InternalNOAAForecast ParsedForecast = new InternalNOAAForecast(); //DB Friendly

        public bool debug = true;

        public NOAA()
        {
            Forecast = getNOAAForecastJson();
            Alerts = GetAlerts();

            ParsedForecast = ParseForecast();
        }

        public InternalNOAAForecast ParseForecast()
        {
            InternalNOAAForecast inf = new InternalNOAAForecast();

            //Current Observation
            inf.stationObservation.Altimeter = float.Parse(Forecast.currentobservation.Altimeter);
            inf.stationObservation.Barometer = float.Parse(Forecast.currentobservation.SLP);

            string[] dateArr = Forecast.currentobservation.Date.Substring(0, Forecast.currentobservation.Date.Length - 7).Split(' ');
            string date = dateArr[0] + ' ' + dateArr[1] + ' ' + DateTime.Now.Year + ' ' + dateArr[2];
            inf.stationObservation.Date = DateTime.Parse(date);
            
            inf.stationObservation.DewPoint = int.Parse(Forecast.currentobservation.Dewp);
            inf.stationObservation.Elevation = float.Parse(Forecast.currentobservation.elev);
            inf.stationObservation.Icon = Forecast.currentobservation.Weatherimage.Split('.')[0];
            inf.stationObservation.Latitude = float.Parse(Forecast.currentobservation.latitude);
            inf.stationObservation.Longitude = float.Parse(Forecast.currentobservation.longitude);
            inf.stationObservation.RelativeHumidity = int.Parse(Forecast.currentobservation.Relh);
            inf.stationObservation.Temperature = int.Parse(Forecast.currentobservation.Temp);
            inf.stationObservation.Visibility = float.Parse(Forecast.currentobservation.Visibility);
            inf.stationObservation.WeatherText = Forecast.currentobservation.Weather;
            
            if(Forecast.currentobservation.WindChill != null || Forecast.currentobservation.WindChill != "" || Forecast.currentobservation.WindChill != "NA")
            {
                inf.stationObservation.WindChill = null;
            }
            else
            {
                inf.stationObservation.WindChill = int.Parse(Forecast.currentobservation.WindChill);
            }
            
            inf.stationObservation.WindDirection = int.Parse(Forecast.currentobservation.Windd);
            inf.stationObservation.WindGust = int.Parse(Forecast.currentobservation.Gust);
            inf.stationObservation.WindSpeed = int.Parse(Forecast.currentobservation.Winds);

            //InternalNOAAForecast.LocalForecast localForecast = new InternalNOAAForecast.LocalForecast();
            string[] When = Forecast.time.startPeriodName;
            string[] ForecastIcon = Forecast.data.iconLink;
            string[] ForecastText = Forecast.data.text;
            string[] ForecastTitle = Forecast.data.weather;
            string[] Percip = Forecast.data.pop;
            string[] Temp = Forecast.data.temperature;
            string[] TempLabel = Forecast.time.tempLabel;

            for (int i = 0; i < When.Length; i++)
            {
                InternalNOAAForecast.LocalForecast period = new InternalNOAAForecast.LocalForecast();
                period.When = When[i];

                string icon = ForecastIcon[i].Split('/')[ForecastIcon[i].Split('/').Length - 1].Split('.')[0];
                period.Icon = icon;

                period.WeatherText = ForecastText[i];
                period.WeatherTitle = ForecastTitle[i];

                if (Percip[i] != null && Percip[i] != "" && Percip[i] != "NA")
                {
                    period.PercPrecip = int.Parse(Percip[i]);
                }
                else
                {
                    period.PercPrecip = null;
                }

                period.Temperature = int.Parse(Temp[i]);
                period.TemperatureLabel = TempLabel[i];

                inf.localForecast.Add(period);
            }

            return inf;
        }

        public List<NOAAAlerts> GetAlerts()
        {
            List<string> urls = new List<string>();
            List<NOAAAlerts> alerts = new List<NOAAAlerts>();

            for (int i = 0; i < Forecast.data.hazard.Length; i++)
            {
                //Add all hazards
                urls.Add(Forecast.data.hazardUrl[i]);
            }

            List<NOAAAlerts> tempAlerts = new List<NOAAAlerts>();
            foreach (string url in urls)
            {
                tempAlerts.AddRange(getAlertsHtml(url));
            }

            //Prevent Duplicates
            foreach (NOAAAlerts tempAlert in tempAlerts)
            {
                int exists = alerts
                .Where(x => x.Type.ToLower() == tempAlert.Type.ToLower() && x.ExpireDateTime.Value.ToString() == tempAlert.ExpireDateTime.Value.ToString()).ToList().Count;

                if (exists <= 0)
                {
                    alerts.Add(tempAlert);
                }
            }

            return alerts;
        }
        
        public List<NOAAAlerts> getAlertsHtml(string url)
        {
            List<NOAAAlerts> alerts = new List<NOAAAlerts>();

            string webRequestResultText = "";
            HtmlDocument warningHtml = new HtmlDocument();

            if (debug)
            {
                webRequestResultText = @"
  <!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Strict//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd'>
<!-- saved from url=(0079)https://forecast.weather.gov/wwamap/wwatxtget.php?cwa=JAN&wwa=tornado%20warning -->
<html xmlns='http://www.w3.org/1999/xhtml' xml:lang='en'><head><meta http-equiv='Content-Type' content='text/html; charset=UTF-8'>

<title>WWA Summary for Tornado Warning Issued by JAN</title><meta name='title' content='National Weather Service Watch Warning Advisory Summary'>
<meta name='description' content='The National Weather Service is your best source for complete weather forecast and weather related information on the web!'>
<meta name='keywords' content='weather, local weather forecast, local forecast, weather forecasts, local weather, radar, fire weather, center weather service units, JetStream'>
<meta name='rating' content='General'>
<meta name='DC.publisher' content='NWS Southern Region HQ Fort Worth, Texas'>
<meta name='DC.contributor' content='NWS Southern Region HQ Fort Worth, Texas'>
<meta name='DC.rights' content='http://www.weather.gov/disclaimer.php'>
<meta name='DC.author' content='NWS Southern Region HQ Fort Worth, Texas (Dennis Cain and Leon Minton)'>
<meta name='robots' content='index,follow'>
<link rel='STYLESHEET' type='text/css' href='./WWA Summary for Tornado Warning Issued by JAN_files/main_041007.css' title='nws'>
<link rel='STYLESHEET' type='text/css' href='./WWA Summary for Tornado Warning Issued by JAN_files/print_041007.css' title='nws' media='print'>
<link href='https://forecast.weather.gov/images/favicon.ico' rel='shortcut icon'>

<meta name='d41d8cd98f00b204e9800998ecf8427e_lib_detect' id='d41d8cd98f00b204e9800998ecf8427e_lib_detect'><script src='chrome-extension://cgaocdmhkmfnkdkbnckgmpopcbpaaejo/library/libraries.js'></script><script src='chrome-extension://cgaocdmhkmfnkdkbnckgmpopcbpaaejo/content_scripts/lib_detect.js'></script></head>
<body class='nonav670'>
<div class='header670'>
<div class='rightalign'><a href='http://weather.gov/' class='noprint' title='Go to the NWS Homepage'>weather.gov</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</div>
<span class='title_small'>National Weather Service</span><br>
<br>
<span class='title_large'>Watches, Warnings &amp; Advisories</span></div>
<div id='noaalink'><a href='http://www.noaa.gov/'><img src='./WWA Summary for Tornado Warning Issued by JAN_files/noaalink.gif' alt='Go to the NOAA Homepage' width='80' height='80'></a></div>
<div id='nwslink670'><a href='http://www.nws.noaa.gov/'><img src='./WWA Summary for Tornado Warning Issued by JAN_files/noaalink.gif' alt='NWS Homepage' width='80' height='80'></a></div>
<div id='topnav670'><label for='zipcity' class='yellow'>Local weather
forecast by 'City, St' or zip code</label>
<form method='post' action='https://forecast.weather.gov/zipcity.php'>
<div class='searchinput'><input type='text' id='zipcity' name='inputstring' size='10' value='City, St'> <input type='submit' name='Go2' value='Go'></div>
</form>
</div>

<!-- main content -->

<div id='mainnonav670'><div id='content'>
<br><h3>Tornado Warning</h3><hr><pre>Severe Weather Statement
National Weather Service Jackson MS
618 PM CDT Wed Apr 13 2022

LAC065-132330-
/O.CON.KJAN.TO.W.0060.000000T0000Z-220413T2330Z/
Madison LA-
618 PM CDT Wed Apr 13 2022

...A TORNADO WARNING REMAINS IN EFFECT UNTIL 630 PM CDT FOR MADISON
PARISH...

At 618 PM CDT, a severe thunderstorm capable of producing a tornado
was located over Tendal, or 9 miles west of Tallulah, moving east at
60 mph.

HAZARD...Tornado.

SOURCE...Radar indicated rotation.

IMPACT...Flying debris will be dangerous to those caught without
         shelter. Mobile homes will be damaged or destroyed. Damage
         to roofs, windows, and vehicles will occur.  Tree damage is
         likely.

This dangerous storm will be near...
  Tallulah around 625 PM CDT.
  Omega and Mansford around 630 PM CDT.

Other locations impacted by this tornadic thunderstorm include Mound
and Richmond.

PRECAUTIONARY/PREPAREDNESS ACTIONS...

TAKE COVER NOW! Move to a basement or an interior room on the lowest
floor of a sturdy building. Avoid windows. If you are outdoors, in a
mobile home, or in a vehicle, move to the closest substantial shelter
and protect yourself from flying debris.

&amp;&amp;

LAT...LON 3225 9152 3230 9152 3234 9148 3235 9150
      3236 9145 3240 9149 3249 9148 3254 9137
      3255 9112 3248 9111 3244 9102 3236 9100
      3233 9093 3221 9124 3223 9128 3220 9132
      3221 9154 3223 9151 3225 9156
TIME...MOT...LOC 2318Z 249DEG 51KT 3246 9134

TORNADO...RADAR INDICATED
MAX HAIL SIZE...&lt;.75 IN

$$

DC</pre><hr><pre>Severe Weather Statement
National Weather Service Jackson MS
612 PM CDT Wed Apr 13 2022

MSC049-121-132345-
/O.CON.KJAN.TO.W.0061.000000T0000Z-220413T2345Z/
Rankin MS-Hinds MS-
612 PM CDT Wed Apr 13 2022

...A TORNADO WARNING REMAINS IN EFFECT UNTIL 645 PM CDT FOR CENTRAL
RANKIN AND EAST CENTRAL HINDS COUNTIES...

At 612 PM CDT, a severe thunderstorm capable of producing a tornado
was located over Pearl, moving northeast at 40 mph.

HAZARD...Tornado and ping pong ball size hail.

SOURCE...Radar indicated rotation.

IMPACT...Flying debris will be dangerous to those caught without
         shelter. Mobile homes will be damaged or destroyed. Damage
         to roofs, windows, and vehicles will occur.  Tree damage is
         likely.

This dangerous storm will be near...
  Flowood around 615 PM CDT.
  Brandon around 620 PM CDT.
  Fannin around 630 PM CDT.

PRECAUTIONARY/PREPAREDNESS ACTIONS...

TAKE COVER NOW! Move to a basement or an interior room on the lowest
floor of a sturdy building. Avoid windows. If you are outdoors, in a
mobile home, or in a vehicle, move to the closest substantial shelter
and protect yourself from flying debris.

&amp;&amp;

LAT...LON 3222 9021 3232 9024 3248 8988 3229 8981
TIME...MOT...LOC 2312Z 240DEG 33KT 3229 9012

TORNADO...RADAR INDICATED
MAX HAIL SIZE...1.50 IN

$$

86</pre><hr><pre>Severe Weather Statement
National Weather Service Memphis TN
609 PM CDT Wed Apr 13 2022

MSC009-093-139-132330-
/O.CON.KMEG.TO.W.0039.000000T0000Z-220413T2330Z/
Marshall MS-Tippah MS-Benton MS-
609 PM CDT Wed Apr 13 2022

...A TORNADO WARNING REMAINS IN EFFECT UNTIL 630 PM CDT FOR
SOUTHEASTERN MARSHALL...WESTERN TIPPAH AND BENTON COUNTIES...

At 609 PM CDT, a severe thunderstorm producing a tornado was located
near Snow Lake Shores, or 13 miles east of Holly Springs, moving east
at 40 mph.

HAZARD...Tornado.

SOURCE...Radar indicated rotation.

IMPACT...Flying debris will be dangerous to those caught without
         shelter. Mobile homes will be damaged or destroyed. Damage
         to roofs, windows, and vehicles will occur.  Tree damage is
         likely.

Locations impacted include...
Ripley, Blue Mountain, Snow Lake Shores, Canaan, Gravestown, Walnut,
Ashland, Potts Camp, Falkner, Murry, Brody, Spring Hill, New Canaan,
Lake Center, Brownfield, Whitten Town, Pine Grove, Hamilton, Bethel
and Tiplersville.

PRECAUTIONARY/PREPAREDNESS ACTIONS...

TAKE COVER NOW! Move to a storm shelter or an interior room on the
lowest floor of a sturdy building. Avoid windows. If you are
outdoors, in a mobile home, or in a vehicle, move to the closest
substantial shelter and protect yourself from flying debris.

This cluster of thunderstorms is producing tornadoes and widespread
significant wind damage. Do not wait to see or hear the tornado. For
your protection move to an interior room on the lowest floor of a
building.

&amp;&amp;

LAT...LON 3460 8948 3486 8920 3500 8917 3500 8888
      3470 8890
TIME...MOT...LOC 2309Z 263DEG 34KT 3475 8921

TORNADO...RADAR INDICATED
MAX HAIL SIZE...&lt;.75 IN

$$

KRM</pre><hr></div>
<div id='footer670'>
<div id='footer670td2'>U.S. Dept. of Commerce<br>
NOAA National Weather Service<br>
1325 East West Highway<br>
Silver Spring, MD 20910<br>
E-mail: <a href='mailto:w-nws.webmaster@noaa.gov'>w-nws.webmaster@noaa.gov</a><br>
Page last modified: June 2, 2009</div>
<div id='footer670td3'><input type='button' value='Back to previous page' onclick='history.back()'></div>
<div id='footer670td4'>
<ul>
	<li><a href='http://www.weather.gov/disclaimer.php'>Disclaimer</a></li>
	<li><a href='http://www.weather.gov/credits.php'>Credits</a></li>
	<li><a href='http://www.weather.gov/glossary/'>Glossary</a></li>
	<li><a href='http://weather.gov/privacy.php'>Privacy Policy</a></li>
	<li><a href='http://www.weather.gov/admin.php'>About Us</a></li>
	<li><a href='http://www.weather.gov/careers.php'>Career Opportunities</a></li>
</ul>
</div>
<div id='tag670'>NATIONAL WEATHER SERVICE: <span class='italic'>for
Safety, for Work, for Fun</span> - FOR LIFE</div>
</div>
</div>


<div id='vgc^e&gt;fD1649892214499'></div></body></html>";
            }
            else
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "text/html";
                httpWebRequest.Method = "GET";
                httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0";

                HttpWebResponse httpResponse;
                try
                {
                    httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        webRequestResultText = streamReader.ReadToEnd();
                    }
                }
                catch
                {
                    return getAlertsHtml(url);
                }
            }

            warningHtml.LoadHtml(webRequestResultText);

            HtmlNodeCollection documentNodes = warningHtml.DocumentNode.QuerySelector("#content").ChildNodes;

            string alertType = "";
            foreach (HtmlNode documentNode in documentNodes)
            {
                //Get all hazards
                if (documentNode.Name == "h3")
                {
                    alertType = documentNode.InnerText.Trim();
                }

                if ((alertType != null && alertType != "") && documentNode.Name == "pre")
                {
                    NOAAAlerts alert = new NOAAAlerts();
                    alert.Type = alertType;

                    //Parse out expire date time
                    DateTime? expireDateTime = null;
                    try
                    {
                        //ARC011-013-025-039-053-059-069-085-103-117-119-125-147-132215-/ O.CON.KLZK.TO.A.0120.000000T0000Z - 220413T2200Z
                        Regex regexExpireTime = new Regex(@"\/*\.*\.*\.*\.*\S*Z/"); //Regex to match expire datetime
                        string expireDateTimeString = regexExpireTime.Match(documentNode.InnerText).ToString();
                        expireDateTimeString = expireDateTimeString.Substring(expireDateTimeString.LastIndexOf('-') + 1).Replace("Z", "").Replace("/", "");
                        string expireDate = expireDateTimeString.Substring(0, expireDateTimeString.LastIndexOf('T'));
                        string expireTime = expireDateTimeString.Substring(expireDateTimeString.LastIndexOf('T') + 1);
                        expireTime = expireTime.Substring(0, 2) + ":" + expireTime.Substring(2, 2);
                        string expireDateYear = "20" + expireDate.Substring(0, 2);
                        string expireDateMonth = expireDate.Substring(2, 2);
                        string expireDateDay = expireDate.Substring(4, 2);
                        expireDateTimeString = expireDateMonth + "/" + expireDateDay + "/" + expireDateYear + " " + expireTime;
                        expireDateTime = DateTime.Parse(expireDateTimeString).AddHours(-5); //Convert Zulu to CST
                    }
                    catch (Exception) { }

                    alert.ExpireDateTime = expireDateTime;
                    alert.AlertText = documentNode.InnerText;
                    alerts.Add(alert);
                }
            }
            
            return alerts;
        }

        public NOAAForecastJson getNOAAForecastJson()
        {
            //JSON Forecast
            //#.YlcvqcjMKUl //NOAA random string to prevent cache issues // 11 chars
            //string JsonForecastURL = "https://forecast.weather.gov/MapClick.php?lat=34.82407365710183&lon=-92.27942868460316&unit=0&lg=english&FcstType=json";// + "#" + RandomString(11);
            string JsonForecastURL = "https://forecast.weather.gov/MapClick.php?lat=34.8241&lon=-92.2794&unit=0&lg=english&FcstType=json";

            string result = "";

            if (debug)
            {
                result = "{'operationalMode':'Production','srsName':'WGS 1984','creationDate':'2022-04-13T15:36:27-05:00','creationDateLocal':'13 Apr 16:53 pm CDT','productionCenter':'Little Rock, AR','credit':'https://www.weather.gov/lzk','moreInformation':'https://weather.gov','location':{'region':'srh','latitude':'34.82','longitude':'-92.29','elevation':'499','wfo':'LZK','timezone':'C|Y|6','areaDescription':'North Little Rock Airport AR','radar':'KLZK','zone':'ARZ044','county':'ARC119','firezone':'ARZ044','metar':'KLIT'},'time':{'layoutKey':'k-p12h-n15-1','startPeriodName':['This Afternoon','Tonight','Thursday','Thursday Night','Friday','Friday Night','Saturday','Saturday Night','Sunday','Sunday Night','Monday','Monday Night','Tuesday','Tuesday Night','Wednesday'],'startValidTime':['2022-04-13T17:00:00-05:00','2022-04-13T18:00:00-05:00','2022-04-14T06:00:00-05:00','2022-04-14T18:00:00-05:00','2022-04-15T06:00:00-05:00','2022-04-15T18:00:00-05:00','2022-04-16T06:00:00-05:00','2022-04-16T18:00:00-05:00','2022-04-17T06:00:00-05:00','2022-04-17T18:00:00-05:00','2022-04-18T06:00:00-05:00','2022-04-18T18:00:00-05:00','2022-04-19T06:00:00-05:00','2022-04-19T18:00:00-05:00','2022-04-20T06:00:00-05:00'],'tempLabel':['High','Low','High','Low','High','Low','High','Low','High','Low','High','Low','High','Low','High']},'data':{'temperature':['72','41','69','48','77','57','67','50','63','47','65','43','65','46','70'],'pop':['100','60',null,null,null,'40','40','40','40','40','40','20',null,null,null],'weather':['Severe Thunderstorms','Severe Thunderstorms and Breezy then Mostly Clear','Sunny','Mostly Clear','Sunny','Chance T-storms','Chance T-storms','Chance T-storms','Chance T-storms','Chance T-storms','Chance T-storms','Slight Chance T-storms then Mostly Clear','Sunny','Partly Cloudy','Partly Sunny'],'iconLink':['https://forecast.weather.gov/newimages/medium/tsra100.png','https://forecast.weather.gov/DualImage.php?i=hi_ntsra&j=nfew&ip=60','https://forecast.weather.gov/newimages/medium/skc.png','https://forecast.weather.gov/newimages/medium/nfew.png','https://forecast.weather.gov/newimages/medium/few.png','https://forecast.weather.gov/newimages/medium/hi_ntsra40.png','https://forecast.weather.gov/newimages/medium/tsra40.png','https://forecast.weather.gov/newimages/medium/ntsra40.png','https://forecast.weather.gov/newimages/medium/tsra40.png','https://forecast.weather.gov/newimages/medium/ntsra40.png','https://forecast.weather.gov/newimages/medium/hi_tsra40.png','https://forecast.weather.gov/DualImage.php?i=hi_ntsra&j=nfew&ip=20','https://forecast.weather.gov/newimages/medium/few.png','https://forecast.weather.gov/newimages/medium/nsct.png','https://forecast.weather.gov/newimages/medium/bkn.png'],'hazard':['Hazardous Weather Outlook','Wind Advisory'],'hazardUrl':['https://forecast.weather.gov/showsigwx.php?warnzone=ARZ044&amp;warncounty=ARC119&amp;firewxzone=ARZ044&amp;local_place1=North+Little+Rock+Airport+AR&amp;product1=Hazardous+Weather+Outlook','https://forecast.weather.gov/showsigwx.php?warnzone=ARZ044&amp;warncounty=ARC119&amp;firewxzone=ARZ044&amp;local_place1=North+Little+Rock+Airport+AR&amp;product1=Wind+Advisory'],'text':['Showers and thunderstorms. Some of the storms could be severe.  High near 72. Southwest wind around 15 mph.  Chance of precipitation is 100%. New rainfall amounts between a tenth and quarter of an inch, except higher amounts possible in thunderstorms. ','Showers and thunderstorms likely, mainly before 7pm. Some of the storms could be severe.  Mostly cloudy during the early evening, then gradual clearing, with a low around 41. Breezy, with a northwest wind 15 to 20 mph becoming north 10 to 15 mph after midnight. Winds could gust as high as 35 mph.  Chance of precipitation is 60%. New precipitation amounts between a tenth and quarter of an inch, except higher amounts possible in thunderstorms. ','Sunny, with a high near 69. East wind around 5 mph becoming south in the afternoon. ','Mostly clear, with a low around 48. Southeast wind around 5 mph. ','Sunny, with a high near 77. South wind 5 to 15 mph. ','A 40 percent chance of showers and thunderstorms after 8pm.  Partly cloudy, with a low around 57. South wind 5 to 10 mph becoming east northeast after midnight. ','A 40 percent chance of showers and thunderstorms.  Mostly cloudy, with a high near 67. Northeast wind 5 to 10 mph. ','A 40 percent chance of showers and thunderstorms.  Mostly cloudy, with a low around 50. Northeast wind 10 to 15 mph. ','A 40 percent chance of showers and thunderstorms.  Mostly cloudy, with a high near 63. East northeast wind 10 to 15 mph. ','A 40 percent chance of showers and thunderstorms.  Mostly cloudy, with a low around 47.','A 40 percent chance of showers and thunderstorms.  Mostly sunny, with a high near 65.','A 20 percent chance of showers and thunderstorms.  Mostly clear, with a low around 43.','Sunny, with a high near 65.','Partly cloudy, with a low around 46.','Partly sunny, with a high near 70.']},'currentobservation':{'id':'KLIT','name':'Little Rock, Adams Field','elev':'259','latitude':'34.73','longitude':'-92.24','Date':'13 Apr 16:53 pm CDT','Temp':'65','Dewp':'61','Relh':'87','Winds':'15','Windd':'310','Gust':'23','Weather':'Overcast','Weatherimage':'ovc.png','Visibility':'10.00','Altimeter':'1003.0','SLP':'29.62','timezone':'CDT','state':'AR','WindChill':'NA'}}";

                result = result.Replace((char)39, '"');
            }
            else
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(JsonForecastURL);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";
                httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0";

                HttpWebResponse httpResponse;
                try
                {
                    httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        result = streamReader.ReadToEnd();
                    }
                }
                catch
                {
                    return getNOAAForecastJson();
                }
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            NOAAForecastJson json = new NOAAForecastJson();

            try
            {
                json = serializer.Deserialize<NOAAForecastJson>(result);
            }
            catch (Exception)
            {
                throw new Exception("JSON not currently available");
            }

            return json;
        }

        public class NOAAAlerts
        {
            public string Type { get; set; }
            public DateTime? ExpireDateTime { get; set; }
            public string AlertText { get; set; }
        }

        public class NOAAForecastJson
        {
            public string operationalMode { get; set; }
            public string srsName { get; set; }
            public DateTime creationDate { get; set; }
            public string creationDateLocal { get; set; }
            public string productionCenter { get; set; }
            public string credit { get; set; }
            public string moreInformation { get; set; }
            public Location location { get; set; }
            public Time time { get; set; }
            public Data data { get; set; }
            public Currentobservation currentobservation { get; set; }

            public class Location
            {
                public string region { get; set; }
                public string latitude { get; set; }
                public string longitude { get; set; }
                public string elevation { get; set; }
                public string wfo { get; set; }
                public string timezone { get; set; }
                public string areaDescription { get; set; }
                public string radar { get; set; }
                public string zone { get; set; }
                public string county { get; set; }
                public string firezone { get; set; }
                public string metar { get; set; }
            }

            public class Time
            {
                public string layoutKey { get; set; }
                public string[] startPeriodName { get; set; }
                public DateTime[] startValidTime { get; set; }
                public string[] tempLabel { get; set; }
            }

            public class Data
            {
                public string[] temperature { get; set; }
                public string[] pop { get; set; }
                public string[] weather { get; set; }
                public string[] iconLink { get; set; }
                public string[] hazard { get; set; }
                public string[] hazardUrl { get; set; }
                public string[] text { get; set; }
            }

            public class Currentobservation
            {
                public string id { get; set; }
                public string name { get; set; }
                public string elev { get; set; }
                public string latitude { get; set; }
                public string longitude { get; set; }
                public string Date { get; set; }
                public string Temp { get; set; }
                public string Dewp { get; set; }
                public string Relh { get; set; }
                public string Winds { get; set; }
                public string Windd { get; set; }
                public string Gust { get; set; }
                public string Weather { get; set; }
                public string Weatherimage { get; set; }
                public string Visibility { get; set; }
                public string Altimeter { get; set; }
                public string SLP { get; set; }
                public string timezone { get; set; }
                public string state { get; set; }
                public string WindChill { get; set; }
            }
        }

        public string RandomString(int length)
        {
            Random random = new Random();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }

    public class InternalNOAAForecast
    {
        public List<LocalForecast> localForecast { get; set; }
        public StationObservation stationObservation { get; set; }

        public InternalNOAAForecast()
        {
            localForecast = new List<LocalForecast>();
            stationObservation = new StationObservation();
        }


        public class LocalForecast
        {
            public string When { get; set; }
            public string TemperatureLabel { get; set; }
            public int? Temperature { get; set; }
            public int? PercPrecip { get; set; }
            public string WeatherTitle { get; set; }
            public string WeatherText { get; set; }
            public string Icon { get; set; }
        }
        
        public class StationObservation
        {
            public float Elevation { get; set; }
            public float Latitude { get; set; }
            public float Longitude { get; set; }
            public DateTime Date { get; set; }
            public int Temperature { get; set; }
            public string TemperatureLabel { get; set; }
            public int DewPoint { get; set; }
            public int RelativeHumidity { get; set; }
            public int WindSpeed { get; set; }
            public int WindDirection { get; set; }
            public int WindGust { get; set; }
            public string WeatherText { get; set; }
            public string Icon { get; set; } //Image
            public float Visibility { get; set; }
            public float Altimeter { get; set; }
            public float Barometer { get; set; } //SLP

            public int? WindChill { get; set; }
            //Heat Index

        }
    }
    
}





