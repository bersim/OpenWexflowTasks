using System;
using Wexflow.Core;
using System.Xml.Linq;
using System.IO;
using System.Threading;

using Scada.Client;

namespace Wexflow.Tasks.RSGetCurSrez
{
	public class RSGetCurSrez : Task
	{
		public RSServers srvs { get; private set;}

		public int[] srvIDs { get; private set;}

		public RSGetCurSrez (XElement xe, Workflow wf) : base(xe,wf)
		{
			srvs = wf.RSSrvs;

			srvIDs = GetSettingsInt("rssrvID");




		}

		public override TaskStatus Run ()
		{
			Info ("Get current srez from Rapid Scada.");

			bool success = false;

			try
			{
				for(int i=0;i<srvIDs.Length;i++)
				{
					string filePath = Path.Combine(Workflow.WorkflowTempFolder,string.Format("rssrezID.{0}.dat",srvIDs[i].ToString()));
					RSServer rssrv = srvs.GetRSSrvId(srvIDs[i]);

					Stream fstream = new FileStream(filePath,FileMode.OpenOrCreate,FileAccess.Write,FileShare.ReadWrite);

					bool frec = rssrv.ReceiveFile(ServerComm.Dirs.Cur,"current.dat",fstream);
					fstream.Flush();

					success = frec;

					Files.Add(new FileInf(filePath,Id));

				}

				Info("Get current srez has been executed.");

			}
			catch(ThreadAbortException)
			{
				throw;
			}
			catch(Exception ex)
			{
				ErrorFormat ("Ann error get current srez from Rapid Scada. Error: {0}", ex.Message);
				success = false;
			}

			Status status = Status.Success;

			Info ("Task GetCurSrez finished.");
			return new TaskStatus (status, success);
		}
	}
}

