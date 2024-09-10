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
            
            var site = Sitecore.Context.Site;
            Sitecore.Diagnostics.Log.Warn($"Site Sitename: {site?.Name}, Host={site?.HostName}", this);
            var page = rootItem.Paths.Path;


            
            Sitecore.Diagnostics.Log.Warn("Root item found: " + rootItem, this);
            stopWatch.Stop();
            Sitecore.Diagnostics.Log.Warn("Finishing up. Total elapsed time: " + stopWatch.Elapsed, this);
            Sitecore.Diagnostics.Log.Warn("Custom settings value: " + Sitecore.Configuration.Settings.GetSetting("CustomSettingViaEnv", "No values"), this);
        }
    }
}