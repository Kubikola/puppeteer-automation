﻿namespace Frontend.UserControls
{
    partial class EditUserControl
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
            this.components = new System.ComponentModel.Container();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.saveAndExitButton = new System.Windows.Forms.Button();
            this.codeGenButton = new System.Windows.Forms.Button();
            this.replayButton = new System.Windows.Forms.Button();
            this.recordButton = new System.Windows.Forms.Button();
            this.optimizeButton = new System.Windows.Forms.Button();
            this.browserConnection = new System.Windows.Forms.Button();
            this.processGroupBox = new System.Windows.Forms.GroupBox();
            this.selectedRadioButton = new System.Windows.Forms.RadioButton();
            this.enabledRadioButton = new System.Windows.Forms.RadioButton();
            this.selectedAllCheckBox = new System.Windows.Forms.CheckBox();
            this.filterButton = new System.Windows.Forms.Button();
            this.filterLabel = new System.Windows.Forms.Label();
            this.addAsFirst = new System.Windows.Forms.RadioButton();
            this.addAsLast = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.actionsFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.processGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // nameTextBox
            // 
            this.nameTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nameTextBox.Location = new System.Drawing.Point(39, 28);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(114, 26);
            this.nameTextBox.TabIndex = 1;
            this.toolTip.SetToolTip(this.nameTextBox, "The name of this recording.");
            // 
            // saveAndExitButton
            // 
            this.saveAndExitButton.Location = new System.Drawing.Point(3, 3);
            this.saveAndExitButton.Name = "saveAndExitButton";
            this.saveAndExitButton.Size = new System.Drawing.Size(96, 23);
            this.saveAndExitButton.TabIndex = 0;
            this.saveAndExitButton.Text = "Save && Exit";
            this.saveAndExitButton.UseVisualStyleBackColor = true;
            this.saveAndExitButton.Click += new System.EventHandler(this.saveAndExitButton_Click);
            // 
            // codeGenButton
            // 
            this.codeGenButton.Enabled = false;
            this.codeGenButton.Location = new System.Drawing.Point(77, 12);
            this.codeGenButton.Name = "codeGenButton";
            this.codeGenButton.Size = new System.Drawing.Size(75, 23);
            this.codeGenButton.TabIndex = 2;
            this.codeGenButton.Text = "Code Gen";
            this.codeGenButton.UseVisualStyleBackColor = true;
            this.codeGenButton.Click += new System.EventHandler(this.codeGenButton_Click);
            // 
            // replayButton
            // 
            this.replayButton.Enabled = false;
            this.replayButton.Location = new System.Drawing.Point(77, 37);
            this.replayButton.Name = "replayButton";
            this.replayButton.Size = new System.Drawing.Size(75, 23);
            this.replayButton.TabIndex = 3;
            this.replayButton.Text = "Replay";
            this.replayButton.UseVisualStyleBackColor = true;
            this.replayButton.Click += new System.EventHandler(this.replayButton_Click);
            // 
            // recordButton
            // 
            this.recordButton.Enabled = false;
            this.recordButton.Location = new System.Drawing.Point(263, 12);
            this.recordButton.Name = "recordButton";
            this.recordButton.Size = new System.Drawing.Size(96, 37);
            this.recordButton.TabIndex = 3;
            this.recordButton.Text = "Start Recording";
            this.recordButton.UseVisualStyleBackColor = true;
            this.recordButton.Click += new System.EventHandler(this.recordButton_Click);
            // 
            // optimizeButton
            // 
            this.optimizeButton.Enabled = false;
            this.optimizeButton.Location = new System.Drawing.Point(453, 3);
            this.optimizeButton.Name = "optimizeButton";
            this.optimizeButton.Size = new System.Drawing.Size(75, 23);
            this.optimizeButton.TabIndex = 5;
            this.optimizeButton.Text = "Optimize";
            this.optimizeButton.UseVisualStyleBackColor = true;
            this.optimizeButton.Click += new System.EventHandler(this.optimizeButton_Click);
            // 
            // browserConnection
            // 
            this.browserConnection.Location = new System.Drawing.Point(159, 12);
            this.browserConnection.Name = "browserConnection";
            this.browserConnection.Size = new System.Drawing.Size(98, 37);
            this.browserConnection.TabIndex = 2;
            this.browserConnection.Text = "Connect/Start Browser";
            this.browserConnection.UseVisualStyleBackColor = true;
            this.browserConnection.Click += new System.EventHandler(this.browserConnection_Click);
            // 
            // processGroupBox
            // 
            this.processGroupBox.Controls.Add(this.replayButton);
            this.processGroupBox.Controls.Add(this.codeGenButton);
            this.processGroupBox.Controls.Add(this.selectedRadioButton);
            this.processGroupBox.Controls.Add(this.enabledRadioButton);
            this.processGroupBox.Location = new System.Drawing.Point(543, 0);
            this.processGroupBox.Name = "processGroupBox";
            this.processGroupBox.Size = new System.Drawing.Size(164, 62);
            this.processGroupBox.TabIndex = 8;
            this.processGroupBox.TabStop = false;
            this.processGroupBox.Text = "Process";
            // 
            // selectedRadioButton
            // 
            this.selectedRadioButton.AutoSize = true;
            this.selectedRadioButton.Location = new System.Drawing.Point(6, 37);
            this.selectedRadioButton.Name = "selectedRadioButton";
            this.selectedRadioButton.Size = new System.Drawing.Size(67, 17);
            this.selectedRadioButton.TabIndex = 1;
            this.selectedRadioButton.Text = "Selected";
            this.selectedRadioButton.UseVisualStyleBackColor = true;
            this.selectedRadioButton.CheckedChanged += new System.EventHandler(this.processRadioButtons_CheckedChanged);
            // 
            // enabledRadioButton
            // 
            this.enabledRadioButton.AutoSize = true;
            this.enabledRadioButton.Checked = true;
            this.enabledRadioButton.Location = new System.Drawing.Point(6, 18);
            this.enabledRadioButton.Name = "enabledRadioButton";
            this.enabledRadioButton.Size = new System.Drawing.Size(64, 17);
            this.enabledRadioButton.TabIndex = 0;
            this.enabledRadioButton.TabStop = true;
            this.enabledRadioButton.Text = "Enabled";
            this.enabledRadioButton.UseVisualStyleBackColor = true;
            this.enabledRadioButton.CheckedChanged += new System.EventHandler(this.processRadioButtons_CheckedChanged);
            // 
            // selectedAllCheckBox
            // 
            this.selectedAllCheckBox.AutoSize = true;
            this.selectedAllCheckBox.Location = new System.Drawing.Point(14, 40);
            this.selectedAllCheckBox.Name = "selectedAllCheckBox";
            this.selectedAllCheckBox.Size = new System.Drawing.Size(15, 14);
            this.selectedAllCheckBox.TabIndex = 9;
            this.selectedAllCheckBox.ThreeState = true;
            this.toolTip.SetToolTip(this.selectedAllCheckBox, "Three-state checkbox for selection of actions.");
            this.selectedAllCheckBox.UseVisualStyleBackColor = true;
            this.selectedAllCheckBox.Click += new System.EventHandler(this.selectedAllCheckBox_Click);
            // 
            // filterButton
            // 
            this.filterButton.Location = new System.Drawing.Point(378, 4);
            this.filterButton.Name = "filterButton";
            this.filterButton.Size = new System.Drawing.Size(58, 39);
            this.filterButton.TabIndex = 4;
            this.filterButton.Text = "Filter Actions";
            this.filterButton.UseVisualStyleBackColor = true;
            this.filterButton.Click += new System.EventHandler(this.filterButton_Click);
            // 
            // filterLabel
            // 
            this.filterLabel.AutoSize = true;
            this.filterLabel.ForeColor = System.Drawing.Color.Red;
            this.filterLabel.Location = new System.Drawing.Point(373, 47);
            this.filterLabel.Name = "filterLabel";
            this.filterLabel.Size = new System.Drawing.Size(73, 13);
            this.filterLabel.TabIndex = 0;
            this.filterLabel.Text = "Filter Disabled";
            // 
            // addAsFirst
            // 
            this.addAsFirst.AutoSize = true;
            this.addAsFirst.Location = new System.Drawing.Point(489, 27);
            this.addAsFirst.Name = "addAsFirst";
            this.addAsFirst.Size = new System.Drawing.Size(44, 17);
            this.addAsFirst.TabIndex = 6;
            this.addAsFirst.Text = "First";
            this.toolTip.SetToolTip(this.addAsFirst, "If checked, the new action will be added as the first action.");
            this.addAsFirst.UseVisualStyleBackColor = true;
            // 
            // addAsLast
            // 
            this.addAsLast.AutoSize = true;
            this.addAsLast.Checked = true;
            this.addAsLast.Location = new System.Drawing.Point(489, 43);
            this.addAsLast.Name = "addAsLast";
            this.addAsLast.Size = new System.Drawing.Size(45, 17);
            this.addAsLast.TabIndex = 7;
            this.addAsLast.TabStop = true;
            this.addAsLast.Text = "Last";
            this.toolTip.SetToolTip(this.addAsLast, "If checked, the new action will be added as the last action.");
            this.addAsLast.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(447, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Add as";
            this.toolTip.SetToolTip(this.label1, "Defines the position where the new action will be added.");
            // 
            // actionsFlowLayoutPanel
            // 
            this.actionsFlowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.actionsFlowLayoutPanel.AutoScroll = true;
            this.actionsFlowLayoutPanel.Location = new System.Drawing.Point(2, 63);
            this.actionsFlowLayoutPanel.Name = "actionsFlowLayoutPanel";
            this.actionsFlowLayoutPanel.Size = new System.Drawing.Size(817, 362);
            this.actionsFlowLayoutPanel.TabIndex = 8;
            // 
            // toolTip
            // 
            this.toolTip.AutomaticDelay = 100;
            this.toolTip.AutoPopDelay = 2147483647;
            this.toolTip.InitialDelay = 100;
            this.toolTip.IsBalloon = true;
            this.toolTip.ReshowDelay = 0;
            this.toolTip.ShowAlways = true;
            // 
            // EditUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.actionsFlowLayoutPanel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.addAsFirst);
            this.Controls.Add(this.addAsLast);
            this.Controls.Add(this.optimizeButton);
            this.Controls.Add(this.filterLabel);
            this.Controls.Add(this.filterButton);
            this.Controls.Add(this.selectedAllCheckBox);
            this.Controls.Add(this.browserConnection);
            this.Controls.Add(this.recordButton);
            this.Controls.Add(this.processGroupBox);
            this.Controls.Add(this.saveAndExitButton);
            this.Controls.Add(this.nameTextBox);
            this.Name = "EditUserControl";
            this.Size = new System.Drawing.Size(822, 428);
            this.Resize += new System.EventHandler(this.EditUserControl_Resize);
            this.processGroupBox.ResumeLayout(false);
            this.processGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Button saveAndExitButton;
        private System.Windows.Forms.Button codeGenButton;
        private System.Windows.Forms.Button replayButton;
        private System.Windows.Forms.Button recordButton;
        private System.Windows.Forms.Button optimizeButton;
        private System.Windows.Forms.Button browserConnection;
        private System.Windows.Forms.GroupBox processGroupBox;
        private System.Windows.Forms.RadioButton selectedRadioButton;
        private System.Windows.Forms.RadioButton enabledRadioButton;
        private System.Windows.Forms.CheckBox selectedAllCheckBox;
        private System.Windows.Forms.Button filterButton;
        private System.Windows.Forms.Label filterLabel;
        private System.Windows.Forms.RadioButton addAsFirst;
        private System.Windows.Forms.RadioButton addAsLast;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel actionsFlowLayoutPanel;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
