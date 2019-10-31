using System;

namespace Rsdn.Api.Models.Service
{
	public class ServiceInfo
	{
		public string Name { get; set; }
		public DateTimeOffset ServerTime { get; set; }
		public string ServerVersion { get; set; }
		public DateTimeOffset ServerBuildDate { get; set; }
	}
}