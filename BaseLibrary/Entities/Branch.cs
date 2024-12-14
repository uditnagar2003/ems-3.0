﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BaseLibrary.Entities
{
    public class Branch : BaseEntity
    {
        // Many to One Relationship with Department

        public Department? Department { get; set; }

        public int DepartmentId {  get; set; }
        [JsonIgnore]
        // one to Many Relationship with employee
        public List<Employee>? Employees { get; set; }
    }
}
