using System.Collections.Generic;

namespace YourAppNamespace.Models
{
    public class DiagnosisRule
    {
        public int Id { get; set; }
        public string? SymptomCombo { get; set; } // Store as comma-separated string
        public string? Suggestion { get; set; }
    }
}
