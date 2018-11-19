﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Entity.HR;

namespace DataAccess.LeaveManagement
{
    public class LeaveApplicationMaster
    {
       
        public static int LeaveApplicationMaster_Save(Entity.LeaveManagement.LeaveApplicationMaster objLeaveApplicationMaster)
        {
            int retValue = 0;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "usp_HR_LeaveApplicationMaster_Save";

                    cmd.Parameters.AddWithValue("@LeaveApplicationId", objLeaveApplicationMaster.LeaveApplicationId);
                    cmd.Parameters.AddWithValue("@LeaveApplicationNumber", objLeaveApplicationMaster.LeaveApplicationNumber);
                    cmd.Parameters.AddWithValue("@RequestorId", objLeaveApplicationMaster.RequestorId);
                    cmd.Parameters.AddWithValue("@LeaveTypeId", objLeaveApplicationMaster.LeaveTypeId);
                    cmd.Parameters.AddWithValue("@LeaveAccumulationTypeId", objLeaveApplicationMaster.LeaveAccumulationTypeId);
                    cmd.Parameters.AddWithValue("@FromDate", objLeaveApplicationMaster.FromDate);
                    cmd.Parameters.AddWithValue("@ToDate", objLeaveApplicationMaster.ToDate);
                    cmd.Parameters.AddWithValue("@LeaveStatusId", objLeaveApplicationMaster.LeaveStatusId);
                    cmd.Parameters.AddWithValue("@Reason", objLeaveApplicationMaster.Reason);
                    cmd.Parameters.AddWithValue("@Attachment", objLeaveApplicationMaster.Attachment);


                    if (con.State == ConnectionState.Closed)
                        con.Open();
                    retValue = cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            return retValue;
        }
        public static int LeaveApplicationMaster_GetAll(Entity.LeaveManagement.LeaveApplicationMaster objLeaveApplicationMaster)
        {
            throw new NotImplementedException();
        }

        public static DataTable LeaveApplicationMaster_GetAll()
        {
            using (DataTable dt = new DataTable())
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ToString()))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "usp_HR_LeaveApplicationMaster_GetAll";
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                        con.Close();
                    }
                }
                return dt;
            }
        }
        public static int LeaveApplicationMaster_Delete(Entity.LeaveManagement.LeaveApplicationMaster objLeaveApplicationMaster)
        {
            int rowsAffacted = 0;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "usp_HR_LeaveApplicationMaster_Delete";

                    cmd.Parameters.AddWithValue("@LeaveApplicationId", objLeaveApplicationMaster);

                    if (con.State == ConnectionState.Closed)
                        con.Open();
                    rowsAffacted = cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            return rowsAffacted;
        }






    }
}