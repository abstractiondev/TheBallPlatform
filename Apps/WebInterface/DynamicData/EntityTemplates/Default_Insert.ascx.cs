using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.DynamicData;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DynamicDataWebApp
{
    public partial class Default_InsertEntityTemplate : System.Web.DynamicData.EntityTemplateUserControl
    {
        public override Control NamingContainer
        {
            get
            {
                var namingContainer = base.NamingContainer;
                return namingContainer;
            }
        }

        private MetaColumn currentColumn;

        protected override void OnLoad(EventArgs e)
        {
            foreach (MetaColumn column in Table.GetScaffoldColumns(Mode, ContainerType))
            {
                currentColumn = column;
                Control item = new DefaultEntityTemplate._NamingContainer();
                //item.ClientIDMode = ClientIDMode.Static;
                EntityTemplate1.ItemTemplate.InstantiateIn(item);
                EntityTemplate1.Controls.Add(item);
            }
            // Domain Type Input
            HiddenField domainName = new HiddenField
            {
                ID = "SemanticDomainName",
                Value = Table.Name,
                //ClientIDMode = ClientIDMode.Static
            };
            EntityTemplate1.Controls.Add(domainName);

            HiddenField rootNamingContainer = new HiddenField();
            rootNamingContainer.ID = "RootNamingContainer";
            rootNamingContainer.Value = NamingContainer.UniqueID;
            EntityTemplate1.Controls.Add(rootNamingContainer);

        }

        protected void Label_Init(object sender, EventArgs e)
        {
            Label label = (Label)sender;
            label.Text = currentColumn.DisplayName;
        }

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

        protected void DynamicControl_Init(object sender, EventArgs e)
        {
            DynamicControl dynamicControl = (DynamicControl)sender;
            dynamicControl.DataField = currentColumn.Name;
        }

    }
}
