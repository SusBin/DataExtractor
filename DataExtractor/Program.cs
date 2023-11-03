using Microsoft.AspNetCore.Http.Features;
using System.Reflection.PortableExecutable;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Configure the FormOptions to increase the multipart body length limit
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 150000000;  // In bytes, 150 MB
});

// Configure Kestrel server options
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 150000000; // In bytes, 150 MB
});

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();  // Enables attribute routing
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();

