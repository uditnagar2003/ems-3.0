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
    public class BranchRepository(AppDbContext appDbContext) : IGenericRepositoryInterface<Branch>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var dep = await appDbContext.Branches.FindAsync(id);
            if (dep is null)
                return NotFound();
            appDbContext.Branches.Remove(dep);
            await Commit();
            return Success();
        }

        public async Task<List<Branch>> GetAll() => await appDbContext
            .Branches
            .AsNoTracking()
            .Include(gd => gd.Department)
            .ToListAsync();

        public async Task<Branch> GetById(int id) => await appDbContext.Branches.FindAsync(id);

        public async Task<GeneralResponse> Insert(Branch item)
        {
            if (!await CheckName(item.name!)) return new GeneralResponse(false, "Branch already exists");
            appDbContext.Branches.Add(item);
            await Commit();
            return Success();

        }

        private async Task<bool> CheckName(string v)
        {
            var item = await appDbContext.Branches.FirstOrDefaultAsync(x => x.name.ToLower().Equals(v.ToLower()));
            return item is null;

        }

        public async Task<GeneralResponse> Update(Branch item)
        {
            var branch = await appDbContext.Branches.FindAsync(item.id);
            if (branch is null) return NotFound();
            branch.name = item.name;
            branch.DepartmentId = item.DepartmentId;
            await Commit();
            return Success();
        }
        private static GeneralResponse NotFound() => new(false, "Sorry No Branch of this name present");
        private static GeneralResponse Success() => new(true, "Process Completed");
        private async Task Commit() => await appDbContext.SaveChangesAsync();

    }


}
