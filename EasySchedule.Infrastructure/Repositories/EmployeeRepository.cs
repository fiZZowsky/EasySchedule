using EasySchedule.Application.Interfaces.Repositories;
using EasySchedule.Domain.Entities;
using EasySchedule.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EasySchedule.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext dbContext;

        public EmployeeRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await dbContext.Employees.Include(e => e.Profession).OrderBy(x => x.Surname).ThenBy(x => x.Name).AsNoTracking().ToListAsync();
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await dbContext.Employees.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task AddAsync(Employee employee)
        {
            await dbContext.Employees.AddAsync(employee);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Employee employee)
        {
            dbContext.ChangeTracker.Clear();

            dbContext.Employees.Update(employee);
            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Employee employee)
        {
            dbContext.ChangeTracker.Clear();

            dbContext.Remove(employee);
            await dbContext.SaveChangesAsync();
        }
    }
}
