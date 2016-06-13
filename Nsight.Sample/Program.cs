using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nsight.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var telemetry = InitializeTelemetry();

            TestTelemetryAsync(telemetry).Wait();
        }

        /// <summary>
        /// Initializes telemetry.
        /// </summary>
        private static Nsight.Core.ITelemetryProvider InitializeTelemetry()
        {
            throw new NotImplementedException("TODO: Configure Piwik API");

            //  Configure Piwik API endpoint.
            var piwikApiConfig = new Nsight.Piwik.Api.PiwikApiOptions()
            {
                SiteId = 0,                                                     //  Site ID in your Piwik installation (a number).
                Endpoint = new Uri("https://www.example.com/piwik.php"),        //  An endpoint to your Piwik setup.
                SwallowExceptions = true                                        //  Prefer to swallow exceptions, you don't want to crash your app due to the problems with Piwik.
            };

            //  Initialize telemetry provider object that provides telemetry facilities for your code.
            var telemetry = new Nsight.Core.Impl.TelemetryProvider();

            throw new NotImplementedException("TODO: Configure Piwik sink");

            //  Configure Piwik API sink for activity telemetry.
            var piwikSink = new Nsight.Piwik.Sink.PiwikSink(new Nsight.Piwik.Sink.PiwikSinkOptions()
            {
                Api = new Nsight.Piwik.Api.PiwikApi(piwikApiConfig),
                AppHostName = "yourapp",                                        //  A "host name" that will represent your app. Piwik will operate on "app://yourapp/url" URLs.
                TelemetrySource = telemetry
            });

            return telemetry;
        }

        /// <summary>
        /// Makes asynchronous telemetry calls.
        /// </summary>
        /// <param name="telemetry">Telemetry provider instance.</param>
        private static async Task TestTelemetryAsync(Nsight.Core.ITelemetryProvider telemetry)
        {
            //  Provide information about user environment. May happen at any point in time.
            await telemetry.Activity.SetEnvironmentInfoAsync(new Nsight.Core.EnvironmentInfo()
            {
                DeviceName = "Test PC Make and Model",
                DeviceType = "Desktop",
                OperatingSystem = Environment.OSVersion.VersionString,
                DeviceScreen = new Nsight.Core.ScreenResolution()
                {
                    Width = 1920,
                    Height = 1080,
                    Dpi = 96
                }
            });

            var session = new Nsight.Core.SessionInfo()
            {
                UniqueVisitorId = Guid.NewGuid().ToString(),                    //  An ID that uniquely identifies user in your app.
                //UserId = "some@userid.com",                                   //  You may choose to provide another kind of identifier, but it's not recommended, because it might be PII.
                FirstVisit = DateTimeOffset.Now.AddDays(-1),                    //  Timestamp of a first user visit/session. Null, if user had never visited.
                LastVisit = DateTimeOffset.Now.AddHours(-3),                    //  Timestamp of a last user visit/session.
                VisitsCount = 2                                                 //  # of visits/sessions.
            };

            //  Optionally start session.
            await telemetry.Activity.BeginSessionAsync(session);

            await telemetry.Activity.ReportViewAsync(new Nsight.Core.ViewInfo()
            {
                AbsolutePath = "/main/view",
                Title = "Main View",
                Time = TimeSpan.FromMilliseconds(150)                           //  Time it took to generate the view. Null, if unknown.
            });

            await telemetry.Activity.ReportActionAsync(new Nsight.Core.ActionInfo()
            {
                Name = "Test",
                Category = "Videos",
                Verb = "Play"
            });

            //  Optionally complete session.
            await telemetry.Activity.EndSessionAsync(session);
        }
    }
}
