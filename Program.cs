using FerramentariaTest.DAL;
using FerramentariaTest.ModifiedServices;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add services to the container.
builder.Services.AddControllersWithViews();

var ItemsString = builder.Configuration.GetConnectionString("Items") ?? throw new InvalidOperationException("Connection string 'Items' not found.");
var connectionStringAccount = builder.Configuration.GetConnectionString("Account") ?? throw new InvalidOperationException("Connection string 'Account' not found.");
var connectionStringThirdParty = builder.Configuration.GetConnectionString("ThirdParty") ?? throw new InvalidOperationException("Connection string 'ThirdParty' not found.");
var connectionStringEmployees = builder.Configuration.GetConnectionString("Employees") ?? throw new InvalidOperationException("Connection string 'Employees' not found.");

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddMvc();
builder.Services.AddDbContext<ContextoBanco>(options =>
    options.UseSqlServer(ItemsString));

builder.Services.AddDbContext<ContextoBancoBS>(options =>
    options.UseSqlServer(connectionStringAccount));

builder.Services.AddDbContext<ContextoBancoRM>(options =>
    options.UseSqlServer(connectionStringThirdParty));


builder.Services.AddDbContext<ContextoBancoSeek>(options =>
    options.UseSqlServer(connectionStringEmployees));

//builder.Services.AddSession();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10); 
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCookiePolicy();
app.UseSession();

app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
	pattern: "{controller=Home}/{action=Login}");



app.Run();
