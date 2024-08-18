using Domain.Entities.Base.Inteface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Base
{
    public abstract class AuditEntity<TKey> : DeleteEntity<TKey>, IAuditEntity<TKey>
    {
        public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;
        public String CreatedByName { get; set; } = String.Empty;
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public String? UpdatedByName { get; set; }
        public Guid? UpdateById { get; set; }
    }
}
