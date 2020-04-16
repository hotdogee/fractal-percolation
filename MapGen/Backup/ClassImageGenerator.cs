using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO; // file manipulation
using System.Drawing.Imaging; // pixel format
using System.Text.RegularExpressions; // Regex pattern matching

// Private class variables are lower case and begin with an underscore
// Private method variables are lower case without underscore
// Public variables are capitalized
namespace MapGen
{
	public class ImageGeneratorInput
	{
		private Color[] _byteToColor;
		private int _cellSize = 1; // width = height in pixels
		private int _borderSize = 0; // size in pixels
		private byte _borderIndex = 2; // matches the border color index in _byteToColor aray
		
		#region Properties
		public Color[] ByteToColor
		{
			get {return _byteToColor;}
			set { _byteToColor = value; }
		}
		public int CellSize
		{
			get {return _cellSize;}
			set { _cellSize = value > 0 ? value : 1; }
		}
		public int BorderSize
		{
			get {return _borderSize;}
			set { _borderSize = value >= 0 ? value : 0; }
		}
		public byte BorderIndex
		{
			get {return _borderIndex;}
			set { _borderIndex = value >= 0 ? (value <= 255 ? value : (byte)255) : (byte)0; }
		}
		#endregion
		
		#region Constructor
		public ImageGeneratorInput() : this(new Color[3], 1, 0, 2)
		{
		}
		public ImageGeneratorInput(Color[] byteToColor) : this(byteToColor, 1, 0, 2)
		{
		}
		public ImageGeneratorInput(Color[] byteToColor, int cellSize) : this(byteToColor, cellSize, 0, 2)
		{
		}
		public ImageGeneratorInput(Color[] byteToColor, int cellSize, int borderSize) : this(byteToColor, cellSize, borderSize, 2)
		{
		}
		public ImageGeneratorInput(Color[] byteToColor, int cellSize, int borderSize, byte borderIndex)
		{
			ByteToColor = byteToColor;
			CellSize = cellSize;
			BorderSize = borderSize;
			BorderIndex = borderIndex;
		}
		#endregion
	}
	/// <summary>
	/// Generates gif images from byte arrays
	/// </summary>
	public class GifGenerator
	{
		public Bitmap _image;
		private byte[,] _byteMap;
		// Image settings
		private ImageGeneratorInput _input;

		#region Properties
		public ImageGeneratorInput Input
		{
			get {return _input;}
			set { _input = value; }
		}
		public byte[,] ByteMap
		{
			get { return _byteMap; }
			set { _byteMap = value; }
		}
		public Bitmap Image
		{
			get { return _image; }
		}
		#endregion

		#region Constructor
		public GifGenerator()
		{
			_input = new ImageGeneratorInput();
		}
		public GifGenerator(byte[,] byteMap)
		{
			_byteMap = byteMap;
			_input = new ImageGeneratorInput();
		}
		public GifGenerator(byte[,] byteMap, Color[] byteToColor)
		{
			_byteMap = byteMap;
			_input = new ImageGeneratorInput(byteToColor);
		}
		public GifGenerator(byte[,] byteMap, Color[] byteToColor, int cellSize, int borderSize)
		{
			_byteMap = byteMap;
			_input = new ImageGeneratorInput(byteToColor, cellSize, borderSize);
		}
		public GifGenerator(byte[,] byteMap, Color[] byteToColor, int cellSize, int borderSize, byte borderIndex)
		{
			_byteMap = byteMap;
			_input = new ImageGeneratorInput(byteToColor, cellSize, borderSize, borderIndex);
		}
		public GifGenerator(byte[,] byteMap, ImageGeneratorInput input)
		{
			_byteMap = byteMap;
			_input = input;
		}
		#endregion
		
		#region Destructor
		~GifGenerator()
		{
			_byteMap = null;
		}
		#endregion

		#region Settings
		public void SetProperties(Color[] byteToColor)
		{
			_input.ByteToColor = byteToColor;
		}
		public void SetProperties(Color[] byteToColor, int cellSize, int borderSize)
		{
			_input.ByteToColor = byteToColor;
			_input.CellSize = cellSize;
			_input.BorderSize = borderSize;
		}
		public void SetProperties(Color[] byteToColor, int cellSize, int borderSize, byte borderIndex)
		{
			_input.ByteToColor = byteToColor;
			_input.CellSize = cellSize;
			_input.BorderSize = borderSize;
			_input.BorderIndex = borderIndex;
		}
		public void SetProperties(int cellSize, int borderSize)
		{
			_input.CellSize = cellSize;
			_input.BorderSize = borderSize;
		}
		public void SetProperties(int cellSize, int borderSize, byte borderIndex)
		{
			_input.CellSize = cellSize;
			_input.BorderSize = borderSize;
			_input.BorderIndex = borderIndex;
		}
		#endregion

		public void Save(string fileName)
		{
			_image.Save(fileName, ImageFormat.Gif);
		}

		private ColorPalette _generatePalette(Color[] byteToColor)
		{
			Bitmap tempbmp;
			if (byteToColor.Length <= 2)
				tempbmp = new Bitmap(1, 1, PixelFormat.Format1bppIndexed);
			else if (byteToColor.Length <= 16)
				tempbmp = new Bitmap(1, 1, PixelFormat.Format4bppIndexed);
			else
				tempbmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed);
			ColorPalette palette = tempbmp.Palette;
			tempbmp.Dispose();
			for (int i = 0; i < byteToColor.Length; i++)
				palette.Entries[i] = byteToColor[i];
			return palette;
		}

		public Bitmap Generate()
		{
			_generate();
			return _image;
		}
		public Bitmap Generate(Color[] byteToColor)
		{
			SetProperties(byteToColor);
			_generate();
			return _image;
		}
		public Bitmap Generate(Color[] byteToColor, int cellSize, int borderSize)
		{
			SetProperties(byteToColor, cellSize, borderSize);
			_generate();
			return _image;
		}
		public Bitmap Generate(Color[] byteToColor, int cellSize, int borderSize, byte borderIndex)
		{
			SetProperties(byteToColor, cellSize, borderSize, borderIndex);
			_generate();
			return _image;
		}
		public Bitmap Generate(byte[,] byteMap, Color[] byteToColor)
		{
			_byteMap = byteMap;
			SetProperties(byteToColor);
			_generate();
			return _image;
		}
		public Bitmap Generate(byte[,] byteMap, Color[] byteToColor, int cellSize, int borderSize)
		{
			_byteMap = byteMap;
			SetProperties(byteToColor, cellSize, borderSize);
			_generate();
			return _image;
		}
		public Bitmap Generate(byte[,] byteMap, Color[] byteToColor, int cellSize, int borderSize, byte borderIndex)
		{
			_byteMap = byteMap;
			SetProperties(byteToColor, cellSize, borderSize, borderIndex);
			_generate();
			return _image;
		}
		private void _generate()
		{
			// Calculate image _width and _height
			int width = Convert.ToInt32(_byteMap.GetLength(0) * (_input.BorderSize + _input.CellSize) + _input.BorderSize);
			int height = Convert.ToInt32(_byteMap.GetLength(1) * (_input.BorderSize + _input.CellSize) + _input.BorderSize);
			// new Bitmap
			_image = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
			_image.Palette = _generatePalette(_input.ByteToColor);
			// Get BitmapData
			BitmapData imageData = _image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
			unsafe
			{
				// Get pointer to first row
				byte* pSourceRow = (byte *)imageData.Scan0.ToPointer();
				for (int y = 0; y < height; y++)
				{
					byte* row = pSourceRow + (y * imageData.Stride);
					for (int x = 0; x < width; x++)
					{
						if (((y % (_input.CellSize + _input.BorderSize)) < _input.BorderSize) || ((x % (_input.CellSize + _input.BorderSize)) < _input.BorderSize))
							row[x] = _input.BorderIndex;
						else
                            row[x] = _byteMap[(int)Math.Floor((double)x / (_input.CellSize + _input.BorderSize)), (int)Math.Floor((double)y / (_input.CellSize + _input.BorderSize))];
					}
				}
			} // end unsafe

			// Unlock bits
			_image.UnlockBits(imageData);
		}
	}
	public class ImageFileSettings
	{
		private string _fileNamePrefix = "Map";
		private string _extention = "gif";
		private string _filter = "GIF Image|*.gif";
		private ImageFormat _format = ImageFormat.Gif;

		#region Properties
		public string FileNamePrefix
		{
			get {return _fileNamePrefix;}
			set { _fileNamePrefix = value; }
		}
		public string Extention
		{
			get {return _extention;}
			set { _extention = value; }
		}
		public string Filter
		{
			get {return _filter;}
			set { _filter = value; }
		}
		public ImageFormat Format
		{
			get {return _format;}
			set { _format = value; }
		}
		#endregion
		
		#region Constructor
		public ImageFileSettings(string fileNamePrefix)
		{
			FileNamePrefix = fileNamePrefix;
		}
		public ImageFileSettings(string fileNamePrefix, string extention, string filter, ImageFormat format)
		{
			FileNamePrefix = fileNamePrefix;
			Extention = extention;
			Filter = filter;
			Format = format;
		}
		#endregion
	}
	public class ImageFile
	{
		public static string Save(Bitmap image, MapGeneratorInput mapInput, ImageFileSettings fileSettings)
		{
			string fileName = GenerateAutoSaveFileName(mapInput, fileSettings);
			image.Save(fileName, fileSettings.Format);
			return fileName;
		}
		public static string SaveAs(Bitmap image, MapGeneratorInput mapInput, ImageFileSettings fileSettings)
		{
			string fileName = GetFileNameFromSaveDialog(mapInput, fileSettings);
			image.Save(fileName, fileSettings.Format);
			return fileName;
		}
		public static string GetFileNameFromSaveDialog(MapGeneratorInput mapInput, ImageFileSettings fileSettings)
		{
			// Set default file name
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.DefaultExt = fileSettings.Extention;
			saveFileDialog.Filter = fileSettings.Filter;
			saveFileDialog.FileName = GenerateAutoSaveFileName(mapInput, fileSettings);
			if (saveFileDialog.ShowDialog() != DialogResult.Cancel)
			{
				if (saveFileDialog.FileName == "" || saveFileDialog.FileName == null)
				{
					MessageBox.Show("Invalid File Name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				else
				{
					return Regex.Match(saveFileDialog.FileName, @".+\\(\w+\.\w+)").Groups[1].ToString();
				}
			}
			return "";
		}

		// returns a file name with the specified prefix and auto increased number postfix as the filename
		public static string GenerateAutoSaveFileName(MapGeneratorInput mapInput, ImageFileSettings fileSettings)
		{
			// Get files that start with fileNamePrefix
			string[] fileList = Directory.GetFiles(Directory.GetCurrentDirectory(), fileSettings.FileNamePrefix + "*");
			// Filter out only the files matching prefix+"0000"
			// Get largest postfix number
			int postfixNum = -1;
			foreach (string file in fileList)
			{
				Match match = Regex.Match(file, fileSettings.FileNamePrefix + @"\[.+\].(\d\d\d\d).\w+");
				if (match.Success)
				{
					int num = Int32.Parse(match.Groups[1].ToString());
					if (num > postfixNum)
					{
						postfixNum = num;
					}
				}
			}
			postfixNum++;
			string postfix = postfixNum.ToString("0000");
			return fileSettings.FileNamePrefix + "[" + mapInput.FillRate.ToString("0.00") + " " + mapInput.FillAlgorithm + " " + mapInput.Width + " " + mapInput.Height + "]." + postfix + "." + fileSettings.Extention;
		}
	}
}
































