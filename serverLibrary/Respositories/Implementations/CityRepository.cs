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
    public class CityRepository(AppDbContext appDbContext):IGenericRepositoryInterface<City>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var dep = await appDbContext.Cities.FindAsync(id);
            if (dep is null)
                return NotFound();
            appDbContext.Cities.Remove(dep);
            await Commit();
            return Success();
        }

        public async Task<List<City>> GetAll() => await appDbContext.Cities
            .AsNoTracking()
            .Include(C=>C.Country)
            .ToListAsync();

        public async Task<City> GetById(int id) => await appDbContext.Cities.FindAsync(id);

        public async Task<GeneralResponse> Insert(City item)
        {
            if (!await CheckName(item.name!)) return new GeneralResponse(false, "City already exists");
            appDbContext.Cities.Add(item);
            await Commit();
            return Success();

        }

        private async Task<bool> CheckName(string v)
        {
            var item = await appDbContext.Cities.FirstOrDefaultAsync(x => x.name.ToLower().Equals(v.ToLower()));
            return item is null;

        }

        public async Task<GeneralResponse> Update(City item)
        {
            var dep = await appDbContext.Cities.FindAsync(item.id);
            if (dep is null) return NotFound();
            dep.name = item.name;
            dep.CountryId = item.CountryId;
            await Commit();
            return Success();
        }
        private static GeneralResponse NotFound() => new(false, "Sorry No City of this name present");
        private static GeneralResponse Success() => new(true, "Process Completed");
        private async Task Commit() => await appDbContext.SaveChangesAsync();
    }
}
