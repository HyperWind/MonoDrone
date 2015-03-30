#!/bin/bash
mcs -unsafe -target:library Source/Drone_AT_Packet.cs Source/Drone_Packet_Getter.cs Source/Drone_Packet_Sender.cs Source/Drone_Main.cs -out:MonoDrone.dll

mv MonoDrone.dll Libs/MonoDrone.dll
