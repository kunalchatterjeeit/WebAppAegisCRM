﻿using Business.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebAppAegisCRM.LeaveManagement
{
    public partial class LeaveDesignationConfiguration : System.Web.UI.Page
    {
        private int LeaveDesignationWiseConfigurationId
        {
            get { return Convert.ToInt32(ViewState["LeaveDesignationWiseConfigurationId"]); }
            set { ViewState["LeaveDesignationWiseConfigurationId"] = value; }
        }

        private void LoadLeaveType()
        {
            Business.LeaveManagement.LeaveType objLeaveType = new Business.LeaveManagement.LeaveType();
            DataTable dtLeaveMaster = objLeaveType.LeaveTypeGetAll(new Entity.LeaveManagement.LeaveType());
            if (dtLeaveMaster != null)
            {
                ddlLeaveType.DataSource = dtLeaveMaster;
                ddlLeaveType.DataTextField = "LeaveTypeName";
                ddlLeaveType.DataValueField = "LeaveTypeId";
                ddlLeaveType.DataBind();
            }
            ddlLeaveType.InsertSelect();
        }

        private void DesignationMaster_GetAll()
        {
            Business.HR.EmployeeMaster objEmployeeMaster = new Business.HR.EmployeeMaster();
            Entity.HR.EmployeeMaster employeeMaster = new Entity.HR.EmployeeMaster();
            employeeMaster.CompanyId_FK = 1;
            DataTable dt = objEmployeeMaster.DesignationMaster_GetAll(employeeMaster);
            if (dt.Rows.Count > 0)
            {
                ddlDesignation.DataSource = dt;
                ddlDesignation.DataTextField = "DesignationName";
                ddlDesignation.DataValueField = "DesignationMasterId";
                ddlDesignation.DataBind();
            }
            ddlDesignation.InsertSelect();
        }

        protected void LeaveDesignationConfig_GetById()
        {
            Business.LeaveManagement.LeaveDesignationWiseConfiguration objLeaveDesignationWiseConfiguration = new Business.LeaveManagement.LeaveDesignationWiseConfiguration();

            DataTable dt = objLeaveDesignationWiseConfiguration.LeaveDesignationConfig_GetById(LeaveDesignationWiseConfigurationId);

            if (dt.Rows.Count > 0)
            {
                ddlLeaveType.SelectedValue = dt.Rows[0]["LeaveTypeId"].ToString();
                ddlDesignation.Text = dt.Rows[0]["DesignationId"].ToString();
                txtCarryForwardCount.Text = dt.Rows[0]["CarryForwardCount"].ToString();
                txtLeaveCount.Text = dt.Rows[0]["LeaveCount"].ToString();
            }
        }

        private void Clear()
        {
            LeaveDesignationWiseConfigurationId = 0;
            ddlLeaveType.SelectedIndex = 0;
            ddlDesignation.SelectedIndex = 0;
            txtCarryForwardCount.Text = string.Empty;
            txtLeaveCount.Text = string.Empty;
            Message.Show = false;
        }

        private void LeaveDesignationWiseConfiguration_GetAll()
        {
            Business.LeaveManagement.LeaveDesignationWiseConfiguration objLeaveDesignationWiseConfiguration = new Business.LeaveManagement.LeaveDesignationWiseConfiguration();
            Entity.LeaveManagement.LeaveDesignationWiseConfiguration leaveDesignationWiseConfiguration = new Entity.LeaveManagement.LeaveDesignationWiseConfiguration();

            DataTable dt = objLeaveDesignationWiseConfiguration.LeaveDesignationConfig_GetAll(leaveDesignationWiseConfiguration);

            gvLeaveDesignationConfiguration.DataSource = dt;
            gvLeaveDesignationConfiguration.DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadLeaveType();
                DesignationMaster_GetAll();
                LeaveDesignationWiseConfiguration_GetAll();
                Message.Show = false;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Business.LeaveManagement.LeaveDesignationWiseConfiguration objLeaveDesignationWiseConfiguration = new Business.LeaveManagement.LeaveDesignationWiseConfiguration();
            Entity.LeaveManagement.LeaveDesignationWiseConfiguration leaveDesignationWiseConfiguration = new Entity.LeaveManagement.LeaveDesignationWiseConfiguration();

            leaveDesignationWiseConfiguration.LeaveDesignationConfigId = LeaveDesignationWiseConfigurationId;
            leaveDesignationWiseConfiguration.LeaveTypeId = Convert.ToInt32(ddlLeaveType.SelectedValue);
            leaveDesignationWiseConfiguration.DesignationId = Convert.ToInt32(ddlDesignation.SelectedValue);
            leaveDesignationWiseConfiguration.LeaveCount = Convert.ToDecimal(txtLeaveCount.Text.Trim());
            leaveDesignationWiseConfiguration.CarryForwardCount = Convert.ToDecimal(txtCarryForwardCount.Text.Trim());
            int response = objLeaveDesignationWiseConfiguration.LeaveDesignationConfig_Save(leaveDesignationWiseConfiguration);
            if (response > 0)
            {
                Clear();
                LeaveDesignationWiseConfiguration_GetAll();
                Message.IsSuccess = true;
                Message.Text = "Saved Successfully";
            }
            else
            {
                Message.IsSuccess = false;
                Message.Text = "Exists";
            }
            Message.Show = true;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Clear();
        }

        protected void gvLeaveDesignationConfiguration_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "E")
            {
                LeaveDesignationWiseConfigurationId = Convert.ToInt32(e.CommandArgument.ToString());
                LeaveDesignationConfig_GetById();
            }
            else
            {
                if (e.CommandName == "D")
                {
                    Business.LeaveManagement.LeaveDesignationWiseConfiguration objLeaveDesignationWiseConfiguration = new Business.LeaveManagement.LeaveDesignationWiseConfiguration();
                    LeaveDesignationWiseConfigurationId = Convert.ToInt32(e.CommandArgument.ToString());
                    int RowsAffected = objLeaveDesignationWiseConfiguration.LeaveDesignationConfig_Delete(LeaveDesignationWiseConfigurationId);
                    if (RowsAffected > 0)
                    {
                        LoadLeaveType();
                        LeaveDesignationWiseConfiguration_GetAll();
                        Message.Show = true;
                        Message.Text = "Deleted Successfully";
                    }
                    else
                    {
                        Message.Show = false;
                        Message.Text = "Data Dependency Exists";
                    }
                    Message.Show = true;
                }
            }
        }
    }
}