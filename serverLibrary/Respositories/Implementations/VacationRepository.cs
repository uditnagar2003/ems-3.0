﻿using BaseLibrary.Entities;
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
    public class VacationRepository(AppDbContext appDbContext) : IGenericRepositoryInterface<Vacation>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var item = await appDbContext.Vacations.FirstOrDefaultAsync(eid => eid.EmployeeId == id);
            if (item is null) return NotFound();

            appDbContext.Vacations.Remove(item);
            await Commit();
            return Success();
        }
        private async Task Commit() => await appDbContext.SaveChangesAsync();
        private static GeneralResponse NotFound() => new(false, "Sorry Data not Found");
        private static GeneralResponse Success() => new(true, "Process COmpleted");

        public async Task<List<Vacation>> GetAll() => await appDbContext.Vacations.AsNoTracking().Include(t=>t.VacationType).ToListAsync();


        public async Task<Vacation> GetById(int id) =>
            await appDbContext.Vacations.FirstOrDefaultAsync(eid => eid.EmployeeId == id);

        public async Task<GeneralResponse> Insert(Vacation item)
        {
            appDbContext.Vacations.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(Vacation item)
        {
            var obj = await appDbContext.Vacations.FirstOrDefaultAsync(eid => eid.EmployeeId == item.Id);
            if (obj is null) return NotFound();
            obj.StartDate = item.StartDate;
            obj.NumberOfDays=item.NumberOfDays;
            obj.VacationTypeId=item.VacationTypeId;
            await Commit();
            return Success();
        }
    }
}
