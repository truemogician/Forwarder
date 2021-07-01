using System.Diagnostics.CodeAnalysis;
using Forwarder.Converter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Forwarder {
	public class Startup {
		public Startup(IConfiguration configuration) => Configuration = configuration;

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services) {
			services.AddControllers();
			services.AddMvc(
					options => {
						options.InputFormatters.RemoveType<SystemTextJsonInputFormatter>();
						options.OutputFormatters.RemoveType<SystemTextJsonOutputFormatter>();
					}
				)
				.AddNewtonsoftJson(
					options => {
						options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
						options.SerializerSettings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
						options.SerializerSettings.Converters.Add(new HttpRequestMessageConverter());
						options.SerializerSettings.Converters.Add(new HttpResponseMessageConverter());
					}
				)
				.AddXmlSerializerFormatters();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
			app.UseRouting();
			app.UseAuthorization();
			app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
		}
	}
}