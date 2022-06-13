﻿using System.Collections.Generic;
using RPS.Core.Models.Enums;

namespace RPS.Core.Models
{
    public class PtItem : PtObjectBase
    {
        public string Description { get; set; }
        public int Estimate { get; set; }
        public PriorityEnum Priority { get; set; }
        public StatusEnum Status { get; set; }
        public ItemTypeEnum Type { get; set; }
        public PtUser Assignee { get; set; }
        public List<PtTask> Tasks { get; set; }
        public List<PtComment> Comments { get; set; }

        public PtItem()
        {
            Tasks = new List<PtTask>();
            Comments = new List<PtComment>();
        }
    }
}
