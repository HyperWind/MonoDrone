using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MonoDrone;

namespace MonoDrone {

	/*
	 * <summary>
	 * Keeps the drone status and the following command.
	 * </summary>
	*/
	public class DroneStatus {

		public bool Flying = false; // Flying or not.
		public int Flags = 1; // Flag of the comming command. 
		public double TiltLR = 0; // Forwards/backwards tilt.
		public double TiltFB = 0; // Left/Right tilt.
		public double Vert = 0; // Vertical speed, aka. the amount you move up or down.
		public double Ang = 0;	// Angular speed, aka. the amount you turn left or right.
		public int LastAlt = 0; // Last altitude.
		public int Battery = 100; // Battery left.

		/*
		 * <summary>
		 * Resets the important bits, so you don't have to.
		 * </summary>
		*/
		public void Reset () {

			Flags = 1;
			TiltLR = 0;
			TiltFB = 0;
			Vert = 0;
			Ang = 0;

		}
			
	}

	/*
	 * <summary> 
	 * The exception thrown if a value passed to any of the movement commands is out of bounds (1 >= val >= -1).
	 * </summary>
	*/ 
	class CommandOutOfBounds : Exception {
		public CommandOutOfBounds () {}
	}

	public class Drone {

		private static int REPLY_PORT = 5554; // UDC port for drone navdata.
		private static int VIDEO_PORT = 5555; // UDC port for drone video. (not yet implamented!)
		private static int COMMAND_PORT = 5556; // UDC port for sending commands to the drone.
		private static IPAddress IP; // Ip address.
		private	Sender Sends; // Sender object.
		private Getter Gets; // Getter object.
		private DroneStatus Status; // Status class, keeps everything in check.

		/*
		 * <summary> 
		 * Initializes the drone.
		 * </summary>
		*/
		public Drone (string ip="192.168.1.1") {
			
			IP = IPAddress.Parse (ip);

			Status = new DroneStatus ();
			Sends = new Sender (IP, COMMAND_PORT);
			Gets = new Getter (REPLY_PORT);
		
		}

		/*
		 * <summary> 
		 * Transforms the value given to an integer word, aka. does magic and checks if the value is in bounds.
		 * </summary>
		*/
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

		/*
		 * <summary> 
		 * Serializes the command and sends it to the mothership.
		 * </summary>
		*/
		private void Move (int flags, double _LeftRightTilt = 0, double _FrontBackTilt = 0, double _VerticalSpeed = 0, double _AngularSpeed = 0) {

			int LeftRightTilt = DoubleRefToInt (_LeftRightTilt);
			int FrontBackTilt = DoubleRefToInt (_FrontBackTilt);
			int VerticalSpeed = DoubleRefToInt (_VerticalSpeed);
			int AngularSpeed = DoubleRefToInt (_AngularSpeed);

			AT at = new AT ("PCMD", flags, LeftRightTilt, FrontBackTilt, VerticalSpeed, AngularSpeed);
			AT buffer = new AT ("PCMD", 1, 0, 0, 0, 0);
	
			Sends.Send (at.Serialize ());
			Sends.Send (buffer.Serialize ());	
		}

		/*
		 * <summary>
		 * Makes the drone move forwards, only allows commands from 1 to 0.
		 * </summary>
		*/
		public void Forwards (double amount) {

			try {

				if (amount >= 0 && amount <= 1) {
				
					Status.TiltFB = amount;
					Status.Flags = 1;

				} else {

					throw new CommandOutOfBounds();

				}

			} catch (Exception err) {

				Console.Error.WriteLine ("{0} Argument cannot be over 1 or negative.", err);

			}

		}

		/*
		 * <summary>
		 * Makes the drone move backwards, also accepts only positive numbers (and 0) up to 1.
		 * </summary>
		*/
		public void Backwards (double amount) {

			try {

				if (amount >= 0 && amount <= 1) {

					Status.TiltFB = -amount;
					Status.Flags = 1;

				} else {

					throw new CommandOutOfBounds();

				}

			} catch (Exception err) {

				Console.Error.WriteLine("{0} Argument cannot be over 1 or negative.", err);

			}

		}
	
		/*
		 * <summary>
		 * Makes the drone strafe left or right, that depends on the sign of the number.
		 * (positive - left, negative - right)
		 * </summary>
		*/
		public void Strafe (double amount) {
			Status.TiltLR = amount;
			Status.Flags = 1;
		}

		/*
		 * <summary> 
		 * Makes the drone turn left or right, also dictated by the sign of the number.
		 * (positive - left, negative - right)
		 * </summary>
		 * 
		 * <suggestion>
		 * Make it accept degrees instead?
		 * </suggestion>
		*/
		public void Turn (double amount) {
			Status.Ang = amount;
			Status.Flags = 1;
		}

		/*
		 * <summary> 
		 * Makes the thing go up, 0 <= amount <= 1.
		 * </summary>
		*/
		public void Up (double amount) {

			try {

				if (amount >= 0 && amount <= 1) {

					Status.Vert = amount;
					Status.Flags = 1;

				} else {

					throw new CommandOutOfBounds();

				}

			} catch (Exception err) {

				Console.Error.WriteLine ("{0} Argument cannot be over 1 or negative.", err);

			}

		}
	
		/*
		 * <summary> 
		 * Makes it go down (wow), again, the 'amount' can't be over 1 or negative.
		 * </summary>
		*/
		public void Down (double amount) {

			try {

				if (amount >= 0 && amount <= 1) {

					Status.Vert = -amount;
					Status.Flags = 1;

				} else {

					throw new CommandOutOfBounds();

				}

			} catch (Exception err) {

				Console.Error.WriteLine ("{0} Argument cannot be over 1 or negative.", err);

			}
		}

		/*
		 * <summary>
		 * Makes the thing hover.
		 * </summary>
		*/
		public void Hover () {
			Status.Reset ();
			Status.Flags = 0;
		}

		/*
		 * <summary>
		 * After all of the wanted commands have been issued - sends them to the drone and resets the state.
		 * 
		 * If the drone isn't flying - throws a warning instead and resets the state.
		 * </summary>
		*/
		public void Send () {

			if (Status.Flying == true) {

				Move (Status.Flags, Status.TiltLR, Status.TiltFB, Status.Vert, Status.Ang); 

			} else {

				Console.Error.WriteLine ("MonoDrone Warning - Drone must be flying first!");

			}
				
			Status.Reset ();

		}

		/*
		 * <summary>
		 * Lands the drone.
		 * </summary>
		*/
		public void Land () {

			Status.Flying = false;
			AT at = new AT ("REF", 290717696);
			Sends.Send (at.Serialize ());
		
		}

		/*
		 * <summary>
		 * As the name suggests, makes the drone take off.
		 * </summary>
		*/
		public void TakeOff () {

			Status.Flying = true;
			AT at = new AT ("REF", 290718208);
			Sends.Send (at.Serialize ());		

		}

		/*
		 * <summary> 
		 * Returns the state of the drone as a formatted string.
		 * </summary>
		*/
		public string State (bool Altitude = true, bool Battery = true, bool Flying = true, bool Command = true) {

			string state = "";

			if (Altitude == true) 
				state = state + String.Format ("Altitude: {0}\r", Status.LastAlt);
			if (Battery == true)
				state = state + String.Format ("Battery left: {0}%\r", Status.Battery);
			if (Flying == true)
				state = state + String.Format ("Flying: {0}\r", Status.Flying);
			if (Command == true) {
				state = state + String.Format (
					"Next Command: {0}, {1}, {2}, {3}, {4}\r",
					Status.Flags,
					Status.TiltFB,
					Status.TiltLR,
					Status.Vert,
					Status.Ang
				);
			}

			return state;

		}

	}

}
