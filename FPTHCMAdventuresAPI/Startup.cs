using BusinessObjects.Model;
using DataAccess.Configuration;
using DataAccess.GenericRepositories;
using DataAccess.Repositories.EventRepositories;
using DataAccess.Repositories.EventTaskRepositories;
using DataAccess.Repositories.MajorRepositories;
using DataAccess.Repositories.QuestionRepositories;
using DataAccess.Repositories.TaskRepositories;
using DataAccess.Repositories.UserRepositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.OpenApi.Models;

using Service.Services.EventService;
using Service.Services.EventTaskService;
using Service.Services.MajorService;
using Service.Services.QuestionService;
using Service.Services.TaskService;
using System;
using System.Collections.Generic;

using Microsoft.IdentityModel.Tokens;
using System.Text;
using DataAccess.Repositories.LocationRepositories;
using DataAccess.Repositories.NPCRepository;
using Service.Services.LocationServoce;
using Service.Services.NpcService;
using DataAccess.Repositories.SchoolEventRepositories;
using DataAccess.Repositories.SchoolRepositories;
using DataAccess.Repositories.AnswerRepositories;
using DataAccess.Repositories.InventoryRepositories;
using DataAccess.Repositories.ItemRepositories;
using DataAccess.Repositories.PlayerRepositories;
using DataAccess.Repositories.PlayerHistoryRepositories;
using DataAccess.Repositories.ExchangeHistoryRepositories;
using Service.Services.SchoolEventService;
using Service.Services.SchoolService;
using Service.Services.AnswerService;
using Service.Services.InventoryService;
using Service.Services.ItemService;
using Service.Services.PlayerService;
using Service.Services.PlayerHistoryService;
using Service.Services.ExchangeHistoryService;
using Service.Services.ItemInventoryService;
using DataAccess.Repositories.ItemInventoryRepositories;
using OfficeOpenXml;
using ClosedXML.Excel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using DataAccess.Dtos.Users;
using Microsoft.AspNetCore.Authentication.Cookies;
using DataAccess.MiddleWareException;
using Serilog;
using Serilog.Events;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;
using HealthChecks.UI.Client;
using Service.HealthCheck;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using DataAccess.Repositories.PrizeRepositories;
using DataAccess.Repositories.StudentRepositories;
using DataAccess.Repositories.PlayerPrizeRepositories;
using Service.Services.StudentService;
using Service.Services.PlayerPrizeService;
using Service.Services.PrizeService;
using DataAccess.ImageSetting;
using DataAccess.Repositories.ImageRepository;
using Service.UpdateStatusHandler;
using Service.Email;

namespace FPTHCMAdventuresAPI
{
    public class Startup
    {
        public string ConectionString { get; set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ConectionString = Configuration.GetConnectionString("CapstonProjectDbConnectionString");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();

            services.AddDbContext<db_a9c31b_capstoneContext>(options => options.UseSqlServer(ConectionString));
            services.AddControllersWithViews().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddAutoMapper(typeof(MapperConfig));
            services.Configure<ImgurSettings>(Configuration.GetSection("ImgurSettings"));

            services.AddCors(options => {
                options.AddPolicy("AllowAll",
                    b => b.AllowAnyHeader()
                          .AllowAnyOrigin()
                          .AllowAnyMethod());
            });
            services.AddHostedService<StatusUpdateService>();

            services.AddSingleton<StatusUpdateScheduler>(
            serviceProvider => new StatusUpdateScheduler(
                     serviceProvider.GetRequiredService<IServiceScopeFactory>(),
                     TimeSpan.FromMinutes(10) 
                )
            );
          
            #region Repositories
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IEventRepositories, EventRepositories>();
            services.AddScoped<ITaskRepositories, TaskRepositories>();
            services.AddScoped<IEventTaskRepository, EventTaskRepository>();
            services.AddScoped<IMajorRepository, MajorRepository>();
            services.AddScoped<IQuestionRepository, QuestionRepository>();
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<INpcRepository, NpcRepository>();
            services.AddScoped<ISchoolEventRepository, SchoolEventRepository>();
            services.AddScoped<ISchoolRepository, SchoolRepository>();
            services.AddScoped<IAnswerRepository, AnswerRepository>();
            services.AddScoped<IItemInventoryRepositories, ItemInventoryRepositories>();
            services.AddScoped<IItemRepository, ItemRepository>();
            services.AddScoped<IInventoryRepository, InventoryRepository>();
            services.AddScoped<IPrizeRepository, PrizeRepository>();
            services.AddScoped<IPlayerRepository, PlayerRepository>();
            services.AddScoped<IPlayerHistoryRepository, PlayerHistoryRepository>();
            services.AddScoped<IExchangeHistoryRepository, ExchangeHistoryRepository>();
            services.AddScoped<IStudentRepositories, StudentRepositories>();
            services.AddScoped<IPlayerPrizeRepositories, PlayerPrizeRepositories>();
            services.AddScoped<IAuthManager, AuthManager>();
            services.AddScoped<IImageRepository, ImageRepository>();
            #endregion
            #region Excel
            services.AddScoped<ExcelPackage>();
            services.AddScoped<IXLWorkbook, XLWorkbook>();
            services.AddScoped<IWorkbook, XSSFWorkbook>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            #endregion

            services.AddHttpContextAccessor();

            var jwtSection = Configuration.GetSection("JWTSettings");
            var appSettings = jwtSection.Get<JWTSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.SecretKey);
            services.Configure<JWTSettings>(jwtSection);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = Configuration["JwtSettings:Issuer"],
                    ValidAudience = Configuration["JwtSettings:Audience"],
                    ClockSkew = TimeSpan.Zero
                };
            });


            services.AddSwaggerGen(options => {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "FPTHCM Adventure API", Version = "v1" });
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {
                                            Type = ReferenceType.SecurityScheme,
                                            Id = JwtBearerDefaults.AuthenticationScheme
                                        },
                            Scheme = "0auth2",
                            Name = JwtBearerDefaults.AuthenticationScheme,
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });


            services.AddResponseCaching(options =>
            {
                options.UseCaseSensitivePaths = true;
                options.MaximumBodySize = 1024;
            });
            #region Service
            //For DI Service
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IEventTaskService, EventTaskService>();
            services.AddScoped<IMajorService, MajorService>();
            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<INpcService, NpcService>();
            services.AddScoped<ISchoolEventService, SchoolEventService>();
            services.AddScoped<ISchoolService, SchoolService>();
            services.AddScoped<IAnswerService, AnswerService>();
            services.AddScoped<IItemInventoryService, ItemInventoryService>();
            services.AddScoped<IItemService, ItemService>();
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<IPlayerService, PlayerService>();
            services.AddScoped<IPlayerHistoryService, PlayerHistoryService>();
            services.AddScoped<IExchangeHistoryService, ExchangHistoryService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IPlayerPrizeService, PlayerPrizeService>();
            services.AddScoped<IPrizeService, PrizeService>();
            services.AddTransient<IEmailSender, EmailSender>();
            #endregion

            services.AddHealthChecks()
              .AddCheck<CustomHealthChecks>("Custom Health Check", failureStatus: HealthStatus.Degraded, tags: new[] { "custom" })
              .AddSqlServer(ConectionString , tags: new[] {"database"})
              .AddDbContextCheck<db_a9c31b_capstoneContext>(tags: new[] { "database" });
            services.AddHealthChecksUI(opt =>
            {
                opt.SetEvaluationTimeInSeconds(15); //time in seconds between check
            }).AddInMemoryStorage();
            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            /*  if (env.IsDevelopment())
              {
                 *//* app.UseDeveloperExceptionPage();
                  app.UseSwagger();

                  app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "XavalorAdventuresAPI v1"));*//*
              }*/

            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Documentation");
            });
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseCors("AllowAll");

            app.UseResponseCaching();

            app.Use(async (context, next) =>
            {
                context.Response.GetTypedHeaders().CacheControl =
                    new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
                    {
                        Public = true,
                        MaxAge = TimeSpan.FromSeconds(10)
                    };
                context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] =
                    new string[] { "Accept-Encoding" };

                await next();
            });
            app.UseAuthentication();
            app.UseAuthorization();
           
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

            });
            app.UseRouting()
          .UseEndpoints(config =>
          {
              config.MapHealthChecks("/health", new HealthCheckOptions
              {
                  ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
              });
              config.MapHealthChecks("/healthcheck", new HealthCheckOptions
              {
                  Predicate = healthcheck => healthcheck.Tags.Contains("custom"),
                  ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
                    [HealthStatus.Degraded] = StatusCodes.Status200OK,
                },
                  ResponseWriter = HealthCheckResponseWriter.WriteResponse
              });
              config.MapHealthChecks("/databasehealthcheck", new HealthCheckOptions
              {
                  Predicate = healthcheck => healthcheck.Tags.Contains("database"),
                  ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
                    [HealthStatus.Degraded] = StatusCodes.Status200OK,
                },
                  ResponseWriter = HealthCheckResponseWriter.WriteResponse
              });

              config.MapHealthChecksUI();

              config.MapDefaultControllerRoute();
          });
        }
    }
}
