using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using serverLibrary.Data;
using serverLibrary.Respositories.contract;
namespace serverLibrary.Respositories.Implementations
{
    public class EmployeeRepository(AppDbContext appDbContext) : IGenericRepositoryInterface<Employee>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var item = await appDbContext.Employees.FindAsync(id);
            if (item is null)
                return NotFound();
            appDbContext.Employees.Remove(item);
            await Commit();
            return Success();
        }

        public async Task<List<Employee>> GetAll()
        {
            var employees =
            await appDbContext
            .Employees
            .AsNoTracking()
            .Include(t => t.Town)
            .ThenInclude(ci => ci.City)
            .ThenInclude(co => co.Country)
            .Include(b => b.Branch)
            .ThenInclude(d => d.Department)
            .ThenInclude(gd => gd.GeneralDepartment)
            .ToListAsync();
            return employees;
        }
        public async Task<Employee> GetById(int id)
        {
            var employee =
            await appDbContext
            .Employees
            .AsNoTracking()
            .Include(t => t.Town)
            .ThenInclude(ci => ci.City)
            .ThenInclude(co => co.Country)
            .Include(b => b.Branch)
            .ThenInclude(d => d.Department)
            .ThenInclude(gd => gd.GeneralDepartment)
            .FirstOrDefaultAsync(f=>f.id==id);
            return employee!;
        }
        public async Task<GeneralResponse> Insert(Employee item)
        {
            if (!await CheckName(item.name!)) return new GeneralResponse(false, "Employee already exists");
            appDbContext.Employees.Add(item);
            await Commit();
            return Success();

        }

        private async Task<bool> CheckName(string v)
        {
            var item = await appDbContext.Employees.FirstOrDefaultAsync(x => x.name.ToLower().Equals(v.ToLower()));
            return item is null;

        }

        public async Task<GeneralResponse> Update(Employee item)
        {
            var finduser = await appDbContext.Employees.FirstOrDefaultAsync(e => e.id == item.id);
            if (finduser is null) return new GeneralResponse(false, "EMployee Does Not Exist");

            finduser.name = item.name;
            finduser.Other = item.Other;
            finduser.Address = item.Address;
            finduser.TelephoneNumber = item.TelephoneNumber;
            finduser.BranchId = item.BranchId;
            finduser.CivilId = item.CivilId;
            finduser.FileNumber = item.FileNumber;
            finduser.JobName = item.JobName;
            finduser.Photo = item.Photo;
            await appDbContext.SaveChangesAsync();
            await Commit();
            return Success();
        }
        private static GeneralResponse NotFound() => new(false, "Sorry No Employee of this name present");
        private static GeneralResponse Success() => new(true, "Process Completed");
        private async Task Commit() => await appDbContext.SaveChangesAsync();
    }
}
