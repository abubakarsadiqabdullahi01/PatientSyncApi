using System;

namespace YourAppNamespace.Models
{
    public class SyncQueue
    {
        public int Id { get; set; }
        public string EntityName { get; set; } = string.Empty; // e.g., "Patient"
        public string EntityId { get; set; } = string.Empty;    // GUID or FK to the record
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool Synced { get; set; } = false;
        public string? SyncError { get; set; }
    }
}
