using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rsdn.Api.Models
{
	public static class SerializerSettings
	{
		public static readonly JsonSerializerOptions Default =
			new JsonSerializerOptions
			{
				DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				Converters = { new JsonStringEnumConverter() },
				IgnoreNullValues = true,
#if DEBUG
				WriteIndented = true,
#endif
			};
	}
}