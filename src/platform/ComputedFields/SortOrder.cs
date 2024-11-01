using Sitecore.ContentSearch.ComputedFields;
using Sitecore.ContentSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;

namespace XmCloudSXAStarter.ComputedFields
{
    public class SortOrder : AbstractComputedIndexField
    {
        public override object ComputeFieldValue(IIndexable indexable)
        {
            Item item = (Item)(indexable as SitecoreIndexableItem);
            return (item == null) ? (object)null : (object)item[Sitecore.FieldIDs.Sortorder];
        }
    }
}