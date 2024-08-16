using Microsoft.EntityFrameworkCore;
using MotorRental.Domain.Dtos;
using MotorRental.Domain.Entities;
using MotorRental.Domain.Interfaces;
using MotorRental.Infrastructure.Data;

namespace MotorRental.Infrastructure.Repositories
{
    public class RentalRepository : BaseRepository<Rental>, IRentalRepository
    {
        private readonly ApplicationDbContext _context;
        public RentalRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<RentalDto>> Get()
        {
            var query = _context.Rentals.AsNoTracking().Select(a => new RentalDto
            {
                Id = a.Id,
                EndDate = a.EndDate,
                ExpectedEndDate = a.ExpectedEndDate,
                StartDate = a.StartDate,
                Price = a.Price,
                ExpectedPrice = a.ExpectedPrice,
                DeliverDriverId = a.DeliverDriverId,
                MotorcycleId = a.MotorcycleId,
                PlanId = a.PlanId
            }).ToListAsync();

            return await query;
        }
    }
}
