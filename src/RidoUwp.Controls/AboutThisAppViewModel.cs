using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Media.Imaging;

namespace RidoUwp.Controls
{
    public class AboutThisAppViewModel
    {
        public Package Package => Package.Current;
        public BitmapImage AppLogo => new BitmapImage(Package.Current.Logo);
        public string VersionString => string.Format("{0}.{1}.{2}.{3}", 
                                            Package.Current.Id.Version.Major, 
                                            Package.Current.Id.Version.Minor, 
                                            Package.Current.Id.Version.Build, 
                                            Package.Current.Id.Version.Revision);
        public string MyVersion => this.GetType().AssemblyQualifiedName;

        public string Metadata
        {
            get
            {
                XDocument doc = XDocument.Load("AppxManifest.xml", LoadOptions.None);
                var xname = XNamespace.Get("http://schemas.microsoft.com/developer/appx/2015/build");
                var metadata = doc.Descendants(xname + "Metadata").First();
                var res = new StringBuilder();
                foreach (XElement n in metadata.Elements())
                {
                    res.Append(n.Attribute("Name").Value + ":");
                    res.Append(n.Attribute("Value")?.Value);
                    res.Append(n.Attribute("Version")?.Value + "\r\n");
                }
                return res.ToString();
            }
        }

        public string StoreInfo => GetStoreInfo();

        public static string GetStoreInfo()
        {
            var ctx = Windows.Services.Store.StoreContext.GetDefault();
            if (ctx==null)
            {
                return "Context is null";
            }
            if (ctx.User == null)
            {
                return "Context.User is null";
            }
            return ctx.User.AuthenticationStatus.ToString();

        }

        public string InstalledOn
        {
            get
            {
                return SafeInstalledOn();
            }
        }

        private static string SafeInstalledOn()
        {
            DateTimeOffset installed;
            try
            {
                installed = Package.Current.InstalledDate;
            }
            catch (Exception)
            {

                installed = DateTime.Now;
            }
            return installed.ToLocalTime().ToString();
        }


    }
}
