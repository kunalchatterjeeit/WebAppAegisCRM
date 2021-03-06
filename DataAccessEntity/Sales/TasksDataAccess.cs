﻿using Entity.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessEntity.Sales
{
    public class TasksDataAccess
    {
        public static List<TaskPriorityDbModel> GetTaskPriority()
        {
            using (var Context = new CRMContext())
            {
                return Context.TaskPriority.ToList();
            }
        }
        public static List<TaskRelatedToDbModel> GetTaskRelatedTo()
        {
            using (var Context = new CRMContext())
            {
                return Context.TaskRelatedTo.ToList();
            }
        }

        public static List<GetTasksDbModel> GetAllTasks(GetTasksParamDbModel Param)
        {
            using (var Context = new CRMContext())
            {
                return Context.Database.SqlQuery<GetTasksDbModel>(
                                "exec dbo.[usp_Sales_Tasks_GetAll] @Subject,@TasksStatusId,@TasksPriorityId,@TasksRelatedId,@StartFromDateTime,@StartToDateTime,@AssignEngineer",
                                new Object[]
                                {
                                    new SqlParameter("Subject", (!string.IsNullOrEmpty(Param.Subject))?Param.Subject:(object)DBNull.Value),
                                    new SqlParameter("TasksStatusId", (Param.TaskStatusId>0)?Param.TaskStatusId:(object)DBNull.Value),
                                    new SqlParameter("TasksPriorityId", (Param.TaskPriorityId>0)?Param.TaskPriorityId:(object)DBNull.Value),
                                    new SqlParameter("TasksRelatedId", (Param.TaskRelatedToId>0)?Param.TaskRelatedToId:(object)DBNull.Value),
                                    new SqlParameter("StartFromDateTime", (Param.StartDateTime!=DateTime.MinValue)?Param.StartDateTime:(object)DBNull.Value),
                                    new SqlParameter("StartToDateTime", (Param.EndDateTime!=DateTime.MinValue)?Param.EndDateTime:(object)DBNull.Value),
                                    new SqlParameter("LinkId", (Param.LinkId>0)?Param.LinkId:(object)DBNull.Value),
                                    new SqlParameter("LinkType", (Param.LinkType != SalesLinkType.None)?(int)Param.LinkType:(object)DBNull.Value),
                                    new SqlParameter("AssignEngineer", (Param.AssignEngineer==0)?(object)DBNull.Value:Param.AssignEngineer)
                                }
                             ).ToList();
            }
        }
        public static TasksDbModel GetTaskById(int Id)
        {
            using (var Context = new CRMContext())
            {
                return Context.Database.SqlQuery<TasksDbModel>(
                                "exec dbo.[usp_Sales_Tasks_GetById] @Id",
                                new Object[]
                                {
                                    new SqlParameter("Id", Id)
                                }
                             ).FirstOrDefault();
            }
        }
        public static int SaveTasks(TasksDbModel Model)
        {
            var outParam = new SqlParameter();
            outParam.ParameterName = "Id";
            outParam.Value = Model.Id;
            outParam.SqlDbType = SqlDbType.BigInt;
            outParam.Direction = ParameterDirection.InputOutput;
            int result = 0;
            using (var Context = new CRMContext())
            {
                Context.Database.ExecuteSqlCommand(
                                "exec dbo.[usp_Sales_Tasks_Save] @Id OUT,@Subject,@Description,@StartDateTime,@EndDateTime," +
                                "@TasksPriorityId,@TasksStatusId,@TasksRelatedTo,@CreatedBy,@IsActive",
                                new Object[]
                                {
                                    outParam,
                                    new SqlParameter("Subject", Model.Subject),
                                    new SqlParameter("Description", Model.Description),
                                    new SqlParameter("StartDateTime", Model.StartDateTime),
                                    new SqlParameter("EndDateTime", Model.EndDateTime),
                                    new SqlParameter("TasksPriorityId", Model.TasksPriorityId),
                                    new SqlParameter("TasksStatusId", Model.TasksStatusId),
                                    new SqlParameter("TasksRelatedTo", Model.TasksRelatedTo),
                                    new SqlParameter("CreatedBy", Model.CreatedBy),
                                    new SqlParameter("IsActive", Model.IsActive)
                                }
                             );
            }
            result = Convert.ToInt32(outParam.Value);
            return result;
        }
        public static int DeleteTask(int Id)
        {
            using (var Context = new CRMContext())
            {
                return Context.Database.ExecuteSqlCommand(
                                "exec dbo.[usp_Sales_Tasks_Delete] @Id",
                                new Object[]
                                {
                                    new SqlParameter("Id",Id)
                                }
                             );
            }
        }

        public static int SaveTaskLinks(TasksDbModel Model)
        {
            using (var Context = new CRMContext())
            {
                return Context.Database.ExecuteSqlCommand(
                                "exec dbo.[usp_Sales_TaskLinks_Save] @Id,@TaskId,@LinkId,@LinkType",
                                new Object[]
                                {
                                    new SqlParameter("Id", Model.TaskLinkId),
                                    new SqlParameter("TaskId", Model.Id),
                                    new SqlParameter("LinkId", Model.LinkId),
                                    new SqlParameter("LinkType", (int)Model.LinkType)
                                }
                             );
            }
        }
    }
}
