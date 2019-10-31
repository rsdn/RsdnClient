using System;
using System.Net.Http.Headers;
using System.Reflection;

namespace Rsdn.ApiClient
{
	/// <summary>
	/// Common client information.
	/// </summary>
	public static class ClientInfo
	{
		private static readonly AssemblyName _assemblyName = typeof (RsdnApiClient).Assembly.GetName();

		/// <summary>
		/// Client version.
		/// </summary>
		public static Version ClientVersion => _assemblyName.Version;

		/// <summary>
		/// UserAgent header value.
		/// </summary>
		public static ProductInfoHeaderValue DefaultUserAgentHeader =
			new ProductInfoHeaderValue(_assemblyName.Name, _assemblyName.Version.ToString());
	}
}