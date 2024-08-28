using Sitecore.Connector.ContentHub.DAM.FieldSerializers;
using Sitecore.Diagnostics;
using Sitecore.LayoutService.Serialization;
using Sitecore.LayoutService.Serialization.FieldSerializers;
using Sitecore.LayoutService.Serialization.Pipelines.GetFieldSerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XmCloudSXAStarter.Pipelines.GetFieldSerializer
{
    public class GetImageFieldSerializerExtendCustom : BaseGetFieldSerializer
    {
        public GetImageFieldSerializerExtendCustom(IFieldRenderer fieldRenderer)
          : base(fieldRenderer)
        {
        }

        protected override void SetResult(GetFieldSerializerPipelineArgs args)
        {
            Assert.ArgumentNotNull((object)args, nameof(args));
            args.Result = (IFieldSerializer)new ImageFieldSerializerExtendCustom(this.FieldRenderer);
        }
    }
}