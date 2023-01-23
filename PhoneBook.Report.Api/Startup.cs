using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PhoneBook.Report.Api.BackgroundServices;
using PhoneBook.Report.Api.Contexts;
using PhoneBook.Report.Api.Models;
using PhoneBook.Report.Api.RabbitMQ;
using System;

namespace PhoneBook.Report.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ReportDbContext>(options => options.UseInMemoryDatabase(databaseName: "ReportDb"));

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PhoneBook.Report.Api", Version = "v1" });
            });

            #region HttpClient
            
            var sms_mail = Configuration["ContactApiUrl"];
            services.AddHttpClient("ContactClient", c =>
            {
                c.BaseAddress = new Uri(sms_mail);
            }); 

            #endregion

            #region RabbitMQ

            var rabbitConfig = Configuration.GetSection("RabbitMQ");
            services.Configure<RabbitOptions>(rabbitConfig);

            services.AddSingleton<IRabbitMqService, RabbitMqService>();
            services.AddSingleton<IConsumerService, ConsumerService>();
            services.AddHostedService<ConsumerHostedService>();

            #endregion

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PhoneBook.Report.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
