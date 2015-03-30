using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MonoDrone;

namespace MonoDrone {

	class CommandOutOfBounds : Exception {
		public CommandOutOfBounds () {}
	}

	public class Drone {

		private static int REPLY_PORT = 5554; // UDC
		private static int VIDEO_PORT = 5555; // UDC
		private static int COMMAND_PORT = 5556; // UDC
		private static IPAddress IP; // ip address
		private	Sender sends;
		private Getter gets;

		public Drone (string ip="192.168.1.1") {
			
			IP = IPAddress.Parse (ip);

			sends = new Sender (IP, COMMAND_PORT);
			gets = new Getter (REPLY_PORT);
		
		}

		private unsafe int DoubleRefToInt (double conv) {

			int converted = 0;

			try {
			
				if (conv <= 1 && conv >= -1) {
			
					float convF = (float)conv;
					converted = *(int*)&convF;
				
				} else {

					throw new CommandOutOfBounds ();
				
				}

			}
			catch (Exception err) {

				Console.Error.WriteLine ("{0} argument exceeds maximum size (must be between 1 and -1)", err);
			
			}

			return converted;
		
		}

		public void Move (int flags, double _LeftRightTilt = 0, double _FrontBackTilt = 0, double _VerticalSpeed = 0, double _AngularSpeed = 0) {

			int LeftRightTilt = DoubleRefToInt (_LeftRightTilt);
			int FrontBackTilt = DoubleRefToInt (_FrontBackTilt);
			int VerticalSpeed = DoubleRefToInt (_VerticalSpeed);
			int AngularSpeed = DoubleRefToInt (_AngularSpeed);

			AT at = new AT ("PCMD", flags, LeftRightTilt, FrontBackTilt, VerticalSpeed, AngularSpeed);
			AT buffer = new AT ("PCMD", 1, 0, 0, 0, 0);
	
			sends.Send (at.Serialize ());
			sends.Send (buffer.Serialize ());	
		}

/*		public void Forwards (double amount) {
			Move (1);
		}

		public void Backwards (double amount) {
			Move (1);
		}
	
		public void StrafeLeft (double amount) {
			Move (1);
		}

		public void StrafeRight (double amount) {
			Move (1);
		}

		public void TurnLeft (double amount) {
			Move (1);
		}

		public void TurnRight (double amount) {
			Move (1);
		}

		public void Up (double amount) {
			Move (1);
		}
	
		public void Down (double amount) {
			Move (1);
		}
*/		
		public void Hover () {

			Move (0);
		
		}

		public void Land () {

			AT at = new AT ("REF", 290717696);
			sends.Send (at.Serialize ());
		
		}

		public void TakeOff () {

			AT at = new AT ("REF", 290718208);
			sends.Send (at.Serialize ());		

		}

		public void Wait (int seconds, Action action) {

			Thread.Sleep (seconds * 1000);

			action ();

		} 

	}

}
