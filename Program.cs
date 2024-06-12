
using Microsoft.AspNetCore.Authentication;
using TestControlAPI.Services;

namespace TestControlAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddAuthentication("ApiKeyScheme")
            .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("ApiKeyScheme", null);

            builder.Services.AddSwaggerGen(c =>
            {
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

                c.AddSecurityDefinition("ApiKeyScheme", securityScheme);

                var securityRequirement = new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            {
                { securityScheme, new string[] { } }
            };

                c.AddSecurityRequirement(securityRequirement);
            });

            builder.Services.AddScoped<ExcelProcessor>();
            builder.Services.AddScoped<XmlProcessor>();
            builder.Services.AddScoped<FileValidator>();
            builder.Services.AddScoped<ReadExcelProcess>();
            builder.Services.AddScoped<ReadColumnValueProcess>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseStaticFiles(); // To serve the uploaded files

            app.MapControllers();

            app.Run();
        }
    }
}
