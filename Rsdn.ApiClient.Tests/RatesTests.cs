using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Rsdn.Api.Models.Errors;
using Rsdn.Api.Models.Rates;

namespace Rsdn.ApiClient.Tests
{
	[TestFixture]
	public class RatesTests : TestsBase
	{
		private const int _messageID = 7484481;

		[Test]
		public async Task AddRate()
		{
			var me = await Client.Accounts.GetMeAsync();
			var now = DateTimeOffset.UtcNow;
			var smile = await Client.Rates.AddRate(RateType.Smile, _messageID);
			Assert.NotNull(smile);
			Assert.NotZero(smile.ID);
			Assert.AreEqual(me.ID, smile.RatedBy.ID);
			Assert.AreEqual(RateType.Smile, smile.Type);
			Assert.Greater(smile.RatedOn, now);

			var like = await Client.Rates.AddRate(RateType.Like, _messageID);
			Assert.NotNull(like);
			Assert.NotZero(like.ID);
			Assert.AreEqual(me.ID, like.RatedBy.ID);
			Assert.AreEqual(RateType.Like, like.Type);
			Assert.Greater(like.RatedOn, now);

			var x3 = await Client.Rates.AddRate(RateType.X3, _messageID);
			Assert.NotNull(x3);
			Assert.NotZero(x3.ID);
			Assert.AreEqual(me.ID, x3.RatedBy.ID);
			Assert.AreEqual(RateType.X3, x3.Type);
			Assert.NotZero(x3.RateBase.GetValueOrDefault());
			Assert.NotZero(x3.RateValue.GetValueOrDefault());
			Assert.Greater(x3.RatedOn, now);

			var msg = await Client.Messages.GetMessageAsync(_messageID, withRates: true);
			Assert.NotNull(msg?.Rates?.Rates.FirstOrDefault(r => r.Type == RateType.Smile && r.RateBy.ID == me.ID));
			Assert.NotNull(msg.Rates.Rates.FirstOrDefault(r => r.Type == RateType.Like && r.RateBy.ID == me.ID));
			Assert.NotNull(msg.Rates.Rates.FirstOrDefault(r => r.Type == RateType.X3 && r.RateBy.ID == me.ID));
		}

		[Test]
		public void AnonAddRate()
		{
			var ex = Assert.ThrowsAsync<RsdnServiceException>(async () =>
			{
				await AnonymousClient.Rates.AddRate(RateType.Smile, 7484481);
			});
			Assert.AreEqual(HttpStatusCode.Unauthorized, ex.StatusCode);
		}

		[Test]
		public void AddRateToSelf()
		{
			var ex = Assert.ThrowsAsync<RsdnServiceException>(
				async () => await Client.Rates.AddRate(RateType.Smile, 7495180));
			Assert.AreEqual(WellKnownRateErrors.CannotRateSelf.StatusCode, ex.StatusCode);
			Assert.AreEqual(WellKnownRateErrors.CannotRateSelf.Type, ex.ErrorCode);
		}

		[Test]
		public async Task DeleteRates()
		{
			await Client.Rates.AddRate(RateType.Smile, _messageID);
			await Client.Rates.AddRate(RateType.Dislike, _messageID);
			await Client.Rates.AddRate(RateType.X2, _messageID);

			await Client.Rates.DeleteRatesAsync(_messageID);

			var msg = await Client.Messages.GetMessageAsync(_messageID, withRates: true);
			var me = await Client.Accounts.GetMeAsync();
			Assert.IsFalse(msg?.Rates?.Rates.Any(r => r.RateBy.ID == me.ID));
		}

		[Test]
		public void AnonDeleteRates()
		{
			var ex = Assert.ThrowsAsync<RsdnServiceException>(
				async () => await AnonymousClient.Rates.DeleteRatesAsync(_messageID));
			Assert.AreEqual(HttpStatusCode.Unauthorized, ex.StatusCode);
		}
	}
}