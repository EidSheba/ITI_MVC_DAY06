using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseManagement.Business.ViewModels;
using CourseManagement.Data.Models;
using CourseManagement.Data.Repositories;
using Microsoft.Extensions.Logging;

namespace CourseManagement.Business.Services
{
    /// <summary>
    /// Course service implementation
    /// تنفيذ خدمة الدورات
    /// </summary>
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IInstructorRepository _instructorRepository;
        private readonly ILogger<CourseService> _logger;

        public CourseService(
            ICourseRepository courseRepository,
            IInstructorRepository instructorRepository,
            ILogger<CourseService> logger)
        {
            _courseRepository = courseRepository;
            _instructorRepository = instructorRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<CourseVM>> GetAllCoursesAsync()
        {
            var courses = await _courseRepository.GetAllAsync();
            return courses.Select(MapToViewModel);
        }

        public async Task<IEnumerable<CourseVM>> GetCoursesByCategoryAsync(Category? category)
        {
            IEnumerable<Course> courses;

            if (category.HasValue)
                courses = await _courseRepository.GetByCategory(category.Value);
            else
                courses = await _courseRepository.GetAllAsync();

            return courses.Select(MapToViewModel);
        }

        public async Task<CourseVM> GetCourseByIdAsync(Guid id)
        {
            var course = await _courseRepository.GetCourseWithInstructor(id);
            return course != null ? MapToViewModel(course) : null;
        }

        public async Task<CourseVM> CreateCourseAsync(CourseVM courseVM)
        {
            try
            {
                _logger.LogInformation("Creating course: {CourseName}", courseVM.Name);

                var course = MapToEntity(courseVM);
                course.Id = Guid.NewGuid();
                course.IsActive = true;

                var created = await _courseRepository.AddAsync(course); // Repo يحفظ تلقائياً في تنفيذك الحالي
                _logger.LogInformation("Course created: {CourseId}", created.Id);

                return MapToViewModel(created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating course: {CourseName}", courseVM.Name);
                throw;
            }
        }

        public async Task UpdateCourseAsync(CourseVM courseVM)
        {
            var course = await _courseRepository.GetByIdAsync(courseVM.Id.Value);
            if (course == null)
                throw new KeyNotFoundException($"Course with ID {courseVM.Id} not found");

            course.Name = courseVM.Name;
            course.Description = courseVM.Description;
            course.Category = courseVM.Category;
            course.StartDate = courseVM.StartDate;
            course.EndDate = courseVM.EndDate;
            course.InstructorId = courseVM.InstructorId;
            course.IsActive = courseVM.IsActive;

            await _courseRepository.UpdateAsync(course); // Repo يحفظ تلقائياً في تنفيذك الحالي
        }

        public async Task DeleteCourseAsync(Guid id)
        {
            await _courseRepository.DeleteAsync(id); // Repo يحفظ تلقائياً في تنفيذك الحالي
        }

        public Task<bool> IsNameUniqueAsync(string name, Guid? excludeId = null)
        {
            return _courseRepository.IsNameUnique(name, excludeId);
        }

        public async Task<CourseListVM> SearchCoursesAsync(string search, Category? category, int page, int pageSize)
        {
            var (items, totalCount) = await _courseRepository.SearchAsync(search, category, page, pageSize);

            return new CourseListVM
            {
                Items = items.Select(MapToViewModel),
                Search = search ?? string.Empty,
                Category = category,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        // Mapping helpers
        private CourseVM MapToViewModel(Course course)
        {
            if (course == null)
                return null;

            return new CourseVM
            {
                Id = course.Id,
                Name = course.Name ?? string.Empty,
                Description = course.Description ?? string.Empty,
                Category = course.Category,
                StartDate = course.StartDate,
                EndDate = course.EndDate,
                InstructorId = course.InstructorId,
                InstructorName = course.Instructor != null
                    ? $"{course.Instructor.FirstName} {course.Instructor.LastName}"
                    : "N/A",
                IsActive = course.IsActive
            };
        }

        private Course MapToEntity(CourseVM vm)
        {
            if (vm == null)
                return null;

            return new Course
            {
                Id = vm.Id ?? Guid.NewGuid(),
                Name = vm.Name?.Trim() ?? string.Empty,
                Description = vm.Description?.Trim() ?? string.Empty,
                Category = vm.Category,
                StartDate = vm.StartDate,
                EndDate = vm.EndDate,
                InstructorId = vm.InstructorId,
                IsActive = vm.IsActive
            };
        }
    }
}
