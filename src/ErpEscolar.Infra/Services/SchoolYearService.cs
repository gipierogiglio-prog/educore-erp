using ErpEscolar.Core.Entities;
using ErpEscolar.Core.Interfaces;
using ErpEscolar.Core.Services;

namespace ErpEscolar.Infra.Services;

public class SchoolYearService : ISchoolYearService
{
    private readonly ISchoolYearRepository _repo;

    public SchoolYearService(ISchoolYearRepository repo) => _repo = repo;

    public async Task<List<SchoolYearResponse>> GetAllAsync()
    {
        var years = await _repo.GetAllAsync();
        return years.Select(y => new SchoolYearResponse(
            y.Id, y.Year, y.Description, y.StartDate, y.EndDate, y.Status
        )).ToList();
    }

    public async Task<SchoolYearResponse?> GetByIdAsync(Guid id)
    {
        var y = await _repo.GetByIdAsync(id);
        if (y == null) return null;
        return new SchoolYearResponse(y.Id, y.Year, y.Description, y.StartDate, y.EndDate, y.Status);
    }

    public async Task<SchoolYearResponse> CreateAsync(CreateSchoolYearRequest request)
    {
        var existing = await _repo.GetByYearAsync(request.Year);
        if (existing != null)
            throw new InvalidOperationException($"Ano letivo {request.Year} já cadastrado");

        var schoolYear = new SchoolYear
        {
            Year = request.Year,
            Description = request.Description,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Status = "planned",
        };

        schoolYear = await _repo.CreateAsync(schoolYear);
        return new SchoolYearResponse(
            schoolYear.Id, schoolYear.Year, schoolYear.Description,
            schoolYear.StartDate, schoolYear.EndDate, schoolYear.Status
        );
    }

    public async Task UpdateStatusAsync(Guid id, string status)
    {
        var schoolYear = await _repo.GetByIdAsync(id);
        if (schoolYear == null) throw new KeyNotFoundException("Ano letivo não encontrado");
        schoolYear.Status = status;
        await _repo.UpdateAsync(schoolYear);
    }
}
