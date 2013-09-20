set -e

PATH=/c/bin/mono/bin:$PATH

mcs -target:library -out:Our.dll -lib:lib -r:Assembly-CSharp.dll,Mono.Cecil.dll,LinFu.AOP.Cecil.dll,LinFu.AOP.Interfaces.dll,UnityEngine.dll,Google.ProtocolBuffersLite.dll AroundLog.cs BattleNetMock.cs MockGameServer.cs MockResponse.cs Our.cs Util.cs
cp -t /c/dev/mpq/Work/Hearthstone_Data/Managed Our.dll
