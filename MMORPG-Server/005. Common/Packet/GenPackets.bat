START ../../"004. Tools"/PacketGenerator/bin/PacketGenerator.exe ../../"004. Tools"/PacketGenerator/PDL.xml
XCOPY /Y GenPackets.cs "../../003. Client/DummyClient/001. Packet"
XCOPY /Y GenPackets.cs "../../002. Server/Server/001. Packet"
XCOPY /Y ClientPacketManager.cs "../../003. Client/DummyClient/001. Packet"
XCOPY /Y ServerPacketManager.cs "../../002. Server/Server/001. Packet"