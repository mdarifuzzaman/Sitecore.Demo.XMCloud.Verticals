using Sitecore.Publishing;
using Sitecore.Sites;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace XmCloudSXAStarter.Events.PublishEnd
{
    public class SampleEvent
    {
        public IItemSiteResolver _siteResolver;
        public SampleEvent(IItemSiteResolver itemSiteResolver) { 
            _siteResolver = itemSiteResolver;
        }
        public void OnPublishEnd(object sender, EventArgs args)
        {
            var stopWatch = Stopwatch.StartNew();
            Sitecore.Diagnostics.Log.Audit("SampleEvent -> OnPublishEnd: ", this);
            var sitecoreArgs = args as Sitecore.Events.SitecoreEventArgs;
            if (sitecoreArgs == null)
                return;

            var publisher = sitecoreArgs.Parameters[0] as Publisher;
            if(publisher == null) return;
            var rootItem = publisher.Options.RootItem;

            var siteObject = _siteResolver.ResolveSite(rootItem);
            if (siteObject == null)
            {
                Sitecore.Diagnostics.Log.Warn("Siteobject found: ", this);
            }
            else
            {
                Sitecore.Diagnostics.Log.Warn($"Siteobject Sitename: {siteObject.Name}, Host={siteObject.HostName}" , this);
            }
            
            var site = Sitecore.Context.Site.Name;
            var page = rootItem.Paths.Path;


            
            Sitecore.Diagnostics.Log.Warn("Root item found: " + rootItem, this);
            stopWatch.Stop();
            Sitecore.Diagnostics.Log.Warn("Finishing up. Total elapsed time: " + stopWatch.Elapsed, this);
            Sitecore.Diagnostics.Log.Warn("Custom settings value: " + Sitecore.Configuration.Settings.GetSetting("CustomSettingViaEnv", "No values"), this);
        }
    }
}