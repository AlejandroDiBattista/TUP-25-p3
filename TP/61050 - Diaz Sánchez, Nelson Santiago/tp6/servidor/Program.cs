using Microsoft.EntityFrameworkCore;
using servidor.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => {
    options.AddPolicy("AllowClientApp", policy => {
       policy.WithOrigins("http://localhost:5177") 
      .AllowAnyHeader()
      .AllowAnyMethod();
    });
});

builder.Services.AddControllers(); // 
builder.Services.AddDbContext<AppDbContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowClientApp");

app.UseRouting(); 

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
