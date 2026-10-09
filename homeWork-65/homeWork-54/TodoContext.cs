using Microsoft.EntityFrameworkCore;
using homeWork_54.Models;

namespace homeWork_54;

public class TodoContext : DbContext
{
    public TodoContext(DbContextOptions<TodoContext> options) : base(options) { }

    public DbSet<TodoTask> Tasks { get; set; }
}