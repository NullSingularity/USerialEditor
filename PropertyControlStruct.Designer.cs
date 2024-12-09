namespace USerialEditor
{
    partial class PropertyControlStruct
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.structListBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // structListBox
            // 
            this.structListBox.FormattingEnabled = true;
            this.structListBox.Location = new System.Drawing.Point(18, 15);
            this.structListBox.Name = "structListBox";
            this.structListBox.Size = new System.Drawing.Size(275, 251);
            this.structListBox.TabIndex = 1;
            // 
            // PropertyControlStruct
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.Controls.Add(this.structListBox);
            this.Name = "PropertyControlStruct";
            this.Size = new System.Drawing.Size(435, 347);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox structListBox;
    }
}
