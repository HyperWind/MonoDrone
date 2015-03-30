using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace MonoDrone
{

	public class Getter {

		private UdpClient Listener;

		private IPEndPoint Client;

		public Getter (int PORT) {

			Listener = new UdpClient (PORT);

			Client = new IPEndPoint (IPAddress.Any, PORT);

		}

		public string Get () {

			byte[] data = new byte [1024];

			try {

				data = Listener.Receive (ref Client);

			}
			catch (Exception err) {

				Console.Error.WriteLine ("{0} The received package was too large! (>1024)", err);

			}

			return System.Text.Encoding.ASCII.GetString(data);

		}

		public void Close () {

			Listener.Close ();

		}

	}

}

