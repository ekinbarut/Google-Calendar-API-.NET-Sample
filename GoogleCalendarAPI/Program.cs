using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util;
using Google.Apis.Util.Store;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleCalendarAPI
{
    class Program
    {
        static string[] Scopes = { CalendarService.Scope.Calendar };
        static string ApplicationName = "";
        static string serviceAccountName = "";
        static string publicCalendarId = "tr.Turkish#holiday@group.v.calendar.google.com";
        static string scvAccountKey = "notasecret"; //default key by google

        static void Main(string[] args)
        {

            try
            {
                #region [ Google User Creds ]

                UserCredential userCredential;

                using (var stream =
                    new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
                {
                    // The file token.json stores the user's access and refresh tokens, and is created
                    // automatically when the authorization flow completes for the first time.
                    string credPath = "token.json";
                    userCredential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                }

                #endregion

                // Create Google Calendar API service.
                var sservice = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = userCredential,
                    ApplicationName = ApplicationName,
                });

                var calendar = sservice.Events.List(publicCalendarId);

                //only get 2020 events
                calendar.TimeMin = new DateTime(2019, 12, 31);
                calendar.TimeMax = new DateTime(2021, 01, 01);

                var holidayCalendar = calendar.Execute();
                var holidayData = holidayCalendar.Items.ToList();

                #region [ Google Service Account Creds ]

                var certificate = new X509Certificate2(@"key.p12", scvAccountKey, X509KeyStorageFlags.Exportable);

                ServiceAccountCredential svcAccCredential = new ServiceAccountCredential(
                   new ServiceAccountCredential.Initializer(serviceAccountName)
                   {
                       Scopes = Scopes
                   }.FromCertificate(certificate));

                var service = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = svcAccCredential,
                    ApplicationName = ApplicationName,
                });

                #endregion

                var defaultCalendar = service.CalendarList.List().Execute().Items.FirstOrDefault();

                foreach (var day in holidayData)
                {
                    var holidayEvent = service.Events.Insert(new Event()
                    {
                        Created = DateTime.UtcNow,
                        Start = new EventDateTime()
                        {
                            Date = day.Start.Date
                        },
                        End = new EventDateTime()
                        {
                            Date = day.End.Date
                        },
                        Summary = "Holiday",
                        Location = "TR",
                        Description = day.Summary,
                    }, defaultCalendar.Id);

                    var eventResponse = holidayEvent.Execute();
                }
            }

            catch (Exception ex)
            {
                throw;
            }

        }

    }
}
