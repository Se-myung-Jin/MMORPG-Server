START ../../PacketGenerator/bin/PacketGenerator.exe ../../PacketGenerator/PacketDefinitionList.xml
XCOPY /Y GenPackets.cs "../../DummyClient/001. Packet"
XCOPY /Y GenPackets.cs "../../Server/001. Packet"