using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

using BobNetProto;
using PegasusGame;
using UnityEngine;
using PegasusShared;
using Google.ProtocolBuffers;

namespace Our {
  class MockGameServer {
    public static void OnPacket(ClientConnection<PegasusPacket> conn, int method, byte[] body) {
      switch (method) {
        case 168:
          var auroraHandshake = AuroraHandshake.ParseFrom(body);
          Util.Log("AuroraHandshake = {0}, {1}, {2}", auroraHandshake.GameHandle, auroraHandshake.ClientHandle, auroraHandshake.Password);

          var gameStarting = GameStarting.CreateBuilder()
            .SetGameHandle(auroraHandshake.GameHandle);
          queue(conn, Network.PacketID.GAME_STARTING, gameStarting.Build());
          break;
        case 1:
          var gameSetup = GameSetup.CreateBuilder()
            .SetBoard("STR")
            .SetMaxFriendlyMinionsPerPlayer(9)
            .SetMaxSecretsPerPlayer(3);
          queue(conn, Network.PacketID.GAME_SETUP, gameSetup.Build());

          var histCreateGame = PowerHistoryCreateGame.CreateBuilder()
            .SetGameEntity(entity(0)
              .AddTags(gameTag(GAME_TAG.CARDTYPE, (int)TAG_CARDTYPE.GAME))
              .AddTags(gameTag(GAME_TAG.STEP, (int)TAG_STEP.BEGIN_FIRST))
              .AddTags(gameTag(GAME_TAG.NEXT_STEP, (int)TAG_STEP.MAIN_READY)))
            .AddPlayers(PegasusGame.Player.CreateBuilder()
              .SetId(1)
              .SetGameAccountId(bnetId(12, 34))
              .SetEntity(entity(1)
                .AddTags(gameTag(GAME_TAG.CARDTYPE, (int)TAG_CARDTYPE.PLAYER))
                .AddTags(gameTag(GAME_TAG.ENTITY_ID, 1))
                .AddTags(gameTag(GAME_TAG.CONTROLLER, 1))
                .AddTags(gameTag(GAME_TAG.PLAYER_ID, 1))))
            .AddPlayers(PegasusGame.Player.CreateBuilder()
              .SetId(2)
              .SetGameAccountId(bnetId(0, 0))
              .SetEntity(entity(2)
                .AddTags(gameTag(GAME_TAG.CARDTYPE, (int)TAG_CARDTYPE.PLAYER))
                .AddTags(gameTag(GAME_TAG.ENTITY_ID, 2))
                .AddTags(gameTag(GAME_TAG.CONTROLLER, 2))
                .AddTags(gameTag(GAME_TAG.PLAYER_ID, 2))));
          var hist = PowerHistory.CreateBuilder()
            .AddList(PowerHistoryData.CreateBuilder()
              .SetCreateGame(histCreateGame));
          queue(conn, Network.PacketID.POWER_HISTORY, hist.Build());

          var finGameState = FinishGameState.CreateBuilder();
          queue(conn, Network.PacketID.FIN_GAMESTATE, finGameState.Build());

          // Timer t;
          // t = new Timer((_) => {
          //   var fields = typeof(LoadingScreen).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
          //   var inst = LoadingScreen.Get();
          //   foreach (var field in fields) {
          //     Util.Log("LoadingScreen.{0} = {1}", field.Name, field.GetValue(inst));
          //   }
          //   var wellAreThey = typeof(Gameplay)
          //     .GetMethod("AreCriticalAssetsLoaded", BindingFlags.Instance | BindingFlags.NonPublic)
          //     .Invoke(Gameplay.Get(), null);
          //   Util.Log("Gameplay.AreCriticalAssetsLoaded = {0}", wellAreThey);
          //   t.Dispose(); }, null, 1000, Timeout.Infinite);
          break;
        case 113:
          var beginPlaying = BeginPlaying.ParseFrom(body);
          Util.Log("BeginPlaying = {0}", beginPlaying.Mode);
          if (beginPlaying.Mode == BeginPlaying.Types.Mode.READY) {
          }
          break;
        default:
          Util.Log(" ^^^ Unimplemented response (game) ^^^ ");
          break;
      }
    }

    static void queue(ClientConnection<PegasusPacket> conn, Network.PacketID type, IMessageLite msg) {
      conn.QueuePacket(new PegasusPacket((int)type, msg));
    }

    static PegasusGame.Tag.Builder gameTag(GAME_TAG name, int value) {
      return PegasusGame.Tag.CreateBuilder()
        .SetName((int)name)
        .SetValue(value);
    }

    static PegasusShared.BnetId.Builder bnetId(ulong hi, ulong lo) {
      return PegasusShared.BnetId.CreateBuilder()
        .SetHi(hi)
        .SetLo(lo);
    }

    static PegasusGame.Entity.Builder entity(int id) {
      return PegasusGame.Entity.CreateBuilder()
        .SetId(id);
    }
  }
}
