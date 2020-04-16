using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Globalization;

namespace MapGen
{
	/// <summary>
	/// FormSearchResults 的摘要描述。
	/// </summary>
	public class FormSearchResults : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBoxResults;
		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.TextBox textBoxAnalyzing;
		private System.Windows.Forms.TextBox textBoxFound;
		private System.Windows.Forms.TextBox textBoxTime;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox textBoxOutput;
		private System.Windows.Forms.Button buttonResetSearch;
		
		// variables
		private SearchTaskInput _taskInput;
		private PathSearcherInput _pathInput;
		private MapGeneratorInput _mapInput;
		private Task _task;
		private System.Windows.Forms.Timer timerUpdateDisplay;
		private System.Windows.Forms.Timer timerSaveProgress;
		private System.ComponentModel.IContainer components;

		public FormSearchResults(SearchTaskInput taskInput, PathSearcherInput pathInput, MapGeneratorInput mapInput)
		{
			//
			// Windows Form 設計工具支援的必要項
			//
			InitializeComponent();

			// Input
			_taskInput = taskInput;
			_pathInput = pathInput;
			_mapInput = mapInput;

			// Write input values to textBoxOutput
            textBoxOutput.AppendText(String.Format("Matrix: {0} x {1}, {2}\r\n", _mapInput.Width, _mapInput.Height, _mapInput.FillRate));
			string fillAlgorithm = "Error";
            switch (_mapInput.FillAlgorithm) // 0 = Fixed Count, 1 = Random Count, 2 = Fractal
            {
			    case 0:
			        fillAlgorithm = "Fixed Count";
			        break;
			    case 1:
			        fillAlgorithm = "Random Count";
			        break;
			    case 2:
			        fillAlgorithm = "Fractal, H " + _mapInput.H.ToString();
			        break;
			}
            textBoxOutput.AppendText(String.Format("Fill: {0}\r\n", fillAlgorithm));
            string pathAlgorithm = "Error";
            /*
            switch (_pathInput.SearchAlgorithm) // 1 = Priority Queue Search, 2 = DepthFirstSearch
            {
                case 1:
                    pathAlgorithm = "Priority Queue Search";
                    break;
                case 2:
                    pathAlgorithm = "Depth First Search";
                    break;
            }
            */
            switch (_pathInput.SearchType) // 1 = 4 way, 2 = 8 way
            {
                case 1:
                    pathAlgorithm = "4 way";
                    break;
                case 2:
                    pathAlgorithm = "8 way";
                    break;
            }
            pathAlgorithm += ", Jump " + _pathInput.Jump.ToString();
            textBoxOutput.AppendText(String.Format("Path: {0}\r\n", pathAlgorithm));

			// Initialize progress display interface
			progressBar.Maximum = _taskInput.InnerIterations * _taskInput.OuterIterations;
			
            // Start task
			_task = new Task(taskInput, pathInput, mapInput);
			_task.Start();
			
			// Enable UI progress timer
			timerUpdateDisplay.Enabled = true;
			if (_taskInput.SaveProgress)
				timerSaveProgress.Enabled = true;
		}

		/// <summary>
		/// 清除任何使用中的資源。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form 設計工具產生的程式碼
		/// <summary>
		/// 此為設計工具支援所必須的方法 - 請勿使用程式碼編輯器修改
		/// 這個方法的內容。
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.groupBoxResults = new System.Windows.Forms.GroupBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.textBoxAnalyzing = new System.Windows.Forms.TextBox();
            this.textBoxFound = new System.Windows.Forms.TextBox();
            this.textBoxTime = new System.Windows.Forms.TextBox();
            this.textBoxOutput = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonResetSearch = new System.Windows.Forms.Button();
            this.timerUpdateDisplay = new System.Windows.Forms.Timer(this.components);
            this.timerSaveProgress = new System.Windows.Forms.Timer(this.components);
            this.groupBoxResults.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxResults
            // 
            this.groupBoxResults.Controls.Add(this.progressBar);
            this.groupBoxResults.Controls.Add(this.textBoxAnalyzing);
            this.groupBoxResults.Controls.Add(this.textBoxFound);
            this.groupBoxResults.Controls.Add(this.textBoxTime);
            this.groupBoxResults.Font = new System.Drawing.Font("Courier New", 9F);
            this.groupBoxResults.Location = new System.Drawing.Point(8, 8);
            this.groupBoxResults.Name = "groupBoxResults";
            this.groupBoxResults.Size = new System.Drawing.Size(355, 120);
            this.groupBoxResults.TabIndex = 30;
            this.groupBoxResults.TabStop = false;
            this.groupBoxResults.Text = "Progress";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(9, 19);
            this.progressBar.Maximum = 10000;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(340, 14);
            this.progressBar.TabIndex = 16;
            // 
            // textBoxAnalyzing
            // 
            this.textBoxAnalyzing.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxAnalyzing.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxAnalyzing.Font = new System.Drawing.Font("Courier New", 9F);
            this.textBoxAnalyzing.Location = new System.Drawing.Point(8, 48);
            this.textBoxAnalyzing.MaxLength = 50;
            this.textBoxAnalyzing.Name = "textBoxAnalyzing";
            this.textBoxAnalyzing.ReadOnly = true;
            this.textBoxAnalyzing.Size = new System.Drawing.Size(341, 14);
            this.textBoxAnalyzing.TabIndex = 33;
            this.textBoxAnalyzing.Text = "Analyzing...";
            // 
            // textBoxFound
            // 
            this.textBoxFound.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxFound.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxFound.Font = new System.Drawing.Font("Courier New", 9F);
            this.textBoxFound.Location = new System.Drawing.Point(8, 72);
            this.textBoxFound.MaxLength = 50;
            this.textBoxFound.Name = "textBoxFound";
            this.textBoxFound.ReadOnly = true;
            this.textBoxFound.Size = new System.Drawing.Size(341, 14);
            this.textBoxFound.TabIndex = 34;
            this.textBoxFound.Text = "Found:";
            // 
            // textBoxTime
            // 
            this.textBoxTime.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxTime.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxTime.Font = new System.Drawing.Font("Courier New", 9F);
            this.textBoxTime.Location = new System.Drawing.Point(8, 96);
            this.textBoxTime.MaxLength = 50;
            this.textBoxTime.Name = "textBoxTime";
            this.textBoxTime.ReadOnly = true;
            this.textBoxTime.Size = new System.Drawing.Size(341, 14);
            this.textBoxTime.TabIndex = 35;
            this.textBoxTime.Text = "Time:";
            // 
            // textBoxOutput
            // 
            this.textBoxOutput.BackColor = System.Drawing.SystemColors.Menu;
            this.textBoxOutput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxOutput.Font = new System.Drawing.Font("Courier New", 9F);
            this.textBoxOutput.Location = new System.Drawing.Point(8, 16);
            this.textBoxOutput.MaxLength = 50;
            this.textBoxOutput.Multiline = true;
            this.textBoxOutput.Name = "textBoxOutput";
            this.textBoxOutput.ReadOnly = true;
            this.textBoxOutput.Size = new System.Drawing.Size(341, 144);
            this.textBoxOutput.TabIndex = 36;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxOutput);
            this.groupBox1.Font = new System.Drawing.Font("Courier New", 9F);
            this.groupBox1.Location = new System.Drawing.Point(8, 136);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(355, 168);
            this.groupBox1.TabIndex = 37;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Output";
            // 
            // buttonResetSearch
            // 
            this.buttonResetSearch.Enabled = false;
            this.buttonResetSearch.Location = new System.Drawing.Point(159, 310);
            this.buttonResetSearch.Name = "buttonResetSearch";
            this.buttonResetSearch.Size = new System.Drawing.Size(54, 24);
            this.buttonResetSearch.TabIndex = 38;
            this.buttonResetSearch.Text = "Pause";
            this.buttonResetSearch.Click += new System.EventHandler(this.buttonResetSearch_Click);
            // 
            // timerUpdateDisplay
            // 
            this.timerUpdateDisplay.Tick += new System.EventHandler(this.timerUpdateDisplay_Tick);
            // 
            // timerSaveProgress
            // 
            this.timerSaveProgress.Interval = 5000;
            this.timerSaveProgress.Tick += new System.EventHandler(this.timerSaveProgress_Tick);
            // 
            // FormSearchResults
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 15);
            this.ClientSize = new System.Drawing.Size(372, 344);
            this.Controls.Add(this.buttonResetSearch);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBoxResults);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormSearchResults";
            this.Text = "Search";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSearchResults_FormClosing);
            this.groupBoxResults.ResumeLayout(false);
            this.groupBoxResults.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private void timerUpdateDisplay_Tick(object sender, System.EventArgs e)
        {
            double percentCompleted = (double)(_task.TotalCounter) / _task.TotalIterations;
            double runTime = (double)(_task.TotalTime + DateTime.Now.Ticks) / TimeSpan.TicksPerSecond;
            string strEstRunTime = "Calculating...";
            if (percentCompleted > 0.03)
            {
                double estRunTime = runTime / percentCompleted;
                double estSec = estRunTime % 60;
                estRunTime = Math.Floor(estRunTime / 60);
                double estMin = estRunTime % 60;
                estRunTime = Math.Floor(estRunTime / 60);
                double estHour = estRunTime;
                strEstRunTime = String.Format("{0:00}:{1:00}:{2:00}", estHour, estMin, estSec);
            }
            double sec = runTime % 60;
            runTime = Math.Floor(runTime / 60);
            double min = runTime % 60;
            runTime = Math.Floor(runTime / 60);
            double hour = runTime;
            string strRunTime = String.Format("{0:00}:{1:00}:{2:00.0}", hour, min, sec);
            //textBoxTime.Text = "Time: " + strRunTime;
			//textBoxTime.Text = String.Format("Timing: {0:0.0} Seconds", (double)(_task.TotalTime + DateTime.Now.Ticks) / TimeSpan.TicksPerSecond);
			//textBoxTime.Refresh();
			if (_task.TotalCounter < _task.TotalIterations)
			{
				// ProgressBar_Total
				progressBar.Value = _task.TotalCounter;
				progressBar.Refresh();
				// Label_Analyzing
                textBoxAnalyzing.Text = String.Format("Processing...{0,3:D}/{1,3:D} ({2:#0.00%})", _task.TotalCounter, _task.TotalIterations, percentCompleted);
				textBoxAnalyzing.Refresh();
				// Label_Found
				textBoxFound.Text = String.Format("Found: {0,3:D}/{1,3:D} ({2:#0.00%})", _task.TotalFound, _task.TotalCounter, ((double)(_task.TotalFound) / _task.TotalCounter));
				textBoxFound.Refresh();
                // Label_Time
                textBoxTime.Text = "Time: " + strRunTime + " (Est: " + strEstRunTime + ")";
				//textBoxTime.Text = String.Format("Timing: {0:0.0} Seconds", (double)(_task.TotalTime + DateTime.Now.Ticks) / TimeSpan.TicksPerSecond);
				textBoxTime.Refresh();
			}
			else if (_task.Results[_taskInput.OuterIterations - 1].Total == _taskInput.InnerIterations)
			{
				// Stop update timer
				timerUpdateDisplay.Enabled = false;
				timerSaveProgress.Enabled = false;
				// ProgressBar_Total
				progressBar.Value = _task.TotalCounter;
				// Label_Analyzing
				textBoxAnalyzing.Text = "Complete";
				// textBoxAnalyzing.Text = _task.Results[_taskInput.OuterIterations - 1].Total.ToString();
				// Label_Found
                textBoxFound.Text = String.Format("Found: {0,3:D}/{1,3:D} ({2:#0.00%}) ({3:#0.0000%})", _task.TotalFound, _task.TotalCounter, ((double)(_task.TotalFound) / _task.TotalCounter), Stdev(_task.Results));
                // Label_Time
                textBoxTime.Text = "Time: " + strRunTime;
                // Output
                textBoxOutput.AppendText(textBoxFound.Text + "\r\n");

				// build results string

                string strFillAlgorithm = "";
                switch (_mapInput.FillAlgorithm) // 0 = Fixed Count, 1 = Random Count, 2 = Fractal
                {
                    case 0:
                        strFillAlgorithm = "Fixed Count";
                        break;
                    case 1:
                        strFillAlgorithm = "Random Count";
                        break;
                    case 2:
                        strFillAlgorithm = "Fractal H " + _mapInput.H.ToString();
                        break;
                }
                string strDirectionJump = "";
                switch (_pathInput.SearchType) // 1 = 4 way, 2 = 8 way
                {
                    case 1:
                        strDirectionJump += "N4";
                        break;
                    case 2:
                        strDirectionJump += "N8";
                        break;
                }
                strDirectionJump += "J" + _pathInput.Jump;
                String report = String.Format(" {0} {1}, {2},{3}x{4},{5},{6},{7}x{8},{9},{10},{11}", DateTime.Now.ToString("d"), DateTime.Now.ToString("T", DateTimeFormatInfo.InvariantInfo), strRunTime, _taskInput.InnerIterations, _taskInput.OuterIterations, strFillAlgorithm, strDirectionJump, _mapInput.Width, _mapInput.Height, _mapInput.FillRate, ((double)(_task.TotalFound) / _task.TotalCounter), Stdev(_task.Results));
				foreach (Result result in _task.Results)
					report += String.Format(",{0}", (double)(result.Found) / result.Total);
                if (!File.Exists("results.csv"))
                {
                    using (StreamWriter streamWriterResults = File.AppendText("results.csv")) // save results to file
                    {
                        streamWriterResults.WriteLine("DateTime,RunTime,Iterations,FillAlgorithm,Path,MatrixSize,Fill,Found,Stdev");
                        streamWriterResults.Close();
                    }
                }
                using (StreamWriter streamWriterResults = File.AppendText("results.csv")) // save results to file
                {
                    streamWriterResults.WriteLine(report);
                    streamWriterResults.Close();
                }
				// reset button text
				//buttonStart.Text = "Start";
                // delete progress file
                if (File.Exists("progress.txt"))
                {
                    File.Delete("progress.txt");
                }
			}
		}

		private void timerSaveProgress_Tick(object sender, System.EventArgs e)
		{
			_task.SaveProgress();
		}

		private double Stdev(Result[] results)
		{
			if (results.Length <= 1)
				return 0;
			// Caculate AVG
			double[] dataArray = new double[results.Length];
			for (int i = 0; i < results.Length; i++)
				dataArray[i] = (double)(results[i].Found) / results[i].Total;
			double total = 0;
			foreach (double data in dataArray)
				total += data;
			double avg = total / dataArray.Length;
			// Calculate STDEV
			double stdev = 0;
			foreach (double data in dataArray)
				stdev += (data - avg) * (data - avg);
			return Math.Sqrt(stdev / (dataArray.Length * (dataArray.Length - 1)));
		}

        private void buttonResetSearch_Click(object sender, EventArgs e)
        {

        }

        private void FormSearchResults_FormClosing(object sender, FormClosingEventArgs e)
        {
            _task.Abort();
        }
	}
}
