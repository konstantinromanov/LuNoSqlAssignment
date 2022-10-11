using LuNoSqlAssignment;
using Redis.OM;



var builder = WebApplication.CreateBuilder(args);
ConfigurationManager? configuration = builder.Configuration;

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "RedisDemo_";
});

//    .AddUserSecrets<Program>();
//builder.Services.AddStackExchangeRedisCache
//builder.Services.AddStackExchangeRedisCache(options => { options.Configuration = configuration["RedisCacheUrl"]; });

//builder.Services.AddStackExchangeRedisCache(async x => await RedisConnection.InitializeAsync(connectionString: configuration["CacheConnection"].ToString()));

//var configuration = configurationBuilder.Build();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton(async x => await RedisConnection.InitializeAsync(connectionString: builder.Configuration["CacheConnection"].ToString()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Register services here 
//builder.Services.AddSingleton(new RedisConnectionProvider(builder.Configuration["REDIS_CONNECTION_STRING"]));




app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
