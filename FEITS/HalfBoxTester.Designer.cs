namespace FEITS
{
    partial class HalfBoxTester
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HalfBoxTester));
            this.PB_TextBox = new System.Windows.Forms.PictureBox();
            this.RTB_Line = new System.Windows.Forms.RichTextBox();
            this.B_Reload = new System.Windows.Forms.Button();
            this.CB_TB = new System.Windows.Forms.ComboBox();
            this.LBL_TBType = new System.Windows.Forms.Label();
            this.LBL_Warning = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.PB_TextBox)).BeginInit();
            this.SuspendLayout();
            // 
            // PB_TextBox
            // 
            this.PB_TextBox.Image = global::FEITS.Properties.Resources.HalfBox;
            this.PB_TextBox.InitialImage = global::FEITS.Properties.Resources.TextBox;
            this.PB_TextBox.Location = new System.Drawing.Point(93, 12);
            this.PB_TextBox.Name = "PB_TextBox";
            this.PB_TextBox.Size = new System.Drawing.Size(198, 56);
            this.PB_TextBox.TabIndex = 1;
            this.PB_TextBox.TabStop = false;
            // 
            // RTB_Line
            // 
            this.RTB_Line.Location = new System.Drawing.Point(4, 126);
            this.RTB_Line.Name = "RTB_Line";
            this.RTB_Line.Size = new System.Drawing.Size(372, 119);
            this.RTB_Line.TabIndex = 2;
            this.RTB_Line.Text = "This is an example\\nmessage.";
            this.RTB_Line.TextChanged += new System.EventHandler(this.RTB_Line_TextChanged);
            // 
            // B_Reload
            // 
            this.B_Reload.Location = new System.Drawing.Point(4, 92);
            this.B_Reload.Name = "B_Reload";
            this.B_Reload.Size = new System.Drawing.Size(88, 28);
            this.B_Reload.TabIndex = 3;
            this.B_Reload.Text = "Reload Text";
            this.B_Reload.UseVisualStyleBackColor = true;
            this.B_Reload.Click += new System.EventHandler(this.B_Reload_Click);
            // 
            // CB_TB
            // 
            this.CB_TB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_TB.FormattingEnabled = true;
            this.CB_TB.Location = new System.Drawing.Point(294, 97);
            this.CB_TB.Name = "CB_TB";
            this.CB_TB.Size = new System.Drawing.Size(82, 21);
            this.CB_TB.TabIndex = 14;
            // 
            // LBL_TBType
            // 
            this.LBL_TBType.AutoSize = true;
            this.LBL_TBType.Location = new System.Drawing.Point(213, 100);
            this.LBL_TBType.Name = "LBL_TBType";
            this.LBL_TBType.Size = new System.Drawing.Size(75, 13);
            this.LBL_TBType.TabIndex = 13;
            this.LBL_TBType.Text = "TextBox Style:";
            // 
            // LBL_Warning
            // 
            this.LBL_Warning.AutoSize = true;
            this.LBL_Warning.ForeColor = System.Drawing.Color.Red;
            this.LBL_Warning.Location = new System.Drawing.Point(2, 251);
            this.LBL_Warning.Name = "LBL_Warning";
            this.LBL_Warning.Size = new System.Drawing.Size(305, 13);
            this.LBL_Warning.TabIndex = 15;
            this.LBL_Warning.Text = "Warning: Text contains one or more unsupported characters: {}";
            this.LBL_Warning.Visible = false;
            // 
            // HalfBoxTester
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(379, 271);
            this.Controls.Add(this.LBL_Warning);
            this.Controls.Add(this.CB_TB);
            this.Controls.Add(this.LBL_TBType);
            this.Controls.Add(this.B_Reload);
            this.Controls.Add(this.RTB_Line);
            this.Controls.Add(this.PB_TextBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(395, 310);
            this.MinimumSize = new System.Drawing.Size(395, 310);
            this.Name = "HalfBoxTester";
            this.Text = "Half-Size TextBox Tester";
            ((System.ComponentModel.ISupportInitialize)(this.PB_TextBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox PB_TextBox;
        private System.Windows.Forms.RichTextBox RTB_Line;
        private System.Windows.Forms.Button B_Reload;
        private System.Windows.Forms.ComboBox CB_TB;
        private System.Windows.Forms.Label LBL_TBType;
        private System.Windows.Forms.Label LBL_Warning;
    }
}