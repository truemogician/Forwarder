// ReSharper disable CheckNamespace
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using NUnit.Framework;

namespace Forwarder.Client.Flurl.Test {
	public class ForwarderClientTests {
		public static readonly ForwarderClient Client = new("http://localhost:10000");

		[SetUp]
		public void Setup() { }

		[Test]
		public async Task RequestTest() {
			Url url = "https://www.truemogician.com:20210/api/token";
			Client.Cookies.Add(new FlurlCookie("client", "value", url));
			Client.Headers.Add("Client-Header", "Client");
			string response = await url.WithCookie("request", "value")
				.WithHeader("Content-Type", "application/json")
				.UseForwarder(Client)
				.GetStringAsync();
			Assert.IsNotNull(response);
		}
	}
}