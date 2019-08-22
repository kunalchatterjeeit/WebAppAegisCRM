﻿using Business.Common;
using Entity.ClaimManagement;
using Entity.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebAppAegisCRM.ClaimManagement
{
    public partial class ClaimDisbursementAndVoucher : System.Web.UI.Page
    {
        private DataTable _ClaimPaymentDetails
        {
            get
            {
                return Business.Common.Context.ClaimPaymentDetails;
            }

            set { Business.Common.Context.ClaimPaymentDetails = value; }
        }
        private void EmployeeMaster_GetAll()
        {
            Business.HR.EmployeeMaster ObjBelEmployeeMaster = new Business.HR.EmployeeMaster();
            Entity.HR.EmployeeMaster ObjElEmployeeMaster = new Entity.HR.EmployeeMaster();
            ObjElEmployeeMaster.CompanyId_FK = 1;
            DataTable dt = ObjBelEmployeeMaster.Employee_GetAll_Active(ObjElEmployeeMaster);

            ddlEmployee.DataSource = dt;
            ddlEmployee.DataTextField = "EmployeeName";
            ddlEmployee.DataValueField = "EmployeeMasterId";
            ddlEmployee.DataBind();
        }
        private void ClaimApplication_GetAll()
        {
            Entity.ClaimManagement.ClaimApplicationMaster ClaimApplicationMaster = new Entity.ClaimManagement.ClaimApplicationMaster();
            ClaimApplicationMaster.EmployeeId = Convert.ToInt32(ddlEmployee.SelectedValue);
            ClaimApplicationMaster.PeriodFrom = (string.IsNullOrEmpty(txtFromClaimDate.Text.Trim())) ? DateTime.MinValue : Convert.ToDateTime(txtFromClaimDate.Text.Trim());
            ClaimApplicationMaster.PeriodTo = (string.IsNullOrEmpty(txtToClaimDate.Text.Trim())) ? DateTime.MinValue : Convert.ToDateTime(txtToClaimDate.Text.Trim());
            ClaimApplicationMaster.Status = (int)ClaimStatusEnum.Approved;
            Business.ClaimManagement.ClaimApplication objClaimApplication = new Business.ClaimManagement.ClaimApplication();
            DataTable dtClaimApplication = objClaimApplication.ClaimApplication_GetAll(ClaimApplicationMaster);
            if (dtClaimApplication != null)
            {
                gvApprovedClaim.DataSource = dtClaimApplication;
                gvApprovedClaim.DataBind();
            }
        }
        private void VoucherPaymentMode_GetAll()
        {
            ddlPaymentModes.DataSource = new Business.ClaimManagement.VoucherPayment().VoucherPaymentMode_GetAll();
            ddlPaymentModes.DataTextField = "ModeName";
            ddlPaymentModes.DataValueField = "PaymentModeId";
            ddlPaymentModes.DataBind();
        }
        private void FetchBasicDetails()
        {
            decimal totalClaimedAmount = 0, totalApprovedAmount = 0, totalAdjustedAmount = 0;

            foreach (GridViewRow gvr in gvApprovedClaim.Rows)
            {
                CheckBox chkSelect = (CheckBox)gvr.FindControl("chkitem");
                if (chkSelect.Checked)
                {
                    int claimApplicationId = Convert.ToInt32(gvApprovedClaim.DataKeys[gvr.RowIndex].Values[0].ToString());
                    DataSet dsClaimApplicationDetails = new Business.ClaimManagement.ClaimApplication().GetClaimApplicationDetails_ByClaimApplicationId(claimApplicationId);
                    if (dsClaimApplicationDetails != null)
                    {
                        totalClaimedAmount += Convert.ToDecimal(dsClaimApplicationDetails.Tables[0].Rows[0]["TotalAmount"].ToString());
                        totalApprovedAmount += Convert.ToDecimal(dsClaimApplicationDetails.Tables[0].Rows[0]["ApprovedAmount"].ToString());
                        totalAdjustedAmount += Convert.ToDecimal(dsClaimApplicationDetails.Tables[0].Rows[0]["AdjustRequestAmount"].ToString());
                    }
                }
            }
            lblAdvanceAdjustAmount.Text = totalAdjustedAmount.ToString();
            lblTotalApprovedAmount.Text = totalApprovedAmount.ToString();
            lblTotalClaimAmount.Text = totalClaimedAmount.ToString();
        }
        private void GetClaimAccountBalance()
        {
            lblAdvanceBalance.Text = Convert.ToString(new Business.ClaimManagement.ClaimDisbursement()
                .GetClaimAccountBalance(Convert.ToInt32(ddlEmployee.SelectedValue)));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                EmployeeMaster_GetAll();
                MessageSuccess.Show = false;
                Message.Show = false;
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            ClaimApplication_GetAll();
        }

        protected void gvClaimApprovalList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "View")
            {
                Message1.Show = false;
                Business.Common.Context.ClaimApplicationId = Convert.ToInt32(e.CommandArgument.ToString());
                GetClaimApplicationDetails_ByClaimApplicationId(Business.Common.Context.ClaimApplicationId);
                ModalPopupExtender2.Show();
            }
        }

        protected void gvPaymentDetails_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "D")
            {
                string autoId = e.CommandArgument.ToString();

                if (DeleteItem(autoId))
                {
                    ComputeTotalPaying();
                    LoadClaimPaymentDetails();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "mmsg", "alert('Data can not be deleted!!!....');", true);
                }
            }

            ModalPopupExtender1.Show();
        }

        private bool DeleteItem(string autoId)
        {
            bool retValue = false;
            int lastCount = 0;
            if (_ClaimPaymentDetails.Rows.Count > 0)
            {
                lastCount = _ClaimPaymentDetails.Rows.Count;
                _ClaimPaymentDetails.Rows[_ClaimPaymentDetails.Rows.IndexOf(_ClaimPaymentDetails.Select("AutoId='" + autoId + "'").FirstOrDefault())].Delete();
                _ClaimPaymentDetails.AcceptChanges();
            }
            if (lastCount > _ClaimPaymentDetails.Rows.Count)
            {
                retValue = true;
            }
            return retValue;
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (ValidatePaymentItem())
            {
                if (_ClaimPaymentDetails.Rows.Count == 0)
                {
                    using (DataTable dtInstance = new DataTable())
                    {
                        DataColumn column = new DataColumn("AutoId");
                        column.AutoIncrement = true;
                        column.ReadOnly = true;
                        column.Unique = false;
                        dtInstance.Columns.Add(column);

                        dtInstance.Columns.Add("PaymentModeName");
                        dtInstance.Columns.Add("PaymentModeId");
                        dtInstance.Columns.Add("Amount", typeof(decimal));
                        dtInstance.Columns.Add("PaymentDetails");
                        _ClaimPaymentDetails = dtInstance;
                    }
                }

                DataRow drItem = _ClaimPaymentDetails.NewRow();
                drItem["PaymentModeName"] = ddlPaymentModes.SelectedItem.Text;
                drItem["PaymentModeId"] = ddlPaymentModes.SelectedValue;
                drItem["Amount"] = Convert.ToDecimal(txtAmount.Text.Trim());
                drItem["PaymentDetails"] = txtPaymentDetails.Text.Trim();

                _ClaimPaymentDetails.Rows.Add(drItem);
                _ClaimPaymentDetails.AcceptChanges();

                ComputeTotalPaying();
                LoadClaimPaymentDetails();
                ClearPaymentDetailsControls();
                Message.Show = false;
            }
            ModalPopupExtender1.Show();
        }

        private bool ValidatePaymentItem()
        {
            if (!(!string.IsNullOrEmpty(txtAmount.Text.Trim()) && Convert.ToDecimal(txtAmount.Text.Trim()) > 0))
            {
                Message.IsSuccess = false;
                Message.Text = "Please enter amount";
                Message.Show = true;
                return false;
            }
            if ((Convert.ToDecimal(txtAmount.Text.Trim()) + Convert.ToDecimal(lblTotalAmountPaying.Text.Trim())) >
                Convert.ToDecimal(lblTotalApprovedAmount.Text.Trim()))
            {
                Message.IsSuccess = false;
                Message.Text = "Paying amount cannot be more than approved amount.";
                Message.Show = true;
                return false;
            }
            return true;
        }

        private void ComputeTotalPaying()
        {
            if (_ClaimPaymentDetails != null && _ClaimPaymentDetails.AsEnumerable().Any())
                lblTotalAmountPaying.Text = _ClaimPaymentDetails.Compute("SUM(Amount)", string.Empty).ToString();
            else
                lblTotalAmountPaying.Text = "0.00";
        }

        private void ClearPaymentDetailsControls()
        {
            ddlPaymentModes.SelectedIndex = 0;
            txtAmount.Text = "0.00";
            txtPaymentDetails.Text = string.Empty;
        }

        private void LoadClaimPaymentDetails()
        {
            gvPaymentDetails.DataSource = _ClaimPaymentDetails;
            gvPaymentDetails.DataBind();
        }

        protected void btnGenerateVoucher_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateGenerateVoucher())
                {
                    Voucher voucher = PrepareVoucher();
                    Business.ClaimManagement.Voucher objVoucher = new Business.ClaimManagement.Voucher();
                    Business.ClaimManagement.VoucherPayment objVoucherPayment = new Business.ClaimManagement.VoucherPayment();
                    Business.ClaimManagement.VoucherPaymentDetailsDetails objVoucherPaymentDetailsDetails = new Business.ClaimManagement.VoucherPaymentDetailsDetails();
                    int voucherId = objVoucher.Voucher_Save(voucher);

                    VoucherPayment voucherPayment = voucher.voucherPayment;
                    voucherPayment.VoucherId = voucherId;
                    int voucherPaymentId = objVoucherPayment.VoucherPayment_Save(voucherPayment);

                    int retValue = 0;
                    List<VoucherPaymentDetails> voucherPaymentDetailsList = new List<VoucherPaymentDetails>();
                    voucherPaymentDetailsList = voucher.voucherPayment.VoucherPaymentDetailsList;
                    retValue = VoucherPaymentDetails_Save(objVoucherPaymentDetailsDetails, voucherPaymentId, retValue, voucherPaymentDetailsList);

                    DataTable dtVoucher = objVoucher.Voucher_GetById(voucherId);
                    if (dtVoucher != null && dtVoucher.AsEnumerable().Any() && retValue == voucherPaymentDetailsList.Count())
                    {
                        ClaimApplicationStatusUpdate();
                        int claimDisbursementId = ClaimDisbursement_Save(voucherId);
                        ClaimDisbursementDetails_Save(claimDisbursementId);
                        ClaimApplication_GetAll();
                        MessageSuccess.IsSuccess = true;
                        MessageSuccess.Text = string.Format("Voucher generated successfully. Voucher number is {0}", dtVoucher.Rows[0]["VoucherNo"].ToString());
                        MessageSuccess.Show = true;
                    }
                    else
                    {
                        Message.IsSuccess = false;
                        Message.Text = "Voucher generation failed.";
                        Message.Show = true;
                        ModalPopupExtender1.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Message.IsSuccess = false;
                Message.Text = ex.Message;
                Message.Show = true;
                ModalPopupExtender1.Show();
            }
        }

        private void ClaimDisbursementDetails_Save(int claimDisbursementId)
        {
            Business.ClaimManagement.ClaimDisbursementDetails objClaimDisbursementDetails = new Business.ClaimManagement.ClaimDisbursementDetails();
            foreach (GridViewRow gvr in gvApprovedClaim.Rows)
            {
                if (((CheckBox)gvr.FindControl("chkitem")).Checked)
                {
                    ClaimDisbursementDetails claimDisbursementDetails = new ClaimDisbursementDetails()
                    {
                        ClaimDisburseId = claimDisbursementId,
                        ClaimId = Convert.ToInt32(gvApprovedClaim.DataKeys[gvr.RowIndex].Values[0].ToString())
                    };
                    objClaimDisbursementDetails.ClaimDisbursementDetails_Save(claimDisbursementDetails);
                }
            }
        }

        private static int ClaimDisbursement_Save(int voucherId)
        {
            Business.ClaimManagement.ClaimDisbursement objClaimDisbursement = new Business.ClaimManagement.ClaimDisbursement();
            ClaimDisbursement claimDisbursement = new ClaimDisbursement()
            {
                VoucherId = voucherId,
                CreatedBy = Convert.ToInt32(HttpContext.Current.User.Identity.Name)
            };
            int claimDisbursementId = objClaimDisbursement.ClaimDisbursement_Save(claimDisbursement);
            return claimDisbursementId;
        }

        private static int VoucherPaymentDetails_Save(Business.ClaimManagement.VoucherPaymentDetailsDetails objVoucherPaymentDetailsDetails, int voucherPaymentId, int retValue, List<VoucherPaymentDetails> voucherPaymentDetailsList)
        {
            foreach (VoucherPaymentDetails voucherPaymentDetails in voucherPaymentDetailsList)
            {
                voucherPaymentDetails.VoucherPaymentId = voucherPaymentId;
                retValue += objVoucherPaymentDetailsDetails.VoucherPaymentDetails_Save(voucherPaymentDetails);
            }

            return retValue;
        }

        private void ClaimApplicationStatusUpdate()
        {
            Business.ClaimManagement.ClaimApplication objClaimApplication = new Business.ClaimManagement.ClaimApplication();
            foreach (GridViewRow gvr in gvApprovedClaim.Rows)
            {
                if (((CheckBox)gvr.FindControl("chkitem")).Checked)
                {
                    ClaimApplicationMaster claimApplicationMaster = new ClaimApplicationMaster()
                    {
                        ClaimApplicationId = Convert.ToInt32(gvApprovedClaim.DataKeys[gvr.RowIndex].Values[0].ToString()),
                        Status = (int)ClaimStatusEnum.Processed
                    };
                    objClaimApplication.Claim_StatusUpdate(claimApplicationMaster);
                }
            }
        }

        private Voucher PrepareVoucher()
        {
            List<VoucherPaymentDetails> voucherPaymentDetailsList = new List<VoucherPaymentDetails>();

            foreach (DataRow dr in _ClaimPaymentDetails.Rows)
            {
                VoucherPaymentDetails voucherPaymentDetails = new VoucherPaymentDetails()
                {
                    CreatedBy = Convert.ToInt32(HttpContext.Current.User.Identity.Name),
                    PaymentModeId = Convert.ToInt32(dr["PaymentModeId"].ToString()),
                    PaymentAmount = Convert.ToDecimal(dr["Amount"].ToString()),
                    PaymentModeName = dr["PaymentModeName"].ToString(),
                    Description = dr["PaymentDetails"].ToString()
                };
                voucherPaymentDetailsList.Add(voucherPaymentDetails);
            }

            VoucherPayment voucherPayment = new VoucherPayment()
            {
                VoucherPaymentDetailsList = voucherPaymentDetailsList,
                TotalAmount = Convert.ToDecimal(lblTotalApprovedAmount.Text.Trim()),
                CreatedBy = Convert.ToInt32(HttpContext.Current.User.Identity.Name),
            };

            Voucher voucher = new Voucher()
            {
                voucherPayment = voucherPayment,
                CreatedBy = Convert.ToInt32(HttpContext.Current.User.Identity.Name)
            };

            voucher.VoucherJson = JsonConvert.SerializeObject(voucher);

            return voucher;
        }

        private bool ValidateGenerateVoucher()
        {
            if (Convert.ToDecimal(lblTotalAmountPaying.Text.Trim()) != Convert.ToDecimal(lblTotalApprovedAmount.Text.Trim()))
            {
                Message.IsSuccess = false;
                Message.Text = "Paying amount and approved amount must be equal.";
                Message.Show = true;
                return false;
            }
            if (!(_ClaimPaymentDetails != null && _ClaimPaymentDetails.AsEnumerable().Any()))
            {
                Message.IsSuccess = false;
                Message.Text = "Please add payment details.";
                Message.Show = true;
                return false;
            }
            return true;
        }

        private void GetClaimApplicationDetails_ByClaimApplicationId(int ClaimApplicationId)
        {
            DataSet dsClaimApplicationDetails = new Business.ClaimManagement.ClaimApplication().GetClaimApplicationDetails_ByClaimApplicationId(ClaimApplicationId);
            if (dsClaimApplicationDetails != null)
            {
                lblClaimApplicationNumber.Text = dsClaimApplicationDetails.Tables[0].Rows[0]["ClaimNo"].ToString();
                lblName.Text = dsClaimApplicationDetails.Tables[0].Rows[0]["Requestor"].ToString();
                lblFromDate.Text = dsClaimApplicationDetails.Tables[0].Rows[0]["FromDate"].ToString();
                lblToDate.Text = dsClaimApplicationDetails.Tables[0].Rows[0]["ToDate"].ToString();
                lblTotalClaimCount.Text = dsClaimApplicationDetails.Tables[0].Rows[0]["TotalAmount"].ToString();
                lblClaimHeader.Text = dsClaimApplicationDetails.Tables[0].Rows[0]["ClaimHeading"].ToString();
            }

            gvClaimDetails.DataSource = dsClaimApplicationDetails.Tables[2];
            gvClaimDetails.DataBind();

            ComputeTotalApprovedAmount();

            if (dsClaimApplicationDetails.Tables.Count > 1)
            {
                gvApprovalHistory.DataSource = dsClaimApplicationDetails.Tables[1];
                gvApprovalHistory.DataBind();
            }
        }

        private void ComputeTotalApprovedAmount()
        {
            decimal total = 0;
            foreach (GridViewRow gvr in gvClaimDetails.Rows)
            {
                if (!string.IsNullOrEmpty(gvr.Cells[5].Text.ToString()) && !gvr.Cells[5].Text.Equals("&nbsp;"))
                {
                    total += Convert.ToDecimal(gvr.Cells[5].Text.ToString());
                }
            }
            lblTotalApprovedAmount.Text = total.ToString();
        }

        protected void btnPay_Click(object sender, EventArgs e)
        {
            FetchBasicDetails();
            GetClaimAccountBalance();
            VoucherPaymentMode_GetAll();
            ModalPopupExtender1.Show();
        }


        protected void gvClaimDetails_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    LinkButton lnkBtnAttachment = (LinkButton)e.Row.FindControl("lnkBtnAttachment");
                    if (!string.IsNullOrEmpty(((DataTable)gvClaimDetails.DataSource).Rows[e.Row.RowIndex]["Attachment"].ToString()))
                    {
                        lnkBtnAttachment.CssClass = "fa fa-paperclip fa-fw";
                        lnkBtnAttachment.Enabled = true;
                        lnkBtnAttachment.ToolTip = "Click to download";
                    }
                    else
                    {
                        lnkBtnAttachment.CssClass = "fa fa-exclamation-triangle fa-fw";
                        lnkBtnAttachment.Enabled = false;
                        lnkBtnAttachment.ToolTip = "No attachment";
                    }

                    DropDownList ddlLineItemStatus = (DropDownList)e.Row.FindControl("ddlLineItemStatus");
                    //LoadClaimStatus(ddlLineItemStatus);
                    ddlLineItemStatus.SelectedValue = ((DataTable)(gvClaimDetails.DataSource)).Rows[e.Row.RowIndex]["Status"].ToString();

                    HiddenField hdnChecked = (HiddenField)e.Row.FindControl("hdnChecked");
                    if (!string.IsNullOrEmpty(((DataTable)gvClaimDetails.DataSource).Rows[e.Row.RowIndex]["ApprovedAmount"].ToString()))
                    {
                        hdnChecked.Value = "Checked";
                        e.Row.Attributes.CssStyle.Add("color", "#038a10");
                        e.Row.Font.Italic = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.WriteException();
            }
        }


        private void DownloadAttachment(string claimAttachmentName)
        {
            try
            {
                string FileName = claimAttachmentName;
                string OriginalFileName = claimAttachmentName;
                string FilePath = Server.MapPath(" ") + "\\ClaimAttachments\\" + FileName;
                FileInfo file = new FileInfo(FilePath);
                if (file.Exists)
                {
                    Response.ContentType = ContentType;
                    Response.AppendHeader("Content-Disposition", "attachment; filename=" + OriginalFileName);
                    Response.Headers.Set("Cache-Control", "private, max-age=0");
                    Response.WriteFile(FilePath);
                    Response.End();
                }
            }
            catch
            {
                // do nothing
            }
        }

        protected void gvClaimDetails_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "A")
                {
                    string claimAttachmentName = e.CommandArgument.ToString();
                    DownloadAttachment(claimAttachmentName);
                }
            }
            catch (Exception ex)
            {
                ex.WriteException();
                Message.IsSuccess = false;
                Message.Text = ex.Message;
                Message.Show = true;
            }
            finally
            {
                ModalPopupExtender1.Show();
            }
        }
    }
}