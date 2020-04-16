using System;
using System.Drawing;
using System.Drawing.Imaging;
using fftwlib;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;

namespace MapGen
{
	// 
	// Gaussian Random Number Generator class
	// ref. ``Numerical Recipes in C++ 2/e'', p.293 ~ p.294
	// Returns N(0, 1) 
	public class GaussianRNG
	{
		private int iset;
		private double gset;
		private Random r1, r2;
    
		public GaussianRNG()
		{
			r1 = new Random(unchecked((int)DateTime.Now.Ticks));
			r2 = new Random(~unchecked((int)DateTime.Now.Ticks));
			iset = 0;
		}
    
		public double Next()
		{
			double fac, rsq, v1, v2;    
			if (iset == 0) 
			{
				do 
				{
					v1 = 2.0 * r1.NextDouble() - 1.0;
					v2 = 2.0 * r2.NextDouble() - 1.0;
					rsq = v1 * v1 + v2 * v2;
				} while (rsq >= 1.0 || rsq == 0.0);
        
				fac = Math.Sqrt(-2.0 * Math.Log(rsq) / rsq);
				gset = v1 * fac;
				iset = 1;
				return v2 * fac;
			} 
			else 
			{
				iset = 0;
				return gset;
			}
		}
	}
    /// <summary>
    /// The first method is to assign real or imaginary (random) height data to a convenient lattice. Then, an iteration process takes place 
    /// (using fractal Brownian noise -- fBM) to define the heightfields between the primary lattice. The iterative process is known generally 
    /// as the midpoint displacement method. The roughness of the terrain is controlled by the fractal dimension (or Hurst exponent) assigned 
    /// to the iterative process. Once the data points are created via iteration, one can create polygons and render a landscape. 
    /// 
    /// The second method is to use fBM noise to generate a matrix of phase and amplitude data (complex numbers) in the frequency domain 
    /// associated with Fourier methodology, then perform an inverse Fast Fourier Transform (FFT) to create a heightfield in the time (or space) 
    /// domain of the Euclidean world. The transformed matrix is rendered after conversion to an array of polygons. This method is sometimes 
    /// called Spectral Synthesis. Spectral Synthesis provides a much more realistic landscape but is MUCH more computationally intensive. 
    /// Furthermore, the method (at least to the limit of my understanding at this point) cannot iterate between points. The whole batch is 
    /// done at once. 
    /// </summary>
	public class FractionalBrownianMotion
	{
		private double _h = 0.5; // 0 < H < 1, D = 2 - H
		private byte[][] _byteMap; // 2D output array, values = 0 - 255
		private double[][] _realMap; // 2D output array, original real values
		private GaussianRNG _guass; // Gaussian RNG
		private Random _rand; // Uniform RNG
		private int _width = 100; // Array size
		private int _height = 100; // Array size

		#region Properties
		public int Width
		{
			get { return _width; }
			set { _width = value > 1 ? value : 100; }
		}
		public int Height
		{
			get { return _height; }
			set { _height = value > 1 ? value : 100; }
		}
		public double H
		{
			get { return _h; }
			set { _h = value; }
		}
		public byte[][] ByteMap
		{
			get { return _byteMap; }
		}
		public double[][] RealMap
		{
			get { return _realMap; }
		}
		#endregion

		#region Constructor
		public FractionalBrownianMotion()
		{
			_guass = new GaussianRNG();
			_rand = new Random(unchecked((int)DateTime.Now.Ticks));
		}
		public FractionalBrownianMotion(int width, int height)
		{
			Width = width;
            Height = height;
            _byteMap = new byte[width][];
            _realMap = new double[width][];
            for (int i = 0; i < width; i++)
            {
                _byteMap[i] = new byte[height];
                _realMap[i] = new double[height];
            }
			_guass = new GaussianRNG();
			_rand = new Random(unchecked((int)DateTime.Now.Ticks));
		}
		public FractionalBrownianMotion(int width, int height, double h)
		{
			Width = width;
			Height = height;
            _h = h;
            _byteMap = new byte[width][];
            _realMap = new double[width][];
            for (int i = 0; i < width; i++)
            {
                _byteMap[i] = new byte[height];
                _realMap[i] = new double[height];
            }
			_guass = new GaussianRNG();
			_rand = new Random(unchecked((int)DateTime.Now.Ticks));
		}
		#endregion

		#region Settings
		public void SetProperties(int width, int height)
		{
			Width = width;
			Height = height;
            _byteMap = new byte[width][];
            _realMap = new double[width][];
            for (int i = 0; i < width; i++)
            {
                _byteMap[i] = new byte[height];
                _realMap[i] = new double[height];
            }
		}
		public void SetProperties(int width, int height, double h)
		{
			Width = width;
			Height = height;
			_h = h;
            _byteMap = new byte[width][];
            _realMap = new double[width][];
            for (int i = 0; i < width; i++)
            {
                _byteMap[i] = new byte[height];
                _realMap[i] = new double[height];
            }
		}
		#endregion

		public void SpectralSynthesis2D()
		{
            _spectralSynthesis2D_FFTW();
		}

		public void SpectralSynthesis2D(int width, int height)
		{
			Width = width;
			Height = height;
			_byteMap = new byte[width][];
            _realMap = new double[width][];
            for (int i = 0; i < width; i++)
            {
                _byteMap[i] = new byte[height];
                _realMap[i] = new double[height];
            }
            _spectralSynthesis2D_FFTW();
		}

		public void SpectralSynthesis2D(int width, int height, double h)
		{
			Width = width;
			Height = height;
            _h = h;
            _byteMap = new byte[width][];
            _realMap = new double[width][];
            for (int i = 0; i < width; i++)
            {
                _byteMap[i] = new byte[height];
                _realMap[i] = new double[height];
            }
            _spectralSynthesis2D_FFTW();
        }
        /// <summary>
        /// Spectrally  synthesised	fractal  motion in two
        /// dimensions.  This algorithm is given under  the
        /// name   SpectralSynthesisFM2D  on  page  108  of
        /// Peitgen & Saupe. */
        /// </summary>
        private void _spectralSynthesis2D_FFTW()
        {
            double rad, phase;
            long i0, j0;
            int halfWidth = Convert.ToInt32(Math.Ceiling((double)_width / 2));
            int halfHeight = Convert.ToInt32(Math.Ceiling((double)_height / 2));

            // create two unmanaged arrays, properly aligned
            long size = (long)2 * _height * _width;
            IntPtr pin = fftw.malloc(new IntPtr(size * sizeof(double)));
            IntPtr pout = fftw.malloc(new IntPtr(size * sizeof(double)));
            GC.AddMemoryPressure(size * sizeof(double) * 2);

            // managed arrays
            // n*2 because we are dealing with complex numbers
            double[] fin = new double[size];
            double[] fout = new double[size];

            // create plan
            IntPtr plan = fftw.dft_2d(_height, _width, pin, pout, fftw_direction.Backward, fftw_flags.Estimate);

            // initialize data
            //Parallel.For(0, halfHeight + 1, i =>
            for (long i = 0; i <= halfHeight; i++)
            {
                for (long j = 0; j <= halfWidth; j++)
                {
                    phase = 2 * Math.PI * _rand.NextDouble();
                    if ((i != 0) || (j != 0))
                        rad = Math.Pow(i * i + j * j, -(_h + 1) / 2) * _guass.Next();
                    else
                        rad = 0;
                    fin[(i * _width + j) * 2] = rad * Math.Cos(phase);
                    fin[(i * _width + j) * 2 + 1] = rad * Math.Sin(phase);

                    if (i == 0)
                        i0 = 0;
                    else
                        i0 = _height - i;
                    if (j == 0)
                        j0 = 0;
                    else
                        j0 = _width - j;
                    fin[(i0 * _width + j0) * 2] = rad * Math.Cos(phase);
                    fin[(i0 * _width + j0) * 2 + 1] = -rad * Math.Sin(phase);
                }
            }
            //}); // Parallel.For
            fin[halfHeight * _width * 2 + 1] = 0;
            fin[halfWidth * 2 + 1] = 0;
            fin[(halfHeight * _width + halfWidth) * 2 + 1] = 0;
            
            //Parallel.For(1, halfHeight, i =>
            for (long i = 1; i < halfHeight; i++)
            {
                for (long j = 1; j < halfWidth; j++)
                {
                    phase = 2 * Math.PI * _rand.NextDouble();
                    rad = Math.Pow(i * i + j * j, -(_h + 1) / 2) * _guass.Next();
                    fin[(i * _width + _width - j) * 2] = rad * Math.Cos(phase);
                    fin[(i * _width + _width - j) * 2 + 1] = rad * Math.Sin(phase);
                    fin[((_height - i) * _width + j) * 2] = rad * Math.Cos(phase);
                    fin[((_height - i) * _width + j) * 2 + 1] = -rad * Math.Sin(phase);
                }
            }
            //}); // Parallel.For

            //copy managed arrays to unmanaged arrays
            Marshal.Copy(fin, 0, pin, _height * _width * 2);
            //Marshal.Copy(fout, 0, pout, _height * _width * 2);

            // execute IFFT
            fftw.execute(plan);

            //copy unmanaged array to managed array
            Marshal.Copy(pout, fout, 0, _height * _width * 2);

            // Find largest real
            double max = 0;
            for (long i = 0; i < _height; i++)
            {
                for (long j = 1; j < _width; j++)
                {
                    if (fout[(i * _width + j) * 2] > max)
                    {
                        max = fout[(i * _width + j) * 2];
                    }
                }
            }
            // Find smallest real
            double min = max;
            for (long i = 0; i < _height; i++)
            {
                for (long j = 1; j < _width; j++)
                {
                    if (fout[(i * _width + j) * 2] < min)
                    {
                        min = fout[(i * _width + j) * 2];
                    }
                }
            }
            // Convert to 2D byte array
            //Parallel.For(0, _height, i =>
            for (long i = 0; i < _height; i++)
            {
                for (long j = 0; j < _width; j++)
                {
                    _byteMap[j][i] = Convert.ToByte(Math.Min(255, Math.Max(0, (int)(256 * (fout[(i * _width + j) * 2] - min) / (max - min)))));
                    _realMap[j][i] = fout[(i * _width + j) * 2];
                }
            }
            //}); // Parallel.For
            // destroy plan
            // free memery
            fftw.free(pin);
            fftw.free(pout);
            GC.RemoveMemoryPressure(size * sizeof(double) * 2);
            fftw.destroy_plan(plan);
        }
	}
}
