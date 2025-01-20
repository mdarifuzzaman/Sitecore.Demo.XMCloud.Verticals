using Sitecore.Abstractions;
using Sitecore.Connector.CMP;
using Sitecore.Connector.CMP.Conversion;
using Sitecore.Connector.CMP.Helpers;
using Sitecore.Connector.CMP.Pipelines.ImportEntity;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace XmCloudSXAStarter.Pipelines.CMP.SaveFieldValues
{
    public class SaveFieldValuesExt : ImportEntityProcessor
    {
        private static CmpSettings _settings;
        private readonly ICmpConverterMapper _mapper;
        private readonly CmpHelper _cmpHelper;

        public SaveFieldValuesExt(
          ICmpConverterMapper mapper,
          BaseLog logger,
          CmpHelper cmpHelper,
          CmpSettings settings)
          : base(logger, settings)
        {
            this._mapper = mapper;
            _settings = settings;
            this._cmpHelper = cmpHelper;
        }

        public override void Process(ImportEntityPipelineArgs args, BaseLog logger)
        {
            Assert.IsNotNull((object)args.Item, "The item is null.");
            Assert.IsNotNull((object)args.Language, "The language is null.");
            using (new SecurityDisabler())
            {
                using (new LanguageSwitcher(args.Language))
                {
                    bool flag = false;
                    try
                    {
                        args.Item.Editing.BeginEdit();
                        args.Item[Sitecore.Connector.CMP.Constants.EntityIdentifierFieldId] = args.EntityIdentifier;
                        flag = this.TryMapConfiguredFields(args) && this.TryUpdateItemName(args);
                    }
                    catch
                    {
                        flag = false;
                        throw;
                    }
                    finally
                    {
                        if (flag)
                        {
                            args.Item.Editing.EndEdit();
                        }
                        else
                        {
                            args.Item.Editing.CancelEdit();
                            args.Item.Editing.BeginEdit();
                            args.Item[Sitecore.Connector.CMP.Constants.EntityIdentifierFieldId] = args.EntityIdentifier;
                            args.Item.Editing.EndEdit();
                        }
                    }
                }
            }
        }

        internal virtual bool TryMapConfiguredFields(ImportEntityPipelineArgs args)
        {
            if (args.EntityMappingItem == null)
                args.EntityMappingItem = this._cmpHelper.GetEntityMappingItem(args);
            Assert.IsNotNull((object)args.EntityMappingItem, "Could not find any Entity Mapping item for the Entity Type (Schema): " + args.ContentTypeIdentifier);
            bool flag = false;
            foreach (Item obj in args.EntityMappingItem.Children.Where<Item>((Func<Item, bool>)(i => i.TemplateID == Sitecore.Connector.CMP.Constants.FieldMappingTemplateId || i.TemplateID == Sitecore.Connector.CMP.Constants.RelationFieldMappingTemplateId)))
            {
                string fieldName = obj[Sitecore.Connector.CMP.Constants.FieldMappingSitecoreFieldNameFieldId];
                string str = obj[Sitecore.Connector.CMP.Constants.FieldMappingCmpFieldNameFieldId];
                if (!string.IsNullOrEmpty(fieldName))
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        try
                        {
                            if (obj.TemplateID == Sitecore.Connector.CMP.Constants.RelationFieldMappingTemplateId)
                            {
                                string cmpRelationName = obj[Sitecore.Connector.CMP.Constants.RelationFieldMappingCmpRelationFieldNameFieldId];
                                if (string.IsNullOrEmpty(cmpRelationName))
                                {
                                    this.Logger.Error(BaseHelper.GetLogMessageText(_settings.LogMessageTitle, string.Format("Configuration of the field mapping '{0}' is incorrect. Required fields are not specified.", (object)obj.ID)), (object)this);
                                    flag = true;
                                    continue;
                                }
                                List<string> values = this._cmpHelper.TryMapRelationPropertyValues(args, cmpRelationName, str);
                                args.Item[fieldName] = values.Count != 0 ? string.Join(_settings.RelationFieldMappingSeparator, (IEnumerable<string>)values) : string.Empty;
                                continue;
                            }
                            if (!args.Entity.GetProperty(str).IsMultiLanguage)
                            {
                                args.Item[fieldName] = this._mapper.Convert(args.EntityDefinition, str, args.Entity.GetPropertyValue(str));
                                continue;
                            }
                            args.Item[fieldName] = this._mapper.Convert(args.EntityDefinition, str, args.Entity.GetPropertyValue(str, this._cmpHelper.GetLanguageCultureOrDefault((IEnumerable<CultureInfo>)args.Entity.Cultures, args.Item.Language.CultureInfo)));
                            continue;
                        }
                        catch (Exception ex)
                        {
                            this.Logger.Error(BaseHelper.GetLogMessageText(_settings.LogMessageTitle, string.Format("An error occured during converting '{0}' field to '{1}' field. Field mapping ID: '{2}'.", (object)str, (object)fieldName, (object)obj.ID)), ex, (object)this);
                            flag = true;
                            args.Exception = ex;
                            continue;
                        }
                    }
                }
                this.Logger.Error(BaseHelper.GetLogMessageText(_settings.LogMessageTitle, string.Format("Configuration of the field mapping '{0}' is incorrect. Required fields are not specified.", (object)obj.ID)), (object)this);
                flag = true;
            }
            return !flag;
        }

        internal virtual bool TryUpdateItemName(ImportEntityPipelineArgs args)
        {
            bool flag = false;
            try
            {
                CheckboxField field = (CheckboxField)args.EntityMappingItem.Fields[Sitecore.Connector.CMP.Constants.ShouldUpdateItemNameTemplateId];
                if (field != null)
                {
                    if (field.Checked)
                    {
                        string name = args.Entity.GetPropertyValue(args.EntityMappingItem[Sitecore.Connector.CMP.Constants.EntityMappingItemNamePropertyField], this._cmpHelper.GetLanguageCultureOrDefault((IEnumerable<CultureInfo>)args.Entity.Cultures, args.Item.Language.CultureInfo))?.ToString();
                        Assert.IsNotNullOrEmpty(name, "Could not get the property value from Content Hub as Sitecore item name. Check this field isn't valid and the value should not be null in Content Hub.");
                        string str = ItemUtil.ProposeValidItemName(name);
                        Assert.IsNotNullOrEmpty(str, "Could not proposed the valid item name as Sitecore Item Name.");
                        if (!args.Item.Name.Equals(str, StringComparison.Ordinal))
                        {
                            using (new SecurityDisabler())
                                args.Item.Name = str;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.Logger.Error(BaseHelper.GetLogMessageText(_settings.LogMessageTitle, string.Format("An error occured during updating item name for '{0}'.", (object)args.Item.ID)), ex, (object)this);
                flag = true;
                args.Exception = ex;
            }
            return !flag;
        }
    }
}