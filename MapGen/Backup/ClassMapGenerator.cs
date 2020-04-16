using System;
using System.Windows.Forms;

namespace MapGen
{
	public class MapGeneratorInput
	{
		private int _width = 1;
		private int _height = 1;
		private double _fillRate = 0;
		private int _fillAlgorithm = 0;
		private double _h = 0.5; // fractal
		
		#region Properties
		public int Width
		{
			get {return _width;}
			set { _width = value > 0 ? value : 1; }
		}
		public int Height
		{
			get { return _height; }
			set { _height = value > 0 ? value : 1; }
		}
		public double FillRate
		{
			get { return _fillRate; }
			set
			{
				if (value < 0)
					_fillRate = 0;
				else if (value > 1)
					_fillRate = 1;
				else
					_fillRate = value;
			}
		}
		public int FillAlgorithm
		{
			get { return _fillAlgorithm; }
			set { _fillAlgorithm = value; }
		}
		public double H
		{
			get { return _h; }
			set { _h = value; }
		}
		#endregion

		#region Constructor
		public MapGeneratorInput() : this(1, 1, 0, 0, 0.5)
		{
		}
		public MapGeneratorInput(int width, int height) : this(width, height, 0, 0, 0.5)
		{
		}
		public MapGeneratorInput(int width, int height, double fillRate) : this(width, height, fillRate, 0, 0.5)
		{
		}
		public MapGeneratorInput(int width, int height, double fillRate, int fillAlgorithm) : this(width, height, fillRate, fillAlgorithm, 0.5)
		{
		}
		public MapGeneratorInput(int width, int height, double fillRate, int fillAlgorithm, double h)
		{
			Width = width;
			Height = height;
			FillRate = fillRate;
			FillAlgorithm = fillAlgorithm;
			H = h;
		}
		#endregion
	}
	/// <summary>
	/// Outputs a 2D binary map, 1 = obstacle, 0 = pass
	/// </summary>
	public class MapGenerator
	{
		private byte[,] _byteMap;
        // MapGenerator properties
        private MapGeneratorInput _input;
        private Random _rand;
        private FractionalBrownianMotion fBm;

		#region Properties
		public MapGeneratorInput Input
		{
			get {return _input;}
			set
			{
				_input = value;
				_byteMap = new byte[value.Width, value.Height];
			}
		}
		public byte[,] ByteMap
		{
			get { return _byteMap; }
		}
		#endregion

		#region Constructor
		public MapGenerator()
		{
			_input = new MapGeneratorInput();
			_byteMap = new byte[_input.Width, _input.Height];
            _rand = new Random(unchecked((int)DateTime.Now.Ticks));
            fBm = new FractionalBrownianMotion(_input.Width, _input.Height, _input.H);
		}
		public MapGenerator(int width, int height)
		{
			_input = new MapGeneratorInput(width, height);
            _byteMap = new byte[_input.Width, _input.Height];
            _rand = new Random(unchecked((int)DateTime.Now.Ticks));
            fBm = new FractionalBrownianMotion(_input.Width, _input.Height, _input.H);
		}
		public MapGenerator(int width, int height, double fillRate)
		{
			_input = new MapGeneratorInput(width, height, fillRate);
            _byteMap = new byte[_input.Width, _input.Height];
            _rand = new Random(unchecked((int)DateTime.Now.Ticks));
            fBm = new FractionalBrownianMotion(_input.Width, _input.Height, _input.H);
		}
		public MapGenerator(int width, int height, double fillRate, int fillAlgorithm)
		{
			_input = new MapGeneratorInput(width, height, fillRate, fillAlgorithm);
            _byteMap = new byte[_input.Width, _input.Height];
            _rand = new Random(unchecked((int)DateTime.Now.Ticks));
            fBm = new FractionalBrownianMotion(_input.Width, _input.Height, _input.H);
		}
		public MapGenerator(int width, int height, double fillRate, int fillAlgorithm, double h)
		{
			_input = new MapGeneratorInput(width, height, fillRate, fillAlgorithm, h);
            _byteMap = new byte[_input.Width, _input.Height];
            _rand = new Random(unchecked((int)DateTime.Now.Ticks));
            fBm = new FractionalBrownianMotion(_input.Width, _input.Height, _input.H);
		}
		public MapGenerator(MapGeneratorInput input)
		{
			_input = input;
            _byteMap = new byte[_input.Width, _input.Height];
            _rand = new Random(unchecked((int)DateTime.Now.Ticks));
            fBm = new FractionalBrownianMotion(_input.Width, _input.Height, _input.H);
		}
		#endregion

		#region Destructor
		~MapGenerator()
		{
			_byteMap = null;
		}
		#endregion

		#region Settings
		public void SetProperties(int width, int height)
		{
			_input.Width = width;
			_input.Height = height;
			_byteMap = new byte[width, height];
		}
		public void SetProperties(int width, int height, double fillRate)
		{
			_input.Width = width;
			_input.Height = height;
			_input.FillRate = fillRate;
			_byteMap = new byte[width, height];
		}
		public void SetProperties(int width, int height, double fillRate, int fillAlgorithm)
		{
			_input.Width = width;
			_input.Height = height;
			_input.FillRate = fillRate;
			_input.FillAlgorithm = fillAlgorithm;
			_byteMap = new byte[width, height];
		}
		public void SetProperties(int width, int height, double fillRate, int fillAlgorithm, double h)
		{
			_input.Width = width;
			_input.Height = height;
			_input.FillRate = fillRate;
			_input.FillAlgorithm = fillAlgorithm;
			_input.H = h;
			_byteMap = new byte[width, height];
		}
		#endregion

		/// <summary>
		/// Calculate the expected number of good nodes according to fillRate, then random the positions those nodes so we have the same ammount of nodes every time.
		/// </summary>
		public byte[,] GenerateFixedCountMap()
		{
			_generateFixedCountMap();
			return _byteMap;
		}
		public byte[,] GenerateFixedCountMap(int width, int height)
		{
			_input.Width = width;
			_input.Height = height;
			_byteMap = new byte[_input.Width, _input.Height];
			_generateFixedCountMap();
			return _byteMap;
		}
		public byte[,] GenerateFixedCountMap(int width, int height, double fillRate)
		{
			_input.Width = width;
			_input.Height = height;
			_input.FillRate = fillRate;
			_byteMap = new byte[_input.Width, _input.Height];
			_generateFixedCountMap();
			return _byteMap;
		}
		public byte[,] GenerateFixedCountMap(MapGeneratorInput input)
		{
			_input = input;
			_byteMap = new byte[_input.Width, _input.Height];
			_generateFixedCountMap();
			return _byteMap;
		}
		private void _generateFixedCountMap()
		{
			// Initialize with 1 (1 = can't pass)
			for (int y = 0; y < _input.Height; y++)
			{
				for (int x = 0; x < _input.Width; x++)
				{
					_byteMap[x, y] = 1;
				}
			}
			//Random rand = new Random(unchecked((int)DateTime.Now.Ticks));
            Random rand = _rand;
			// calculate good node count
			int count = Convert.ToInt32(_input.Width * _input.Height * _input.FillRate);
			// place them in the _byteMap
			while (count > 0)
			{
                int x = Convert.ToInt32(Math.Floor(rand.NextDouble() * _input.Width));
                if (x == _input.Width)
                    x = _input.Width - 1;
                int y = Convert.ToInt32(Math.Floor(rand.NextDouble() * _input.Height));
                if (y == _input.Height)
                    y = _input.Height - 1;

				if (_byteMap[x, y] == 1)
				{
					_byteMap[x, y] = 0; // 0 = can pass
					count--;
				}
			}
		}
		/// <summary>
		/// Each node generates a random number and is compared to the fillRate, thus we will have a variable number of nodes with the same fillRate.
		/// </summary>
		public byte[,] GenerateRandomCountMap()
		{
			_generateRandomCountMap();
			return _byteMap;
		}
		public byte[,] GenerateRandomCountMap(int width, int height)
		{
			_input.Width = width;
			_input.Height = height;
			_byteMap = new byte[_input.Width, _input.Height];
			_generateRandomCountMap();
			return _byteMap;
		}
		public byte[,] GenerateRandomCountMap(int width, int height, double fillRate)
		{
			_input.Width = width;
			_input.Height = height;
			_input.FillRate = fillRate;
			_byteMap = new byte[_input.Width, _input.Height];
			_generateRandomCountMap();
			return _byteMap;
		}
		public byte[,] GenerateRandomCountMap(MapGeneratorInput input)
		{
			_input = input;
			_byteMap = new byte[_input.Width, _input.Height];
			_generateRandomCountMap();
			return _byteMap;
		}
		private void _generateRandomCountMap()
		{
            //Random rand = new Random(unchecked((int)DateTime.Now.Ticks));
            Random rand = _rand;
			for (int y = 0; y < _input.Height; y++)
			{
				for (int x = 0; x < _input.Width; x++)
				{
					if (rand.NextDouble() <= _input.FillRate) // 產生一個亂數與Fill比較
					{
						_byteMap[x, y] = 0;
					} 
					else 
					{
						_byteMap[x, y] = 1;
					}
				}
			}
		}
		
		/// <summary>
		/// Generates a 2D fractional Brownian motion map using spectal synthesis algorithm
		/// </summary>
		private double kthSmallest(double[] a, int k)
		{
			int i, j, l, m;
			double x, temp;

			l = 0;
			m = a.Length - 1;
			while (l < m) 
			{
				x = a[k];
				i = l;
				j = m;
				do 
				{
				while (a[i] < x)
					i++;
				while (x < a[j])
					j--;
					if (i <= j) 
					{
						temp = a[i];
						a[i] = a[j];
						a[j] = temp;
						i++;
						j--;
					}
				} while (i <= j);
				if (j < k)
					l = i;
				if (k < i)
					m = j;
			}
			return a[k];
		}
		
		public byte[,] GenerateSpectralSynthesisFractioalBrownianMotionMap()
		{
			_generateSpectralSynthesisFractioalBrownianMotionMap();
			return _byteMap;
		}
		public byte[,] GenerateSpectralSynthesisFractioalBrownianMotionMap(int width, int height)
		{
			_input.Width = width;
			_input.Height = height;
			_byteMap = new byte[_input.Width, _input.Height];
			_generateSpectralSynthesisFractioalBrownianMotionMap();
			return _byteMap;
		}
		public byte[,] GenerateSpectralSynthesisFractioalBrownianMotionMap(int width, int height, double fillRate)
		{
			_input.Width = width;
			_input.Height = height;
			_input.FillRate = fillRate;
			_byteMap = new byte[_input.Width, _input.Height];
			_generateSpectralSynthesisFractioalBrownianMotionMap();
			return _byteMap;
		}
		public byte[,] GenerateSpectralSynthesisFractioalBrownianMotionMap(int width, int height, double fillRate, double h)
		{
			_input.Width = width;
			_input.Height = height;
			_input.FillRate = fillRate;
			_input.H = h;
			_byteMap = new byte[_input.Width, _input.Height];
			_generateSpectralSynthesisFractioalBrownianMotionMap();
			return _byteMap;
		}
		public byte[,] GenerateSpectralSynthesisFractioalBrownianMotionMap(MapGeneratorInput input)
		{
			_input = input;
			_byteMap = new byte[_input.Width, _input.Height];
			_generateSpectralSynthesisFractioalBrownianMotionMap();
			return _byteMap;
		}
		private void _generateSpectralSynthesisFractioalBrownianMotionMap()
		{
            fBm.SpectralSynthesis2D();
			double[,] realMap = fBm.RealMap;
			// Build 1D array cells from heightMap
			double[] realArray = new double[_input.Width * _input.Height];
			for (int y = 0; y < _input.Height; y++)
				for (int x = 0; x < _input.Width; x++)
					realArray[y*_input.Width+x] = realMap[x, y];
			// Calculate good node count = k (black nodes)(0 nodes)
			int passNodeCount = Convert.ToInt32(_input.Width * _input.Height * _input.FillRate);
			// Find kth smallest item value
			double pivot = kthSmallest(realArray, passNodeCount);
			// Construct _byteMap
			for (int y = 0; y < _input.Height; y++)
			{
				for (int x = 0; x < _input.Width; x++)
				{
					if (fBm.RealMap[x, y] < pivot)
					{
						_byteMap[x, y] = 0;
					} 
					else 
					{
						_byteMap[x, y] = 1;
					}
				}
			}
		}

		/// <summary>
		/// _fillAlgorithm: 0 = Fixed Count, 1 = Random Count, 2 = Spectral Synthesis fBm
		/// </summary>
		public byte[,] Generate()
		{
			switch (Input.FillAlgorithm)
			{
				case 0:
					return GenerateFixedCountMap();
				case 1:
					return GenerateRandomCountMap();
				case 2:
					return GenerateSpectralSynthesisFractioalBrownianMotionMap();
			}
			return new byte[1,1];
		}
		public byte[,] Generate(int fillAlgorithm)
		{
			switch (fillAlgorithm)
			{
				case 0:
					return GenerateFixedCountMap();
				case 1:
					return GenerateRandomCountMap();
				case 2:
					return GenerateSpectralSynthesisFractioalBrownianMotionMap();
			}
			return new byte[1,1];
		}
		public byte[,] Generate(int fillAlgorithm, int width, int height)
		{
			switch (fillAlgorithm)
			{
				case 0:
					return GenerateFixedCountMap(width, height);
				case 1:
					return GenerateRandomCountMap(width, height);
				case 2:
					return GenerateSpectralSynthesisFractioalBrownianMotionMap(width, height);
			}
			return new byte[1,1];
		}
		public byte[,] Generate(int fillAlgorithm, int width, int height, double fillRate)
		{
			switch (fillAlgorithm)
			{
				case 0:
					return GenerateFixedCountMap(width, height, fillRate);
				case 1:
					return GenerateRandomCountMap(width, height, fillRate);
				case 2:
					return GenerateSpectralSynthesisFractioalBrownianMotionMap(width, height, fillRate);
			}
			return new byte[1,1];
		}
		public byte[,] Generate(int fillAlgorithm, int width, int height, double fillRate, double h)
		{
			switch (fillAlgorithm)
			{
				case 0:
					return GenerateFixedCountMap(width, height, fillRate);
				case 1:
					return GenerateRandomCountMap(width, height, fillRate);
				case 2:
					return GenerateSpectralSynthesisFractioalBrownianMotionMap(width, height, fillRate, h);
			}
			return new byte[1,1];
		}
		public byte[,] Generate(MapGeneratorInput input)
		{
			switch (input.FillAlgorithm)
			{
				case 0:
					return GenerateFixedCountMap(input);
				case 1:
					return GenerateRandomCountMap(input);
				case 2:
					return GenerateSpectralSynthesisFractioalBrownianMotionMap(input);
			}
			return new byte[1,1];
		}
	}
}
