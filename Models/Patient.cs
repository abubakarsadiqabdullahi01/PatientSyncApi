using System;
using System.Collections.Generic;

namespace YourAppNamespace.Models
{
    public class Patient
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public required string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public required string Gender { get; set; }
        public double HeightCm { get; set; }
        public double WeightKg { get; set; }
        public double Bmi { get; set; }
        public string OtherSymptoms { get; set; } = string.Empty;
        public string DiagnosisNote { get; set; } = string.Empty;
        public bool ConsentGiven { get; set; }
        public string? ImagePath { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool Synced { get; set; } = false;
        public string? SyncError { get; set; }

        // Navigation
        public ICollection<PatientSymptomRecord> Symptoms { get; set; } = new List<PatientSymptomRecord>();
        public ICollection<PatientImage> Images { get; set; } = new List<PatientImage>();
    }
}