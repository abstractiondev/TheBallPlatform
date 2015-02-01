using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.DynamicData;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DynamicDataWebApp
{
    public class TheBallEntityTemplate : EntityTemplateUserControl
    {
        private MetaColumn currentColumn;
        protected global::System.Web.DynamicData.EntityTemplate EntityTemplate;

        protected override void OnLoad(EventArgs e)
        {
            foreach (MetaColumn column in Table.GetScaffoldColumns(Mode, ContainerType))
            {
                currentColumn = column;
                Control item = new _NamingContainer();
                EntityTemplate.ItemTemplate.InstantiateIn(item);
                EntityTemplate.Controls.Add(item);
            }

            HiddenField domainName = new HiddenField
            {
                ID = "SemanticDomainName",
                Value = Table.Name,
                //ClientIDMode = ClientIDMode.Static
            };
            EntityTemplate.Controls.Add(domainName);

            HiddenField rootNamingContainer = new HiddenField();
            rootNamingContainer.ID = "RootNamingContainer";
            rootNamingContainer.Value = NamingContainer.UniqueID;
            EntityTemplate.Controls.Add(rootNamingContainer);
        }

        protected void Label_Init(object sender, EventArgs e)
        {
            Label label = (Label)sender;
            label.Text = currentColumn.DisplayName;
        }

        protected void DynamicControl_Init(object sender, EventArgs e)
        {
            DynamicControl dynamicControl = (DynamicControl)sender;
            dynamicControl.DataField = currentColumn.Name;
        }

        public class _NamingContainer : Control, INamingContainer { }

    }
}