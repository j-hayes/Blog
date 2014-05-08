using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Amazon.SimpleEmail.Model;
using HtmlAgilityPack;

namespace Jackson.Home.Helpers
{
    public static class DailyReadingsScraper
    {
        public static string GetReadings()
        {
            return GetReadings(DateTime.Now);
        }

        public static string GetReadings(DateTime dateTime)
        {
            string date = "";
            date += dateTime.ToString("MM:dd:yy");
            date = date.Replace(":", "");


            WebClient w = new WebClient();
            string s = w.DownloadString("http://www.usccb.org/bible/readings/" + date + ".cfm");


            var htmldoc = new HtmlDocument();
            var encoding =  htmldoc.DetectEncodingHtml(s);
            

            htmldoc.LoadHtml(s);

            if (htmldoc.ParseErrors != null && htmldoc.ParseErrors.Any())
            {
                //handle errors
            }

                if (htmldoc.DocumentNode != null)
                {
                    HtmlNode bodyNode = htmldoc.DocumentNode.SelectSingleNode("//div[@id='CS_Element_maincontent']");
                    string returnString = bodyNode.WriteTo();

                    returnString = returnString.Replace("â€œ", "\"");
                    returnString = returnString.Replace("â€", "\"");
                    return returnString;
                }
            


            return null;
        }
    }
}