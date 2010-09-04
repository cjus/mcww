#region GPL_FILE_HEADER
/* AppLogger.cs
   Copyright (C) 2005 Carlos Justiniano
   cjus@chessbrain.net, cjus34@yahoo.com, cjus@users.sourceforge.net

AppLogger.cs is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License as
published by the Free Software Foundation; either version 2 of the
License, or (at your option) any later version.

AppLogger.cs was developed by Carlos Justiniano for use on the
msgCourier project and the ChessBrain Project and is now distributed in
the hope that it will be useful, but WITHOUT ANY WARRANTY; without
even the implied warranty of MERCHANTABILITY or FITNESS FOR A
PARTICULAR PURPOSE.  See the GNU General Public License for more
details.

You should have received a copy of the GNU General Public License
along with AppLogger.cs; if not, write to the Free Software 
Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
*/
#endregion

using System;
using System.Windows.Forms;

namespace mcww
{
	/// <summary>
	/// Application message logger
	/// </summary>
	public class AppLogger
	{
		private ListBox appLog;

		public AppLogger(ListBox listBoxAppLog)
		{
			appLog = listBoxAppLog;
		}

		public void Reset()
		{
			appLog.Items.Clear();
		}

		public void Append(string message)
		{
			string TimeInString = "";
			int hour = DateTime.Now.Hour;
			if (hour > 12) hour -= 12;
			int min = DateTime.Now.Minute;
			int sec = DateTime.Now.Second;
			TimeInString = DateTime.Now.Year.ToString() + 
							DateTime.Now.Month.ToString() + 
							DateTime.Now.Day.ToString();
			TimeInString += "-";
			TimeInString += (hour < 10)? "0" + hour.ToString() : hour.ToString();
			TimeInString += ((min<10)? "0" + min.ToString() : min.ToString());
			TimeInString += ((sec<10)? "0" + sec.ToString() : sec.ToString());
			appLog.Items.Add("[" + TimeInString + "]  " + message);
		}
	}
}
