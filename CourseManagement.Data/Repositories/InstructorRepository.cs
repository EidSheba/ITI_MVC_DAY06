using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CourseManagement.Data.Context;
using CourseManagement.Data.Models;
using Microsoft.Extensions.Logging;

namespace CourseManagement.Data.Repositories
{
    /// <summary>
    /// Instructor-specific repository interface
    /// واجهة مستودع المدربين
    /// </summary>
    public interface IInstructorRepository : IRepository<Instructor>
    {
        Task<IEnumerable<Instructor>> GetActiveInstructors();
        Task<Instructor> GetInstructorWithCourses(Guid id);
    }

    /// <summary>
    /// Instructor repository implementation
    /// تنفيذ مستودع المدربين
    /// </summary>
    public class InstructorRepository : Repository<Instructor>, IInstructorRepository
    {
        public InstructorRepository(ApplicationDbContext context, ILogger<InstructorRepository> logger) : base(context, logger) { }

        public async Task<IEnumerable<Instructor>> GetActiveInstructors()
        {
            return await _context.Instructors
                .Where(i => i.IsActive)
                .ToListAsync();
        }

        public async Task<Instructor> GetInstructorWithCourses(Guid id)
        {
            return await _context.Instructors
                .Include(i => i.Courses)
                .FirstOrDefaultAsync(i => i.Id == id);
        }
    }
}