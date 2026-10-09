using System.ComponentModel.DataAnnotations;

namespace homeWork_54.Models;

public enum TodoStatus
{
    [Display(Name = nameof(New))]
    New = 1,
    
    [Display(Name = nameof(Opened))]
    Opened = 2,
    
    [Display(Name = nameof(Closed))]
    Closed = 3
}