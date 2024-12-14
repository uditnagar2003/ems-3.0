using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using serverLibrary.Data;
using serverLibrary.Respositories.contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace serverLibrary.Respositories.Implementations
{
    public class GeneralDepartmentRepository(AppDbContext appDbContext) : IGenericRepositoryInterface<GeneralDepartment>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var dep = await appDbContext.GeneralDepartments.FindAsync(id);
            if (dep is null)
                return NotFound();
            appDbContext.GeneralDepartments.Remove(dep);
            await Commit();
            return Success();   
        }

        public async Task<List<GeneralDepartment>> GetAll() => await  appDbContext.GeneralDepartments.ToListAsync();

        public async Task<GeneralDepartment> GetById(int id) => await appDbContext.GeneralDepartments.FindAsync(id);

        public async Task<GeneralResponse> Insert(GeneralDepartment item)
        {
            var checkNamenull = await CheckName(item.name);
            if (!checkNamenull) 
                return new GeneralResponse(false, " General Department already exists");
            appDbContext.GeneralDepartments.Add(item);
            await Commit();
            return Success();
          
        }

        private async Task<bool> CheckName(string v)
        { 
            var item = await appDbContext.GeneralDepartments.FirstOrDefaultAsync(x => x.name.ToLower().Equals(v.ToLower()));
            return item is null; 
           
        }

        public async Task<GeneralResponse> Update(GeneralDepartment item)
        {
            var dep = await appDbContext.GeneralDepartments.FindAsync(item.id);
            if(dep is null) return NotFound();
            dep.name = item.name;
            await Commit();
            return Success();
        }
        private static GeneralResponse NotFound() => new(false, "Sorry No Department of this name present");
        private static GeneralResponse Success() => new(true, "Process Completed");
        private async Task Commit() =>await appDbContext.SaveChangesAsync();
    }
}
