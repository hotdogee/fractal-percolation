using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace MapGen
{
	/// <Features>
	/// * Map Generation
	/// * Image Generation
    /// * Multithreaded Path Search
    /// </Features>
    /// <TODO>
    /// - Improve UI
    /// </TODO>
	/// <ChangeLog>
	/// 0.2
	/// - Option to set border width and cell size
	/// 0.3
	/// - New spectral synthesis fractal Brownian motion map generation algorithm
	/// 0.3a
	/// - Remembers window size
	/// 0.3b
	/// - Fixed non square fractal maps
    /// 0.8
    /// - Fixed Current Window not working after closing a FormDisplay window
    /// - Added Output Window
    /// - Use FFTW library instead of Dew.Math, run times are 2 times faster
    /// - Disable threading when using fractal fill algorithm
    /// - Abort unfinished threads when closing the window
    /// 0.8a
    /// - Fixed "Fixed Count" map gen algorithm
    /// 0.8b
    /// - Code clean up
    /// - Tweaked FormControl controls to look better in 120dpi
    /// 0.8c
    /// - Removed "%" from FormSearchResults output
    /// 0.8d
    /// - Output modifications
    /// 0.8e
    /// - Recompiled fftwlib to support multithreading
    /// - Fixed crashes when running more then one fractal search
    /// - Reenabled threading in fractal fill algorithm
    /// 0.8f
    /// - Output file name changed to .csv
    /// - Output file string reorganized
    /// - FormSearchResults added estimated run time
	/// </ChangeLog>
	public class FormMapGen : System.Windows.Forms.Form
	{
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItemWindow;

		// Variables
        public FormControl formControl;
        private IContainer components;

		public FormMapGen()
		{
			// Windows Form �]�p�u��䴩�����n��\
			InitializeComponent();
			// Display control panal
			formControl = new FormControl();
			formControl.MdiParent = this;
			formControl.Show();
			/*
			FormDebug formDebug = new FormDebug();
			formDebug.MdiParent = this;
			formDebug.Show();
			*/
		}

		/// <summary>
		/// �M������ϥΤ����귽�C
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form �]�p�u�㲣�ͪ��{���X
		/// <summary>
		/// �����]�p�u��䴩�ҥ�������k - �ФŨϥε{���X�s�边�ק�
		/// �o�Ӥ�k�����e�C
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItemWindow = new System.Windows.Forms.MenuItem();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemWindow});
            // 
            // menuItemWindow
            // 
            this.menuItemWindow.Index = 0;
            this.menuItemWindow.MdiList = true;
            this.menuItemWindow.Text = "Window";
            // 
            // FormMapGen
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 15);
            this.ClientSize = new System.Drawing.Size(892, 850);
            this.IsMdiContainer = true;
            this.Menu = this.mainMenu1;
            this.Name = "FormMapGen";
            this.Text = "Path 0.8h";
            this.Load += new System.EventHandler(this.FormMapGen_Load);
            this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// ���ε{�����D�i�J�I�C
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.EnableVisualStyles();
			Application.Run(new FormMapGen());
		}

		private void FormMapGen_Load(object sender, System.EventArgs e)
		{
			
			// Display Direct3D window
			//FormDirectX formDirectX = new FormDirectX();
			//formDirectX.Show();
			//formDirectX.Location = new Point(this.Location.X + this.Size.Width + 15, this.Location.Y);
			
		}
	}
}
