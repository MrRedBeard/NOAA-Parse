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

namespace noaa_parse
{
    class Program
    {
        static void Main(string[] args)
        {
            NOAA noaa = new NOAA();
        }
    }

    class NOAA
    {
        public NOAAForecastJson Forecast { get;set; }
        public bool debug = true;

        public NOAA()
        {
            Forecast = getNOAAForecastJson();
            GetWarnings();
        }

        public void GetWarnings()
        {
            List<string> urls = new List<string>();

            for (int i = 0; i < Forecast.data.hazard.Length; i++)
            {
                if(Forecast.data.hazard[i].ToLower().Contains("tornado") || Forecast.data.hazard[i].ToLower().Contains("tornado watch") || Forecast.data.hazard[i].ToLower().Contains("hazard"))
                {
                    urls.Add(Forecast.data.hazardUrl[i]);
                }
            }

            foreach (string url in urls)
            {
                string data = getWarningHtml(url);
            }
        }

        public string getWarningHtml(string url)
        {
            string result = "";

            if (debug)
            {
                result = @"
  <!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Strict//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd'>
  <html xmlns='http://www.w3.org/1999/xhtml' xml:lang='en'><head>
  <meta http-equiv='Content-Type' content='text/html; charset=ISO-8859-1' />
  <title>WWA Summary by Location with ARZ044/ARC119/ARZ044 emphasis Tornado Watch</title>
  <meta name='title' content='National Weather Service Watch Warning Advisory Summary' />
  <meta name='description' content='The National Weather Service is your best source for complete weather forecast and weather related information on the web!' />
  <meta name='keywords' content='weather, local weather forecast, local forecast, weather forecasts, local weather, radar, fire weather, center weather service units, JetStream' />
  <meta name='rating' content='General' />
  <meta name='DC.publisher' content='NWS Southern Region HQ Fort Worth, Texas' />
  <meta name='DC.contributor' content='NWS Southern Region HQ Fort Worth, Texas' />
  <meta name='DC.rights' content='http://www.weather.gov/disclaimer.php' />
  <meta name='DC.author' content='NWS Southern Region HQ Fort Worth, Texas (Dennis Cain and Leon Minton)' />
  <meta name='robots' content='index,follow' />
  <link rel='STYLESHEET' type='text/css' href='css/main_041007.css' title='nws' />
  <link rel='STYLESHEET' type='text/css' href='css/print_041007.css' title='nws' media='print' />
  <link href='/images/favicon.ico' rel='shortcut icon' />
  </head>
  <body class='nonav670'>
    <div class='header670'>
    	<div class='rightalign'><a href='http://weather.gov' class='noprint' title='Go to the NWS Homepage'>weather.gov</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</div>
    	<span class='title_small'>National Weather Service</span><br /><br />
    	<span class='title_large'>Watches, Warnings &amp; Advisories</span>
        </div>
        <div id='noaalink'><a href='http://www.noaa.gov'><img src='/images/wtf/noaalink.gif' alt='Go to the NOAA Homepage' width='80' height='80' /></a></div>
        <div id='nwslink670'><a href='http://www.nws.noaa.gov'><img src='/images/wtf/noaalink.gif' alt='NWS Homepage' width='80' height='80' /></a></div>
        <div id='topnav670'>
    	<label for='zipcity' class='yellow'>Local weather forecast by &quot;City, St&quot; or zip code</label> 
    	<form method='post' action='/zipcity.php'>
  	    <div class='searchinput'>
      		<input type='text' id='zipcity' name='inputstring' size='10' value='City, St' /> 
      		<input type='submit' name='Go2' value='Go' />
  	    </div>
    	</form>
    </div><div id='mainnonav670' style='position:relative !important; top:0px !important;'>	<div id='content'><br />4 products issued by NWS for: North Little Rock Airport AR  <!-- AddThis Button BEGIN -->
	<div class='addthis_toolbox addthis_default_style ' style='float:right;'>
	<a href='//www.addthis.com/bookmark.php?v=250&amp;pubid=ra-5127a6364d551d04' class='addthis_button_compact'>Share</a> 
	<span class='addthis_separator'>|</span>
	<a class='addthis_button_preferred_1'></a>
	<a class='addthis_button_preferred_2'></a>
	<a class='addthis_button_preferred_3'></a>
	<a class='addthis_button_preferred_4'></a>
	<a class='addthis_button_preferred_5'></a>
	</div>
	<!-- AddThis Button END -->   <hr /><br /><h3>Tornado Watch</h3><pre>
WATCH COUNTY NOTIFICATION FOR WATCHES 120/122
NATIONAL WEATHER SERVICE LITTLE ROCK AR
221 PM CDT WED APR 13 2022

ARC011-013-019-023-025-039-045-051-053-059-063-067-069-075-085-
097-103-105-109-117-119-121-125-135-145-147-132200-
/O.CON.KLZK.TO.A.0120.000000T0000Z-220413T2200Z/

TORNADO WATCH 120 REMAINS VALID UNTIL 5 PM CDT THIS AFTERNOON FOR
THE FOLLOWING AREAS

IN ARKANSAS THIS WATCH INCLUDES 26 COUNTIES

IN CENTRAL ARKANSAS

FAULKNER              GARLAND               GRANT
LONOKE                PERRY                 PRAIRIE
PULASKI               SALINE                WHITE

IN EASTERN ARKANSAS

JACKSON               LAWRENCE              RANDOLPH
WOODRUFF

IN NORTH CENTRAL ARKANSAS

CLEBURNE              INDEPENDENCE          SHARP

IN SOUTHEAST ARKANSAS

BRADLEY               CLEVELAND             JEFFERSON

IN SOUTHWEST ARKANSAS

CALHOUN               CLARK                 DALLAS
HOT SPRING            OUACHITA              PIKE

IN WESTERN ARKANSAS

MONTGOMERY

THIS INCLUDES THE CITIES OF ARKADELPHIA, ASH FLAT, ATTICA,
AUGUSTA, BATESVILLE, BEEBE, BENTON, BRYANT, CABOT, CAMDEN,
CAVE CITY, CONWAY, COTTON PLANT, DE VALLS BLUFF, DES ARC,
FORDYCE, GLENWOOD, HAMPTON, HARDY, HAZEN, HEBER SPRINGS,
HOT SPRINGS, HOXIE, KINGSLAND, LITTLE ROCK, LONOKE, MALVERN,
MCCRORY, MOUNT IDA, MURFREESBORO, NEWPORT, NORMAN,
NORTH LITTLE ROCK, PERRYVILLE, PINE BLUFF, POCAHONTAS, RISON,
SEARCY, SHERIDAN, THORNTON, WALNUT RIDGE, AND WARREN.

$$

</pre><hr/><br/><h3>Flash Flood Warning</h3><pre>
Flash Flood Warning
ARC045-085-119-125-145-132200-
/O.NEW.KLZK.FF.W.0016.220413T1950Z-220413T2150Z/
/00000.0.ER.000000T0000Z.000000T0000Z.000000T0000Z.OO/

BULLETIN - EAS ACTIVATION REQUESTED
Flash Flood Warning
National Weather Service Little Rock AR
250 PM CDT Wed Apr 13 2022

The National Weather Service in Little Rock has issued a

* Flash Flood Warning for...
  Southeastern Faulkner County in central Arkansas...
  Northwestern Lonoke County in central Arkansas...
  Central Pulaski County in central Arkansas...
  Central Saline County in central Arkansas...
  Southwestern White County in central Arkansas...

* Until 450 PM CDT.

* At 250 PM CDT, Doppler radar indicated thunderstorms producing
  heavy rain across the warned area. Between 1 and 2 inches of rain
  have fallen. The expected rainfall rate is 1 to 2 inches in 1
  hour. Flash flooding is ongoing or expected to begin shortly.

  HAZARD...Flash flooding caused by thunderstorms.

  SOURCE...Radar.

  IMPACT...Flash flooding of small creeks and streams, urban
           areas, highways, streets and underpasses as well as
           other poor drainage and low-lying areas.

* Some locations that will experience flash flooding include...
  Little Rock, North Little Rock, Benton, Sherwood, Jacksonville,
  Cabot, West Little Rock, Maumelle, Bryant, Downtown Little Rock,
  North Little Rock Airport, Little Rock AFB, Southwest Little Rock,
  Beebe, Ward, Haskell, Vilonia, Shannon Hills, Austin in Lonoke
  County and Argenta.

PRECAUTIONARY/PREPAREDNESS ACTIONS...

Turn around, don`t drown when encountering flooded roads. Most flood
deaths occur in vehicles.

&&

LAT...LON 3477 9260 3493 9236 3522 9201 3523 9189
      3519 9183 3510 9182 3502 9187 3488 9202
      3476 9218 3452 9241 3448 9256 3451 9269
      3464 9267

FLASH FLOOD...RADAR INDICATED
EXPECTED RAINFALL RATE...1-2 INCHES IN 1 HOUR

$$

Cavanaugh

</pre><hr/><br/><h3>Hazardous Weather Outlook</h3><pre>
Hazardous Weather Outlook
National Weather Service Little Rock AR
404 AM CDT Wed Apr 13 2022

ARZ004>008-014>017-024-025-031>034-039-042>047-052>057-062>069-
103-112-113-121>123-130-137-138-140-141-203-212-213-221>223-230-
237-238-240-241-313-340-341-141000-
Marion-Baxter-Fulton-Sharp-Randolph-Stone-Izard-Independence-
Lawrence-Cleburne-Jackson-Conway-Faulkner-White-Woodruff-Perry-
Garland-Saline-Pulaski-Lonoke-Prairie-Monroe-Pike-Clark-
Hot Spring-Grant-Jefferson-Arkansas-Dallas-Cleveland-Lincoln-
Desha-Ouachita-Calhoun-Bradley-Drew-Boone County Except Southwest-
Newton County Higher Elevations-Searcy County Lower Elevations-
Southern Johnson County-Southern Pope County-
Southeast Van Buren County-Western and Northern Logan County-
Northern Scott County-Northwest Yell County-
Polk County Lower Elevations-
Central and Eastern Montgomery County-
Boone County Higher Elevations-Newton County Lower Elevations-
Northwest Searcy County Higher Elevations-
Johnson County Higher Elevations-Pope County Higher Elevations-
Van Buren County Higher Elevations-
Southern and Eastern Logan County-
Central and Southern Scott County-Yell Excluding Northwest-
Northern Polk County Higher Elevations-
Northern Montgomery County Higher Elevations-
Eastern, Central, and Southern Searcy County Higher Elevations-
Southeast Polk County Higher Elevations-
Southwest Montgomery County Higher Elevations-
404 AM CDT Wed Apr 13 2022

This Hazardous Weather Outlook is for a Large Part of Arkansas.

.DAY ONE...Today and Tonight

Showers and thunderstorms continue in the forecast today and
tonight as a cold front moves through the state. Some strong to
severe thunderstorms will be possible today into the evening.
Damaging winds and large hail will be possible and a few tornadoes
cannot be ruled out. Mid morning through early evening will be
the best timing for severe thunderstorms. Heavy rainfall may also
be seen. With gusty south winds today up to 35 mph, caution is
advised on area lakes and rivers.

Expect the threat for strong to severe thunderstorms to decrease
mid evening.

.DAYS TWO THROUGH SEVEN...Thursday Through Tuesday

Some patchy frost may be seen Thursday morning. Otherwise...expect
the threat for hazardous weather to remain low into Friday morning.

Showers and thunderstorms return to the forecast late Friday and
through the weekend as a slow moving front moves through the state.
While a few strong to severe thunderstorms could be seen over the
weekend...widespread and organized severe weather is not anticipated
at this time. Given the slow  moving nature of the front...some
locally heavy rainfall could be seen over the weekend...which may
lead to an isolated flash flood threat.

Expect the threat for hazardous weather to become low by early next
week.

.Spotter Information Statement...

Spotter activation may be needed each day through Wednesday
evening.

&&

Visit NWS Little Rock on the web. Go to http://weather.gov/lzk.

$$

51/62

</pre><hr/><br/><h3>Wind Advisory</h3><pre>
URGENT - WEATHER MESSAGE
National Weather Service Little Rock AR
334 AM CDT Wed Apr 13 2022

ARZ016-025-033-034-044>047-056-057-064-065-132100-
/O.NEW.KLZK.WI.Y.0004.220413T1500Z-220414T0000Z/
Independence-Jackson-White-Woodruff-Pulaski-Lonoke-Prairie-Monroe-
Jefferson-Arkansas-Lincoln-Desha-
Including the cities of Batesville, Newport, Searcy, Beebe,
Augusta, McCrory, Cotton Plant, Little Rock, North Little Rock,
Cabot, Lonoke, Des Arc, Hazen, De Valls Bluff, Brinkley,
Clarendon, Pine Bluff, Stuttgart, De Witt, Star City, Gould,
Dumas, and McGehee
334 AM CDT Wed Apr 13 2022

...WIND ADVISORY IN EFFECT FROM 10 AM THIS MORNING TO 7 PM CDT
THIS EVENING...

* WHAT...South winds 15 to 25 mph with gusts up to 35 mph
  expected.

* WHERE...Portions of central, eastern, north central and
  southeast Arkansas.

* WHEN...From 10 AM this morning to 7 PM CDT this evening.

* IMPACTS...Gusty winds could blow around unsecured objects.
  Tree limbs could be blown down and a few power outages may
  result.

PRECAUTIONARY/PREPAREDNESS ACTIONS...

Use extra caution when driving, especially if operating a high
profile vehicle. Secure outdoor objects.

&&

$$

51

</pre><hr/><br/></div></div>    <div id='footer670'>
  	<div id='footer670td2'>
  	    U.S. Dept. of Commerce<br />
  	    NOAA National Weather Service<br />
  	    1325 East West Highway<br />
  	    Silver Spring, MD 20910<br />
  	    E-mail: <a href='mailto:w-nws.webmaster@noaa.gov'>w-nws.webmaster@noaa.gov</a><br />
  	    Page last modified: May 16, 2007
  	</div>
  	<div id='footer670td3'><input type='button' value='Back to previous page' onclick='history.back()' /></div>
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
  	 <div id='tag670'>NATIONAL WEATHER SERVICE: <span class='italic'>for Safety, for Work, for Fun</span> - FOR LIFE</div>
    </div>

  </body>
</html>";
                HtmlDocument warningHtml = new HtmlDocument();
                warningHtml.LoadHtml(result);

                var document = warningHtml.DocumentNode.QuerySelector("#content");

                var headers = document.QuerySelectorAll("h3").ToList();
                var warning = document.QuerySelectorAll("pre").ToList();

                for (int i = 0; i < headers.Count(); i++)
                {
                    if(headers[i].InnerText.ToLower().Contains("tornado"))
                    {
                        Console.WriteLine(headers[i].InnerText);

                        var x = warning[i].InnerText.Split('\r');

                        Regex regexExpireTime = new Regex(@"\/*\.*\.*\.*\.*\S*Z/"); //Regex to match expire datetime
                        var xx = regexExpireTime.Match(warning[i].InnerText);
                    }
                }







                //Parse Expire Time
                //string expireDateTimeString = "ARC011-013-025-039-053-059-069-085-103-117-119-125-147-132215-/ O.CON.KLZK.TO.A.0120.000000T0000Z - 220413T2200Z";
                //expireDateTimeString.LastIndexOf('-');
                //expireDateTimeString = expireDateTimeString.Substring(expireDateTimeString.LastIndexOf('-') + 2);
                //string expireDate = expireDateTimeString.Substring(0, expireDateTimeString.LastIndexOf('T'));
                //string expireTime = expireDateTimeString.Substring(expireDateTimeString.LastIndexOf('T') + 1).Replace("Z", "");

                //expireTime = expireTime.Substring(0, 2) + ":" + expireTime.Substring(2, 2);

                //string expireDateYear = "20" + expireDate.Substring(0, 2);
                //string expireDateMonth = expireDate.Substring(2, 2);
                //string expireDateDay = expireDate.Substring(4, 2);

                //string newDateString = expireDateMonth + "/" + expireDateDay + "/" + expireDateYear + " " + expireTime;

                //DateTime expireDateTime = DateTime.Parse(newDateString).AddHours(-5);
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
                        result = streamReader.ReadToEnd();
                    }

                    HtmlDocument warningHtml = new HtmlDocument();
                    warningHtml.LoadHtml(result);

                    // ARC011-013-025-039-053-059-069-085-103-117-119-125-147-132215-/ O.CON.KLZK.TO.A.0120.000000T0000Z - 220413T2200Z /
                    // 220413T2200Z
                    //        2200Z 10pm is 5pm in CST
                    //DateTimeOffset.Parse(string).UtcDateTime


                    //h3
                    //pre

                    //"NORTH LITTLE ROCK"
                    //TORNADO WATCH
                    //TORNADO WARNING
                }
                catch
                {
                    return getWarningHtml(url);
                }
            }

            return result;
        }

        public NOAAForecastJson getNOAAForecastJson()
        {
            //JSON Forecast
            //#.YlcvqcjMKUl //NOAA random string to prevent cache issues // 11 chars
            string JsonForecastURL = "https://forecast.weather.gov/MapClick.php?lat=34.82407365710183&lon=-92.27942868460316&unit=0&lg=english&FcstType=json" + "#" + RandomString(11);

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
            NOAAForecastJson json = serializer.Deserialize<NOAAForecastJson>(result);

            return json;
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
}




