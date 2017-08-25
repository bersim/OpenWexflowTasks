using System;
using Scada.Data.Models;

namespace Wexflow.Tasks.RSSendCmd
{
	public class RSCmd
	{
		public int IDSrv { get; private set;}
		public Command Cmd { get; set;}

		public RSCmd (int idsrv)
		{
			IDSrv = idsrv;
			Cmd = new Command ();
		}
	}
}

