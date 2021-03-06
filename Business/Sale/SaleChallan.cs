﻿using System;
using System.Data;
using Business.Purchase;

namespace Business.Sale
{
    public class SaleChallan
    {
        public SaleChallan() { }

        public int SaleChallan_Save(Entity.Sale.SaleChallan saleChallan)
        {
            return DataAccess.Sale.SaleChallan.SaleChallan_Save(saleChallan);
        }
        public int Sale_ChallanDetails_Save(Entity.Sale.SaleChallan saleChallan)
        {
            return DataAccess.Sale.SaleChallan.Sale_ChallanDetails_Save(saleChallan);
        }
        public DataTable Sale_Challan_GetAll(Entity.Sale.SaleChallan saleChallan)
        {
            return DataAccess.Sale.SaleChallan.Sale_Challan_GetAll(saleChallan);
        }
        public DataTable Sale_Challan_GetById(int saleChallanid)
        {
            return DataAccess.Sale.SaleChallan.Sale_Challan_GetById(saleChallanid);
        }

        public DataTable SaleChallanDetails_GetBySaleChallanId(long saleChallanId)
        {
            return DataAccess.Sale.SaleChallan.SaleChallanDetails_GetBySaleChallanId(saleChallanId);
        }

        public int Sale_Challan_Delete(int saleChallanId)
        {
            return DataAccess.Sale.SaleChallan.Sale_Challan_Delete(saleChallanId);
        }
    }
}
