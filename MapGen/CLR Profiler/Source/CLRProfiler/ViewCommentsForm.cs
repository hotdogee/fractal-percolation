// ==++==
// 
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// ==--==
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace CLRProfiler
{
    /// <summary>
    /// Summary description for ViewCommentsForm.
    /// </summary>
    public class ViewCommentsForm : System.Windows.Forms.Form
    {
        private System.Windows.Forms.TextBox commentTextBox;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public ViewCommentsForm(string[] comments)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            int count = 0;
            foreach (string s in comments)
                if (s != null)
                    count++;
            string[] lines = new string[count];
            count = 0;
            foreach (string s in comments)
                if (s != null)
                    lines[count++] = s;
            this.commentTextBox.Lines = lines;
        }

        /// <summary>
        /// Clean up any resources being used.
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

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.commentTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // commentTextBox
            // 
            this.commentTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.commentTextBox.Multiline = true;
            this.commentTextBox.Name = "commentTextBox";
            this.commentTextBox.ReadOnly = true;
            this.commentTextBox.Size = new System.Drawing.Size(880, 312);
            this.commentTextBox.TabIndex = 0;
            this.commentTextBox.Text = "";
            // 
            // ViewCommentsForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(880, 310);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.commentTextBox});
            this.Name = "ViewCommentsForm";
            this.Text = "Comments";
            this.ResumeLayout(false);

        }
        #endregion
    }
}
