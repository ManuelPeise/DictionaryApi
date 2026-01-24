using Web.Core.StartUp;

string corsPolicy = "DictionaryApiCorsPolicy";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.RegisterServices(builder.Configuration, corsPolicy);

var app = builder.Build();

app.ConfigureApplication(corsPolicy);

Database.Migrate(app);

app.Run();
