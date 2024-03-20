using DockerizedMvcNbg.Extensions;
using DockerizedMvcNbg.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("Database")));

builder.Services.AddStackExchangeRedisCache(
    options => options.Configuration = builder.Configuration.GetConnectionString("Cache"));


var app = builder.Build();

 app.ApplyMigrations();

// ###equivalent
//var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
//optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("Database"));
//var options = optionsBuilder.Options;
//using (var dbContext = new ApplicationDbContext(options))
//{
//    dbContext.Database.Migrate();
//}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
