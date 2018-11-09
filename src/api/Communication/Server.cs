using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace api.Communication {
    public static class Server {
        private static readonly string ip = "localhost";
        private static readonly ushort port = 1548;

        private static IWebHost host = new WebHostBuilder()
            .UseKestrel()
            .UseStartup<Startup>()
            .UseUrls($"http://{ip}:{port}/")
            .Build();

        private class Startup {
            public Startup(IHostingEnvironment env) {}

            public void ConfigureServices(IServiceCollection services) {
                services.AddCors(options => {
                    options.AddPolicy("AllowAllOrigins", builder => { // Required for Frontend to be able to properly access API.
                        builder.AllowAnyOrigin();
                    });
                });

                services.AddMvc().AddJsonOptions(options => {
                    options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                });
            }

            public void Configure(IApplicationBuilder app) {
                app.UseCors("AllowAllOrigins");
                app.UseMvc();
            }
        }

        public static async void Start() {
            await Task.Run(() => {
                host.Run();
            });
        }
    }
}