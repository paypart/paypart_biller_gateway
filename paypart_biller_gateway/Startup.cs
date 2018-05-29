using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using paypart_biller_gateway.Services;
using Microsoft.Extensions.HealthChecks;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace paypart_biller_gateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options => {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            //in memory caching
            services.AddSession();


            //services.AddScoped<MongoAccess>();
            services.Configure<Settings>(options =>
            {
                options.connectionString = Configuration.GetSection("MongoConnection:ConnectionString").Value;
                options.database = Configuration.GetSection("MongoConnection:Database").Value;
                options.addBillerTopic = Configuration.GetSection("AppSettings:kafkaAddBillerTopic").Value;
                options.brokerList = Configuration.GetSection("AppSettings:kafkaBrokerList").Value;
                options.redisCancellationToken = Convert.ToInt32(Configuration.GetSection("AppSettings:redisCancellationToken").Value);
                options.hostingEnv = Environment.ContentRootPath;
            });

            //health check
            services.AddHealthChecks(checks =>
            {
                checks.AddValueTaskCheck("HTTP Endpoint", () => new
                ValueTask<IHealthCheckResult>(HealthCheckResult.Healthy("Ok")));
            });

            //Add Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "paypart billers gateway", Version = "v1" });
            });
            services.AddTransient<IBillerMongoRepository, BillerMongoRepository>();
            services.AddTransient<IBillerSqlServerRepository, BillerSqlServerRepository>();

            //Redis
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = Configuration.GetSection("AppSettings:redisIP").Value;
                options.InstanceName = Configuration.GetSection("AppSettings:redisInstanceName").Value;
            });

            //SQL Server
            services.AddDbContext<BillerSqlServerContext>(options => options.UseSqlServer(
                Configuration.GetSection("SqlServerConnection:ConnectionString").Value
                ));

            //Jwt Auth
            var audienceConfig = Configuration.GetSection("Audience");

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(audienceConfig["Secret"]));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = audienceConfig["Iss"],
                ValidateAudience = true,
                ValidAudience = audienceConfig["Aud"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,
            };

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = "TestKey";
            })
            .AddJwtBearer("TestKey", x =>
            {
                x.RequireHttpsMetadata = false;
                x.TokenValidationParameters = tokenValidationParameters;
            });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //app.UseSession();

            app.UseCors("CorsPolicy");
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "paypart billers gateway v1");
            });

            app.UseAuthentication();
            app.UseMvc();

        }
    }
}

