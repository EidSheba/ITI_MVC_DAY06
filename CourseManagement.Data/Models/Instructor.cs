using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Data.Models
{
    /// <summary>
    /// Instructor entity model
    /// نموذج كيان المدرب
    /// </summary>
    public class Instructor
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [MaxLength(1000)]
        public string Bio { get; set; }

        public Specialization Specialization { get; set; }

        public bool IsActive { get; set; }

        // Navigation property / خاصية التنقل
        public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
    }

    /// <summary>
    /// Instructor specialization enumeration
    /// تعداد تخصصات المدرب
    /// </summary>
    public enum Specialization
    {
        SoftwareDevelopment,
        Marketing,
        Business
    }
}