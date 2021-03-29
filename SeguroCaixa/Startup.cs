using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SeguroCaixa.Configuration;
using SeguroCaixa.Models;
using SeguroCaixa.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SeguroCaixa
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


            services.AddCors(options =>
            {
                options.AddPolicy("AllowAny", builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                );
            });

            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
            
            services.AddHttpsRedirection(options =>
            {
                options.HttpsPort = 443;
            });

            services.AddMemoryCache();
            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
               
            ) ; 

            services.AddDbContext<DbEscrita>(
               options =>
               {
                   options.UseSqlServer(Configuration.GetConnectionString("Escrita"),
                   providerOptions => providerOptions.EnableRetryOnFailure());
                   options.UseLazyLoadingProxies();
               });

            services.AddDbContext<DbLeitura>(
                options =>
                {
                    options.UseSqlServer(Configuration.GetConnectionString("Leitura"),
                    providerOptions => providerOptions.EnableRetryOnFailure());
                    options.UseLazyLoadingProxies();
                });

            services.AddDbContext<DbLeituraHistorico>(
           options =>
           {
               options.UseSqlServer(Configuration.GetConnectionString("LeituraHistorico"),
               providerOptions => providerOptions.EnableRetryOnFailure());
               options.UseLazyLoadingProxies();
           });

            services.AddScoped<SeguroCaixaService>();
            services.AddScoped<HistoricoService>(); 
            services.AddScoped<ProcessoService>();
            services.AddScoped<DocumentoService>();
            services.AddScoped<LocalidadeService>();
            services.AddScoped<NotificacaoService>();
            services.AddScoped<ParticipanteService>();


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SeguroCaixa", Version = "v1" });
            });

            services.AddSingleton<ITelemetryInitializer, CloudRoleNameTelemetryInitializer>();
            services.AddSingleton<ITelemetryInitializer, ApplicationInsightsInitializer>();
            services.AddApplicationInsightsTelemetryProcessor<SamplingProcessor>();
            services.AddApplicationInsightsTelemetry(new ApplicationInsightsServiceOptions { EnableAdaptiveSampling = false });

            services.Configure<LoginMicrosoftConfig>(Configuration.GetSection("LoginMicrosoft"));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                List<SecurityKey> lsKeys = new List<SecurityKey>();
                var publicJwkInternet = new JsonWebKey
                {
                    KeyId = Configuration["JWT:keyInternet:kid"],
                    Alg = Configuration["JWT:keyInternet:alg"],
                    E = Configuration["JWT:keyInternet:e"],
                    N = Configuration["JWT:keyInternet:n"],
                    Kty = Configuration["JWT:keyInternet:kty"],
                    Use = Configuration["JWT:keyInternet:use"]
                };
                lsKeys.Add(publicJwkInternet);
                var publicJwkIntranet = new JsonWebKey
                {
                    KeyId = Configuration["JWT:keyIntranet:kid"],
                    Alg = Configuration["JWT:keyIntranet:alg"],
                    E = Configuration["JWT:keyIntranet:e"],
                    N = Configuration["JWT:keyIntranet:n"],
                    Kty = Configuration["JWT:keyIntranet:kty"],
                    Use = Configuration["JWT:keyIntranet:use"]
                };
                lsKeys.Add(publicJwkIntranet);

                var publicJwkB2cCaixaTem = new JsonWebKey
                {
                    KeyId = Configuration["JWT:keyB2cCaixaTem:kid"],
                    E = Configuration["JWT:keyB2cCaixaTem:e"],
                    N = Configuration["JWT:keyB2cCaixaTem:n"],
                    Kty = Configuration["JWT:keyB2cCaixaTem:kty"],
                    Use = Configuration["JWT:keyB2cCaixaTem:use"]
                };
                lsKeys.Add(publicJwkB2cCaixaTem);


                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidIssuers = Configuration.GetSection("JWT:issuers").AsEnumerable().Select(q => q.Value),
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = false,
                    RequireExpirationTime = false,
                    IssuerSigningKeys = lsKeys,
                    ClockSkew = TimeSpan.FromSeconds(20)
                };
            });


            services.AddAuthorization(options =>
            {
                options.AddPolicy("Nivel12", policy => policy.RequireClaim("nivel", "12"));
                options.AddPolicy("Client_id_intranet", policy => policy.RequireClaim("azp", "cli-web-sdc"));
                options.AddPolicy("requer_role_usuario", policy => policy.RequireAssertion(
                   context => context.User.Claims.FirstOrDefault(x => x.Type.Equals("realm_access")).Value.Contains("SDC_USUARIO")
                    ));
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            // Definindo a cultura padr�o: pt-BR
            var supportedCultures = new[] { new CultureInfo("pt-BR") };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(culture: "pt-BR", uiCulture: "pt-BR"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });
            app.UseCors("AllowAny");

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "segurocaixa");
            });

            if (env.IsProduction())
            {
                app.UseHsts();
            }
            else
            {
                app.UseDeveloperExceptionPage();
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
