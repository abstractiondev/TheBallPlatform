using System;
using System.Web.DynamicData;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DynamicDataWebApp
{
    public class TheBallEditableEntityTemplate : TheBallEntityTemplate
    {
        protected void Label_PreRender(object sender, EventArgs e)
        {
            Label label = (Label)sender;
            DynamicControl dynamicControl = (DynamicControl)label.FindControl("DynamicControl");
            FieldTemplateUserControl ftuc = dynamicControl.FieldTemplate as FieldTemplateUserControl;
            if (ftuc != null && ftuc.DataControl != null)
            {
                label.AssociatedControlID = ftuc.DataControl.GetUniqueIDRelativeTo(label);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Control hiddenFieldNaming = new _NamingContainer();

            HiddenField tableName = new HiddenField
            {
                ID = "ObjectTableName",
                Value = Table.Name
            };

            HiddenField semanticDomainName = new HiddenField
            {
                ID = "ObjectDataContextName",
                Value = Table.DataContextType.FullName
            };

            hiddenFieldNaming.Controls.Add(tableName);
            hiddenFieldNaming.Controls.Add(semanticDomainName);
            EntityTemplate.Controls.Add(hiddenFieldNaming);
        }
    }
}