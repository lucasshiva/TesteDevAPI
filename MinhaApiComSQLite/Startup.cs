#region assembly Microsoft.AspNetCore.Mvc.NewtonsoftJson, Version=5.0.14.0, Culture=neutral, PublicKeyToken=adb9793829ddae60

// C:\Users\isaias.gomes\.nuget\packages\microsoft.aspnetcore.mvc.newtonsoftjson\5.0.14\lib\net5.0\Microsoft.AspNetCore.Mvc.NewtonsoftJson.dll
// Decompiled with ICSharpCode.Decompiler 8.1.1.7464

#endregion

using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MinhaApiComSQLite.Data;
using MinhaApiComSQLite.DTOs;
using MinhaApiComSQLite.Repositories;
using MinhaApiComSQLite.Services;
using MinhaApiComSQLite.Validators;

namespace MinhaApiComSQLite;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // Método chamado pela ASP.NET Core para adicionar serviços ao container.
    public void ConfigureServices(IServiceCollection services)
    {
        // Configurar banco de dados SQLite
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(Configuration.GetConnectionString("DefaultConnection"))
        );

        // Configurar o Swagger
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Title = "Minha API com SQLite",
                    Version = "v1",
                    Description = "API para gerenciar produtos com SQLite",
                    Contact = new OpenApiContact
                    {
                        Name = "Seu Nome",
                        Email = "seu.email@exemplo.com",
                        Url = new Uri("https://seuwebsite.com"),
                    },
                }
            );
        });

        // Configurar controladores e endpoints
        services.AddControllers();

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IValidator<CreateProductDto>, CreateProductDtoValidator>();
        services.AddScoped<IValidator<CreateCategoryDto>, CreateCategoryValidator>();
        services.AddScoped<IValidator<UpdateCategoryDto>, UpdateCategoryValidator>();
    }

    // Método chamado pela ASP.NET Core para configurar o pipeline HTTP.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            // Configurar Swagger apenas em desenvolvimento
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Minha API com SQLite v1");
                options.RoutePrefix = ""; // Swagger abre na URL raiz
            });
        }

        app.UseHttpsRedirection(); // Força HTTPS
        app.UseRouting(); // Habilita o roteamento
        app.UseAuthorization(); // Habilita a autorização

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers(); // Mapeia controladores
        });
    }
}
