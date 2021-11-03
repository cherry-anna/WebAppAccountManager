using FluentValidation.AspNetCore;

using AccountManager.BusinessLogic.Services.Implementation;
using AccountManager.BusinessLogic.Services.Interfaces;
using AccountManager.DataAccess.Context;
using AccountManager.Domain.Models;
using WebAppAccountManager.Profiles;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using Microsoft.AspNetCore.Authentication;
using WebAppAccountManager.AuthenticationHandlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Any;
using WebAppAccountManager.Converter;

namespace WebAppAccountManager
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<AccountManagerContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("AccountManagerDB")));
            services.AddIdentity<User, IdentityRole<int>>()  
            .AddEntityFrameworkStores<AccountManagerContext>()
            .AddDefaultTokenProviders();

            services.AddScoped<IProjectService, ProjectService>(x=> new ProjectService(x.GetRequiredService<AccountManagerContext>()));
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IReportService, ReportService>();

            services.AddAuthentication("BasicAuthentication")
            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
            
            services.AddAuthorization(options =>
            {
                options.AddPolicy("BasicAuthentication", new AuthorizationPolicyBuilder("BasicAuthentication").RequireAuthenticatedUser().Build());
            });

            services.AddControllers()
                .AddFluentValidation(fvc =>
                {
                    fvc.RegisterValidatorsFromAssemblyContaining<Startup>();
                })

               .AddJsonOptions(options =>
            options.JsonSerializerOptions.Converters.Add(new CustomConverterTimeSpan()));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAppAccountManager", Version = "v1" });
                c.CustomSchemaIds(i => i.FullName);
                c.DocInclusionPredicate((docName, description) => true);
                c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "basic",
                    In = ParameterLocation.Header,
                    Description = "Basic Authorization header using the Bearer scheme."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "basic"
                                }
                            },
                            new string[] {}
                    }
                });

                //c.MapType<TimeSpan>(() => new OpenApiSchema { Type = "string", Format = "time-span" });

                c.MapType<TimeSpan>(() => new OpenApiSchema
                {
                    Type = "string",
                    Example = new OpenApiString("00:00:00")
                });

                //c.MapType<DateTime>(() => new OpenApiSchema { Type = "string", Format = "date" });

                //c.MapType<DateTime>(() => new OpenApiSchema
                //{
                //    Type = "string",
                //    Example = new OpenApiString("dd-MM-yyyy")
                //});
            });

            services.AddAutoMapper(typeof(ProjectProfile));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAppAccountManager v1"));
            }
            
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }

    }
}
