
using Microsoft.AspNetCore.Authentication;
using TestControlAPI.Services;

namespace TestControlAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Create a new instance of WebApplicationBuilder with the given arguments.
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the DI container.

            // Register controllers to the DI container.
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

            // Add support for API endpoint exploration and documentation generation using Swagger/OpenAPI.
            builder.Services.AddEndpointsApiExplorer();

            // Configure authentication with a custom API key scheme.
            builder.Services.AddAuthentication("ApiKeyScheme")
            .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("ApiKeyScheme", null);

            builder.Services.AddSwaggerGen(c =>
            {
                // Define the API title and version.
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "TestControlAPI", Version = "v1" });

                var securityScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = ApiKeyAuthenticationHandler.ApiKeyHeaderName,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Scheme = "ApiKeyScheme",
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    {
                        Id = "ApiKeyScheme",
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme
                    }
                };

                // Add the defined security scheme to Swagger.
                c.AddSecurityDefinition("ApiKeyScheme", securityScheme);

                // Define a security requirement using the API key scheme.
                var securityRequirement = new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            {
                { securityScheme, new string[] { } }
            };
                // Add the security requirement to Swagger.
                c.AddSecurityRequirement(securityRequirement);
            });

            // Register custom services for dependency injection.
            builder.Services.AddScoped<ExcelProcessor>();
            builder.Services.AddScoped<XmlProcessor>();
            builder.Services.AddScoped<FileValidator>();
            builder.Services.AddScoped<ReadExcelProcess>();
            builder.Services.AddScoped<ReadColumnValueProcess>();
            builder.Services.AddScoped<ReadRowCellValue>();

            // Build the WebApplication instance.
            var app = builder.Build();

            // Configure the HTTP request pipeline.

            // Enable Swagger in the development environment.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Configure Swagger UI with a specific endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                c.RoutePrefix = string.Empty;
            });

            // Redirect HTTP requests to HTTPS.
            app.UseHttpsRedirection();

            // Enable authorization middleware.
            app.UseAuthorization();

            // Serve static files, such as the uploaded files.
            app.UseStaticFiles(); // To serve the uploaded files

            // Map controller routes.
            app.MapControllers();

            // Run the application.
            app.Run();
        }
    }
}
