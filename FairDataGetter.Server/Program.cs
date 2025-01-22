using FairDataGetter.Server.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Kestrel to use HTTP only
builder.WebHost.ConfigureKestrel(options => {
    options.ListenAnyIP(5019); // HTTP port
    // options.ListenAnyIP(5001, listenOptions => listenOptions.UseHttps()); // Comment out or remove if HTTPS is not required
});

var app = builder.Build();

// Apply pending migrations in development
if (app.Environment.IsDevelopment()) {
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

// Middleware pipeline
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();