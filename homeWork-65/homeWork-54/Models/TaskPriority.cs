using System.ComponentModel.DataAnnotations;

namespace homeWork_54.Models;

public enum TaskPriority
{
    [Display(Name = nameof(High))]
    High = 1,
    
    [Display(Name = nameof(Medium))]
    Medium = 2,
    
    [Display(Name = nameof(Low))]
    Low = 3
}