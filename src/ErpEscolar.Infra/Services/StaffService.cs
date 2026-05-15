using ErpEscolar.Core.Entities;
using ErpEscolar.Core.Interfaces;
using ErpEscolar.Core.Services;
using Microsoft.EntityFrameworkCore;

namespace ErpEscolar.Infra.Services;

public class StaffService : IStaffService
{
    private readonly IStaffRepository _repo;
    private readonly IUserRepository _userRepo;

    public StaffService(IStaffRepository repo, IUserRepository userRepo)
    {
        _repo = repo;
        _userRepo = userRepo;
    }

    public async Task<List<StaffResponse>> GetAllAsync(Guid orgId)
    {
        var staff = await _repo.GetAllAsync(orgId);
        return staff.Select(s => new StaffResponse(
            s.Id, s.User.Name, s.User.Email, s.Position,
            s.Department, s.User.Phone, s.HireDate, s.Active
        )).ToList();
    }

    public async Task<StaffResponse?> GetByIdAsync(Guid id)
    {
        var s = await _repo.GetByIdAsync(id);
        if (s == null) return null;
        return new StaffResponse(s.Id, s.User.Name, s.User.Email, s.Position,
            s.Department, s.User.Phone, s.HireDate, s.Active);
    }

    public async Task<StaffResponse> CreateAsync(CreateStaffRequest request, Guid orgId)
    {
        var existing = await _userRepo.GetByEmailAsync(request.Email);
        if (existing != null)
            throw new InvalidOperationException("Email ja cadastrado");

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = "staff",
            Phone = request.Phone,
            OrganizationId = orgId
        };
        user = await _userRepo.CreateAsync(user);

        var staff = new Staff
        {
            UserId = user.Id,
            Position = request.Position,
            Department = request.Department,
            OrganizationId = orgId,
            Salary = request.Salary
        };
        staff = await _repo.CreateAsync(staff);

        return new StaffResponse(staff.Id, user.Name, user.Email, staff.Position,
            staff.Department, user.Phone, staff.HireDate, true);
    }

    public async Task<StaffResponse> UpdateAsync(Guid id, UpdateStaffRequest request, Guid orgId)
    {
        var staff = await _repo.GetByIdAsync(id);
        if (staff == null) throw new KeyNotFoundException("Funcionario nao encontrado");

        if (request.Name != null) staff.User.Name = request.Name;
        if (request.Position != null) staff.Position = request.Position;
        if (request.Department != null) staff.Department = request.Department;
        if (request.Phone != null) staff.User.Phone = request.Phone;
        if (request.Salary.HasValue) staff.Salary = request.Salary;

        staff.User.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(staff);

        return new StaffResponse(staff.Id, staff.User.Name, staff.User.Email, staff.Position,
            staff.Department, staff.User.Phone, staff.HireDate, staff.Active);
    }

    public async Task ToggleStatusAsync(Guid id)
    {
        var staff = await _repo.GetByIdAsync(id);
        if (staff == null) throw new KeyNotFoundException("Funcionario nao encontrado");
        staff.Active = !staff.Active;
        staff.User.Active = staff.Active;
        await _repo.UpdateAsync(staff);
    }
}
