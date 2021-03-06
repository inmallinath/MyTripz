﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyTripz.Services;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MyTripz.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using AutoMapper;
using MyTripz.ViewModels;

namespace MyTripz
{
    //StartUp File
    public class Startup
    {
        public static IConfigurationRoot Configuration;

        public Startup(IHostingEnvironment hostEnv)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(hostEnv.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddJsonOptions(opt =>
                {
                    opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });

            services.AddLogging();
            var connection = @"Server=.\sqlexpress;Database=Tripz;Trusted_Connection=True;";
            services.AddDbContext<TripzContext>(options => options.UseSqlServer(connection));
            services.AddTransient<TripzContextSeedData>();
            services.AddScoped<ITripzRepository, TripzRepository>();
            services.AddScoped<CoordService>();

#if DEBUG
            services.AddScoped<IMailService, DebugMailService>();
#else
            services.AddScoped<IMailService, RealMailService>();
#endif
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, TripzContextSeedData seeder)
        {
            loggerFactory.AddDebug(LogLevel.Information);

            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
            app.UseStaticFiles();

            Mapper.Initialize( config =>
                    {
                        config.CreateMap<Trip, TripViewModel>().ReverseMap();
                        config.CreateMap<Stop, StopViewModel>().ReverseMap();
                    }
                );

            app.UseMvc(config => {
                config.MapRoute(
                    name: "Default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "App", action = "Index" }
                    );
            });

            seeder.EnsureSeedData();
        }
    }
}
