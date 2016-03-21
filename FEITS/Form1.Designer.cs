namespace FEITS
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.PB_TextBox = new System.Windows.Forms.PictureBox();
            this.RTB_Line = new System.Windows.Forms.RichTextBox();
            this.B_Reload = new System.Windows.Forms.Button();
            this.B_Prev = new System.Windows.Forms.Button();
            this.B_Next = new System.Windows.Forms.Button();
            this.LBL_Warning = new System.Windows.Forms.Label();
            this.LBL_CharName = new System.Windows.Forms.Label();
            this.TB_CharName = new System.Windows.Forms.TextBox();
            this.LBL_CharType = new System.Windows.Forms.Label();
            this.LBL_TBType = new System.Windows.Forms.Label();
            this.CB_Corrin = new System.Windows.Forms.ComboBox();
            this.CB_TB = new System.Windows.Forms.ComboBox();
            this.LBL_HairColorA = new System.Windows.Forms.Label();
            this.MTB_HairColorA = new System.Windows.Forms.MaskedTextBox();
            this.MTB_HairColorB = new System.Windows.Forms.MaskedTextBox();
            this.LBL_HairColorB = new System.Windows.Forms.Label();
            this.LBL_Eyes = new System.Windows.Forms.Label();
            this.CB_Eyes = new System.Windows.Forms.ComboBox();
            this.LBL_HairStyle = new System.Windows.Forms.Label();
            this.CB_HairStyle = new System.Windows.Forms.ComboBox();
            this.B_PortraitGeneration = new System.Windows.Forms.Button();
            this.B_LvlUpTester = new System.Windows.Forms.Button();
            this.CHK_UseBackgrounds = new System.Windows.Forms.CheckBox();
            this.LBL_FacialFeature = new System.Windows.Forms.Label();
            this.CB_FacialFeature = new System.Windows.Forms.ComboBox();
            this.LBL_Accessory = new System.Windows.Forms.Label();
            this.CB_Accessory = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.PB_TextBox)).BeginInit();
            this.SuspendLayout();
            // 
            // PB_TextBox
            // 
            this.PB_TextBox.Image = global::FEITS.Properties.Resources.TextBox;
            this.PB_TextBox.InitialImage = global::FEITS.Properties.Resources.TextBox;
            this.PB_TextBox.Location = new System.Drawing.Point(7, 5);
            this.PB_TextBox.Name = "PB_TextBox";
            this.PB_TextBox.Size = new System.Drawing.Size(400, 240);
            this.PB_TextBox.TabIndex = 0;
            this.PB_TextBox.TabStop = false;
            this.PB_TextBox.Click += new System.EventHandler(this.PB_TextBox_Click);
            // 
            // RTB_Line
            // 
            this.RTB_Line.Location = new System.Drawing.Point(17, 378);
            this.RTB_Line.Name = "RTB_Line";
            this.RTB_Line.Size = new System.Drawing.Size(379, 119);
            this.RTB_Line.TabIndex = 1;
            this.RTB_Line.Text = "$t1$Wmエリーゼ|3$w0|$Wsエリーゼ|$Wa$Eびっくり,汗|This is an example conversation.$k$p$Wmサクラ|7$" +
                "w0|$Wsサクラ|$Wa$E怒,汗|It takes place between\\nSakura and Elise.$k";
            this.RTB_Line.TextChanged += new System.EventHandler(this.RTB_Line_TextChanged);
            // 
            // B_Reload
            // 
            this.B_Reload.Location = new System.Drawing.Point(17, 253);
            this.B_Reload.Name = "B_Reload";
            this.B_Reload.Size = new System.Drawing.Size(88, 28);
            this.B_Reload.TabIndex = 2;
            this.B_Reload.Text = "Reload Text";
            this.B_Reload.UseVisualStyleBackColor = true;
            this.B_Reload.Click += new System.EventHandler(this.B_Reload_Click);
            // 
            // B_Prev
            // 
            this.B_Prev.Enabled = false;
            this.B_Prev.Location = new System.Drawing.Point(111, 253);
            this.B_Prev.Name = "B_Prev";
            this.B_Prev.Size = new System.Drawing.Size(72, 28);
            this.B_Prev.TabIndex = 3;
            this.B_Prev.Text = "Previous";
            this.B_Prev.UseVisualStyleBackColor = true;
            this.B_Prev.Click += new System.EventHandler(this.B_Prev_Click);
            // 
            // B_Next
            // 
            this.B_Next.Enabled = false;
            this.B_Next.Location = new System.Drawing.Point(189, 253);
            this.B_Next.Name = "B_Next";
            this.B_Next.Size = new System.Drawing.Size(57, 28);
            this.B_Next.TabIndex = 4;
            this.B_Next.Text = "Next";
            this.B_Next.UseVisualStyleBackColor = true;
            this.B_Next.Click += new System.EventHandler(this.B_Next_Click);
            // 
            // LBL_Warning
            // 
            this.LBL_Warning.AutoSize = true;
            this.LBL_Warning.ForeColor = System.Drawing.Color.Red;
            this.LBL_Warning.Location = new System.Drawing.Point(14, 500);
            this.LBL_Warning.Name = "LBL_Warning";
            this.LBL_Warning.Size = new System.Drawing.Size(305, 13);
            this.LBL_Warning.TabIndex = 5;
            this.LBL_Warning.Text = "Warning: Text contains one or more unsupported characters: {}";
            this.LBL_Warning.Visible = false;
            // 
            // LBL_CharName
            // 
            this.LBL_CharName.AutoSize = true;
            this.LBL_CharName.Location = new System.Drawing.Point(256, 261);
            this.LBL_CharName.Name = "LBL_CharName";
            this.LBL_CharName.Size = new System.Drawing.Size(38, 13);
            this.LBL_CharName.TabIndex = 6;
            this.LBL_CharName.Text = "Name:";
            // 
            // TB_CharName
            // 
            this.TB_CharName.Location = new System.Drawing.Point(300, 258);
            this.TB_CharName.Name = "TB_CharName";
            this.TB_CharName.Size = new System.Drawing.Size(96, 20);
            this.TB_CharName.TabIndex = 7;
            this.TB_CharName.Text = "Corrin";
            this.TB_CharName.TextChanged += new System.EventHandler(this.TB_CharName_TextChanged);
            // 
            // LBL_CharType
            // 
            this.LBL_CharType.AutoSize = true;
            this.LBL_CharType.Location = new System.Drawing.Point(13, 291);
            this.LBL_CharType.Name = "LBL_CharType";
            this.LBL_CharType.Size = new System.Drawing.Size(39, 13);
            this.LBL_CharType.TabIndex = 9;
            this.LBL_CharType.Text = "Player:";
            // 
            // LBL_TBType
            // 
            this.LBL_TBType.AutoSize = true;
            this.LBL_TBType.Location = new System.Drawing.Point(264, 291);
            this.LBL_TBType.Name = "LBL_TBType";
            this.LBL_TBType.Size = new System.Drawing.Size(49, 13);
            this.LBL_TBType.TabIndex = 10;
            this.LBL_TBType.Text = "TextBox:";
            // 
            // CB_Corrin
            // 
            this.CB_Corrin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Corrin.FormattingEnabled = true;
            this.CB_Corrin.Location = new System.Drawing.Point(55, 288);
            this.CB_Corrin.Name = "CB_Corrin";
            this.CB_Corrin.Size = new System.Drawing.Size(73, 21);
            this.CB_Corrin.TabIndex = 11;
            this.CB_Corrin.SelectedIndexChanged += new System.EventHandler(this.CB_Corrin_SelectedIndexChanged);
            // 
            // CB_TB
            // 
            this.CB_TB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_TB.FormattingEnabled = true;
            this.CB_TB.Location = new System.Drawing.Point(314, 288);
            this.CB_TB.Name = "CB_TB";
            this.CB_TB.Size = new System.Drawing.Size(82, 21);
            this.CB_TB.TabIndex = 12;
            // 
            // LBL_HairColorA
            // 
            this.LBL_HairColorA.AutoSize = true;
            this.LBL_HairColorA.Location = new System.Drawing.Point(113, 321);
            this.LBL_HairColorA.Name = "LBL_HairColorA";
            this.LBL_HairColorA.Size = new System.Drawing.Size(71, 13);
            this.LBL_HairColorA.TabIndex = 13;
            this.LBL_HairColorA.Text = "Hair Color (L):";
            // 
            // MTB_HairColorA
            // 
            this.MTB_HairColorA.Location = new System.Drawing.Point(190, 318);
            this.MTB_HairColorA.Mask = "\\#>AAAAAA";
            this.MTB_HairColorA.Name = "MTB_HairColorA";
            this.MTB_HairColorA.Size = new System.Drawing.Size(61, 20);
            this.MTB_HairColorA.TabIndex = 14;
            this.MTB_HairColorA.TextChanged += new System.EventHandler(this.MTB_HairColorA_TextChanged);
            // 
            // MTB_HairColorB
            // 
            this.MTB_HairColorB.Location = new System.Drawing.Point(335, 318);
            this.MTB_HairColorB.Mask = "\\#>AAAAAA";
            this.MTB_HairColorB.Name = "MTB_HairColorB";
            this.MTB_HairColorB.Size = new System.Drawing.Size(61, 20);
            this.MTB_HairColorB.TabIndex = 16;
            this.MTB_HairColorB.TextChanged += new System.EventHandler(this.MTB_HairColorB_TextChanged);
            // 
            // LBL_HairColorB
            // 
            this.LBL_HairColorB.AutoSize = true;
            this.LBL_HairColorB.Location = new System.Drawing.Point(256, 321);
            this.LBL_HairColorB.Name = "LBL_HairColorB";
            this.LBL_HairColorB.Size = new System.Drawing.Size(73, 13);
            this.LBL_HairColorB.TabIndex = 15;
            this.LBL_HairColorB.Text = "Hair Color (R):";
            // 
            // LBL_Eyes
            // 
            this.LBL_Eyes.AutoSize = true;
            this.LBL_Eyes.Location = new System.Drawing.Point(134, 291);
            this.LBL_Eyes.Name = "LBL_Eyes";
            this.LBL_Eyes.Size = new System.Drawing.Size(54, 13);
            this.LBL_Eyes.TabIndex = 17;
            this.LBL_Eyes.Text = "Eye Style:";
            // 
            // CB_Eyes
            // 
            this.CB_Eyes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Eyes.FormattingEnabled = true;
            this.CB_Eyes.Location = new System.Drawing.Point(190, 288);
            this.CB_Eyes.Name = "CB_Eyes";
            this.CB_Eyes.Size = new System.Drawing.Size(73, 21);
            this.CB_Eyes.TabIndex = 18;
            // 
            // LBL_HairStyle
            // 
            this.LBL_HairStyle.AutoSize = true;
            this.LBL_HairStyle.Location = new System.Drawing.Point(13, 321);
            this.LBL_HairStyle.Name = "LBL_HairStyle";
            this.LBL_HairStyle.Size = new System.Drawing.Size(50, 13);
            this.LBL_HairStyle.TabIndex = 19;
            this.LBL_HairStyle.Text = "Hairstyle:";
            // 
            // CB_HairStyle
            // 
            this.CB_HairStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_HairStyle.FormattingEnabled = true;
            this.CB_HairStyle.Location = new System.Drawing.Point(64, 317);
            this.CB_HairStyle.Name = "CB_HairStyle";
            this.CB_HairStyle.Size = new System.Drawing.Size(43, 21);
            this.CB_HairStyle.TabIndex = 20;
            // 
            // B_PortraitGeneration
            // 
            this.B_PortraitGeneration.Location = new System.Drawing.Point(285, 527);
            this.B_PortraitGeneration.Name = "B_PortraitGeneration";
            this.B_PortraitGeneration.Size = new System.Drawing.Size(111, 23);
            this.B_PortraitGeneration.TabIndex = 21;
            this.B_PortraitGeneration.Text = "Portrait Generator";
            this.B_PortraitGeneration.UseVisualStyleBackColor = true;
            this.B_PortraitGeneration.Click += new System.EventHandler(this.B_PortraitGeneration_Click);
            // 
            // B_LvlUpTester
            // 
            this.B_LvlUpTester.Location = new System.Drawing.Point(17, 527);
            this.B_LvlUpTester.Name = "B_LvlUpTester";
            this.B_LvlUpTester.Size = new System.Drawing.Size(111, 23);
            this.B_LvlUpTester.TabIndex = 22;
            this.B_LvlUpTester.Text = "Half-Size Box Tester";
            this.B_LvlUpTester.UseVisualStyleBackColor = true;
            this.B_LvlUpTester.Click += new System.EventHandler(this.B_LvlUpTester_Click);
            // 
            // CHK_UseBackgrounds
            // 
            this.CHK_UseBackgrounds.AutoSize = true;
            this.CHK_UseBackgrounds.Location = new System.Drawing.Point(148, 531);
            this.CHK_UseBackgrounds.Name = "CHK_UseBackgrounds";
            this.CHK_UseBackgrounds.Size = new System.Drawing.Size(125, 17);
            this.CHK_UseBackgrounds.TabIndex = 23;
            this.CHK_UseBackgrounds.Text = "Enable Backgrounds";
            this.CHK_UseBackgrounds.UseVisualStyleBackColor = true;
            this.CHK_UseBackgrounds.CheckedChanged += new System.EventHandler(this.CHK_UseBackgrounds_CheckedChanged);
            // 
            // LBL_FacialFeature
            // 
            this.LBL_FacialFeature.AutoSize = true;
            this.LBL_FacialFeature.Location = new System.Drawing.Point(14, 352);
            this.LBL_FacialFeature.Name = "LBL_FacialFeature";
            this.LBL_FacialFeature.Size = new System.Drawing.Size(77, 13);
            this.LBL_FacialFeature.TabIndex = 24;
            this.LBL_FacialFeature.Text = "Facial Feature:";
            // 
            // CB_FacialFeature
            // 
            this.CB_FacialFeature.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_FacialFeature.FormattingEnabled = true;
            this.CB_FacialFeature.Location = new System.Drawing.Point(95, 349);
            this.CB_FacialFeature.Name = "CB_FacialFeature";
            this.CB_FacialFeature.Size = new System.Drawing.Size(134, 21);
            this.CB_FacialFeature.TabIndex = 25;
            // 
            // LBL_Accessory
            // 
            this.LBL_Accessory.AutoSize = true;
            this.LBL_Accessory.Location = new System.Drawing.Point(235, 352);
            this.LBL_Accessory.Name = "LBL_Accessory";
            this.LBL_Accessory.Size = new System.Drawing.Size(59, 13);
            this.LBL_Accessory.TabIndex = 26;
            this.LBL_Accessory.Text = "Accessory:";
            // 
            // CB_Accessory
            // 
            this.CB_Accessory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Accessory.FormattingEnabled = true;
            this.CB_Accessory.Location = new System.Drawing.Point(300, 349);
            this.CB_Accessory.Name = "CB_Accessory";
            this.CB_Accessory.Size = new System.Drawing.Size(92, 21);
            this.CB_Accessory.TabIndex = 27;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(414, 561);
            this.Controls.Add(this.CB_Accessory);
            this.Controls.Add(this.LBL_Accessory);
            this.Controls.Add(this.CB_FacialFeature);
            this.Controls.Add(this.LBL_FacialFeature);
            this.Controls.Add(this.CHK_UseBackgrounds);
            this.Controls.Add(this.B_LvlUpTester);
            this.Controls.Add(this.B_PortraitGeneration);
            this.Controls.Add(this.CB_HairStyle);
            this.Controls.Add(this.LBL_HairStyle);
            this.Controls.Add(this.CB_Eyes);
            this.Controls.Add(this.LBL_Eyes);
            this.Controls.Add(this.MTB_HairColorB);
            this.Controls.Add(this.LBL_HairColorB);
            this.Controls.Add(this.MTB_HairColorA);
            this.Controls.Add(this.LBL_HairColorA);
            this.Controls.Add(this.CB_TB);
            this.Controls.Add(this.CB_Corrin);
            this.Controls.Add(this.LBL_TBType);
            this.Controls.Add(this.LBL_CharType);
            this.Controls.Add(this.TB_CharName);
            this.Controls.Add(this.LBL_CharName);
            this.Controls.Add(this.LBL_Warning);
            this.Controls.Add(this.B_Next);
            this.Controls.Add(this.B_Prev);
            this.Controls.Add(this.B_Reload);
            this.Controls.Add(this.RTB_Line);
            this.Controls.Add(this.PB_TextBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(430, 600);
            this.MinimumSize = new System.Drawing.Size(430, 600);
            this.Name = "Form1";
            this.Text = "Fire Emblem: If Text Simulator";
            ((System.ComponentModel.ISupportInitialize)(this.PB_TextBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox PB_TextBox;
        private System.Windows.Forms.RichTextBox RTB_Line;
        private System.Windows.Forms.Button B_Reload;
        private System.Windows.Forms.Button B_Prev;
        private System.Windows.Forms.Button B_Next;
        private System.Windows.Forms.Label LBL_Warning;
        private System.Windows.Forms.Label LBL_CharName;
        private System.Windows.Forms.TextBox TB_CharName;
        private System.Windows.Forms.Label LBL_CharType;
        private System.Windows.Forms.Label LBL_TBType;
        private System.Windows.Forms.ComboBox CB_Corrin;
        private System.Windows.Forms.ComboBox CB_TB;
        private System.Windows.Forms.Label LBL_HairColorA;
        private System.Windows.Forms.MaskedTextBox MTB_HairColorA;
        private System.Windows.Forms.MaskedTextBox MTB_HairColorB;
        private System.Windows.Forms.Label LBL_HairColorB;
        private System.Windows.Forms.Label LBL_Eyes;
        private System.Windows.Forms.ComboBox CB_Eyes;
        private System.Windows.Forms.Label LBL_HairStyle;
        private System.Windows.Forms.ComboBox CB_HairStyle;
        private System.Windows.Forms.Button B_PortraitGeneration;
        private System.Windows.Forms.Button B_LvlUpTester;
        private System.Windows.Forms.CheckBox CHK_UseBackgrounds;
        private System.Windows.Forms.Label LBL_FacialFeature;
        private System.Windows.Forms.ComboBox CB_FacialFeature;
        private System.Windows.Forms.Label LBL_Accessory;
        private System.Windows.Forms.ComboBox CB_Accessory;
    }
}

