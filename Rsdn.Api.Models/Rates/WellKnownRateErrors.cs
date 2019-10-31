using System.Net;
using Rsdn.Api.Models.Errors;

namespace Rsdn.Api.Models.Rates
{
	public static class WellKnownRateErrors
	{
		public const string BaseUri = WellKnownErrors.BaseUri + "rates/";

		public static readonly ErrorType CannotRateSelf = new ErrorType(
			HttpStatusCode.BadRequest,
			BaseUri + "cannotRateSelf");

		public static readonly ErrorType RateLimitReached = new ErrorType(
			HttpStatusCode.BadRequest,
			BaseUri + "rateLimitReached");

		public static readonly ErrorType RateLimitToUserReached = new ErrorType(
			HttpStatusCode.BadRequest,
			BaseUri + "rateLimitToUserReached");
	}
}