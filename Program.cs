
using Microsoft.AspNetCore.DataProtection;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using ThreadMapLLM.Interfaces;
using ThreadMapLLM.Models;
using ThreadMapLLM.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


//var connectionString = Environment.GetEnvironmentVariable("MONGODB_URI");
var connectionString = builder.Configuration["MONGODB_URI"];

if (!string.IsNullOrWhiteSpace(connectionString))
{
    builder.Services.AddSingleton<IMongoClient>(sp_ =>
        new MongoClient(connectionString));

    builder.Services.AddScoped<IChatRepository, MongoDBRepository>();
}
else
{
    builder.Services.AddScoped<IChatRepository>(_ =>
        new NullChatRepository());
}

/*try
{
    var secret = builder.Configuration["apikey"];
    builder.Services.AddHttpClient<IHuggingFaceClient, HuggingFaceClient>(client =>
    {

    }).AddTypedClient<IHuggingFaceClient>((httpClient, serviceProvider) =>
        new HuggingFaceClient(secret!, httpClient));
}
catch (Exception ex)
{
    Console.WriteLine($"Error configuring HuggingFaceClient: {ex.Message}");
}*/
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
