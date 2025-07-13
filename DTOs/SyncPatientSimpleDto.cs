using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace YourAppNamespace.DTOs
{
    public class SyncPatientMultipartDto
    {
        [Required]
        public string Name { get; set; } = "";

        [Required]
        public string Gender { get; set; } = "";

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Range(0, 300)]
        public double HeightCm { get; set; }

        [Range(0, 300)]
        public double WeightKg { get; set; }

        [Range(0, 100)]
        public double Bmi { get; set; }

        public string? DiagnosisNote { get; set; }

        public string? OtherSymptoms { get; set; }

        public bool ConsentGiven { get; set; }

        [Required]
        public string Symptoms { get; set; } = "[]"; // JSON array from Flutter

        public IFormFile? Image { get; set; } // Uploaded file
    }
}
