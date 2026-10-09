using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using homeWork_54.Models;

namespace homeWork_54.Controllers;

public class TasksController : Controller
{
    private readonly TodoContext _context;
    private readonly IMemoryCache _memoryCache;
    private const string TasksCacheKeyPrefix = "tasks_list_";

    public TasksController(TodoContext context, IMemoryCache memoryCache)
    {
        _context = context;
        _memoryCache = memoryCache;
    }

    [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "searchTitle", "searchDescription", "dateFrom", "dateTo", "filterPriority", "filterStatus", "sortOrder", "page" })]
    public async Task<IActionResult> Index(
        string? searchTitle,
        string? searchDescription,
        DateTime? dateFrom,
        DateTime? dateTo,
        TaskPriority? filterPriority,
        TodoStatus? filterStatus,
        string sortOrder = "date_desc",
        int page = 1)
    {
        int pageSize = 10;
        
        string cacheKey = $"{TasksCacheKeyPrefix}_{searchTitle}_{searchDescription}_{dateFrom}_{dateTo}_{filterPriority}_{filterStatus}_{sortOrder}_{page}";
        
        if (!_memoryCache.TryGetValue(cacheKey, out TaskListViewModel? viewModel))
        {
            IQueryable<TodoTask> query = _context.Tasks;

            if (!string.IsNullOrWhiteSpace(searchTitle))
                query = query.Where(t => t.Title.ToLower().Contains(searchTitle.ToLower()));

            if (!string.IsNullOrWhiteSpace(searchDescription))
                query = query.Where(t => t.Description != null && t.Description.ToLower().Contains(searchDescription.ToLower()));

            if (dateFrom.HasValue)
                query = query.Where(t => t.CreatedDate >= dateFrom.Value.Date);

            if (dateTo.HasValue)
                query = query.Where(t => t.CreatedDate <= dateTo.Value.Date.AddDays(1).AddTicks(-1));

            if (filterPriority.HasValue)
                query = query.Where(t => t.Priority == filterPriority.Value);

            if (filterStatus.HasValue)
                query = query.Where(t => t.Status == filterStatus.Value);

            query = sortOrder switch
            {
                "title_asc" => query.OrderBy(t => t.Title),
                "title_desc" => query.OrderByDescending(t => t.Title),
                "priority_asc" => query.OrderBy(t => t.Priority),
                "priority_desc" => query.OrderByDescending(t => t.Priority),
                "status_asc" => query.OrderBy(t => t.Status),
                "status_desc" => query.OrderByDescending(t => t.Status),
                "date_asc" => query.OrderBy(t => t.CreatedDate),
                _ => query.OrderByDescending(t => t.CreatedDate)
            };

            int totalItems = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            viewModel = new TaskListViewModel
            {
                Tasks = items,
                SearchTitle = searchTitle,
                SearchDescription = searchDescription,
                DateFrom = dateFrom,
                DateTo = dateTo,
                FilterPriority = filterPriority,
                FilterStatus = filterStatus,
                CurrentSort = sortOrder,
                PageNumber = page,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            };
            
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(2));

            _memoryCache.Set(cacheKey, viewModel, cacheOptions);
        }

        return View(viewModel);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TodoTask task)
    {
        if (ModelState.IsValid)
        {
            task.Status = TodoStatus.New;
            task.CreatedDate = DateTime.UtcNow;
            _context.Add(task);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index));
        }
        return View(task);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Open(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task != null && task.Status == TodoStatus.New)
        {
            task.Status = TodoStatus.Opened;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Close(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task != null && task.Status == TodoStatus.Opened)
        {
            task.Status = TodoStatus.Closed;
            task.ClosedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task != null && task.Status != TodoStatus.Opened)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return NotFound();

        return View(task);
    }
}