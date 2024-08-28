using HtmlAgilityPack;
using Sitecore.LayoutService.Serialization;
using Sitecore.LayoutService.Serialization.FieldSerializers;
using System.Collections.Generic;
using System.Web;

namespace XmCloudSXAStarter.Pipelines.GetFieldSerializer
{
    public class ImageFieldSerializerExtendCustom : ImageFieldSerializer
    {
        public ImageFieldSerializerExtendCustom(IFieldRenderer fieldRenderer)
          : base(fieldRenderer)
        {
        }

        protected override IDictionary<string, string> ParseRenderedImage(
          string renderedField)
        {
            Dictionary<string, string> renderedImage = new Dictionary<string, string>();
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(renderedField);
            if (htmlDocument.DocumentNode == null || !htmlDocument.DocumentNode.HasChildNodes)
                return (IDictionary<string, string>)renderedImage;
            HtmlNode htmlNode1 = htmlDocument.DocumentNode.SelectSingleNode("//img");
            if (htmlNode1 == null)
            {
                htmlNode1 = htmlDocument.DocumentNode.SelectSingleNode("//image");
                if (htmlNode1 == null)
                {
                    htmlNode1 = htmlDocument.DocumentNode.SelectSingleNode("//video");
                    if (htmlNode1 == null)
                    {
                        htmlNode1 = htmlDocument.DocumentNode.SelectSingleNode("//a");
                        if (htmlNode1 == null)
                            return (IDictionary<string, string>)renderedImage;
                        renderedImage["dam-content-type"] = "Document";
                        renderedImage["label"] = htmlNode1.InnerText;
                    }
                    else
                    {
                        HtmlNode htmlNode2 = htmlDocument.DocumentNode.SelectSingleNode("//source");
                        renderedImage["dam-content-type"] = "Video";
                        foreach (HtmlAttribute attribute in (IEnumerable<HtmlAttribute>)htmlNode2.Attributes)
                            renderedImage[attribute.Name] = HttpUtility.HtmlDecode(attribute.Value);
                    }
                }
            }
            foreach (HtmlAttribute attribute in (IEnumerable<HtmlAttribute>)htmlNode1.Attributes)
                renderedImage[attribute.Name] = HttpUtility.HtmlDecode(attribute.Value);
            
            if(renderedImage.ContainsKey("dam-id") && renderedImage["dam-id"].Length > 0 && !renderedImage.ContainsKey("stylelabs-content-type"))
            {
                renderedImage["stylelabs-content-type"] = renderedImage["dam-content-type"];
            }

            return (IDictionary<string, string>)renderedImage;
        }
    }
}