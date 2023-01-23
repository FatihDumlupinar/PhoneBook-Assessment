using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PhoneBook.Contact.Repositories.Contexts;
using PhoneBook.Contact.Repositories.Interfaces;
using PhoneBook.Contact.Repositories.Methods;
using PhoneBook.Contact.Repositories.Seed;
using System;

namespace PhoneBook.Contact.Api
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
            services
               .AddDbContext<ContactDbContext>(optionsAction:
               options => options.UseNpgsql(Configuration.GetConnectionString("ContactDbConnection")));

            services.AddTransient<IContactInfoRepository, ContactInfoRepository>();
            services.AddTransient<IContactDetailRepository, ContactDetailRepository>();

            services.AddCors();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PhoneBook.Contact.Api", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IServiceProvider provider, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PhoneBook.Contact.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            SeedData.EnsurePopulatedAsync(provider.GetRequiredService<ContactDbContext>()).Wait();
        }
    }
}
