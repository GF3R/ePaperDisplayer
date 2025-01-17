using EPaper.Web.Core.Models.Configurations;
using EPaper.Web.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EPaper.Web.Core
{
    using Services.Desk;

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
            services.AddControllers();

            var mqttConfiguration = Configuration
                .GetSection("MqttConfiguration")
                .Get<MqttConfiguration>();
            var typeCodeConfiguration = Configuration
                .GetSection("TypeCodeConfiguration")
                .Get<TypeCodeConfiguration>();
            var imageConfiguration = Configuration.GetSection("ImageConfiguration").Get<ImageConfiguration>();

            services.AddSingleton(mqttConfiguration);
            services.AddSingleton(typeCodeConfiguration);
            services.AddSingleton(imageConfiguration);
            services.AddScoped<IWeatherService, WeatherService>();
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IDeskService, DeskService>();
            services.AddScoped<IMqttService, MqttService>();
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();

            app.UseRouting();
            
            app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
            app.UseSwagger();
            app.UseSwaggerUI();
        }
    }
}
