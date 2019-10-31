using System.Net;
using Rsdn.Api.Models.Errors;

namespace Rsdn.Api.Models.Messages
{
	public static class WellKnownMessageErrors
	{
		public const string BaseUri = WellKnownErrors.BaseUri + "messages/";

		public static readonly ErrorType InvalidForum = new ErrorType(
			HttpStatusCode.BadRequest,
			BaseUri + "invalidForum");

		public static readonly ErrorType WriteToForumNotAuthorized = new ErrorType(
			HttpStatusCode.Unauthorized,
			BaseUri + "writeToForumNotAuthorized");

		public static readonly ErrorType ReplyToMessageNotFound = new ErrorType(
			HttpStatusCode.BadRequest,
			"replyToMessageNotFound");

		public static readonly ErrorType SubjectIsEmpty = new ErrorType(
			HttpStatusCode.BadRequest,
			BaseUri + "subjectIsEmpty");

		public static readonly ErrorType TopicIsClosed = new ErrorType(
			HttpStatusCode.Unauthorized,
			BaseUri + "topicIsClosed");

		public static readonly ErrorType CreateTopicNotAllowed = new ErrorType(
			HttpStatusCode.Unauthorized,
			BaseUri + "createTopicNotAllowed");
	}
}