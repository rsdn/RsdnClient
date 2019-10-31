using System;
using System.Net;
using JetBrains.Annotations;

namespace Rsdn.Api.Models.Errors
{
	public class RsdnServiceException : Exception
	{
		/// <inheritdoc />
		public RsdnServiceException(
			ErrorType type,
			ErrorSeverity severity,
			string message,
			string detail,
			[CanBeNull] params ErrorParam[] errorParams) :  base(message)
		{
			ErrorCode = type.Type;
			StatusCode = type.StatusCode;
			Severity = severity;
			Detail = detail;
			if (errorParams != null)
				ErrorParams = errorParams;
		}

		public RsdnServiceException(
			ErrorType type,
			ErrorSeverity severity,
			string message,
			[CanBeNull] params ErrorParam[] errorParams)
			: this(type, severity, message, null, errorParams)
		{}

		[NotNull]
		public string ErrorCode { get; }

		public HttpStatusCode StatusCode { get; }

		public ErrorSeverity Severity { get; }

		[CanBeNull]
		public string Detail { get; }

		[NotNull]
		public ErrorParam[] ErrorParams { get; } = Array.Empty<ErrorParam>();
	}
}