using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MonoDrone
{
	class PacketLengthError : Exception {
		public PacketLengthError () {} 
	}

	public class Sender {

		private Socket S_socket = new Socket (AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

		private int PORT;

		private IPAddress IP;

		private IPEndPoint DroneServer;

		public Sender (IPAddress ar_ip, int ar_port) {

			IP = ar_ip;

			PORT = ar_port;

			DroneServer = new IPEndPoint (IP, PORT);

		}

		public void Send (string UnparsedCommand) { // the drone takes AT commands!

			byte[] command = GeneratePacket (UnparsedCommand);

			S_socket.SendTo (command, DroneServer); 

		}

		private byte[] GeneratePacket (string unparsed) {

			byte[] parsed = new byte [0];

			try {

				if (unparsed.Length > 1024) {

					throw new PacketLengthError ();

				} else {

					parsed = System.Text.ASCIIEncoding.ASCII.GetBytes (unparsed);

				}

			}
			catch (Exception err) {

				Console.Error.WriteLine ("{0} String exceeds the maximum packet lenght (1024)", err);

			}

			return parsed;

		}
			
	}

}

