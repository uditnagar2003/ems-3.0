using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using serverLibrary.Data;
using serverLibrary.Respositories.contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace serverLibrary.Respositories.Implementations
{
    public class DepartmentRepository(AppDbContext appDbContext) : IGenericRepositoryInterface<Department>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var dep = await appDbContext.Departments.FindAsync(id);
            if (dep is null)
                return NotFound();
            appDbContext.Departments.Remove(dep);
            await Commit();
            return Success();
        }

        public async Task<List<Department>> GetAll() => await appDbContext
            .Departments
            .AsNoTracking()
            .Include(gd=>gd.GeneralDepartment)
            .ToListAsync();

        public async Task<Department> GetById(int id) => await appDbContext.Departments.FindAsync(id);

        public async Task<GeneralResponse> Insert(Department item)
        {
            if (!await CheckName(item.name!)) return new GeneralResponse(false, "Department already exists");
            appDbContext.Departments.Add(item);
            await Commit();
            return Success();

        }

        private async Task<bool> CheckName(string v)
        {
            var item = await appDbContext.Departments.FirstOrDefaultAsync(x => x.name.ToLower().Equals(v.ToLower()));
            return item is null;

        }

        public async Task<GeneralResponse> Update(Department item)
        {
            var dep = await appDbContext.Departments.FindAsync(item.id);
            if (dep is null) return NotFound();
            dep.name = item.name;
            dep.GeneralDepartmentId = item.GeneralDepartmentId;
            await Commit();
            return Success();
        }
        private static GeneralResponse NotFound() => new(false, "Sorry No Department of this name present");
        private static GeneralResponse Success() => new(true, "Process Completed");
        private async Task Commit() => await appDbContext.SaveChangesAsync();
    }

}
