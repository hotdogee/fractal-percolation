using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MapGen
{
	/// <summary>
	/// FormDebug ���K�n�y�z�C
	/// </summary>
	public class FormDebug : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button button1;
		/// <summary>
		/// �]�p�u��һݪ��ܼơC
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormDebug()
		{
			//
			// Windows Form �]�p�u��䴩�����n��
			//
			InitializeComponent();

			//
			// TODO: �b InitializeComponent �I�s����[�J����غc�禡�{���X
			//
		}

		/// <summary>
		/// �M������ϥΤ����귽�C
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

		#region Windows Form �]�p�u�㲣�ͪ��{���X
		/// <summary>
		/// �����]�p�u��䴩�ҥ�������k - �ФŨϥε{���X�s�边�ק�
		/// �o�Ӥ�k�����e�C
		/// </summary>
		private void InitializeComponent()
		{
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(48, 104);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(200, 112);
			this.textBox1.TabIndex = 0;
			this.textBox1.Text = "";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(104, 40);
			this.button1.Name = "button1";
			this.button1.TabIndex = 1;
			this.button1.Text = "Test";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// FormDebug
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 15);
			this.ClientSize = new System.Drawing.Size(292, 271);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.textBox1);
			this.Name = "FormDebug";
			this.Text = "FormDebug";
			this.ResumeLayout(false);

		}
		#endregion

		private void button1_Click(object sender, System.EventArgs e)
		{
			int a = 3;
			int b = 5;
			textBox1.Text = String.Format("a = {0}, b = {1}", a, b);
		}
	}
}
