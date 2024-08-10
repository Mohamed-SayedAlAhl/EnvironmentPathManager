
namespace Manager
{
    partial class FormBulkAdd
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormBulkAdd));
            this.txt1 = new System.Windows.Forms.RichTextBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.btnChoose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txt1
            // 
            this.txt1.Location = new System.Drawing.Point(182, 27);
            this.txt1.Name = "txt1";
            this.txt1.Size = new System.Drawing.Size(255, 33);
            this.txt1.TabIndex = 16;
            this.txt1.Text = "";
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(147, 134);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(115, 40);
            this.btnAdd.TabIndex = 15;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click_1);
            // 
            // btnChoose
            // 
            this.btnChoose.BackColor = System.Drawing.Color.Gainsboro;
            this.btnChoose.BackgroundImage = global::tst.Properties.Resources._4673908;
            this.btnChoose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnChoose.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnChoose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChoose.Location = new System.Drawing.Point(69, 27);
            this.btnChoose.Name = "btnChoose";
            this.btnChoose.Size = new System.Drawing.Size(49, 33);
            this.btnChoose.TabIndex = 20;
            this.btnChoose.UseVisualStyleBackColor = false;
            this.btnChoose.Click += new System.EventHandler(this.btnChoose_Click_1);
            // 
            // FormBulkAdd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(458, 188);
            this.Controls.Add(this.btnChoose);
            this.Controls.Add(this.txt1);
            this.Controls.Add(this.btnAdd);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FormBulkAdd";
            this.Text = "Bulk Add";
          
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnChoose;
        private System.Windows.Forms.RichTextBox txt1;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}