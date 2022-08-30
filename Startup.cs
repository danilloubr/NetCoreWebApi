using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.ResponseCompression;
using System.Linq;
using Microsoft.OpenApi.Models;

namespace Shop
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

            //CORS = Cors Origin Resource Chain - libera outros endereços de origin solicitar os métodos da api;
            services.AddCors();

            // Vaviável de ambiente para api comprimir todos os dados que forem application/json antes de mandar para o front, o html automagicamente irá saber converter.
            services.AddResponseCompression(options => {
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new [] { "application/json" });
            });

            //services.AddResponseCoching(); // Variável de ambiente para deixar a toda aplicação com funcionamento de Cache
            services.AddControllers();

            var key = Encoding.ASCII.GetBytes(Settings.Secret);

            services.AddAuthentication(user => {
                user.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                user.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(user => {
                user.RequireHttpsMetadata = false;
                user.SaveToken = true;
                user.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            // Variável de ambiente para teste da aplicação com dados em memória!
            // services.AddDbContext<DataContext>(options => options.UseInMemoryDatabase("Database"));

            // Variável de ambiente para conectar ao banco de dados Sql
            services.AddDbContext<DataContext>(options => options.UseSqlServer(Configuration.GetConnectionString("connectionString")));
            // services.AddScoped<DataContext, DataContext>(); // Nas novas versões o AddScoped já vem incluso no método AddDbContext;

            services.AddSwaggerGen(swagger => {
                swagger.SwaggerDoc("v1", new OpenApiInfo { Title = "Shop Api", Version = "v1"});
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(swagger => {
                swagger.SwaggerEndpoint("/swagger/v1/swagger.json", "Shop API V1");
            });

            app.UseRouting();

            app.UseCors( cors => {
                cors.AllowAnyOrigin();
                cors.AllowAnyMethod();
                cors.AllowAnyHeader();
            });

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
