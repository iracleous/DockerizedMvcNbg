using Microsoft.EntityFrameworkCore;

namespace DockerizedMvcNbg.Models;

public class ApplicationDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; }

    public ApplicationDbContext()
    {

    }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {

    }
}
