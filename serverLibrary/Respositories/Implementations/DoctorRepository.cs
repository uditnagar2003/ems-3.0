using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using serverLibrary.Data;
using serverLibrary.Respositories.contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace serverLibrary.Respositories.Implementations
{
    public class DoctorRepository(AppDbContext appDbContext) : IGenericRepositoryInterface<Doctor>
    {
        public async Task<GeneralResponse> DeleteById(int id)
        {
            var item = await appDbContext.Doctors.FirstOrDefaultAsync(eid => eid.EmployeeId == id);
            if (item is null) return NotFound();

            appDbContext.Doctors.Remove(item);
            await Commit();
            return Success();
        }
        private async Task Commit() => await appDbContext.SaveChangesAsync();
        private static GeneralResponse NotFound() => new(false, "Sorry Data not Found");
        private static GeneralResponse Success() => new(true, "Process COmpleted");

        public async Task<List<Doctor>> GetAll()=> await appDbContext.Doctors.AsNoTracking().ToListAsync();


        public async Task<Doctor> GetById(int id) =>
            await appDbContext.Doctors.FirstOrDefaultAsync(eid => eid.EmployeeId == id);
       
        public async Task<GeneralResponse> Insert(Doctor item)
        {
            appDbContext.Doctors.Add(item);
            await Commit();
            return Success();
        }

        public async Task<GeneralResponse> Update(Doctor item)
        {
           var obj = await appDbContext.Doctors.FirstOrDefaultAsync(eid=> eid.EmployeeId == item.Id);
            if (obj is null) return NotFound();
            obj.MedicalRecomendation
                = item.MedicalRecomendation;
            obj.MedicalDiagnose = item.MedicalDiagnose;
            obj.Date = item.Date;
            await Commit();
            return Success();
        }
    }
}
