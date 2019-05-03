﻿<%@ Page Title="SALE CHALLAN LIST" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="SaleChallanList.aspx.cs" Inherits="WebAppAegisCRM.Sale.SaleChallanList" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="../UserControl/Message.ascx" TagName="Message" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ToolkitScriptManager>
    <br />
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1" DisplayAfter="1">
        <ProgressTemplate>
            <div class="divWaiting">
                <div class="loading">
                    <div class="loading-bar"></div>
                    <div class="loading-bar"></div>
                    <div class="loading-bar"></div>
                    <div class="loading-bar"></div>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="row">
                <div class="col-lg-12">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            List Filter
                        </div>
                        <div class="panel-body">
                            <div class="col-lg-3">
                                <div class="form-group">
                                    Challan No :
                                <asp:TextBox ID="txtChallanNo" CssClass="form-control" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-lg-3">
                                <div class="form-group">
                                    Order No :
                                <asp:TextBox ID="txtOrderNo" CssClass="form-control" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-lg-3">
                                <div class="form-group">
                                    Sale From Date :
                                <asp:TextBox ID="txtSaleFromDate" CssClass="form-control" runat="server"></asp:TextBox>
                                    <asp:CalendarExtender ID="CalendarExtender4" runat="server" Enabled="True"
                                        Format="dd MMM yyyy" TargetControlID="txtSaleFromDate">
                                    </asp:CalendarExtender>
                                </div>
                            </div>
                            <div class="col-lg-3">
                                <div class="form-group">
                                    Sale To Date :
                                <asp:TextBox ID="txtSaleToDate" CssClass="form-control" runat="server"></asp:TextBox>
                                    <asp:CalendarExtender ID="CalendarExtender3" runat="server" Enabled="True"
                                        Format="dd MMM yyyy" TargetControlID="txtSaleToDate">
                                    </asp:CalendarExtender>
                                </div>
                            </div>
                            <div class="col-lg-3">
                                <div class="form-group">
                                    Challan Type
                                <asp:DropDownList ID="ddlChallanType" CssClass="form-control" runat="server">
                                    <asp:ListItem Value="0">--SELECT--</asp:ListItem>
                                    <asp:ListItem Value="1">Sale</asp:ListItem>
                                </asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-lg-1">
                                <div class="form-group">
                                    <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-outline btn-success" OnClick="btnSearch_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-lg-12">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            Sale Challan List
                        </div>
                        <div class="panel-body">
                            <div class="table-responsive">
                                <asp:GridView ID="gvSale" DataKeyNames="SaleChallanId" runat="server"
                                    AutoGenerateColumns="False" Width="100%" CellPadding="4" ForeColor="#333333"
                                    class="table table-striped" GridLines="None" Style="text-align: left" 
                                    PageSize="20" AllowPaging="true" OnPageIndexChanging="gvSale_PageIndexChanging"
                                    OnRowCommand="gvSale_RowCommand">
                                    <Columns>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                SN.
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <%# Container.DataItemIndex+1 %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField HeaderText="Challan No" DataField="ChallanNo" />
                                        <asp:BoundField HeaderText="Order No" DataField="OrderNo" />
                                        <asp:BoundField HeaderText="Order Date" DataField="OrderDate" />
                                        <asp:BoundField HeaderText="Sale Date" DataField="SaleDate" />
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:Button ID="btnSaleDetails" runat="server" Text="Details" CommandName="SaleDetails" class="btn btn-outline btn-success"
                                                    CommandArgument='<%# Eval("SaleChallanId") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <FooterStyle BackColor="#5bb0de" Font-Bold="True" ForeColor="White" />
                                    <HeaderStyle BackColor="#379ed6" Font-Bold="True" ForeColor="White" />
                                    <RowStyle CssClass="RowStyle" BackColor="#F7F6F3" ForeColor="#333333" />
                                    <EditRowStyle BackColor="#999999" />
                                    <EmptyDataRowStyle CssClass="EditRowStyle" />
                                    <AlternatingRowStyle CssClass="AltRowStyle" BackColor="White" ForeColor="#284775" />
                                    <PagerStyle CssClass="PagerStyle" BackColor="#379ed6" ForeColor="White" HorizontalAlign="Center" />
                                    <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                    <EmptyDataTemplate>
                                        No Record Found...
                                    </EmptyDataTemplate>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <a id="lnk" runat="server"></a>
            <asp:ModalPopupExtender ID="ModalPopupExtender1" BackgroundCssClass="myModalPopupbackGrnd"
                runat="server" TargetControlID="lnk" PopupControlID="Panel1" CancelControlID="imgbtn">
            </asp:ModalPopupExtender>
            <asp:Panel ID="Panel1" runat="server" CssClass="myModalPopup-10" Style="display: none; z-index: 10000; position: absolute">
                <asp:Panel ID="dragHandler" runat="server" class="popup-working-section" ScrollBars="Auto">
                    <h6 id="popupHeader1" runat="server" class="popup-header-companyname"></h6>
                    <div style="width: 1055px; overflow: scroll">
                        <asp:GridView ID="gvSaleDetails" DataKeyNames="SaleChallanDetailsId" runat="server"
                            AutoGenerateColumns="False" CellPadding="4" ForeColor="#333333"
                            class="table table-striped" GridLines="None" Style="text-align: left">
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        SN.
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <%# Container.DataItemIndex+1 %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField HeaderText="Name" DataField="ItemName" />
                                <asp:BoundField HeaderText="Type" DataField="ItemType" />
                                <asp:BoundField HeaderText="Quantity" DataField="ItemQty" />
                                <asp:BoundField HeaderText="Rate" DataField="ItemRate" />
                                <asp:BoundField HeaderText="GST" DataField="GST" />
                                <asp:BoundField HeaderText="HSN Code" DataField="HSNCode" />
                            </Columns>
                            <FooterStyle BackColor="#5bb0de" Font-Bold="True" ForeColor="White" />
                            <HeaderStyle BackColor="#5bc0de" Font-Bold="True" ForeColor="White" Wrap="false" />
                            <RowStyle CssClass="RowStyle" BackColor="#F7F6F3" ForeColor="#333333" Wrap="false" />
                            <EditRowStyle BackColor="#999999" />
                            <EmptyDataRowStyle CssClass="EditRowStyle" />
                            <AlternatingRowStyle CssClass="AltRowStyle" BackColor="White" ForeColor="#284775" />
                            <PagerStyle CssClass="PagerStyle" BackColor="#379ed6" ForeColor="White" HorizontalAlign="Center" />
                            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                            <EmptyDataTemplate>
                                No Record Found...
                            </EmptyDataTemplate>
                        </asp:GridView>
                    </div>
                </asp:Panel>
                <img id="imgbtn" runat="server" src="../images/close-button.png" style="float: right; margin-right: 1px; cursor: pointer"
                    alt="Close" />
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>