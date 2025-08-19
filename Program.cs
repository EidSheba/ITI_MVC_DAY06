using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CourseManagement.Business.Services;
using CourseManagement.Data.Context;
using CourseManagement.Data.Repositories;
using CourseManagement.Web.Middleware;
using CourseManagement.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container / إضافة الخدمات إلى الحاوية
builder.Services.AddControllersWithViews();

// Configure Entity Framework / تكوين Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories / تسجيل المستودعات
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IInstructorRepository, InstructorRepository>();

// Register services / تسجيل الخدمات
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IInstructorService, InstructorService>();

// Pagination options
builder.Services.Configure<PaginationOptions>(builder.Configuration.GetSection("Pagination"));

var app = builder.Build();

// Configure the HTTP request pipeline / تكوين خط أنابيب طلبات HTTP
if (app.Environment.IsDevelopment())
{
    // صفحة الأخطاء التفصيلية في وضع التطوير
    app.UseDeveloperExceptionPage();
}
else
{
    // صفحة أخطاء عامة في وضع الإنتاج
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Use custom middleware / استخدام الوسيط المخصص
app.UseRequestLogging();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Course}/{action=Index}/{id?}");

// Ensure database is up-to-date with migrations / التأكد من تحديث قاعدة البيانات بالمخططات
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
}

app.Run();
