﻿namespace Frontend.Forms
{
    partial class RecorderSettingsForm
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
            this.components = new System.ComponentModel.Container();
            this.recordedEventsPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.eventsToRecordLabel = new System.Windows.Forms.Label();
            this.puppeteerOptionsUserControl = new Frontend.UserControls.PuppeteerOptionsUserControl();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // recordedEventsPropertyGrid
            // 
            this.recordedEventsPropertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.recordedEventsPropertyGrid.Location = new System.Drawing.Point(254, 25);
            this.recordedEventsPropertyGrid.Name = "recordedEventsPropertyGrid";
            this.recordedEventsPropertyGrid.Size = new System.Drawing.Size(237, 271);
            this.recordedEventsPropertyGrid.TabIndex = 1;
            this.toolTip.SetToolTip(this.recordedEventsPropertyGrid, "Enables or disables recording of events.");
            // 
            // eventsToRecordLabel
            // 
            this.eventsToRecordLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.eventsToRecordLabel.AutoSize = true;
            this.eventsToRecordLabel.Location = new System.Drawing.Point(327, 9);
            this.eventsToRecordLabel.Name = "eventsToRecordLabel";
            this.eventsToRecordLabel.Size = new System.Drawing.Size(90, 13);
            this.eventsToRecordLabel.TabIndex = 2;
            this.eventsToRecordLabel.Text = "Recorded Events";
            this.toolTip.SetToolTip(this.eventsToRecordLabel, "Enables or disables recording of events.");
            // 
            // puppeteerOptionsUserControl
            // 
            this.puppeteerOptionsUserControl.Location = new System.Drawing.Point(12, 3);
            this.puppeteerOptionsUserControl.Name = "puppeteerOptionsUserControl";
            this.puppeteerOptionsUserControl.Size = new System.Drawing.Size(236, 290);
            this.puppeteerOptionsUserControl.TabIndex = 0;
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
            // RecorderSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(495, 305);
            this.Controls.Add(this.eventsToRecordLabel);
            this.Controls.Add(this.recordedEventsPropertyGrid);
            this.Controls.Add(this.puppeteerOptionsUserControl);
            this.Name = "RecorderSettingsForm";
            this.Text = " Recorder and Browser Settings";
            this.Load += new System.EventHandler(this.RecorderSettings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal UserControls.PuppeteerOptionsUserControl puppeteerOptionsUserControl;
        private System.Windows.Forms.PropertyGrid recordedEventsPropertyGrid;
        private System.Windows.Forms.Label eventsToRecordLabel;
        private System.Windows.Forms.ToolTip toolTip;
    }
}