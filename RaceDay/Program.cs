using RaceDay.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Register MVC services
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
// Register API communication
builder.Services.AddHttpClient("RaceDayApi", client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["ApiSettings:BaseUrl"]!);
});
builder.Services.AddScoped<EventApiService>();
builder.Services.AddScoped<CategoryApiService>();
builder.Services.AddScoped<AccountApiService>();
// Add services to the container.
builder.Services.AddControllersWithViews();

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
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

//Password123!

/* {
"email": "sarah@test.com",
"password": "Password123!"
} 

{
"email": "john@test.com",
"password": "Password123!"
} 
*/
//  / Organiser / Dashboard 

// https://localhost:7231/Organiser/CreateEvent
// cd "C:\Users\Student\source\repos\RaceDay.API"