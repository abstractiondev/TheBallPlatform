using System;
using System.Web.DynamicData;
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
    }
}