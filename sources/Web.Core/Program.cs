using Web.Core.StartUp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.RegisterServices();

var app = builder.Build();

app.ConfigureApplication();

Database.Migrate(app);

app.Run();
