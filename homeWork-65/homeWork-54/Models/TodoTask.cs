using System.ComponentModel.DataAnnotations;

namespace homeWork_54.Models;

public class TodoTask
{
    public int Id { get; set; }

    [Required(ErrorMessage = "TitleRequired")]
    [Display(Name = nameof(Title))]
    public string Title { get; set; } = string.Empty;

    [Display(Name = nameof(Description))]
    public string? Description { get; set; }

    [Required(ErrorMessage = "AssigneeRequired")]
    [Display(Name = nameof(Assignee))]
    public string Assignee { get; set; } = string.Empty;

    [Required(ErrorMessage = "PriorityRequired")]
    [Display(Name = nameof(Priority))]
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    [Display(Name = nameof(Status))]
    public TodoStatus Status { get; set; } = TodoStatus.New;

    [Display(Name = nameof(CreatedDate))]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [Display(Name = nameof(ClosedDate))]
    public DateTime? ClosedDate { get; set; }
}