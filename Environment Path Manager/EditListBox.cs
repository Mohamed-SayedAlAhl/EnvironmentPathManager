using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Manager
{
    public class EditListBox : ListBox
    {
        public EditListBox()
        {
            this.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;

            this.CommitOnLeave = true;
            this.CommitOnEnter = true;
            this.AllowDelete = true;
            this.ConfirmDelete = true;
            this.ConfirmDeleteText = "Are you sure you want to delete the selected items?";

            // Create a new TextBox and set some properties
            textBox = new TextBox();


            textBox.Visible = false;
            textBox.AcceptsReturn = false;
            textBox.AcceptsTab = true;
            textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            textBox.BackColor = this.BackColor;
            textBox.ForeColor = this.ForeColor;
            textBox.Font = this.Font;
            textBox.Leave += TextBox_Leave;
            textBox.KeyDown += TextBox_KeyDown;

            this.Controls.Add(textBox);
        }

        #region Private Fields

        private TextBox textBox;
        private int editingIndex = -1;

        #endregion

        #region Constants

        private const int WM_VSCROLL = 0x115;
        private const int WM_MOUSEWHEEL = 0x20A;

        #endregion

        #region Events

        public event EventHandler<EditEventArgs> ItemEdited;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value determining whether the editing text is committed when the editing textbox loses focus.
        /// </summary>
        public bool CommitOnLeave { get; set; }

        /// <summary>
        /// Gets or sets a value determining whether the editing text is committed when the user presses the Enter or Return key.
        /// </summary>
        public bool CommitOnEnter { get; set; }

        /// <summary>
        /// Gets or sets a value determining whether the user can delete items using the Delete key.
        /// </summary>
        public bool AllowDelete { get; set; }

        /// <summary>
        /// Gets or sets a value determining whether a confirmation MessageBox is shown when the Delete key is pressed to delete items.
        /// </summary>
        public bool ConfirmDelete { get; set; }

        /// <summary>
        /// Gets or sets the text displayed in the confirmation MessageBox when the Delete key is pressed to delete items.
        /// </summary>
        public string ConfirmDeleteText { get; set; }

        #endregion

        #region Overrides

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);

            // Match the font of the editing TextBox
            textBox.Font = this.Font;
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);

            // Match the BackColor of the editing TextBox
            textBox.BackColor = this.BackColor;
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);

            // Match the ForeColor of the editing TextBox
            textBox.ForeColor = this.ForeColor;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            // Dispose the TextBox
            if (disposing) textBox.Dispose();
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);

            // Draw the default background
            e.DrawBackground();

            if (e.Index > -1 && e.Index < this.Items.Count)
            {
                object item = this.Items[e.Index];
                string text = this.GetItemText(item);
                Rectangle itemRect = this.GetItemRectangle(e.Index);

                Color textColor;

                // Check if the item is selected
                // if so, the background and textcolor is different
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                {
                    textColor = SystemColors.HighlightText;
                    using (var b = new SolidBrush(SystemColors.Highlight))
                    {
                        e.Graphics.FillRectangle(b, itemRect);
                    }
                }
                else
                {
                    textColor = this.ForeColor;
                }

                // Draw the text
                using (var b = new SolidBrush(textColor))
                {
                    e.Graphics.DrawString(text, this.Font, b, itemRect.Location);
                }
            }

            // Draw the focus rectangle
            e.DrawFocusRectangle();
        }


        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                // If the left button is double-clicked, check if the double-click occured in an Item Rectangle
                for (int i = 0; i < this.Items.Count; i++)
                {
                    Rectangle itemRect = this.GetItemRectangle(i);

                    if (itemRect.Contains(e.Location))
                    {
                        // The double-click occured in this item, so display the TextBox in the right place, match its Text to the item's Text and give it focus.




                        editingIndex = i;
                        textBox.Bounds = itemRect;
                        textBox.Visible = true;
                        textBox.Text = this.GetItemText(this.Items[i]);
                        textBox.SelectionStart = 0;
                        textBox.SelectionLength = textBox.TextLength;
                        textBox.Focus();
                        break;



                    }
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            // Check for Ctrl + A
            if (e.Control && e.KeyCode == Keys.A)
            {
                // Select all items
                for (int i = 0; i < this.Items.Count; i++)
                {
                    this.SetSelected(i, true);
                }
                e.SuppressKeyPress = true; // Optional: suppress further processing of the key press
            }


            if (e.KeyCode == Keys.Delete && this.AllowDelete)
            {
                this.ConfirmDelete = false;
                this.DeleteSelectedItems();
            }


        }

        protected override void WndProc(ref Message m)
        {
            

            base.WndProc(ref m);

            // When the user starts scrolling, commit the edit

            if ((m.Msg == WM_VSCROLL || m.Msg == WM_MOUSEWHEEL)&& this.SelectedItems.Count == 1)
            {
                
                    if (this.CommitOnLeave)
                        this.CommitTextBox();
                    else
                        this.RollBackTextBox();

                
              
            }
            else
            {
                return;
            }
           
        }

        #endregion

        #region Textbox Events

        private void TextBox_Leave(object sender, EventArgs e)
        {
            if (this.CommitOnLeave)
                this.CommitTextBox();
            else
                this.RollBackTextBox();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.CommitOnEnter)
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
                {
                    this.CommitTextBox();
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes all selected items, after possibly showing a confirmation MessageBox.
        /// </summary>
        /// 

     

        public void DeleteSelectedItems()
        {


            // Display confirmation MessageBox if required
            bool canDelete = true;
            if (this.ConfirmDelete)
            {
                canDelete = (MessageBox.Show(this.ConfirmDeleteText, "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes);
            }

            if (canDelete && this.SelectedItems.Count > 0)
            {
                

                while (this.SelectedItems.Count > 0)
                {

                    this.Items.RemoveAt(this.SelectedIndex);
                }

            }

            this.Height -= 20; // Adjusts height
            

        }

        public void MoveItem(int direction)
        {



            // Check if there are any selected items
            if (this.SelectedItems.Count == 0)
                return;

            // Store selected items and their original indices
            var selectedItems = this.SelectedItems.Cast<object>().ToList();
            var originalIndices = this.SelectedIndices.Cast<int>().ToList();

            // Calculate the new positions for selected items
            int firstSelectedIndex = originalIndices.First();
            int lastSelectedIndex = originalIndices.Last();

            // Check if the movement is valid based on direction
            if (direction == 1) // Moving down
            {
                if (lastSelectedIndex + 1 >= this.Items.Count)
                    return; // Can't move down if it exceeds the bounds
            }
            else // Moving up
            {
                if (firstSelectedIndex <= 0)
                    return; // Can't move up if the first selected item is at the top
            }

           
                // Create a list of items to be moved down/up
                List<object> itemsToMove = new List<object>(selectedItems);

                

                // Remove the selected items from their current positions
                foreach (var item in itemsToMove)
                {
                    this.Items.Remove(item);
                }

                // New position for the first selected item
                int newPosition = direction == 1 ? firstSelectedIndex + 1 : firstSelectedIndex - 1;

                // Insert the items back at their new positions
                for (int i = 0; i < itemsToMove.Count; i++)
                {
                    this.Items.Insert(newPosition + i, itemsToMove[i]);
                }

                // Reselect the moved items
                foreach (var item in itemsToMove)
                {
                    int newIndex = this.Items.IndexOf(item);
                    this.SetSelected(newIndex, true); // Reselect moved items
                }
           
           
           
        }
        public void AddNewItem(string newItemText,bool editMode=false)
        {
            this.ClearSelected();
            this.Items.Add(newItemText); // Add the new item
           
            this.TopIndex = this.Items.Count - 1;
            

            // Enter edit mode for the new item
            if(editMode)
            {
                editingIndex = this.Items.Count - 1; // Set the editing index to the new item
                Rectangle itemRect = this.GetItemRectangle(editingIndex);

                // Set TextBox properties
                textBox.Bounds = itemRect;
                textBox.Visible = true;
                textBox.Text = newItemText;
                textBox.SelectionStart = 0; // Set cursor at the beginning
                textBox.SelectionLength = textBox.TextLength; // Select the whole text
                textBox.Focus(); // Focus on the TextBox

                
                this.SelectedIndex = this.Items.Count - 1; // Select the newly added item 
            }


            

            this.Refresh();

        }

        public void PopulateListFromArray(string[] items)
        {

            if (items.Length > 0)
            {
                // Remove the last empty item if it exists
                if (this.Items.Count > 0 && string.IsNullOrEmpty(this.Items[this.Items.Count - 1].ToString()))
                {
                    this.Items.RemoveAt(this.Items.Count - 1);
                }

                // Add new items
                this.Items.AddRange(items);

                this.ClearSelected();
                // Select the first newly added item
                this.SelectedIndex = this.Items.Count - items.Length;

                this.Refresh();
            }
        }

        public void PopulateListFromString(string input)
        {
            string[] items = input.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                                  .Select(item => item.Trim())
                                  .Where(item => !string.IsNullOrWhiteSpace(item))
                                  .ToArray();


            PopulateListFromArray(items);
        }

        public string GetListBoxItemsAsString()
        {
            // Initialize a StringBuilder to efficiently build the string
            StringBuilder sb = new StringBuilder();

            // Iterate through all items in the ListBox
            foreach (var item in this.Items)
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
        private void CommitTextBox()
        {
            if (editingIndex > -1 && editingIndex < this.Items.Count)
            {
                object item = this.Items[editingIndex];
                if (item.GetType() != typeof(string))
                {
                    // Item is not a string, so let the user handle the change of the text via the ItemEdited event
                    EditEventArgs e = new EditEventArgs(item, textBox.Text);
                    if (this.ItemEdited != null)
                    {
                        this.ItemEdited(this, e);
                        this.Items[editingIndex] = e.Item;
                    }
                    else
                    {
                        // User is not handling the event, so it won't work!
                        throw new Exception("You should handle the ItemEdited event to allow for item editing if the items are not strings.");
                    }
                }
                else
                {
                    // Item is a string, so we can handle it manually:
                    this.Items[editingIndex] = textBox.Text;
                }

                textBox.Visible = false;
            }
        }

        private void RollBackTextBox()
        {
            textBox.Clear();
            textBox.Visible = false;
        }

        #endregion

        #region Nested Classes

        public class EditEventArgs : EventArgs
        {
            /// <summary>
            /// The item that is being edited.
            /// </summary>
            public object Item { get; set; }

            /// <summary>
            /// The text that the user entered as the new item text.
            /// </summary>
            public string NewText { get; set; }

            public EditEventArgs(object item, string newText)
            {
                this.Item = item;
                this.NewText = newText;
            }
        }

        #endregion
    }
}
