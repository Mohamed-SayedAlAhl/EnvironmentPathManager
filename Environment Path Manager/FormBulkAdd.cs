using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Manager
{
    public partial class FormBulkAdd : Form
    {
        public FormBulkAdd()
        {
            InitializeComponent();

            btnChoose.BackColor = Color.Transparent;

            // Remove the button's border
            btnChoose.FlatStyle = FlatStyle.Flat;
            btnChoose.FlatAppearance.BorderSize = 0;

        }




        string _BulkAdd="" ;
       
     
        public string GetBulkAdded()
        {
            return _BulkAdd;
        }

     
        private void btnChoose_Click_1(object sender, EventArgs e)
        {
            folderBrowserDialog1.RootFolder = Environment.SpecialFolder.MyComputer;


            folderBrowserDialog1.Description = "Choose Folder to Add to Path : ";

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {


                if (!string.IsNullOrWhiteSpace(folderBrowserDialog1.SelectedPath))
                {
                    if (!string.IsNullOrWhiteSpace(txt1.Text))
                    {
                        txt1.Text += ";";

                    }

                    txt1.Text += folderBrowserDialog1.SelectedPath;
                  
                }
                else
                    MessageBox.Show("Path Choice Failed :-(", "Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;




            }
        }

        private void btnAdd_Click_1(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txt1.Text))
            {
                _BulkAdd = txt1.Text;
            }
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

       
    }
}
