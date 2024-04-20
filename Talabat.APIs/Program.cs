using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.APIs.Middleware;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Repository;
using Talabat.Repository.Data;

namespace Talabat.APIs
{
    public class Program
    {
        //Entry point
        public static async Task Main(string[] args)
        {


            var webApplicationBuilder = WebApplication.CreateBuilder(args);

            #region Configure Services
            // Add services to the container.

            webApplicationBuilder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

            webApplicationBuilder.Services.AddEndpointsApiExplorer();
            webApplicationBuilder.Services.AddSwaggerGen();

            webApplicationBuilder.Services.AddDbContext<StoreContext>(options =>
            {
                options.UseLazyLoadingProxies().UseSqlServer(webApplicationBuilder.Configuration.GetConnectionString("DefaultConnection"));
            }
                );
            //For All [Product- productBrand- productCategory]
            webApplicationBuilder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            webApplicationBuilder.Services.AddAutoMapper(typeof(MappingProfiles));
            webApplicationBuilder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var errors = actionContext.ModelState.Where(p => p.Value.Errors.Count > 0)
                                                         .SelectMany(p => p.Value.Errors)
                                                         .Select(e => e.ErrorMessage).ToList();
                    var response = new ApiValidationErrorResponse()
                    {
                        Errors = errors
                    };
                    return new BadRequestObjectResult(response);
                };
            }
            );

            #endregion

            var app = webApplicationBuilder.Build();




            #region Update Database Dynamic
            var Scope = app.Services.CreateScope();
            var services = Scope.ServiceProvider;
            var _dbContext = services.GetRequiredService<StoreContext>();
            // Ask CLR for creating object from DbContext [Explicitly]

            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<Program>();
            try
            {
                 await _dbContext.Database.MigrateAsync(); //Update DataBase
                await StoreContextSeeding.SeedAsync(_dbContext);  // Data Seeding

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error has been occurred during apply Migration");
            }
            #endregion

            #region Kestrel Middlewares
            // Configure the HTTP request pipeline.

            ///1. ByConvention Based
            //app.UseMiddleware<ExceptionMiddleware>();

            ///2.Factory Based
            app.Use(async (httpContext, _next) =>
            {
                try
                {
                    //take an action with the request
                    await _next.Invoke(httpContext); // Go to next middleware
                                                     //take an action with the response
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message); // Development env
                    httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    httpContext.Response.ContentType = "application/json";
                    var response = app.Environment.IsDevelopment() ? new ApiExceptionResponse((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace.ToString()) :
                    new ApiExceptionResponse((int)HttpStatusCode.InternalServerError);
                    var options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                    var json = JsonSerializer.Serialize(response, options);
                    await httpContext.Response.WriteAsync(json);
                }

            });

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStatusCodePagesWithReExecute("/errors/{0}");
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.MapControllers();
            
            #endregion

            app.Run();
        }
    }
}
