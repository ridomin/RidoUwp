using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Media.Imaging;

namespace RidoUwp.Controls
{
    public sealed class  AboutThisAppViewModel
    {
        public Package Package => Package.Current;
        public BitmapImage AppLogo => new BitmapImage(Package.Current.Logo);
        public string VersionString => string.Format("{0}.{1}.{2}.{3}",
                                            Package.Current.Id.Version.Major,
                                            Package.Current.Id.Version.Minor,
                                            Package.Current.Id.Version.Build,
                                            Package.Current.Id.Version.Revision);
        public string MyVersion => GetThisControlVersion();

        private string GetThisControlVersion()
        {
            return this.GetType().AssemblyQualifiedName;
            //AssemblyName an = a.GetName();
            //return $"{an.Name} {an.Version.Major}.{an.Version.Minor}.{an.Version.Revision}.{an.Version.Build}";
        }

        public string AppVersion => GetAppVersion();

        private string GetAppVersion()
        {
            return "unavailable with this version of the platform";
        }

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
            var res = "Store API not available ";
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Services.Store.StoreContext"))
            {
                var ctx = Windows.Services.Store.StoreContext.GetDefault();
                var lic = ctx.GetStoreProductForCurrentAppAsync().AsTask().Result;

                if (lic == null)
                {
                    res = "License is null";
                }
                else if (lic.Product == null)
                {
                    res = "License Product is null:" + lic.ExtendedError.Message;
                }
                else
                {
                    res = lic.Product.LinkUri.ToString();
                }
            }
            return res;
        }

        public string InstalledOn => SafeInstalledOn();
        
        private string SafeInstalledOn()
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
