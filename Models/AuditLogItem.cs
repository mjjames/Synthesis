using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mjjames.AdminSystem.Models
{
    public class AuditLogItemDto
    {
        public DateTimeOffset Timestamp { get; set; }
        public string User { get; set; }
        public string Area { get; set; }
        public string Details { get; set; }
        public AuditEvent Event { get; set; }
    }

    public enum AuditEvent
    {
        Login,
        Created,
        Updated,
        Deleted
    }
}