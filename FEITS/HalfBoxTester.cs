using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using FEITS.Properties;

namespace FEITS
{
    public partial class HalfBoxTester : Form
    {
        private bool[] ValidCharacters;
        private FontCharacter[] Characters;

        private Image[] TextBoxes = { Resources.HalfBox, Resources.HalfBox_Nohr, Resources.HalfBox_Hoshido };

        public HalfBoxTester(bool[] VC, FontCharacter[] C)
        {
            InitializeComponent();
            ValidCharacters = VC;
            Characters = C;
            CB_TB.Items.AddRange(new[] { "Standard", "Nohrian", "Hoshidan" });
            CB_TB.SelectedIndex = 0;
            B_Reload_Click(null, null);
        }

        private void B_Reload_Click(object sender, EventArgs e)
        {
            if (LBL_Warning.Visible)
                return;

            Image HB = TextBoxes[CB_TB.SelectedIndex].Clone() as Bitmap;
            Image Text = DrawString(new Bitmap(165,50), RTB_Line.Text.Replace("\\n", "\n"), 0, 22, Color.FromArgb(68, 8, 0)) as Bitmap;
            using (Graphics g = Graphics.FromImage(HB))
            {
                g.DrawImage(Text, new Point(10, 0));
                g.DrawImage(Resources.KeyPress, new Point(PB_TextBox.Width - 30, PB_TextBox.Height - HB.Height + 32));
            }
            PB_TextBox.Image = HB;
        }

        private Image DrawString(Image BaseImage, string Message, int StartX, int StartY, Color? TC = null)
        {
            Color TextColor = TC.HasValue ? TC.Value : Color.Black;
            int CurX = StartX;
            int CurY = StartY;
            Bitmap NewImage = BaseImage.Clone() as Bitmap;
            using (Graphics g = Graphics.FromImage(NewImage))
            {
                foreach (char c in Message)
                {
                    if (c == '\n')
                    {
                        CurY += 20;
                        CurX = StartX;
                    }
                    else
                    {
                        FontCharacter cur = Characters[GetValue(c)];
                        g.DrawImage(cur.GetGlyph(TextColor), new Point(CurX, CurY - cur.CropHeight));
                        CurX += cur.CropWidth;
                    }
                }
            }
            return NewImage;
        }

        private ushort GetValue(char c)
        {
            return BitConverter.ToUInt16(Encoding.Unicode.GetBytes(string.Empty + c), 0);
        }

        private void RTB_Line_TextChanged(object sender, EventArgs e)
        {
            bool invalids = false;
            List<char> inv = new List<char>();
            foreach (char c in RTB_Line.Text.Where(c => !ValidCharacters[GetValue(c)]))
            {
                if (!invalids)
                    invalids = true;
                if (!inv.Contains(c))
                    inv.Add(c);
            }
            if (invalids)
            {
                LBL_Warning.Text = string.Format("Warning: Text contains one or more unsupported characters: {0}", string.Join(",", inv));
                LBL_Warning.Visible = true;
            }
            else if (LBL_Warning.Visible)
                LBL_Warning.Visible = false;
        }

    }
}
