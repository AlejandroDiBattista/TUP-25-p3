    using Microsoft.EntityFrameworkCore;
    using servidor.Data;
    using modelos_compartidos;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

    var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: MyAllowSpecificOrigins,
                            policy =>
                            {
                                policy.AllowAnyOrigin()
                                    .AllowAnyHeader()
                                    .AllowAnyMethod();
                            });
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddControllers();
    builder.Services.AddRazorPages();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseWebAssemblyDebugging();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MiTienda API V1"));
    }
    else
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    
    app.UseBlazorFrameworkFiles();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseCors(MyAllowSpecificOrigins);

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating or seeding the database.");
        }
    }

    // Endpoint para obtener productos (ya existente)
    app.MapGet("/productos", async (string? q, ApplicationDbContext db) =>
    {
        var query = db.Productos.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(p => p.Nombre.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                                     p.Descripcion.Contains(q, StringComparison.OrdinalIgnoreCase));
        }
        return Results.Ok(await query.ToListAsync());
    });

    // NUEVO ENDPOINT para actualizar el stock después de una compra
    app.MapPost("/stock/update", async (List<StockUpdateDto> updates, ApplicationDbContext db) =>
    {
        foreach (var update in updates)
        {
            var producto = await db.Productos.FindAsync(update.ProductoId);
            if (producto != null)
            {
                producto.Stock -= update.CantidadVendida;
                // Asegurarse de que el stock no baje de cero (aunque se debería validar antes en el cliente)
                if (producto.Stock < 0)
                {
                    producto.Stock = 0;
                }
            }
        }
        await db.SaveChangesAsync(); // Guardar los cambios en la base de datos
        return Results.Ok(); // Retornar éxito
    });

    app.MapRazorPages();
    app.MapControllers();

    app.MapFallbackToFile("index.html");

    app.Run();
    