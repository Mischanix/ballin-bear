﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using PegasusUtil;
using BobNetProto;

namespace Our {
  /// <summary>
  /// Don't worry; it gets worse.
  /// </summary>
  class BattleNetMock : IBattleNet, IConnectionListener<PegasusPacket> {
    ServerConnection<PegasusPacket> gameServer;
    public BattleNetMock() {
      outPackets = new Queue<PegasusPacket>();
      queueEvents = new Queue<BattleNet.QueueEvent>();
      gameQueue = new Queue<PegasusPacket>();
      gameServer = new ServerConnection<PegasusPacket>();
      gameServer.Open(1227);

    }

    #region Stateful global shit
    bool initialized;
    public bool Init(bool fromEditor) {
      if (initialized)
        return true;

      Util.Log("BattleNetMock Init");
      DebugConsole.Get().Init();
      initialized = ConnectAPI.ConnectInit();

      return initialized;
    }

    ClientConnection<PegasusPacket> gameClient;
    public void ProcessAurora() {
      if (gameClient != null && gameClient.Active) {
        gameClient.Update();
        return;
      }
      var conn = gameServer.GetNextAcceptedConnection();
      if (conn == null)
        return;
      gameClient = conn;

      gameClient.AddListener(this, gameQueue);
      gameClient.StartReceiving();
      gameClient.SetOnDisconnectHandler((e) => Util.Log("OnDisconnect = {0}", e));
      Util.Log("Game server accepted connection");
      gameClient.Update();
    }

    Queue<PegasusPacket> gameQueue;
    public void PacketReceived(PegasusPacket p, object state) {
      Util.Log("PacketReceived = {0}, {1}", p.Type, BitConverter.ToString((byte[])p.Body));
      // gameQueue.Enqueue(p);

      MockGameServer.OnPacket(gameClient, p.Type, (byte[])p.Body);
    }

    public int BattleNetStatus() {
      // Network.BnetLoginState enum
      return (int)Network.BnetLoginState.BATTLE_NET_LOGGED_IN;
    }

    public void AppQuit() {
    }
    #endregion
    #region Stateful bnet shit
    public int GetBnetEventsSize() {
      return 0;
    }

    public void GetBnetEvents(BattleNet.BnetEvent[] events) {
      // ^ See that array?  Fill it.
    }

    public void ClearBnetEvents() {
    }
    #endregion
    #region Stateful presence shit
    public int PresenceSize() {
      return 0;
    }

    public void GetPresence(BattleNet.PresenceUpdate[] updates) {
    }

    public void ClearPresence() {
    }

    public void SetPresenceBool(int field, bool val) {
      Util.Log("SetPresenceBool: {0}, {1}", field, val);
    }

    public void SetPresenceInt(int field, long val) {
      Util.Log("SetPresenceInt: {0}, {1}", field, val);
    }
    #endregion
    #region Stateful friends shit
    public void GetFriendsInfo(ref BattleNet.DllFriendsInfo info) {
      info.friendsSize = 0;
      info.updateSize = 0;
      info.maxFriends = 50;
      info.maxRecvInvites = 50;
      info.maxSentInvites = 50;
    }

    public void GetFriendsUpdates(BattleNet.FriendsUpdate[] updates) {
    }

    public void ClearFriendsUpdates() {
    }
    #endregion
    #region Stateful whispers shit
    public void GetWhisperInfo(ref BattleNet.DllWhisperInfo info) {
      info.whisperSize = 0;
      info.sendResultsSize = 0; // Should probably be not 0, yeah.
    }

    public void GetWhispers(BnetWhisper[] whispers) {
    }

    public void ClearWhispers() {
    }
    #endregion
    #region Stateful challenges shit
    public int NumChallenges() {
      return 0;
    }

    public void GetChallenges(BattleNet.DllChallengeInfo[] challenges) {
    }

    public void ClearChallenges() {
    }
    #endregion
    #region Stateful parties shit
    public void GetPartyUpdatesInfo(ref BattleNet.DllPartyInfo info) {
      info.size = 0; // Why isn't this just an int()?
    }

    public void GetPartyUpdates(BattleNet.PartyEvent[] updates) {
    }

    public void ClearPartyUpdates() {
    }
    #endregion
    #region Stateful errors! shit
    public int GetErrorsCount() {
      return 0;
    }

    public void GetErrors(BnetErrorInfo[] errors) {
    }

    public void ClearErrors() {
    }
    #endregion
    #region matchmaking
    Queue<BattleNet.QueueEvent> queueEvents;
    public BattleNet.QueueEvent GetQueueEvent() {
      if (queueEvents.Count > 0) {
        return queueEvents.Dequeue();
      }
      return null;
    }
    #endregion
    #region Login queue?
    bool queueInfoGotten;
    public void GetQueueInfo(ref BattleNet.DllQueueInfo queueInfo) {
      queueInfo.position = 0;
      queueInfo.end = 0;
      queueInfo.stdev = 100;
      if (!queueInfoGotten) {
        queueInfo.changed = true;
        queueInfoGotten = true;
      } else {
        queueInfo.changed = false;
      }
    }
    #endregion
    #region Account info
    // Network.BnetRegion
    public int GetAccountRegion() {
      return (int)Network.BnetRegion.REGION_US;
    }

    public int GetCurrentRegion() {
      return (int)Network.BnetRegion.REGION_US;
    }

    public BattleNet.DllEntityId GetMyGameAccountId() {
      var result = new BattleNet.DllEntityId();
      result.hi = 12;
      result.lo = 34;
      return result;
    }
    #endregion
    #region Util
    Queue<PegasusPacket> outPackets;
    public PegasusPacket NextUtilPacket() {
      if (outPackets.Count > 0) {
        var result = outPackets.Dequeue();
        Util.Log("Sending packet = {0}, {1}", result.Type, BitConverter.ToString((byte[])result.Body));
        return result;
      }
      return null;
    }

    public void SendUtilPacket(int packetId, int systemId, byte[] bytes, int size, int subID) {
      Util.Log("{0}, {1}, {2}, {3}, {4}", packetId, systemId, bytes, size, subID);
      switch (packetId) { // lol im server i promise ^^
        case 201:
          var getAccountInfo = GetAccountInfo.ParseFrom(bytes);
          Util.Log("GetAccountInfo = {0}", getAccountInfo);
          outPackets.Enqueue(MockResponse.OnGetAccountInfo(getAccountInfo));
          break;
        case 240:
          var getClientOptions = GetOptions.ParseFrom(bytes);
          outPackets.Enqueue(MockResponse.GetClientOptions());
          Util.Log("GetClientOptions = {0}", getClientOptions);
          break;
        case 253:
          var getAchieves = GetAchieves.ParseFrom(bytes);
          outPackets.Enqueue(MockResponse.OnGetAchieves(getAchieves));
          Util.Log("GetAchieved = {0}", getAchieves);
          break;
        case 237:
          var getBattlePayConfig = GetBattlePayConfig.ParseFrom(bytes);
          outPackets.Enqueue(MockResponse.OnGetBattlePayConfig());
          Util.Log("GetBattlePayConfig = {0}", getBattlePayConfig);
          break;
        case 255:
          var getBattlePayStatus = GetBattlePayStatus.ParseFrom(bytes);
          outPackets.Enqueue(MockResponse.OnGetBattlePayStatus());
          Util.Log("GetBattlePayStatus = {0}", getBattlePayStatus);
          break;
        case 225:
          var openBooster = OpenBooster.ParseFrom(bytes);
          Util.Log("OpenBooster = {0}", openBooster);
          outPackets.Enqueue(MockResponse.OnOpenBooster());
          break;
        case 239:
          var setOptions = SetOptions.ParseFrom(bytes);
          Util.Log("SetOptions = {0}", setOptions);
          // outPackets.Enqueue(MockResponse.OnSetOptions(setOptions));
          break;
        case 279:
          var purchaseWithGold = PurchaseWithGold.ParseFrom(bytes);
          Util.Log("PurchaseWithGold = {0}", purchaseWithGold);
          outPackets.Enqueue(MockResponse.OnPurchaseViaGold(purchaseWithGold));
          break;
        default:
          Util.Log(" ^^^ Unimplemented response ^^^ ");
          break;
      }
    }
    #endregion
    #region Missions
    public void StartScenario(int scenario, long deckID) {
      Util.Log("StartScenario = {0}, {1}", scenario, deckID);

      queueEvents.Enqueue(new BattleNet.QueueEvent(BattleNet.QueueEvent.Type.QUEUE_ENTER));
      queueEvents.Enqueue(new BattleNet.QueueEvent(BattleNet.QueueEvent.Type.QUEUE_GAME_STARTED, 0, 0, 0,
        new BattleNet.GameServerInfo {
          Address = "127.0.0.1",
          AuroraPassword = "",
          ClientHandle = 0xcc,
          GameHandle = 0xdd,
          Port = 1227,
          Version = "I'm a pretty princess"
        }));
      // on that new connection, 114:GameStarting
      // ConnectAPI.s_gameServer needs to be replaced with a non-networked
      // instance of ClientConnection<>, or use ServerConnection<>.
      // ClientConnection<> is totally not sealed though and I feel like that's
      // a useful thing.
    }

    public void StartScenarioAI(int scenario, long deckID, long aiDeckID) {
      Util.Log("StartScenarioAI = {0}, {1}, {2}", scenario, deckID, aiDeckID);
      throw new NotImplementedException();
    }
    #endregion
    public void AcceptPartyInvite(ref BattleNet.DllEntityId partyId) {
      throw new NotImplementedException();
    }

    public void AnswerChallenge(ulong challengeID, string answer) {
      throw new NotImplementedException();
    }

    public void CancelChallenge(ulong challengeID) {
      throw new NotImplementedException();
    }

    public bool CheckWebAuth(out string url) {
      throw new NotImplementedException();
    }

    public void CloseAurora() {
      throw new NotImplementedException();
    }

    public void DeclinePartyInvite(ref BattleNet.DllEntityId partyId) {
      throw new NotImplementedException();
    }

    public void DraftQueue(bool join) {
      throw new NotImplementedException();
    }

    public void MakeMatch(long deckID, bool newbie) {
      throw new NotImplementedException();
    }

    public void ManageFriendInvite(int action, ulong inviteId) {
      throw new NotImplementedException();
    }

    public void ProvideWebAuthToken(string token) {
      throw new NotImplementedException();
    }

    public void QueryAurora() {
      throw new NotImplementedException();
    }

    public void RemoveFriend(ref BnetAccountId account) {
      throw new NotImplementedException();
    }

    public void RescindPartyInvite(ref BattleNet.DllEntityId partyId) {
      throw new NotImplementedException();
    }

    public void SendFriendInvite(string inviter, string invitee, bool byEmail) {
      throw new NotImplementedException();
    }

    public void SendPartyInvite(ref BattleNet.DllEntityId gameAccount) {
      throw new NotImplementedException();
    }

    public void SendWhisper(BnetGameAccountId gameAccount, string message) {
      throw new NotImplementedException();
    }

    public void SetPartyDeck(ref BattleNet.DllEntityId partyId, long deckID) {
      throw new NotImplementedException();
    }

    public void UnrankedMatch(long deckID, bool newbie) {
      throw new NotImplementedException();
    }
  }
}
