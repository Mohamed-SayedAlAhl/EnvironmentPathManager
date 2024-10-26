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

        private Button btnNew;
        private Button btnDelete;
        private Button btnMoveUp;
        private Button btnMoveDown;

        private EditListBox listBox1;

        public Form1()
        {

            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();

            AskUserToBackUp();
            listBox1.PopulateListFromString(env.GetCurrentPath());
            listBox1.SelectionMode = SelectionMode.MultiExtended;

            ToolTip toolTip = new ToolTip();

            // Set up the tooltip delay properties if needed
            toolTip.AutoPopDelay = 4500;   // Time the tooltip remains visible (ms)
            toolTip.InitialDelay = 500;    // Delay before the tooltip appears (ms)
            toolTip.ReshowDelay = 200;     // Delay before it reappears after fading

            // Optional: Show tooltip instantly when hovering
            toolTip.ShowAlways = true;

            // Set the tooltip text for each button
            toolTip.SetToolTip(btnImport, "Import paths from a semicolon-separated file.");
            toolTip.SetToolTip(btnSnapshot, "Export displayed paths for easy editing.");
            toolTip.SetToolTip(btnbackup, "Backup current paths stored in the system environment.");
            toolTip.SetToolTip(btnRestore, "Restore paths from backup, replacing displayed paths.");

        }



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
                env.SavingPath(listBox1.GetListBoxItemsAsString());
                CustomMessageBox.Show("Saved to System Successfully", "Saved", MessageBoxIcon.Information, true, "OK");
            }

        }


        private void BackupOriginal(bool WithMessage = true)
        {
            

            string ThisPcPath = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            saveFileDialog1.InitialDirectory = $@"{ThisPcPath}";


            saveFileDialog1.Title = "Save File As ";
            saveFileDialog1.DefaultExt = "txt";

            saveFileDialog1.Filter = "text (*.txt)|*.txt";

            saveFileDialog1.FilterIndex = 0;
           
            string todayDate = DateTime.Now.ToString("yyyy-MM-dd - hh_mm tt");


            saveFileDialog1.FileName = $"Paths Backup - {todayDate}";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {


                if (env.BackupPath(saveFileDialog1.FileName))
                {
                    if (WithMessage)
                    {
                        CustomMessageBox.Show($"Backup of paths completed successfully.", "Success", MessageBoxIcon.Information, true, "OK");

                    }
                }
                else
                {
                    if (WithMessage)
                    {
                        CustomMessageBox.Show("Backup of paths failed ", "Failure", MessageBoxIcon.Error, true, "OK");

                    }
                }



            }


        }

        private void btnbackup_Click(object sender, EventArgs e)
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



        private void btnBrowse_Click(object sender, EventArgs e)
        {

            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            IntPtr ownerHandle = this.Handle;


            dialog.Multiselect = true;


            bool? DialogResult = dialog.ShowDialog(ownerHandle);

            if (DialogResult == true)
            {

                listBox1.PopulateListFromArray(dialog.SelectedPaths);


            }


        }

        private void GetDataForImport_Restore(bool IsRestore=false)
        {

            string ThisPcPath = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);

            openFileDialog1.InitialDirectory = $@"{ThisPcPath}";

            openFileDialog1.Title = IsRestore ? "Restore backup from a text file " : "Import paths from a semicolon-separated file ";
            openFileDialog1.DefaultExt = "txt";

            openFileDialog1.Filter = "text (*.txt)|*.txt";

            openFileDialog1.FilterIndex = 0;
            openFileDialog1.FileName = "";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {

                if(IsRestore)
                {
                    // Clear ListBox First Before Recovering
                    listBox1.Items.Clear();
                }
                

                listBox1.PopulateListFromString(File.ReadAllText(openFileDialog1.FileName));

            }

        }

        private void btnImport_Click(object sender, EventArgs e)
        {

            GetDataForImport_Restore(false);

        }

        private bool ExportDisplayedPaths(string backupFilePath, string PathsVariable)
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
        private void btnSnapshot_Click(object sender, EventArgs e)
        {


            string ThisPcPath = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            saveFileDialog1.InitialDirectory = $@"{ThisPcPath}";
                        
            saveFileDialog1.Title = "Save File As: ";
            saveFileDialog1.DefaultExt = "txt";
                          
            saveFileDialog1.Filter = "text (*.txt)|*.txt";
                          
            saveFileDialog1.FilterIndex = 0;

           
            string todayDate = DateTime.Now.ToString("yyyy-MM-dd - hh_mm tt");
            saveFileDialog1.FileName = $"Displayed Paths Snapshot - {todayDate}";


            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {


                if (ExportDisplayedPaths(saveFileDialog1.FileName, listBox1.GetListBoxItemsAsString()))
                    CustomMessageBox.Show($"Export to displayed snapshot completed successfully ", "Success", MessageBoxIcon.Information, true, "OK");
                else
                    CustomMessageBox.Show("Export to displayed snapshot failed  ", "Failure", MessageBoxIcon.Error, true, "OK");

            }
        }

        

        private void btnRestore_Click(object sender, EventArgs e)
        {

            GetDataForImport_Restore(true);
        }
    }

   

}

