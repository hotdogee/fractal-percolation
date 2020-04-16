using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO; // file manipulation
using System.Drawing.Imaging; // pixel format
using System.Text.RegularExpressions; // Regex pattern matching

namespace MapGen
{
	/// <summary>
	/// FormDisplay 的摘要描述。
	/// </summary>
	public class FormDisplay : System.Windows.Forms.Form
	{
		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.PictureBox pictureBox;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;

		// variables
		private GifGenerator _gifGen;
		private MapGeneratorInput _mapInput;
		private ImageFileSettings _fileSettings;
		private string _fileName;
        private bool _autoSaved;
        private IContainer components;

		public FormDisplay(GifGenerator gifGen, MapGeneratorInput mapInput, ImageFileSettings fileSettings)
		{
			// Windows Form 設計工具支援的必要項
			InitializeComponent();
			//Initialize variables
			GifGen = gifGen;
			MapInput = mapInput;
			FileSettings = fileSettings;
			// Show bitmap
			Display();
		}
		
		#region Properties
		public string FileName
		{
			get { return _fileName; }
			set
			{
				if (_fileName != value)
				{
					_fileName = value;
					this.Text = _fileName;
				}
			}
		}
		public bool AutoSaved
		{
			get { return _autoSaved; }
			set
			{
				if (_autoSaved != value)
				{
					_autoSaved = value;
					if (value == true)
						menuItem1.Enabled = false;
					else
						menuItem1.Enabled = true;
				}
			}
		}
		public GifGenerator GifGen
		{
			get { return _gifGen; }
			set { _gifGen = value; }
		}
		public MapGeneratorInput MapInput
		{
			get { return _mapInput; }
			set { _mapInput = value; }
		}
		public ImageFileSettings FileSettings
		{
			get { return _fileSettings; }
			set { _fileSettings = value; }
		}
		#endregion
		
		public void Display()
		{
			// Show bitmap
			pictureBox.Image = GifGen.Image;
			reposition();
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
            this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.menuItem2});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.Text = "Save";
            this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 1;
            this.menuItem2.Text = "Save As...";
            this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
            // 
            // pictureBox
            // 
            this.pictureBox.BackColor = System.Drawing.SystemColors.Control;
            this.pictureBox.Location = new System.Drawing.Point(0, 0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(200, 200);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "gif";
            this.saveFileDialog.Filter = "GIF Image|*.gif";
            // 
            // FormDisplay
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 15);
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(592, 550);
            this.Controls.Add(this.pictureBox);
            this.Menu = this.mainMenu;
            this.Name = "FormDisplay";
            this.Text = "Unsaved Image";
            this.Activated += new System.EventHandler(this.FormDisplay_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormDisplay_FormClosing);
            this.Resize += new System.EventHandler(this.FormDisplay_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		// Save
		private void menuItem1_Click(object sender, System.EventArgs e)
		{
			// Set title and save
			FileName = ImageFile.Save(GifGen.Image, MapInput, FileSettings);
			// Disable save button
			AutoSaved = true;
		}

		// Save As
		private void menuItem2_Click(object sender, System.EventArgs e)
		{	
			// Set title and save
			FileName = ImageFile.SaveAs(GifGen.Image, MapInput, FileSettings);
		}

		private void FormDisplay_Resize(object sender, System.EventArgs e)
        {
            if (this.MdiParent != null)
			    ((FormControl)(this.MdiParent.MdiChildren[0])).DisplaySize = this.Size;
			reposition();
		}

		private void reposition()
		{
			int x = 0;
			int y = 0;
			if (pictureBox.Size.Width < (this.Size.Width - 8))
				x = (this.Size.Width - 8 - pictureBox.Size.Width) / 2;
			if (pictureBox.Size.Height < (this.Size.Height - 30))
				y = (this.Size.Height - 30 - pictureBox.Size.Height) / 2;
			pictureBox.SetBounds(x, y, pictureBox.Size.Width, pictureBox.Size.Height);
		}

		private void FormDisplay_Activated(object sender, System.EventArgs e)
		{
            if (this.MdiParent != null)
			    ((FormControl)(this.MdiParent.MdiChildren[0])).CurrentDisplay = this;
		}

        private void FormDisplay_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.MdiParent != null)
                ((FormControl)(this.MdiParent.MdiChildren[0])).CurrentDisplay = null;
        }
	}
}
