using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomBooking.Cofiguration
{
    using System.IO;
    using System.Web.Script.Serialization;

    public class ConfigService
    {
        public AppSettings Load()
        {
            string path = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "appsettings.json");

            if (!File.Exists(path))
                return new AppSettings();

            string json = File.ReadAllText(path);

            JavaScriptSerializer serializer =
                new JavaScriptSerializer();

            AppSettings settings =
                serializer.Deserialize<AppSettings>(json);

            return settings ?? new AppSettings();
        }
    }
}
