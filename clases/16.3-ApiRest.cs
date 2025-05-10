#!/usr/bin/env dotnet-script
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class Post {
    public int UserId { get; set; }
    public int Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
}

HttpClient client = new HttpClient();

try {
    // Obtener todos los posts
    await GetPosts();
    
    // Agregar nuevo post
    var nuevoPost = new Post {
        UserId = 1,
        Title = "Nuevo post",
        Body = "Contenido del nuevo post"
    };
    var postCreado = await AddPost(nuevoPost);
    Console.WriteLine($"\nPost creado con ID: {postCreado?.Id}");
    
    // Eliminar un post
    await DeletePost(1);
    Console.WriteLine("\nPost eliminado exitosamente");
}
catch(HttpRequestException e) {
    Console.WriteLine($"Error: {e.Message}");
}

async Task GetPosts() {
    var response = await client.GetStringAsync("https://jsonplaceholder.typicode.com/posts");
    var posts = JsonSerializer.Deserialize<Post[]>(response);
    
    foreach(var post in posts) {
        Console.WriteLine($"Post ID: {post.Id}");
        Console.WriteLine($"Título: {post.Title}\n");
    }
}

async Task<Post> AddPost(Post nuevoPost) {
    var json = JsonSerializer.Serialize(nuevoPost);
    var content = new StringContent(json, Encoding.UTF8, "application/json");
    
    var response = await client.PostAsync("https://jsonplaceholder.typicode.com/posts", content);
    response.EnsureSuccessStatusCode();
    
    var responseBody = await response.Content.ReadAsStringAsync();
    return JsonSerializer.Deserialize<Post>(responseBody);
}

async Task DeletePost(int postId) {
    var response = await client.DeleteAsync($"https://jsonplaceholder.typicode.com/posts/{postId}");
    response.EnsureSuccessStatusCode();
}