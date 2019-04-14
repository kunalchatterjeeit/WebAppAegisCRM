﻿using AjaxControlToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebAppAegisCRM.UserControl
{
    public partial class SalesLinkTypeControl : System.Web.UI.UserControl
    {
        private void LoadAccountList()
        {
            Business.Sales.Account objAccount = new Business.Sales.Account();
            Entity.Sales.GetAccountsParam getAccountsParam = new Entity.Sales.GetAccountsParam { Name = null, OfficePhone = null };
            
            ddlAccounts.DataSource = objAccount.GetAllAccounts(getAccountsParam);
            ddlAccounts.DataTextField = "Name";
            ddlAccounts.DataValueField = "Id";
            ddlAccounts.DataBind();
        }
        private void LoadLeadList()
        {
            Business.Sales.Leads Obj = new Business.Sales.Leads();
            Entity.Sales.GetLeadsParam Param = new Entity.Sales.GetLeadsParam { CampaignId = null, DepartmentId = null, Name = null, Email = null };
            
            ddlLead.DataSource = Obj.GetAllLeads(Param);
            ddlLead.DataTextField = "Name";
            ddlLead.DataValueField = "Id";
            ddlLead.DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadAccountList();
                LoadLeadList();
                ddlOpportunity.Items.Add(new ListItem() { Text = "Opportunity1", Value = "3" });
            }
        }

        protected void btnContinue_Click(object sender, EventArgs e)
        {
            HiddenField hdnItemType = (HiddenField)Parent.FindControl("hdnItemType");
            HiddenField hdnItemId = (HiddenField)Parent.FindControl("hdnItemId");
            if (TabContainer1.ActiveTabIndex == 0)
            {
                hdnItemType.Value = "SalesAccounts";
                hdnItemId.Value = ddlAccounts.SelectedValue;
            }
            else if (TabContainer1.ActiveTabIndex == 1)
            {
                hdnItemType.Value = "SalesLeads";
                hdnItemId.Value = ddlLead.SelectedValue;
            }
            else if (TabContainer1.ActiveTabIndex == 2)
            {
                hdnItemType.Value = "SalesOpportunity";
                hdnItemId.Value = ddlOpportunity.SelectedValue;
            }
            ((ModalPopupExtender)Parent.FindControl("ModalPopupExtender1")).Hide();
        }

        protected void btnCreateNew_Click(object sender, EventArgs e)
        {
            if (TabContainer1.ActiveTabIndex == 0)
            {
                Response.Redirect("../Sales/Account.aspx");
            }
            else if (TabContainer1.ActiveTabIndex == 1)
            {
                Response.Redirect("../Sales/Lead.aspx");
            }
            else if (TabContainer1.ActiveTabIndex == 2)
            {
                Response.Redirect("../Sales/Opportunities.aspx");
            }
        }

        protected void btnReturn_Click(object sender, EventArgs e)
        {
            Response.Redirect(Business.Common.Context.ReferralUrl);
        }
    }
}