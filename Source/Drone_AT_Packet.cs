using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MonoDrone
{
	public class AT { // AT commands

		private static int Seq_nr = 0;
		private int[] AT_Args;
		private string AT_Type;

		public AT (string type, params int[] args) {

			AT_Type = type;
			AT_Args = args;

		}

		public string Serialize () {

			Seq_nr++;
			return  "AT*" + AT_Type + "=" + Seq_nr + "," + string.Join (",", AT_Args) + "\r";

		}

	}
}

