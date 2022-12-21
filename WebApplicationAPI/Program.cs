using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PATHServer;
using System.Reflection;
using System.Threading;

namespace WebApplicationAPI
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            IConfiguration configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", true, true)
               .Build();

            Server.instance = new Server();
            Server.instance.OnServerLog += _server_OnServerLog;
#if DEBUG
            Server.instance.StartTest().Wait();
#else
            Server.instance.StartTest().Wait();
#endif
            builder.Services.AddDbContext<MyDbContext>();

            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = " 📖 Documentation de l'API",
                    Description = "[![projet](https://badgen.net/badge/github/projet/blue?icon=github)](https://github.com/ichmat/WebServiceAPI) \n\n" +
                    "![NET](https://badgen.net/badge/github/NET7/blue?icon=nuget&label) \n\n" +
                    
                    "## 👩‍💻 Contacts\n" +
                    "[Cédric GUILLEMIN](mailto:cedric.guillemin@ynov.com)\n\n" +
                    "[Mattéo LOPES](mailto:matteo.lopes@ynov.com)\n\n" +
                    "",

                });

                // using System.Reflection;
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
            {
                builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
            }));

            var app = builder.Build();

#if DEBUG

#else
            app.Urls.Add("https://" + Server.GetLocalIPAddress() + ":7199");
#endif

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseCors("corsapp");

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        private static void _server_OnServerLog(string log)
        {
            Console.WriteLine(log);
        }
    }
}