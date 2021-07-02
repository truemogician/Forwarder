using System;
using System.Linq;
using Flurl;
using Flurl.Http;

namespace Forwarder.Client.Flurl {
	/// <summary>
	/// </summary>
	public static class FlurlExtensions {
		/// <summary>
		/// </summary>
		/// <param name="request"></param>
		/// <param name="forwarder"></param>
		/// <returns></returns>
		public static ForwarderRequest UseForwarder(this IFlurlRequest request, ForwarderClient forwarder) => new(request, forwarder);

		/// <summary>
		/// </summary>
		/// <param name="url"></param>
		/// <param name="forwarder"></param>
		/// <returns></returns>
		public static ForwarderRequest UseForwarder(this Url url, ForwarderClient forwarder) => UseForwarder(new FlurlRequest(url), forwarder);

		/// <summary>
		/// </summary>
		/// <param name="url"></param>
		/// <param name="forwarder"></param>
		/// <returns></returns>
		public static ForwarderRequest UseForwarder(this Uri url, ForwarderClient forwarder) => UseForwarder(new Url(url), forwarder);

		/// <summary>
		/// </summary>
		/// <param name="url"></param>
		/// <param name="forwarder"></param>
		/// <returns></returns>
		public static ForwarderRequest UseForwarder(this string url, ForwarderClient forwarder) => UseForwarder(new Url(url), forwarder);

		/// <summary>
		/// </summary>
		/// <param name="request"></param>
		/// <param name="forwarderBaseUrl"></param>
		/// <returns></returns>
		public static ForwarderRequest UseForwarder(this IFlurlRequest request, string forwarderBaseUrl) => request.UseForwarder(new ForwarderClient(forwarderBaseUrl));

		/// <summary>
		/// </summary>
		/// <param name="url"></param>
		/// <param name="forwarderBaseUrl"></param>
		/// <returns></returns>
		public static ForwarderRequest UseForwarder(this Url url, string forwarderBaseUrl) => UseForwarder(new FlurlRequest(url), forwarderBaseUrl);

		/// <summary>
		/// </summary>
		/// <param name="url"></param>
		/// <param name="forwarderBaseUrl"></param>
		/// <returns></returns>
		public static ForwarderRequest UseForwarder(this Uri url, string forwarderBaseUrl) => UseForwarder(new Url(url), forwarderBaseUrl);

		/// <summary>
		/// </summary>
		/// <param name="url"></param>
		/// <param name="forwarderBaseUrl"></param>
		/// <returns></returns>
		public static ForwarderRequest UseForwarder(this string url, string forwarderBaseUrl) => UseForwarder(new Url(url), forwarderBaseUrl);

		/// <summary>
		/// </summary>
		/// <param name="url"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public static Url AppendPath(this Url url, string path) {
			string target = path;
			var result = new Url(url);
			if (target.StartsWith("/"))
				target = target[1..];
			if (target.EndsWith("/"))
				target = target[..^1];
			return result.AppendPathSegments(target.Split('/').AsEnumerable());
		}

		/// <summary>
		/// </summary>
		/// <param name="url"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public static Url AppendPath(this Uri url, string path) => ((Url)url).AppendPath(path);

		/// <summary>
		/// </summary>
		/// <param name="url"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public static Url AppendPath(this string url, string path) => ((Url)url).AppendPath(path);

		/// <summary>
		/// </summary>
		/// <param name="cookie"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public static bool Match(this FlurlCookie cookie, Url url) {
			string domain = cookie.Domain;
			if (domain.StartsWith("."))
				domain = domain[1..];
			int index = url.Host.IndexOf(domain, StringComparison.OrdinalIgnoreCase);
			if (index == -1 || index != 0 && url.Host[index - 1] != '.')
				return false;
			string path = url.Path;
			string cookiePath = cookie.Path;
			if (path == "/")
				path = string.Empty;
			if (cookiePath == "/")
				cookiePath = string.Empty;

			return path.StartsWith(cookiePath) && (path.Length == cookiePath.Length || path[cookiePath.Length] == '/');
		}
	}
}