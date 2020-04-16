using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;

namespace MapGen
{
	public class SearchTaskInput
	{
		#region Input variables
		private int _outerIterations;
		private int _innerIterations;
		private bool _displayImage;
		private int _threadNum;
		private bool _saveProgress;
		#endregion

		#region Properties
		public int OuterIterations
		{
			get {return _outerIterations;}
			set {_outerIterations = value >= 0 ? value : 0;}
		}
		public int InnerIterations
		{
			get {return _innerIterations;}
			set {_innerIterations = value >= 0 ? value : 0;}
		}
		public bool DisplayImage
		{
			get {return _displayImage;}
			set {_displayImage = value;}
		}
		public int ThreadNum
		{
			get {return _threadNum;}
			set {_threadNum = value >= 1 ? value : 1;}
		}
		public bool SaveProgress
		{
			get {return _saveProgress;}
			set {_saveProgress = value;}
		}
		#endregion

		#region Constructor
		public SearchTaskInput()
		{
			OuterIterations = 1;
			InnerIterations = 1;
			DisplayImage = false;
			ThreadNum = 1;
			SaveProgress = true;
		}
		public SearchTaskInput(int outerIterations, int innerIterations, bool displayImage, int threadNum, bool saveProgress)
		{
			OuterIterations = outerIterations;
			InnerIterations = innerIterations;
			DisplayImage = displayImage;
			ThreadNum = threadNum;
			SaveProgress = saveProgress;
		}
		#endregion
	}

	public class Result
	{
		#region Input variables
		private int _found;
		private int _total;
		#endregion

		#region Properties
		public int Found
		{
			get {return _found;}
			set {_found = value;}
		}
		public int Total
		{
			get {return _total;}
			set {_total = value;}
		}
		#endregion

		#region Constructor
		public Result()
		{
			Found = 0;
			Total = 0;
		}
		public Result(int found, int total)
		{
			Found = found;
			Total = total;
		}
		#endregion
	}

	public class Task
	{
		#region Private variables
		// User input data
		private SearchTaskInput _taskInput;
		private PathSearcherInput _pathInput;
		private MapGeneratorInput _mapInput;
		// Calculated variables
		private int _totalIterations;
		// State variables
		private long _totalTime;
		private int _totalFound;
		private int _totalCounter;
		private Result[] _results;
		private Thread[] _threads;
		#endregion

		#region Properties
		public SearchTaskInput TaskInput
		{
			get {return _taskInput;}
			set {_taskInput = value;}
		}
		public MapGeneratorInput MapInput
		{
			get {return _mapInput;}
		}
		public PathSearcherInput PathInput
		{
			get {return _pathInput;}
		}
		public int TotalIterations
		{
			get {return _totalIterations;}
		}
		public long TotalTime
		{
			get {return _totalTime;}
		}
		public int TotalFound
		{
			get {return _totalFound;}
		}
		public int TotalCounter
		{
			get {return _totalCounter;}
		}
		public Result[] Results
		{
			get {return _results;}
		}
		#endregion

		#region Constructors
		public Task()
		{
			
		}
		public Task(SearchTaskInput taskInput, PathSearcherInput pathInput, MapGeneratorInput mapInput)
		{
			_taskInput = taskInput;
			_pathInput = pathInput;
			_mapInput = mapInput;
		}
		#endregion

		#region Thread management
		private static Mutex mut = new Mutex();
		public void Start()
		{
			_totalIterations = _taskInput.InnerIterations * _taskInput.OuterIterations;
			_totalFound = 0;
			_totalCounter = 0;
			_results = new Result[_taskInput.OuterIterations];
			for (int i = 0; i < _taskInput.OuterIterations; i++)
				_results[i] = new Result();
			_totalTime = 0 - DateTime.Now.Ticks; // Record current time
			_threads = new Thread[_taskInput.ThreadNum];
			for(int i = 0; i < _taskInput.ThreadNum; i++)
			{
				_threads[i] = new Thread(new ThreadStart(_doOne));
				_threads[i].Start();
			}
		}

		public void Abort()
		{
			for(int i = 0; i < _taskInput.ThreadNum; i++)
			{
				_threads[i].Abort();
			}
		}

		public void Pause()
		{
		}

		public bool Resume()
		{
			if (File.Exists("progress.txt"))
			{
				StreamReader checkFile = File.OpenText("progress.txt");
				if (checkFile.ReadLine() == "PATH_PROGRESS")
				{
					ReadProgress();
					_totalIterations = _taskInput.InnerIterations * _taskInput.OuterIterations;
					_totalFound = 0;
					_totalCounter = 0;
					foreach (Result result in _results)
					{
						_totalFound += result.Found;
						_totalCounter += result.Total;
					}
					_totalTime -= DateTime.Now.Ticks;
					_threads = new Thread[_taskInput.ThreadNum];
					for(int i = 0; i < _taskInput.ThreadNum; i++)
					{
						_threads[i] = new Thread(new ThreadStart(_doOne));
						_threads[i].Start();
					}
					return true;
				}
			}
			return false;
		}

		public void SaveProgress()
		{
			mut.WaitOne();
			StreamWriter streamWriterProgress = File.CreateText("progress.txt");
			streamWriterProgress.WriteLine("PATH_PROGRESS");
			streamWriterProgress.WriteLine(_mapInput.Width);
			streamWriterProgress.WriteLine(_mapInput.Height);
			streamWriterProgress.WriteLine(_mapInput.FillRate);
			streamWriterProgress.WriteLine(_mapInput.FillAlgorithm);
			streamWriterProgress.WriteLine(_mapInput.H);
			streamWriterProgress.WriteLine(_pathInput.SearchAlgorithm);
			streamWriterProgress.WriteLine(_pathInput.SearchType);
			streamWriterProgress.WriteLine(_pathInput.Jump);
			streamWriterProgress.WriteLine(_taskInput.OuterIterations);
			streamWriterProgress.WriteLine(_taskInput.InnerIterations);
			streamWriterProgress.WriteLine(_taskInput.DisplayImage);
			streamWriterProgress.WriteLine(_taskInput.ThreadNum);
			streamWriterProgress.WriteLine(_taskInput.SaveProgress);

			streamWriterProgress.WriteLine(_totalTime + DateTime.Now.Ticks);
			foreach (Result result in _results)
			{
				streamWriterProgress.WriteLine(result.Found);
				streamWriterProgress.WriteLine(result.Total);
			}
			streamWriterProgress.Close();
			mut.ReleaseMutex();
		}

		public void ReadProgress()
		{
			StreamReader streamReaderProgress = File.OpenText("progress.txt");
			string header = streamReaderProgress.ReadLine();
			_mapInput = new MapGeneratorInput();
			_mapInput.Width = Convert.ToInt32(streamReaderProgress.ReadLine());
			_mapInput.Height = Convert.ToInt32(streamReaderProgress.ReadLine());
			_mapInput.FillRate = Convert.ToDouble(streamReaderProgress.ReadLine());
			_mapInput.FillAlgorithm = Convert.ToInt32(streamReaderProgress.ReadLine());
			_mapInput.H = Convert.ToDouble(streamReaderProgress.ReadLine());
			_pathInput = new PathSearcherInput();
			_pathInput.SearchAlgorithm = Convert.ToInt32(streamReaderProgress.ReadLine());
			_pathInput.SearchType = Convert.ToInt32(streamReaderProgress.ReadLine());
			_pathInput.Jump = Convert.ToInt32(streamReaderProgress.ReadLine());
			_taskInput = new SearchTaskInput();
			_taskInput.OuterIterations = Convert.ToInt32(streamReaderProgress.ReadLine());
			_taskInput.InnerIterations = Convert.ToInt32(streamReaderProgress.ReadLine());
			_taskInput.DisplayImage = Convert.ToBoolean(streamReaderProgress.ReadLine());
			_taskInput.ThreadNum = Convert.ToInt32(streamReaderProgress.ReadLine());
			_taskInput.SaveProgress = Convert.ToBoolean(streamReaderProgress.ReadLine());
			
			_totalTime = Convert.ToInt64(streamReaderProgress.ReadLine());
			_results = new Result[_taskInput.OuterIterations];
			for (int i = 0; i < _taskInput.OuterIterations; i++)
			{
				_results[i] = new Result();
				_results[i].Found = Convert.ToInt32(streamReaderProgress.ReadLine());
				_results[i].Total = Convert.ToInt32(streamReaderProgress.ReadLine());
			}
			streamReaderProgress.Close();
		}
		#endregion

		private void _doOne()
		{
            int currentTaskNum = 0;
            PathSearcher pathSearcher = new PathSearcher(_pathInput, _mapInput);
			
			while (true)
			{
				mut.WaitOne();
				// Get task number
				if (_totalCounter < _totalIterations)
				{
					currentTaskNum = _totalCounter++;
					mut.ReleaseMutex();
				}
				else
				{
					mut.ReleaseMutex();
                    //MessageBox.Show("Thread Finished", "Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    pathSearcher = null;
					return;
                }
				// Run search once
				if (pathSearcher.RunOne())
                {
                    mut.WaitOne();
					_results[currentTaskNum / _taskInput.InnerIterations].Found++;
                    _totalFound++;
                    mut.ReleaseMutex();
                }
                mut.WaitOne();
                _results[currentTaskNum / _taskInput.InnerIterations].Total++;
                mut.ReleaseMutex();
            }
		}
	}
}
