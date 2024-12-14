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
    public class OverTimeTypeRepository(AppDbContext appDbContext) : IGenericRepositoryInterface<OverTimeType>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var item = await appDbContext.OverTimeTypes.FindAsync(id);
            if (item is null) return NotFound();

            appDbContext.OverTimeTypes.Remove(item);
            await Commit();
            return Success();
        }
        private async Task Commit() => await appDbContext.SaveChangesAsync();
        private static GeneralResponse NotFound() => new(false, "Sorry Data not Found");
        private static GeneralResponse Success() => new(true, "Process COmpleted");

        public async Task<List<OverTimeType>> GetAll() => await appDbContext.OverTimeTypes.AsNoTracking().ToListAsync();


        public async Task<OverTimeType> GetById(int id) =>
            await appDbContext.OverTimeTypes.FindAsync(id);

        public async Task<GeneralResponse> Insert(OverTimeType item)
        {
            if (!await CheckName(item.name!)) return new GeneralResponse(false, "Over Time Type already exist");
            appDbContext.OverTimeTypes.Add(item);
            await Commit();
            return Success();
        }

        private async Task<bool> CheckName(string v)
        {
            var item = await appDbContext.OverTimeTypes.FirstOrDefaultAsync(x=>x.name!.ToLower().Equals(v.ToLower()));
            return item is null;
        }

        public async Task<GeneralResponse> Update(OverTimeType item)
        {
            var obj = await appDbContext.OverTimeTypes.FindAsync(item.id);
            if (obj is null) return NotFound();
            obj.name=item.name;
            await Commit();
            return Success();
        }
    }
}
