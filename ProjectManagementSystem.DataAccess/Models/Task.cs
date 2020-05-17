using System;
using System.Collections.Generic;

namespace ProjectManagementSystem.DataAccess.Models
{
    public partial class Task
    {
        public Task()
        {
            InverseParent = new HashSet<Task>();
        }

        public int TaskId { get; set; }
        public int? ParentId { get; set; }
        public int ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public int State { get; set; }

        public virtual Task Parent { get; set; }
        public virtual Project Project { get; set; }
        public virtual ICollection<Task> InverseParent { get; set; }
    }
}
