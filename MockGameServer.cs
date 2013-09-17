using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

          var builder = GameStarting.CreateBuilder()
            .SetGameHandle(auroraHandshake.GameHandle);
          BuildAndQueue(conn, Network.PacketID.GAME_STARTING, Util.AsArray(builder.Build()));


          var gameSetup = GameSetup.CreateBuilder()
            .SetBoard("STR")
            .SetMaxFriendlyMinionsPerPlayer(9)
            .SetMaxSecretsPerPlayer(3);
          BuildAndQueue(conn, Network.PacketID.GAME_SETUP, Util.AsArray(gameSetup.Build()));
          var histCreateGame = PowerHistoryCreateGame.CreateBuilder()
            .SetGameEntity(PegasusGame.Entity.CreateBuilder()
              .SetId(1000))
            .AddPlayers(PegasusGame.Player.CreateBuilder()
              .SetId(1001)
              .SetGameAccountId(BnetId.CreateBuilder()
                .SetHi(12)
                .SetLo(34))
              .SetEntity(PegasusGame.Entity.CreateBuilder()
                .SetId(2001)))
            .AddPlayers(PegasusGame.Player.CreateBuilder()
              .SetId(1002)
              .SetGameAccountId(BnetId.CreateBuilder()
                .SetHi(0)
                .SetLo(0))
              .SetEntity(PegasusGame.Entity.CreateBuilder()
                .SetId(2002)));
          var hist = PowerHistory.CreateBuilder()
            .AddList(PowerHistoryData.CreateBuilder()
              .SetCreateGame(histCreateGame));
          BuildAndQueue(conn, Network.PacketID.POWER_HISTORY, Util.AsArray(hist.Build()));
          break;
        default:
          Util.Log(" ^^^ Unimplemented response (game) ^^^ ");
          break;
      }
    }

    static void BuildAndQueue(ClientConnection<PegasusPacket> conn, Network.PacketID type, byte[] bytes) {
      conn.QueuePacket(new PegasusPacket((int)type, bytes));
    }
  }
}
