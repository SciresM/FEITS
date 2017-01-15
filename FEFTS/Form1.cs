using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using FEFTS.Properties;

namespace FEFTS
{
    public partial class Form1 : Form
    {
        private Image[] Images = { Resources.Awakening_0, Resources.Awakening_1 };

        private bool[] ValidCharacters;
        private FontCharacter[] Characters;
        private Dictionary<string, string> Names;
        private Dictionary<string, byte[]> FaceData;

        private string PLAYER_NAME = "Corrin";
        private string RAW_MESSAGE = "$t1$Wmキヌ|3$w0|$Wsキヌ|$Wa$E差分,|This is an example conversation.$k$p$Wmクロム左|7$w0|$Wsクロム左|$Wa$E笑,|It takes place between Selkie and Chrom.$k";
        private bool HAS_PERMS;
        private bool SET_TYPE;

        private int CUR_INDEX;
        private List<string> Messages = new List<string>();
        private string CHAR_A = string.Empty;
        private string CHAR_B = string.Empty;
        private string CHAR_ACTIVE = string.Empty;
        private const string DEFAULT_EMOTION = "通常,"; // 通常
        private string EMOTION_A = DEFAULT_EMOTION;
        private string EMOTION_B = DEFAULT_EMOTION;
        private Color COLOR_A = Color.FromArgb(0x5B, 0x58, 0x55);
        private Color COLOR_B = Color.FromArgb(0x5B, 0x58, 0x55);
        private ConversationTypes CONVERSATION_TYPE = ConversationTypes.TYPE_1;
        private Image BACKGROUND_IMAGE;


        private string[] EyeStyles = { "a", "b", "c", "d", "e", "f", "g" };
        private string[] Corrins = { "マイユニ男1", "マイユニ男2", "マイユニ女1", "マイユニ女2" };

        private Image[] TextBoxes = { Resources.TextBox, Resources.TextBox_Nohr, Resources.TextBox_Hoshido };

        List<string> ResourceList = new List<string>();

        private enum ConversationTypes
        {
            TYPE_0,
            TYPE_1
        }

        public Form1()
        {
            InitializeComponent();
            ValidCharacters = new bool[0x10000];
            Characters = new FontCharacter[0x10000];
            for (int i = 0; i < Resources.chars.Length / 0x10; i++)
            {
                FontCharacter fc = new FontCharacter(Resources.chars, i * 0x10);
                ValidCharacters[fc.Value] = true;
                fc.SetGlyph(Images[fc.IMG]);
                Characters[fc.Value] = fc;
            }
            Names = new Dictionary<string, string>();
            string[] PIDs = Resources.PID.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string PID in PIDs)
            {
                var P = PID.Split(new[] { '\t' });
                Names[P[0]] = P[1];
            }
            FaceData = new Dictionary<string, byte[]>();
            string[] FIDs = Resources.FID.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < FIDs.Length; i++)
            {
                byte[] Dat = new byte[0x48];
                Array.Copy(Resources.faces, i * 0x48, Dat, 0, 0x48);
                FaceData[FIDs[i]] = Dat;
            }
            ResourceSet set = Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentCulture, true, true);
            foreach (DictionaryEntry o in set)
                ResourceList.Add(o.Key as string);
            Resources.ResourceManager.ReleaseAllResources();

            CB_Corrin.Items.AddRange(new[] { "Male 1", "Male 2", "Female 1", "Female 2" });
            CB_Eyes.Items.AddRange(new[] { "Style A", "Style B", "Style C", "Style D", "Style E", "Style F", "Style G" });
            CB_TB.Items.AddRange(new[] { "Standard", "Nohrian", "Hoshidan" });
            CB_HairStyle.Items.AddRange(Enumerable.Range(0, 12).Select(i => i.ToString("00")).ToArray());
            CB_FacialFeature.Items.AddRange(new[]{"None", "Scratches", "Vertical Scratches", "Horizontal Scratches", "Tattoo 1", "Tattoo 2", "Tattoo 3", "Eye Mole", "Mouth Mole", "Plaster 1", "Plaster 2", "White Eyepatch", "Black Eyepatch"});
            CB_Accessory.Items.AddRange(new[] { "None", "Silver Piece", "Butterfly", "Black Ribbon", "White Ribbon", "White Rose"});

            CB_Corrin.SelectedIndex = CB_TB.SelectedIndex = CB_Eyes.SelectedIndex = CB_HairStyle.SelectedIndex = CB_FacialFeature.SelectedIndex = CB_Accessory.SelectedIndex = 0;

            MTB_HairColorA.Text = MTB_HairColorB.Text = "#F6F4EF";
            BACKGROUND_IMAGE = Resources.SupportBG.Clone() as Bitmap;

            PB_TextBox.AllowDrop = true;
            PB_TextBox.DragEnter += PB_TextBox_DragEnter;
            PB_TextBox.DragDrop += PB_TextBox_DragDrop;
            B_Reload_Click(null, null);
        }

        private void PB_TextBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void PB_TextBox_DragDrop(object sender, DragEventArgs e)
        {
            string file = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
            if (File.Exists(file))
            {
                try
                {
                    Image img = Image.FromFile(file);
                    if (img.Width > 1 && img.Height > 1)
                    {
                        BACKGROUND_IMAGE = img;
                        B_Reload_Click(sender, e);
                    }
                }
                catch
                {
                    // Do nothing
                }
            }
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
            RAW_MESSAGE = RTB_Line.Text;
        }

        private void B_Reload_Click(object sender, EventArgs e)
        {
            if (LBL_Warning.Visible)
                return;
            Messages = ParseMessages(RAW_MESSAGE);
            CUR_INDEX = 0;
            B_Next.Enabled = Messages.Count > 1;
            B_Prev.Enabled = false;
            ResetParameters();
            UpdateBox();
            
        }

        private void UpdateBox()
        {
            string CUR_MSG = Messages[CUR_INDEX];
            CUR_MSG = UpdateParse(CUR_MSG);
            PB_TextBox.Image = RenderBox(CUR_MSG);
        }

        private string UpdateParse(string MSG)
        {
            for (int i = 0; i < MSG.Length; i++)
            {
                if (MSG[i] == '$')
                {
                    Tuple<string, Command> res = ParseCommand(MSG, i);
                    MSG = res.Item1;
                    if (res.Item2.NumParams > 0)
                        if (res.Item2.Params[0] == "ベロア")
                            res.Item2.Params[0] = "べロア"; // Velouria Fix
                    switch (res.Item2.CMD)
                    {
                        case "$E":
                            if (CHAR_ACTIVE != string.Empty && CHAR_ACTIVE == CHAR_B)
                            {
                                EMOTION_B = res.Item2.Params[0];
                            }
                            else
                            {
                                EMOTION_A  = res.Item2.Params[0];
                            }
                            break;
                        case "$Ws":
                            CHAR_ACTIVE = res.Item2.Params[0];
                            break;
                        case "$Wm":
                            if (CONVERSATION_TYPE == ConversationTypes.TYPE_1)
                            {
                                if (res.Item2.Params[1] == "3")
                                {
                                    CHAR_A = res.Item2.Params[0];
                                    EMOTION_A = DEFAULT_EMOTION;
                                }
                                else if (res.Item2.Params[1] == "7")
                                {
                                    CHAR_B = res.Item2.Params[0];
                                    EMOTION_B = DEFAULT_EMOTION;
                                }
                            }
                            else
                            {
                                if (res.Item2.Params[1] == "0" || res.Item2.Params[1] == "2")
                                {
                                    CHAR_A = res.Item2.Params[0];
                                    EMOTION_A = DEFAULT_EMOTION;
                                }
                                else if (res.Item2.Params[1] == "6")
                                {
                                    CHAR_B = res.Item2.Params[0];
                                    EMOTION_B = DEFAULT_EMOTION;
                                }
                            }
                            break;
                        case "$Wd":
                            if (CHAR_ACTIVE == CHAR_B)
                            {
                                CHAR_ACTIVE = CHAR_A;
                                CHAR_B = string.Empty;
                            }
                            else
                                CHAR_A = string.Empty;
                            break;
                        case "$a":
                            HAS_PERMS = true;
                            break;
                        case "$t0":
                            if (!SET_TYPE)
                                CONVERSATION_TYPE = ConversationTypes.TYPE_0;
                            SET_TYPE = true;
                            break;
                        case "$t1":
                            if (!SET_TYPE)
                                CONVERSATION_TYPE = ConversationTypes.TYPE_1;
                            SET_TYPE = true;
                            break;
                        case "$Nu":
                            if (HAS_PERMS)
                            {
                                MSG = MSG.Substring(0, i) + PLAYER_NAME + MSG.Substring(i);
                            }
                            else
                            {
                                MSG = MSG.Substring(0, i) + "$Nu" + MSG.Substring(i);
                                i += 2;
                            }
                            break;
                        case "$Wa":
                        case "$Wc":
                        default:
                            break;
                    }
                    i--;
                }
            }

            MSG = MSG.Replace("\\n", "\n");
            return MSG;
        }

        private string Parse(string MSG)
        {
            for (int i = 0; i < MSG.Length; i++)
            {
                if (MSG[i] == '$')
                {
                    Tuple<string, Command> res = ParseCommand(MSG, i);
                    MSG = res.Item1;
                    switch (res.Item2.CMD)
                    {
                        case "$E":
                        case "$Ws":
                        case "$Wm":
                        case "$Wd":
                        case "$a":
                        case "$t0":
                        case "$t1":
                        case "$Nu":
                        case "$Wa":
                        case "$Wc":
                        default:
                            break;
                    }
                    i--;
                }
            }

            MSG = MSG.Replace("\\n", "\n");
            return MSG;
        }

        private Tuple<string, Command> ParseCommand(string Message, int Offset)
        {
            string trunc = Message.Substring(Offset);
            string[] NoParams = { "$Wa", "$Wc", "$a", "$Nu", "$N0", "$N1", "$t0", "$t1", "$k", "$p" };
            string[] SingleParams = { "$E", "$Sbs", "$Svp", "$Sre", "$Fw", "$Ws", "$VF", "$Ssp", "$Fo", "$VNMPID", "$Fi", "$b", "$Wd", "$w", "$l" };
            string[] DoubleParams = { "$Wm", "$Sbv", "$Sbp", "$Sls", "$Slp" };
            Command CMD = new Command();
            foreach (string delim in NoParams)
            {
                if (trunc.StartsWith(delim))
                {
                    CMD.CMD = delim;
                    CMD.NumParams = 0;
                    CMD.Params = new string[CMD.NumParams];
                    Message = Message.Substring(0, Offset) + Message.Substring(Offset + delim.Length);
                }
            }
            foreach (string delim in SingleParams)
            {
                if (trunc.StartsWith(delim))
                {
                    CMD.CMD = delim;
                    CMD.NumParams = 1;
                    CMD.Params = new string[CMD.NumParams];
                    int ind = Message.IndexOf("|", Offset);
                    CMD.Params[0] = Message.Substring(Offset + delim.Length, ind - (Offset + delim.Length));
                    Message = Message.Substring(0, Offset) + Message.Substring(ind + 1);
                }
            }
            foreach (string delim in DoubleParams)
            {
                if (trunc.StartsWith(delim))
                {
                    CMD.CMD = delim;
                    CMD.NumParams = 2;
                    CMD.Params = new string[CMD.NumParams];
                    int ind = Message.IndexOf("|", Offset);
                    int ind2 = Message.IndexOf("|", ind + 1);
                    if (delim == "$Wm")
                    {
                        CMD.Params[0] = Message.Substring(Offset + delim.Length, ind - (Offset + delim.Length));
                        CMD.Params[1] = Message.Substring(ind + 1, 1);
                        Message = Message.Substring(0, Offset) + Message.Substring(ind + 2);
                    }
                    else
                    {
                        CMD.Params[0] = Message.Substring(Offset + delim.Length, ind - (Offset + delim.Length));
                        CMD.Params[1] = Message.Substring(ind + 1, ind2 - (ind + 1));
                        Message = Message.Substring(0, Offset) + Message.Substring(ind2 + 1);
                    }
                }
            }
            Tuple<string, Command> ret = new Tuple<string, Command>(Message, CMD);
            return ret;
        }

        private List<string> ParseMessages(string RAW)
        {
            List<string> MSGs = new List<string>();
            var splits = RAW.Split(new[] { "$k$p", "$k\\n" }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.TrimStart(' '));

            MSGs.AddRange(splits.Where(s => (!string.IsNullOrWhiteSpace(Parse(s)) && !string.IsNullOrEmpty(Parse(s)))));
            return MSGs;
        }

        private Image RenderBox(string Message)
        {
            return CONVERSATION_TYPE == ConversationTypes.TYPE_1 ? RenderTypeOne(Message) : RenderTypeZero();
        }

        private Image RenderTypeOne(string Message)
        {
            Bitmap Box = new Bitmap(PB_TextBox.Width, PB_TextBox.Height);
            Bitmap TB = TextBoxes[CB_TB.SelectedIndex].Clone() as Bitmap;
            Bitmap Text = DrawString(new Bitmap(312,50), Message, 0,22, Color.FromArgb(68, 8, 0)) as Bitmap;
            using (Graphics g = Graphics.FromImage(TB))
                g.DrawImage(Text, new Point(29, 0));
            string Name = Names.ContainsKey(CHAR_ACTIVE) ? Names[CHAR_ACTIVE] : (CHAR_ACTIVE == "username" ? PLAYER_NAME : CHAR_ACTIVE);
            int NameLen = GetLength(Name);
            Bitmap NB = DrawString(Resources.NameBox, Name, Resources.NameBox.Width / 2 - NameLen / 2, 16, Color.FromArgb(253, 234, 177)) as Bitmap; // Center Name in NameBox
            using (Graphics g = Graphics.FromImage(Box))
            {
                if (CHK_UseBackgrounds.Checked)
                    g.DrawImage(BACKGROUND_IMAGE, new Point(0, 0));
                if (CHAR_A != string.Empty)
                {
                    Image CA = GetCharacterStageImage(CHAR_A, EMOTION_A, COLOR_A, true);
                    g.DrawImage((CHAR_ACTIVE == CHAR_A) ? CA : Fade(CA), new Point(-28, Box.Height - CA.Height + 14));
                }
                if (CHAR_B != string.Empty)
                {
                    Image CB = GetCharacterStageImage(CHAR_B, EMOTION_B, COLOR_B, false);
                    g.DrawImage((CHAR_ACTIVE == CHAR_B) ? CB : Fade(CB), new Point(Box.Width - CB.Width + 28, Box.Height - CB.Height + 14));
                }
                g.DrawImage(TB, new Point(10, Box.Height - TB.Height + 2));
                if (CHAR_ACTIVE != string.Empty)
                {
                    g.DrawImage(NB,
                        CHAR_ACTIVE == CHAR_B
                            ? new Point(Box.Width - NB.Width - 6, Box.Height - TB.Height - 14)
                            : new Point(7, Box.Height - TB.Height - 14));
                }
                if (CUR_INDEX < Messages.Count - 1)
                {
                    g.DrawImage(Resources.KeyPress, new Point(Box.Width - 33 , Box.Height - TB.Height + 32));
                }
            }
            return Box;
        }

        private Image RenderTypeZero()
        {
            string TopMessage = string.Empty, BottomMessage = string.Empty;
            ResetParameters();
            for (int i = 0; i <= CUR_INDEX; i++)
            {
                string MSG = UpdateParse(Messages[i]);
                if (CHAR_ACTIVE == CHAR_A)
                    TopMessage = MSG;
                else
                    BottomMessage = MSG;
            }
            Bitmap Box = new Bitmap(PB_TextBox.Width, PB_TextBox.Height);
            Bitmap TopBox = new Bitmap(1, 1), BottomBox = new Bitmap(1,1);
            if (TopMessage != string.Empty && CHAR_A != string.Empty)
            {
                TopBox = (TextBoxes[CB_TB.SelectedIndex].Clone()) as Bitmap;
                using (Graphics g = Graphics.FromImage(TopBox))
                {
                    g.DrawImage(GetCharacterBUImage(CHAR_A, EMOTION_A, COLOR_A, true), new Point(2, 3));
                    g.DrawImage(DrawString(new Bitmap(282, 50), TopMessage, 0, 22, Color.FromArgb(68, 8, 0)), new Point(76, 0));
                }
            }
            if (BottomMessage != string.Empty && CHAR_B != string.Empty)
            {
                BottomBox = (TextBoxes[CB_TB.SelectedIndex].Clone()) as Bitmap;
                using (Graphics g = Graphics.FromImage(BottomBox))
                {
                    g.DrawImage(GetCharacterBUImage(CHAR_B, EMOTION_B, COLOR_B, true), new Point(2, 3));
                    g.DrawImage(DrawString(new Bitmap(282, 50), BottomMessage, 0, 22, Color.FromArgb(68, 8, 0)), new Point(76, 0));
                }
            }
            using (Graphics g = Graphics.FromImage(Box))
            {
                if (CUR_INDEX < Messages.Count - 1)
                {
                    using (Graphics g2 = Graphics.FromImage(CHAR_ACTIVE == CHAR_A ? TopBox : BottomBox))
                    {
                        g2.DrawImage(Resources.KeyPress, new Point(TextBoxes[CB_TB.SelectedIndex].Width - 30, 32));
                    }
                }
                if (CHK_UseBackgrounds.Checked)
                    g.DrawImage(BACKGROUND_IMAGE, new Point(0, 0));
                g.DrawImage(TopBox, new Point(10, 3));
                g.DrawImage(BottomBox, new Point(10, Box.Height - BottomBox.Height + 2));
                if (TopMessage != string.Empty && CHAR_A != string.Empty)
                {
                    string TopName = Names.ContainsKey(CHAR_A) ? Names[CHAR_A] : (CHAR_A == "username" ? PLAYER_NAME : CHAR_A);
                    int NameLen = GetLength(TopName);
                    Bitmap TopNameBox = DrawString(Resources.NameBox, TopName, Resources.NameBox.Width / 2 - NameLen / 2, 16, Color.FromArgb(253, 234, 177)) as Bitmap; // Center Name in NameBox
                    g.DrawImage(TopNameBox, new Point(7, TopBox.Height - (TopNameBox.Height - 20)));
                }
                if (BottomMessage != string.Empty && CHAR_B != string.Empty)
                {
                    string BottomName = Names.ContainsKey(CHAR_B) ? Names[CHAR_B] : (CHAR_B == "username" ? PLAYER_NAME : CHAR_B);
                    int NameLen = GetLength(BottomName);
                    Bitmap BottomNameBox = DrawString(Resources.NameBox, BottomName, Resources.NameBox.Width / 2 - NameLen / 2, 16, Color.FromArgb(253, 234, 177)) as Bitmap; // Center Name in NameBox
                    g.DrawImage(BottomNameBox, new Point(7, Box.Height - BottomBox.Height - 14));
                }
            }
            return Box;
        }

        private Image GetCharacterCriticalImage(string CName, Color HairColor)
        {
            bool USER = CName == "username";
            string hairname = "_ct_髪";
            string dat_id = "FSID_CT_" + CName;
            if (USER)
            {
                dat_id = "FSID_CT_" + (new[] { "マイユニ_男1", "マイユニ_男2", "マイユニ_女1", "マイユニ_女2" })[CB_Corrin.SelectedIndex] + "_顔" + EyeStyles[CB_Eyes.SelectedIndex].ToUpper();
                CName = EyeStyles[CB_Eyes.SelectedIndex] + Corrins[CB_Corrin.SelectedIndex];
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
                    g.DrawImage(Resources.ResourceManager.GetObject((new[] { "マイユニ男1", "マイユニ男2", "マイユニ女1", "マイユニ女2" })[CB_Corrin.SelectedIndex] + "_ct_アクセサリ1_" + CB_FacialFeature.SelectedIndex) as Image, new Point(0, 0));
                }
                if (ResourceList.Contains(hairname))
                {
                    Bitmap hair = Resources.ResourceManager.GetObject(hairname) as Bitmap;
                    Image CHair = ColorHair(hair, HairColor);
                    g.DrawImage(CHair, new Point(0,0));
                    g.DrawImage(CHair, new Point(0,CHair.Height));
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
                dat_id = "FSID_ST_" + (new[] { "マイユニ_男1", "マイユニ_男2", "マイユニ_女1", "マイユニ_女2" })[CB_Corrin.SelectedIndex] + "_顔" + EyeStyles[CB_Eyes.SelectedIndex].ToUpper();
                CName = EyeStyles[CB_Eyes.SelectedIndex] + Corrins[CB_Corrin.SelectedIndex];
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
                    g.DrawImage(Resources.ResourceManager.GetObject((new[] { "マイユニ男1", "マイユニ男2", "マイユニ女1", "マイユニ女2" })[CB_Corrin.SelectedIndex] + "_st_アクセサリ1_" + CB_FacialFeature.SelectedIndex) as Image, new Point(0, 0));
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
                    g.DrawImage(Resources.ResourceManager.GetObject((new[] { "マイユニ男1", "マイユニ男2", "マイユニ女1", "マイユニ女2" })[CB_Corrin.SelectedIndex] + "_st_アクセサリ2_" + CB_Accessory.SelectedIndex) as Image, new Point(133, 28));
                }
            }
            if (Slot1)
                C.RotateFlip(RotateFlipType.RotateNoneFlipX);
            return C;
        }

        private Image GetCharacterBUImage(string CName, string CEmo, Color HairColor, bool Crop)
        {
            string hairname = "_bu_髪";
            string dat_id = "FSID_BU_" + CName;
            bool USER = CName == "username";
            if (USER)
            {
                dat_id = "FSID_BU_" + (new[] { "マイユニ_男1", "マイユニ_男2", "マイユニ_女1", "マイユニ_女2" })[CB_Corrin.SelectedIndex] + "_顔" + EyeStyles[CB_Eyes.SelectedIndex].ToUpper();
                CName = EyeStyles[CB_Eyes.SelectedIndex] + Corrins[CB_Corrin.SelectedIndex];
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
                    g.DrawImage(Resources.ResourceManager.GetObject((new[] { "マイユニ男1", "マイユニ男2", "マイユニ女1", "マイユニ女2" })[CB_Corrin.SelectedIndex] + "_bu_アクセサリ1_" + CB_FacialFeature.SelectedIndex) as Image, new Point(0, 0));
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
                    Point Acc = new[] { new Point(66, 5), new Point(65, 21) }[CB_Corrin.SelectedIndex - 2];
                    g.DrawImage(Resources.ResourceManager.GetObject((new[] { "マイユニ男1", "マイユニ男2", "マイユニ女1", "マイユニ女2" })[CB_Corrin.SelectedIndex] + "_bu_アクセサリ2_" + CB_Accessory.SelectedIndex) as Image, Acc);
                }
            }
            if (Crop)
            {
                Bitmap Cropped = new Bitmap(BitConverter.ToUInt16(FaceData[dat_id], 0x34), BitConverter.ToUInt16(FaceData[dat_id], 0x36));
                using (Graphics g = Graphics.FromImage(Cropped))
                {
                    g.DrawImage(C, new Point(-BitConverter.ToUInt16(FaceData[dat_id], 0x30), -BitConverter.ToUInt16(FaceData[dat_id], 0x32)));
                }
                C = Cropped;
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

        private Image Fade(Image BaseImage)
        {

            Bitmap bmp = BaseImage as Bitmap;
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);

            IntPtr ptr = bmpData.Scan0;

            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbaValues = new byte[bytes];

            // Copy the RGB values into the array.
            Marshal.Copy(ptr, rgbaValues, 0, bytes);

            const double BLACK_A = 113.0 / 255.0;

            for (int i = 0; i < rgbaValues.Length; i += 4)
            {
                if (rgbaValues[i + 3] <= 0) continue;
                double DST_A = rgbaValues[i + 3] / 255.0;
                // double FINAL_A = BLACK_A + (DST_A) * (1.0 - BLACK_A);
                // rgbaValues[i + 3] = (byte)Math.Round((FINAL_A) * 255.0);
                rgbaValues[i + 2] = (byte)Math.Round((((rgbaValues[i + 2] / 255.0)) * (DST_A) * (1.0 - BLACK_A)) * 255.0);
                rgbaValues[i + 1] = (byte)Math.Round((((rgbaValues[i + 1] / 255.0)) * (DST_A) * (1.0 - BLACK_A)) * 255.0);
                rgbaValues[i + 0] = (byte)Math.Round((((rgbaValues[i + 0] / 255.0)) * (DST_A) * (1.0 - BLACK_A)) * 255.0);
            }
            // Copy the RGB values back to the bitmap
            Marshal.Copy(rgbaValues, 0, ptr, bytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);
            return bmp;
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

        private int GetLength(string s)
        {
            return s.Select(GetValue).Select(val => Math.Max(Characters[val].Width, Characters[val].CropWidth)).Sum();
        }

        private void TB_CharName_TextChanged(object sender, EventArgs e)
        {
            string Fixed = TB_CharName.Text;
            for (int i = 0; i < Fixed.Length; i++)
            {
                if (!ValidCharacters[GetValue(Fixed[i])])
                {
                    Fixed = Fixed.Substring(0, i) + Fixed.Substring(i + 1, Fixed.Length - (i + 1));
                    i--;
                }
            }
            while (GetLength(Fixed) >= 0x62 || Fixed.Length > 16)
            {
                Fixed = Fixed.Substring(0, Fixed.Length - 1);
            }
            if (TB_CharName.Text != Fixed)
                TB_CharName.Text = Fixed;
            PLAYER_NAME = TB_CharName.Text;
        }

        private void B_Prev_Click(object sender, EventArgs e)
        {
            if (CUR_INDEX > 0)
                CUR_INDEX--;
            ResetParameters();
            for (int i = 0; i < CUR_INDEX; i++)
            {
                UpdateParse(Messages[i]);
            }
            B_Prev.Enabled = CUR_INDEX > 0;
            B_Next.Enabled = CUR_INDEX < Messages.Count - 1;
            UpdateBox();
        }

        private void B_Next_Click(object sender, EventArgs e)
        {
            if (CUR_INDEX < Messages.Count - 1)
                CUR_INDEX++;
            B_Prev.Enabled = CUR_INDEX > 0;
            B_Next.Enabled = CUR_INDEX < Messages.Count - 1;
            UpdateBox();
        }

        private void PB_TextBox_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            Image ToSave;
            if ((e as MouseEventArgs).Button == MouseButtons.Right)
            {
                if (Prompt(MessageBoxButtons.YesNo, "Save the current conversation?") != DialogResult.Yes)
                    return;
                sfd.FileName = "FE_Conversation.png";
                List<Image> Images = new List<Image>();
                int stored = CUR_INDEX;
                ResetParameters();
                for (CUR_INDEX = 0; CUR_INDEX < Messages.Count; CUR_INDEX++)
                {
                    string parsed = UpdateParse(Messages[CUR_INDEX]);                   
                    if (!string.IsNullOrWhiteSpace(parsed) && !string.IsNullOrEmpty(parsed))
                    {
                        Images.Add(RenderBox(parsed));
                    }
                }
                CUR_INDEX = stored;
                ResetParameters();
                for (int i = 0; i <= CUR_INDEX; i++)
                {
                    UpdateParse(Messages[i]);
                }
                Bitmap bmp = new Bitmap(Images.Max(i => i.Width), Images.Sum(i => i.Height));
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    int h = 0;
                    foreach (Image img in Images)
                    {
                        g.DrawImage(img, new Point(0, h));
                        h += img.Height;
                    }
                }
                ToSave = bmp;
            }
            else
            {
                if (Prompt(MessageBoxButtons.YesNo, "Save the current image?") != DialogResult.Yes)
                    return;
                ToSave = PB_TextBox.Image;
                sfd.FileName = "FE_Conversation_" + CUR_INDEX + ".png";           
            }
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                ToSave.Save(sfd.FileName, ImageFormat.Png);
            }
        }

        internal static DialogResult Prompt(MessageBoxButtons btn, params string[] lines)
        {
            SystemSounds.Question.Play();
            string msg = String.Join(Environment.NewLine + Environment.NewLine, lines);
            return MessageBox.Show(msg, "Prompt", btn, MessageBoxIcon.Asterisk);
        }

        private void MTB_HairColorA_TextChanged(object sender, EventArgs e)
        {
            string hc = MTB_HairColorA.Text.ToUpper();
            for (int i = 1; i < hc.Length; i++)
            {
                if (!((hc[i] >= 'A' && hc[i] <= 'F') || (hc[i] >= '0' && hc[i] <= '9')))
                {
                    hc = hc.Substring(0, i) + hc.Substring(i + 1);
                }
            }
            if (hc != MTB_HairColorA.Text)
                MTB_HairColorA.Text = hc;
            else
            {
                while (hc.Length < 7)
                    hc = hc + "0";
                hc = hc.Replace("_", "0");
                COLOR_A = Color.FromArgb((int)(0xFF000000 | uint.Parse(hc.Substring(1), NumberStyles.AllowHexSpecifier)));
            }
        }

        private void MTB_HairColorB_TextChanged(object sender, EventArgs e)
        {
            string hc = MTB_HairColorB.Text.ToUpper();
            for (int i = 1; i < hc.Length; i++)
            {
                if (!((hc[i] >= 'A' && hc[i] <= 'F') || (hc[i] >= '0' && hc[i] <= '9')))
                {
                    hc = hc.Substring(0, i) + hc.Substring(i + 1);
                }
            }
            if (hc != MTB_HairColorB.Text)
                MTB_HairColorB.Text = hc;
            else
            {
                while (hc.Length < 7)
                    hc = hc + "0";
                hc = hc.Replace("_", "0");
                COLOR_B = Color.FromArgb((int)(0xFF000000 | uint.Parse(hc.Substring(1), NumberStyles.AllowHexSpecifier)));
            }
        }

        private void B_PortraitGeneration_Click(object sender, EventArgs e)
        {
            (new PortraitGenerator(ResourceList, Names, FaceData)).ShowDialog();
        }

        private void B_LvlUpTester_Click(object sender, EventArgs e)
        {
            (new HalfBoxTester(ValidCharacters, Characters)).ShowDialog();
        }

        private void ResetParameters()
        {
            CHAR_A = CHAR_B = CHAR_ACTIVE = string.Empty;
            HAS_PERMS = SET_TYPE = false;
            EMOTION_A = EMOTION_B = DEFAULT_EMOTION;
            CONVERSATION_TYPE = ConversationTypes.TYPE_1;
        }

        private void CHK_UseBackgrounds_CheckedChanged(object sender, EventArgs e)
        {
            if (CHK_UseBackgrounds.Checked)
            {
                BACKGROUND_IMAGE = Resources.SupportBG.Clone() as Bitmap;
                B_Reload_Click(sender, e);
                MessageBox.Show("Backgrounds enabled." + Environment.NewLine + "Drag/drop an image onto the Picture Box to change the background." + Environment.NewLine + "Uncheck and Recheck this box to reset the background to the default one.", "Alert");
            }
            else
            {
                B_Reload_Click(sender, e);
            }
        }

        private void CB_Corrin_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CB_Corrin.SelectedIndex < 2 && CB_Accessory.SelectedIndex > 0)
            {
                    CB_Accessory.SelectedIndex = 0;
            }
            CB_Accessory.Enabled = LBL_Accessory.Enabled = CB_Corrin.SelectedIndex > 1;
        }
    }

    public class Command
    {
        public string CMD;
        public int NumParams;
        public string[] Params;
    }

    public class FontCharacter
    {
        public ushort Value;
        public ushort IMG;
        public ushort XOfs;
        public ushort YOfs;
        public byte Width;
        public byte Height;
        public byte Padding1;
        public int CropHeight;
        public int CropWidth;
        public byte[] Padding2; // Size = 3;

        public char Character;

        public Image Glyph;

        public FontCharacter() { }

        public FontCharacter(byte[] Data) : this(Data, 0) { }

        public FontCharacter(byte[] Data, int ofs)
        {
            Value = BitConverter.ToUInt16(Data, ofs + 0);
            IMG = BitConverter.ToUInt16(Data, ofs + 2);
            XOfs = BitConverter.ToUInt16(Data, ofs + 4);
            YOfs = BitConverter.ToUInt16(Data, ofs + 6);
            Width = Data[ofs + 8];
            Height = Data[ofs + 9];
            Padding1 = Data[ofs + 0xA];
            CropHeight = Data[ofs + 0xB];
            if (CropHeight > 0x7F)
                CropHeight -= 256;
            CropWidth = Data[ofs + 0xC];
            if (CropWidth > 0x7F)
                CropWidth -= 256;
            Padding2 = new byte[0x3];
            Array.Copy(Data, ofs + 0xD, Padding2, 0, 3);

            Character = Encoding.Unicode.GetString(Data, ofs + 0, 2)[0];
        }

        public void SetGlyph(Image img)
        {
            Bitmap crop = new Bitmap(Width, Height);
            using (Graphics g = Graphics.FromImage(crop))
            {
                g.DrawImage(img as Bitmap, new Rectangle(0, 0, Width, Height), new Rectangle(XOfs, YOfs, Width, Height), GraphicsUnit.Pixel);
            }
            Glyph = crop;
        }

        public Image GetGlyph(Color NewColor)
        {
            Bitmap ColoredGlyph = Glyph as Bitmap;

            Rectangle rect = new Rectangle(0, 0, ColoredGlyph.Width, ColoredGlyph.Height);
            BitmapData bmpData = ColoredGlyph.LockBits(rect, ImageLockMode.ReadWrite, ColoredGlyph.PixelFormat);

            IntPtr ptr = bmpData.Scan0;

            int bytes = Math.Abs(bmpData.Stride) * ColoredGlyph.Height;
            byte[] rgbaValues = new byte[bytes];

            // Copy the RGB values into the array.
            Marshal.Copy(ptr, rgbaValues, 0, bytes);

            for (int i = 0; i < rgbaValues.Length; i += 4)
            {
                if (rgbaValues[i + 3] > 0) // If CurrentColor.A > 0
                {
                    rgbaValues[i + 2] = NewColor.R;
                    rgbaValues[i + 1] = NewColor.G;
                    rgbaValues[i + 0] = NewColor.B;
                }
            }

            // Copy the RGB values back to the bitmap
            Marshal.Copy(rgbaValues, 0, ptr, bytes);

            // Unlock the bits.
            ColoredGlyph.UnlockBits(bmpData);
            return ColoredGlyph;
        }
    }
}
