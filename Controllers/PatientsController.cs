using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using YourAppNamespace.DTOs;
using YourAppNamespace.Models;
using YourAppNamespace.Data;

namespace YourAppNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PatientsController> _logger;

        public PatientsController(AppDbContext context, ILogger<PatientsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/patients
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var patients = await _context.Patients
                .Include(p => p.Symptoms)
                .ThenInclude(ps => ps.Symptom)
                .Include(p => p.Images)
                .ToListAsync();

            return Ok(patients);
        }

        // GET: api/patients/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var patient = await _context.Patients
                .Include(p => p.Symptoms)
                .ThenInclude(ps => ps.Symptom)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            return patient is null ? NotFound() : Ok(patient);
        }

        // POST: api/patients
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePatientRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var patient = new Patient
            {
                Name = request.Name,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                HeightCm = request.HeightCm,
                WeightKg = request.WeightKg,
                Bmi = CalculateBmi(request.HeightCm, request.WeightKg),
                DiagnosisNote = request.DiagnosisNote ?? "",
                OtherSymptoms = request.OtherSymptoms ?? "",
                ConsentGiven = request.ConsentGiven,
                ImagePath = request.ImagePath,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = patient.Id }, patient);
        }

        // POST: api/patients/sync
        [HttpPost("sync")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> SyncFromApp([FromForm] SyncPatientMultipartDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 1. Parse symptoms
            List<string> parsedSymptoms;
            try
            {
                parsedSymptoms = JsonSerializer.Deserialize<List<string>>(dto.Symptoms ?? "[]") ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Invalid JSON for symptoms: {Json}", dto.Symptoms);
                return BadRequest("Invalid JSON array in 'symptoms' field.");
            }

            // 2. Save image (optional)
            string? imageUrl = null;
            try
            {
                imageUrl = await SaveImageAsync(dto.Image);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Image upload failed.");
                return BadRequest("Image upload failed: " + ex.Message);
            }

            // 3. Create patient
            var patient = new Patient
            {
                Name = dto.Name,
                DateOfBirth = dto.DateOfBirth,
                Gender = dto.Gender,
                HeightCm = dto.HeightCm,
                WeightKg = dto.WeightKg,
                Bmi = dto.Bmi,
                DiagnosisNote = dto.DiagnosisNote ?? "",
                OtherSymptoms = dto.OtherSymptoms ?? "",
                ConsentGiven = dto.ConsentGiven,
                ImagePath = imageUrl,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync(); // Needed to get Patient.Id

            // 4. Add symptom relations
            foreach (var symptomName in parsedSymptoms.Where(s => !string.IsNullOrWhiteSpace(s)))
            {
                var trimmed = symptomName.Trim();

                var existingSymptom = await _context.Symptoms
                    .FirstOrDefaultAsync(s => s.Name == trimmed);

                if (existingSymptom == null)
                {
                    existingSymptom = new Symptom { Name = trimmed };
                    _context.Symptoms.Add(existingSymptom);
                    await _context.SaveChangesAsync();
                }

                _context.PatientSymptomsRecord.Add(new PatientSymptomRecord
                {
                    PatientId = patient.Id,
                    SymptomId = existingSymptom.Id
                });
            }

            // 5. Track sync
            _context.SyncQueue.Add(new SyncQueue
            {
                EntityName = "Patient",
                EntityId = patient.Id,
                CreatedAt = DateTime.UtcNow,
                Synced = true
            });

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                patientId = patient.Id,
                imageUrl,
                message = "✅ Patient and symptoms synced"
            });
        }

        // PUT: api/patients/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Patient updated)
        {
            if (id != updated.Id)
                return BadRequest("ID mismatch");

            updated.UpdatedAt = DateTime.UtcNow;
            _context.Patients.Update(updated);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/patients/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return NotFound();

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private double CalculateBmi(double heightCm, double weightKg)
        {
            var heightM = heightCm / 100.0;
            return heightM > 0 ? weightKg / (heightM * heightM) : 0;
        }

        private async Task<string?> SaveImageAsync(IFormFile? imageFile)
        {
            if (imageFile == null) return null;

            var fileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
            var filePath = Path.Combine("wwwroot", "uploads", fileName);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

            using var stream = new FileStream(filePath, FileMode.Create);
            await imageFile.CopyToAsync(stream);

            return $"/uploads/{fileName}";
        }
    }
}
