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
    public class CountryRepository(AppDbContext appDbContext) : IGenericRepositoryInterface<Country>

    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var dep = await appDbContext.Countries.FindAsync(id);
            if (dep is null)
                return NotFound();
            appDbContext.Countries.Remove(dep);
            await Commit();
            return Success();
        }

        public async Task<List<Country>> GetAll() => await appDbContext.Countries.ToListAsync();

        public async Task<Country> GetById(int id) => await appDbContext.Countries.FindAsync(id);

        public async Task<GeneralResponse> Insert(Country item)
        {
            if (!await CheckName(item.name!)) return new GeneralResponse(false, "Country already exists");
            appDbContext.Countries.Add(item);
            await Commit();
            return Success();

        }

        private async Task<bool> CheckName(string v)
        {
            var item = await appDbContext.Countries.FirstOrDefaultAsync(x => x.name.ToLower().Equals(v.ToLower()));
            return item is null;

        }

        public async Task<GeneralResponse> Update(Country item)
        {
            var dep = await appDbContext.Countries.FindAsync(item.id);
            if (dep is null) return NotFound();
            dep.name = item.name;
            await Commit();
            return Success();
        }
        private static GeneralResponse NotFound() => new(false, "Sorry No Country of this name present");
        private static GeneralResponse Success() => new(true, "Process Completed");
        private async Task Commit() => await appDbContext.SaveChangesAsync();
    }
}
