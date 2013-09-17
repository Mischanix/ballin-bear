using System;
using System.IO;
using System.Threading;
using System.Reflection;

using LinFu.AOP.Interfaces;

using UnityEngine;

using Our;

public class OurMono : MonoBehaviour {
  public static StreamWriter log;
  Timer t;
  bool doIt;
  static OurMono() {
    // Reflect our BattleNet implementation.
    // This relies on the static constructor being called after global::BattleNet's; our assembly
    // should be loaded after Assembly-CSharp in all cases, though.
    FieldInfo field = typeof(BattleNet).GetField("s_impl", BindingFlags.Static | BindingFlags.NonPublic);
    field.SetValue(null, new BattleNetMock());

    // Register our AroundInvoke for Debug.Log*
    AroundMethodBodyRegistry.AddProvider(new SimpleAroundInvokeProvider());

    // Test our log queueing
    Debug.LogWarning("Queueing test");
  }

  public void Awake() {
    doIt = false;
    log = File.AppendText("our.log");
    Util.Log("I'm in {0}", Util.AssemblyDirectory);
    t = new Timer((_) => doIt = true, null, 1000, Timeout.Infinite);
  }

  public void Update() {
    if (doIt) {
      doIt = false;
      Debug.LogWarning("LogWarning test");
    }


  }

  public void Start() {

  }
}
