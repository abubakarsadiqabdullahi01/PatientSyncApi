using Microsoft.EntityFrameworkCore;
using YourAppNamespace.Data;
using YourAppNamespace.Models;

public class PatientRepository : IPatientRepository
{
    private readonly AppDbContext _context;

    public PatientRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Patient>> GetAllAsync()
        => await _context.Patients.Include(p => p.Symptoms).ToListAsync();

    public async Task<Patient?> GetByIdAsync(string id)
        => await _context.Patients
            .Include(p => p.Symptoms)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task AddAsync(Patient patient)
    {
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Patient patient)
    {
        _context.Patients.Update(patient);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string id)
    {
        var patient = await GetByIdAsync(id);
        if (patient is not null)
        {
            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
        }
    }
}
