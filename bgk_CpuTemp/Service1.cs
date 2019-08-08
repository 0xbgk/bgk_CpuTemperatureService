using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using OpenHardwareMonitor.Hardware;

namespace bgk_CpuTemp
{
	public partial class Service1 : ServiceBase
	{
		public class UpdateVisitor : IVisitor
		{
			public void VisitComputer(IComputer computer)
			{
				computer.Traverse(this);
			}
			public void VisitHardware(IHardware hardware)
			{
				hardware.Update();
				foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
			}
			public void VisitSensor(ISensor sensor) { }
			public void VisitParameter(IParameter parameter) { }
		}

		public static void GetSystemInfo()
		{
			string documentPath1 = @"\\192.168.0.200\XXXXX\XXXXX\";
			string documentPath2 = @"C:\";			
			string computerName = Environment.MachineName + ".txt";
			string computerHeat = "";
			UpdateVisitor updateVisitor = new UpdateVisitor();
			Computer computer = new Computer();
			computer.Open();
			computer.CPUEnabled = true;
			computer.Accept(updateVisitor);

			for (int i = 0; i < computer.Hardware.Length; i++)
			{
				if (computer.Hardware[i].HardwareType == HardwareType.CPU)
				{
					computerHeat += DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + "\n";
					for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
					{
						if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Temperature)
							computerHeat += (computer.Hardware[i].Sensors[j].Name + ": " + computer.Hardware[i].Sensors[j].Value.ToString() + " °C \n");
					}
				}
			}
			computer.Close();

			try
			{
				if (Directory.Exists(documentPath1))
				{
					File.AppendAllText(Path.Combine(documentPath1, computerName), computerHeat);
				}
			}
			catch (Exception e)
			{

			}
			try
			{
				if (Directory.Exists(documentPath2))
				{
					File.AppendAllText(Path.Combine(documentPath2, computerName), computerHeat);
				}
			}
			catch (Exception e)
			{

			}		

		}

		public Service1()
		{
			InitializeComponent();
		}

		Timer _timer = new Timer();
		int countMax = 0;

		protected override void OnStart(string[] args)
		{
			countMax = 0;
			_timer.Elapsed += TimerElapsed;
			// 2 SAAT
			//_timer.Interval = 2 * 60 * 1000;	
			_timer.Interval = 30 * 1000;
			_timer.Start();

			GetSystemInfo();
		}

		protected override void OnStop()
		{
			_timer.Stop();
		}

		private void TimerElapsed(object sender, ElapsedEventArgs e)
		{
			if (countMax > 9)
			{
				_timer.Enabled = ;
			}
			GetSystemInfo();
			countMax++;
		}

	}
}
