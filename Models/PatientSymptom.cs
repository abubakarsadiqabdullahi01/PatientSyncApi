using System;
using System.Collections.Generic;

namespace YourAppNamespace.Models
{
    public class PatientSymptomRecord
    {
        public int Id { get; set; }
        public string? PatientId { get; set; }
        public int SymptomId { get; set; }

        // Navigation
        public Patient? Patient { get; set; }
        public Symptom? Symptom { get; set; }
    }
}
