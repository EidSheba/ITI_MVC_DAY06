using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseManagement.Data.Context;
using CourseManagement.Data.Models;
using Microsoft.Extensions.Logging;

namespace CourseManagement.Data.Repositories
{
    /// <summary>
    /// Course-specific repository interface
    /// واجهة مستودع الدورات
    /// </summary>
    public interface ICourseRepository : IRepository<Course>
    {
        Task<IEnumerable<Course>> GetByCategory(Category category);
        Task<Course> GetCourseWithInstructor(Guid id);
        Task<bool> IsNameUnique(string name, Guid? excludeId = null);
        Task<(IEnumerable<Course> Items, int TotalCount)> SearchAsync(string search, Category? category, int page, int pageSize);
    }

    /// <summary>
    /// Course repository implementation
    /// تنفيذ مستودع الدورات
    /// </summary>
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        public CourseRepository(ApplicationDbContext context, ILogger<CourseRepository> logger) : base(context, logger) { }

        public async Task<IEnumerable<Course>> GetByCategory(Category category)
        {
            return await _context.Courses
                .Include(c => c.Instructor)
                .Where(c => c.Category == category)
                .ToListAsync();
        }

        public async Task<Course> GetCourseWithInstructor(Guid id)
        {
            return await _context.Courses
                .Include(c => c.Instructor)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> IsNameUnique(string name, Guid? excludeId = null)
        {
            var query = _context.Courses.Where(c => c.Name == name);

            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
            }

            return !await query.AnyAsync();
        }

        public override async Task<IEnumerable<Course>> GetAllAsync()
        {
            return await _context.Courses
                .Include(c => c.Instructor)
                .ToListAsync();
        }

        public override async Task<Course> GetByIdAsync(Guid id)
        {
            return await _context.Courses
                .Include(c => c.Instructor)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<(IEnumerable<Course> Items, int TotalCount)> SearchAsync(string search, Category? category, int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.Courses
                .Include(c => c.Instructor)
                .AsQueryable();

            if (category.HasValue)
            {
                query = query.Where(c => c.Category == category.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.Trim().ToLower();
                query = query.Where(c => c.Name.ToLower().Contains(searchLower));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(c => c.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}