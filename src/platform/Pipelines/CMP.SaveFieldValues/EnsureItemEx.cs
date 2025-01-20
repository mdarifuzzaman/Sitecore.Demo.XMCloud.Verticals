using Sitecore.Abstractions;
using Sitecore.Connector.CMP;
using Sitecore.Connector.CMP.Helpers;
using Sitecore.Connector.CMP.Pipelines.ImportEntity;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace XmCloudSXAStarter.Pipelines.CMP.SaveFieldValues
{
    public class EnsureItemEx : ImportEntityProcessor
    {
        private static CmpSettings _settings;
        private readonly BaseFactory _factory;
        private readonly CmpHelper _cmpHelper;

        public EnsureItemEx(
          BaseFactory factory,
          BaseLog logger,
          CmpSettings cmpSettings,
          CmpHelper cmpHelper)
          : base(logger, cmpSettings)
        {
            this._factory = factory;
            EnsureItemEx._settings = cmpSettings;
            this._cmpHelper = cmpHelper;
        }

        public override void Process(ImportEntityPipelineArgs args, BaseLog logger)
        {
            if (args.Item != null)
            {
                if (args.EntityDefinition == null || string.IsNullOrEmpty(args.EntityDefinition.Name))
                    return;
            }
            else
            {
                if (args.EntityMappingItem == null)
                    args.EntityMappingItem = this._cmpHelper.GetEntityMappingItem(args);
                Assert.IsNotNull((object)args.EntityMappingItem, "Could not find any Entity Mapping item for the Entity Type (Schema): " + args.ContentTypeIdentifier);
                using (new SecurityDisabler())
                {
                    Database database = this._factory.GetDatabase(EnsureItemEx._settings.DatabaseName);
                    Assert.IsNotNull((object)database, "Could not get the master database.");
                    Item cmpItemBucket = this._cmpHelper.GetCmpItemBucket(args, database);
                    Assert.IsNotNull((object)cmpItemBucket, "Could not find the item bucket. Check this field value in the configuration item.");
                    string name1 = "";
                    if (args.Entity.GetProperty(args.EntityMappingItem[Sitecore.Connector.CMP.Constants.EntityMappingItemNamePropertyField]).IsMultiLanguage)
                    {
                        name1 = args.Entity.GetPropertyValue(args.EntityMappingItem[Sitecore.Connector.CMP.Constants.EntityMappingItemNamePropertyField], this._cmpHelper.GetLanguageCultureOrDefault((IEnumerable<CultureInfo>)args.Entity.Cultures, args.Item.Language.CultureInfo))?.ToString();
                    }
                    else
                    {
                        name1 = args.Entity.GetPropertyValue(args.EntityMappingItem[Sitecore.Connector.CMP.Constants.EntityMappingItemNamePropertyField])?.ToString();
                    }
                    Assert.IsNotNullOrEmpty(name1, "Could not get the property value from Content Hub as Sitecore item name. Check this field isn't valid and the value should not be null in Content Hub.");
                    string name2 = ItemUtil.ProposeValidItemName(name1);
                    Assert.IsNotNullOrEmpty(name2, "Could not proposed the valid item name as Sitecore Item Name.");
                    TemplateItem template = (TemplateItem)database.GetItem(new ID(args.EntityMappingItem[Sitecore.Connector.CMP.Constants.EntityMappingTemplateFieldId]), args.Language);
                    Assert.IsNotNull((object)template, "Could not get template item. Check this field value in the configuration item.");
                    //if (args.EntityDefinition != null && !string.IsNullOrEmpty(args.EntityDefinition.Name))
                    //    Sitecore.Connector.CMP.ConsumptionTracking.Telemetry.TrackCMPInsertEntity(args.EntityDefinition.Name);
                    args.Item = cmpItemBucket.Add(name2, template);
                }
            }
        }
    }
}