using System;
using MonoDrone;

namespace Repl {

	class Repl {

		public static void Main(string[] args) {

			Console.WriteLine (" MonoDrone v1.0 REPL");
			Console.WriteLine (" Spoilers, write 'Help'");

			bool quit = false;
			string[] line;
			Drone drone = new Drone(args[0]);

			while (quit != true) {
			
				Console.Write (" -> ");
				line = Console.ReadLine ().Split (' ');
			
				switch (line[0]) {
				case "Up":
					drone.Up (double.Parse (line [1], System.Globalization.CultureInfo.InvariantCulture));
					break;

				case "Down":
					drone.Down (double.Parse (line [1], System.Globalization.CultureInfo.InvariantCulture));
					break;

				case "Strafe":
					drone.Strafe (double.Parse (line [1], System.Globalization.CultureInfo.InvariantCulture));
					break;

				case "Turn":
					drone.Turn (double.Parse (line [1], System.Globalization.CultureInfo.InvariantCulture));
					break;

				case "Forwards":
					drone.Forwards (double.Parse (line [1], System.Globalization.CultureInfo.InvariantCulture));
					break;

				case "Backwards":
					drone.Backwards (double.Parse (line [1], System.Globalization.CultureInfo.InvariantCulture));
					break;

				case "Land":
					drone.Land ();
					Console.WriteLine ("Flying - false");
					break;

				case "TakeOff":
					drone.TakeOff ();
					Console.WriteLine ("Flying - true");
					break;

				case "Status":
					Console.WriteLine ("Altitude: {0}m", drone.State (true, false, false, false));
					Console.WriteLine ("Battery: {0}%", drone.State (false, true, false, false));
					Console.WriteLine ("Flying: {0}", drone.State (false, false, true, false));
					Console.WriteLine ("Next Command: {0}", drone.State (false, false, false));
					break;

				case "Send":
					Console.WriteLine ("Sending Command: {0}", drone.State (false, false, false));
					drone.Send ();
					break;

				case "Quit":
					quit = true;
					Console.WriteLine ("Bye, bye!");
					break;

				case "Help":

					Console.WriteLine ("Available commands:");
					Console.Write ("Help, Quit, Send, Status, TakeOff, Land, Backwards <args>, Forwards <args>, Strafe <args>,");
					Console.WriteLine (" Turn <args>, Up <args>, Down <args>;");
					break;

				default:
					Console.WriteLine ("Unknown method {0}!", line [0]);
					break;
				}

			}

		}

	}

}
