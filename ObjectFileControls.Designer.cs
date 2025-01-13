namespace USerialEditor
{
    partial class ObjectFileControls
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
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.propertyTreeView = new System.Windows.Forms.TreeView();
            this.panelName = new System.Windows.Forms.Panel();
            this.panelControl = new System.Windows.Forms.Panel();
            this.labelPropertyName = new System.Windows.Forms.Label();
            this.labelPropertyType = new System.Windows.Forms.Label();
            this.labelPropertySubtype = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panelName.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(0, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 344);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(3, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.propertyTreeView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.panelControl);
            this.splitContainer1.Panel2.Controls.Add(this.panelName);
            this.splitContainer1.Size = new System.Drawing.Size(466, 344);
            this.splitContainer1.SplitterDistance = 155;
            this.splitContainer1.TabIndex = 2;
            // 
            // treeView1
            // 
            this.propertyTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyTreeView.Location = new System.Drawing.Point(0, 0);
            this.propertyTreeView.Name = "treeView1";
            this.propertyTreeView.Size = new System.Drawing.Size(155, 344);
            this.propertyTreeView.TabIndex = 0;
            this.propertyTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // panelName
            // 
            this.panelName.Controls.Add(this.labelPropertySubtype);
            this.panelName.Controls.Add(this.labelPropertyType);
            this.panelName.Controls.Add(this.labelPropertyName);
            this.panelName.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelName.Location = new System.Drawing.Point(0, 0);
            this.panelName.Name = "panelName";
            this.panelName.Size = new System.Drawing.Size(307, 72);
            this.panelName.TabIndex = 0;
            // 
            // panelControl
            // 
            this.panelControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl.Location = new System.Drawing.Point(0, 72);
            this.panelControl.Name = "panelControl";
            this.panelControl.Size = new System.Drawing.Size(307, 272);
            this.panelControl.TabIndex = 1;
            // 
            // labelPropertyName
            // 
            this.labelPropertyName.AutoSize = true;
            this.labelPropertyName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPropertyName.Location = new System.Drawing.Point(3, 9);
            this.labelPropertyName.Name = "labelPropertyName";
            this.labelPropertyName.Size = new System.Drawing.Size(47, 15);
            this.labelPropertyName.TabIndex = 0;
            this.labelPropertyName.Text = "Name: \r\n";
            // 
            // labelPropertyType
            // 
            this.labelPropertyType.AutoSize = true;
            this.labelPropertyType.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPropertyType.Location = new System.Drawing.Point(3, 24);
            this.labelPropertyType.Name = "labelPropertyType";
            this.labelPropertyType.Size = new System.Drawing.Size(36, 15);
            this.labelPropertyType.TabIndex = 1;
            this.labelPropertyType.Text = "Type:";
            // 
            // labelPropertySubtype
            // 
            this.labelPropertySubtype.AutoSize = true;
            this.labelPropertySubtype.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPropertySubtype.Location = new System.Drawing.Point(3, 39);
            this.labelPropertySubtype.Name = "labelPropertySubtype";
            this.labelPropertySubtype.Size = new System.Drawing.Size(54, 15);
            this.labelPropertySubtype.TabIndex = 2;
            this.labelPropertySubtype.Text = "Subtype:";
            // 
            // ObjectFileControls
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.splitter1);
            this.Name = "ObjectFileControls";
            this.Size = new System.Drawing.Size(469, 344);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panelName.ResumeLayout(false);
            this.panelName.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView propertyTreeView;
        private System.Windows.Forms.Panel panelControl;
        private System.Windows.Forms.Panel panelName;
        private System.Windows.Forms.Label labelPropertyName;
        private System.Windows.Forms.Label labelPropertySubtype;
        private System.Windows.Forms.Label labelPropertyType;
    }
}
