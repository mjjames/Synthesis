using mjjames.AdminSystem.classes;
using mjjames.AdminSystem.dataContexts;
using mjjames.AdminSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace mjjames.AdminSystem.Services
{
    public class AuditLogService
    {
        public static void LogItem(string area, AuditEvent eventType, string user, string details)
        {
            var item = new AuditLogItem
            {
                Area = area,
                Details = details,
                EventType = eventType.ToString(),
                Timestamp = DateTimeOffset.Now,
                User = user
            };
            var connection = ConfigurationManager.ConnectionStrings["auditLog"];
            if (connection == null)
            {
                return;
            }
            using (var context = new AuditLogDataClassesDataContext(connection.ConnectionString))
            {
                try
                {
                    context.AuditLogItems.InsertOnSubmit(item);
                    context.SubmitChanges();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Unable to log audit item, {0}", ex.ToString());
                    new Logger("AuditLogService").LogError("Unable to log audit item", ex);
                }
            }
        }
    }
}