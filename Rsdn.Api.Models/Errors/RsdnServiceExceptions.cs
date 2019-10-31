namespace Rsdn.Api.Models.Errors
{
	/// <summary>
	/// Common exceptions factory.
	/// </summary>
	public static class RsdnServiceExceptions
	{
		public static RsdnServiceException OutOfRangeException(
			string message ,
			ErrorSeverity severity = ErrorSeverity.UserError) =>
			new RsdnServiceException(
				WellKnownErrors.ParameterOutOfRange,
				severity,
				message);

		public static RsdnServiceException ArgumentNullException(string argName, ErrorSeverity severity = ErrorSeverity.UserError) =>
			new RsdnServiceException(
				WellKnownErrors.ArgumentNull,
				severity,
				$"The argument {argName} can not be null.");
	}
}