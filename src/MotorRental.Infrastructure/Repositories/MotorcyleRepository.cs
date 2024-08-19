using Microsoft.EntityFrameworkCore;
using MotorRental.Domain.Dtos;
using MotorRental.Domain.Entities;
using MotorRental.Domain.Interfaces;
using MotorRental.Infrastructure.Data;

namespace MotorRental.Infrastructure.Repositories
{
    public class MotorcyleRepository : BaseRepository<Motorcycle>, IMotorcycleRepository
    {
        public MotorcyleRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<MotorcycleDto>> GetByLicensePlate(string licensePlate)
        {
            var query = _databaseContext.Motorcycles.AsNoTracking();

            if (!string.IsNullOrEmpty(licensePlate))
                query = query.Where(a => a.LicensePlate.Contains(licensePlate));

            return await query.Select(a => new MotorcycleDto
            {
                Id = a.Id,
                LicensePlate = a.LicensePlate,
                Model = a.Model,
                Year = a.Year
            }).ToListAsync();
        }
    }
}
