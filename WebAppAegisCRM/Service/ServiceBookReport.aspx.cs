﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Entity.Service;
using Business.Common;

namespace WebAppAegisCRM.Service
{
    public partial class ServiceBookReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCallStatus();
                FetchAllEmployee();
                LoadProduct();
            }
        }

        #region User Defined Funtions
        protected void LoadCallStatus()
        {
            Business.Service.CallStatus objCallStatus = new Business.Service.CallStatus();

            DataTable dt = objCallStatus.GetAll(2);
            if (dt != null)
            {
                ddlDocketCallStatus.DataSource = dt;
                ddlDocketCallStatus.DataTextField = "CallStatus";
                ddlDocketCallStatus.DataValueField = "CallStatusId";
                ddlDocketCallStatus.DataBind();
            }
            ListItem li = new ListItem("--SELECT ALL--", "0");
            ddlDocketCallStatus.Items.Insert(0, li);
        }
        private void LoadProduct()
        {
            Business.Inventory.ProductMaster objProductMaster = new Business.Inventory.ProductMaster();
            Entity.Inventory.ProductMaster productmaster = new Entity.Inventory.ProductMaster();

            productmaster.CompanyMasterId = 1;
            DataTable dt = objProductMaster.GetAll(productmaster);
            ddlDocketProduct.DataSource = dt;
            ddlDocketProduct.DataTextField = "ProductName";
            ddlDocketProduct.DataValueField = "ProductMasterId";
            ddlDocketProduct.DataBind();
            ListItem li = new ListItem("--SELECT ALL--", "0");
            ddlDocketProduct.Items.Insert(0, li);
        }
        public string CalculateTimings(int dte)
        {
            int Days = dte / 24;
            int Hours = dte % 24;
            return String.Format("{0} Day(s) {1} HH",
                                Days, Hours);
        }
        protected void FetchAllEmployee()
        {
            Business.HR.EmployeeMaster objEmployeeMaster = new Business.HR.EmployeeMaster();
            Entity.HR.EmployeeMaster employeeMaster = new Entity.HR.EmployeeMaster();
            employeeMaster.CompanyId_FK = 1;
            DataTable dt = objEmployeeMaster.Employee_GetAll_Active(employeeMaster);
            if (dt != null)
            {
                ddlServiceEngineer.DataSource = dt;
                ddlServiceEngineer.DataValueField = "EmployeeMasterId";
                ddlServiceEngineer.DataTextField = "EmployeeName";
                ddlServiceEngineer.DataBind();
            }
            ddlServiceEngineer.InsertSelect();

            if (dt != null && dt.Rows.Count > 0)
            {
                if (!HttpContext.Current.User.IsInRole(Entity.HR.Utility.CUSTOMER_LIST_SHOW_ALL))
                {
                    ddlServiceEngineer.SelectedValue = HttpContext.Current.User.Identity.Name;
                    ddlServiceEngineer.Enabled = false;
                }
            }
        }
        #endregion

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Business.Service.ServiceBook objServiceBook = new Business.Service.ServiceBook();
            Entity.Service.ServiceBook serviceBook = new Entity.Service.ServiceBook();

            serviceBook.CustomerName = txtCustomerName.Text.Trim();
            serviceBook.ModelId = int.Parse(ddlDocketProduct.SelectedValue);
            serviceBook.MachineId = txtMachineId.Text.Trim();
            serviceBook.FromDate = (!string.IsNullOrEmpty(txtFromDocketDate.Text.Trim())) ? Convert.ToDateTime(txtFromDocketDate.Text.Trim()) : DateTime.MinValue;
            serviceBook.ToDate = (!string.IsNullOrEmpty(txtToDocketDate.Text.Trim())) ? Convert.ToDateTime(txtToDocketDate.Text.Trim()) : DateTime.MinValue;
            serviceBook.CallStatusId = int.Parse(ddlDocketCallStatus.SelectedValue);
            serviceBook.DocketType = ddlDocketType.SelectedValue;
            serviceBook.EmployeeId_FK = int.Parse(ddlServiceEngineer.SelectedValue);

            DataTable dt = objServiceBook.Service_ServiceBook_GetAll(Convert.ToInt32(ddlCallType.SelectedValue), serviceBook);
            if (dt != null)
            {
                if (Convert.ToInt32(ddlCallType.SelectedValue) == (int)CallType.Docket)
                {
                    pnlServiceDocket.Visible = true;
                    pnlServiceToner.Visible = false;
                    gvServiceDocket.DataSource = dt;
                    gvServiceDocket.DataBind();
                }
                else if (Convert.ToInt32(ddlCallType.SelectedValue) == (int)CallType.Toner)
                {
                    pnlServiceDocket.Visible = false;
                    pnlServiceToner.Visible = true;
                    gvServiceToner.DataSource = dt;
                    gvServiceToner.DataBind();
                }
            }

            if (ddlCallType.SelectedValue != "0")
                lblListTitle.Text = ddlCallType.SelectedItem.Text;
            else
                lblListTitle.Text = "";
        }

        protected void btnDocketSearch_Click(object sender, EventArgs e)
        {
            btnSearch_Click(sender, e);
        }

        protected void gvServiceDocket_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvServiceDocket.PageIndex = e.NewPageIndex;
            btnSearch_Click(sender, e);
        }

        protected void gvServiceToner_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvServiceToner.PageIndex = e.NewPageIndex;
            btnSearch_Click(sender, e);
        }

        protected void gvServiceDocket_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "D")
                {
                    string serviceBookId = e.CommandArgument.ToString();
                    Entity.Service.CsrJson csrJson = new Entity.Service.CsrJson();
                    Business.Service.ServiceBook objServiceBook = new Business.Service.ServiceBook();
                    DataTable dtCsr = objServiceBook.Service_CsrGetByServiceBookId(Convert.ToInt64(serviceBookId));
                    int response = objServiceBook.Service_CsrDelete(Convert.ToInt64(dtCsr.Rows[0]["CsrId"].ToString()));

                    if (response > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "mmsg", "alert('Existing csr deleted.');", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "mmsg", "alert('Csr can not be deleted!!!....');", true);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.WriteException();
            }
        }
    }
}