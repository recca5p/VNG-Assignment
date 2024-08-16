using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Base.Inteface
{
    public interface IAuditEntity
    {
        DateTime CreatedDateTime { get; set; }
        String CreatedByName { get; set; }
        Int64 CreatedById { get; set; }
        DateTime? UpdatedTime { get; set; }
        String? UpdatedByName { get; set; }
        Int64? UpdateById { get; set; }
    }
    public interface IAuditEntity<TKey> : IAuditEntity, IDeleteEntity<TKey>
    {
    }
}
