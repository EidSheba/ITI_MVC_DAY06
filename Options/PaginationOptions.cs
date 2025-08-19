namespace CourseManagement.Web
{
	public class PaginationOptions
	{
		public int DefaultPageSize { get; set; } = 10;
		public string PageSizeCookieName { get; set; } = "CoursePageSize";
		public int CookieDays { get; set; } = 30;
	}
}
