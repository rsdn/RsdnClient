using System.Collections.Generic;
using JetBrains.Annotations;

namespace Rsdn.ApiClient
{
	[PublicAPI]
	internal static class ClientImplHelpers
	{
		public static TDictionary AddToQuery<TDictionary>(
			this TDictionary query,
			[InvokerParameterName] string name,
			string value)
		where TDictionary : IDictionary<string, string>
		{
			if (value != null)
				query.Add(name, value);
			return query;
		}

		public static TDictionary AddToQuery<TDictionary>(
			this TDictionary query,
			[InvokerParameterName] string name,
			object value)
			where TDictionary : IDictionary<string, string> =>
			AddToQuery(query, name, value?.ToString());
	}
}