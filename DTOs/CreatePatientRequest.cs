using System;
using System.ComponentModel.DataAnnotations;

namespace YourAppNamespace.DTOs
{
    public class CreatePatientRequest
    {
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of birth is required.")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        [RegularExpression("Male|Female", ErrorMessage = "Gender must be either 'Male' or 'Female'.")]
        public string Gender { get; set; } = string.Empty;

        [Range(30, 300, ErrorMessage = "Height must be between 30 and 300 cm.")]
        public double HeightCm { get; set; }

        [Range(1, 300, ErrorMessage = "Weight must be between 1 and 300 kg.")]
        public double WeightKg { get; set; }

        public string? DiagnosisNote { get; set; }

        public string? OtherSymptoms { get; set; }

        public string? ImagePath { get; set; }

        public bool ConsentGiven { get; set; } = false;
    }
}
