using System;
using System.IO;
using System.Windows.Forms;

namespace WAVToArray
{

    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        #region Initiation

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        Random random = new Random();

        const int SamplesPerMinute = 2646000;
        const int TickPerBeat = 4;
        const int MetaEnd = 44;
        const int SampleCut = 140;
        const int RepeatLength = 8;
        const int SongBeatLength = 32;

        const string MissingChordsErrorMsg = "Error. Please fill out missing chords";
        const string NoFileToPlayError = "File does not exist yet, click 'Create New' to make one.";

        const int Kick = 0;
        const int Snare = 1;
        const int OpenHat = 2;
        const int ClosedHat = 3;
        const int Crash = 4;
        const int Synth = 5;
        const int Duck = 6;

        int[] MajorArray = new int[12] { 1, 0, 1, 0, 1, 1, 0, 1, 0, 1, 0, 1 };
        int[] MinorArray = new int[12] { 1, 0, 1, 1, 0, 1, 0, 1, 1, 0, 1, 0 };
        int[] HarmonicArray = new int[12] { 1, 0, 1, 1, 0, 1, 0, 1, 1, 0, 0, 1 };
        int[] BluesArray = new int[12] { 1, 0, 0, 1, 0, 1, 1, 1, 0, 0, 1, 0 };

        int[,] Pattern = new int[7, SongBeatLength * TickPerBeat];

        #endregion

        #region Main Creation Button

        //CREATE BUTTON

        private void button1_Click(object sender, EventArgs e)
        {
            StartStopSound("stop", Application.StartupPath + "/Created File/Song.wav");

            byte[] Main = new byte[Convert.ToInt32((SamplesPerMinute / bpm.Value) * SongBeatLength * TickPerBeat)];

            int Key = GenerateKey();

            if (ChordsCompleted() == false & NoChordSeqCheck.Checked == false)
            {
                MessageBox.Show(MissingChordsErrorMsg);
            }
            else
            {
                //GENERATE ARRAYS - single dimension arrays are used to structure the track in its core sequencial elements

                GeneratecHatPattern(Pattern);

                GenerateKickPattern(Pattern);

                GenerateSnarePattern(Pattern);
                GenerateOHatPattern(Pattern);

                GenerateSynthPattern(Pattern, Key);
                RepeatSynthPattern(Pattern, RepeatLength);

                if (ProgRhythmCheck.Checked == true)
                {
                    muteSnareHat(Pattern);
                    Pattern[Kick, 60] = 0;
                    Pattern[Kick, 66] = 1;
                }

                //HEADER CONSTRUCTION - making sure the .wav file will not be corrupt

                ConstructHeader(Main, "kick");

                //SONG CONSTRUCTION - converting sequencial information into a single array representing the data in a wav file

                for (int i = 0; i < SongBeatLength * TickPerBeat; i++) //go through all arrays and add data from library files to main blank array accordingly
                {
                    if (Pattern[OpenHat, i] == 1) Add(Main, ReadBytes("ohat"), (Convert.ToInt32(((SamplesPerMinute / bpm.Value) * i) / 2)) * 2, 1);
                    if (Pattern[ClosedHat, i] == 1) Add(Main, ReadBytes("chat"), (Convert.ToInt32(((SamplesPerMinute / bpm.Value) * i) / 2)) * 2, 1);
                    if (Pattern[Synth, i] != 0) Add(Main, ReadBytes(Convert.ToString(Pattern[Synth, i])), (Convert.ToInt32(((SamplesPerMinute / bpm.Value) * i) / 2)) * 2, 1);
                }

                AddDucking(Main, Pattern); //ducking is added before the kick and snare is so that the kick and snare do not get ducked

                //ADD CHORD AUDIO

                if (NoChordSeqCheck.Checked == false & IncludeChordsCheck.Checked == true)
                {
                    byte[] ChordPreview = new byte[Convert.ToInt32((SamplesPerMinute / bpm.Value) * TickPerBeat * 16)];
                    ConstructHeader(ChordPreview, "kick");
                    AdjHeadLengthInfo(ChordPreview, MetaEnd, 40);
                    AdjHeadLengthInfo(ChordPreview, 8, 4);

                    CreateChordData(ChordPreview);
                    

                    Add(Main, ChordPreview, 0, 1);
                }

                for (int i = 0; i < SongBeatLength * TickPerBeat; i++) //same loop repeated, now with kick and snare
                {
                    if (Pattern[Kick, i] == 1) Add(Main, ReadBytes("kick"), (Convert.ToInt32(((SamplesPerMinute / bpm.Value) * i) / 2)) * 2, 1);
                    if (Pattern[Snare, i] == 1) Add(Main, ReadBytes("snare"), (Convert.ToInt32(((SamplesPerMinute / bpm.Value) * i) / 2)) * 2, 1);
                }

                //CONSTRUCT FINAL WAV FILE

                AdjHeadLengthInfo(Main, MetaEnd, 40); //adjust for number of bytes in the file (after the metadata, so 44 bytes onwards)
                AdjHeadLengthInfo(Main, 8, 4); //adjust for size of file (after the end of this sub-section, so 8 bytes onwards)

                string file = Application.StartupPath + "/Created File/Song.wav";
                System.IO.File.WriteAllBytes(file, Main); //convert the array "Main" into a wav file
                if (AutoPlay.Checked == Enabled)
                {
                    StartStopSound("start", file);
                }
            }
        }

        #endregion

        #region Procedures to fix WAV file information

        //ADD VALID HEADER INFORMATION TO INPUTTED ARRAY

        private void ConstructHeader(byte[] TargetData, string HeaderFileName)
        {
            for (int i = 0; i < MetaEnd; i++)
            {
                byte[] Header = ReadBytes(HeaderFileName);
                TargetData[i] = Header[i];
            }
        }

        //ADJUST WAV HEADER FOR ALTERED LENGTH

        private void AdjHeadLengthInfo(byte[] Main, int subtrahend, int startPos)
        {
            char[] hexValue = (Main.Length - subtrahend).ToString("X").ToCharArray();
            char[] hex8 = new char[8];
            for (int i = 0; i < 8 - hexValue.Length; i++)
            {
                hex8[i] = Convert.ToChar("0");
            }
            for (int i = 0; i < hexValue.Length; i++)
            {
                hex8[i + (8 - hexValue.Length)] = hexValue[i];
            }
            Main[startPos] = Convert.ToByte(int.Parse((Convert.ToString(hex8[6]) + Convert.ToString(hex8[7])), System.Globalization.NumberStyles.HexNumber));
            Main[startPos + 1] = Convert.ToByte(int.Parse((Convert.ToString(hex8[4]) + Convert.ToString(hex8[5])), System.Globalization.NumberStyles.HexNumber));
            Main[startPos + 2] = Convert.ToByte(int.Parse((Convert.ToString(hex8[2]) + Convert.ToString(hex8[3])), System.Globalization.NumberStyles.HexNumber));
            Main[startPos + 3] = Convert.ToByte(int.Parse((Convert.ToString(hex8[0]) + Convert.ToString(hex8[1])), System.Globalization.NumberStyles.HexNumber));
        }

        #endregion

        #region UI

        #region Chords

        //CHORD LOCKING

        private void PlayChord1_Click(object sender, EventArgs e)
        {
            PlayChord(0);
        }

        private void PlayChord2_Click(object sender, EventArgs e)
        {
            PlayChord(1);
        }

        private void PlayChord3_Click(object sender, EventArgs e)
        {
            PlayChord(2);
        }

        private void PlayChord4_Click(object sender, EventArgs e)
        {
            PlayChord(3);
        }

        private void LockButtonNone_Click(object sender, EventArgs e)
        {
            if (NoChordSeqCheck.Checked == false) LockChords(true, true, true, true);
        }

        private void LockButton1_Click(object sender, EventArgs e)
        {
            if (NoChordSeqCheck.Checked == false) LockChords(false, true, true, true);
        }

        private void LockButton2_Click(object sender, EventArgs e)
        {
            if (NoChordSeqCheck.Checked == false) LockChords(false, false, true, true);
        }

        private void LockButton3_Click(object sender, EventArgs e)
        {
            if (NoChordSeqCheck.Checked == false) LockChords(false, false, false, true);
        }

        private void LockButton4_Click(object sender, EventArgs e)
        {
            if (NoChordSeqCheck.Checked == false) LockChords(false, false, false, false);
        }

        private void LockChords(bool Chord1, bool Chord2, bool Chord3, bool Chord4)
        {
            ChordKey1.Enabled = Chord1;
            ChordKey2.Enabled = Chord2;
            ChordKey3.Enabled = Chord3;
            ChordKey4.Enabled = Chord4;

            Chord3rd1.Enabled = Chord1;
            Chord3rd2.Enabled = Chord2;
            Chord3rd3.Enabled = Chord3;
            Chord3rd4.Enabled = Chord4;
        }

        private void RandChordsButton_Click(object sender, EventArgs e)
        {
            if (ChordKey1.Enabled == true)
            {
                RandomiseChord(ChordKey1, 12);
                RandomiseChord(Chord3rd1, 2);
            }
            if (ChordKey2.Enabled == true)
            {
                RandomiseChord(ChordKey2, 12);
                RandomiseChord(Chord3rd2, 2);
            }
            if (ChordKey3.Enabled == true)
            {
                RandomiseChord(ChordKey3, 12);
                RandomiseChord(Chord3rd3, 2);
            }
            if (ChordKey4.Enabled == true)
            {
                RandomiseChord(ChordKey4, 12);
                RandomiseChord(Chord3rd4, 2);
            }

        }

        private void NoChordSeqCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (NoChordSeqCheck.Checked == Enabled)
            {
                radioMajor.Enabled = true;
                radioMinor.Enabled = true;
                radioHarmonic.Enabled = true;
                radioBlues.Enabled = true;
                LockChords(false, false, false, false);
                ChordNoticeLabel.Visible = true;
            }
            else
            {
                radioMajor.Enabled = false;
                radioMinor.Enabled = false;
                radioHarmonic.Enabled = false;
                radioBlues.Enabled = false;
                LockChords(true, true, true, true);
                ChordNoticeLabel.Visible = false;
            }
        }

        #endregion

        #region Use created WAV file (access, play)

        //START OR STOP PREVIEW

        private void StartStopSound(string Command, string FilePath)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(FilePath);
            if (Command == "start")
            {
                player.Play();
            }
            else if (Command == "stop")
            {
                player.Stop();
            }
        }

        //STOP PREVIEW BUTTON

        private void StopPreview_Click(object sender, EventArgs e)
        {
            StartStopSound("stop", Application.StartupPath + "/Created File/Song.wav");
        }

        //PLAY (BUTTON)

        private void PlayButton_Click(object sender, EventArgs e)
        {
            if (File.Exists(Application.StartupPath + "/Created File/Song.wav"))
            {
                StartStopSound("start", Application.StartupPath + "/Created File/Song.wav");
            }
            else
            {
                MessageBox.Show(NoFileToPlayError);
            }
        }

        //LOCATE CREATED FILE (BUTTON)

        private void OpenFileLocButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Application.StartupPath + "/Created File");
        }

        #endregion

        #region Exit application

        private void ExitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #endregion

        #region WAV manipulation in arrays

        //READ RAW DATA FROM WAV FILES IN THE LIBRARY

        public byte[] ReadBytes(string filename)
        {
            return File.ReadAllBytes(Application.StartupPath + "/Data Library/" + filename + ".wav");
        }

        //ADDING THE SAMPLE FROM THE LIBRARY TO THE MAIN TRACK

        private void Add(byte[] TargetData, byte[] Addend, int delay, double amp)
        {
            for (int i = MetaEnd; i < Addend.Length - SampleCut; i++)
            {
                if (i + delay < TargetData.Length)
                {
                    if (TargetData[i + delay] + Addend[i] > 255)
                    {
                        if (TargetData[i + delay] > 127 & Addend[i] * amp > 127 & TargetData[i + delay] + (Addend[i] * amp) - 256 < 128)
                        {
                            TargetData[i + delay] = 128;
                        }
                        else
                        {
                            TargetData[i + delay] = Convert.ToByte(TargetData[i + delay] + (Addend[i] * amp) - 256);
                        }
                    }
                    else
                    {
                        if (TargetData[i + delay] < 128 & Addend[i] * amp < 128 & TargetData[i + delay] + (Addend[i] * amp) > 127)
                        {
                            TargetData[i + delay] = 127;
                        }
                        else
                        {
                            TargetData[i + delay] = Convert.ToByte((TargetData[i + delay] + (Addend[i] * amp)));
                        }
                    }
                }
            }
        }

        #endregion

        #region Generating instrument pattern arrays

        #region Generate Synth Pattern

        public int[] CreateScaleArray(int Key, int TypeFlag)
        {
            int[] SelectedArray = new int[12];
            int[] Output = new int[12];
            switch (TypeFlag)
            {
                case 0: SelectedArray = MajorArray;
                    break;
                case 1: SelectedArray = MinorArray;
                    break;
                case 2: SelectedArray = HarmonicArray;
                    break;
                case 3: SelectedArray = BluesArray;
                    break;
            }
            for (int i = 0; i < SelectedArray.Length; i++)
            {
                if (i - Key >= 0)
                {
                    Output[i] = SelectedArray[i - Key];
                }
                else
                {
                    Output[i] = SelectedArray[i - Key + 12];
                }
            }
            return Output;
        }

        //GENERATING THE RANDOM SYNTH PATTERN

        private void GenerateSynthPattern(int[,] Pattern, int Key)
        {
            for (int i = 0; i < RepeatLength * TickPerBeat; i++)
            {
                Pattern[Synth, i] = 0;
            }
            int j = 0;
            int NewNote = 1;
            NewNote = random.Next(0,12);
            while (j < RepeatLength * TickPerBeat)
            {
                bool LegalNote = false;
                while (LegalNote == false)
                {
                    int PitchStep = GenerateNotePitch();
                    if (NewNote + PitchStep >= 0 & NewNote + PitchStep < 12)
                    {
                        NewNote = NewNote + PitchStep;
                        if (NoChordSeqCheck.Checked == true)
                        {
                            if (radioMajor.Checked == Enabled & CreateScaleArray(Key, 0)[NewNote] == 1) LegalNote = true; //TypeFlag = 0 means Major scale
                            else if (radioMinor.Checked == Enabled & CreateScaleArray(Key, 1)[NewNote] == 1) LegalNote = true; //TypeFlag = 1 means Minor scale
                            else if (radioHarmonic.Checked == Enabled & CreateScaleArray(Key, 2)[NewNote] == 1) LegalNote = true; //TypeFlag = 2 means Harmonic scale
                            else if (radioBlues.Checked == Enabled & CreateScaleArray(Key, 3)[NewNote] == 1) LegalNote = true; //TypeFlag = 3 means Blues scale
                        }
                        else
                        {
                            if (ChordArray("Third", 0) == 0 & CreateScaleArray(ChordArray("Chord", Convert.ToInt16(Math.Floor(Convert.ToDecimal(j / 8)))), 0)[NewNote] == 1) LegalNote = true;
                            if (ChordArray("Third", 0) == 1 & CreateScaleArray(ChordArray("Chord", Convert.ToInt16(Math.Floor(Convert.ToDecimal(j / 8)))), 1)[NewNote] == 1) LegalNote = true;
                        }
                    }
                }
                Pattern[Synth, j] = NewNote;
                j = j + GenerateNoteTime();
            }
        }

        //REPEAT SYNTH PATTERN

        private void RepeatSynthPattern(int[,] Pattern, int Length)
        {
            if (Length < SongBeatLength & SongBeatLength % Length == 0)
            {
                for (int Repeat = 0; Repeat < SongBeatLength / Length; Repeat++)
                {
                    for (int i = 0; i < Length * TickPerBeat; i++)
                    {
                        Pattern[Synth, i + (Length * TickPerBeat * Repeat)] = Pattern[Synth, i];
                    }
                }
            }
            else
            {
                MessageBox.Show("Song Length not compatible with Repeat Length");
            }
        }

        //GENERATING THE RANDOM TIME INTERVAL BETWEEN SYNTH NOTES

        public int GenerateNoteTime()
        {
            int r = random.Next(1, 180);

            if (0 < r & r <= 16) return 1;
            if (16 < r & r <= 77) return 2;
            if (77 < r & r <= 159) return 3;
            if (159 < r & r <= 176) return 4;
            if (176 < r & r <= 177) return 6;
            if (177 < r & r <= 178) return 7;
            if (178 < r & r <= 179) return 9;
            return 3;
        }

        //GENERATING THE RANDOM PITCH INTERVAL BETWEEN SYNTH NOTES

        public int GenerateNotePitch()
        {
            int r = random.Next(1, 165);

            if (0 < r & r <= 1) return -12;
            if (1 < r & r <= 2) return -10;
            if (2 < r & r <= 3) return -9;
            if (3 < r & r <= 5) return -8;
            if (5 < r & r <= 6) return -7;
            if (6 < r & r <= 12) return -5;
            if (12 < r & r <= 18) return -4;
            if (18 < r & r <= 25) return -3;
            if (25 < r & r <= 36) return -2;
            if (36 < r & r <= 46) return -1;
            if (46 < r & r <= 121) return 0;
            if (121 < r & r <= 132) return 1;
            if (132 < r & r <= 148) return 2;
            if (148 < r & r <= 157) return 3;
            if (157 < r & r <= 158) return 5;
            if (158 < r & r <= 159) return 7;
            if (159 < r & r <= 161) return 10;
            if (161 < r & r <= 162) return 11;
            if (162 < r & r <= 164) return 12;
            return 0;
        }

        #endregion

        #region Ducking

        //DECLARE DUCKING TIME

        private void AddDucking(byte[] TargetData, int[,] Pattern)
        {
            for (int i = 0; i < SongBeatLength * TickPerBeat; i++)
            {
                if (Pattern[Kick, i] == 1)
                {
                    duck(TargetData, Convert.ToInt32((SamplesPerMinute / bpm.Value) * i + MetaEnd));
                }
            }
        }

        //DUCK

        private void duck(byte[] TargetData, int duckTime)
        {
            const double releaseRate = 0.006 / 256;
            double i = 0;
            int j = 0;
            while (i < 1)
            {
                if (duckTime + j < TargetData.Length)
                {
                    if (TargetData[duckTime + j] < 127)
                    {
                        TargetData[duckTime + j] = Convert.ToByte(Convert.ToInt32(TargetData[duckTime + j] * i));
                    }
                    else
                    {
                        TargetData[duckTime + j] = Convert.ToByte(Convert.ToInt32(((TargetData[duckTime + j] - 255) * i) + 255));
                    }
                }
                i = i + releaseRate;
                j++;
            }
        }

        #endregion

        #region Other instruments

        //SPECIAL RHYTHMIC EFFECT

        private void muteSnareHat(int[,] Pattern)
        {
            for (int i = 0; i < SongBeatLength * TickPerBeat / 2; i++)
            {
                Pattern[Snare, i] = 0;
                Pattern[OpenHat, i] = 0;
                Pattern[ClosedHat, i] = 0;
            }
        }

        //GENERATING STRAIGHT KICK BEAT

        private void GenerateKickPattern(int[,] Pattern)
        {
            for (int i = 0; i < SongBeatLength * TickPerBeat; i++)
            {
                if (i % 4 == 0) //for every fourth count
                {
                    Pattern[Kick, i] = 1;
                    Pattern[Duck, i] = 1;
                }
                else
                {
                    Pattern[Kick, i] = 0;
                    Pattern[Duck, i] = 0;
                }
            }
        }

        //GENERATING SNARE EVERY SECOND BEAT

        private void GenerateSnarePattern(int[,] Pattern)
        {
            for (int i = 0; i < SongBeatLength * TickPerBeat; i++)
            {
                if ((i + 4) % 8 == 0) Pattern[Snare, i] = 1;
                else Pattern[Snare, i] = 0;
            }
        }

        //GENERATE OFF-BEAT OPEN HAT PATTERN

        private void GenerateOHatPattern(int[,] Pattern)
        {
            for (int i = 0; i < SongBeatLength * TickPerBeat; i++)
            {
                if ((i + 2) % 4 == 0) Pattern[OpenHat, i] = 1;
                else Pattern[OpenHat, i] = 0;
            }
        }

        //GENERATING RANDOM CLOSED HAT PATTERN

        private void GeneratecHatPattern(int[,] Pattern)
        {
            for (int i = 0; i < SongBeatLength * TickPerBeat; i++)
            {
                if (!(i % 2 == 0))
                {
                    Pattern[ClosedHat, i] = random.Next(0, 2);
                }
            }
        }

        //GENERATE RANDOM KEY

        public int GenerateKey()
        {
            if (DropDownKey.SelectedIndex == 12)
            {
                return random.Next(0, 12);
            }
            return DropDownKey.SelectedIndex;
        }

        //EXPERIMENTAL RANDOM KICK INTERVAL TIMINGS PATTERN

        private void RandomKickPattern(int[,] Pattern)
        {
            for (int i = 0; i < SongBeatLength * TickPerBeat; i++)
            {
                Pattern[Kick, i] = 0;
            }
            int j = 0;
            while (j < SongBeatLength * TickPerBeat)
            {
                Pattern[Kick, j] = 1;
                j = j + random.Next(2, 10);
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
                MessageBox.Show(MissingChordsErrorMsg);
            }
            else
            {
                byte[] ChordPreview = new byte[Convert.ToInt32((SamplesPerMinute / bpm.Value) * TickPerBeat * 16)];
                ConstructHeader(ChordPreview, "kick");
                AdjHeadLengthInfo(ChordPreview, MetaEnd, 40);
                AdjHeadLengthInfo(ChordPreview, 8, 4);

                CreateChordData(ChordPreview);

                string file = Application.StartupPath + "/Created File/Chords.wav";
                System.IO.File.WriteAllBytes(file, ChordPreview);
                StartStopSound("start", file);
            }
        }

        //PREVIEW INDIVIDUAL CHORDS

        private void PlayChord(int Number)
        {
            if (ChordsCompleted() == true)
            {
                byte[] ChordPreview = new byte[Convert.ToInt32((SamplesPerMinute / bpm.Value) * TickPerBeat * 2)];
                ConstructHeader(ChordPreview, "kick");
                AdjHeadLengthInfo(ChordPreview, MetaEnd, 40);
                AdjHeadLengthInfo(ChordPreview, 8, 4);

                for (int i = 0; i < 3; i++)
                {
                    Add(ChordPreview, ReadBytes("p" + Convert.ToString(ConstructChord(ChordArray("Chord", Number), ChordArray("Third", Number))[i])), 0, 1);
                }

                string file = Application.StartupPath + "/Created File/Chords.wav";
                System.IO.File.WriteAllBytes(file, ChordPreview);
                StartStopSound("start", file);
            }
            else
            {
                MessageBox.Show(MissingChordsErrorMsg);
            }
        }

        #endregion

        #region Audio constructoin

        //CHORD VOICINGS

        public int[] ConstructChord(int Key, int Third)
        {
            switch (Convert.ToString(Third) + Convert.ToString(Key))
            {
                //MAJOR CHORDS
                case "00": return new int[3] { 0, 7, 16 };
                case "01": return new int[3] { 1, 8, 17 };
                case "02": return new int[3] { 2, 9, 18 };
                case "03": return new int[3] { 3, 10, 19 };
                case "04": return new int[3] { 4, 11, 20 };
                case "05": return new int[3] { 0, 9, 17 };
                case "06": return new int[3] { 1, 10, 18 };
                case "07": return new int[3] { 2, 11, 19 };
                case "08": return new int[3] { 0, 8, 15 };
                case "09": return new int[3] { 1, 9, 16 };
                case "010": return new int[3] { 2, 10, 17 };
                case "011": return new int[3] { 3, 11, 18 };

                //MINOR CHORDS
                case "10": return new int[3] { 0, 7, 15 };
                case "11": return new int[3] { 1, 8, 16 };
                case "12": return new int[3] { 2, 9, 17 };
                case "13": return new int[3] { 3, 10, 18 };
                case "14": return new int[3] { 4, 11, 19 };
                case "15": return new int[3] { 0, 8, 17 };
                case "16": return new int[3] { 1, 9, 18 };
                case "17": return new int[3] { 2, 10, 19 };
                case "18": return new int[3] { 3, 11, 20 };
                case "19": return new int[3] { 0, 9, 16 };
                case "110": return new int[3] { 1, 10, 17 };
                case "111": return new int[3] { 2, 11, 18 };
            }
            return new int[] { -1 };
        }

        //WRITE WAV-READY AUDIO TO ARRAYS

        private void CreateChordData(byte[] Chords)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    //MessageBox.Show(Convert.ToString(ConstructChord(ChordArray(i, "Chord"), ChordArray(i, "Third"))[j]));
                    Add
                    (
                        Chords,
                        ReadBytes
                        (
                            "p" +
                            Convert.ToString
                            (
                                ConstructChord
                                (
                                    ChordArray("Chord", Convert.ToInt32(i)),
                                    ChordArray("Third", Convert.ToInt32(i))
                                )[j]
                            )
                        ),
                        Convert.ToInt32((SamplesPerMinute / bpm.Value) * i * 8) * 2, //same as ...* 16) / 2) * 2)
                        1
                    );
                }
            }
        }

        #endregion

        #region UI-related

        //RENDOMISE GIVEN CHORD IN UI

        private void RandomiseChord(ComboBox Chord, int range)
        {
            int FirstChord = ChordArray("Chord", 0);
            int[,] ScaleDegree = new int[2,7] { { 0, 1, 1, 0, 0, 1, 1 }, { 1, -1, 0, 1, 0, 0, 0 } };
            Chord.Text = Convert.ToString(Chord.Items[random.Next(0, range)]);
        }

        //CHECK IF CHORD UI IS COMPLETED

        public bool ChordsCompleted()
        {
            for (int i = 0; i < 4; i++)
            {
                if (ChordArray("Chord", i) == -1)
                {
                    return false;
                }
                else
                {
                    //MessageBox.Show(Convert.ToString(ChordArray(i, "Chord")) + " " + Convert.ToString(i) + " Chord");
                }
                if (ChordArray("Third", i) == -1)
                {
                    return false;
                }
                else
                {
                    //MessageBox.Show(Convert.ToString(ChordArray(i, "Third")) + " " + Convert.ToString(i) + " Third");
                }
            }
            return true;
        }

        //RETURN THE CHORDS IN THE UI

        public int ChordArray(string ChordOrThird, int ChordNumber)
        {
            switch (ChordOrThird + Convert.ToString(ChordNumber))
            {
                case "Chord0": return ChordKey1.SelectedIndex;
                case "Chord1": return ChordKey2.SelectedIndex;
                case "Chord2": return ChordKey3.SelectedIndex;
                case "Chord3": return ChordKey4.SelectedIndex;
                case "Third0": return Chord3rd1.SelectedIndex;
                case "Third1": return Chord3rd2.SelectedIndex;
                case "Third2": return Chord3rd3.SelectedIndex;
                case "Third3": return Chord3rd4.SelectedIndex;
            }
            return -1;
        }

        #endregion

        #endregion

        #region MIDI

        private void CreateMidiFile(int[,] Pattern, byte[] Midi)
        {
            byte[] Header = new byte[14] { 77, 84, 104, 100, 0, 0, 0, 6, 0, 0, 0, 0, 0, 1 };
            for (int i = 0; i < 14; i++)
            {
                Midi[i] = Header[i];
            }
            File.WriteAllBytes(Application.StartupPath + "/copy.mid", Midi);
            MessageBox.Show("Midi file created.");
        }

        private void RiffMidiButton_Click(object sender, EventArgs e)
        {
            byte[] Midi = File.ReadAllBytes(Application.StartupPath + "/untitled.mid");
            CreateMidiFile(Pattern, Midi);
        }

        #endregion
    }
}

//\o/\\