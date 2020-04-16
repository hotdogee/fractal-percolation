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
        private MenuItem menuItem1;
        private MenuItem menuItem2;
        private IContainer components;

		public FormMapGen()
		{
			// Windows Form 設計工具支援的必要項
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
		/// 清除任何使用中的資源。
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

		#region Windows Form 設計工具產生的程式碼
		/// <summary>
		/// 此為設計工具支援所必須的方法 - 請勿使用程式碼編輯器修改
		/// 這個方法的內容。
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMapGen));
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItemWindow = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
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
            this.menuItemWindow.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.menuItem2});
            this.menuItemWindow.Text = "Window";
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.Text = "Save Matrix...";
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 1;
            this.menuItem2.Text = "Save Image...";
            // 
            // FormMapGen
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 15);
            this.ClientSize = new System.Drawing.Size(892, 755);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.Menu = this.mainMenu1;
            this.Name = "FormMapGen";
            this.Text = "Path 1.1b";
            this.Load += new System.EventHandler(this.FormMapGen_Load);
            this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// 應用程式的主進入點。
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
