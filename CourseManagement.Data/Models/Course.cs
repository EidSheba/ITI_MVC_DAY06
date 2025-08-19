using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Data.Models
{
    /// <summary>
    /// Course entity model
    /// نموذج كيان الدورة التدريبية
    /// </summary>
    public class Course
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Course name is required")]
        [MaxLength(100, ErrorMessage = "Course name cannot exceed 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        [Required]
        public Category Category { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }

        // Navigation property / خاصية التنقل
        public Guid? InstructorId { get; set; }
        public virtual Instructor Instructor { get; set; }
    }

    /// <summary>
    /// Course category enumeration
    /// تعداد فئات الدورات
    /// </summary>
    public enum Category
    {
        Programming,
        Design,
        Marketing,
        Business,
        DataScience,
        DevOps
    }
}