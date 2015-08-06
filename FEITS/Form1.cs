using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using FEITS.Properties;

namespace FEITS
{
    public partial class Form1 : Form
    {
        private Image[] Images = new Image[] { Resources.Awakening_0, Resources.Awakening_1 };

        private bool[] ValidCharacters;
        private FontCharacter[] Characters;
        private Dictionary<string, string> Names;
        private Dictionary<string, byte[]> FaceData;

        private string PLAYER_NAME = "Kamui";
        private string RAW_MESSAGE = "$t1$Wmエリーゼ|3$w0|$Wsエリーゼ|$Wa$Eびっくり,汗|This is an example conversation.$k$p$Wmサクラ|7$w0|$Wsサクラ|$Wa$E怒,汗|It takes place between\nSakura and Elise.$k";
        private bool HAS_PERMS = false;
        private bool SET_TYPE = false;

        private int CUR_INDEX = 0;
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


        private string[] EyeStyles = new string[] { "a", "b", "c", "d", "e", "f", "g" };
        private string[] Kamuis = new string[] { "マイユニ男1", "マイユニ男2", "マイユニ女1", "マイユニ女2" };

        private Image[] TextBoxes = new Image[] { Resources.TextBox, Resources.TextBox_Nohr, Resources.TextBox_Hoshido };

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
            System.Resources.ResourceSet set = Resources.ResourceManager.GetResourceSet(System.Globalization.CultureInfo.CurrentCulture, true, true);
            foreach (System.Collections.DictionaryEntry o in set)
                ResourceList.Add(o.Key as string);
            Resources.ResourceManager.ReleaseAllResources();

            GetCharacterBUImage("リョウマ", DEFAULT_EMOTION, Color.FromArgb(0x5B5855), true).Save("B:/ryoumacrop.png", ImageFormat.Png);

            CB_Kamui.Items.AddRange(new[] { "Male 1", "Male 2", "Female 1", "Female 2" });
            CB_Eyes.Items.AddRange(new[] { "Style A", "Style B", "Style C", "Style D", "Style E", "Style F", "Style G" });
            CB_TB.Items.AddRange(new[] { "Standard", "Nohrian", "Hoshidan" });
            CB_HairStyle.Items.AddRange(Enumerable.Range(0, 12).Select(i => i.ToString("00")).ToArray());
            CB_Kamui.SelectedIndex = CB_TB.SelectedIndex = CB_Eyes.SelectedIndex = CB_HairStyle.SelectedIndex = 0;

            MTB_HairColorA.Text = MTB_HairColorB.Text = "#5B5855";
            B_Reload_Click(null, null);
        }

        private void RTB_Line_TextChanged(object sender, EventArgs e)
        {
            bool invalids = false;
            List<char> inv = new List<char>();
            for (int i = 0; i < RTB_Line.Text.Length; i++)
            {
                if (!ValidCharacters[GetValue(RTB_Line.Text[i])])
                {
                    if (!invalids)
                        invalids = true;
                    if (!inv.Contains(RTB_Line.Text[i]))
                        inv.Add(RTB_Line.Text[i]);
                }
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
            string old = Message;
            string[] NoParams = new string[] { "$Wa", "$Wc", "$a", "$Nu", "$N0", "$N1", "$t0", "$t1", "$k", "$p" };
            string[] SingleParams = new string[] { "$E", "$Sbs", "$Svp", "$Sre", "$Fw", "$Ws", "$VF", "$Ssp", "$Fo", "$VNMPID", "$Fi", "$b", "$Wd", "$w", "$l" };
            string[] DoubleParams = new string[] { "$Wm", "$Sbv", "$Sbp", "$Sls", "$Slp" };
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
            if (CONVERSATION_TYPE == ConversationTypes.TYPE_1)
                return RenderTypeOne(Message);
            else
                return RenderTypeZero();
        }

        private Image RenderTypeOne(string Message)
        {
            Bitmap Box = new Bitmap(PB_TextBox.Width, PB_TextBox.Height);
            Bitmap TB = DrawString(TextBoxes[CB_TB.SelectedIndex], Message, 29, 22, Color.FromArgb(68, 8, 0)) as Bitmap;
            string Name = Names.ContainsKey(CHAR_ACTIVE) ? Names[CHAR_ACTIVE] : (CHAR_ACTIVE == "username" ? PLAYER_NAME : CHAR_ACTIVE);
            int NameLen = GetLength(Name);
            Bitmap NB = DrawString(Resources.NameBox, Name, Resources.NameBox.Width / 2 - NameLen / 2, 16, Color.FromArgb(253, 234, 177)) as Bitmap; // Center Name in NameBox
            using (Graphics g = Graphics.FromImage(Box))
            {
                if (CHAR_A != string.Empty)
                {
                    Image CA = GetCharacterStageImage(CHAR_A, EMOTION_A, COLOR_A, true);
                    g.DrawImage(CA, new Point(-35, Box.Height - CA.Height + 14));
                }
                if (CHAR_B != string.Empty)
                {
                    Image CB = GetCharacterStageImage(CHAR_B, EMOTION_B, COLOR_B, false);
                    g.DrawImage(CB, new Point(Box.Width - CB.Width + 28, Box.Height - CB.Height + 14));
                }
                g.DrawImage(TB, new Point(3, Box.Height - TB.Height + 2));
                if (CHAR_ACTIVE != string.Empty)
                {
                    if (CHAR_ACTIVE == CHAR_B)
                        g.DrawImage(NB, new Point(Box.Width - NB.Width, Box.Height - TB.Height - 14));
                    else
                        g.DrawImage(NB, new Point(0, Box.Height - TB.Height - 14));
                }
                if (CUR_INDEX < Messages.Count - 1)
                {
                    g.DrawImage(Resources.KeyPress, new Point(Box.Width - 30, Box.Height - TB.Height + 32));
                }

            }
            return Box as Image;
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
                }
                TopBox = DrawString(TopBox, TopMessage, 76, 22, Color.FromArgb(68, 8, 0)) as Bitmap;
            }
            if (BottomMessage != string.Empty && CHAR_B != string.Empty)
            {
                BottomBox = (TextBoxes[CB_TB.SelectedIndex].Clone()) as Bitmap;
                using (Graphics g = Graphics.FromImage(BottomBox))
                {
                    g.DrawImage(GetCharacterBUImage(CHAR_B, EMOTION_B, COLOR_B, true), new Point(2, 3));
                }
                BottomBox = DrawString(BottomBox, BottomMessage, 76, 22, Color.FromArgb(68, 8, 0)) as Bitmap;
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
                g.DrawImage(TopBox, new Point(3, 3));
                g.DrawImage(BottomBox, new Point(3, Box.Height - BottomBox.Height + 2));
                if (TopMessage != string.Empty && CHAR_A != string.Empty)
                {
                    string TopName = Names.ContainsKey(CHAR_A) ? Names[CHAR_A] : (CHAR_A == "username" ? PLAYER_NAME : CHAR_A);
                    int NameLen = GetLength(TopName);
                    Bitmap TopNameBox = DrawString(Resources.NameBox, TopName, Resources.NameBox.Width / 2 - NameLen / 2, 16, Color.FromArgb(253, 234, 177)) as Bitmap; // Center Name in NameBox
                    g.DrawImage(TopNameBox, new Point(0, TopBox.Height - (TopNameBox.Height - 20)));
                }
                if (BottomMessage != string.Empty && CHAR_B != string.Empty)
                {
                    string BottomName = Names.ContainsKey(CHAR_B) ? Names[CHAR_B] : (CHAR_B == "username" ? PLAYER_NAME : CHAR_B);
                    int NameLen = GetLength(BottomName);
                    Bitmap BottomNameBox = DrawString(Resources.NameBox, BottomName, Resources.NameBox.Width / 2 - NameLen / 2, 16, Color.FromArgb(253, 234, 177)) as Bitmap; // Center Name in NameBox
                    g.DrawImage(BottomNameBox, new Point(0, Box.Height - BottomBox.Height - 14));
                }
            }
            return Box as Image;
        }

        private Image GetCharacterCriticalImage(string CName, Color HairColor)
        {
            string hairname = "_ct_髪";
            string dat_id = "FSID_CT_" + CName;
            if (CName == "username")
            {
                dat_id = "FSID_CT_" + (new string[] { "マイユニ_男1", "マイユニ_男2", "マイユニ_女1", "マイユニ_女2" })[CB_Kamui.SelectedIndex] + "_顔" + EyeStyles[CB_Eyes.SelectedIndex].ToUpper();
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
            string hairname = "_st_髪";
            string dat_id = "FSID_ST_" + CName;
            if (CName == "username")
            {
                dat_id = "FSID_ST_" + (new string[] { "マイユニ_男1", "マイユニ_男2", "マイユニ_女1", "マイユニ_女2" })[CB_Kamui.SelectedIndex] + "_顔" + EyeStyles[CB_Eyes.SelectedIndex].ToUpper();
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
                if (ResourceList.Contains(hairname))
                {
                    Bitmap hair = Resources.ResourceManager.GetObject(hairname) as Bitmap;
                    g.DrawImage(ColorHair(hair, HairColor), new Point(0, 0));
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
            }
            if (Slot1)
                C.RotateFlip(RotateFlipType.RotateNoneFlipX);
            return C;
        }

        private Image GetCharacterBUImage(string CName, string CEmo, Color HairColor, bool Crop)
        {
            bool USER = CName == "username";
            string hairname = "_bu_髪";
            string dat_id = "FSID_BU_" + CName;
            if (USER)
            {
                dat_id = "FSID_ST_" + (new string[] { "マイユニ_男1", "マイユニ_男2", "マイユニ_女1", "マイユニ_女2" })[CB_Kamui.SelectedIndex] + "_顔" + EyeStyles[CB_Eyes.SelectedIndex].ToUpper();
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
                if (ResourceList.Contains(hairname))
                {
                    Bitmap hair = Resources.ResourceManager.GetObject(hairname) as Bitmap;
                    g.DrawImage(ColorHair(hair, HairColor), new Point(0, 0));
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
            }
            if (Crop)
            {
                Bitmap Cropped = USER ? new Bitmap(70, 49) : new Bitmap(BitConverter.ToUInt16(FaceData[dat_id], 0x34), BitConverter.ToUInt16(FaceData[dat_id], 0x36));
                using (Graphics g = Graphics.FromImage(Cropped))
                {
                    g.DrawImage(C, USER ? new Point(-(BitConverter.ToUInt16(FaceData[dat_id], 0x38) - Cropped.Width), -((new int[4] { BitConverter.ToUInt16(FaceData[dat_id], 0x3A), BitConverter.ToUInt16(FaceData[dat_id], 0x3A), 52, 50 })[CB_Kamui.SelectedIndex])) : new Point(-BitConverter.ToUInt16(FaceData[dat_id], 0x30), -BitConverter.ToUInt16(FaceData[dat_id], 0x32)));
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
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbaValues, 0, bytes);

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
            System.Runtime.InteropServices.Marshal.Copy(rgbaValues, 0, ptr, bytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);
            return bmp as Image;
        }

        private static byte BlendOverlay(byte Src, byte Dst)
        {
            return ((Dst < 128) ? (byte)Math.Max(Math.Min((Src / 255.0f * Dst / 255.0f) * 255.0f * 2, 255), 0) : (byte)Math.Max(Math.Min(255 - ((255 - Src) / 255.0f * (255 - Dst) / 255.0f) * 255.0f * 2, 255), 0));
        }

        private Image DrawString(Image BaseImage, string Message, int StartX, int StartY, Color? TC = null)
        {
            Color TextColor = TC.HasValue ? TC.Value : Color.Black;
            int CurX = StartX;
            int CurY = StartY;
            Bitmap NewImage = BaseImage.Clone() as Bitmap;
            using (Graphics g = Graphics.FromImage(NewImage))
            {
                for (int i = 0; i < Message.Length; i++)
                {
                    if (Message[i] == '\n')
                    {
                        CurY += 20;
                        CurX = StartX;
                    }
                    else
                    {
                        FontCharacter cur = Characters[GetValue(Message[i])];
                        g.DrawImage(cur.GetGlyph(TextColor), new Point(CurX, CurY - cur.CropHeight));
                        CurX += cur.CropWidth;
                    }
                }
            }
            return NewImage as Image;
        }

        private ushort GetValue(char c)
        {
            return BitConverter.ToUInt16(Encoding.Unicode.GetBytes(string.Empty + c), 0);
        }

        private int GetLength(string s)
        {
            int len = 0;
            for (int i = 0; i < s.Length; i++)
            {
                ushort val = GetValue(s[i]);
                len += Math.Max(Characters[val].Width, Characters[val].CropWidth);
            }
            return len;
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
            for (int i = 0; i <= CUR_INDEX; i++)
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
                    for (int i = 0; i < Images.Count; i++)
                    {
                        g.DrawImage(Images[i], new Point(0, h));
                        h += Images[i].Height;
                    }
                }
                ToSave = bmp as Image;
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
                COLOR_A = Color.FromArgb((int)(0xFF000000 | int.Parse(hc.Substring(1), System.Globalization.NumberStyles.AllowHexSpecifier)));
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
                COLOR_B = Color.FromArgb((int)(0xFF000000 | int.Parse(hc.Substring(1), System.Globalization.NumberStyles.AllowHexSpecifier)));
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
            this.Value = BitConverter.ToUInt16(Data, ofs + 0);
            this.IMG = BitConverter.ToUInt16(Data, ofs + 2);
            this.XOfs = BitConverter.ToUInt16(Data, ofs + 4);
            this.YOfs = BitConverter.ToUInt16(Data, ofs + 6);
            this.Width = Data[ofs + 8];
            this.Height = Data[ofs + 9];
            this.Padding1 = Data[ofs + 0xA];
            this.CropHeight = Data[ofs + 0xB];
            if (this.CropHeight > 0x7F)
                this.CropHeight -= 256;
            this.CropWidth = Data[ofs + 0xC];
            if (this.CropWidth > 0x7F)
                this.CropWidth -= 256;
            this.Padding2 = new byte[0x3];
            Array.Copy(Data, ofs + 0xD, this.Padding2, 0, 3);

            this.Character = Encoding.Unicode.GetString(Data, ofs + 0, 2)[0];
        }

        public void SetGlyph(Image img)
        {
            Bitmap crop = new Bitmap(this.Width, this.Height);
            using (Graphics g = Graphics.FromImage(crop))
            {
                g.DrawImage(img as Bitmap, new Rectangle(0, 0, this.Width, this.Height), new Rectangle(this.XOfs, this.YOfs, this.Width, this.Height), GraphicsUnit.Pixel);
            }
            this.Glyph = crop as Image;
        }

        public Image GetGlyph(Color NewColor)
        {
            Bitmap ColoredGlyph = this.Glyph as Bitmap;

            Rectangle rect = new Rectangle(0, 0, ColoredGlyph.Width, ColoredGlyph.Height);
            BitmapData bmpData = ColoredGlyph.LockBits(rect, ImageLockMode.ReadWrite, ColoredGlyph.PixelFormat);

            IntPtr ptr = bmpData.Scan0;

            int bytes = Math.Abs(bmpData.Stride) * ColoredGlyph.Height;
            byte[] rgbaValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbaValues, 0, bytes);

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
            System.Runtime.InteropServices.Marshal.Copy(rgbaValues, 0, ptr, bytes);

            // Unlock the bits.
            ColoredGlyph.UnlockBits(bmpData);
            return ColoredGlyph as Image;
        }
    }
}
