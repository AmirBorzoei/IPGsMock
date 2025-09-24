using IPGsMock;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ObjectCacheStorage>();

builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();