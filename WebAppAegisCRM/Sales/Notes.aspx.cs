﻿using Business.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebAppAegisCRM.Sales
{
    public partial class Notes : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Business.Common.Context.ReferralUrl = Request.UrlReferrer.AbsoluteUri;
            if (string.IsNullOrEmpty(hdnItemType.Value) || string.IsNullOrEmpty(hdnItemId.Value))
            {
                ModalPopupExtender1.Show();
            }
            if (!IsPostBack)
            {
                LoadContacts();
                LoadNotesList();
                Message.Show = false;
            }
        }
        public int NoteId
        {
            get { return Convert.ToInt32(ViewState["Id"]); }
            set { ViewState["Id"] = value; }
        }
        private void LoadContacts()
        {
            Business.Sales.Notes Obj = new Business.Sales.Notes();
            ddlContact.DataSource = Obj.GetAllContacts();
            ddlContact.DataTextField = "Name";
            ddlContact.DataValueField = "Id";
            ddlContact.DataBind();
            ddlContact.InsertSelect();
        }
        private void LoadNotesList()
        {
            Business.Sales.Notes Obj = new Business.Sales.Notes();
            Entity.Sales.GetNotesParam Param = new Entity.Sales.GetNotesParam { ContactId = null, Name = null };
            gvNotes.DataSource = Obj.GetAllNotes(Param);
            gvNotes.DataBind();
        }
        private void ClearControls()
        {
            NoteId = 0;
            Message.Show = false;
            txtName.Text = string.Empty;
            txtDescription.Text = string.Empty;            
            ddlContact.SelectedIndex = 0;           
            btnSave.Text = "Save";
        }
        private bool NoteControlValidation()
        {
            if (txtName.Text.Trim() == string.Empty)
            {
                Message.IsSuccess = false;
                Message.Text = "Please Enter Note Name";
                Message.Show = true;
                return false;
            }            
            else if (ddlContact.SelectedIndex == 0)
            {
                Message.IsSuccess = false;
                Message.Text = "Please Select Contact Name";
                Message.Show = true;
                return false;
            }
            return true;
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ClearControls();
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            Save();
        }
        protected void gvNotes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Ed")
            {
                NoteId = Convert.ToInt32(e.CommandArgument.ToString());
                GetNoteById();
                Message.Show = false;
                btnSave.Text = "Update";
            }
            else if (e.CommandName == "Del")
            {
                Business.Sales.Notes Obj = new Business.Sales.Notes();
                int rows = Obj.DeleteNotes(Convert.ToInt32(e.CommandArgument.ToString()));
                if (rows > 0)
                {
                    ClearControls();
                    LoadNotesList();
                    Message.IsSuccess = true;
                    Message.Text = "Deleted Successfully";
                }
                else
                {
                    Message.IsSuccess = false;
                    Message.Text = "Data Dependency Exists";
                }
                Message.Show = true;
            }
        }
        private void GetNoteById()
        {
            Business.Sales.Notes Obj = new Business.Sales.Notes();
            Entity.Sales.Notes notes = Obj.GetNoteById(NoteId);
            if (notes.Id != 0)
            {                
                ddlContact.SelectedValue = notes.ContactId.ToString();
                txtDescription.Text = notes.Description;
                txtName.Text = notes.Name.ToString();                
            }
        }
        private void Save()
        {
            if (NoteControlValidation())
            {
                Business.Sales.Notes Obj = new Business.Sales.Notes();
                Entity.Sales.Notes Model = new Entity.Sales.Notes
                {
                    Id = NoteId,
                    ContactId = Convert.ToInt32(ddlContact.SelectedValue),                   
                    CreatedBy = Convert.ToInt32(HttpContext.Current.User.Identity.Name),
                    Description = txtDescription.Text,
                    Name = txtName.Text,                    
                    IsActive = true
                };
                int rows = Obj.SaveNotes(Model);
                if (rows > 0)
                {
                    ClearControls();
                    LoadNotesList();
                    NoteId = 0;
                    Message.IsSuccess = true;
                    Message.Text = "Saved Successfully";
                }
                else
                {
                    Message.IsSuccess = false;
                    Message.Text = "Unable to save data.";
                }
                Message.Show = true;
            }
        }
    }
}