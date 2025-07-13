using System;

namespace YourAppNamespace.DTOs
{
    public class PatientDto
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = "";
        public double HeightCm { get; set; }
        public double WeightKg { get; set; }
        public double Bmi { get; set; }
        public string? DiagnosisNote { get; set; }
        public string? OtherSymptoms { get; set; }
        public bool ConsentGiven { get; set; }
        public string? ImagePath { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool Synced { get; set; }
        public string? SyncError { get; set; }

        public List<string> Symptoms { get; set; } = new();
        public List<string> Images { get; set; } = new(); // Optional
    }
}
