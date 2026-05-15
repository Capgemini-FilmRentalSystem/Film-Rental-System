using FilmRentalStore.MVC.Services.Interfaces;
using FilmRentalStore.MVC.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
{
    var baseUrl = builder.Configuration["ApiSettings:BaseUrl"];
    client.BaseAddress = new Uri(baseUrl!);
});

builder.Services.AddScoped<IAuthApiService, AuthApiService>();
builder.Services.AddScoped<IActorApiService, ActorApiService>();
builder.Services.AddHttpClient<ICategoryApiService, CategoryApiService>(client =>
{
    var baseUrl = builder.Configuration["ApiSettings:BaseUrl"];
    client.BaseAddress = new Uri(baseUrl!);
});

builder.Services.AddScoped<ICustomerApiService, CustomerApiService>();
builder.Services.AddHttpClient<IFilmApiService, FilmApiService>(client =>
{
    var baseUrl = builder.Configuration["ApiSettings:BaseUrl"];
    client.BaseAddress = new Uri(baseUrl!);
});

builder.Services.AddScoped<IInventoryApiService, InventoryApiService>();
builder.Services.AddScoped<ILanguageApiService, LanguageApiService>();
builder.Services.AddScoped<IAddressApiService, AddressApiService>();
builder.Services.AddScoped<IPaymentApiService, PaymentApiService>();
builder.Services.AddScoped<IRentalApiService, RentalApiService>();
builder.Services.AddScoped<IStaffApiService, StaffApiService>();
builder.Services.AddScoped<IStoreApiService, StoreApiService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/Index");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "root",
    pattern: "",
    defaults: new { controller = "Auth", action = "StaffLogin" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.Run();
