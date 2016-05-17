using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using FEITS.Properties;

namespace FEITS
{
    public partial class PortraitGenerator : Form
    {
        public List<string> ResourceList;
        public Dictionary<string, string> Names;

        public List<cbItem>[] Characters = new List<cbItem>[3];
        
        public string[] Prefixes = { "st", "bu", "ct" };

        private Color HAIR_COLOR;
        private string CHARACTER;
        private string EMOTIONS;

        private Control[] Emotion_Spec;
        private Control[] Kamui_Spec;

        private bool loaded;

        private Dictionary<string, int> DefaultHairs = new Dictionary<string, int>();
        private Dictionary<string, byte[]> FaceData;
        private string[] EyeStyles = { "a", "b", "c", "d", "e", "f", "g" };
        private string[] Kamuis = { "マイユニ男1", "マイユニ男2", "マイユニ女1", "マイユニ女2" };

        public PortraitGenerator(List<string> RL, Dictionary<string, string> N, Dictionary<string, byte[]> FD)
        {
            InitializeComponent();

            Emotion_Spec = new Control[] { LBL_Emotions, CB_Emotion, CHK_Blush, CHK_SweatDrop };
            Kamui_Spec = new Control[] { LBL_CharType, CB_Kamui, LBL_Eyes, CB_Eyes, LBL_HairStyle, CB_HairStyle, LBL_FacialFeature, CB_FacialFeature, LBL_Accessory, CB_Accessory };

            ResourceList = RL;
            Names = N;
            FaceData = FD;

            DefaultHairs = new Dictionary<string, int>();
            string[] HCs = Resources.HCs.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string HC in HCs)
            {
                var H = HC.Split(new[] { '\t' });
                DefaultHairs[H[0]] = int.Parse(H[1], NumberStyles.AllowHexSpecifier);
            }
            CB_HairColor.Items.Add("Custom");
            CB_HairColor.Items.AddRange(DefaultHairs.Keys.Select(s => s).ToArray());

            CB_PortraitStyle.Items.AddRange(new[] { "Standard", "Closeup", "Critical" });

            for (int i = 0; i < Characters.Length; i++)
            {
                Characters[i] = new List<cbItem>();
                foreach (string Resource in ResourceList)
                {
                    if (Resource.Contains("_" + Prefixes[i] + "_"))
                    {
                        string Character = Resource.Substring(0, Resource.IndexOf("_" + Prefixes[i] + "_"));
                        cbItem ncbi = new cbItem
                        {
                            Value = Character,
                            Text = Names.ContainsKey(Character) ? Names[Character] : Character
                        };
                        if (!ncbi.Text.Contains("マイユニ") && ncbi.Text != "Kanna")
                        {
                            if (Characters[i].All(cbi => cbi.Text != ncbi.Text) && Characters[i].All(cbi => cbi.Value != ncbi.Value))
                                Characters[i].Add(ncbi);
                        }
                    }
                }
                Characters[i].Add(new cbItem { Text = "Corrin", Value = "username" });
                Characters[i].Add(new cbItem { Text = "Kanna (M)", Value = "カンナ男" });
                Characters[i].Add(new cbItem { Text = "Kanna (F)", Value = "カンナ女" });
                Characters[i] = Characters[i].OrderBy(cbi => cbi.Text).ToList();
            }
            CB_Character.DisplayMember = "Text";
            CB_Character.ValueMember = "Value";

            CB_Kamui.Items.AddRange(new[] { "Male 1", "Male 2", "Female 1", "Female 2" });
            CB_Eyes.Items.AddRange(new[] { "Style A", "Style B", "Style C", "Style D", "Style E", "Style F", "Style G" });
            CB_HairStyle.Items.AddRange(Enumerable.Range(0, 12).Select(i => i.ToString("00")).ToArray());
            CB_FacialFeature.Items.AddRange(new[] { "None", "Scratches", "Vertical Scratches", "Horizontal Scratches", "Tattoo 1", "Tattoo 2", "Tattoo 3", "Eye Mole", "Mouth Mole", "Plaster 1", "Plaster 2", "White Eyepatch", "Black Eyepatch" });
            CB_Accessory.Items.AddRange(new[] { "None", "Silver Piece", "Butterfly", "Black Ribbon", "White Ribbon", "White Rose" });

            CB_PortraitStyle.SelectedIndex = 2;

            CB_Character.SelectedIndex = CB_HairColor.SelectedIndex = CB_Kamui.SelectedIndex = CB_Eyes.SelectedIndex = CB_HairStyle.SelectedIndex = CB_FacialFeature.SelectedIndex = CB_Accessory.SelectedIndex = 0;
            CB_Accessory.Enabled = LBL_Accessory.Enabled = CB_Kamui.SelectedIndex > 1;

            loaded = true;
            UpdateImage();
        }

        private void CB_PortraitStyle_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (CB_Character.SelectedIndex >= 0)
            {
                cbItem curitem = CB_Character.Items[CB_Character.SelectedIndex] as cbItem;
                CB_Character.DataSource = Characters[CB_PortraitStyle.SelectedIndex];
                bool found = false;
                for (int i=0;i<CB_Character.Items.Count;i++)
                {
                    if ((CB_Character.Items[i] as cbItem).Text == curitem.Text)
                    {
                        CB_Character.SelectedIndex = i;
                        found = true;
                    }
                }
                if (!found)
                    CB_Character.SelectedIndex = 0;
            }
            else
            {
                CB_Character.DataSource = Characters[CB_PortraitStyle.SelectedIndex];
                CB_Character.SelectedIndex = 0;
            }
            foreach (Control ctrl in Emotion_Spec)
                ctrl.Enabled = (CB_PortraitStyle.Items[CB_PortraitStyle.SelectedIndex] as string) != "Critical";
            if (!loaded)
                return;
            UpdateImage();
        }

        private void MTB_HairColor_TextChanged(object sender, EventArgs e)
        {
            string hc = MTB_HairColor.Text.ToUpper();
            for (int i = 1; i < hc.Length; i++)
            {
                if (!((hc[i] >= 'A' && hc[i] <= 'F') || (hc[i] >= '0' && hc[i] <= '9')))
                {
                    hc = hc.Substring(0, i) + hc.Substring(i + 1);
                }
            }
            if (hc != MTB_HairColor.Text)
                MTB_HairColor.Text = hc;
            else
            {
                while (hc.Length < 7)
                    hc = hc + "0";
                hc = hc.Replace("_", "0");
                HAIR_COLOR = Color.FromArgb((int)(0xFF000000 | uint.Parse(hc.Substring(1), NumberStyles.AllowHexSpecifier)));
            }
            if (!loaded)
                return;
            UpdateImage();
        }

        private void CB_HairColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((CB_HairColor.Items[CB_HairColor.SelectedIndex] as string) == "Custom")
            {
                MTB_HairColor.Enabled = true;
                string hc = MTB_HairColor.Text;
                while (hc.Length < 7)
                    hc = hc + "0";
                hc = hc.Replace("_", "0");
                HAIR_COLOR = Color.FromArgb((int)(0xFF000000 | uint.Parse(hc.Substring(1), NumberStyles.AllowHexSpecifier)));
            }
            else
            {
                MTB_HairColor.Enabled = false;
                HAIR_COLOR = Color.FromArgb((int)(0xFF000000 | (uint)DefaultHairs[CB_HairColor.Items[CB_HairColor.SelectedIndex] as string]));
            }
            if (!loaded)
                return;
            UpdateImage();
        }

        private void CB_Character_SelectedIndexChanged(object sender, EventArgs e)
        {
            CHARACTER = (CB_Character.Items[CB_Character.SelectedIndex] as cbItem).Value as string;
            foreach (Control ctrl in Kamui_Spec)
                ctrl.Enabled = CHARACTER == "username";
            if (CHARACTER == "username")
            {
                if (CB_Kamui.SelectedIndex < 2 && CB_Accessory.SelectedIndex > 0)
                {
                    CB_Accessory.SelectedIndex = 0;
                    return;
                }
                CB_Accessory.Enabled = LBL_Accessory.Enabled = CB_Kamui.SelectedIndex > 1;
            }
            string cname = string.Empty + CHARACTER;
            if (cname == "username")
                cname = "aマイユニ女1";
            CB_Emotion.Items.Clear();
            List<string> NewEmotions = new List<string>();
            foreach (string Resource in ResourceList)
            {
                if (Resource.Contains(cname + "_" + Prefixes[CB_PortraitStyle.SelectedIndex] + "_"))
                {
                    string emotion = Resource.Substring(Resource.IndexOf("_" + Prefixes[CB_PortraitStyle.SelectedIndex] + "_") + ("_" + Prefixes[CB_PortraitStyle.SelectedIndex] + "_").Length);
                    if (!NewEmotions.Contains(emotion) && emotion != "汗" && emotion != "照" && !emotion.Contains("髪")) // Not Sweatdrop, blushing, or a hair overlay
                        NewEmotions.Add(emotion);
                }
            }
            NewEmotions.Sort();
            CB_Emotion.Items.AddRange(NewEmotions.ToArray());
            CB_Emotion.SelectedIndex = 0;
            if (!loaded)
                return;
            UpdateImage();
        }

        private void Emotions_Changed(object sender, EventArgs e)
        {
            EMOTIONS = CB_Emotion.Items[CB_Emotion.SelectedIndex] as string;
            EMOTIONS += ",";
            if (CHK_Blush.Checked)
                EMOTIONS += "照,";
            if (CHK_SweatDrop.Checked)
                EMOTIONS += "汗,";
            if (!loaded)
                return;
            UpdateImage();
        }

        private void UpdateImage()
        {
            if (!loaded)
                return;
            switch (CB_PortraitStyle.Items[CB_PortraitStyle.SelectedIndex] as string)
            {
                case "Standard":
                    PB_Portrait.Image = GetCharacterStageImage(CHARACTER, EMOTIONS, HAIR_COLOR, true);
                    break;
                case "Closeup":
                    PB_Portrait.Image = GetCharacterBUImage(CHARACTER, EMOTIONS, HAIR_COLOR);
                    break;
                case "Critical":
                    PB_Portrait.Image = GetCharacterCriticalImage(CHARACTER, HAIR_COLOR);
                    break;
            }
        }

        private Image GetCharacterCriticalImage(string CName, Color HairColor)
        {
            string hairname = "_ct_髪";
            // string dat_id = "FSID_CT_" + CName;
            bool USER = CName == "username";
            if (USER)
            {
                // dat_id = "FSID_CT_" + (new[] { "マイユニ_男1", "マイユニ_男2", "マイユニ_女1", "マイユニ_女2" })[CB_Kamui.SelectedIndex] + "_顔" + EyeStyles[CB_Eyes.SelectedIndex].ToUpper();
                CName = EyeStyles[CB_Eyes.SelectedIndex] + Kamuis[CB_Kamui.SelectedIndex];
                hairname = CName.Substring(1) + hairname + CB_HairStyle.SelectedIndex;
            }
            else
                hairname = CName + hairname + "0";
            string resname = CName + "_ct_ベース";
            Image C;
            if (ResourceList.Contains(resname))
                C = Resources.ResourceManager.GetObject(resname) as Image;
            else
                C = new Bitmap(1, 1);
            using (Graphics g = Graphics.FromImage(C))
            {
                if (USER && CB_FacialFeature.SelectedIndex > 0)
                {
                    g.DrawImage(Resources.ResourceManager.GetObject((new[] { "マイユニ男1", "マイユニ男2", "マイユニ女1", "マイユニ女2" })[CB_Kamui.SelectedIndex] + "_ct_アクセサリ1_" + CB_FacialFeature.SelectedIndex) as Image, new Point(0, 0));
                }
                if (ResourceList.Contains(hairname))
                {
                    Bitmap hair = Resources.ResourceManager.GetObject(hairname) as Bitmap;
                    Image CHair = ColorHair(hair, HairColor);
                    g.DrawImage(CHair, new Point(0, 0));
                    g.DrawImage(CHair, new Point(0, CHair.Height));
                }
            }
            return C;
        }

        private Image GetCharacterStageImage(string CName, string CEmo, Color HairColor, bool Slot1)
        {
            bool USER = CName == "username";
            string hairname = "_st_髪";
            string dat_id = "FSID_ST_" + CName;
            if (USER)
            {
                dat_id = "FSID_ST_" + (new[] { "マイユニ_男1", "マイユニ_男2", "マイユニ_女1", "マイユニ_女2" })[CB_Kamui.SelectedIndex] + "_顔" + EyeStyles[CB_Eyes.SelectedIndex].ToUpper();
                CName = EyeStyles[CB_Eyes.SelectedIndex] + Kamuis[CB_Kamui.SelectedIndex];
                hairname = CName.Substring(1) + hairname + CB_HairStyle.SelectedIndex;
            }
            else
                hairname = CName + hairname + "0";
            var Emos = CEmo.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string resname = CName + "_st_" + Emos[0];
            Image C;
            if (ResourceList.Contains(resname))
                C = Resources.ResourceManager.GetObject(resname) as Image;
            else
                C = new Bitmap(1, 1);
            using (Graphics g = Graphics.FromImage(C))
            {
                if (USER && CB_FacialFeature.SelectedIndex > 0)
                {
                    g.DrawImage(Resources.ResourceManager.GetObject((new[] { "マイユニ男1", "マイユニ男2", "マイユニ女1", "マイユニ女2" })[CB_Kamui.SelectedIndex] + "_st_アクセサリ1_" + CB_FacialFeature.SelectedIndex) as Image, new Point(0, 0));
                }
                for (int i = 1; i < Emos.Length; i++)
                {
                    string exresname = CName + "_st_" + Emos[i];
                    if (Emos[i] == "汗" && ResourceList.Contains(exresname))
                    {
                        g.DrawImage(Resources.ResourceManager.GetObject(exresname) as Image, new Point(BitConverter.ToUInt16(FaceData[dat_id], 0x40), BitConverter.ToUInt16(FaceData[dat_id], 0x42)));
                    }
                    else if (Emos[i] == "照" && ResourceList.Contains(exresname))
                    {
                        g.DrawImage(Resources.ResourceManager.GetObject(exresname) as Image, new Point(BitConverter.ToUInt16(FaceData[dat_id], 0x38), BitConverter.ToUInt16(FaceData[dat_id], 0x3A)));
                    }
                }
                if (ResourceList.Contains(hairname))
                {
                    Bitmap hair = Resources.ResourceManager.GetObject(hairname) as Bitmap;
                    g.DrawImage(ColorHair(hair, HairColor), new Point(0, 0));
                }
                if (USER && CB_Accessory.SelectedIndex > 0)
                {
                    g.DrawImage(Resources.ResourceManager.GetObject((new[] { "マイユニ男1", "マイユニ男2", "マイユニ女1", "マイユニ女2" })[CB_Kamui.SelectedIndex] + "_st_アクセサリ2_" + CB_Accessory.SelectedIndex) as Image, new Point(133, 28));
                }
            }
            if (Slot1)
                C.RotateFlip(RotateFlipType.RotateNoneFlipX);
            return C;
        }

        private Image GetCharacterBUImage(string CName, string CEmo, Color HairColor)
        {
            bool USER = CName == "username";
            string hairname = "_bu_髪";
            string dat_id = "FSID_BU_" + CName;
            if (USER)
            {
                dat_id = "FSID_BU_" + (new[] { "マイユニ_男1", "マイユニ_男2", "マイユニ_女1", "マイユニ_女2" })[CB_Kamui.SelectedIndex] + "_顔" + EyeStyles[CB_Eyes.SelectedIndex].ToUpper();
                CName = EyeStyles[CB_Eyes.SelectedIndex] + Kamuis[CB_Kamui.SelectedIndex];
                hairname = CName.Substring(1) + hairname + CB_HairStyle.SelectedIndex;
            }
            else
                hairname = CName + hairname + "0";
            var Emos = CEmo.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string resname = CName + "_bu_" + Emos[0];
            Image C;
            if (ResourceList.Contains(resname))
                C = Resources.ResourceManager.GetObject(resname) as Image;
            else
                C = new Bitmap(1, 1);
            using (Graphics g = Graphics.FromImage(C))
            {
                if (USER && CB_FacialFeature.SelectedIndex > 0)
                {
                    g.DrawImage(Resources.ResourceManager.GetObject((new[] { "マイユニ男1", "マイユニ男2", "マイユニ女1", "マイユニ女2" })[CB_Kamui.SelectedIndex] + "_bu_アクセサリ1_" + CB_FacialFeature.SelectedIndex) as Image, new Point(0, 0));
                }
                for (int i = 1; i < Emos.Length; i++)
                {
                    string exresname = CName + "_bu_" + Emos[i];
                    if (Emos[i] == "汗" && ResourceList.Contains(exresname))
                    {
                        g.DrawImage(Resources.ResourceManager.GetObject(exresname) as Image, new Point(BitConverter.ToUInt16(FaceData[dat_id], 0x40), BitConverter.ToUInt16(FaceData[dat_id], 0x42)));
                    }
                    else if (Emos[i] == "照" && ResourceList.Contains(exresname))
                    {
                        g.DrawImage(Resources.ResourceManager.GetObject(exresname) as Image, new Point(BitConverter.ToUInt16(FaceData[dat_id], 0x38), BitConverter.ToUInt16(FaceData[dat_id], 0x3A)));
                    }
                }
                if (ResourceList.Contains(hairname))
                {
                    Bitmap hair = Resources.ResourceManager.GetObject(hairname) as Bitmap;
                    g.DrawImage(ColorHair(hair, HairColor), new Point(0, 0));
                }
                if (USER && CB_Accessory.SelectedIndex > 0)
                {
                    Point Acc = new[] { new Point(66, 5), new Point(65, 21) }[CB_Kamui.SelectedIndex - 2];
                    g.DrawImage(Resources.ResourceManager.GetObject((new[] { "マイユニ男1", "マイユニ男2", "マイユニ女1", "マイユニ女2" })[CB_Kamui.SelectedIndex] + "_bu_アクセサリ2_" + CB_Accessory.SelectedIndex) as Image, Acc);
                }
            }
            C.RotateFlip(RotateFlipType.RotateNoneFlipX);
            return C;
        }

        private Image ColorHair(Image Hair, Color C)
        {
            Bitmap bmp = Hair as Bitmap;
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);

            IntPtr ptr = bmpData.Scan0;

            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbaValues = new byte[bytes];

            // Copy the RGB values into the array.
            Marshal.Copy(ptr, rgbaValues, 0, bytes);

            for (int i = 0; i < rgbaValues.Length; i += 4)
            {
                if (rgbaValues[i + 3] > 0)
                {
                    rgbaValues[i + 2] = BlendOverlay(C.R, rgbaValues[i + 2]);
                    rgbaValues[i + 1] = BlendOverlay(C.G, rgbaValues[i + 1]);
                    rgbaValues[i + 0] = BlendOverlay(C.B, rgbaValues[i + 0]);
                }
            }
            // Copy the RGB values back to the bitmap
            Marshal.Copy(rgbaValues, 0, ptr, bytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);
            return bmp;
        }

        private static byte BlendOverlay(byte Src, byte Dst)
        {
            return ((Dst < 128) ? (byte)Math.Max(Math.Min((Src / 255.0f * Dst / 255.0f) * 255.0f * 2, 255), 0) : (byte)Math.Max(Math.Min(255 - ((255 - Src) / 255.0f * (255 - Dst) / 255.0f) * 255.0f * 2, 255), 0));
        }

        private void Kamui_Data_Changed(object sender, EventArgs e)
        {
            if (!loaded)
                return;
            if (CB_Kamui.SelectedIndex < 2 && CB_Accessory.SelectedIndex > 0)
            {
                CB_Accessory.SelectedIndex = 0;
                return;
            }
            CB_Accessory.Enabled = LBL_Accessory.Enabled = CB_Kamui.SelectedIndex > 1;
            UpdateImage();
        }

        private void PB_Portrait_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            string DefaultName = string.Empty;
            string hairname = "_ct_髪";
            if (CHARACTER == "username")
            {
                hairname = (EyeStyles[CB_Eyes.SelectedIndex] + Kamuis[CB_Kamui.SelectedIndex]).Substring(1) + hairname + CB_HairStyle.SelectedIndex;
            }
            else
                hairname = CHARACTER + hairname + "0";
            if (ResourceList.Contains(hairname))
            {
                DefaultName += CB_HairColor.Items[CB_HairColor.SelectedIndex] as string;
                if (DefaultName == "Custom")
                    DefaultName += " (" + MTB_HairColor.Text.Substring(1) + ")";
                DefaultName += "_";
            }
            DefaultName +=  (CB_Character.Items[CB_Character.SelectedIndex] as cbItem).Text;
            DefaultName += "_" + CB_PortraitStyle.Items[CB_PortraitStyle.SelectedIndex];
            if (CB_PortraitStyle.Items[CB_PortraitStyle.SelectedIndex] as string != "Critical")
                DefaultName += "_" + string.Join("-", EMOTIONS.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries));
            DefaultName += ".png";
            sfd.FileName = DefaultName;
            if (sfd.ShowDialog() != DialogResult.OK)
                return;
            PB_Portrait.Image.Save(sfd.FileName, ImageFormat.Png);
        }
    }

    public class cbItem
    {
        public string Text { get; set; }
        public object Value { get; set; }
    }
}
