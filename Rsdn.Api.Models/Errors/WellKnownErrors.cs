using System.Net;

namespace Rsdn.Api.Models.Errors
{
	public static class WellKnownErrors
	{
		public const string BaseUri = "https://api.rsdn.org/help/errors/";
		public const string CommonBaseUri = BaseUri + "common/";

		public static readonly ErrorType ParameterOutOfRange =
			new ErrorType(
				HttpStatusCode.BadRequest, 
				CommonBaseUri + "parameterOutOfRange");

		public static readonly ErrorType InternalServerError =
			new ErrorType(
				HttpStatusCode.InternalServerError,
				CommonBaseUri + "internalServerError");

		public static readonly ErrorType ArgumentNull =
			new ErrorType(
				HttpStatusCode.BadRequest,
				CommonBaseUri + "argumentNull");

		public static readonly ErrorType BadArgument =
			new ErrorType(
				HttpStatusCode.BadRequest,
				CommonBaseUri + "badArgument");

		public static readonly ErrorType AccountBanned = new ErrorType(
			HttpStatusCode.Unauthorized,
			BaseUri + "accountBanned");

		public static readonly ErrorType DuplicateRequest = new ErrorType(
			HttpStatusCode.Conflict,
			CommonBaseUri + "duplicateRequest");
	}
}