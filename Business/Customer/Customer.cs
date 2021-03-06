﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Entity.Customer;
using Business.Common;

namespace Business.Customer
{
    public class Customer
    {
        public long Save(Entity.Customer.Customer ObjElCustomer)
        {
            return DataAccess.Customer.Customer.Save(ObjElCustomer);
        }
        public static long GetCustomerIdByNameFromCache(string name)
        {
            long retValue = 0;

            DataTable dtCustomer = GlobalCache.ExecuteCache<DataTable>(typeof(Business.Customer.Customer), "Customer_GetAll", new Entity.Customer.Customer());

            using (DataView dvCustomers = new DataView(dtCustomer))
            {
                dvCustomers.RowFilter = "CustomerName = '" + name + "'";
                dtCustomer = dvCustomers.ToTable();
                if (dtCustomer == null || dtCustomer.Rows.Count == 0)
                    throw new Exception("Something went wrong! Customer ID not found.");
            }
            retValue = Convert.ToInt64(dtCustomer.AsEnumerable().Select(x => x[0].ToString()).ToList().FirstOrDefault());

            return retValue;
        }
        public DataSet GetAllCustomer(Entity.Customer.Customer customer)
        {
            return DataAccess.Customer.Customer.Customer_GetAll(customer);
        }

        public static DataTable Customer_GetAll(Entity.Customer.Customer customer)
        {
            return DataAccess.Customer.Customer.Customer_Customer_GetByAssignEngineerId(customer);
        }

        public DataTable Customer_Customer_GetByAssignEngineerId(Entity.Customer.Customer customer)
        {
            return DataAccess.Customer.Customer.Customer_Customer_GetByAssignEngineerId(customer);
        }

        public DataSet Customer_CustomerMaster_GetByAssignEngineerIdWithPaging(Entity.Customer.Customer customer)
        {
            return DataAccess.Customer.Customer.Customer_CustomerMaster_GetByAssignEngineerIdWithPaging(customer);
        }

        public DataTable FetchCustomerDetailsById(Entity.Customer.Customer customer)
        {
            return DataAccess.Customer.Customer.FetchCustomerDetailsById(customer);
        }

        public int DeleteCustomer(Entity.Customer.Customer customer)
        {
            return DataAccess.Customer.Customer.DeleteCustomer(customer);
        }

        public string CustomerPurchase_Save(Entity.Customer.Customer customerMaster)
        {
            return DataAccess.Customer.Customer.CustomerPurchase_Save(customerMaster);
        }

        public DataTable CustomerPurchase_GetByCustomerId(int customerid)
        {
            return DataAccess.Customer.Customer.CustomerPurchase_GetByCustomerId(customerid);
        }

        public DataTable Customer_CustomerPurchase_GetByCustomerId_ForTonner(int customerid)
        {
            return DataAccess.Customer.Customer.Customer_CustomerPurchase_GetByCustomerId_ForTonner(customerid);
        }

        public Entity.Customer.Customer CustomerPurchase_GetByCustomerPurchaseId(int customerpurchaseid)
        {
            return DataAccess.Customer.Customer.CustomerPurchase_GetByCustomerPurchaseId(customerpurchaseid);
        }

        public int CustomerPurchase_DeleteByCustomerPurchaseId(Int64 customerpurchaseid)
        {
            return DataAccess.Customer.Customer.CustomerPurchase_DeleteByCustomerPurchaseId(customerpurchaseid);
        }

        public DataTable CustomerAuthentication(string emailId, string mobileNo)
        {
            return DataAccess.Customer.Customer.CustomerAuthentication(emailId, mobileNo);
        }

        public int Customer_CustomerPurchaseAssignEngineer_Save(long customerpurchaseid, int assignedEngineerId)
        {
            return DataAccess.Customer.Customer.Customer_CustomerPurchaseAssignEngineer_Save(customerpurchaseid, assignedEngineerId);
        }

        public DataSet Customer_CustomerPurchase_GetAll(Entity.Customer.Customer customer)
        {
            return DataAccess.Customer.Customer.Customer_CustomerPurchase_GetAll(customer);
        }
    }
}
