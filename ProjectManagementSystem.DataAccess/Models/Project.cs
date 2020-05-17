using System;
using System.Collections.Generic;

namespace ProjectManagementSystem.DataAccess.Models
{
    public partial class Project
    {
        public Project()
        {
            InverseParent = new HashSet<Project>();
            Task = new HashSet<Task>();
        }

        public int ProjectId { get; set; }
        public int? ParentId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public int State { get; set; }

        public virtual Project Parent { get; set; }
        public virtual ICollection<Project> InverseParent { get; set; }
        public virtual ICollection<Task> Task { get; set; }
    }
}
