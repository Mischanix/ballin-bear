using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

using Google.ProtocolBuffers;

namespace Our {
  class Util {
    // Get Unity's "Managed" directory without using AppDomain
    public static string AssemblyDirectory {
      get {
        string codeBase = Assembly.GetExecutingAssembly().CodeBase;
        UriBuilder uri = new UriBuilder(codeBase);
        string path = Uri.UnescapeDataString(uri.Path);
        return Path.GetDirectoryName(path);
      }
    }

    class LogArgs {
      public string format;
      public object[] args;
    }

    static Queue<LogArgs> queuedLogs = new Queue<LogArgs>();

    public static void Log(string format, params object[] args) {
      if (OurMono.log != null) {
        while (queuedLogs.Count > 0) {
            var queuedLog = queuedLogs.Dequeue();
            Blizzard.Log.SayToFile(OurMono.log, queuedLog.format, queuedLog.args);
        }
        Blizzard.Log.SayToFile(OurMono.log, format, args);
      } else {
        queuedLogs.Enqueue(new LogArgs { format = format, args = args });
      }
    }

    public static byte[] AsArray(IMessageLite packet) {
      byte[] buf;
      using (var stream = new MemoryStream()) {
        packet.WriteTo(stream);
        buf = stream.ToArray();
      }
      return buf;
    }

    #region What the actual fuck?
    public class Card {
      public int DatabaseAssetID {
        get;
        set;
      }
      public string CardID {
        get;
        set;
      }
      public bool Collectible {
        get;
        set;
      }
      public TAG_CARDTYPE CardType {
        get;
        set;
      }
      public override string ToString() {
        return String.Format(
          "AssetID = {0}, CardID = {1}, Collectible = {2}, CardType = {3}",
          DatabaseAssetID, CardID, Collectible, CardType
        );
      }
    }

    public static List<Card> AllCollectibles() {
      var cardManifestType = typeof(Network).Assembly.GetType("CardManifest", true);
      var cardType = typeof(Network).Assembly.GetType("CardManifest+Card", true);
      var singleton = cardManifestType.GetField("s_cardManifest", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
      var result = cardManifestType.GetMethod("AllCollectibles").Invoke(singleton, null);

      var cardList = new List<Card>();
      var cardArraySize = (int)typeof(List<>).MakeGenericType(cardType).GetProperty("Count").GetValue(result, null);
      var elementAt = typeof(Enumerable).GetMethod("ElementAt").MakeGenericMethod(cardType);
      for (var i = 0; i < cardArraySize; i++) {
        var cardElem = elementAt.Invoke(null, new object[] { result, i });
        var card = new Card();
        card.DatabaseAssetID = (int)cardType.GetProperty("DatabaseAssetID").GetValue(cardElem, null);
        card.CardID = (string)cardType.GetProperty("CardID").GetValue(cardElem, null);
        card.Collectible = (bool)cardType.GetProperty("Collectible").GetValue(cardElem, null);
        card.CardType = (TAG_CARDTYPE)cardType.GetProperty("CardType").GetValue(cardElem, null);
        cardList.Add(card);
      }
      // var cardArray = typeof(List<>).MakeGenericType(cardType).GetMethod("ToArray").Invoke(result, null);
      return cardList;
    }
    #endregion
  }
}
