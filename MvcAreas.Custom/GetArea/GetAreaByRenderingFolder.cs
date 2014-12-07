using System;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Data.Templates;
using Sitecore.SecurityModel;

namespace MvcAreas.Custom.GetArea
{
    public class GetAreaByRenderingFolder : GetAreaProcessor
    {
        public override void Process(GetAreaArgs args)
        {
            if (SkipProcessor(args))
            {
                return;
            }

            FindAreaByFolder(args);
        }


        public static bool IsDerived(Item item, ID templateId)
        {
            if (item == null)
                return false;

            if (templateId == (ID)null || templateId.IsNull)
                return false;

            using (new SecurityStateSwitcher(SecurityState.Disabled))
            {
                TemplateItem templateItem = item.Database.Templates[templateId];

                bool isDerived = false;
                if (templateItem != null)
                {
                    Template template = TemplateManager.GetTemplate(item);
                    if (template == null) return false;
                    isDerived = template.ID == templateItem.ID || template.DescendsFrom(templateItem.ID);
                }

                return isDerived;
            }
        }

        /// <summary>
        ///     Find a parent item of the current item derived from a specific template
        /// </summary>
        /// <param name="item">current item</param>
        /// <param name="templateId">template id to find the item derived from</param>
        /// <returns></returns>
        public static Item FindParentDerivedFrom(Item item, ID templateId)
        {
            if (item == null) return null;

            return IsDerived(item, templateId) ? item : FindParentDerivedFrom(item.Parent, templateId);
        }

        public virtual void FindAreaByFolder(GetAreaArgs args)
        {           
            RenderingItem renderingItem = args.Rendering.RenderingItem;

            Item current = renderingItem.InnerItem;

            Item folder = FindParentDerivedFrom(current, new ID(Const.TemplateIds.MvcAreaNameBase));

            if (folder == null)
            {
                return;
            }

            string areaName = folder["Area Name"];

            if (String.IsNullOrWhiteSpace(areaName))
            {
                return;
            }

            args.AreaName = areaName;
        }
    }
}