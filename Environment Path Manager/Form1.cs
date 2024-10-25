using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using EnvironmentPathManagerBusinessLayer;

namespace Manager
{


    public partial class Form1 : Form
    {
        public EnvironmentPathManager env = new EnvironmentPathManager();
       
        public Form1()
        {

            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();

            AskUserToBackUp();
            PopulateListFromString(env.GetCurrentPath());
            listBox1.SelectionMode = SelectionMode.MultiExtended;

        }
        private Button btnNew;
        private Button btnDelete;
        private Button btnMoveUp;
        private Button btnMoveDown;
        
        public EditListBox listBox1;



        private static string superEnvManagerDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".SuperEnvManager");



        private string markerFilePath = Path.Combine(superEnvManagerDir, "Temp12345.txt");


        private void CheckDir()
        {
            // Ensure the directory exists
            if (!Directory.Exists(superEnvManagerDir))
            {
                Directory.CreateDirectory(superEnvManagerDir);
            }

        }
        private void AskUserToBackUp()
        {


            // Define the path to the marker file in the user's local temp directory


            CheckDir();
            if (!File.Exists(markerFilePath))
            {

                if (CustomMessageBox.Show("Hi, We recommend that you back up your Path first. \n Do you Want to Proceed?", "Welcome ", MessageBoxIcon.Information, true, "OK") == DialogResult.Yes)
                {

                    BackupOriginal(false);
                }


                File.WriteAllText(markerFilePath, "");

            }



        }
       

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {


        }


        private void PopulateListFromString(string input)
        {
            string[] items = input.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                                  .Select(item => item.Trim())
                                  .Where(item => !string.IsNullOrWhiteSpace(item))
                                  .ToArray();

            if (items.Length > 0)
            {
                // Remove the last empty item if it exists
                if (listBox1.Items.Count > 0 && string.IsNullOrEmpty(listBox1.Items[listBox1.Items.Count - 1].ToString()))
                {
                    listBox1.Items.RemoveAt(listBox1.Items.Count - 1);
                }

                // Add new items
                listBox1.Items.AddRange(items);

                

                // Select the first newly added item
                listBox1.SelectedIndex = listBox1.Items.Count - items.Length - 1;
                
                listBox1.Refresh();
            }
        }

        private string GetListBoxItemsAsString()
        {
            // Initialize a StringBuilder to efficiently build the string
            StringBuilder sb = new StringBuilder();

            // Iterate through all items in the ListBox
            foreach (var item in listBox1.Items)
            {
                if (!string.IsNullOrEmpty(item.ToString()) && !string.IsNullOrWhiteSpace(item.ToString()))
                {
                    // Append each item to the StringBuilder with a semicolon
                    sb.Append(item.ToString()).Append(";");
                }

            }

            string st = sb.ToString();

            // Remove the trailing semicolon, if any
            if (st.Length > 0 && st[st.Length - 1] == ';')
            {
                sb.Length--; // Remove the last semicolon
                st = sb.ToString();
            }

            return st;
        }
     
        private void btnNew_Click(object sender, EventArgs e)
        {

            listBox1.AddNewItem("",true);

        }



        private void btnDelete_Click(object sender, EventArgs e)
        {
            

            if (listBox1.SelectedItems.Count > 0)
            {
                listBox1.ConfirmDelete = false;
                listBox1.DeleteSelectedItems();
                listBox1.Height -= 20; // Adjusts height if needed
            }
        }




        private void btnMoveUp_Click(object sender, EventArgs e)
        {
          
            listBox1.MoveItem(-1);
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            listBox1.MoveItem(1);
        }



        private void btnSave_Click(object sender, EventArgs e)
        {



            DialogResult dialogResult = CustomMessageBox.Show("Are You Sure that you want to Save Changes?", "Saving Paths", MessageBoxIcon.Exclamation, false, "Yes", "No", false);

            if (dialogResult == DialogResult.Yes)
            {
                env.SavingPath(GetListBoxItemsAsString());
                CustomMessageBox.Show("Saved to System Successfully", "Saved", MessageBoxIcon.Information, true, "OK");
            }

        }


        private void BackupOriginal(bool WithMessage = true)
        {
            //string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            //saveFileDialog1.InitialDirectory = $@"{desktopPath}";

            string ThisPcPath = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            saveFileDialog1.InitialDirectory = $@"{ThisPcPath}";


            saveFileDialog1.Title = "Save File As: ";
            saveFileDialog1.DefaultExt = "txt";

            saveFileDialog1.Filter = "text (*.txt)|*.txt";

            saveFileDialog1.FilterIndex = 0;
            string todayDate = DateTime.Now.ToString("yyyy-MM-dd");


            saveFileDialog1.FileName = $"Original Path Backup - {todayDate}";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {


                if (env.BackupPath(saveFileDialog1.FileName))
                {
                    if (WithMessage)
                    {
                        CustomMessageBox.Show($"Original Paths Backed Up Successfully ", "Sucess", MessageBoxIcon.Information, true, "OK");

                    }
                }
                else
                {
                    if (WithMessage)
                    {
                        CustomMessageBox.Show("Original Paths Back Up Failed ", "Failed", MessageBoxIcon.Error, true, "OK");

                    }
                }



            }


        }

        private void button3_Click(object sender, EventArgs e)
        {
            BackupOriginal();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = CustomMessageBox.Show("Data Won't be Saved\nAre You Sure that you want to cancel?", "Cancel", MessageBoxIcon.Exclamation, false, "Yes", "No", false);

            if (dialogResult == DialogResult.Yes)
            {
                this.Close();
            }

        }



        private void btnSearch_Click(object sender, EventArgs e)
        {

            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            IntPtr ownerHandle = this.Handle;


            dialog.Multiselect = true;


            bool? DialogResult = dialog.ShowDialog(ownerHandle);

            if (DialogResult == true)
            {

                foreach (string SelectedPath in dialog.SelectedPaths)
                {
                    if (!string.IsNullOrWhiteSpace(SelectedPath))
                    {

                        listBox1.AddNewItem(SelectedPath);
                    }


                }

            }


        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            //string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //openFileDialog1.InitialDirectory = $@"{desktopPath}";

            string ThisPcPath = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);




            openFileDialog1.InitialDirectory = $@"{ThisPcPath}";

            openFileDialog1.Title = "Import Backup text File : ";
            openFileDialog1.DefaultExt = "txt";

            openFileDialog1.Filter = "text (*.txt)|*.txt";

            openFileDialog1.FilterIndex = 0;
            openFileDialog1.FileName = "";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {



                PopulateListFromString(File.ReadAllText(openFileDialog1.FileName));

            }

        }

        private bool BackUpShowingPath(string backupFilePath, string PathsVariable)
        {
            try
            {


                // Check if the pathVariable is not null or empty
                if (!string.IsNullOrEmpty(PathsVariable))
                {
                    // Write the PATH variable to the specified file
                    File.WriteAllText(backupFilePath, PathsVariable);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            catch (Exception)
            {
                return false;
            }
        }
        private void btnBackShowingPath_Click(object sender, EventArgs e)
        {


            string ThisPcPath = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            saveFileDialog2.InitialDirectory = $@"{ThisPcPath}";

            saveFileDialog2.Title = "Save File As: ";
            saveFileDialog2.DefaultExt = "txt";

            saveFileDialog2.Filter = "text (*.txt)|*.txt";

            saveFileDialog2.FilterIndex = 0;

            string todayDate = DateTime.Now.ToString("yyyy-MM-dd");

            saveFileDialog2.FileName = $" Viewed paths Snapshot - {todayDate}";


            if (saveFileDialog2.ShowDialog() == DialogResult.OK)
            {


                if (BackUpShowingPath(saveFileDialog2.FileName, GetListBoxItemsAsString()))
                    CustomMessageBox.Show($"Export to Viewed Snapshot is Saved Successfully ", "Sucess", MessageBoxIcon.Information, true, "OK");
                else
                    CustomMessageBox.Show("Exportion to Viewed Snapshot  Failed  ", "Failure", MessageBoxIcon.Error, true, "OK");

            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItems = new List<object>();
            foreach (int index in listBox1.SelectedIndices)
            {
                selectedItems.Add(listBox1.Items[index]);
            }

        }



    }

   

}

