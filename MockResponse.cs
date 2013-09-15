using System;
using System.Collections.Generic;
using System.Linq;

using PegasusUtil;
using PegasusShared;

namespace Our {
  class MockResponse {
    public static PegasusPacket OnGetAccountInfo(GetAccountInfo getAccountInfo) {
      switch (getAccountInfo.Request) {
        case GetAccountInfo.Types.Request.BOOSTERS:
          return BoostersReply();
        case GetAccountInfo.Types.Request.CAMPAIGN_INFO:
          return CampaignInfoReply();
        case GetAccountInfo.Types.Request.FEATURES:
          return FeaturesReply();
        case GetAccountInfo.Types.Request.NOTICES:
          return NoticesReply();
        case GetAccountInfo.Types.Request.DECK_LIST:
          return DeckListReply();
        case GetAccountInfo.Types.Request.COLLECTION:
          return CollectionReply();
        case GetAccountInfo.Types.Request.DECK_LIMIT:
          return DeckLimitReply();
        case GetAccountInfo.Types.Request.CARD_VALUES:
          return CardValuesReply();
        case GetAccountInfo.Types.Request.ARCANE_DUST_BALANCE:
          return ArcaneDustBalanceReply();
        case GetAccountInfo.Types.Request.REWARD_PROGRESS:
          return RewardProgressReply();
        case GetAccountInfo.Types.Request.PLAYER_RECORD:
          return PlayerRecordReply();
        case GetAccountInfo.Types.Request.DISCONNECTED:
          return DisconnectedReply();
        case GetAccountInfo.Types.Request.GOLD_BALANCE:
          return GoldBalanceReply();
        case GetAccountInfo.Types.Request.CARD_USAGE:
          return CardUsageReply();
        case GetAccountInfo.Types.Request.HERO_XP:
          return HeroXpReply();
        default:
          throw new NotImplementedException();
      }
    }

    static PegasusPacket BoostersReply() {
      var boostersBuilder = BoosterList.CreateBuilder();
      var boosterTestBuilder = BoosterInfo.CreateBuilder();
      boosterTestBuilder.SetCount(1)
        .SetType((int)BoosterType.EXPERT);
      boostersBuilder.AddList(boosterTestBuilder);
      var buf = Util.AsArray(boostersBuilder.Build());
      var packetTest = new PegasusPacket(224, buf.Length, buf);
      return packetTest;
    }

    static PegasusPacket CampaignInfoReply() {
      var builder = ProfileProgress.CreateBuilder();
      builder.SetBestForge(0)
        .SetProgress((long)MissionMgr.MissionProgress.ALL_TUTORIALS_COMPLETE);
      var buf = Util.AsArray(builder.Build());
      return new PegasusPacket(233, buf.Length, buf);
    }

    static PegasusPacket FeaturesReply() {
      var builder = GuardianVars.CreateBuilder();
      builder.SetShowUserUI(1);
      var buf = Util.AsArray(builder.Build());
      return new PegasusPacket(264, buf.Length, buf);
    }

    public static PegasusPacket GetClientOptions() {
      var builder = ClientOptions.CreateBuilder();
      // builder.SetOptions()
      var buf = Util.AsArray(builder.Build());
      return new PegasusPacket(241, buf.Length, buf);
    }

    static PegasusPacket NoticesReply() {
      var builder = ProfileNotices.CreateBuilder();
      // ...
      var buf = Util.AsArray(builder.Build());
      return new PegasusPacket(212, buf.Length, buf);
    }

    static PegasusPacket DeckListReply() {
      var builder = DeckList.CreateBuilder();
      var buf = Util.AsArray(builder.Build());
      return new PegasusPacket(202, buf.Length, buf);
    }

    static PegasusPacket CollectionReply() {
      var builder = DeckList.CreateBuilder();
      var buf = Util.AsArray(builder.Build());
      return new PegasusPacket(207, buf.Length, buf);
    }

    static PegasusPacket DeckLimitReply() {
      var builder = ProfileDeckLimit.CreateBuilder();
      builder.SetDeckLimit(90);
      var buf = Util.AsArray(builder.Build());
      return new PegasusPacket(231, buf.Length, buf);
    }

    static PegasusPacket CardValuesReply() {
      var builder = CardValues.CreateBuilder();
      var buf = Util.AsArray(builder.Build());
      return new PegasusPacket(260, buf.Length, buf);
    }

    static PegasusPacket ArcaneDustBalanceReply() {
      var builder = ArcaneDustBalance.CreateBuilder();
      builder.SetBalance(1001);
      var buf = Util.AsArray(builder.Build());
      return new PegasusPacket(262, buf.Length, buf);
    }

    static PegasusPacket RewardProgressReply() {
      var builder = RewardProgress.CreateBuilder();
      builder.SetGoldPerReward(5)
        .SetMaxHeroLevel(30)
        .SetSeasonEnd(Date.CreateBuilder()
          .SetYear(2013)
          .SetMonth(10)
          .SetDay(15)
          .SetHours(3)
          .SetMin(0)
          .SetSec(0))
        .SetWinsPerGold(5)
        .SetXpSoloLimit(10);
      var buf = Util.AsArray(builder.Build());
      return new PegasusPacket(271, buf.Length, buf);
    }

    static PegasusPacket PlayerRecordReply() {
      var builder = PlayerRecords.CreateBuilder();
      var buf = Util.AsArray(builder.Build());
      return new PegasusPacket(270, buf.Length, buf);
    }

    static PegasusPacket DisconnectedReply() {
      var builder = Disconnected.CreateBuilder();
      var buf = Util.AsArray(builder.Build());
      return new PegasusPacket(289, buf.Length, buf);
    }

    static PegasusPacket GoldBalanceReply() {
      var builder = GoldBalance.CreateBuilder();
      builder.SetBalance(1002);
      var buf = Util.AsArray(builder.Build());
      return new PegasusPacket(278, buf.Length, buf);
    }

    static PegasusPacket CardUsageReply() {
      var builder = CardUsage.CreateBuilder();
      var buf = Util.AsArray(builder.Build());
      return new PegasusPacket(236, buf.Length, buf);
    }

    static PegasusPacket HeroXpReply() {
      var builder = HeroXP.CreateBuilder();
      var buf = Util.AsArray(builder.Build());
      return new PegasusPacket(283, buf.Length, buf);
    }

    public static PegasusPacket OnGetAchieves(GetAchieves getAchieves) {
      var builder = Achieves.CreateBuilder();
      var buf = Util.AsArray(builder.Build());
      return new PegasusPacket(252, buf.Length, buf);
    }

    public static PegasusPacket OnGetBattlePayConfig() {
      var builder = BattlePayConfigResponse.CreateBuilder();
      builder.SetCurrency((int)Currency.USD);
      var buf = Util.AsArray(builder.Build());
      return new PegasusPacket(238, buf.Length, buf);
    }

    public static PegasusPacket OnGetBattlePayStatus() {
      var builder = BattlePayStatusResponse.CreateBuilder();
      builder.SetBattlePayAvailable(true)
        .SetStatus(BattlePayStatusResponse.Types.PurchaseState.PS_READY);
      var buf = Util.AsArray(builder.Build());
      return new PegasusPacket(265, buf.Length, buf);
    }

    public static PegasusPacket OnOpenBooster() {
      var builder = BoosterContent.CreateBuilder();

      var allCards = Util.AllCollectibles().Where(n => n.CardType != TAG_CARDTYPE.HERO).ToList();
      var r = new Random();
      for (var n = allCards.Count; n > 1;) {
        n--;
        var k = r.Next(n + 1);
        var c = allCards[k];
        allCards[k] = allCards[n];
        allCards[n] = c;
      }

      var now = DateTime.UtcNow;

      for (var i = 0; i < 5; i++) {
        builder.AddList(BoosterCard.CreateBuilder()
          .SetCardDef(PegasusShared.CardDef.CreateBuilder()
            .SetAsset(allCards[i].DatabaseAssetID)
            .SetPremium(r.Next(20) == 0 ? (int)CardFlair.PremiumType.FOIL : (int)CardFlair.PremiumType.STANDARD))
          .SetInsertDate(Date.CreateBuilder()
            .SetSec(now.Second)
            .SetMin(now.Minute)
            .SetHours(now.Hour)
            .SetDay(now.Day)
            .SetMonth(now.Month)
            .SetYear(now.Year))
        );
        Util.Log("BoosterCard = {0}", allCards[i]);
      }

      var buf = Util.AsArray(builder.Build());
      return new PegasusPacket(226, buf.Length, buf);
    }
  }
}
