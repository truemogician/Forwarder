// ReSharper disable CollectionNeverUpdated.Global
using System.Collections.Generic;
using Flurl.Http;

namespace Forwarder.Client.Flurl {
	/// <summary>
	/// </summary>
	public class ForwarderClientOptions {
		/// <summary>
		/// </summary>
		public bool AutoContentLength { get; set; } = true;

		/// <summary>
		/// </summary>
		public bool IgnoreConflictingHeader { get; set; } = true;
	}

	/// <summary>
	/// </summary>
	public class ForwarderClient {
		/// <summary>
		/// </summary>
		/// <param name="baseUrl"></param>
		/// <param name="options"></param>
		public ForwarderClient(string baseUrl, ForwarderClientOptions options = null) {
			BaseUrl = baseUrl;
			Options = options ?? new ForwarderClientOptions();
		}

		/// <summary>
		/// </summary>
		public ForwarderClientOptions Options { get; set; }

		/// <summary>
		/// </summary>
		public string BaseUrl { get; init; }

		/// <summary>
		/// </summary>
		public Dictionary<string, string> Headers { get; } = new();

		/// <summary>
		/// </summary>
		public List<FlurlCookie> Cookies { get; } = new();
	}
}