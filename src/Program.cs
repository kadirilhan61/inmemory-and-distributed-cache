using CacheManagerApi.CacheManager;

var builder = WebApplication.CreateBuilder(args);


IConfiguration configuration = builder.Configuration;
 
builder.Services.Configure<CacheSettings>(configuration.GetSection("CacheSettings"));



// Add services to the container.
builder.Services
    .AddControllersWithViews()
    .AddRazorRuntimeCompilation();



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Cache aktif mi?
if (configuration.GetValue<bool>("CacheSettings:CacheEnabled"))
{
    // InMemory Cache
    if (configuration.GetValue<CacheType>("CacheSettings:CacheType") == CacheType.InMemoryCache)
    {
        // InMemory Cachhe'i uygulama seviyesinde Configure etme ve DI seviyesinde ayağa kaldırma.
        builder.Services.AddMemoryCache();
        builder.Services.AddSingleton<ICacheManager, InMemoryCacheManager>();
    }
    // Distributed Cache - Redis
    else
    {
        // Distributed Cache'i uygulama seviyesinde Configure etme ve DI seviyesinde ayağa kaldırma.
        builder.Services.AddDistributedRedisCache(option =>
        {
            option.Configuration = configuration.GetValue<string>("CacheSettings:RedisConfiguration");
            option.InstanceName = configuration.GetValue<string>("CacheSettings:RedisInstanceName");
        });
        builder.Services.AddSingleton<ICacheManager, RedisCacheManager>();
    }
}
 
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
