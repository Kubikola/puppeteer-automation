﻿namespace Frontend.Forms
{
    partial class PlayerForm
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
            this.codeGeneratorOptionsPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // codeGeneratorOptionsPropertyGrid
            // 
            this.codeGeneratorOptionsPropertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.codeGeneratorOptionsPropertyGrid.Location = new System.Drawing.Point(12, 12);
            this.codeGeneratorOptionsPropertyGrid.Name = "codeGeneratorOptionsPropertyGrid";
            this.codeGeneratorOptionsPropertyGrid.Size = new System.Drawing.Size(259, 157);
            this.codeGeneratorOptionsPropertyGrid.TabIndex = 1;
            // 
            // PlayerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(286, 181);
            this.Controls.Add(this.codeGeneratorOptionsPropertyGrid);
            this.Name = "PlayerForm";
            this.Text = "PlayerForm";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PropertyGrid codeGeneratorOptionsPropertyGrid;
    }
}