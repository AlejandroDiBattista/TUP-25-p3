using Microsoft.EntityFrameworkCore;
using servidor.Data;
using servidor.Models;
using servidor.Cart;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// DB Context
builder.Services.AddDbContext<StoreDbContext>(options =>
    options.UseSqlite("Data Source=store.db"));

// Carts
builder.Services.AddSingleton<CartService>();

// ✅ CORS Policy (IMPORTANTE)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5177") // Blazor WASM client
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ✅ Activar CORS antes de cualquier endpoint
app.UseCors("AllowFrontend");

// DB Seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<StoreDbContext>();
    db.Database.EnsureCreated();

    if (!db.Products.Any())
    {
        db.Products.AddRange(new List<Product>
        {
            new() { Name = "Smartphone X", Description = "High-end phone", Price = 799, Stock = 10, ImageUrl = "images/smartphone.jpg" },
            new() { Name = "Wireless Earbuds", Description = "Noise cancelling", Price = 129, Stock = 25, ImageUrl = "images/earbuds.jpg" },
            new() { Name = "Gaming Mouse", Description = "RGB lighting", Price = 59, Stock = 15, ImageUrl = "images/mouse.jpg" },
            new() { Name = "Laptop Pro", Description = "Powerful and portable", Price = 1499, Stock = 8, ImageUrl = "images/laptop.jpg" },
            new() { Name = "Mechanical Keyboard", Description = "Cherry MX switches", Price = 99, Stock = 12, ImageUrl = "images/keyboard.jpg" },
            new() { Name = "Monitor 4K", Description = "Ultra HD display", Price = 329, Stock = 6, ImageUrl = "images/monitor.jpg" },
            new() { Name = "USB-C Hub", Description = "Multiple ports", Price = 39, Stock = 20, ImageUrl = "images/hub.jpg" },
            new() { Name = "External SSD", Description = "1TB storage", Price = 159, Stock = 10, ImageUrl = "images/ssd.jpg" },
            new() { Name = "Bluetooth Speaker", Description = "Portable audio", Price = 79, Stock = 18, ImageUrl = "images/speaker.jpg" },
            new() { Name = "Webcam HD", Description = "Clear video", Price = 49, Stock = 14, ImageUrl = "images/webcam.jpg" }
        });
        db.SaveChanges();
    }
}

app.UseRouting();

// ✅ API endpoints
app.MapGet("/products", async (HttpContext http, StoreDbContext db) =>
{
    var query = http.Request.Query["search"].ToString();

    var products = string.IsNullOrWhiteSpace(query)
        ? await db.Products.ToListAsync()
        : await db.Products
            .Where(p => p.Name.ToLower().Contains(query.ToLower()))
            .ToListAsync();

    return Results.Ok(products);
});

app.MapGet("/products/{id}", async (int id, StoreDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    return product is not null ? Results.Ok(product) : Results.NotFound();
});

app.MapPost("/carts", (CartService carts) =>
{
    var id = carts.CreateCart();
    return Results.Ok(id); // NO usar Results.Ok(new { cartId = id });
});

app.MapGet("/carts/{id}", (string id, CartService carts) =>
{
    return carts.GetCart(id);
});

app.MapDelete("/carts/{id}", (string id, CartService carts) =>
{
    carts.ClearCart(id);
    return Results.NoContent();
});

app.MapPut("/carts/{id}/{productId}", async (string id, int productId, int quantity, CartService carts, StoreDbContext db) =>
{
    var product = await db.Products.FindAsync(productId);
    if (product is null) return Results.NotFound("Product not found");

    if (quantity > product.Stock)
        return Results.BadRequest("Not enough stock");

    carts.SetItem(id, productId, quantity);
    return Results.Ok();
});

app.MapDelete("/carts/{id}/{productId}", (string id, int productId, CartService carts) =>
{
    carts.RemoveItem(id, productId);
    return Results.NoContent();
});

app.MapPut("/carts/{id}/confirm", async (string id, Customer customer, CartService carts, StoreDbContext db) =>
{
    var cart = carts.GetCart(id);
    if (cart.Count == 0) return Results.BadRequest("Cart is empty");

    var products = await db.Products.Where(p => cart.Keys.Contains(p.Id)).ToListAsync();

    foreach (var item in cart)
    {
        var product = products.FirstOrDefault(p => p.Id == item.Key);
        if (product is null || item.Value > product.Stock)
            return Results.BadRequest($"Insufficient stock for {product?.Name ?? "unknown"}.");
    }

    var purchase = new Purchase
    {
        Date = DateTime.UtcNow,
        FirstName = customer.FirstName,
        LastName = customer.LastName,
        Email = customer.Email,
        Items = cart.Select(kvp =>
        {
            var product = products.First(p => p.Id == kvp.Key);
            product.Stock -= kvp.Value;
            return new PurchaseItem
            {
                ProductId = product.Id,
                Quantity = kvp.Value,
                UnitPrice = product.Price
            };
        }).ToList(),
        Total = cart.Sum(kvp =>
        {
            var product = products.First(p => p.Id == kvp.Key);
            return kvp.Value * product.Price;
        })
    };

    db.Purchases.Add(purchase);
    await db.SaveChangesAsync();
    carts.ClearCart(id);

    return Results.Ok(purchase.Id);
});

app.Run();