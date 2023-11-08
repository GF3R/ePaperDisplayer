namespace EPaper.Web.Core.Models.Desk.iCal
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net.Http;
    using System.Net.NetworkInformation;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public class IcalCalendar
    {
        private const string CalendarParameterPattern = "BEGIN:VCALENDAR\\r\\n(.+?)\\r\\nBEGIN:VEVENT";
        private const RegexOptions CalendarParameterRegexOptions = RegexOptions.Singleline;

        public const string vEventPattern = "(BEGIN:VEVENT.+?END:VEVENT)";
        public const RegexOptions vEventRegexOptions = RegexOptions.Singleline;

        public string Source { get; set; }
        public CalendarParameters Parameters { get; set; }
        public List<IcalEvent> Events { get; set; } = new List<IcalEvent>();

        public IcalCalendar(string source)
        {
            Source = source;
            Match parameterMatch = Regex.Match(source, CalendarParameterPattern, CalendarParameterRegexOptions);
            string parameterString = parameterMatch.Groups[1].ToString();
            Parameters = new CalendarParameters(parameterString);
            foreach (Match vEventMatch in Regex.Matches(source, vEventPattern, vEventRegexOptions))
            {
                string vEventString = vEventMatch.Groups[1].ToString();
                this.Events.Add(new IcalEvent(vEventString));
            }
        }

        public static async Task<IcalCalendar> FromUri(string uri)
        {
            // load ics from uri
            return new IcalCalendar(await GetIcsFromUri(uri));
        }

        private static async Task<string> GetIcsFromUri(string icsUrl)
        {
            // using (var httpClient = new HttpClient())
            // {
            //     return await httpClient.GetStringAsync(icsUrl);
            // }
            return await System.IO.File.ReadAllTextAsync(icsUrl);
        }

        public class IcalEvent
        {
            private const string vEventContentPattern = "BEGIN:VEVENT\\r\\n(.+)\\r\\nEND:VEVENT";
            private const RegexOptions vEventContentRegexOptions = RegexOptions.Singleline;
            private const string ContentLinePattern = "(.+?):(.+?)(?=\\r\\n[A-Z]|$)";
            private const RegexOptions ContentLineTRegexOptions = RegexOptions.Singleline;
            private const string startKey = "DTSTART";
            private const string endKey = "DTEND";
            private const string format = "yyyyMMdd'T'HHmmss";

            public DateTimeOffset Start { get; set; }
            public DateTimeOffset End { get; set; }
            public Dictionary<string, ContentLine> ContentLines { get; set; }

            public IcalEvent(string source)
            {
                Match contentMatch = Regex.Match(source, vEventContentPattern, vEventContentRegexOptions);
                string content = contentMatch.Groups[1].ToString();
                MatchCollection matches = Regex.Matches(content, ContentLinePattern, ContentLineTRegexOptions);
                ContentLines = new Dictionary<string, ContentLine>();
                foreach (Match match in matches)
                {
                    string contentLineString = match.Groups[0].ToString();
                    ContentLine contentLine = new ContentLine(contentLineString);
                    ContentLines[contentLine.Name] = contentLine;
                }

                Start = DateTimeOffset.ParseExact(ContentLines[startKey].Value, format, CultureInfo.InvariantCulture);
                End = DateTimeOffset.ParseExact(ContentLines[endKey].Value, format, CultureInfo.InvariantCulture);
            }
        }

        public class CalendarParameters : Dictionary<string, CalendarParameter>
        {
            private const string ParameterPattern = "(.+?):(.+?)(?=\\r\\n[A-Z]|$)";
            private const RegexOptions ParameteRegexOptions = RegexOptions.Singleline;

            public CalendarParameters(string source)
            {
                MatchCollection parametereMatches = Regex.Matches(source, ParameterPattern, ParameteRegexOptions);
                foreach (Match parametereMatch in parametereMatches)
                {
                    string parameterString = parametereMatch.Groups[0].ToString();
                    CalendarParameter calendarParameter = new CalendarParameter(parameterString);
                    this[calendarParameter.Name] = calendarParameter;
                }
            }
        }

        public class CalendarParameter
        {
            private const string NameValuePattern = "(.+?):(.+)";

            public string Name { get; set; }
            public string Value { get; set; }

            public CalendarParameter(string source)
            {
                string unfold = ContentLine.UnfoldAndUnescape(source);
                Match nameValueMatch = Regex.Match(unfold, NameValuePattern);
                Name = nameValueMatch.Groups[1].ToString().Trim();
                Value = nameValueMatch.Groups[2].ToString().Trim();
            }

        }

        public class ContentLine
        {
            private const string ContentLineContentPattern = "(.+?)((;.+?)*):(.+)";
            private const RegexOptions ContentLineContentRegexOptions = RegexOptions.Singleline;

            public string Name { get; set; }
            public string Value { get; set; }
            public ContentLineParameters Parameters { get; set; }

            public ContentLine(string source)
            {
                source = UnfoldAndUnescape(source);
                Match match = Regex.Match(source, ContentLineContentPattern, ContentLineContentRegexOptions);

                Name = match.Groups[1].ToString().Trim();
                Parameters = new ContentLineParameters(match.Groups[2].ToString());
                Value = match.Groups[4].ToString().Trim();
            }

            public static string UnfoldAndUnescape(string s)
            {
                string unfold = Regex.Replace(s, "(\\r\\n )", "");
                string unescaped = Regex.Unescape(unfold);
                return unescaped;
            }

        }

        public class ContentLineParameters : Dictionary<string, ContentLineParameter>
        {
            private const string ParameterPattern = "([^;]+)(?=;|$)";

            public ContentLineParameters(string source)
            {
                MatchCollection matches = Regex.Matches(source, ParameterPattern);
                foreach (Match match in matches)
                {
                    ContentLineParameter contentLineParameter = new ContentLineParameter(match.Groups[1].ToString());
                    this[contentLineParameter.Name] = contentLineParameter;
                }
            }

        }

        public class ContentLineParameter
        {
            private const string NameValuePattern = "(.+?)=(.+)";
            private const string ValueListPattern = "([^,]+)(?=,|$)";

            public string Name { get; set; }
            public List<string> Values { get; set; } = new List<string>();

            public ContentLineParameter(string source)
            {
                Match match = Regex.Match(source, NameValuePattern);
                Name = match.Groups[1].ToString().Trim();
                string valueString = match.Groups[2].ToString();
                MatchCollection matches = Regex.Matches(valueString, ValueListPattern);
                foreach (Match paramMatch in matches)
                    Values.Add(paramMatch.Groups[1].ToString().Trim());
            }

        }
    }
}
