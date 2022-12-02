using Microsoft.OpenApi.Models;
using PATHServer;
using System.Reflection;

namespace WebApplicationAPI
{
    public class Program
    {
        public static Server _server;

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            _server = new Server();
#if DEBUG
            _server.StartTest();
#else
            _server.Start();
#endif
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
                    "![NET](https://badgen.net/badge/github/NET6/blue?icon=nuget&label) \n\n" +
                    
                    "## 👩‍💻 Contacts\n" +
                    "[Cédric GUILLEMIN](mailto:cedric.guillemin@ynov.com)\n\n" +
                    "[Mattéo LOPES](mailto:matteo.lopes@ynov.com)\n\n" +
                    "",

                });

                // using System.Reflection;
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}