using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Readora.API.Extentions;
using Readora.DataBase;
using Readora.DataBase.Abstractions;
using Readora.Models.Authentification;
using Readora.Services;
using Readora.Services.Abstractions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options => 
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options => {
    options.AddPolicy("AllowClient", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "https://localhost:3000",
                "https://mai-kun.github.io"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddDbContext<ReadoraDbContext>(optionsBuilder =>
{
    optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("ReadoraDbContext"));
});

builder.Services.AddScoped<IDbWriter>(c => c.GetRequiredService<ReadoraDbContext>());
builder.Services.AddScoped<IDbReader>(c => c.GetRequiredService<ReadoraDbContext>());
builder.Services.AddScoped<IUnitOfWork>(c => c.GetRequiredService<ReadoraDbContext>());

var jwtSettings = builder.Configuration.GetSection(nameof(JwtOptions));
builder.Services.AddApiAuthentication(jwtSettings);

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFileSaver, FileSaver>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddSingleton<BlockchainService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
        options
            .WithTitle("Readora.API")
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
    );
}

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
if (app.Environment.IsProduction())
{
    app.UseStaticFiles(new StaticFileOptions {
        FileProvider = new PhysicalFileProvider(
            Path.Combine(builder.Environment.ContentRootPath, "files")),
        RequestPath = "/files",
        ContentTypeProvider = CreateContentTypeProvider(),
    });
}

app.UseCors("AllowClient");
//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
return;

static FileExtensionContentTypeProvider CreateContentTypeProvider()
{
    var provider = new FileExtensionContentTypeProvider
    {
        Mappings =
        {
            [".webp"] = "image/webp",
            [".jpg"] = "image/webp",
            [".png"] = "image/webp",
        },
    };

    return provider;
}