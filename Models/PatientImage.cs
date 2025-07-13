using System;

namespace YourAppNamespace.Models
{
    public class PatientImage
    {
        public int Id { get; set; }
        public string? PatientId { get; set; }
        public string? Type { get; set; } // e.g., "form", "rash"
        public string? FilePath { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Patient? Patient { get; set; }
    }
}