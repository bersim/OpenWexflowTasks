using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Wexflow.Core;
using System.Xml.Linq;

using Scada.Data.Models;
using Scada.Data.Tables;

using org.mariuszgromada.math.mxparser;

namespace Wexflow.Tasks.RSSendValues
{
	public class RSSendValues : Task
	{

		public RSServers srvs { get; private set;}
		public string indata { get;private set;}
		public string[] cnlNums { get; private set;}
		public string[] xVars { get; private set;}
		public string[] sendValues {get;private set;}

		public STLs stls { get; private set;}

		public List<Argument> xArgs { get; private set;}
		public List<Cnl> Cnls { get; private set;}
		public List<Argument> CalcArgs { get; private set;}
		public List<Argument> SendVals { get; private set;}

		public SrezAdapter sa { get; private set;}

		public RSSendValues (XElement xe,Workflow wf): base(xe,wf)
		{
			srvs = wf.RSSrvs;

			indata = GetSetting ("indata");
			cnlNums = GetSettings ("cnlnum");
			xVars = GetSettings ("var");
			sendValues = GetSettings("sendValue");

			sa = new SrezAdapter ();
			stls = new STLs ();
			xArgs = new List<Argument> ();
			Cnls = new List<Cnl> ();
			CalcArgs = new List<Argument> ();
			SendVals = new List<Argument> ();

			LoadCnls ();
			LoadVars ();
			LoadSendValues ();

		}


		public void LoadCnls()
		{
			foreach(string cnl in cnlNums)
			{
				int srvId = int.Parse(cnl.Split(new char[]{'.'},StringSplitOptions.RemoveEmptyEntries)[0]);
				int cnlNum = int.Parse(cnl.Split(new char[]{'.'},StringSplitOptions.RemoveEmptyEntries)[1]);
				Argument lArg = new Argument("c"+ srvId + "_" + cnlNum.ToString(),0);
				Cnl cCnl = new Cnl (srvId, cnlNum);
				Cnls.Add (cCnl);
			}
		}

		public void LoadVars()
		{
			foreach(string xVar in xVars)
			{
				Argument lArg = new Argument (xVar);
				xArgs.Add (lArg);
			}
		}

		public void LoadSendValues()
		{
			foreach(string sv in sendValues)
			{
				Argument lArg = new Argument (sv);
				SendVals.Add (lArg);
			}
		}

		public override TaskStatus Run ()
		{
			Info("Start RSSendValues task");


			bool IsIndata = false;

			try {

				if(indata=="file")
				{
					foreach(FileInf fi in SelectFiles())
					{
						SrezTableLight stl = new SrezTableLight ();
						sa.FileName = fi.Path;
						sa.Fill(stl);
						int id = int.Parse (fi.FileName.Split (new char[]{'.'},StringSplitOptions.RemoveEmptyEntries)[1]);
						stls.AddSrez(id,stl.SrezList.Values[0]);
					}
					IsIndata  = true;
				}

				if (indata == "mem")
				{

					foreach(RSServer srv in srvs.GetRSServers())
					{
						stls.AddSrez(srv.Id,srv.GetCurrSrez());
					}
					IsIndata = true;

				}


				if(IsIndata)
				{
					foreach(Cnl cnl in Cnls)
					{
						Argument cArg = new Argument(cnl.ToString(),stls.GetCnlData(cnl.ID,cnl.Num));
						CalcArgs.Add(cArg);
					}
					CalcArgs.AddRange(xArgs);

					List<Cnl> cnls = new List<Cnl>();

					foreach(Argument arg in SendVals)
					{
						PrimitiveElement[] pes = CalcArgs.ToArray();
						arg.addDefinitions(pes);
						Cnl cnl = new Cnl(arg.getArgumentName(),arg.getArgumentValue(),1);
						cnls.Add(cnl);
						arg.removeDefinitions(CalcArgs.ToArray());
					}
					srvs.SendCurrSrez(cnls);
				}
				else{
					Info("No set parametr indata");
				}

				CalcArgs.Clear();
				stls.ClearSrez();
			} catch (ThreadAbortException) {
				throw;
			} catch (Exception ex) {
				InfoFormat ("An error RSSendValues: {0}", ex.Message);
			}
			Info("Task RSSendValues finished");
			return new TaskStatus (Status.Success, false);
		}
	}
}

