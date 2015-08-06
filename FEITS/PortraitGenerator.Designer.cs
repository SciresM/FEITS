namespace FEITS
{
    partial class PortraitGenerator
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
            this.PB_Portrait = new System.Windows.Forms.PictureBox();
            this.CB_PortraitStyle = new System.Windows.Forms.ComboBox();
            this.LBL_PortraitStyle = new System.Windows.Forms.Label();
            this.LBL_Character = new System.Windows.Forms.Label();
            this.CB_Character = new System.Windows.Forms.ComboBox();
            this.LBL_HairColor = new System.Windows.Forms.Label();
            this.CB_HairColor = new System.Windows.Forms.ComboBox();
            this.MTB_HairColor = new System.Windows.Forms.MaskedTextBox();
            this.CB_HairStyle = new System.Windows.Forms.ComboBox();
            this.LBL_HairStyle = new System.Windows.Forms.Label();
            this.CB_Eyes = new System.Windows.Forms.ComboBox();
            this.LBL_Eyes = new System.Windows.Forms.Label();
            this.CB_Kamui = new System.Windows.Forms.ComboBox();
            this.LBL_CharType = new System.Windows.Forms.Label();
            this.LBL_Emotions = new System.Windows.Forms.Label();
            this.CB_Emotion = new System.Windows.Forms.ComboBox();
            this.CHK_Blush = new System.Windows.Forms.CheckBox();
            this.CHK_SweatDrop = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Portrait)).BeginInit();
            this.SuspendLayout();
            // 
            // PB_Portrait
            // 
            this.PB_Portrait.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PB_Portrait.ImageLocation = "";
            this.PB_Portrait.InitialImage = global::FEITS.Properties.Resources.ギュンター_ct_ベース;
            this.PB_Portrait.Location = new System.Drawing.Point(6, 6);
            this.PB_Portrait.Name = "PB_Portrait";
            this.PB_Portrait.Size = new System.Drawing.Size(512, 512);
            this.PB_Portrait.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.PB_Portrait.TabIndex = 0;
            this.PB_Portrait.TabStop = false;
            this.PB_Portrait.Click += new System.EventHandler(this.PB_Portrait_Click);
            // 
            // CB_PortraitStyle
            // 
            this.CB_PortraitStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_PortraitStyle.FormattingEnabled = true;
            this.CB_PortraitStyle.Location = new System.Drawing.Point(82, 529);
            this.CB_PortraitStyle.Name = "CB_PortraitStyle";
            this.CB_PortraitStyle.Size = new System.Drawing.Size(67, 21);
            this.CB_PortraitStyle.TabIndex = 1;
            this.CB_PortraitStyle.SelectedIndexChanged += new System.EventHandler(this.CB_PortraitStyle_SelectedIndexChanged);
            // 
            // LBL_PortraitStyle
            // 
            this.LBL_PortraitStyle.AutoSize = true;
            this.LBL_PortraitStyle.Location = new System.Drawing.Point(12, 532);
            this.LBL_PortraitStyle.Name = "LBL_PortraitStyle";
            this.LBL_PortraitStyle.Size = new System.Drawing.Size(69, 13);
            this.LBL_PortraitStyle.TabIndex = 2;
            this.LBL_PortraitStyle.Text = "Portrait Style:";
            // 
            // LBL_Character
            // 
            this.LBL_Character.AutoSize = true;
            this.LBL_Character.Location = new System.Drawing.Point(153, 532);
            this.LBL_Character.Name = "LBL_Character";
            this.LBL_Character.Size = new System.Drawing.Size(56, 13);
            this.LBL_Character.TabIndex = 4;
            this.LBL_Character.Text = "Character:";
            // 
            // CB_Character
            // 
            this.CB_Character.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Character.FormattingEnabled = true;
            this.CB_Character.Location = new System.Drawing.Point(210, 529);
            this.CB_Character.Name = "CB_Character";
            this.CB_Character.Size = new System.Drawing.Size(83, 21);
            this.CB_Character.TabIndex = 3;
            this.CB_Character.SelectedIndexChanged += new System.EventHandler(this.CB_Character_SelectedIndexChanged);
            // 
            // LBL_HairColor
            // 
            this.LBL_HairColor.AutoSize = true;
            this.LBL_HairColor.Location = new System.Drawing.Point(299, 532);
            this.LBL_HairColor.Name = "LBL_HairColor";
            this.LBL_HairColor.Size = new System.Drawing.Size(56, 13);
            this.LBL_HairColor.TabIndex = 6;
            this.LBL_HairColor.Text = "Hair Color:";
            // 
            // CB_HairColor
            // 
            this.CB_HairColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_HairColor.FormattingEnabled = true;
            this.CB_HairColor.Location = new System.Drawing.Point(356, 529);
            this.CB_HairColor.Name = "CB_HairColor";
            this.CB_HairColor.Size = new System.Drawing.Size(95, 21);
            this.CB_HairColor.TabIndex = 5;
            this.CB_HairColor.SelectedIndexChanged += new System.EventHandler(this.CB_HairColor_SelectedIndexChanged);
            // 
            // MTB_HairColor
            // 
            this.MTB_HairColor.Enabled = false;
            this.MTB_HairColor.Location = new System.Drawing.Point(457, 530);
            this.MTB_HairColor.Mask = "\\#>AAAAAA";
            this.MTB_HairColor.Name = "MTB_HairColor";
            this.MTB_HairColor.Size = new System.Drawing.Size(61, 20);
            this.MTB_HairColor.TabIndex = 17;
            this.MTB_HairColor.Text = "5B5855";
            this.MTB_HairColor.TextChanged += new System.EventHandler(this.MTB_HairColor_TextChanged);
            // 
            // CB_HairStyle
            // 
            this.CB_HairStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_HairStyle.Enabled = false;
            this.CB_HairStyle.FormattingEnabled = true;
            this.CB_HairStyle.Location = new System.Drawing.Point(469, 561);
            this.CB_HairStyle.Name = "CB_HairStyle";
            this.CB_HairStyle.Size = new System.Drawing.Size(43, 21);
            this.CB_HairStyle.TabIndex = 26;
            this.CB_HairStyle.SelectedIndexChanged += new System.EventHandler(this.Kamui_Data_Changed);
            // 
            // LBL_HairStyle
            // 
            this.LBL_HairStyle.AutoSize = true;
            this.LBL_HairStyle.Enabled = false;
            this.LBL_HairStyle.Location = new System.Drawing.Point(418, 565);
            this.LBL_HairStyle.Name = "LBL_HairStyle";
            this.LBL_HairStyle.Size = new System.Drawing.Size(50, 13);
            this.LBL_HairStyle.TabIndex = 25;
            this.LBL_HairStyle.Text = "Hairstyle:";
            // 
            // CB_Eyes
            // 
            this.CB_Eyes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Eyes.Enabled = false;
            this.CB_Eyes.FormattingEnabled = true;
            this.CB_Eyes.Location = new System.Drawing.Point(359, 561);
            this.CB_Eyes.Name = "CB_Eyes";
            this.CB_Eyes.Size = new System.Drawing.Size(58, 21);
            this.CB_Eyes.TabIndex = 24;
            this.CB_Eyes.SelectedIndexChanged += new System.EventHandler(this.Kamui_Data_Changed);
            // 
            // LBL_Eyes
            // 
            this.LBL_Eyes.AutoSize = true;
            this.LBL_Eyes.Enabled = false;
            this.LBL_Eyes.Location = new System.Drawing.Point(326, 564);
            this.LBL_Eyes.Name = "LBL_Eyes";
            this.LBL_Eyes.Size = new System.Drawing.Size(33, 13);
            this.LBL_Eyes.TabIndex = 23;
            this.LBL_Eyes.Text = "Eyes:";
            // 
            // CB_Kamui
            // 
            this.CB_Kamui.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Kamui.Enabled = false;
            this.CB_Kamui.FormattingEnabled = true;
            this.CB_Kamui.Location = new System.Drawing.Point(241, 561);
            this.CB_Kamui.Name = "CB_Kamui";
            this.CB_Kamui.Size = new System.Drawing.Size(84, 21);
            this.CB_Kamui.TabIndex = 22;
            this.CB_Kamui.SelectedIndexChanged += new System.EventHandler(this.Kamui_Data_Changed);
            // 
            // LBL_CharType
            // 
            this.LBL_CharType.AutoSize = true;
            this.LBL_CharType.Enabled = false;
            this.LBL_CharType.Location = new System.Drawing.Point(202, 564);
            this.LBL_CharType.Name = "LBL_CharType";
            this.LBL_CharType.Size = new System.Drawing.Size(39, 13);
            this.LBL_CharType.TabIndex = 21;
            this.LBL_CharType.Text = "Player:";
            // 
            // LBL_Emotions
            // 
            this.LBL_Emotions.AutoSize = true;
            this.LBL_Emotions.Enabled = false;
            this.LBL_Emotions.Location = new System.Drawing.Point(12, 565);
            this.LBL_Emotions.Name = "LBL_Emotions";
            this.LBL_Emotions.Size = new System.Drawing.Size(53, 13);
            this.LBL_Emotions.TabIndex = 27;
            this.LBL_Emotions.Text = "Emotions:";
            // 
            // CB_Emotion
            // 
            this.CB_Emotion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Emotion.Enabled = false;
            this.CB_Emotion.FormattingEnabled = true;
            this.CB_Emotion.Location = new System.Drawing.Point(64, 561);
            this.CB_Emotion.Name = "CB_Emotion";
            this.CB_Emotion.Size = new System.Drawing.Size(59, 21);
            this.CB_Emotion.TabIndex = 28;
            this.CB_Emotion.SelectedIndexChanged += new System.EventHandler(this.Emotions_Changed);
            // 
            // CHK_Blush
            // 
            this.CHK_Blush.AutoSize = true;
            this.CHK_Blush.Enabled = false;
            this.CHK_Blush.Location = new System.Drawing.Point(127, 564);
            this.CHK_Blush.Name = "CHK_Blush";
            this.CHK_Blush.Size = new System.Drawing.Size(38, 17);
            this.CHK_Blush.TabIndex = 29;
            this.CHK_Blush.Text = "照";
            this.CHK_Blush.UseVisualStyleBackColor = true;
            this.CHK_Blush.CheckedChanged += new System.EventHandler(this.Emotions_Changed);
            // 
            // CHK_SweatDrop
            // 
            this.CHK_SweatDrop.AutoSize = true;
            this.CHK_SweatDrop.Enabled = false;
            this.CHK_SweatDrop.Location = new System.Drawing.Point(164, 564);
            this.CHK_SweatDrop.Name = "CHK_SweatDrop";
            this.CHK_SweatDrop.Size = new System.Drawing.Size(38, 17);
            this.CHK_SweatDrop.TabIndex = 30;
            this.CHK_SweatDrop.Text = "汗";
            this.CHK_SweatDrop.UseVisualStyleBackColor = true;
            this.CHK_SweatDrop.CheckedChanged += new System.EventHandler(this.Emotions_Changed);
            // 
            // PortraitGenerator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(524, 596);
            this.Controls.Add(this.CHK_SweatDrop);
            this.Controls.Add(this.CHK_Blush);
            this.Controls.Add(this.CB_Emotion);
            this.Controls.Add(this.LBL_Emotions);
            this.Controls.Add(this.CB_HairStyle);
            this.Controls.Add(this.LBL_HairStyle);
            this.Controls.Add(this.CB_Eyes);
            this.Controls.Add(this.LBL_Eyes);
            this.Controls.Add(this.CB_Kamui);
            this.Controls.Add(this.LBL_CharType);
            this.Controls.Add(this.MTB_HairColor);
            this.Controls.Add(this.LBL_HairColor);
            this.Controls.Add(this.CB_HairColor);
            this.Controls.Add(this.LBL_Character);
            this.Controls.Add(this.CB_Character);
            this.Controls.Add(this.LBL_PortraitStyle);
            this.Controls.Add(this.CB_PortraitStyle);
            this.Controls.Add(this.PB_Portrait);
            this.MaximumSize = new System.Drawing.Size(540, 635);
            this.MinimumSize = new System.Drawing.Size(540, 635);
            this.Name = "PortraitGenerator";
            this.Text = "FEITS Portrait Generator";
            ((System.ComponentModel.ISupportInitialize)(this.PB_Portrait)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox PB_Portrait;
        private System.Windows.Forms.ComboBox CB_PortraitStyle;
        private System.Windows.Forms.Label LBL_PortraitStyle;
        private System.Windows.Forms.Label LBL_Character;
        private System.Windows.Forms.ComboBox CB_Character;
        private System.Windows.Forms.Label LBL_HairColor;
        private System.Windows.Forms.ComboBox CB_HairColor;
        private System.Windows.Forms.MaskedTextBox MTB_HairColor;
        private System.Windows.Forms.ComboBox CB_HairStyle;
        private System.Windows.Forms.Label LBL_HairStyle;
        private System.Windows.Forms.ComboBox CB_Eyes;
        private System.Windows.Forms.Label LBL_Eyes;
        private System.Windows.Forms.ComboBox CB_Kamui;
        private System.Windows.Forms.Label LBL_CharType;
        private System.Windows.Forms.Label LBL_Emotions;
        private System.Windows.Forms.ComboBox CB_Emotion;
        private System.Windows.Forms.CheckBox CHK_Blush;
        private System.Windows.Forms.CheckBox CHK_SweatDrop;
    }
}