namespace homeWork_54.Models;

public class TaskListViewModel
{
    public List<TodoTask> Tasks { get; set; } = new();
    
    public string? SearchTitle { get; set; }
    public string? SearchDescription { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public TaskPriority? FilterPriority { get; set; }
    public TodoStatus? FilterStatus { get; set; }
    
    public string CurrentSort { get; set; } = "date_desc";
    public int PageNumber { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}