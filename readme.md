### Hearthstone server mock-up

No plans for now, just having fun with it for whatever duration I can.

Whatever happens, making the gameplay/tutorials functional is last on my to-do
list.  If anything I'd like to make an actual server, with like a database n
stuff, before I do that.  Since I don't have pegasus protobuf stuff nor a way
to gen it, idk what to do about that besides proxy it to a different format or
write the protobufs by hand (ew).

#### Depends

- Everything in Hearthstone_Data/Managed
- LinFu.* -- This isn't necessary for the *functionality*, I'd like to stick to
reflection for that, it's just there as a PoC that currently modifies
UnityEngine.Debug
- See my gists for the script I use to get Unity to load OurMono; if you wanna
build this stuff or whatever, good luck, I'm not the best at documenting this
sort of thing, but feel free to ask any questions.
