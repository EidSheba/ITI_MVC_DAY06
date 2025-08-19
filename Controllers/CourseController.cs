using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;
using CourseManagement.Business.Services;
using CourseManagement.Business.ViewModels;
using CourseManagement.Data.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using CourseManagement.Web;

namespace CourseManagement.Web.Controllers
{
    /// <summary>
    /// Course management controller
   
    /// </summary>
    public class CourseController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly IInstructorService _instructorService;
        private readonly PaginationOptions _paginationOptions;

        public CourseController(ICourseService courseService, IInstructorService instructorService, IOptions<PaginationOptions> paginationOptions)
        {
            _courseService = courseService;
            _instructorService = instructorService;
            _paginationOptions = paginationOptions.Value;
        }

        // GET: Course
        public async Task<IActionResult> Index(Category? category, string search, int page = 1, int? pageSize = null)
        {
            // Determine pageSize from query, cookie, or default config
            int resolvedPageSize = pageSize ?? ReadPageSizeFromCookie() ?? _paginationOptions.DefaultPageSize;
            resolvedPageSize = resolvedPageSize <= 0 ? _paginationOptions.DefaultPageSize : resolvedPageSize;

            // Persist pageSize if provided by user
            if (pageSize.HasValue)
            {
                WritePageSizeCookie(resolvedPageSize);
            }

            var listVm = await _courseService.SearchCoursesAsync(search, category, page, resolvedPageSize);

            
            ViewBag.Categories = new SelectList(Enum.GetValues(typeof(Category)));
            ViewBag.SelectedCategory = category;
            ViewBag.Search = search;

            return View(listVm);
        }

        // GET: Course/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);

            if (course == null)
            {
                TempData["Error"] = "Course not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(course);
        }

        // GET: Course/Create
        public async Task<IActionResult> Create()
        {
            await PrepareViewBagData();
            return View(new CourseVM { StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(30) });
        }

        // POST: Course/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseVM courseVM)
        {
            try
            {
                // Log the incoming data
                Console.WriteLine($"Creating course: {courseVM.Name}");
                Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");
                Console.WriteLine($"ModelState.Count: {ModelState.Count}");
                
                // Log all ModelState errors in detail
                if (!ModelState.IsValid)
                {
                    Console.WriteLine("=== VALIDATION ERRORS ===");
                    foreach (var key in ModelState.Keys)
                    {
                        var entry = ModelState[key];
                        if (entry.Errors.Count > 0)
                        {
                            Console.WriteLine($"Field: {key}");
                            foreach (var error in entry.Errors)
                            {
                                Console.WriteLine($"  Error: {error.ErrorMessage}");
                            }
                        }
                    }
                    Console.WriteLine("=== END VALIDATION ERRORS ===");
                    
                    await PrepareViewBagData();
                    return View(courseVM);
                }

                await _courseService.CreateCourseAsync(courseVM);
                TempData["Success"] = "Course created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in Create: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                
                TempData["Error"] = $"Error creating course: {ex.Message}";
                await PrepareViewBagData();
                return View(courseVM);
            }
        }

        // GET: Course/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);

            if (course == null)
            {
                TempData["Error"] = "Course not found.";
                return RedirectToAction(nameof(Index));
            }

            await PrepareViewBagData();
            return View(course);
        }

        // POST: Course/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CourseVM courseVM)
        {
            if (id != courseVM.Id)
            {
                TempData["Error"] = "Invalid course ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                if (ModelState.IsValid)
                {
                    await _courseService.UpdateCourseAsync(courseVM);
                    TempData["Success"] = "Course updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Log validation errors
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine($"Validation error: {error.ErrorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in Edit: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                TempData["Error"] = $"Error updating course: {ex.Message}";
            }

            await PrepareViewBagData();
            return View(courseVM);
        }

        // GET: Course/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);

            if (course == null)
            {
                TempData["Error"] = "Course not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(course);
        }

        // POST: Course/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                await _courseService.DeleteCourseAsync(id);
                TempData["Success"] = "Course deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting course: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // Remote validation for unique name
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> IsNameUnique(string name, Guid? id)
        {
            var isUnique = await _courseService.IsNameUniqueAsync(name, id);

            if (!isUnique)
            {
                return Json($"Course name '{name}' is already taken.");
            }

            return Json(true);
        }

        // Helper method to prepare ViewBag data
        private async Task PrepareViewBagData()
        {
            var instructors = await _instructorService.GetActiveInstructorsAsync();
            ViewBag.Instructors = new SelectList(
                instructors.Select(i => new { i.Id, Name = $"{i.FirstName} {i.LastName}" }),
                "Id",
                "Name"
            );
            ViewBag.Categories = new SelectList(Enum.GetValues(typeof(Category)));
        }

        private int? ReadPageSizeFromCookie()
        {
            var cookieName = _paginationOptions.PageSizeCookieName ?? "CoursePageSize";
            if (Request.Cookies.TryGetValue(cookieName, out var value) && int.TryParse(value, out var parsed))
            {
                return parsed;
            }
            return null;
        }

        private void WritePageSizeCookie(int pageSize)
        {
            var cookieName = _paginationOptions.PageSizeCookieName ?? "CoursePageSize";
            var days = _paginationOptions.CookieDays <= 0 ? 30 : _paginationOptions.CookieDays;
            Response.Cookies.Append(cookieName, pageSize.ToString(), new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(days),
                IsEssential = true,
                HttpOnly = false,
                Secure = false
            });
        }
    }
}