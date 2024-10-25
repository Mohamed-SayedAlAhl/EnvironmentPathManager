using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Manager
{
    public class CustomMessageBox : Form
    {
        // Constants for Windows API functions
        private const int MF_BYPOSITION = 0x400;
        private const int MF_REMOVE = 0x1000;

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        private static extern int GetMenuItemCount(IntPtr hMenu);

        [DllImport("user32.dll")]
        private static extern bool RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);


        private Label lblMessage;
        private Button btnOk;
        private Button btnLater;
        private PictureBox iconBox;

        private const int PADDING = 20;
        private const int BUTTON_HEIGHT = 23;
        private const int BUTTON_BOTTOM_MARGIN = 15;
        private const int ADDITIONAL_VERTICAL_PADDING = 15;
        public CustomMessageBox(string message, string title, MessageBoxIcon icon, bool isOneButton = false, string YesButton = "Yes", string NoButton = "No", bool isHighlitedButtonYes = true, string fontStyleString = "Regular", string fontFamilyKey = "segui", float fontSize = 9)
        {
            // Set up the form
            this.Text = title;
            this.Width = 430;
            this.StartPosition = FormStartPosition.CenterScreen;

            this.ShowIcon = false;

            // Set the form's properties to have a fixed size and no maximize button
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;

            this.BackColor = Color.White;
            this.TopMost = true;

            DisableCloseButton();

            // PictureBox for the icon
            iconBox = new PictureBox();
            iconBox.Location = new Point(PADDING + 5, PADDING + 5);
            iconBox.Size = new Size(32, 32);
            this.Controls.Add(iconBox);

            SetIcon(icon);

            // Label for the message
            lblMessage = new Label();
            lblMessage.Text = message;
            lblMessage.AutoSize = false;
            lblMessage.Width = this.ClientSize.Width - iconBox.Right - PADDING * 2;

            FontStyle messageFontStyle = GetFontStyleFromString(fontStyleString);
            string fontFamily = GetFontFamilyFromDictionary(fontFamilyKey);
            lblMessage.Font = new Font(fontFamily, fontSize, messageFontStyle);

            // Calculate required height for the label
            Size textSize = TextRenderer.MeasureText(lblMessage.Text, lblMessage.Font,
                new Size(lblMessage.Width, 0),
                TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl);

            lblMessage.Height = Math.Max(iconBox.Height, textSize.Height);

            // Align label vertically with icon
            int labelY = iconBox.Top + (iconBox.Height - Math.Min(iconBox.Height, textSize.Height)) / 2;
            lblMessage.Location = new Point(iconBox.Right + 5, labelY);

            this.Controls.Add(lblMessage);

            // Adjust form height
            int contentHeight = Math.Max(iconBox.Bottom, lblMessage.Bottom) + PADDING;
            int formHeight = Math.Max(150, contentHeight + BUTTON_HEIGHT + BUTTON_BOTTOM_MARGIN + ADDITIONAL_VERTICAL_PADDING);
            this.ClientSize = new Size(this.ClientSize.Width, formHeight);

            // Adjust button positions
            int buttonY = this.ClientSize.Height - BUTTON_HEIGHT - BUTTON_BOTTOM_MARGIN + 5;

            // OK button (behaves as Yes)
            btnOk = new Button();
            btnOk.Width = 80;
            btnOk.Height = BUTTON_HEIGHT;
            btnOk.Text = YesButton;
            btnOk.Location = new Point(this.ClientSize.Width - PADDING - 80 - (isOneButton ? 0 : 90), buttonY);
            btnOk.UseVisualStyleBackColor = true;
            btnOk.DialogResult = DialogResult.Yes;
            this.Controls.Add(btnOk);

            if (!isOneButton)
            {
                // Later button (behaves as No)
                btnLater = new Button();
                btnLater.Width = 80;
                btnLater.Height = BUTTON_HEIGHT;
                btnLater.Text = NoButton;
                btnLater.Location = new Point(this.ClientSize.Width - PADDING - 80, buttonY);
                btnLater.UseVisualStyleBackColor = true;
                btnLater.DialogResult = DialogResult.No;
                this.Controls.Add(btnLater);
            }

            if (!isOneButton && isHighlitedButtonYes)
            {
                btnOk.TabIndex = 1;
                btnLater.TabIndex = 2;
            }
            else if (!isOneButton && !isHighlitedButtonYes)
            {
                btnLater.TabIndex = 1;
                btnOk.TabIndex = 2;
            }

            this.AcceptButton = btnOk;
            this.CancelButton = btnLater;
        }














        private void DisableCloseButton()
        {
            // Get the handle for the system menu (this includes the close button)
            IntPtr hMenu = GetSystemMenu(this.Handle, false);

            if (hMenu != IntPtr.Zero)
            {
                // Get the count of items in the system menu
                int menuItemCount = GetMenuItemCount(hMenu);

                // Remove the close button from the system menu
                // (It's usually the last item in the system menu)
                RemoveMenu(hMenu, (uint)(menuItemCount - 1), MF_BYPOSITION | MF_REMOVE);
            }
        }

        private string GetFontFamilyFromDictionary(string fontFamilyKey)
        {
            // Dictionary mapping font names to their corresponding family names
            Dictionary<string, string> fontFamilyMap = new Dictionary<string, string>
    {
        { "segui", "Segoe UI" },
        { "arial", "Arial" },
        { "times", "Times New Roman" },
        { "courier", "Courier New" },
        { "verdana", "Verdana" },
        { "tahoma", "Tahoma" }
        // Add more mappings as needed
    };

            // Return the corresponding font family or default to Arial
            return fontFamilyMap.ContainsKey(fontFamilyKey.ToLower()) ? fontFamilyMap[fontFamilyKey.ToLower()] : "Arial";
        }
        private FontStyle GetFontStyleFromString(string fontStyleString)
        {
            // Use a dictionary to map string to FontStyle
            var fontStyleMap = new Dictionary<string, FontStyle>()
        {
            { "Regular", FontStyle.Regular },
            { "Bold", FontStyle.Bold },
            { "Italic", FontStyle.Italic },
            { "BoldItalic", FontStyle.Bold | FontStyle.Italic },
            { "Underline", FontStyle.Underline },
            { "Strikeout", FontStyle.Strikeout }
        };

            // Return the mapped FontStyle if valid, otherwise default to Regular
            return fontStyleMap.TryGetValue(fontStyleString, out var fontStyle) ? fontStyle : FontStyle.Regular;
        }
        // Set the appropriate icon based on the MessageBoxIcon
        private void SetIcon(MessageBoxIcon icon)
        {
            switch (icon)
            {
                case MessageBoxIcon.Information:
                    iconBox.Image = SystemIcons.Information.ToBitmap();
                    break;
                case MessageBoxIcon.Warning:
                    iconBox.Image = SystemIcons.Warning.ToBitmap();
                    break;
                case MessageBoxIcon.Error:
                    iconBox.Image = SystemIcons.Error.ToBitmap();
                    break;
                case MessageBoxIcon.Question:
                    iconBox.Image = SystemIcons.Question.ToBitmap();
                    break;
                default:
                    iconBox.Image = null;
                    break;
            }
        }

        // Static method to show the custom message box
        public static DialogResult Show(string message, string title, MessageBoxIcon icon, bool isOneButton = false, string YesButton = "Yes", string NoButton = "No", bool isHighlitedButtonYes = true, string fontStyleString = "Regular", string fontFamilyKey = "segui", float fontSize = 9)
        {
            using (CustomMessageBox box = new CustomMessageBox(message, title, icon, isOneButton, YesButton, NoButton, isHighlitedButtonYes, fontStyleString, fontFamilyKey, fontSize))
            {
                // Show the dialog and return result
                return box.ShowDialog();
            }
        }
    }

}
