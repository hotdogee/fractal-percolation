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
	/// FormControl 的摘要描述。
	/// </summary>
	public class FormControl : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label labelWidth;
		private System.Windows.Forms.Label labelHeight;
		private System.Windows.Forms.Label labelFill;
		private System.Windows.Forms.Label labelPercent;
		private System.Windows.Forms.Label labelAlgorithm;
		private System.Windows.Forms.Label labelImages;
		private System.Windows.Forms.Label labelAutoSave;
		private System.Windows.Forms.Label labelFileNamePrefix;
		private System.Windows.Forms.Label labelCellSize;
		private System.Windows.Forms.Label labelPassColor;
		private System.Windows.Forms.Label labelObstacleColor;
		private System.Windows.Forms.Label labelBorderColor;
		private System.Windows.Forms.Label labelBorder;
		private System.Windows.Forms.Label labelH;
		private System.Windows.Forms.Label colorPickerPass;
		private System.Windows.Forms.Label colorPickerObstacle;
		private System.Windows.Forms.Label colorPickerBorder;
		private System.Windows.Forms.Label labelDisplayWindow;
		private System.Windows.Forms.GroupBox groupBoxMap;
		private System.Windows.Forms.GroupBox groupBoxGenerate;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox textBoxWidth;
		private System.Windows.Forms.TextBox textBoxHeight;
		private System.Windows.Forms.TextBox textBoxFill;
		private System.Windows.Forms.TextBox textBoxFileNamePrefix;
		private System.Windows.Forms.TextBox textBoxImages;
		private System.Windows.Forms.TextBox textBoxCellSize;
		private System.Windows.Forms.TextBox textBoxBorder;
		private System.Windows.Forms.TextBox textBoxH;
		private System.Windows.Forms.ComboBox comboBoxFillAlgorithm;
		private System.Windows.Forms.ComboBox comboBoxAutoSave;
		private System.Windows.Forms.ComboBox comboBoxDisplayImage;
		private System.Windows.Forms.ColorDialog colorDialog;
		private System.Windows.Forms.Button buttonGenerate;
		
		private System.Windows.Forms.GroupBox groupBoxSearchPath;
		private System.Windows.Forms.GroupBox groupBoxSearchAlgorithm;
		private System.Windows.Forms.RadioButton radioButtonPriorityQueue;
		private System.Windows.Forms.RadioButton radioButtonDepthFirstSearch;
		private System.Windows.Forms.GroupBox groupBoxSearchType;
		private System.Windows.Forms.RadioButton radioButton4Way;
		private System.Windows.Forms.RadioButton radioButton8Way;
		private System.Windows.Forms.Label labelJump;
		private System.Windows.Forms.TextBox textBoxJump;

		private System.Windows.Forms.GroupBox groupBoxRunSearch;
		private System.Windows.Forms.Label labelInnerLoop;
		private System.Windows.Forms.TextBox textBoxInnerLoop;
		private System.Windows.Forms.Label labelOuterLoop;
		private System.Windows.Forms.TextBox textBoxOuterLoop;
		private System.Windows.Forms.Label labelThreads;
		private System.Windows.Forms.TextBox textBoxThreads;
		private System.Windows.Forms.CheckBox checkBoxDisplayImage;
		private System.Windows.Forms.CheckBox checkBoxSaveProgress;
		private System.Windows.Forms.Button buttonResetSearch;
        private System.Windows.Forms.Button buttonRunSearch;
        private IContainer components;
        private ToolTip toolTip1;

        public Form mdiParant;

		public FormControl()
		{
			// Windows Form 設計工具支援的必要項
			InitializeComponent();
			// Initialize DropDownLists default value
			comboBoxFillAlgorithm.SelectedIndex = 2;
			comboBoxAutoSave.SelectedIndex = 1;
			comboBoxDisplayImage.SelectedIndex = 2;
            mdiParant = this.MdiParent;
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
            this.textBoxWidth = new System.Windows.Forms.TextBox();
            this.textBoxHeight = new System.Windows.Forms.TextBox();
            this.labelWidth = new System.Windows.Forms.Label();
            this.labelHeight = new System.Windows.Forms.Label();
            this.labelFill = new System.Windows.Forms.Label();
            this.textBoxFill = new System.Windows.Forms.TextBox();
            this.labelPercent = new System.Windows.Forms.Label();
            this.comboBoxFillAlgorithm = new System.Windows.Forms.ComboBox();
            this.labelAlgorithm = new System.Windows.Forms.Label();
            this.groupBoxMap = new System.Windows.Forms.GroupBox();
            this.textBoxH = new System.Windows.Forms.TextBox();
            this.labelH = new System.Windows.Forms.Label();
            this.groupBoxGenerate = new System.Windows.Forms.GroupBox();
            this.labelImages = new System.Windows.Forms.Label();
            this.buttonGenerate = new System.Windows.Forms.Button();
            this.textBoxImages = new System.Windows.Forms.TextBox();
            this.comboBoxDisplayImage = new System.Windows.Forms.ComboBox();
            this.labelDisplayWindow = new System.Windows.Forms.Label();
            this.comboBoxAutoSave = new System.Windows.Forms.ComboBox();
            this.labelAutoSave = new System.Windows.Forms.Label();
            this.textBoxFileNamePrefix = new System.Windows.Forms.TextBox();
            this.labelFileNamePrefix = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxBorder = new System.Windows.Forms.TextBox();
            this.labelBorder = new System.Windows.Forms.Label();
            this.colorPickerBorder = new System.Windows.Forms.Label();
            this.labelBorderColor = new System.Windows.Forms.Label();
            this.colorPickerObstacle = new System.Windows.Forms.Label();
            this.labelObstacleColor = new System.Windows.Forms.Label();
            this.colorPickerPass = new System.Windows.Forms.Label();
            this.labelPassColor = new System.Windows.Forms.Label();
            this.textBoxCellSize = new System.Windows.Forms.TextBox();
            this.labelCellSize = new System.Windows.Forms.Label();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.groupBoxRunSearch = new System.Windows.Forms.GroupBox();
            this.labelThreads = new System.Windows.Forms.Label();
            this.textBoxThreads = new System.Windows.Forms.TextBox();
            this.checkBoxSaveProgress = new System.Windows.Forms.CheckBox();
            this.buttonRunSearch = new System.Windows.Forms.Button();
            this.checkBoxDisplayImage = new System.Windows.Forms.CheckBox();
            this.labelOuterLoop = new System.Windows.Forms.Label();
            this.textBoxOuterLoop = new System.Windows.Forms.TextBox();
            this.labelInnerLoop = new System.Windows.Forms.Label();
            this.textBoxInnerLoop = new System.Windows.Forms.TextBox();
            this.buttonResetSearch = new System.Windows.Forms.Button();
            this.groupBoxSearchPath = new System.Windows.Forms.GroupBox();
            this.groupBoxSearchType = new System.Windows.Forms.GroupBox();
            this.radioButton4Way = new System.Windows.Forms.RadioButton();
            this.radioButton8Way = new System.Windows.Forms.RadioButton();
            this.groupBoxSearchAlgorithm = new System.Windows.Forms.GroupBox();
            this.radioButtonPriorityQueue = new System.Windows.Forms.RadioButton();
            this.radioButtonDepthFirstSearch = new System.Windows.Forms.RadioButton();
            this.labelJump = new System.Windows.Forms.Label();
            this.textBoxJump = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBoxMap.SuspendLayout();
            this.groupBoxGenerate.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBoxRunSearch.SuspendLayout();
            this.groupBoxSearchPath.SuspendLayout();
            this.groupBoxSearchType.SuspendLayout();
            this.groupBoxSearchAlgorithm.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxWidth
            // 
            this.textBoxWidth.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxWidth.Location = new System.Drawing.Point(144, 16);
            this.textBoxWidth.MaxLength = 5;
            this.textBoxWidth.Name = "textBoxWidth";
            this.textBoxWidth.Size = new System.Drawing.Size(56, 21);
            this.textBoxWidth.TabIndex = 0;
            this.textBoxWidth.Text = "100";
            this.textBoxWidth.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxNumOnly_KeyPress);
            this.textBoxWidth.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxNumOnly_Validating);
            // 
            // textBoxHeight
            // 
            this.textBoxHeight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxHeight.Location = new System.Drawing.Point(144, 40);
            this.textBoxHeight.MaxLength = 5;
            this.textBoxHeight.Name = "textBoxHeight";
            this.textBoxHeight.Size = new System.Drawing.Size(56, 21);
            this.textBoxHeight.TabIndex = 1;
            this.textBoxHeight.Text = "100";
            this.textBoxHeight.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxNumOnly_KeyPress);
            this.textBoxHeight.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxNumOnly_Validating);
            // 
            // labelWidth
            // 
            this.labelWidth.Location = new System.Drawing.Point(8, 16);
            this.labelWidth.Name = "labelWidth";
            this.labelWidth.Size = new System.Drawing.Size(128, 21);
            this.labelWidth.TabIndex = 2;
            this.labelWidth.Text = "Width:";
            this.labelWidth.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelHeight
            // 
            this.labelHeight.Location = new System.Drawing.Point(8, 40);
            this.labelHeight.Name = "labelHeight";
            this.labelHeight.Size = new System.Drawing.Size(128, 21);
            this.labelHeight.TabIndex = 3;
            this.labelHeight.Text = "Height:";
            this.labelHeight.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelFill
            // 
            this.labelFill.Location = new System.Drawing.Point(8, 64);
            this.labelFill.Name = "labelFill";
            this.labelFill.Size = new System.Drawing.Size(128, 21);
            this.labelFill.TabIndex = 5;
            this.labelFill.Text = "Fill:";
            this.labelFill.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxFill
            // 
            this.textBoxFill.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxFill.Location = new System.Drawing.Point(144, 64);
            this.textBoxFill.MaxLength = 10;
            this.textBoxFill.Name = "textBoxFill";
            this.textBoxFill.Size = new System.Drawing.Size(56, 21);
            this.textBoxFill.TabIndex = 2;
            this.textBoxFill.Text = "59.28";
            this.textBoxFill.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxFill_KeyPress);
            this.textBoxFill.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxFill_Validating);
            // 
            // labelPercent
            // 
            this.labelPercent.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPercent.Location = new System.Drawing.Point(200, 64);
            this.labelPercent.Name = "labelPercent";
            this.labelPercent.Size = new System.Drawing.Size(24, 21);
            this.labelPercent.TabIndex = 6;
            this.labelPercent.Text = "%";
            this.labelPercent.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // comboBoxFillAlgorithm
            // 
            this.comboBoxFillAlgorithm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFillAlgorithm.Items.AddRange(new object[] {
            "Fixed Count",
            "Random Count",
            "Fractal"});
            this.comboBoxFillAlgorithm.Location = new System.Drawing.Point(144, 88);
            this.comboBoxFillAlgorithm.MaxDropDownItems = 4;
            this.comboBoxFillAlgorithm.Name = "comboBoxFillAlgorithm";
            this.comboBoxFillAlgorithm.Size = new System.Drawing.Size(112, 23);
            this.comboBoxFillAlgorithm.TabIndex = 3;
            // 
            // labelAlgorithm
            // 
            this.labelAlgorithm.Location = new System.Drawing.Point(8, 88);
            this.labelAlgorithm.Name = "labelAlgorithm";
            this.labelAlgorithm.Size = new System.Drawing.Size(128, 21);
            this.labelAlgorithm.TabIndex = 16;
            this.labelAlgorithm.Text = "Algorithm:";
            this.labelAlgorithm.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBoxMap
            // 
            this.groupBoxMap.BackColor = System.Drawing.SystemColors.Control;
            this.groupBoxMap.Controls.Add(this.textBoxH);
            this.groupBoxMap.Controls.Add(this.labelH);
            this.groupBoxMap.Controls.Add(this.comboBoxFillAlgorithm);
            this.groupBoxMap.Controls.Add(this.labelAlgorithm);
            this.groupBoxMap.Controls.Add(this.textBoxHeight);
            this.groupBoxMap.Controls.Add(this.labelHeight);
            this.groupBoxMap.Controls.Add(this.textBoxWidth);
            this.groupBoxMap.Controls.Add(this.labelWidth);
            this.groupBoxMap.Controls.Add(this.labelPercent);
            this.groupBoxMap.Controls.Add(this.labelFill);
            this.groupBoxMap.Controls.Add(this.textBoxFill);
            this.groupBoxMap.Location = new System.Drawing.Point(8, 8);
            this.groupBoxMap.Name = "groupBoxMap";
            this.groupBoxMap.Size = new System.Drawing.Size(264, 144);
            this.groupBoxMap.TabIndex = 17;
            this.groupBoxMap.TabStop = false;
            this.groupBoxMap.Text = "Matrix";
            // 
            // textBoxH
            // 
            this.textBoxH.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxH.Location = new System.Drawing.Point(144, 112);
            this.textBoxH.MaxLength = 5;
            this.textBoxH.Name = "textBoxH";
            this.textBoxH.Size = new System.Drawing.Size(56, 21);
            this.textBoxH.TabIndex = 17;
            this.textBoxH.Text = "0.85";
            // 
            // labelH
            // 
            this.labelH.Location = new System.Drawing.Point(8, 112);
            this.labelH.Name = "labelH";
            this.labelH.Size = new System.Drawing.Size(128, 21);
            this.labelH.TabIndex = 18;
            this.labelH.Text = "H:";
            this.labelH.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBoxGenerate
            // 
            this.groupBoxGenerate.Controls.Add(this.labelImages);
            this.groupBoxGenerate.Controls.Add(this.buttonGenerate);
            this.groupBoxGenerate.Controls.Add(this.textBoxImages);
            this.groupBoxGenerate.Controls.Add(this.comboBoxDisplayImage);
            this.groupBoxGenerate.Controls.Add(this.labelDisplayWindow);
            this.groupBoxGenerate.Controls.Add(this.comboBoxAutoSave);
            this.groupBoxGenerate.Controls.Add(this.labelAutoSave);
            this.groupBoxGenerate.Controls.Add(this.textBoxFileNamePrefix);
            this.groupBoxGenerate.Controls.Add(this.labelFileNamePrefix);
            this.groupBoxGenerate.Location = new System.Drawing.Point(8, 296);
            this.groupBoxGenerate.Name = "groupBoxGenerate";
            this.groupBoxGenerate.Size = new System.Drawing.Size(264, 128);
            this.groupBoxGenerate.TabIndex = 18;
            this.groupBoxGenerate.TabStop = false;
            this.groupBoxGenerate.Text = "Generate Image";
            // 
            // labelImages
            // 
            this.labelImages.Location = new System.Drawing.Point(192, 96);
            this.labelImages.Name = "labelImages";
            this.labelImages.Size = new System.Drawing.Size(56, 21);
            this.labelImages.TabIndex = 23;
            this.labelImages.Text = "Images";
            this.labelImages.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonGenerate
            // 
            this.buttonGenerate.Location = new System.Drawing.Point(64, 96);
            this.buttonGenerate.Name = "buttonGenerate";
            this.buttonGenerate.Size = new System.Drawing.Size(75, 23);
            this.buttonGenerate.TabIndex = 13;
            this.buttonGenerate.Text = "Generate";
            this.buttonGenerate.Click += new System.EventHandler(this.buttonGenerate_Click);
            // 
            // textBoxImages
            // 
            this.textBoxImages.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxImages.Location = new System.Drawing.Point(144, 96);
            this.textBoxImages.MaxLength = 4;
            this.textBoxImages.Name = "textBoxImages";
            this.textBoxImages.Size = new System.Drawing.Size(40, 21);
            this.textBoxImages.TabIndex = 12;
            this.textBoxImages.Text = "1";
            this.textBoxImages.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxNumOnly_KeyPress);
            this.textBoxImages.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxNumOnly_Validating);
            // 
            // comboBoxDisplayImage
            // 
            this.comboBoxDisplayImage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDisplayImage.Items.AddRange(new object[] {
            "None",
            "New Window",
            "Current Window"});
            this.comboBoxDisplayImage.Location = new System.Drawing.Point(144, 64);
            this.comboBoxDisplayImage.MaxDropDownItems = 5;
            this.comboBoxDisplayImage.Name = "comboBoxDisplayImage";
            this.comboBoxDisplayImage.Size = new System.Drawing.Size(112, 23);
            this.comboBoxDisplayImage.TabIndex = 11;
            // 
            // labelDisplayWindow
            // 
            this.labelDisplayWindow.Location = new System.Drawing.Point(8, 64);
            this.labelDisplayWindow.Name = "labelDisplayWindow";
            this.labelDisplayWindow.Size = new System.Drawing.Size(128, 21);
            this.labelDisplayWindow.TabIndex = 20;
            this.labelDisplayWindow.Text = "Display Window:";
            this.labelDisplayWindow.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // comboBoxAutoSave
            // 
            this.comboBoxAutoSave.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxAutoSave.Items.AddRange(new object[] {
            "Yes",
            "No"});
            this.comboBoxAutoSave.Location = new System.Drawing.Point(144, 14);
            this.comboBoxAutoSave.MaxDropDownItems = 5;
            this.comboBoxAutoSave.Name = "comboBoxAutoSave";
            this.comboBoxAutoSave.Size = new System.Drawing.Size(112, 23);
            this.comboBoxAutoSave.TabIndex = 9;
            // 
            // labelAutoSave
            // 
            this.labelAutoSave.Location = new System.Drawing.Point(8, 16);
            this.labelAutoSave.Name = "labelAutoSave";
            this.labelAutoSave.Size = new System.Drawing.Size(128, 21);
            this.labelAutoSave.TabIndex = 18;
            this.labelAutoSave.Text = "Auto Save:";
            this.labelAutoSave.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxFileNamePrefix
            // 
            this.textBoxFileNamePrefix.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxFileNamePrefix.Location = new System.Drawing.Point(144, 40);
            this.textBoxFileNamePrefix.MaxLength = 20;
            this.textBoxFileNamePrefix.Name = "textBoxFileNamePrefix";
            this.textBoxFileNamePrefix.Size = new System.Drawing.Size(96, 21);
            this.textBoxFileNamePrefix.TabIndex = 10;
            this.textBoxFileNamePrefix.Text = "Map";
            // 
            // labelFileNamePrefix
            // 
            this.labelFileNamePrefix.Location = new System.Drawing.Point(8, 40);
            this.labelFileNamePrefix.Name = "labelFileNamePrefix";
            this.labelFileNamePrefix.Size = new System.Drawing.Size(152, 21);
            this.labelFileNamePrefix.TabIndex = 5;
            this.labelFileNamePrefix.Text = "File Name Prefix:";
            this.labelFileNamePrefix.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxBorder);
            this.groupBox1.Controls.Add(this.labelBorder);
            this.groupBox1.Controls.Add(this.colorPickerBorder);
            this.groupBox1.Controls.Add(this.labelBorderColor);
            this.groupBox1.Controls.Add(this.colorPickerObstacle);
            this.groupBox1.Controls.Add(this.labelObstacleColor);
            this.groupBox1.Controls.Add(this.colorPickerPass);
            this.groupBox1.Controls.Add(this.labelPassColor);
            this.groupBox1.Controls.Add(this.textBoxCellSize);
            this.groupBox1.Controls.Add(this.labelCellSize);
            this.groupBox1.Location = new System.Drawing.Point(8, 152);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(264, 144);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Image";
            // 
            // textBoxBorder
            // 
            this.textBoxBorder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxBorder.Location = new System.Drawing.Point(144, 40);
            this.textBoxBorder.MaxLength = 5;
            this.textBoxBorder.Name = "textBoxBorder";
            this.textBoxBorder.Size = new System.Drawing.Size(56, 21);
            this.textBoxBorder.TabIndex = 5;
            this.textBoxBorder.Text = "1";
            this.textBoxBorder.TextChanged += new System.EventHandler(this.textBoxBorder_TextChanged);
            // 
            // labelBorder
            // 
            this.labelBorder.Location = new System.Drawing.Point(8, 40);
            this.labelBorder.Name = "labelBorder";
            this.labelBorder.Size = new System.Drawing.Size(80, 21);
            this.labelBorder.TabIndex = 18;
            this.labelBorder.Text = "Border:";
            this.labelBorder.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // colorPickerBorder
            // 
            this.colorPickerBorder.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.colorPickerBorder.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.colorPickerBorder.CausesValidation = false;
            this.colorPickerBorder.Cursor = System.Windows.Forms.Cursors.Hand;
            this.colorPickerBorder.Location = new System.Drawing.Point(144, 112);
            this.colorPickerBorder.Name = "colorPickerBorder";
            this.colorPickerBorder.Size = new System.Drawing.Size(20, 20);
            this.colorPickerBorder.TabIndex = 8;
            this.colorPickerBorder.BackColorChanged += new System.EventHandler(this.colorPickerBorder_BackColorChanged);
            this.colorPickerBorder.Click += new System.EventHandler(this.colorPicker_Click);
            // 
            // labelBorderColor
            // 
            this.labelBorderColor.Location = new System.Drawing.Point(8, 112);
            this.labelBorderColor.Name = "labelBorderColor";
            this.labelBorderColor.Size = new System.Drawing.Size(128, 21);
            this.labelBorderColor.TabIndex = 9;
            this.labelBorderColor.Text = "Border Color:";
            this.labelBorderColor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // colorPickerObstacle
            // 
            this.colorPickerObstacle.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.colorPickerObstacle.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.colorPickerObstacle.CausesValidation = false;
            this.colorPickerObstacle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.colorPickerObstacle.Location = new System.Drawing.Point(144, 88);
            this.colorPickerObstacle.Name = "colorPickerObstacle";
            this.colorPickerObstacle.Size = new System.Drawing.Size(20, 20);
            this.colorPickerObstacle.TabIndex = 7;
            this.colorPickerObstacle.BackColorChanged += new System.EventHandler(this.colorPickerObstacle_BackColorChanged);
            this.colorPickerObstacle.Click += new System.EventHandler(this.colorPicker_Click);
            // 
            // labelObstacleColor
            // 
            this.labelObstacleColor.Location = new System.Drawing.Point(8, 88);
            this.labelObstacleColor.Name = "labelObstacleColor";
            this.labelObstacleColor.Size = new System.Drawing.Size(128, 21);
            this.labelObstacleColor.TabIndex = 7;
            this.labelObstacleColor.Text = "Obstacle Color:";
            this.labelObstacleColor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // colorPickerPass
            // 
            this.colorPickerPass.BackColor = System.Drawing.SystemColors.ControlText;
            this.colorPickerPass.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.colorPickerPass.CausesValidation = false;
            this.colorPickerPass.Cursor = System.Windows.Forms.Cursors.Hand;
            this.colorPickerPass.Location = new System.Drawing.Point(144, 64);
            this.colorPickerPass.Name = "colorPickerPass";
            this.colorPickerPass.Size = new System.Drawing.Size(20, 20);
            this.colorPickerPass.TabIndex = 6;
            this.colorPickerPass.BackColorChanged += new System.EventHandler(this.colorPickerPass_BackColorChanged);
            this.colorPickerPass.Click += new System.EventHandler(this.colorPicker_Click);
            // 
            // labelPassColor
            // 
            this.labelPassColor.Location = new System.Drawing.Point(8, 64);
            this.labelPassColor.Name = "labelPassColor";
            this.labelPassColor.Size = new System.Drawing.Size(128, 21);
            this.labelPassColor.TabIndex = 5;
            this.labelPassColor.Text = "Pass Color:";
            this.labelPassColor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxCellSize
            // 
            this.textBoxCellSize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxCellSize.Location = new System.Drawing.Point(144, 16);
            this.textBoxCellSize.MaxLength = 5;
            this.textBoxCellSize.Name = "textBoxCellSize";
            this.textBoxCellSize.Size = new System.Drawing.Size(56, 21);
            this.textBoxCellSize.TabIndex = 4;
            this.textBoxCellSize.Text = "10";
            this.textBoxCellSize.TextChanged += new System.EventHandler(this.textBoxCellSize_TextChanged);
            this.textBoxCellSize.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxNumOnly_KeyPress);
            this.textBoxCellSize.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxNumOnly_Validating);
            // 
            // labelCellSize
            // 
            this.labelCellSize.Location = new System.Drawing.Point(8, 16);
            this.labelCellSize.Name = "labelCellSize";
            this.labelCellSize.Size = new System.Drawing.Size(96, 21);
            this.labelCellSize.TabIndex = 4;
            this.labelCellSize.Text = "Cell Size:";
            this.labelCellSize.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // colorDialog
            // 
            this.colorDialog.AnyColor = true;
            this.colorDialog.FullOpen = true;
            // 
            // groupBoxRunSearch
            // 
            this.groupBoxRunSearch.Controls.Add(this.labelThreads);
            this.groupBoxRunSearch.Controls.Add(this.textBoxThreads);
            this.groupBoxRunSearch.Controls.Add(this.checkBoxSaveProgress);
            this.groupBoxRunSearch.Controls.Add(this.buttonRunSearch);
            this.groupBoxRunSearch.Controls.Add(this.checkBoxDisplayImage);
            this.groupBoxRunSearch.Controls.Add(this.labelOuterLoop);
            this.groupBoxRunSearch.Controls.Add(this.textBoxOuterLoop);
            this.groupBoxRunSearch.Controls.Add(this.labelInnerLoop);
            this.groupBoxRunSearch.Controls.Add(this.textBoxInnerLoop);
            this.groupBoxRunSearch.Controls.Add(this.buttonResetSearch);
            this.groupBoxRunSearch.Location = new System.Drawing.Point(280, 248);
            this.groupBoxRunSearch.Name = "groupBoxRunSearch";
            this.groupBoxRunSearch.Size = new System.Drawing.Size(208, 176);
            this.groupBoxRunSearch.TabIndex = 31;
            this.groupBoxRunSearch.TabStop = false;
            this.groupBoxRunSearch.Text = "Run Search";
            // 
            // labelThreads
            // 
            this.labelThreads.Font = new System.Drawing.Font("Courier New", 9F);
            this.labelThreads.Location = new System.Drawing.Point(7, 64);
            this.labelThreads.Name = "labelThreads";
            this.labelThreads.Size = new System.Drawing.Size(128, 21);
            this.labelThreads.TabIndex = 29;
            this.labelThreads.Text = "Threads:";
            this.labelThreads.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxThreads
            // 
            this.textBoxThreads.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxThreads.Font = new System.Drawing.Font("Courier New", 9F);
            this.textBoxThreads.Location = new System.Drawing.Point(144, 64);
            this.textBoxThreads.MaxLength = 3;
            this.textBoxThreads.Name = "textBoxThreads";
            this.textBoxThreads.Size = new System.Drawing.Size(56, 21);
            this.textBoxThreads.TabIndex = 28;
            this.textBoxThreads.Text = "1";
            // 
            // checkBoxSaveProgress
            // 
            this.checkBoxSaveProgress.Enabled = false;
            this.checkBoxSaveProgress.Location = new System.Drawing.Point(8, 88);
            this.checkBoxSaveProgress.Name = "checkBoxSaveProgress";
            this.checkBoxSaveProgress.Size = new System.Drawing.Size(192, 24);
            this.checkBoxSaveProgress.TabIndex = 27;
            this.checkBoxSaveProgress.Text = "Save Progress";
            // 
            // buttonRunSearch
            // 
            this.buttonRunSearch.Location = new System.Drawing.Point(144, 144);
            this.buttonRunSearch.Name = "buttonRunSearch";
            this.buttonRunSearch.Size = new System.Drawing.Size(54, 24);
            this.buttonRunSearch.TabIndex = 13;
            this.buttonRunSearch.Text = "Run";
            this.buttonRunSearch.Click += new System.EventHandler(this.buttonRunSearch_Click);
            // 
            // checkBoxDisplayImage
            // 
            this.checkBoxDisplayImage.Enabled = false;
            this.checkBoxDisplayImage.Location = new System.Drawing.Point(8, 112);
            this.checkBoxDisplayImage.Name = "checkBoxDisplayImage";
            this.checkBoxDisplayImage.Size = new System.Drawing.Size(192, 24);
            this.checkBoxDisplayImage.TabIndex = 19;
            this.checkBoxDisplayImage.Text = "Display Image";
            // 
            // labelOuterLoop
            // 
            this.labelOuterLoop.Font = new System.Drawing.Font("Courier New", 9F);
            this.labelOuterLoop.Location = new System.Drawing.Point(7, 40);
            this.labelOuterLoop.Name = "labelOuterLoop";
            this.labelOuterLoop.Size = new System.Drawing.Size(128, 21);
            this.labelOuterLoop.TabIndex = 26;
            this.labelOuterLoop.Text = "Outer Loop:";
            this.labelOuterLoop.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxOuterLoop
            // 
            this.textBoxOuterLoop.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxOuterLoop.Font = new System.Drawing.Font("Courier New", 9F);
            this.textBoxOuterLoop.Location = new System.Drawing.Point(144, 40);
            this.textBoxOuterLoop.MaxLength = 5;
            this.textBoxOuterLoop.Name = "textBoxOuterLoop";
            this.textBoxOuterLoop.Size = new System.Drawing.Size(56, 21);
            this.textBoxOuterLoop.TabIndex = 25;
            this.textBoxOuterLoop.Text = "10";
            // 
            // labelInnerLoop
            // 
            this.labelInnerLoop.Font = new System.Drawing.Font("Courier New", 9F);
            this.labelInnerLoop.Location = new System.Drawing.Point(8, 16);
            this.labelInnerLoop.Name = "labelInnerLoop";
            this.labelInnerLoop.Size = new System.Drawing.Size(128, 21);
            this.labelInnerLoop.TabIndex = 12;
            this.labelInnerLoop.Text = "Inner Loop:";
            this.labelInnerLoop.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxInnerLoop
            // 
            this.textBoxInnerLoop.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxInnerLoop.Font = new System.Drawing.Font("Courier New", 9F);
            this.textBoxInnerLoop.Location = new System.Drawing.Point(144, 16);
            this.textBoxInnerLoop.MaxLength = 5;
            this.textBoxInnerLoop.Name = "textBoxInnerLoop";
            this.textBoxInnerLoop.Size = new System.Drawing.Size(56, 21);
            this.textBoxInnerLoop.TabIndex = 11;
            this.textBoxInnerLoop.Text = "100";
            // 
            // buttonResetSearch
            // 
            this.buttonResetSearch.Enabled = false;
            this.buttonResetSearch.Location = new System.Drawing.Point(80, 144);
            this.buttonResetSearch.Name = "buttonResetSearch";
            this.buttonResetSearch.Size = new System.Drawing.Size(54, 24);
            this.buttonResetSearch.TabIndex = 23;
            this.buttonResetSearch.Text = "Reset";
            // 
            // groupBoxSearchPath
            // 
            this.groupBoxSearchPath.Controls.Add(this.groupBoxSearchType);
            this.groupBoxSearchPath.Controls.Add(this.groupBoxSearchAlgorithm);
            this.groupBoxSearchPath.Controls.Add(this.labelJump);
            this.groupBoxSearchPath.Controls.Add(this.textBoxJump);
            this.groupBoxSearchPath.Location = new System.Drawing.Point(280, 8);
            this.groupBoxSearchPath.Name = "groupBoxSearchPath";
            this.groupBoxSearchPath.Size = new System.Drawing.Size(208, 208);
            this.groupBoxSearchPath.TabIndex = 30;
            this.groupBoxSearchPath.TabStop = false;
            this.groupBoxSearchPath.Text = "Search Path";
            // 
            // groupBoxSearchType
            // 
            this.groupBoxSearchType.Controls.Add(this.radioButton4Way);
            this.groupBoxSearchType.Controls.Add(this.radioButton8Way);
            this.groupBoxSearchType.Location = new System.Drawing.Point(8, 91);
            this.groupBoxSearchType.Name = "groupBoxSearchType";
            this.groupBoxSearchType.Size = new System.Drawing.Size(192, 72);
            this.groupBoxSearchType.TabIndex = 24;
            this.groupBoxSearchType.TabStop = false;
            this.groupBoxSearchType.Text = "Direction";
            // 
            // radioButton4Way
            // 
            this.radioButton4Way.Checked = true;
            this.radioButton4Way.Location = new System.Drawing.Point(8, 16);
            this.radioButton4Way.Name = "radioButton4Way";
            this.radioButton4Way.Size = new System.Drawing.Size(160, 21);
            this.radioButton4Way.TabIndex = 20;
            this.radioButton4Way.TabStop = true;
            this.radioButton4Way.Text = "4 Way";
            // 
            // radioButton8Way
            // 
            this.radioButton8Way.Location = new System.Drawing.Point(8, 40);
            this.radioButton8Way.Name = "radioButton8Way";
            this.radioButton8Way.Size = new System.Drawing.Size(160, 21);
            this.radioButton8Way.TabIndex = 21;
            this.radioButton8Way.Text = "8 Way";
            // 
            // groupBoxSearchAlgorithm
            // 
            this.groupBoxSearchAlgorithm.Controls.Add(this.radioButtonPriorityQueue);
            this.groupBoxSearchAlgorithm.Controls.Add(this.radioButtonDepthFirstSearch);
            this.groupBoxSearchAlgorithm.Location = new System.Drawing.Point(8, 17);
            this.groupBoxSearchAlgorithm.Name = "groupBoxSearchAlgorithm";
            this.groupBoxSearchAlgorithm.Size = new System.Drawing.Size(192, 71);
            this.groupBoxSearchAlgorithm.TabIndex = 22;
            this.groupBoxSearchAlgorithm.TabStop = false;
            this.groupBoxSearchAlgorithm.Text = "Algorithm";
            // 
            // radioButtonPriorityQueue
            // 
            this.radioButtonPriorityQueue.Checked = true;
            this.radioButtonPriorityQueue.Location = new System.Drawing.Point(8, 16);
            this.radioButtonPriorityQueue.Name = "radioButtonPriorityQueue";
            this.radioButtonPriorityQueue.Size = new System.Drawing.Size(160, 21);
            this.radioButtonPriorityQueue.TabIndex = 20;
            this.radioButtonPriorityQueue.TabStop = true;
            this.radioButtonPriorityQueue.Text = "Priority Queue";
            this.toolTip1.SetToolTip(this.radioButtonPriorityQueue, "Faster with large matrix, non recursive");
            // 
            // radioButtonDepthFirstSearch
            // 
            this.radioButtonDepthFirstSearch.AutoSize = true;
            this.radioButtonDepthFirstSearch.Location = new System.Drawing.Point(8, 40);
            this.radioButtonDepthFirstSearch.Name = "radioButtonDepthFirstSearch";
            this.radioButtonDepthFirstSearch.Size = new System.Drawing.Size(151, 19);
            this.radioButtonDepthFirstSearch.TabIndex = 21;
            this.radioButtonDepthFirstSearch.Text = "Depth First Search";
            this.toolTip1.SetToolTip(this.radioButtonDepthFirstSearch, "Faster with small matrix, recursive");
            // 
            // labelJump
            // 
            this.labelJump.Font = new System.Drawing.Font("Courier New", 9F);
            this.labelJump.Location = new System.Drawing.Point(8, 176);
            this.labelJump.Name = "labelJump";
            this.labelJump.Size = new System.Drawing.Size(128, 21);
            this.labelJump.TabIndex = 10;
            this.labelJump.Text = "Jump:";
            this.labelJump.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxJump
            // 
            this.textBoxJump.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxJump.Font = new System.Drawing.Font("Courier New", 9F);
            this.textBoxJump.Location = new System.Drawing.Point(144, 176);
            this.textBoxJump.MaxLength = 5;
            this.textBoxJump.Name = "textBoxJump";
            this.textBoxJump.Size = new System.Drawing.Size(54, 21);
            this.textBoxJump.TabIndex = 9;
            this.textBoxJump.Text = "0";
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 50000;
            this.toolTip1.InitialDelay = 500;
            this.toolTip1.ReshowDelay = 100;
            // 
            // FormControl
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(8, 14);
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(498, 432);
            this.ControlBox = false;
            this.Controls.Add(this.groupBoxRunSearch);
            this.Controls.Add(this.groupBoxSearchPath);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBoxGenerate);
            this.Controls.Add(this.groupBoxMap);
            this.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormControl";
            this.Text = "Path Control";
            this.groupBoxMap.ResumeLayout(false);
            this.groupBoxMap.PerformLayout();
            this.groupBoxGenerate.ResumeLayout(false);
            this.groupBoxGenerate.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBoxRunSearch.ResumeLayout(false);
            this.groupBoxRunSearch.PerformLayout();
            this.groupBoxSearchPath.ResumeLayout(false);
            this.groupBoxSearchPath.PerformLayout();
            this.groupBoxSearchType.ResumeLayout(false);
            this.groupBoxSearchAlgorithm.ResumeLayout(false);
            this.groupBoxSearchAlgorithm.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		// variables
		public FormDisplay CurrentDisplay; // Handler to the current active image window
		public Size DisplaySize = new Size(500, 500); // For remembering the current window size for new windows

		private void buttonGenerate_Click(object sender, System.EventArgs e)
		{
			#region Parse UI input
			// MapGenerator properties
			MapGeneratorInput mapInput = new MapGeneratorInput();
			mapInput.Width = Int32.Parse(textBoxWidth.Text);
			mapInput.Height = Int32.Parse(textBoxHeight.Text);
			mapInput.FillRate = Double.Parse(textBoxFill.Text) / 100;
			mapInput.FillAlgorithm = comboBoxFillAlgorithm.SelectedIndex; // 0 = Fixed Count, 1 = Random Count, 2 = Fractal
			mapInput.H = Double.Parse(textBoxH.Text);

            // PathSearcherInput properties
            PathSearcherInput pathInput = new PathSearcherInput();
            // Search algorithm, 1 = Priority Queue Search, 2 = DepthFirstSearch
            if (radioButtonPriorityQueue.Checked)
                pathInput.SearchAlgorithm = 1;
            else if (radioButtonDepthFirstSearch.Checked)
                pathInput.SearchAlgorithm = 2;
            // Search rule
            if (radioButton4Way.Checked)
                pathInput.SearchType = 1;
            else if (radioButton8Way.Checked)
                pathInput.SearchType = 2;
            // Jump range
            pathInput.Jump = Int32.Parse(textBoxJump.Text);

			// ImageGenerator properties
			ImageGeneratorInput imageInput = new ImageGeneratorInput();
			imageInput.CellSize = Int32.Parse(textBoxCellSize.Text);
			imageInput.BorderSize = Int32.Parse(textBoxBorder.Text);
			Color[] byteToColor = new Color[4];
			byteToColor[0] = colorPickerPass.BackColor;
			byteToColor[1] = colorPickerObstacle.BackColor;
            byteToColor[2] = colorPickerBorder.BackColor;
            byteToColor[3] = Color.Red;
			imageInput.ByteToColor = byteToColor;
			imageInput.BorderIndex = 2;

			// ImageFile properties
			ImageFileSettings fileSettings = new ImageFileSettings(textBoxFileNamePrefix.Text);
			// Execute options
			int autoSave = comboBoxAutoSave.SelectedIndex; // 0 = Yes, 1 = No
			int displayImage = comboBoxDisplayImage.SelectedIndex; // 0 = None, 1 = New Window, 2 = Current Window
			int imageCount = Int32.Parse(textBoxImages.Text);
			#endregion

			for (int ic = 0; ic < imageCount; ic++)
			{
                bool found = false;
                PathSearcher pathSearcher = new PathSearcher(pathInput, mapInput);
                if (pathSearcher.RunOne())
                {
                    found = true;
                }
                GifGenerator gifGenerator = new GifGenerator(pathSearcher.ByteMap, imageInput);
                gifGenerator.Generate();

				// Save bitmap
				string fileName = "Unsaved Image";
				if (autoSave == 0) // 0 = Yes, 1 = No
				{
					fileName = ImageFile.Save(gifGenerator.Image, mapInput, fileSettings);
                }
                if (found == true) // 0 = Yes, 1 = No
                {
                    fileName += " - Path Found";
                }
                else
                {
                    fileName += " - Not Found";
                }
				// Show bitmap
				// 0 = None, 1 = New Window, 2 = Current Window
				if ((displayImage == 2) && (CurrentDisplay != null))
				{
					CurrentDisplay.GifGen = gifGenerator;
					CurrentDisplay.AutoSaved = autoSave == 0 ? true : false;
					CurrentDisplay.FileName = fileName;
					CurrentDisplay.Display();
				}
				else if (displayImage != 0) // New window
				{
					FormDisplay formDisplay = new FormDisplay(gifGenerator, mapInput, fileSettings);
					formDisplay.MdiParent = this.MdiParent;
					formDisplay.AutoSaved = autoSave == 0 ? true : false;
					formDisplay.FileName = fileName;
					formDisplay.Size = DisplaySize;
					formDisplay.Show();
				}
			}
        }


        #region TextBox Code
        // Integer only text box
		private void textBoxNumOnly_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (e.KeyChar > 31 && (e.KeyChar < '0' || e.KeyChar > '9'))
			{
				e.Handled = true;
			}
		}

		private void textBoxNumOnly_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			double num;
			if (!Double.TryParse(((TextBox)(sender)).Text, System.Globalization.NumberStyles.Integer, null, out num))
			{
				e.Cancel=true;
				MessageBox.Show("Please enter a number");
				((TextBox)(sender)).SelectAll();
			}
        }

        // Percentage only text box
		private void textBoxFill_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (e.KeyChar > 31 && (e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '.')
			{
				e.Handled = true;
			}
		}

		private void textBoxFill_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			double num;
			if (Double.TryParse(((TextBox)(sender)).Text, System.Globalization.NumberStyles.Float, null, out num))
			{
				if (!((num >= 0) && (num <= 100)))
				{
					e.Cancel=true;
					MessageBox.Show("Please enter a value between 0 and 100");
					((TextBox)(sender)).SelectAll();
				}
			}
			else
			{
				e.Cancel=true;
				MessageBox.Show("Please enter a value between 0 and 100");
				((TextBox)(sender)).SelectAll();
			}
		}

		private void colorPicker_Click(object sender, System.EventArgs e)
		{
			if (colorDialog.ShowDialog() != DialogResult.Cancel)
			{
				((Label)sender).BackColor = colorDialog.Color;
			}
		}

		private void colorPickerPass_BackColorChanged(object sender, System.EventArgs e)
		{
			if (CurrentDisplay != null)
			{
				CurrentDisplay.GifGen.Input.ByteToColor[0] = ((Label)sender).BackColor;
				CurrentDisplay.GifGen.Generate();
				CurrentDisplay.AutoSaved = false;
				CurrentDisplay.FileName = "Unsaved Image";
				CurrentDisplay.Display();
			}
		}

		private void colorPickerObstacle_BackColorChanged(object sender, System.EventArgs e)
		{
			if (CurrentDisplay != null)
			{
				CurrentDisplay.GifGen.Input.ByteToColor[1] = ((Label)sender).BackColor;
				CurrentDisplay.GifGen.Generate();
				CurrentDisplay.AutoSaved = false;
				CurrentDisplay.FileName = "Unsaved Image";
				CurrentDisplay.Display();
			}
		}

		private void colorPickerBorder_BackColorChanged(object sender, System.EventArgs e)
		{
			if (CurrentDisplay != null)
			{
				CurrentDisplay.GifGen.Input.ByteToColor[2] = ((Label)sender).BackColor;
				CurrentDisplay.GifGen.Generate();
				CurrentDisplay.AutoSaved = false;
				CurrentDisplay.FileName = "Unsaved Image";
				CurrentDisplay.Display();
			}
		}

		private void textBoxBorder_TextChanged(object sender, System.EventArgs e)
		{
			if ((CurrentDisplay != null) && (((TextBox)sender).Text.Length != 0))
			{
				CurrentDisplay.GifGen.Input.BorderSize = Int32.Parse(((TextBox)sender).Text);
				CurrentDisplay.GifGen.Generate();
				CurrentDisplay.AutoSaved = false;
				CurrentDisplay.FileName = "Unsaved Image";
				CurrentDisplay.Display();
			}
		}

		private void textBoxCellSize_TextChanged(object sender, System.EventArgs e)
		{
			if ((CurrentDisplay != null) && (((TextBox)sender).Text.Length != 0))
			{
				CurrentDisplay.GifGen.Input.CellSize = Int32.Parse(((TextBox)sender).Text);
				CurrentDisplay.GifGen.Generate();
				CurrentDisplay.AutoSaved = false;
				CurrentDisplay.FileName = "Unsaved Image";
				CurrentDisplay.Display();
			}
        }
        #endregion

        private void buttonRunSearch_Click(object sender, System.EventArgs e)
		{
			#region Parse UI input
			// MapGenerator properties
			MapGeneratorInput mapInput = new MapGeneratorInput();
			mapInput.Width = Int32.Parse(textBoxWidth.Text);
			mapInput.Height = Int32.Parse(textBoxHeight.Text);
			mapInput.FillRate = Double.Parse(textBoxFill.Text) / 100;
			mapInput.FillAlgorithm = comboBoxFillAlgorithm.SelectedIndex; // 0 = Fixed Count, 1 = Random Count, 2 = Fractal
			mapInput.H = Double.Parse(textBoxH.Text);

			// PathSearcherInput properties
			PathSearcherInput pathInput = new PathSearcherInput();
			// Search algorithm, 1 = Priority Queue Search, 2 = DepthFirstSearch
			if (radioButtonPriorityQueue.Checked)
				pathInput.SearchAlgorithm = 1;
			else if (radioButtonDepthFirstSearch.Checked)
				pathInput.SearchAlgorithm = 2;
			// Search rule
			if (radioButton4Way.Checked)
				pathInput.SearchType = 1;
			else if (radioButton8Way.Checked)
				pathInput.SearchType = 2;
			// Jump range
			pathInput.Jump = Int32.Parse(textBoxJump.Text);

			// SearchTaskInput properties
			SearchTaskInput taskInput = new SearchTaskInput();
			// Number of iterations
			taskInput.OuterIterations = Int32.Parse(textBoxOuterLoop.Text);
			taskInput.InnerIterations = Int32.Parse(textBoxInnerLoop.Text);
			// Display image
			taskInput.DisplayImage = checkBoxDisplayImage.Checked;
			// ThreadNum
            // don't multithread with fractal (FFTW limitation)
            //if (mapInput.FillAlgorithm == 2)
            //    taskInput.ThreadNum = 1;
            //else
			    taskInput.ThreadNum = Int32.Parse(textBoxThreads.Text);
			// Save Progress
			taskInput.SaveProgress = checkBoxSaveProgress.Checked;
			#endregion

			FormSearchResults formSearch = new FormSearchResults(taskInput, pathInput, mapInput);
			formSearch.MdiParent = this.MdiParent;
			formSearch.Show();
		}
	}
}
