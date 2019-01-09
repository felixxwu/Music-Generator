using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace WAVToArray
{

    public partial class Form1 : Form
    {

        #region Initiation and Globals

        public Form1()
        {
            // Error trapping for missing data library
            if (Directory.Exists(Application.StartupPath + "/Data Library") == false)
            {
                MessageBox.Show(NoFolderAndClose, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
            InitializeComponent();

            // Allows a confirmation message to be displayed before closing
            FormClosing += Form1_FormClosing;
        }

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
        const string NoFolderAndClose = "The folder 'Data Library' was not found the application will now close.";
        const string NoFolderWarning = "The folder 'Data Library' was not found";

        // Instrument pattern indexes
        // Used to determine which column belongs to which instrument in the array "Pattern"
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

        public bool DataLibraryExists()
        {
            if (Directory.Exists(Application.StartupPath + "/Data Library") == false)
            {
                MessageBox.Show(NoFolderWarning, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        #endregion

        #region Creation Buttons

        // "Play New Riff" button
        private void PlayNewRiff_Click(object sender, EventArgs e)
        {
            if (DataLibraryExists() == false) return;
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
                string ChordPitch = Chord("P", 0).Items[KeyPitch].ToString();
                string ChordThird = Chord("T", 0).Items[KeyThird].ToString();
                KeyLabel.Text = "Key: " + ChordPitch + ChordThird;
            }
            else
            {
                // If chords are not empty, the first chord represents the key
                KeyPitch = Chord("P", 0).SelectedIndex;
                KeyThird = Chord("T", 0).SelectedIndex;
                string ChordPitch = Chord("P", 0).SelectedItem.ToString();
                string ChordThird = Chord("T", 0).SelectedItem.ToString();
                KeyLabel.Text = "Key: " + ChordPitch + ChordThird;
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

            // Generate synth pattern
            // Created here because it must be exclusive to the "Play New Riff" button
            GenerateSynthPattern();
            RepeatSynthPattern();

            // Create and play the preview
            CreatePreview();

            // Display the riff in the UI
            DrawPictureBox(Picture);
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

            // Define the array that will store the waveform of the preview
            // It will be used to create the WAV file
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
                // 4 total chords, each played 4 times, once every beat
                // A total of 16 beats (4 x 4)
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
            // Error trapping for when "Data Library" does not exist
            if (DataLibraryExists() == false) return;

            // The synth must not change when this button is pressed
            // GenerateSynthPattern() is therefore not found here

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
                int Length = TargetHeader.Length - StartPos;
                byte Byte = (byte)(Math.Floor(Length / Math.Pow(256, i) % 256));

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
            Chord("P", 0).SelectedIndex = random.Next(0, 12);
            Chord("T", 0).SelectedIndex = random.Next(0, 2);

            // Update key
            KeyPitch = Chord("P", 0).SelectedIndex;
            KeyThird = Chord("T", 0).SelectedIndex;
            string ChordPitch = Chord("P", 0).SelectedItem.ToString();
            string ChordThird = Chord("T", 0).SelectedItem.ToString();
            KeyLabel.Text = "Key: " + ChordPitch + ChordThird;

            // Randomise other three chords using RandomiseChord(ComboBox, ComboBox)
            for (int i = 1; i < 4; i++)
            {
                RandomiseChord(Chord("P", i), Chord("T", i));
            }

            // Disable the "Play Current Riff" button, because the chords will not fit anymore
            PlayCurrentRiff.Visible = false;
        }

        // Randomise second chord only
        private void Rand2Button_Click(object sender, EventArgs e)
        {
            int PrevChord = Chord("P", 1).SelectedIndex;

            // If there is no first chord, a random chord cannot be created, so randomise first
            if (Chord("P", 0).SelectedIndex == -1 || Chord("T", 0).SelectedIndex == -1)
            {
                // Randomise first chord so that this chord has a key to work with
                ChordPitch1.SelectedIndex = random.Next(0, 12);
                ChordThird1.SelectedIndex = random.Next(0, 2);
            }
            while (Chord("P", 1).SelectedIndex == PrevChord)
            {
                // Generate a random chord until it is different to the last
                RandomiseChord(ChordPitch2, ChordThird2);
            }
        }

        // Randomise third chord only
        private void Rand3Button_Click(object sender, EventArgs e)
        {
            int PrevChord = Chord("P", 2).SelectedIndex;

            // If there is no first chord, a random chord cannot be created, so randomise first
            if (Chord("P", 0).SelectedIndex == -1 || Chord("T", 0).SelectedIndex == -1)
            {
                // Randomise first chord so that this chord has a key to work with
                ChordPitch1.SelectedIndex = random.Next(0, 12);
                ChordThird1.SelectedIndex = random.Next(0, 2);
            }
            while (Chord("P", 2).SelectedIndex == PrevChord)
            {
                // Generate a random chord until it is different to the last
                RandomiseChord(ChordPitch3, ChordThird3);
            }
        }

        // Randomise fourth chord only

        private void Rand4Button_Click(object sender, EventArgs e)
        {
            int PrevChord = Chord("P", 3).SelectedIndex;

            // If there is no first chord, a random chord cannot be created, so randomise first
            if (Chord("P", 0).SelectedIndex == -1 || Chord("T", 0).SelectedIndex == -1)
            {
                // Randomise first chord so that this chord has a key to work with
                ChordPitch1.SelectedIndex = random.Next(0, 12);
                ChordThird1.SelectedIndex = random.Next(0, 2);
            }
            while (Chord("P", 3).SelectedIndex == PrevChord)
            {
                // Generate a random chord until it is different to the last
                RandomiseChord(ChordPitch4, ChordThird4);
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
            int ChordPitch = Chord("P", 0).SelectedIndex;
            int ChordThird = Chord("T", 0).SelectedIndex;
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
                int ChordPitch = Chord("P", i).SelectedIndex;
                int ChordThird = Chord("T", i).SelectedIndex;
                int FirstPitch = Chord("P", 0).SelectedIndex;
                int FirstThird = Chord("T", 0).SelectedIndex;
                if (ChordPitch != -1 & ChordThird != -1 & FirstPitch != -1 & FirstThird != -1)
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
            // Clear all chords, -1 is blank
            ChordPitch1.SelectedIndex = -1;
            ChordPitch2.SelectedIndex = -1;
            ChordPitch3.SelectedIndex = -1;
            ChordPitch4.SelectedIndex = -1;
            ChordThird1.SelectedIndex = -1;
            ChordThird2.SelectedIndex = -1;
            ChordThird3.SelectedIndex = -1;
            ChordThird4.SelectedIndex = -1;
        }

        #endregion

        #region Chords Other

        // Check if the chords in the UI are all fileed out
        public bool ChordsCompleted()
        {
            for (int i = 0; i < 4; i++)
            {
                // As soon as one chord is detected to be unfilled, return false
                if (Chord("P", i).SelectedIndex == -1) return false;
                if (Chord("T", i).SelectedIndex == -1) return false;
            }

            // Otherwise return true
            return true;
        }

        // Check if no chords are filled out in the UI
        public bool ChordsCleared()
        {
            for (int i = 0; i < 4; i++)
            {
                // As soon as a chord is not empty, return false
                if (Chord("P", i).SelectedIndex != -1 || Chord("T", i).SelectedIndex != -1)
                {
                    return false;
                }
            }

            // Otherwise return true
            return true;
        }

        // Return the requested chord from the UI
        public ComboBox Chord(string ChordOrThird, int ChordNumber)
        {
            switch (ChordOrThird + Convert.ToString(ChordNumber))
            {
                // Return requested ComboBox
                // P - Pitch, T - Third
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

        #region WAV usage

        // Stops any sound that is currently playing
        private void StopPreview_Click(object sender, EventArgs e)
        {
            StartStopSound("stop",".");
        }

        // Exports the riff to a 16-bit WAV file
        private void ExportWav_Click(object sender, EventArgs e)
        {
            // Show a "Save As" dialogue from which the user can select the save location for the file
            SaveFileDialog SaveMelody = new SaveFileDialog();

            // Default file name
            SaveMelody.FileName = "Riff.wav";

            // File type
            SaveMelody.Filter = "WAV Files (*.wav)|*.wav";

            // If the user clicks "Save", write the file
            if (SaveMelody.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(SaveMelody.FileName, File.ReadAllBytes(FilePath));
            }
            // Otherwise, do not do anything
        }
        
        #endregion

        #region UI Other

        // This process is called whenever the window closes,
        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Show a dialogue to confirm closing with user
            DialogResult ExitDialogue = MessageBox.Show("Progress will be lost, are you sure you want to exit?",
                "Exit Application", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            // If the user selects "No", do not close application
            if (ExitDialogue == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        // Starts or stops the sound depending on input
        private void StartStopSound(string Command, string SoundFilePath)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(SoundFilePath);
            if (Command == "start")
            {
                // If input is "start", play the file
                player.Play();
            }
            else if (Command == "stop")
            {
                // If input is "stop", stop the file
                player.Stop();
            }
        }

        // Process called whenever the pitch of chord 1 is changed
        private void ChordPitch1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Call CheckLegalChords() to see if warning message needs to be displayed
            CheckLegalChords();

            // Udate the key to display the correct key
            UpdateKey();
        }

        // Process called whenever the third of chord 1 is changed
        private void ChordThird1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Call CheckLegalChords() to see if warning message needs to be displayed
            CheckLegalChords();

            // Udate the key to display the correct key
            UpdateKey();
        }

        // Process called whenever the pitch of chord 2 is changed
        private void ChordPitch2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Call CheckLegalChords() to see if warning message needs to be displayed
            CheckLegalChords();
        }

        // Process called whenever the third of chord 2 is changed
        private void ChordThird2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Call CheckLegalChords() to see if warning message needs to be displayed
            CheckLegalChords();
        }

        // Process called whenever the pitch of chord 3 is changed
        private void ChordPitch3_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Call CheckLegalChords() to see if warning message needs to be displayed
            CheckLegalChords();
        }

        // Process called whenever the third of chord 3 is changed
        private void ChordThird3_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Call CheckLegalChords() to see if warning message needs to be displayed
            CheckLegalChords();
        }

        // Process called whenever the pitch of chord 4 is changed
        private void ChordPitch4_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Call CheckLegalChords() to see if warning message needs to be displayed
            CheckLegalChords();
        }

        // Process called whenever the third of chord 4 is changed
        private void ChordThird4_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Call CheckLegalChords() to see if warning message needs to be displayed
            CheckLegalChords();
        }

        // Displays the correct key under the chord section
        private void UpdateKey()
        {
            // Check if the first chord is empty or not
            if (Chord("P", 0).SelectedIndex != -1 & Chord("T", 0).SelectedIndex != -1)
            {
                // If the first chord is filled out, proceed
                string ChordPitch = Chord("P", 0).SelectedItem.ToString();
                string ChordThird = Chord("T", 0).SelectedItem.ToString();

                // Set the text to whatever the first chord is
                KeyLabel.Text = "Key: " + ChordPitch + ChordThird;
            }
        }

        // Updates the text above the BPM slider to the value of the slider instandtly
        private void BpmBar_Scroll(object sender, EventArgs e)
        {
            bpmLabel.Text = "Beats Per Minute: (" + BpmBar.Value + ")";
        }

        // Simply closes the application
        private void ExitButton_Click(object sender, EventArgs e)   //exit application
        {
            this.Close();
        }

        // Resets the BPM
        private void BPMResetButton_Click(object sender, EventArgs e)
        {
            // SEt both the slider value, and the text to 128
            BpmBar.Value = 128;
            bpmLabel.Text = "Beats Per Minute: (128)";
        }

        // Draws the riff to the picture box
        private void DrawPictureBox(PictureBox Picture)
        {
            // Pixel size determines the quality of the visual
            // (picture is stretched in the UI, so a smaller picture would be of lower quality)
            const int PixelSize = 50;

            // Sets the size of the picture
            Picture.Image = new Bitmap(32 * PixelSize, 12 * PixelSize);

            // Draw thin lines on every beat to aid visual understanding
            for (int i = 0; i < 8; i++)
            {
                // 8 is the number of lines to be drawn
                for (int j = 0; j < 12 * PixelSize; j++)
                {
                    // 12 * PixelSize is the height of the picture
                    ((Bitmap)Picture.Image).SetPixel(i * 4 * PixelSize, j, Color.DimGray);
                }
            }

            // Draws the actual notes
            for (int i = 0; i < 32; i++)
            {
                // For each item in the array Pattern[]
                if (Pattern[Synth, i] != -1)
                {
                    // When Pattern[] contains a value, draw the value into the picture using FillPixel
                    FillPixel(Picture, i, 11 - Pattern[Synth, i], PixelSize);
                }
            }

            // Refresh picture to show the updates
            Picture.Refresh();
        }

        // Fills a pixel (not exactly one pixel, but rather a square of length PixelSize)
        private void FillPixel(PictureBox Picture, int x, int y, int PixelSize)
        {
            for (int i = 0; i < PixelSize; i++)
            {
                for (int j = 0; j < PixelSize; j++)
                {
                    // Nested loops to fill a square
                    int newx = x * PixelSize + i;
                    int newy = y * PixelSize + j;
                    ((Bitmap)Picture.Image).SetPixel(newx, newy, Color.FromArgb(51,77,90));
                }
            }
        }

        #endregion

        #endregion

        #region Waveform Manipulation

        // Reads the bytes from the requested file
        public byte[] ReadBytes(string FileName)
        {
            string ReadFilePath = Application.StartupPath + "/Data Library/" + FileName + ".wav";
            if (File.Exists(ReadFilePath))
            {
                // Returns the bytes in the file if file exists
                return File.ReadAllBytes(ReadFilePath);
            }
            else
            {
                // Else return an empty array (so that the application does not crash)
                return new byte[500];
            }
        }

        // Process to add two Waveforms with a certain delay
        private void Add(byte[] TargetWav, byte[] Addend, int Delay)
        {
            // Max is the uppermost value in the waveform
            const int Max = 32767;

            // Min is the lowermost value in the waveform
            const int Min = 32768;

            // Delay must not be any other value than a multiple of 4, since a sample is 4 bytes long
            Delay = Convert.ToInt32(Delay / 4) * 4;

            // Start writing after the header
            for (int i = MetaEnd; i < Addend.Length -  SampleCut; i += 2)
            {
                // Make sure writing doesnt occur outside of the array
                if (i + Delay < TargetWav.Length - 1)
                {
                    int TargetValue = Read(TargetWav, i + Delay);
                    int AddendValue = Read(Addend, i);

                    // Check if the addition causes the waveform to be in the upper or lower half
                    if (TargetValue + AddendValue > 65535)
                    {
                        // If both are in the lower half, clipping may occur
                        if (TargetValue >= Min & AddendValue >= Min & TargetValue + AddendValue - 65536 < Min)
                        {
                            // Check for clipping and apply Min for any clipping at the bottom
                            Write(TargetWav, Min, i + Delay);
                        }
                        else
                        {
                            // Otherwise do normal addition
                            //Target + Addend - 65536 is equivalent to (Target - 32768) + (Addend - 32768)
                            Write(TargetWav, TargetValue + AddendValue - 65536, i + Delay);
                        }
                    }
                    else
                    {
                        // If both are in the upper half, clipping may occur
                        if (TargetValue <= Max & AddendValue <= Max & TargetValue + AddendValue > Max)
                        {
                            // Check for clipping and apply Max for any clipping at the top
                            Write(TargetWav, Max, i + Delay);
                        }
                        else
                        {
                            // Otherwise do normal addition
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

        #region Instrument Patterns

        #region Synth Pattern

        // Generate the pattern for the synth
        private void GenerateSynthPattern()
        {
            // Console write for testing
            Console.WriteLine();
            Console.WriteLine("GENERATING THE SYNTH PATTERN -----------------------");
            Console.WriteLine();

            bool PrevGoodNote = false;
            for (int i = 0; i < RepeatLength * TickPerBeat; i++)
            {
                // -1 indicates when the synth does not play
                Pattern[Synth, i] = -1;
            }
            int j = 0;
            int NewNote = random.Next(0, 12);

            // Go through all items in the array Pattern[]
            while (j < RepeatLength * TickPerBeat)
            {
                int CurrentChordNumber = Convert.ToInt32(Math.Floor(Convert.ToDecimal(j / 16)));
                int CurrentChordKey = Chord("P", CurrentChordNumber).SelectedIndex;
                int CurrentChordThird = Chord("T", CurrentChordNumber).SelectedIndex;

                // LegalNote is initialised to 0
                bool LegalNote = false;
                int PitchStep = 0;

                // Keep generating new notes until LegalNote = true
                while (LegalNote == false)
                {
                    // Generate a new PitchStep
                    PitchStep = GenerateNotePitch();

                    // If note is outside the range (one octave) do not set LegalNote to true
                    if (NewNote + PitchStep >= 0 & NewNote + PitchStep < 12)
                    {
                        // Check if riff should be optimised to chords
                        if (ChordsCleared() == true)
                        {
                            // No chords means the riff uses notes froma randomised scale
                            // A scale is created using the key
                            int[] ScaleArray = CreateScaleArray(KeyPitch, KeyThird);

                            // Set LegalNote to true if the note is found in the scale
                            if (ScaleArray[NewNote + PitchStep] == 1) LegalNote = true;
                        }
                        else
                        {
                            // Chords are filled out, so CheckGoodNote needs to be used
                            // A scale is created using the current chord
                            int[] ScaleArray = CreateScaleArray(CurrentChordKey, CurrentChordThird);

                            // For LegalNote to be true, the note must be in the scale, and be a "good" note
                            // If the previous note was "good", the current one need not be
                            if (ScaleArray[NewNote + PitchStep] == 1 & (PrevGoodNote == true ||
                                CheckGoodNote(CurrentChordNumber, NewNote + PitchStep) == true))
                            {
                                LegalNote = true;
                            }
                        }
                    }
                }
                NewNote = NewNote + PitchStep;

                // Make the previous note good if the current one is, and not good otherwise
                PrevGoodNote = CheckGoodNote(CurrentChordNumber, NewNote);

                // Console write for testing
                Console.WriteLine("Note position: " + j + ", Note pitch: " + NewNote);

                // Note will be added to Pattern[]
                Pattern[Synth, j] = NewNote;

                // Time until the next note is indicated by j, and incremented by GenerateNoteTime()
                j = j + GenerateNoteTime();

                // Increment NoteCount - this will be used in the Midi sections
                NoteCount += 1;
            }                                  
        }

        // Check whether the inputted note is "good"
        public bool CheckGoodNote(int ChordNumber, int Pitch)
        {
            // Create an array that will contain he three notes that make up the chord
            int[] ChordNotes = new int[3];
            int ChordPitch = Chord("P", ChordNumber).SelectedIndex;
            int ChordThird = Chord("T", ChordNumber).SelectedIndex;
            CreateChordNotes(ChordNotes, ChordPitch, ChordThird);

            // If the note matches any of the notes in the chord, return true
            for (int i = 0; i < 3; i++)
            {
                if (ChordNotes[i] == Pitch) return true;
            }

            // Otherwise return false
            return false;
        }

        // Repeat the synth pattern
        private void RepeatSynthPattern()
        {
            // For each number of repeats
            for (int Repeat = 0; Repeat < SongBeatLength / RepeatLength; Repeat++)
            {
                // For each note

                for (int i = 0; i < RepeatLength * TickPerBeat; i++)
                {
                    Pattern[Synth, i + (RepeatLength * TickPerBeat * Repeat)] = Pattern[Synth, i];
                }
            }
        }

        // Generate the random time intervals between notes
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

        // Generate the random pitch intervals between notes
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

        #region Ducking Pattern

        // Add the ducking to the main waveform
        private void AddDucking(byte[] TargetWav, int[,] Pattern)
        {
            decimal SamplesPerTick = SamplesPerMinute / BpmBar.Value;
            for (int i = 0; i < PatternLength; i++)
            {
                // For each kick, apply ducking once
                if (Pattern[Ducking, i] == 1)
                {
                    Duck(TargetWav, Convert.ToInt32((SamplesPerTick) * i + MetaEnd));
                }
            }
        }

        // Apply ducking to specified time
        private void Duck(byte[] TargetWav, int DuckTime)
        {
            // i is a decimal between 0 and 1 used in a multiplication to simulate ducking
            double i = 0;

            // j is the time in samples from when the ducking started
            int j = 0;

            // Both i and j are incremented until i >= 1
            while (i < 1)
            {
                // Do not apply ducking outside of the array
                if (DuckTime + j < TargetWav.Length - 1)
                {
                    if (TargetWav[DuckTime + j] < 127)
                    {
                        // If the sample is in the upeer half, multiply by i to duck
                        double Value = TargetWav[DuckTime + j] * i;
                        TargetWav[DuckTime + j] = Convert.ToByte(Value);
                    }
                    else
                    {
                        // If the sample is in the lower half, 255 needs to be subtracted first
                        // Then it can be multiplied by i, and have 255 added back
                        double Value = ((TargetWav[DuckTime + j] - 255) * i) + 255;
                        TargetWav[DuckTime + j] = Convert.ToByte(Value);
                    }
                }

                // Both i and j are incremented, creating a linear ducking
                i = i + ReleaseRate;
                j++;
            }
        }

        #endregion

        #region Other Patterns

        // Deletes Snares and Hats in the first half of the preview
        private void DeleteSnareHat()
        {
            for (int i = 0; i < PatternLength / 2; i++)
            {
                // Set all items to 0 in the first half
                Pattern[Snare, i] = 0;
                Pattern[OpenHat, i] = 0;
                Pattern[ClosedHat, i] = 0;
            }

            // Apply the rhythmic effect on the kick and ducking channel
            Pattern[Kick, PatternLength / 2 - 4] = 0;
            Pattern[Kick, PatternLength / 2 + 2] = 1;
            Pattern[Ducking, PatternLength / 2 + 2] = 1;
        }

        // Create the pattern for the kick
        private void GenerateKickPattern()
        {
            for (int i = 0; i < PatternLength; i++)
            {
                // For every fourth tick (every beat)
                if (i % 4 == 0)
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

        // Create the pattern for the snare
        private void GenerateSnarePattern()
        {
            for (int i = 0; i < PatternLength; i++)
            {
                // For every second fourth tick
                if ((i + 4) % 8 == 0) Pattern[Snare, i] = 1;
                else Pattern[Snare, i] = 0;
            }
        }

        // Create the pattern for the Open Hat
        private void GenerateOHatPattern()
        {
            for (int i = 0; i < PatternLength; i++)
            {
                // In between every beat
                if ((i + 2) % 4 == 0) Pattern[OpenHat, i] = 1;
                else Pattern[OpenHat, i] = 0;
            }
        }

        // Create the pattern for the Closed Hat
        private void GeneratecHatPattern()
        {
            for (int i = 0; i < PatternLength; i++)
            {
                // In between every half beat
                if (!(i % 2 == 0))
                {
                    // 50% chance of the hat playing
                    Pattern[ClosedHat, i] = random.Next(0, 2);
                }
            }
        }
        
        #endregion

        #endregion

        #region Chords

        #region Chord previewing

        // Preview All Chords button
        private void PreviewChordsButton_Click(object sender, EventArgs e)
        {
            // Display error if Data Library is not found
            if (DataLibraryExists() == false) return;

            // If chords are not fully filled out
            if (ChordsCompleted() == false)
            {
                // Display error
                MessageBox.Show(MissingChordsErrorMsg);
                return;
            }

            // Create a header using CreateHeader()
            decimal SamplesPerTick = SamplesPerMinute / BpmBar.Value;
            byte[] ChordPreview = new byte[Convert.ToInt32((SamplesPerTick) * TickPerBeat * 16)];
            CreateHeader(ChordPreview);

            // Make the loading bar visible to indicate that the application is working
            ProgressBar.Visible = true;

            // Create the waveforms for the chord preview
            CreateChordData(ChordPreview);
            
            // Hide loading bar when finished
            ProgressBar.Visible = false;

            // Write waveform to a WAV file
            string ChordFilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".wav");
            System.IO.File.WriteAllBytes(ChordFilePath, ChordPreview);

            // Play created file
            StartStopSound("start", ChordFilePath);
        }

        // Preview individual chords
        private void PlayChord(int Number)
        {
            if (Chord("P", Number).SelectedIndex == -1 || Chord("T", Number).SelectedIndex == -1)
            {
                // If the chord is not filled out, display error and do not continue with algorithm
                MessageBox.Show(MissingChordsErrorMsg);
                return;
            }

            // Using Chord(), find the file name of the chord that is filled out
            string ChordPitch = Chord("P", Number).SelectedItem.ToString();
            string ChordThird = Chord("T", Number).SelectedItem.ToString();
            string FileName = ChordPitch + ChordThird;

            // Play corresponding file from the Data Library
            StartStopSound("start", Application.StartupPath + "/Data Library/" + FileName + ".wav");
        }

        // Preview the first chord
        private void PlayChord1_Click(object sender, EventArgs e)
        {
            // Error trapping for when the Data Library does not exist
            if (DataLibraryExists() == false) return;
            PlayChord(0);
        }

        // Preview the second chord
        private void PlayChord2_Click(object sender, EventArgs e)
        {
            // Error trapping for when the Data Library does not exist
            if (DataLibraryExists() == false) return;
            PlayChord(1);
        }

        // Preview the third chord
        private void PlayChord3_Click(object sender, EventArgs e)
        {
            // Error trapping for when the Data Library does not exist
            if (DataLibraryExists() == false) return;
            PlayChord(2);
        }

        // Preview the fourth chord
        private void PlayChord4_Click(object sender, EventArgs e)
        {
            // Error trapping for when the Data Library does not exist
            if (DataLibraryExists() == false) return;
            PlayChord(3);
        }

        #endregion

        #region Audio Constructoin

        // Creates the waveform for the all chords
        private void CreateChordData(byte[] Chords)
        {
            // Reset the progress bar
            ProgressBar.Value = 0;
            decimal SamplesPerTick = SamplesPerMinute / BpmBar.Value;
            int Beat = Convert.ToInt32((SamplesPerTick) * TickPerBeat);

            // For each chord
            for (int i = 0; i < 4; i++)
            {
                // For each beat
                for (int j = 0; j < 4; j++)
                {
                    // Selecting the correct chord using i
                    string ChordPitch = Chord("P", i).SelectedItem.ToString();
                    string ChordThird = Chord("T", i).SelectedItem.ToString();
                    byte[] Bytes = ReadBytes(ChordPitch + ChordThird);

                    // Getting the correct time using i and j
                    int Delay = (Beat * 4 * i) + (Beat * j);

                    // Add the waveform of the chord to the main waveform
                    Add(Chords, Bytes, Delay);

                    // Update the progress bar
                    ProgressBar.Value += 256 / 16;
                }
            }
        }
        
        #endregion

        #region Notes

        // Creates an array that contains information about a particular scale
        public int[] CreateScaleArray(int LocalKey, int LocalThird)
        {
            // Default scales
            int[] MajorArray = new int[12] { 1, 0, 1, 0, 1, 1, 0, 1, 0, 1, 0, 1 };
            int[] MinorArray = new int[12] { 1, 0, 1, 1, 0, 1, 0, 1, 1, 0, 1, 0 };

            int[] SelectedArray = new int[12];
            int[] Output = new int[12];

            // Assign correct scale to the selected scale
            if (LocalThird == 0) SelectedArray = MajorArray;
            else SelectedArray = MinorArray;
            
            // Shift the scale by certain amount to create the scale in the requested key                           
            for (int i = 0; i < SelectedArray.Length; i++)
            {
                if (i - LocalKey >= 0)
                {
                    Output[i] = SelectedArray[i - LocalKey];
                }
                else
                {
                    // Shifting makes the values go beyond the end of the array
                    // So wrap around by subtracting 12
                    Output[i] = SelectedArray[i - LocalKey + 12];
                }
            }

            // Return the output array to be used
            return Output;
        }

        // Creates an array that contains information about the notes in a particular chord
        private void CreateChordNotes(int[] ChordNotes, int ChordPitch, int ChordThird)
        {
            // Default chords
            int[] MajorDefault = new int[3] { 0, 4, 7 };
            int[] MinorDefault = new int[3] { 0, 3, 7 };
            int[] SelectedDefault = new int[3];

            // Assign correct default chord
            if (ChordThird == 0) SelectedDefault = MajorDefault;
            else SelectedDefault = MinorDefault;

            // Shift values up to create the chord notes in the requested pitch and third
            for (int i = 0; i < 3; i++)
            {
                if (SelectedDefault[i] + ChordPitch < 12)
                {
                    ChordNotes[i] = SelectedDefault[i] + ChordPitch;
                }
                else
                {
                    // Shifting can make the values exceed 11, so wrap around by subtracting 12
                    ChordNotes[i] = SelectedDefault[i] + ChordPitch - 12;
                }
            }
        }

        #endregion

        #endregion

        #region Midi

        #region Midi Melody

        // Create the events for the melody file
        private void CreateMidiMelodyEvents(byte[] Midi)
        {
            // EventCount is used to keep track of event position
            int EventCount = 0;
            for (int i = 0; i < SongBeatLength; i++)
            {
                // For each item that is not = -1 (essentially for each note in the riff)
                if (Pattern[Synth, i] != -1)
                {
                    byte j = 1;

                    // Find the number of ticks until the next note
                    while (i + j < SongBeatLength & Pattern[Synth, i + j] == -1)
                    {
                        j += 1;
                    }

                    // Write the midi event to an array using WriteMidiEvent()
                    WriteMidiEvent(Pattern[Synth, i], j, Midi, EventCount);

                    // Increment by 2 because WriteMidiEvent() creates two events
                    EventCount += 2;
                }
            }
        }

        // Write a Midi event into an array (both on and off)
        private void WriteMidiEvent(int Pitch, byte EventLength, byte[] Midi, int EventCount)
        {
            // Bytes that are written are, in order: "note on" control, pitch, velocity and event length
            // 60 is added to the pitch so that the midi is at a reasonable octave
            // Velocity is set to max
            Midi[MHeadSize + (EventCount * 4)] = 144;
            Midi[MHeadSize + (EventCount * 4) + 1] = Convert.ToByte(60 + Pitch);
            Midi[MHeadSize + (EventCount * 4) + 2] = 127;
            Midi[MHeadSize + (EventCount * 4) + 3] = EventLength;

            // Bytes that are weritten are, in order: "note off" control, pitch, velocity and event length
            // Event length for the "note off" controls are 0 because the next note is to start immediately after
            Midi[MHeadSize + (EventCount * 4) + 4] = 128;
            Midi[MHeadSize + (EventCount * 4) + 5] = Convert.ToByte(60 + Pitch);
            Midi[MHeadSize + (EventCount * 4) + 6] = 127;
            Midi[MHeadSize + (EventCount * 4) + 7] = 0;
        }

        // Create the Midi Melody file
        private void CreateMidiMelodyFile(byte[] Midi)
        {
            // Copy the header from the reference file
            string FilePath = Application.StartupPath + "/Data Library/" + MIDIReferenceHeader + ".mid";
            byte[] ReferenceHeader = File.ReadAllBytes(FilePath);
            for (int i = 0; i < MHeadSize; i++)
            {
                Midi[i] = ReferenceHeader[i];
            }

            // Modify the ticks per beat indicator
            Midi[13] = TickPerBeat;

            // Modify the length of the file
            Midi[48] = Convert.ToByte(NoteCount * 8 + 15);

            // Write the end of track indicator after the events
            Midi[MHeadSize + (NoteCount * 8)] = 255;
            Midi[MHeadSize + (NoteCount * 8) + 1] = 47;
            Midi[MHeadSize + (NoteCount * 8) + 2] = 0;

            // Show "Save As" dialogue for the user to save the file in a location of choice
            SaveFileDialog SaveMelody = new SaveFileDialog();

            // Default filename
            SaveMelody.FileName = FileMidiMelody;

            // File type
            SaveMelody.Filter = "MIDI Files (*.mid)|*.mid";
            if (SaveMelody.ShowDialog() == DialogResult.OK)
            {
                // If user clicks save, write the Midi file
                File.WriteAllBytes(SaveMelody.FileName, Midi);
            }
        }

        #endregion

        #region Midi Chords

        // Create the events for the chords
        private void CreateMidiChordEvents(byte[] MidiChords)
        {
            int[] MidiChordEvents = new int[3];

            // For each chord
            for (int i = 0; i < 4; i++)
            {
                // Create the chord notes using CreateChordNotes()
                int ChordPitch = Chord("P", i).SelectedIndex;
                int ChordThird = Chord("T", i).SelectedIndex;
                CreateChordNotes(MidiChordEvents, ChordPitch, ChordThird);

                // For each note in the chord, start the notes
                for (int j = 0; j < 3; j++)
                {
                    // Bytes that are written are, in order: "note on" control, pitch, velocity and event length
                    // Event length is 0 because all notes start at the same time
                    int Delay = MHeadSize + (i * 24) + (j * 4);
                    MidiChords[Delay] = 144;
                    MidiChords[Delay + 1] = Convert.ToByte(60 + MidiChordEvents[j]);
                    MidiChords[Delay + 2] = 127;
                    MidiChords[Delay + 3] = 0;
                }

                // Last note will need to have an event length of 4 (4 beats at 1 tick per beat)
                // This will give the spacing between all the note on controls and all the note off controls
                MidiChords[MHeadSize + (i * 24) + 11] = 4;

                // For each note in the chord, stop the notes
                for (int j = 0; j < 3; j++)
                {
                    // Bytes that are written are, in order: "note off" control, pitch, velocity and event length
                    // Event length here is also 0 because the next chord will start immediately after
                    int Delay = MHeadSize + (i * 24) + (j * 4);
                    MidiChords[Delay + 12] = 128;
                    MidiChords[Delay + 13] = Convert.ToByte(60 + MidiChordEvents[j]);
                    MidiChords[Delay + 14] = 127;
                    MidiChords[Delay + 15] = 0;
                }
            }
        }

        // Create the Midi Chords file
        private void CreateMidiChordFile(byte[] MidiChords)
        {
            // Copy the header from the reference file
            string FilePath = Application.StartupPath + "/Data Library/" + MIDIReferenceHeader + ".mid";
            byte[] ReferenceHeader = File.ReadAllBytes(FilePath);
            for (int i = 0; i < MHeadSize; i++)
            {
                MidiChords[i] = ReferenceHeader[i];
            }

            // Modify the ticks per beat indicator to 1 (there are no notes in between each beat)
            MidiChords[13] = 1;

            // Modify the length of the file
            // 111 = no. of notes (3 * 4) * no. of bytes for two events (2 * 4)
            // + 3 for end of track indicator + 12 for other track indicators
            MidiChords[48] = 111;

            // Modify the channel name indicator to "Chords"
            MidiChords[53] = 67;
            MidiChords[54] = 104;
            MidiChords[55] = 111;
            MidiChords[56] = 114;
            MidiChords[57] = 100;
            MidiChords[58] = 115;
            MidiChords[59] = 32;

            // Write the end of track indicator after the events
            MidiChords[MHeadSize + 96] = 255;
            MidiChords[MHeadSize + 97] = 47;
            MidiChords[MHeadSize + 98] = 0;

            // Show "Save As" dialogye for the user to save the file in a location of choice
            SaveFileDialog SaveMelody = new SaveFileDialog();

            // Default filename
            SaveMelody.FileName = FileMidiChords;

            // File type
            SaveMelody.Filter = "MIDI Files (*.mid)|*.mid";
            if (SaveMelody.ShowDialog() == DialogResult.OK)
            {
                // If user clicks save, write Midi file
                File.WriteAllBytes(SaveMelody.FileName, MidiChords);
            }
        }

        #endregion

        #region Midi Main

        // Export Midi button and main algorithm
        private void ExportMIDI_Click(object sender, EventArgs e)
        {
            // Error trapping for when Data Library does not exist
            if (DataLibraryExists() == false) return;

            // Error trapping for when the chords are not completely filled out
            if (ChordsCompleted() == false & ChordsCleared() == false)
            {
                // Display error message telling user to fill out missing chords
                MessageBox.Show(MissingChordsErrorMsg);
                return;
            }

            // Each note needs 2 events, each event needs 4 bytes (2 * 4)
            // At the end of the track, 3 bytes are for the end of track indicator
            byte[] Midi = new byte[MHeadSize + (NoteCount * 8) + 3];

            // Create the Midi Melody file
            CreateMidiMelodyEvents(Midi);
            CreateMidiMelodyFile(Midi);

            // If the chords are not cleared, create a Midi Chords file too
            if (ChordsCleared() == false)
            {
                // 99 is like the 111 from before, but without the 12 for other track indicators
                byte[] MidiChords = new byte[MHeadSize + 99];
                CreateMidiChordEvents(MidiChords);
                CreateMidiChordFile(MidiChords);
            }
        }

        #endregion

        #endregion
    }
}

//\o/\\ 