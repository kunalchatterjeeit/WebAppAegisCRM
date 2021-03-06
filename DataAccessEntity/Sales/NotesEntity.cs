﻿using Entity.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace DataAccessEntity.Sales
{
    public class NotesDbModel
    {
        [Key]
        public Int64 Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ContactId { get; set; }
        public int CreatedBy { get; set; }     
        public bool IsActive { get; set; }
        public int NoteLinkId { get; set; }
        public int LinkId { get; set; }
        public SalesLinkType LinkType { get; set; }
    }
    public class GetNotesDbModel
    {
        public Int64 Id { get; set; }     
        public string Name { get; set; }
        public string ContactName { get; set; }
        public string CallStatus { get; set; }     
    }

    public class GetNotesParamDbModel
    {
        public string Name { get; set; }
        public int? ContactId { get; set; }
        public int LinkId { get; set; }
        public SalesLinkType LinkType { get; set; }
        public int AssignEngineer { get; set; }
    }
}
