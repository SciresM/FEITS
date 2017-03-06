using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using FEFTS.Properties;

namespace FEFTS
{
    /// <summary>
    /// Generates the portraits of the current characters in dialogue.
    /// </summary>
    public partial class PortraitGenerator : Form
    {
        public List<string> ResourceList;           // holds all resources
        public Dictionary<string, string> Names;    // holds names of characters in dialogue

        public List<cbItem>[] Characters = new List<cbItem>[3]; // initialize list of characters in dialogue
        
        public string[] Prefixes = { "st", "bu", "ct" };

        // variables holding character attributes
        private Color HAIR_COLOR;
        private string CHARACTER;
        private string EMOTIONS;

        // list of all character emotions assets
        private Control[] Emotion_Spec;
        private Control[] Corrin_Spec;

        private bool loaded;    // indicates whether or not the portraits have been loaded on screen

        // contains list of all default character attributes
        private Dictionary<string, int> DefaultHairs = new Dictionary<string, int>();
        private Dictionary<string, byte[]> FaceData;
        private string[] EyeStyles = { "a", "b", "c", "d", "e", "f", "g" };
        private string[] Corrins = { "マイユニ男1", "マイユニ男2", "マイユニ女1", "マイユニ女2" };

        /**
        Constructor for portrait generation of dialogue characters.
        * @param RL Resource list.
        * @param N Names of the characters in dialogue.
        * @param FD Face data of the characters in dialogue.
        */
        public PortraitGenerator(List<string> RL, Dictionary<string, string> N, Dictionary<string, byte[]> FD)
        {
            InitializeComponent();

            // list of all character emotion assets
            Emotion_Spec = new Control[] { LBL_Emotions, CB_Emotion, CHK_Blush, CHK_SweatDrop };
            Corrin_Spec = new Control[] { LBL_CharType, CB_Corrin, LBL_Eyes, CB_Eyes, LBL_HairStyle, CB_HairStyle, LBL_FacialFeature, CB_FacialFeature, LBL_Accessory, CB_Accessory };

            // set local variables
            ResourceList = RL;
            Names = N;
            FaceData = FD;

            // gets all possible hair colours of characters
            DefaultHairs = new Dictionary<string, int>();
            string[] HCs = Resources.HCs.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string HC in HCs)
            {
                var H = HC.Split(new[] { '\t' });
                DefaultHairs[H[0]] = int.Parse(H[1], NumberStyles.AllowHexSpecifier);
            }
            CB_HairColor.Items.Add("Custom");   // adds custom hair colour option
            CB_HairColor.Items.AddRange(DefaultHairs.Keys.Select(s => s).ToArray());

            CB_PortraitStyle.Items.AddRange(new[] { "Standard", "Closeup", "Critical" });   // adds various portrait range options

            // sets the character that is currently in dialogue
            for (int i = 0; i < Characters.Length; i++) // for each character in the dialogue
            {
                Characters[i] = new List<cbItem>();     // initialize the character as a new cbItem
                foreach (string Resource in ResourceList)
                {
                    if (Resource.Contains("_" + Prefixes[i] + "_")) // finds character name for the current character
                    {
                        string Character = Resource.Substring(0, Resource.IndexOf("_" + Prefixes[i] + "_"));    // gets the character name
                        cbItem ncbi = new cbItem    // create a new cbItem containing the character name
                        {
                            Value = Character,
                            Text = Names.ContainsKey(Character) ? Names[Character] : Character
                        };
                        // if the character name is not "マイユニ" or "Kana"
                        if (!ncbi.Text.Contains("マイユニ") && ncbi.Text != "Kana")
                        {
                            // and if the character name found does not exist in the character list already, add it to the list
                            if (Characters[i].All(cbi => cbi.Text != ncbi.Text) && Characters[i].All(cbi => cbi.Value != ncbi.Value))
                                Characters[i].Add(ncbi);
                        }
                    }
                }
                Characters[i].Add(new cbItem { Text = "Corrin", Value = "username" });      // add "Corrin" to the list
                Characters[i].Add(new cbItem { Text = "Kana (M)", Value = "カンナ男" });    // add "Kana (M)" to the list
                Characters[i].Add(new cbItem { Text = "Kana (F)", Value = "カンナ女" });    // add "Kana (F)" to the list
                Characters[i] = Characters[i].OrderBy(cbi => cbi.Text).ToList();
            }
            // set member values of the character
            CB_Character.DisplayMember = "Text";
            CB_Character.ValueMember = "Value";

            // add options for the character's appearance
            CB_Corrin.Items.AddRange(new[] { "Male 1", "Male 2", "Female 1", "Female 2" }); // add options for character gender
            CB_Eyes.Items.AddRange(new[] { "Style A", "Style B", "Style C", "Style D", "Style E", "Style F", "Style G" });  // add options for eyes
            CB_HairStyle.Items.AddRange(Enumerable.Range(0, 12).Select(i => i.ToString("00")).ToArray());   // add options for hairstyles
            CB_FacialFeature.Items.AddRange(new[] { "None", "Scratches", "Vertical Scratches", "Horizontal Scratches", "Tattoo 1", "Tattoo 2", "Tattoo 3", "Eye Mole", "Mouth Mole", "Plaster 1", "Plaster 2", "White Eyepatch", "Black Eyepatch" });   // add options for facial features
            CB_Accessory.Items.AddRange(new[] { "None", "Silver Piece", "Butterfly", "Black Ribbon", "White Ribbon", "White Rose" });   // add options for accessories

            CB_PortraitStyle.SelectedIndex = 2; // set default portrait style to style 2

            CB_Character.SelectedIndex = CB_HairColor.SelectedIndex = CB_Corrin.SelectedIndex = CB_Eyes.SelectedIndex = CB_HairStyle.SelectedIndex = CB_FacialFeature.SelectedIndex = CB_Accessory.SelectedIndex = 0;   // set default indices of all character appearance options to 0
            CB_Accessory.Enabled = LBL_Accessory.Enabled = CB_Corrin.SelectedIndex > 1; // enable accessories if the gender is female

            loaded = true;  // set indicator that portrait generation is completed
            UpdateImage();  // refresh image with portraits on screen
        }

        /**
         Updates the portrait upon changes to portrait style of the current character.
         * @param sender Object that calls the change in character portrait style.
         * @param e Holds information regarding the changes.
         */
        private void CB_PortraitStyle_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (CB_Character.SelectedIndex >= 0)    // if the index number of the character is valid (positive number)
            {
                cbItem curitem = CB_Character.Items[CB_Character.SelectedIndex] as cbItem;  // get items of the character
                CB_Character.DataSource = Characters[CB_PortraitStyle.SelectedIndex];       // set the character's portrait style
                bool found = false; // indicates whether or not the item was found
                for (int i=0;i<CB_Character.Items.Count;i++)    // for each item in the character
                {
                    if ((CB_Character.Items[i] as cbItem).Text == curitem.Text) // if the item is the one at the selected index
                    {
                        CB_Character.SelectedIndex = i; // set the selected index of the character to the index of the item
                        found = true;                   // set indicator to indicate that the item was found
                    }
                }
                if (!found)     // if item wasn't found
                    CB_Character.SelectedIndex = 0; // set the selected index to 0 (by default)
            }
            else                // if item was found
            {
                CB_Character.DataSource = Characters[CB_PortraitStyle.SelectedIndex];   // reset the character's portrait style to the index just found
                CB_Character.SelectedIndex = 0;     // reset the selected index to 0 (by default)
            }
            foreach (Control ctrl in Emotion_Spec)  // for each emotion
                ctrl.Enabled = (CB_PortraitStyle.Items[CB_PortraitStyle.SelectedIndex] as string) != "Critical";    // set emotion to true if the portrait style is not "Critical"
            if (!loaded)    // if portrait generation is not completed
                return;     // exit the function
            UpdateImage();  // if portrait generation is completed, update the dialogue image
        }

        /**
         Updates the portrait upon changes to the hexcode of the hair colour of the current character.
         * @param sender Object that calls the change in hexcode of the hair colour.
         * @param e Holds information regarding the changes.
         */
        private void MTB_HairColor_TextChanged(object sender, EventArgs e)
        {
            string hc = MTB_HairColor.Text.ToUpper();   // gets the new hair colour hex as a string in all caps format
            for (int i = 1; i < hc.Length; i++)         // for each character in the hair colour hex code
            {
                if (!((hc[i] >= 'A' && hc[i] <= 'F') || (hc[i] >= '0' && hc[i] <= '9'))) // if the character is not between 'A' to 'F' or not between '0' to '9'
                {
                    hc = hc.Substring(0, i) + hc.Substring(i + 1);  // remove the invalid character
                }
            }
            if (hc != MTB_HairColor.Text)   // if the newly formatted hexcode does not match what was originally retrieved
                MTB_HairColor.Text = hc;    // set the original hex to the new formatted hex
            else
            {
                while (hc.Length < 7)       // if the hexcode is not long enough
                    hc = hc + "0";          // append 0's at the end
                hc = hc.Replace("_", "0");  // replace all underscore characters with 0's
                HAIR_COLOR = Color.FromArgb((int)(0xFF000000 | uint.Parse(hc.Substring(1), NumberStyles.AllowHexSpecifier)));   // set the hair colour
            }
            if (!loaded)    // if portrait generation is not completed
                return;     // exit the function
            UpdateImage();  // if portrait generation is completed, update the dialogue image
        }

        /**
         Updates the portrait upon changes to the index of the hair colour of the current character.
         * @param sender Object that calls the change in index of hair colour.
         * @param e Holds information regarding the changes.
         */
        private void CB_HairColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((CB_HairColor.Items[CB_HairColor.SelectedIndex] as string) == "Custom") // if the current hair colour is "Custom"
            {
                MTB_HairColor.Enabled = true;   // set hair colour enabled to true
                string hc = MTB_HairColor.Text; // get the hexcode of the hair colour
                while (hc.Length < 7)           // if the hexcode is not long enough
                    hc = hc + "0";              // append 0's at the end
                hc = hc.Replace("_", "0");      // replace all underscore characters with 0's
                HAIR_COLOR = Color.FromArgb((int)(0xFF000000 | uint.Parse(hc.Substring(1), NumberStyles.AllowHexSpecifier)));   // set the hair colour
            }
            else     // if current hair colour is not "Custom"
            {
                MTB_HairColor.Enabled = false;  // set hair colour enabled to false
                HAIR_COLOR = Color.FromArgb((int)(0xFF000000 | (uint)DefaultHairs[CB_HairColor.Items[CB_HairColor.SelectedIndex] as string]));  // set the default hair colour based of selected index
            }
            if (!loaded)    // if portrait generation is not completed
                return;     // exit the function
            UpdateImage();  // if portrait generation is completed, update the dialogue image
        }

        /**
         Updates the portrait upon changes to the index the current character.
         * @param sender Object that calls the change in index of the character.
         * @param e Holds information regarding the changes.
         */
        private void CB_Character_SelectedIndexChanged(object sender, EventArgs e)
        {
            CHARACTER = (CB_Character.Items[CB_Character.SelectedIndex] as cbItem).Value as string; // get the value of the item of the character at the selected index
            foreach (Control ctrl in Corrin_Spec)       // for each specification in the character
                ctrl.Enabled = CHARACTER == "username"; // set the specification to enabled if it is the "username"
            if (CHARACTER == "username")
            {
                if (CB_Corrin.SelectedIndex < 2 && CB_Accessory.SelectedIndex > 0)  // if the gender is male and there is an accesory
                {
                    CB_Accessory.SelectedIndex = 0; // remove the accessory
                    return;                         // exit the function
                }
                CB_Accessory.Enabled = LBL_Accessory.Enabled = CB_Corrin.SelectedIndex > 1; // enable accessories if the gender is female
            }
            string cname = string.Empty + CHARACTER;
            if (cname == "username")
                cname = "aマイユニ女1";  // if the character name is "username", change it to "aマイユニ女1"
            CB_Emotion.Items.Clear();   // clear all emotions of the character
            List<string> NewEmotions = new List<string>();  // initialize new list of emotions
            foreach (string Resource in ResourceList)
            {
                if (Resource.Contains(cname + "_" + Prefixes[CB_PortraitStyle.SelectedIndex] + "_"))    // if the current resource matches the character's emotion resource
                {
                    string emotion = Resource.Substring(Resource.IndexOf("_" + Prefixes[CB_PortraitStyle.SelectedIndex] + "_") + ("_" + Prefixes[CB_PortraitStyle.SelectedIndex] + "_").Length);    // get the emotion
                    if (!NewEmotions.Contains(emotion) && emotion != "汗" && emotion != "照" && !emotion.Contains("髪")) // Not Sweatdrop, blushing, or a hair overlay
                        NewEmotions.Add(emotion);   // add the emotion to the list
                }
            }
            NewEmotions.Sort();
            CB_Emotion.Items.AddRange(NewEmotions.ToArray());
            CB_Emotion.SelectedIndex = 0;   // set default index of emotions to 0
            if (!loaded)    // if portrait generation is not completed
                return;     // exit the function
            UpdateImage();  // if portrait generation is completed, update the dialogue image
        }

        /**
         Updates the portrait upon changes to the emotion of the current character.
         * @param sender Object that calls the change in emotions.
         * @param e Holds information regarding the changes.
         */
        private void Emotions_Changed(object sender, EventArgs e)
        {
            EMOTIONS = CB_Emotion.Items[CB_Emotion.SelectedIndex] as string;    // get current emotion
            EMOTIONS += ",";
            if (CHK_Blush.Checked)  // if blush option enabled
                EMOTIONS += "照,";   // append "照," to emotions
            if (CHK_SweatDrop.Checked)  // if sweatdrop option enabled
                EMOTIONS += "汗,";   // append "汗," to emotions
            if (!loaded)    // if portrait generation is not completed
                return;     // exit the function
            UpdateImage();  // if portrait generation is completed, update the dialogue image
        }

        /**
         Loads the new portrait dialogue image on screen.
         */
        private void UpdateImage()
        {
            if (!loaded)    // if portrait generation not loaded yet, exit the function
                return;
            // call various functions to get portrait images based on portrait style of the current character
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

        /**
         Get the critical portrait image of the current character.
         * @param CName Name of the character.
         * @param HairColor Hair colour of the character.
         */
        private Image GetCharacterCriticalImage(string CName, Color HairColor)
        {
            string hairname = "_ct_髪";
            // string dat_id = "FSID_CT_" + CName;
            bool USER = CName == "username";
            // if the name of the character is "username", set the character name and hairstyle name based upon the indices of the current character
            if (USER)
            {
                // dat_id = "FSID_CT_" + (new[] { "マイユニ_男1", "マイユニ_男2", "マイユニ_女1", "マイユニ_女2" })[CB_Corrin.SelectedIndex] + "_顔" + EyeStyles[CB_Eyes.SelectedIndex].ToUpper();
                CName = EyeStyles[CB_Eyes.SelectedIndex] + Corrins[CB_Corrin.SelectedIndex];
                hairname = CName.Substring(1) + hairname + CB_HairStyle.SelectedIndex;
            }
            // if name of the character is not "username", set the hairstyle named based off the given character and hairstyle name
            else
                hairname = CName + hairname + "0";
            string resname = CName + "_ct_ベース"; // get resource name based off given character name
            Image C;
            if (ResourceList.Contains(resname)) // if resources contain the resource name
                C = Resources.ResourceManager.GetObject(resname) as Image;  // retrieve the matching image
            else
                C = new Bitmap(1, 1);   // if resources do not contain the image already, create a new image
            using (Graphics g = Graphics.FromImage(C))
            {
                if (USER && CB_FacialFeature.SelectedIndex > 0)
                {
                    g.DrawImage(Resources.ResourceManager.GetObject((new[] { "マイユニ男1", "マイユニ男2", "マイユニ女1", "マイユニ女2" })[CB_Corrin.SelectedIndex] + "_ct_アクセサリ1_" + CB_FacialFeature.SelectedIndex) as Image, new Point(0, 0));   // draw the new image
                }
                // if resources contain the hair style already, retrieve and draw the image of the hair from resources
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

        /**
         Get the standard portrait image of the current character.
         * @param CName Name of the character.
         * @param CEmo Emotions of the character.
         * @param HairColor Hair colour of the character.
         * @param Slot1 Which side of the dialogue the character is located in.
         */
        private Image GetCharacterStageImage(string CName, string CEmo, Color HairColor, bool Slot1)
        {
            bool USER = CName == "username";    // indicates if character name is "username"
            string hairname = "_st_髪";
            string dat_id = "FSID_ST_" + CName;
            // if character name is "username", set character name and hair name accordingly (according to indices)
            if (USER)
            {
                dat_id = "FSID_ST_" + (new[] { "マイユニ_男1", "マイユニ_男2", "マイユニ_女1", "マイユニ_女2" })[CB_Corrin.SelectedIndex] + "_顔" + EyeStyles[CB_Eyes.SelectedIndex].ToUpper();
                CName = EyeStyles[CB_Eyes.SelectedIndex] + Corrins[CB_Corrin.SelectedIndex];
                hairname = CName.Substring(1) + hairname + CB_HairStyle.SelectedIndex;
            }
            // if character name is not "username", set hairname according to given character name and given hair name
            else
                hairname = CName + hairname + "0";
            var Emos = CEmo.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);    // get and clean up emotions
            string resname = CName + "_st_" + Emos[0];  // get emotion resource of character
            Image C;
            // if resources contain the character's resource, get the image in resources
            if (ResourceList.Contains(resname))
                C = Resources.ResourceManager.GetObject(resname) as Image;
            // if not, create a new image
            else
                C = new Bitmap(1, 1);
            using (Graphics g = Graphics.FromImage(C))
            {
                // if character name is "username" and facial feature is not default, draw the image accordingly
                if (USER && CB_FacialFeature.SelectedIndex > 0)
                {
                    g.DrawImage(Resources.ResourceManager.GetObject((new[] { "マイユニ男1", "マイユニ男2", "マイユニ女1", "マイユニ女2" })[CB_Corrin.SelectedIndex] + "_st_アクセサリ1_" + CB_FacialFeature.SelectedIndex) as Image, new Point(0, 0));
                }
                for (int i = 1; i < Emos.Length; i++)   // for each emotion
                {
                    string exresname = CName + "_st_" + Emos[i];    // get the resource name of the emotion
                    
                    // if the emotion is contained in resources and is "汗", draw it
                    if (Emos[i] == "汗" && ResourceList.Contains(exresname))
                    {
                        g.DrawImage(Resources.ResourceManager.GetObject(exresname) as Image, new Point(BitConverter.ToUInt16(FaceData[dat_id], 0x40), BitConverter.ToUInt16(FaceData[dat_id], 0x42)));
                    }
                    // if the emotions is contained in resources and is "照", draw it
                    else if (Emos[i] == "照" && ResourceList.Contains(exresname))
                    {
                        g.DrawImage(Resources.ResourceManager.GetObject(exresname) as Image, new Point(BitConverter.ToUInt16(FaceData[dat_id], 0x38), BitConverter.ToUInt16(FaceData[dat_id], 0x3A)));
                    }
                }
                // if resources contain the character's hair, draw it
                if (ResourceList.Contains(hairname))
                {
                    Bitmap hair = Resources.ResourceManager.GetObject(hairname) as Bitmap;
                    g.DrawImage(ColorHair(hair, HairColor), new Point(0, 0));
                }
                // if the character has an accessory, draw it
                if (USER && CB_Accessory.SelectedIndex > 0)
                {
                    g.DrawImage(Resources.ResourceManager.GetObject((new[] { "マイユニ男1", "マイユニ男2", "マイユニ女1", "マイユニ女2" })[CB_Corrin.SelectedIndex] + "_st_アクセサリ2_" + CB_Accessory.SelectedIndex) as Image, new Point(133, 28));
                }
            }
            // flip the character according to where they should be facing
            if (Slot1)
                C.RotateFlip(RotateFlipType.RotateNoneFlipX);
            return C;
        }

        /**
         Get the close-up portrait image of the current character.
         * @param CName Name of the character.
         * @param CEmo Emotions of the character.
         * @param HairColor Hair colour of the character.
         */
        private Image GetCharacterBUImage(string CName, string CEmo, Color HairColor)
        {
            bool USER = CName == "username";
            string hairname = "_bu_髪";
            string dat_id = "FSID_BU_" + CName;
            // if the character name is "username", set the variables accordingly (indices of the character)
            if (USER)
            {
                dat_id = "FSID_BU_" + (new[] { "マイユニ_男1", "マイユニ_男2", "マイユニ_女1", "マイユニ_女2" })[CB_Corrin.SelectedIndex] + "_顔" + EyeStyles[CB_Eyes.SelectedIndex].ToUpper();
                CName = EyeStyles[CB_Eyes.SelectedIndex] + Corrins[CB_Corrin.SelectedIndex];
                hairname = CName.Substring(1) + hairname + CB_HairStyle.SelectedIndex;
            }
            // if the character name is not "username", set the hair name according the given character name and default hair name
            else
                hairname = CName + hairname + "0";
            var Emos = CEmo.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);    // get and clean up emotions of the character
            string resname = CName + "_bu_" + Emos[0];  // get the resource name of the character's emotions
            Image C;
            // if the resource list contains the resource name found, get the corresponding image in resources
            if (ResourceList.Contains(resname))
                C = Resources.ResourceManager.GetObject(resname) as Image;
            // if resource not found in resources, create new image
            else
                C = new Bitmap(1, 1);
            using (Graphics g = Graphics.FromImage(C))
            {
                // if character name is "username" and facial features is not default, draw image accordingly
                if (USER && CB_FacialFeature.SelectedIndex > 0)
                {
                    g.DrawImage(Resources.ResourceManager.GetObject((new[] { "マイユニ男1", "マイユニ男2", "マイユニ女1", "マイユニ女2" })[CB_Corrin.SelectedIndex] + "_bu_アクセサリ1_" + CB_FacialFeature.SelectedIndex) as Image, new Point(0, 0));
                }
                for (int i = 1; i < Emos.Length; i++)   // for each emotion
                {
                    string exresname = CName + "_bu_" + Emos[i];    // get the resource name based on the current emotion
                    // if the emotion is contained in resources and is "汗", draw it
                    if (Emos[i] == "汗" && ResourceList.Contains(exresname))
                    {
                        g.DrawImage(Resources.ResourceManager.GetObject(exresname) as Image, new Point(BitConverter.ToUInt16(FaceData[dat_id], 0x40), BitConverter.ToUInt16(FaceData[dat_id], 0x42)));
                    }
                    // if the emotion is contained in resources and is "照", draw it
                    else if (Emos[i] == "照" && ResourceList.Contains(exresname))
                    {
                        g.DrawImage(Resources.ResourceManager.GetObject(exresname) as Image, new Point(BitConverter.ToUInt16(FaceData[dat_id], 0x38), BitConverter.ToUInt16(FaceData[dat_id], 0x3A)));
                    }
                }
                // if the resource list contains the hair name, draw it
                if (ResourceList.Contains(hairname))
                {
                    Bitmap hair = Resources.ResourceManager.GetObject(hairname) as Bitmap;
                    g.DrawImage(ColorHair(hair, HairColor), new Point(0, 0));
                }
                // if the character name is "username" and character has an accesory, draw it
                if (USER && CB_Accessory.SelectedIndex > 0)
                {
                    Point Acc = new[] { new Point(66, 5), new Point(65, 21) }[CB_Corrin.SelectedIndex - 2];
                    g.DrawImage(Resources.ResourceManager.GetObject((new[] { "マイユニ男1", "マイユニ男2", "マイユニ女1", "マイユニ女2" })[CB_Corrin.SelectedIndex] + "_bu_アクセサリ2_" + CB_Accessory.SelectedIndex) as Image, Acc);
                }
            }
            C.RotateFlip(RotateFlipType.RotateNoneFlipX);   // do not flip the character
            return C;
        }

        /**
         Get the image of coloured hair.
         * @param Hair Image of the original hair.
         * @param C Desired colour of the coloured hair.
         */
        private Image ColorHair(Image Hair, Color C)
        {
            // get image data
            Bitmap bmp = Hair as Bitmap;
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);

            IntPtr ptr = bmpData.Scan0;

            // get colour data
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

        /**
         Return the value of the overlayed byte of two bytes.
         * @param Src First byte to overlay.
         * @param Dst Second byte to overlay.
         */
        private static byte BlendOverlay(byte Src, byte Dst)
        {
            return ((Dst < 128) ? (byte)Math.Max(Math.Min((Src / 255.0f * Dst / 255.0f) * 255.0f * 2, 255), 0) : (byte)Math.Max(Math.Min(255 - ((255 - Src) / 255.0f * (255 - Dst) / 255.0f) * 255.0f * 2, 255), 0));
        }

        /**
         Updates the portrait upon changes to the data of the current character.
         * @param sender Object that calls the change in data of the current character.
         * @param e Holds information regarding the changes.
         */
        private void Corrin_Data_Changed(object sender, EventArgs e)
        {
            // exit function is portrait generated is not loaded yet
            if (!loaded)
                return;
            // if character is male, set accessories to none
            if (CB_Corrin.SelectedIndex < 2 && CB_Accessory.SelectedIndex > 0)
            {
                CB_Accessory.SelectedIndex = 0;
                return;
            }
            // if character is female, enable accessories
            CB_Accessory.Enabled = LBL_Accessory.Enabled = CB_Corrin.SelectedIndex > 1;
            UpdateImage();
        }

        /**
         Saves the current dialogue image upon clicking the portrait.
         * @param sender Object that calls the image save.
         * @param e Holds information regarding the save.
         */
        private void PB_Portrait_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            string DefaultName = string.Empty;
            string hairname = "_ct_髪";
            
            // if character name is "username", set hair name according to the character's properties
            if (CHARACTER == "username")
            {
                hairname = (EyeStyles[CB_Eyes.SelectedIndex] + Corrins[CB_Corrin.SelectedIndex]).Substring(1) + hairname + CB_HairStyle.SelectedIndex;
            }
            // if character name is not "username", set hair name according to given character name and default hair name
            else
                hairname = CHARACTER + hairname + "0";

            // if resources contain the hair, set the default name of the file according to the hair properties
            if (ResourceList.Contains(hairname))
            {
                DefaultName += CB_HairColor.Items[CB_HairColor.SelectedIndex] as string;
                if (DefaultName == "Custom")
                    DefaultName += " (" + MTB_HairColor.Text.Substring(1) + ")";
                DefaultName += "_";
            }
            // append default name of the file with the character's items and portrait style
            DefaultName +=  (CB_Character.Items[CB_Character.SelectedIndex] as cbItem).Text;
            DefaultName += "_" + CB_PortraitStyle.Items[CB_PortraitStyle.SelectedIndex];
            // if character portrait style is critical, also append the emotions of the character to the file name
            if (CB_PortraitStyle.Items[CB_PortraitStyle.SelectedIndex] as string != "Critical")
                DefaultName += "_" + string.Join("-", EMOTIONS.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries));
            DefaultName += ".png";  // lastly, add the file extension ".png" to the file name
            sfd.FileName = DefaultName;
            if (sfd.ShowDialog() != DialogResult.OK)
                return;
            PB_Portrait.Image.Save(sfd.FileName, ImageFormat.Png);  // save the portrait file in .png format
        }
    }

    /**
    Object that holds a string Text and object Value field. Used to represent a character during dialogue.
    */
    public class cbItem
    {
        public string Text { get; set; }
        public object Value { get; set; }
    }
}
