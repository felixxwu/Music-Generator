using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace WAVToArray
{

    public partial class Form1 : Form
    {

        public Form1()
        {
            // Error trapping for missing data library

            if (Directory.Exists(Application.StartupPath + "/Data Library") == false)
            {
                MessageBox.Show(DataLibraryNotFound);
                this.Close();
            }

            InitializeComponent();

            // Allows a confirmation message to be displayed before closing

            FormClosing += Form1_FormClosing;
        }

        #region Initiation and Globals

        // Random number generator

        Random random = new Random();

        // Global constants

        const int SampleRate = 44100;
        const int SamplesPerMinute = SampleRate * 60;
        const int TickPerBeat = 4;
        const int MetaEnd = 44;
        const int SampleCut = 140;
        const int RepeatLength = 8;
        const int SongBeatLength = 32;
        const int MHeadSize = 61;
        const int PatternLength = SongBeatLength * TickPerBeat;
        const double ReleaseRate = 0.006 / 256;

        // Error messages

        const string MissingChordsErrorMsg = "Please fill out missing chords and try again";
        const string DataLibraryNotFound = "The folder 'Data Library' was not found, the application will now close.";

        // Instrument pattern indexes - used to determine which column belongs to which instrument in the array "Pattern"

        const int Kick = 0;
        const int Snare = 1;
        const int OpenHat = 2;
        const int ClosedHat = 3;
        const int Synth = 4;
        const int Ducking = 5;

        // Filenames for the reference headers

        const string WAVReferenceHeader = "kick";
        const string MIDIReferenceHeader = "reference";

        // Filenames of files that will be created

        const string FileMidiMelody = "MIDI Melody.mid";
        const string FileMidiChords = "MIDI Chords.mid";

        // Global Variables

        int[,] Pattern = new int[6, PatternLength];
        int KeyPitch;
        int KeyThird;
        int NoteCount;
        string FilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".wav");

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        #endregion

        #region "Create" Buttons

        // "Play New Riff" button

        private void PlayNewRiff_Click(object sender, EventArgs e)
        {
            if (ChordsCompleted() == false & ChordsCleared() == false)
            {
                // Error trapping for when chords are not properly filled out

                MessageBox.Show(MissingChordsErrorMsg);
                return;
            }
            if (ChordsCleared() == true)
            {
                // If chords are empty, make a random key

                KeyPitch = random.Next(0, 12);
                KeyThird = random.Next(0, 2);
                KeyLabel.Text = "Key: " + ChordArray("P", 0).Items[KeyPitch].ToString() + ChordArray("T", 0).Items[KeyThird].ToString();
            }
            else
            {
                // If chords are not empty, the first chord represents the key

                KeyPitch = ChordArray("P", 0).SelectedIndex;
                KeyThird = ChordArray("T", 0).SelectedIndex;
                KeyLabel.Text = "Key: " + ChordArray("P", 0).SelectedItem.ToString() + ChordArray("T", 0).SelectedItem.ToString();
            }

            // Stop any playing previews

            StartStopSound("stop", ".");

            // Initialise and show the progress bar

            ProgressBar.Value = 0;
            ProgressBar.Visible = true;

            // Show the label "Current Riff" which is just above the visual riff preview

            RiffLabel.Visible = true;

            // Make the "Play Current Riff" button visible

            PlayCurrentRiff.Visible = true;

            // Current riff controls will now be available

            GroupBoxCurrent.Visible = true;

            // NoteCount is used to count the total number of notes in the riff

            NoteCount = 0;

            // Generate synth pattern - created here because it must be exclusive to the "Play New Riff" button

            GenerateSynthPattern();
            RepeatSynthPattern();

            // Display the riff in the UI

            DrawPictureBox(Picture);

            // Create and play the preview

            CreatePreview();

            // Hide the progress bar

            ProgressBar.Visible = false;
        }

        private void CreatePreview()
        {
            // Create pattern for Kick, Hats and Snare

            GeneratecHatPattern();
            GenerateOHatPattern();
            GenerateSnarePattern();
            GenerateKickPattern();

            // If required, add the rhythmical effect

            if (ProgRhythmCheck.Checked == true) DeleteSnareHat();

            // Turn the cursor into a waiting cursor to signal that the programme is working

            Cursor.Current = Cursors.WaitCursor;

            // Define the array that will store the waveform of the preview - it will be used to create the WAV file

            decimal SamplesPerTick = SamplesPerMinute / BpmBar.Value;
            byte[] Main = new byte[MetaEnd + Convert.ToInt32((SamplesPerTick) * PatternLength)];

            // Construct a header for the preview file

            CreateHeader(Main);

            // First add the instruments that will be ducked to the main array

            for (int i = 0; i < PatternLength; i++)
            {
                // Delay is the number of ticks that position i represents in the preview

                int Delay = Convert.ToInt32((SamplesPerTick) * i);

                // For each item in the pattern array check if the instrument needs to be sequenced
                // (Represented by a 1 at the appropriate position)

                if (Pattern[OpenHat, i] == 1) Add(Main, ReadBytes("ohat"), Delay);
                if (Pattern[ClosedHat, i] == 1) Add(Main, ReadBytes("chat"), Delay);

                // For the synth, -1 represents when the synth is not to be played
                // Any other number represents the pitch of the note that should be played

                if (Pattern[Synth, i] != -1)
                {
                    // Read which note is at position i

                    string Note = Convert.ToString(Pattern[Synth, i]);

                    // Synth files in the data library are named 0 - 11 for each pitch
                    // Add the synth file with the filename: Note to the main waveform

                    Add(Main, ReadBytes(Note), Delay);
                }

                // Increment progress bar for visual feedback

                ProgressBar.Value = i;
            }

            // If the chords are filled out, add them to the waveform

            if (ChordsCleared() == false)
            {
                // 4 total chords, each played 4 times, once every beat - so a total of 16 beats (4 x 4)

                int SamplesPerBeat = Convert.ToInt32((SamplesPerTick) * TickPerBeat);
                byte[] ChordPreview = new byte[MetaEnd + SamplesPerBeat * 16];

                // Create the waveform for the chords

                CreateChordData(ChordPreview);

                // Add waveform to main waveform
                // Since the chords are only 16 beats, add a copy on the end

                Add(Main, ChordPreview, 0);
                Add(Main, ChordPreview, SamplesPerBeat * 16);
            }

            // Apply ducking to what is in the main array so far (Synth, Hats and Chords)

            AddDucking(Main, Pattern);

            // Add kick and snare waveforms to the main array

            for (int i = 0; i < PatternLength; i++)
            {
                // Delay is the number of ticks that position i represents in the preview

                int Delay = Convert.ToInt32((SamplesPerTick) * i);

                // For each item in the pattern array check if the instrument needs to be sequenced
                // (Represented by a 1 at the appropriate position)

                if (Pattern[Kick, i] == 1) Add(Main, ReadBytes("kick"), Delay);
                if (Pattern[Snare, i] == 1) Add(Main, ReadBytes("snare"), Delay);

                // Continue the progress of prgress bar

                ProgressBar.Value = i + 128;
            }

            // Write the preview file using the waveform in the main array to a temporary location

            FilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".wav");
            System.IO.File.WriteAllBytes(FilePath, Main);

            // If the option is selected, play automatically

            if (AutoPlay.Checked == Enabled)
            {
                StartStopSound("start", FilePath);
            }

            // Reset cursor type

            Cursor.Current = Cursors.Default;
        }

        // "Play Current Riff" button

        private void PlayCurrentRiff_Click(object sender, EventArgs e)
        {
            // Since the synth must not change when this button is pressed, GenerateSynthPattern() is not found here

            // Reset and show the progress bar

            ProgressBar.Value = 0;
            ProgressBar.Visible = true;

            // Stop any playing previews

            StartStopSound("stop",".");

            // Create a new preview (will be using updated settings if any are changed)

            CreatePreview();

            // Hide progress bar

            ProgressBar.Visible = false;
        }

        #endregion

        #region WAV File Header

        // Copy the header from the reference file

        private void CopyHeader(byte[] TargetHeader)
        {
            for (int i = 0; i < MetaEnd; i++)
            {
                byte[] Header = ReadBytes(WAVReferenceHeader);
                TargetHeader[i] = Header[i];
            }
        }

        // Adjust the file length information in the header
        
        private void AdjHeadLengthInfo(byte[] TargetHeader, int StartPos)
        {
            for (int i = 0; i < 4; i++)
            {
                // Calculate the value of the byte to be written to the header
                // StartPos is the point from which the length of the file is counted

                byte Byte = Convert.ToByte(Math.Floor((TargetHeader.Length - StartPos) / Math.Pow(256, i) % 256));

                // Write the 4 bytes in front of StartPos

                TargetHeader[StartPos - 4 + i] = Byte;
            }
        }

        private void CreateHeader(byte[] TargetHeader)
        {
            // Copy the header from the reference file

            CopyHeader(TargetHeader);

            // Adjust for number of bytes in the file (after the metadata, so 44 bytes onwards)

            AdjHeadLengthInfo(TargetHeader, MetaEnd);

            // Adjust for size of file (after the end of this sub-section, so 8 bytes onwards)

            AdjHeadLengthInfo(TargetHeader, 8);
        }

        #endregion

        #region UI

        #region Chords

        #region Randomise

        // Randomise all chords

        private void RandAllButton_Click(object sender, EventArgs e)
        {
            // Randomise first chord with no guidelines

            ChordArray("P", 0).SelectedIndex = random.Next(0, 12);
            ChordArray("T", 0).SelectedIndex = random.Next(0, 2);

            // Update key

            KeyPitch = ChordArray("P", 0).SelectedIndex;
            KeyThird = ChordArray("T", 0).SelectedIndex;
            KeyLabel.Text = "Key: " + ChordArray("P", 0).SelectedItem.ToString() + ChordArray("T", 0).SelectedItem.ToString();

            // Randomise other three chords using RandomiseChord(ComboBox, ComboBox)

            for (int i = 1; i < 4; i++)
            {
                RandomiseChord(ChordArray("P", i), ChordArray("T", i));
            }

            // Disable the "Play Current Riff" button, because the chords will not fit anymore

            PlayCurrentRiff.Visible = false;
        }

        // Randomise second chord only

        private void Rand2Button_Click(object sender, EventArgs e)
        {
            int PrevChord = ChordArray("P", 1).SelectedIndex;

            // If there is no first chord, a random chord cannot be created, so randomise first

            if (ChordArray("P", 0).SelectedIndex == -1 || ChordArray("T", 0).SelectedIndex == -1)
            {
                // Randomise first chord so that this chord has a key to work with

                ChordArray("P", 0).SelectedIndex = random.Next(0, 12);
                ChordArray("T", 0).SelectedIndex = random.Next(0, 2);
            }
            while (ChordArray("P", 1).SelectedIndex == PrevChord)
            {
                // Generate a random chord until it is different to the last

                RandomiseChord(ChordArray("P", 1), ChordArray("T", 1));
            }
        }

        // Randomise third chord only

        private void Rand3Button_Click(object sender, EventArgs e)
        {
            int PrevChord = ChordArray("P", 2).SelectedIndex;

            // If there is no first chord, a random chord cannot be created, so randomise first

            if (ChordArray("P", 0).SelectedIndex == -1 || ChordArray("T", 0).SelectedIndex == -1)
            {
                // Randomise first chord so that this chord has a key to work with

                ChordArray("P", 0).SelectedIndex = random.Next(0, 12);
                ChordArray("T", 0).SelectedIndex = random.Next(0, 2);
            }
            while (ChordArray("P", 2).SelectedIndex == PrevChord)
            {
                // Generate a random chord until it is different to the last

                RandomiseChord(ChordArray("P", 2), ChordArray("T", 2));  //randomise third chord
            }
        }

        // Randomise fourth chord only

        private void Rand4Button_Click(object sender, EventArgs e)
        {
            int PrevChord = ChordArray("P", 3).SelectedIndex;

            // If there is no first chord, a random chord cannot be created, so randomise first

            if (ChordArray("P", 0).SelectedIndex == -1 || ChordArray("T", 0).SelectedIndex == -1)
            {
                // Randomise first chord so that this chord has a key to work with

                ChordArray("P", 0).SelectedIndex = random.Next(0, 12);
                ChordArray("T", 0).SelectedIndex = random.Next(0, 2);
            }
            while (ChordArray("P", 3).SelectedIndex == PrevChord)
            {
                // Generate a random chord until it is different to the last

                RandomiseChord(ChordArray("P", 3), ChordArray("T", 3));  //randomise fourth chord
            }
        }

        // Randomise specified chord using guidelines

        private void RandomiseChord(ComboBox ComboPitch, ComboBox ComboThird)
        {
            // New pitch and third is generated at random, these will be tested

            int TryChordPitch = random.Next(0, 12);
            int TryChordThird = random.Next(0, 2);

            // Loop while the generated pitch and third is not legal

            while (LegalChord(TryChordPitch, TryChordThird) == false)
            {
                // Try new chord

                TryChordPitch = random.Next(0, 12);
                TryChordThird = random.Next(0, 2);
            }

            // When a legal chord is found, assign it to the chord in the UI

            ComboPitch.SelectedIndex = TryChordPitch;
            ComboThird.SelectedIndex = TryChordThird;
        }

        // Inputted pitch and third will be tested to see if it is legal

        public bool LegalChord(int TryChordPitch, int TryChordThird)
        {
            // ChordProfile will be used at the end to check if the chord is legal

            int[] ChordProfile = new int[12] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, };

            // ScaleDegree specifies which chords work with the key (default position at C)
            // 0 = major, 1 = minor, -1 = none
            // First group are the major scale degrees, second is the minor

            int[,] ScaleDegree = new int[2, 7] { { 0, 1, 1, 0, 0, 1, 1 }, { 1, -1, 0, 1, 0, 0, 0 } };

            // Create the scale in the key specified in the UI

            int ChordPitch = ChordArray("P", 0).SelectedIndex;
            int ChordThird = ChordArray("T", 0).SelectedIndex;
            int[] ScaleArray = CreateScaleArray(ChordPitch, ChordThird);

            // Key is global and should not be manipulated here, therefore assign new variable

            int KeyOffset = KeyPitch;
            int j = 0;

            for (int i = 0; i < 12; i++)
            {
                // Start writing values into ChordProfile

                // If i + KeyOffset >= 12, the item will be outside the bounds of the array
                // Wrap around back to the start by subtracting 12

                if (i + KeyOffset >= 12)
                {
                    KeyOffset -= 12;
                }

                // Using ScaleArray, the pitch of each legal chord can be found

                if (ScaleArray[i + KeyOffset] == 1)
                {
                    // Using ScaleDegree, the third of the legal chord can be found
                    // These are then written to ChordProfile

                    ChordProfile[i + KeyOffset] = ScaleDegree[KeyThird, j];

                    // j is incremented by 1 representing the next position in the ScaleDegree array

                    j++;
                }
            }

            // The input pitch and thirds are tested for, return true or false depending on the test

            if (ChordProfile[TryChordPitch] != -1 & ChordProfile[TryChordPitch] == TryChordThird)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Check if all chords in UI are legal

        private void CheckLegalChords()
        {
            bool ChordsLegal = true;
            for (int i = 1; i < 4; i++)
            {
                int ChordPitch = ChordArray("P", i).SelectedIndex;
                int ChordThird = ChordArray("T", i).SelectedIndex;
                if (ChordPitch != -1 & ChordThird != -1 & ChordArray("P", 0).SelectedIndex != -1 & ChordArray("T", 0).SelectedIndex != -1)
                {
                    if (LegalChord(ChordPitch, ChordThird) == false)
                    {
                        ChordsLegal = false;
                    }
                }
            }

            // Display warning if something is wrong

            WrongChordWarning.Visible = !ChordsLegal;
        }

        private void ChordsClearButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 4; i++)
            {
                ChordArray("P", i).SelectedIndex = -1;
                ChordArray("T", i).SelectedIndex = -1;
            }
        }

        #endregion

        #region Other

        //CHECK IF CHORD UI IS COMPLETED

        public bool ChordsCompleted()
        {
            for (int i = 0; i < 4; i++)
            {
                if (ChordArray("P", i).SelectedIndex == -1) return false;
                if (ChordArray("T", i).SelectedIndex == -1) return false;
            }
            return true;
        }

        //CHECK IF CHORD UI IS EMPTY

        public bool ChordsCleared()
        {
            for (int i = 0; i < 4; i++)
            {
                if (ChordArray("P", i).SelectedIndex != -1 || ChordArray("T", i).SelectedIndex != -1) return false;
            }
            return true;
        }

        //RETURN THE CHORDS IN THE UI

        public ComboBox ChordArray(string PitchOrThird, int ChordNumber)
        {
            switch (PitchOrThird + Convert.ToString(ChordNumber))
            {
                case "P0": return ChordPitch1;
                case "P1": return ChordPitch2;
                case "P2": return ChordPitch3;
                case "P3": return ChordPitch4;
                case "T0": return ChordThird1;
                case "T1": return ChordThird2;
                case "T2": return ChordThird3;
                case "T3": return ChordThird4;
            }
            return null;
        }

        #endregion

        #endregion

        #region Use created WAV file (access, play)

        private void StopPreview_Click(object sender, EventArgs e)          //STOP PREVIEW (BUTTON)
        {
            StartStopSound("stop",".");
        }

        private void ExportWav_Click(object sender, EventArgs e)
        {
            SaveFileDialog SaveMelody = new SaveFileDialog();
            SaveMelody.FileName = "Riff.wav";
            SaveMelody.Filter = "WAV Files (*.wav)|*.wav";
            if (SaveMelody.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(SaveMelody.FileName, File.ReadAllBytes(FilePath));
            }
        }
        
        #endregion

        #region Other

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult ExitDialogue = MessageBox.Show("Progress will be lost, are you sure you want to exit?", "Exit Application", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (ExitDialogue == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void StartStopSound(string Command, string SoundFilePath)        //START OR STOP PREVIEW
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(SoundFilePath);
            if (Command == "start")                                         //play file if instructed
            {
                player.Play();
            }
            else if (Command == "stop")                                     //stop file if instructed
            {
                player.Stop();
            }
        }

        private void ChordPitch1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckLegalChords();
            UpdateKey();
        }

        private void ChordThird1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckLegalChords();
            UpdateKey();
        }

        private void ChordPitch2_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckLegalChords();
        }

        private void ChordThird2_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckLegalChords();
        }

        private void ChordPitch3_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckLegalChords();
        }

        private void ChordThird3_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckLegalChords();
        }

        private void ChordPitch4_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckLegalChords();
        }

        private void ChordThird4_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckLegalChords();
        }

        private void UpdateKey()
        {
            if (ChordArray("P", 0).SelectedIndex != -1 & ChordArray("T", 0).SelectedIndex != -1)
            {
                KeyLabel.Text = "Key: " + ChordArray("P", 0).SelectedItem.ToString() + ChordArray("T", 0).SelectedItem.ToString();
            }
        }

        private void BpmBar_Scroll(object sender, EventArgs e)
        {
            bpmLabel.Text = "Beats Per Minute: (" + BpmBar.Value + ")";
        }

        private void ExitButton_Click(object sender, EventArgs e)   //exit application
        {
            this.Close();
        }

        private void BPMResetButton_Click(object sender, EventArgs e)
        {
            BpmBar.Value = 128;
            bpmLabel.Text = "Beats Per Minute: (128)";
        }

        private void DrawPictureBox(PictureBox Picture)
        {
            const int PixelSize = 50;
            Picture.Image = new Bitmap(32 * PixelSize, 12 * PixelSize);
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 12 * PixelSize; j++)
                {
                    ((Bitmap)Picture.Image).SetPixel(i * 4 * PixelSize, j, Color.DimGray);
                }
            }
            for (int i = 0; i < 32; i++)
            {
                if (Pattern[Synth, i] != -1)
                {
                    FillPixel(Picture, i, 11 - Pattern[Synth, i], PixelSize);
                }
            }
            Picture.Refresh();
        }

        private void FillPixel(PictureBox Picture, int x, int y, int PixelSize)
        {
            for (int i = 0; i < PixelSize; i++)
            {
                for (int j = 0; j < PixelSize; j++)
                {
                    ((Bitmap)Picture.Image).SetPixel(x * PixelSize + i, y * PixelSize + j, Color.FromArgb(51,77,90));
                }
            }
        }

        #endregion

        #endregion

        #region Waveform manipulation

        public byte[] ReadBytes(string FileName)                        //read data from required file in the library
        {
            string ReadFilePath = Application.StartupPath + "/Data Library/" + FileName + ".wav";
            if (File.Exists(ReadFilePath))
            {
                return File.ReadAllBytes(ReadFilePath);
            }
            else
            {
                return new byte[500];
            }
        }

        private void Add(byte[] TargetWav, byte[] Addend, int Delay)
        {
            const int Max = 32767;
            const int Min = 32768;
            Delay = Convert.ToInt32(Delay / 4) * 4;                         //makes sure laft and right is not swapped
            for (int i = MetaEnd; i < Addend.Length -  SampleCut; i += 2)   //start writing after the header
            {
                if (i + Delay < TargetWav.Length - 1)                       //do not proceed if outside the range of target waveform (lengthwise)
                {
                    int TargetValue = Read(TargetWav, i + Delay);
                    int AddendValue = Read(Addend, i);
                    if (TargetValue + AddendValue > 65535)                  //if addition causes waveform to be in the LOWER half...
                    {
                        if (TargetValue >= Min & AddendValue >= Min & TargetValue + AddendValue - 65536 < Min)
                        {
                            Write(TargetWav, Min, i + Delay);               //if both waveforms are in the LOWER half and their addition causes the final
                        }                                                   //  waveform to be in the UPPER half, the final waveform must be clipped (at 32768, the bottom)
                        else                                                //otherwise proceed to add normally
                        {
                            Write(TargetWav, TargetValue + AddendValue - 65536, i + Delay);
                        }                                                   //(Target + Addend - 65536) is equivalent to (Target - 32768) + (Addend - 32768)
                    }
                    else                                                    //if addition causes waveform to be in the UPPER half
                    {
                        if (TargetValue <= Max & AddendValue <= Max & TargetValue + AddendValue > Max)
                        {
                            Write(TargetWav, Max, i + Delay);               //if both waveforms are in the UPPER half and their addition causes the final
                        }                                                   //  waveform to be in the LOWER half, the final waveform must be clipped (at 127, the top)
                        else                                                //otherwise proceed to add normally
                        {
                            Write(TargetWav, TargetValue + AddendValue, i + Delay);
                        }
                    }
                }
            }
        }

        public int Read(byte[] TargetWav, int Position)
        {
            return TargetWav[Position + 1] * 256 + TargetWav[Position];
        }

        private void Write(byte[] TargetWav, int Value, int Position)
        {
            TargetWav[Position] = Convert.ToByte(Value % 256);
            TargetWav[Position + 1] = Convert.ToByte(Math.Floor(Convert.ToDecimal(Value / 256)));
        }

        #endregion

        #region Generating instrument pattern arrays

        #region Generate Synth Pattern

        //GENERATING THE RANDOM SYNTH PATTERN

        private void GenerateSynthPattern()
        {
            bool PrevGoodNote = false;
            for (int i = 0; i < RepeatLength * TickPerBeat; i++)
            {
                Pattern[Synth, i] = -1;                                         //-1 indicates when the synth does not play
            }
            int j = 0;
            int NewNote = random.Next(0, 12);
            while (j < RepeatLength * TickPerBeat)                              //loop until end of the repeat is reached
            {
                int CurrentChordNumber = Convert.ToInt32(Math.Floor(Convert.ToDecimal(j / 16)));
                int CurrentChordKey = ChordArray("P", CurrentChordNumber).SelectedIndex;
                int CurrentChordThird = ChordArray("T", CurrentChordNumber).SelectedIndex;
                bool LegalNote = false;                                         //initiate the legal note check to false
                int PitchStep = 0;                                              //create new pitch interval
                while (LegalNote == false)                                      //loop until the legal note check is true
                {
                    PitchStep = GenerateNotePitch();                            //try a new pitch step for its validity
                    if (NewNote + PitchStep >= 0 & NewNote + PitchStep < 12)    //proceed only if the new note is within the valid range (one octave)
                    {
                        if (ChordsCleared() == true)
                        {
                            int[] ScaleArray = CreateScaleArray(KeyPitch, KeyThird);
                            if (ScaleArray[NewNote + PitchStep] == 1) LegalNote = true;
                        }
                        else
                        {
                            int[] ScaleArray = CreateScaleArray(CurrentChordKey, CurrentChordThird);
                            if (ScaleArray[NewNote + PitchStep] == 1 & (PrevGoodNote == true || CheckGoodNote(CurrentChordNumber, NewNote + PitchStep) == true))
                            {
                                LegalNote = true;
                            }
                        }
                    }
                }
                NewNote = NewNote + PitchStep;
                PrevGoodNote = CheckGoodNote(Convert.ToInt32(Math.Floor(Convert.ToDecimal(j / 16))), NewNote);
                Pattern[Synth, j] = NewNote;                                    //the new note is now valid, and added to the synth pattern at position j (default: 0)
                j = j + GenerateNoteTime();                                     //increment position of the next note by an integer given by GenerateNoteTime()
                NoteCount += 1;
            }                                  
        }

        public bool CheckGoodNote(int ChordNumber, int Pitch)
        {
            int[] ChordNotes = new int[3];
            CreateChordNotes(ChordNotes, ChordArray("P", ChordNumber).SelectedIndex, ChordArray("T", ChordNumber).SelectedIndex);
            for (int i = 0; i < 3; i++)
            {
                if (ChordNotes[i] == Pitch) return true;
            }
            return false;
        }

        private void RepeatSynthPattern() //REPEAT SYNTH PATTERN
        {
            if (SongBeatLength % RepeatLength == 0)             //repeat riff after a certain time for the duration of the song
            {
                for (int Repeat = 0; Repeat < SongBeatLength / RepeatLength; Repeat++)
                {
                    for (int i = 0; i < RepeatLength * TickPerBeat; i++)
                    {
                        Pattern[Synth, i + (RepeatLength * TickPerBeat * Repeat)] = Pattern[Synth, i];
                    }
                }
            }
        }

        //GENERATING THE RANDOM TIME INTERVAL BETWEEN SYNTH NOTES

        public int GenerateNoteTime()
        {
            int r = random.Next(0, 179);
            //Probablities:
            if (0 <= r & r < 16) return 1;          //(16 / 179) = 8.94%
            if (16 <= r & r < 77) return 2;         //(61 / 179) = 34.1%
            if (77 <= r & r < 159) return 3;        //(82 / 179) = 45.8%
            if (159 <= r & r < 176) return 4;       //(17 / 179) = 9.50%
            if (176 <= r & r < 177) return 6;       //(1 / 179) = 0.56%
            if (177 <= r & r < 178) return 7;       //(1 / 179) = 0.56%
            if (178 <= r & r < 179) return 9;       //(1 / 179) = 0.56%
            return 3;
        }

        //GENERATING THE RANDOM PITCH INTERVAL BETWEEN SYNTH NOTES

        public int GenerateNotePitch()
        {
            int r = random.Next(0, 164);
            //Probablities:
            if (0 <= r & r < 1) return -12;         //(1 / 164) = 0.61%
            if (1 <= r & r < 2) return -10;         //(1 / 164) = 0.61%
            if (2 <= r & r < 3) return -9;          //(1 / 164) = 0.61%
            if (3 <= r & r < 5) return -8;          //(2 / 164) = 1.22%
            if (5 <= r & r < 6) return -7;          //(1 / 164) = 0.61%
            if (6 <= r & r < 12) return -5;         //(6 / 164) = 3.66%
            if (12 <= r & r < 18) return -4;        //(6 / 164) = 3.66%
            if (18 <= r & r < 25) return -3;        //(7 / 164) = 4.27%
            if (25 <= r & r < 36) return -2;        //(11 / 164) = 6.71%
            if (36 <= r & r < 46) return -1;        //(10 / 164) = 6.10%
            if (46 <= r & r < 121) return 0;        //(75 / 164) = 45.7%
            if (121 <= r & r < 132) return 1;       //(11 / 164) = 6.71%
            if (132 <= r & r < 148) return 2;       //(16 / 164) = 9.75%
            if (148 <= r & r < 157) return 3;       //(9 / 164) = 5.49%
            if (157 <= r & r < 158) return 5;       //(1 / 164) = 0.61%
            if (158 <= r & r < 159) return 7;       //(1 / 164) = 0.61%
            if (159 <= r & r < 161) return 10;      //(2 / 164) = 1.22%
            if (161 <= r & r < 162) return 11;      //(1 / 164) = 0.61%
            if (162 <= r & r < 164) return 12;      //(2 / 164) = 1.22%
            return 0;
        }

        #endregion

        #region Ducking

        //DECLARE DUCKING TIME

        private void AddDucking(byte[] TargetWav, int[,] Pattern)   //determine when ducking should occur
        {
            decimal SamplesPerTick = SamplesPerMinute / BpmBar.Value;
            for (int i = 0; i < PatternLength; i++)
            {
                if (Pattern[Kick, i] == 1)                          //duck whenever the kick plays
                {
                    Duck(TargetWav, Convert.ToInt32((SamplesPerTick) * i + MetaEnd));
                }
            }
        }

        //DUCK

        private void Duck(byte[] TargetWav, int DuckTime)           //create actual ducking at requested time
        {
            double i = 0;
            int j = 0;
            while (i < 1)
            {
                if (DuckTime + j < TargetWav.Length)
                {
                    if (TargetWav[DuckTime + j] < 127)              //waveform in the UPPER half are multiplied by i normally
                    {
                        TargetWav[DuckTime + j] = Convert.ToByte(Convert.ToInt32(TargetWav[DuckTime + j] * i));
                    }
                    else                                            //waveform in the LOWER half need to have 255 subtracted first
                    {
                        TargetWav[DuckTime + j] = Convert.ToByte(Convert.ToInt32(((TargetWav[DuckTime + j] - 255) * i) + 255));
                    }
                }
                i = i + ReleaseRate;                                //i will slowly be incremented up to 1 as j gets bigger
                j++;
            }
        }

        #endregion

        #region Other instruments

        //SPECIAL RHYTHMIC EFFECT

        private void DeleteSnareHat()                                 //deletes Snares and Hats in the first half of the song
        {
            for (int i = 0; i < PatternLength / 2; i++)
            {
                Pattern[Snare, i] = 0;
                Pattern[OpenHat, i] = 0;
                Pattern[ClosedHat, i] = 0;
            }                                                       //rhythmical effect on the kick
            Pattern[Kick, PatternLength / 2 - 4] = 0;
            Pattern[Kick, PatternLength / 2 + 2] = 1;
        }

        //GENERATING STRAIGHT KICK BEAT

        private void GenerateKickPattern()                          //straight kick beat on every beat
        {
            for (int i = 0; i < PatternLength; i++)
            {
                if (i % 4 == 0)                                     //for every fourth count of i ( 0, 4, 8 ... )
                {
                    Pattern[Kick, i] = 1;
                    Pattern[Ducking, i] = 1;
                }
                else
                {
                    Pattern[Kick, i] = 0;
                    Pattern[Ducking, i] = 0;
                }
            }
        }

        //GENERATING SNARE EVERY SECOND BEAT

        private void GenerateSnarePattern()                         //on every second beat
        {
            for (int i = 0; i < PatternLength; i++)
            {
                if ((i + 4) % 8 == 0) Pattern[Snare, i] = 1;        //( 4, 12, 20 ... )
                else Pattern[Snare, i] = 0;
            }
        }

        //GENERATE OFF-BEAT OPEN HAT PATTERN

        private void GenerateOHatPattern()                          //between every beat
        {
            for (int i = 0; i < PatternLength; i++)
            {
                if ((i + 2) % 4 == 0) Pattern[OpenHat, i] = 1;      //( 2, 6, 10 ... )
                else Pattern[OpenHat, i] = 0;
            }
        }

        //GENERATING RANDOM CLOSED HAT PATTERN

        private void GeneratecHatPattern()                          //on every second quarter beat starting on the second quarter beat
        {
            for (int i = 0; i < PatternLength; i++)
            {
                if (!(i % 2 == 0))                                  //( 1, 3, 5 ... )
                {
                    Pattern[ClosedHat, i] = random.Next(0, 2);      //50% chance between 1 and 0
                }
            }
        }
        
        #endregion

        #endregion

        #region Chords

        #region Chord previewing

        //PREVIEW ALL CHORDS IN THE SEQUENCE

        private void PreviewChordsButton_Click(object sender, EventArgs e)
        {
            if (ChordsCompleted() == false)
            {
                MessageBox.Show(MissingChordsErrorMsg);                 //if any chords are not specified, display error
            }
            else
            {
                Directory.CreateDirectory(Application.StartupPath + "/Created Files/");
                decimal SamplesPerTick = SamplesPerMinute / BpmBar.Value;
                byte[] ChordPreview = new byte[Convert.ToInt32((SamplesPerTick) * TickPerBeat * 16)];
                CreateHeader(ChordPreview);

                ProgressBar.Visible = true;
                CreateChordData(ChordPreview);                          //insert waveform data
                Add(ChordPreview, ChordPreview, 0);
                ProgressBar.Visible = false;

                string ChordFilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".wav");
                System.IO.File.WriteAllBytes(ChordFilePath, ChordPreview);    //create audible file
                StartStopSound("start", ChordFilePath);                          //play Created Files
            }
        }

        //PREVIEW INDIVIDUAL CHORDS

        private void PlayChord(int Number)
        {
            if (ChordArray("P", Number).SelectedIndex == -1 || ChordArray("T", Number).SelectedIndex == -1)                             //if any chords are not specified, display error
            {
                MessageBox.Show(MissingChordsErrorMsg);
                return;
            }
            decimal SamplesPerTick = SamplesPerMinute / BpmBar.Value;
            byte[] ChordPreview = new byte[Convert.ToInt32(SamplesPerTick) * TickPerBeat * 2];
            CreateHeader(ChordPreview);
            //insert waveform data
            string ChordPitch = ChordArray("P", Number).SelectedItem.ToString();
            string ChordThird = ChordArray("T", Number).SelectedItem.ToString();
            string FileName = ChordPitch + ChordThird;
            int Delay = 0;
            Add(ChordPreview, ReadBytes(FileName), Delay);
            
            string ChordFilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".wav");
            System.IO.File.WriteAllBytes(ChordFilePath, ChordPreview);       //create audible file
            StartStopSound("start", ChordFilePath);                          //play Created Files
        }

        private void PlayChord1_Click(object sender, EventArgs e)
        {
            PlayChord(0);   //preview the first chord
        }

        private void PlayChord2_Click(object sender, EventArgs e)
        {
            PlayChord(1);   //preview the second chord
        }

        private void PlayChord3_Click(object sender, EventArgs e)
        {
            PlayChord(2);   //preview the third chord
        }

        private void PlayChord4_Click(object sender, EventArgs e)
        {
            PlayChord(3);   //preview the fourth chord
        }

        #endregion

        #region Audio constructoin

        //WRITE WAVEFORM TO ARRAYS

        private void CreateChordData(byte[] Chords)
        {
            ProgressBar.Value = 0;
            decimal SamplesPerTick = SamplesPerMinute / BpmBar.Value;
            int Beat = Convert.ToInt32((SamplesPerTick) * TickPerBeat);
            for (int i = 0; i < 4; i++)                                             //for each chord
            {
                for (int j = 0; j < 4; j++)
                {
                    string ChordPitch = ChordArray("P", i).SelectedItem.ToString();
                    string ChordThird = ChordArray("T", i).SelectedItem.ToString();
                    byte[] Bytes = ReadBytes(ChordPitch + ChordThird);
                    int Delay = (Beat * 4 * i) + (Beat * j);
                    Add(Chords, Bytes, Delay);
                    ProgressBar.Value += 256 / 16;
                }
            }
        }
        
        #endregion

        #region Notes

        public int[] CreateScaleArray(int LocalKey, int LocalThird)    //return an array of the requested scale
        {
            int[] MajorArray = new int[12] { 1, 0, 1, 0, 1, 1, 0, 1, 0, 1, 0, 1 };      //define scale defaults
            int[] MinorArray = new int[12] { 1, 0, 1, 1, 0, 1, 0, 1, 1, 0, 1, 0 };

            int[] SelectedArray = new int[12];
            int[] Output = new int[12];
            
            if (LocalThird == 0) SelectedArray = MajorArray;
            else SelectedArray = MinorArray;                        //select the type of scale requested                              
            
            for (int i = 0; i < SelectedArray.Length; i++)
            {
                if (i - LocalKey >= 0)
                {
                    Output[i] = SelectedArray[i - LocalKey];         //shift the items in the default scale array
                }
                else
                {
                    Output[i] = SelectedArray[i - LocalKey + 12];    //wrap shifted items to the other end of the array
                }
            }
            return Output;
        }

        private void CreateChordNotes(int[] ChordNotes, int ChordPitch, int ChordThird)
        {
            int[] MajorDefault = new int[3] { 0, 4, 7 };
            int[] MinorDefault = new int[3] { 0, 3, 7 };
            int[] SelectedDefault = new int[3];
            if (ChordThird == 0) SelectedDefault = MajorDefault;
            else SelectedDefault = MinorDefault;
            for (int i = 0; i < 3; i++)
            {
                if (SelectedDefault[i] + ChordPitch < 12)
                {
                    ChordNotes[i] = SelectedDefault[i] + ChordPitch;         //shift the items in the default scale array
                }
                else
                {
                    ChordNotes[i] = SelectedDefault[i] + ChordPitch - 12;    //wrap shifted items to the other end of the array
                }
            }
        }

        #endregion

        #endregion

        #region MIDI

        #region Melody

        private void CreateMidiMelodyEvents(byte[] Midi)
        {
            int EventCount = 0;                                             //EventCount is used to keep track of event position
            for (int i = 0; i < SongBeatLength; i++)
            {
                if (Pattern[Synth, i] != -1)                                //for each note
                {
                    byte j = 1;
                    while (i + j < SongBeatLength & Pattern[Synth, i + j] == -1)
                    {
                        j += 1;                                             //find time until next note
                    }
                    WriteMidiEvent(Pattern[Synth, i], j, Midi, EventCount); //insert start and sotp event into Midi[]
                    EventCount += 2;                                        //two events have been inserted
                }
            }
        }

        private void WriteMidiEvent(int Pitch, byte EventLength, byte[] Midi, int EventCount)
        {
            Midi[MHeadSize + (EventCount * 4)] = 144;                                   //indicates starting the note
            Midi[MHeadSize + (EventCount * 4) + 1] = Convert.ToByte(60 + Pitch);        //pitch
            Midi[MHeadSize + (EventCount * 4) + 2] = 127;                               //velocity
            Midi[MHeadSize + (EventCount * 4) + 3] = EventLength;                       //how long until the next event
            Midi[MHeadSize + (EventCount * 4) + 4] = 128;                               //indicates stopping the note
            Midi[MHeadSize + (EventCount * 4) + 5] = Convert.ToByte(60 + Pitch);        //pitch
            Midi[MHeadSize + (EventCount * 4) + 6] = 64;
            Midi[MHeadSize + (EventCount * 4) + 7] = 0;                                 //note extends until the next note, so stop event is of length 0
        }

        private void CreateMidiMelodyFile(byte[] Midi)
        {
            byte[] ReferenceHeader = File.ReadAllBytes(Application.StartupPath + "/Data Library/" + MIDIReferenceHeader + ".mid");
            for (int i = 0; i < MHeadSize; i++)
            {
                Midi[i] = ReferenceHeader[i];                                            //midi file header adjustment
            }
            Midi[13] = TickPerBeat;                                                             //ticks per beat
            Midi[48] = Convert.ToByte(NoteCount * 8 + 15);                                      //length of file after byte 48
            Midi[MHeadSize + (NoteCount * 8)] = 255;                                            //mark the end of the file
            Midi[MHeadSize + (NoteCount * 8) + 1] = 47;
            Midi[MHeadSize + (NoteCount * 8) + 2] = 0;
            SaveFileDialog SaveMelody = new SaveFileDialog();
            SaveMelody.FileName = FileMidiMelody;
            SaveMelody.Filter = "MIDI Files (*.mid)|*.mid";
            if (SaveMelody.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(SaveMelody.FileName, Midi);
            }
        }

        #endregion

        #region Chords

        private void CreateMidiChordEvents(byte[] MidiChords)
        {
            int[] MidiChordEvents = new int[3];
            for (int i = 0; i < 4; i++)
            {
                CreateChordNotes(MidiChordEvents, ChordArray("P", i).SelectedIndex, ChordArray("T", i).SelectedIndex);
                for (int j = 0; j < 3; j++)
                {
                    MidiChords[MHeadSize + (i * 24) + (j * 4)] = 144;                                               //start note
                    MidiChords[MHeadSize + (i * 24) + (j * 4) + 1] = Convert.ToByte(60 + MidiChordEvents[j]);    //pitch
                    MidiChords[MHeadSize + (i * 24) + (j * 4) + 2] = 127;                                           //velocity
                    MidiChords[MHeadSize + (i * 24) + (j * 4) + 3] = 0;                                             //duration
                }
                MidiChords[MHeadSize + (i * 24) + 11] = 4;                                                          //after third note, wait before stopping notes
                for (int j = 0; j < 3; j++)
                {
                    MidiChords[MHeadSize + (i * 24) + (j * 4) + 12] = 128;                                          //start note
                    MidiChords[MHeadSize + (i * 24) + (j * 4) + 13] = Convert.ToByte(60 + MidiChordEvents[j]);   //pitch
                    MidiChords[MHeadSize + (i * 24) + (j * 4) + 14] = 64;
                    MidiChords[MHeadSize + (i * 24) + (j * 4) + 15] = 0;                                            //duration
                }
            }
        }

        private void CreateMidiChordFile(byte[] MidiChords)
        {
            byte[] ReferenceHeader = File.ReadAllBytes(Application.StartupPath + "/Data Library/" + MIDIReferenceHeader + ".mid");
            for (int i = 0; i < MHeadSize; i++)
            {
                MidiChords[i] = ReferenceHeader[i];                                            //midi file header adjustment
            }
            MidiChords[13] = 1;
            MidiChords[48] = 111;                                                           //file byte length after byte 48: 111 = 99 + 12 [non midi message before notes]
            MidiChords[53] = 67;
            MidiChords[54] = 104;
            MidiChords[55] = 111;
            MidiChords[56] = 114;
            MidiChords[57] = 100;
            MidiChords[58] = 115;
            MidiChords[59] = 32;
            MidiChords[MHeadSize + 96] = 255;                                            //mark the end of the file
            MidiChords[MHeadSize + 97] = 47;
            MidiChords[MHeadSize + 98] = 0;
            SaveFileDialog SaveMelody = new SaveFileDialog();
            SaveMelody.FileName = FileMidiChords;
            SaveMelody.Filter = "MIDI Files (*.mid)|*.mid";
            if (SaveMelody.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(SaveMelody.FileName, MidiChords);
            }
        }

        #endregion

        #region Main

        private void ExportMIDI_Click(object sender, EventArgs e)
        {
            if (ChordsCompleted() == false & ChordsCleared() == false)
            {
                MessageBox.Show(MissingChordsErrorMsg);
                return;
            } 
            byte[] Midi = new byte[MHeadSize + (NoteCount * 8) + 3];        //for each note there are 8 bytes (4 for start, 4 for end) +3 for end of file message
            CreateMidiMelodyEvents(Midi);                                         //create event data in Midi[] ready to be turned into a file
            CreateMidiMelodyFile(Midi);                                           //create the actual midi file with appropriate header
            if (ChordsCleared() == false)
            {
                byte[] MidiChords = new byte[MHeadSize + 99];               //99 = (3 + 3) * 4 * 4 + 3 ([note ons] + [note offs]) * [chords] * [bytes in each message] + [end of file]
                CreateMidiChordEvents(MidiChords);
                CreateMidiChordFile(MidiChords);
            }
        }

        #endregion

        #endregion
    }
}

//\o/\\ 