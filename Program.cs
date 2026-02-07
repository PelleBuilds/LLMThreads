using HuggingFaceApiClient;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();



try
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
}
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
