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
        private int currentEditingIndex = -1;
        private int lastTopIndex = -1;


        private Timer scrollTimer;
        private int lastScrollPosition = 0;

        public Form1()
        {
           

            InitializeComponent();
            
            AskUserToBackUp();
            PopulateListFromString(env.GetCurrentPath());
            SetupFloatingEditBox();
            InitializeEditingControls();
           
            // Initialize and start the timer for handling scrolling 
            scrollTimer = new Timer();
            scrollTimer.Interval = 10; // Check every 10 milliseconds
            scrollTimer.Tick += ScrollTimer_Tick;
            scrollTimer.Start();
           

        }
        private Button btnNew;
        private Button btnDelete;
        private Button btnMoveUp;
        private Button btnMoveDown;
        private TextBox editBox, editBox1;




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
                
                if (MessageBox.Show("Hi, We recommend that you back up your Path first. \n Do you Want to Proceed?", "Welcome ", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                  
                    BackupOriginal(false);
                }
                   

                File.WriteAllText(markerFilePath, "");

            }
        


        }
        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            if (e.Index >= 0)
            {
                string text = listBox1.Items[e.Index].ToString();
                using (SolidBrush brush = new SolidBrush(e.ForeColor))
                {
                    e.Graphics.DrawString(text, e.Font, brush, e.Bounds);
                }
            }
            e.DrawFocusRectangle();
        }

        private void listBox1_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = (int)e.Graphics.MeasureString(listBox1.Items[e.Index].ToString(), listBox1.Font).Height;
        }





        private void ScrollTimer_Tick(object sender, EventArgs e)
        {
            // Get the current scroll position
            int currentScrollPosition = GetListBoxScrollPosition(listBox1);

            // If the scroll position has changed, handle it
            if (currentScrollPosition != lastScrollPosition)
            {
                lastScrollPosition = currentScrollPosition;
                HandleScrollForEditing();
            }
        }

        // Method to get the current scroll position of the ListBox
        private int GetListBoxScrollPosition(ListBox listBox)
        {
            return listBox.TopIndex;
        }



        private void HandleScrollForEditing()
        {
            // Check if the TopIndex has changed
            if (listBox1.TopIndex != lastTopIndex)
            {
                lastTopIndex = listBox1.TopIndex;
                if (editBox1.Visible )
                {
                    SaveEdit();
                }
               
                
            }

            
           

        }

        protected override void WndProc(ref Message m)
        {
            const int WM_MOUSEWHEEL = 0x020A;

            int currentScrollPosition = GetListBoxScrollPosition(listBox1);

            // Check if the message is a mouse wheel event
            if (m.Msg == WM_MOUSEWHEEL)
            {
                
                //HandleScrollForEditing();

                if (currentScrollPosition != lastScrollPosition)
                {
                    lastScrollPosition = currentScrollPosition;
                    HandleScrollForEditing();
                }
            }

            base.WndProc(ref m);
        }

        private void InitializeEditingControls()
        {
            // Initialize the TextBox for editing
            editBox1 = new TextBox
            {
                Visible = false, // Initially hidden
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(editBox1);

            // Subscribe to the ListBox DoubleClick event

           
            listBox1.DoubleClick += ListBox1_DoubleClick;

            editBox1.Leave += EditTextBox_Leave;
            editBox1.KeyDown += EditTextBox_KeyDown;
        }


       
       

      
       private void EditListBoxElement(int index)
        {
           
            // Ensure the index is valid
            if (index >= 0 && index < listBox1.Items.Count)
            {
                // Show and position the TextBox
                Rectangle itemRect = listBox1.GetItemRectangle(index);
                editBox1.Location = new Point(listBox1.Left + itemRect.Left, listBox1.Top + itemRect.Top + 12);


                editBox1.Size = new Size(itemRect.Width + 3, itemRect.Height);

                editBox1.Text = listBox1.Items[index].ToString();
                editBox1.Visible = true;

                editBox1.BringToFront();
                editBox1.Focus();
              

            }
        }



        private void ListBox1_DoubleClick(object sender, EventArgs e)
        {
            if (listBox1.SelectionMode == SelectionMode.MultiExtended)
                return;

            // Hide previous TextBox if any
            if (editBox1.Visible)
            {
                SaveEdit();
            }

            // Get the index of the clicked item
            int index = listBox1.IndexFromPoint(listBox1.PointToClient(MousePosition));
            currentEditingIndex = index;

            EditListBoxElement(index);





        }






        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
          
        }




        private void EditTextBox_Leave(object sender, EventArgs e)
        {
           
                SaveEdit();

        }

        private void EditTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SaveEdit();
            }
        }

        private void SaveEdit()
        {
            if (editBox1.Visible && currentEditingIndex >= 0)
            {
                // Update the item text at the currently edited index
                if (currentEditingIndex >= 0 && !string.IsNullOrWhiteSpace(editBox1.Text) && currentEditingIndex < listBox1.Items.Count)
                {
                    listBox1.Items[currentEditingIndex] = editBox1.Text;
                }

               

                // Hide the TextBox after editing
                editBox1.Visible = false;
                currentEditingIndex = -1; // Reset the current editing index
            }
        }
        private void SetupFloatingEditBox()
        {
            editBox = new TextBox();
            editBox.Visible = false;
            editBox.KeyPress += EditBox_KeyPress;
            this.Controls.Add(editBox);

        }

        private void EditBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                AddNewItem(editBox.Text);
                editBox.Visible = false;
                e.Handled = true;
            }
        }



        private void UpdateListBoxWidth()
        {
            int maxItemWidth = 0;

            // Measure the width of the widest item
            foreach (var item in listBox1.Items)
            {
                int itemWidth = TextRenderer.MeasureText(item.ToString(), listBox1.Font).Width;
                if (itemWidth > maxItemWidth)
                {
                    maxItemWidth = itemWidth;
                }
            }

            // Add some padding
            listBox1.Width = maxItemWidth + 20; // Adjust the padding as needed
          
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

                // Ensure there's an empty item at the end for adding new entries
                listBox1.Items.Add(string.Empty);

                // Select the first newly added item
                listBox1.SelectedIndex = listBox1.Items.Count - items.Length - 1;
                UpdateListBoxWidth();
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
                if(!string.IsNullOrEmpty(item.ToString()) && !string.IsNullOrWhiteSpace(item.ToString()))
                {
                    // Append each item to the StringBuilder with a semicolon
                    sb.Append(item.ToString()).Append(";");
                }
               
            }

            string st = sb.ToString();

            // Remove the trailing semicolon, if any
            if (st.Length > 0 && st[st.Length-1]==';')
            {
                sb.Length--; // Remove the last semicolon
                st = sb.ToString();
            }

            return st;
        }
        private void AddNewItem(string item)
        {
            if (!string.IsNullOrWhiteSpace(item))
            {
                listBox1.Items.Insert(listBox1.Items.Count - 1, item.Trim());
                listBox1.SelectedIndex = listBox1.Items.Count - 2; // Select the newly added item
                UpdateListBoxWidth();
                listBox1.Refresh();


            }
        }


 

      

     





    private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            
            int index = listBox1.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {

               
                
                    editBox.Visible = false;
                
            }



           

        }
        private void btnNew_Click(object sender, EventArgs e)
        {
           

           
            listBox1.Items.Insert(listBox1.Items.Count - 1, " ");
         

            listBox1.SelectedIndex = listBox1.Items.Count - 2; // Select the newly added item
            lastScrollPosition = GetListBoxScrollPosition(listBox1);
            UpdateListBoxWidth();
            listBox1.Refresh();

          
            EditListBoxElement(listBox1.Items.Count - 2);
            currentEditingIndex = listBox1.Items.Count - 2;


            
           
        }



        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1 && listBox1.SelectedIndex != listBox1.Items.Count - 1)
            {
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);
                listBox1.Height -= 20;
               
            }
        }

      


        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            MoveItem(-1);
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            MoveItem(1);
        }



        private void btnSave_Click(object sender, EventArgs e)
        {



            DialogResult dialogResult = MessageBox.Show("Are You Sure that you want to Save Changes?", "Saving Paths", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);

            if (dialogResult == DialogResult.Yes)
            {
                env.SavingPath(GetListBoxItemsAsString());
                this.Close();
            }

        }

       
        private void BackupOriginal(bool WithMessage=true)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            saveFileDialog1.InitialDirectory = $@"{desktopPath}";

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
                        MessageBox.Show($"Original Paths Backed Up Successfully ", "Sucess", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                }
                else
                {
                    if (WithMessage)
                    {
                        MessageBox.Show("Original Paths Back Up Failed ", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);

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
            DialogResult dialogResult = MessageBox.Show("Data Wont be Saved\nAre You Sure that you want to cancel?", "Cancel", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);

            if (dialogResult == DialogResult.Yes)
            {
                this.Close();
            }

        }

        private void btnBulkAdd_Click(object sender, EventArgs e)
        {
            using (FormBulkAdd frm = new FormBulkAdd())
            {
                frm.Owner = this;
                frm.StartPosition = FormStartPosition.CenterParent; // Center the form within the parent

                DialogResult result = frm.ShowDialog();

                // Check if the form was closed with OK 
                if (result == DialogResult.OK)
                {
                    // Retrieve the result from FormBulkAdd
                    string bulkAdd = frm.GetBulkAdded();

                    // Use the result as needed
                    PopulateListFromString(bulkAdd);
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.RootFolder = Environment.SpecialFolder.MyComputer;


            folderBrowserDialog1.Description = "Choose Folder to Add to Path : ";

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {


                if (!string.IsNullOrWhiteSpace(folderBrowserDialog1.SelectedPath))
                {

                    AddNewItem(folderBrowserDialog1.SelectedPath);
                }
                else
                    MessageBox.Show("Path Choice Failed ", "Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;




            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

           openFileDialog1.InitialDirectory = $@"{desktopPath}";

            openFileDialog1.Title = "Save File As: ";
            openFileDialog1.DefaultExt = "txt";

            openFileDialog1.Filter = "text (*.txt)|*.txt";

            openFileDialog1.FilterIndex = 0;
            openFileDialog1.FileName = "";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
               


                PopulateListFromString(File.ReadAllText(openFileDialog1.FileName));

            }
           
        }

        private bool BackUpShowingPath(string backupFilePath, string PathsVariable )
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

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            saveFileDialog2.InitialDirectory = $@"{desktopPath}";

            saveFileDialog2.Title = "Save File As: ";
            saveFileDialog2.DefaultExt = "txt";

            saveFileDialog2.Filter = "text (*.txt)|*.txt";

            saveFileDialog2.FilterIndex = 0;

            string todayDate = DateTime.Now.ToString("yyyy-MM-dd");

            saveFileDialog2.FileName = $" Viewed paths Snapshot - {todayDate}";


            if (saveFileDialog2.ShowDialog() == DialogResult.OK)
            {


                if (BackUpShowingPath(saveFileDialog2.FileName, GetListBoxItemsAsString()))
                    MessageBox.Show($"Export to Viewed Snapshot is Saved Successfully ", "Sucess", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("Exportion to Viewed Snapshot  Failed  ", "Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);

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

     

        private void MoveItem(int direction)
        {
            if (listBox1.SelectedItem == null || listBox1.SelectedIndex < 0 ||
                listBox1.SelectedIndex == listBox1.Items.Count - 1)
                return;

            int newIndex = listBox1.SelectedIndex + direction;

            if (newIndex < 0 || newIndex >= listBox1.Items.Count - 1)
                return;

            object selected = listBox1.SelectedItem;

            listBox1.Items.RemoveAt(listBox1.SelectedIndex);
            listBox1.Items.Insert(newIndex, selected);
            listBox1.SelectedIndex = newIndex;
        }

       
    }

}

