using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Threading;
using Wexflow.Core;
using Scada.Data.Models;
using Scada.Data.Tables;


namespace Wexflow.Tasks.RSSendCmd
{
	public class RSSendCmd : Task
	{

		public List<RSCmd> rscmds { get; private set;}
		public XElement[] XCmds { get; private set;}

		public int Cnt { get; private set;}

		public RSSendCmd (XElement xe,Workflow wf) : base(xe,wf)
		{
			//XCmds = GetXSettings ("rscmd");


			rscmds = new List<RSCmd> ();


			
		}

		public override TaskStatus Run ()
		{
			Info ("Start RSSendCmd task");

			bool success = false;

			Cnt++;

			InfoFormat ("Value Cnt: {0}", Cnt);

			try
			{
				
			}
			catch(ThreadAbortException)
			{
				throw;
			}
			catch(Exception ex)
			{
				ErrorFormat ("Ann error RSSendCmd. Error: {0}", ex.Message);
			}

			Info ("Task RSSendCmd finished.");
			return new TaskStatus(Status.Success,success);
		}
	}
}

