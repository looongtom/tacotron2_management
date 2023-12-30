using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Drawing;
using tacotron2_management.Models;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession();

builder.Services.AddDbContext<TacotronTtsContext>(options =>
    {
     options.UseSqlServer("Data Source = DESKTOP - 7BMV341" +
    "CSDLPTTEST; Initial Catalog = tacotron_tts; " +
    "User ID = sa; " +
    "Password = 88888888 " +
    "Connect Timeout = 30; " +
    "Encrypt = False; " +
    "Trust Server Certificate=False; " +
    "Application Intent = ReadWrite; " +
    "Multi Subnet Failover=False;");
      options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    });
     

    

var app = builder.Build();

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

app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
