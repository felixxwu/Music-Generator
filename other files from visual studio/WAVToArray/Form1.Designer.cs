namespace WAVToArray
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.bpmLabel = new System.Windows.Forms.Label();
            this.AutoPlay = new System.Windows.Forms.CheckBox();
            this.openFile = new System.Windows.Forms.OpenFileDialog();
            this.KeyLabel = new System.Windows.Forms.Label();
            this.ProgRhythmCheck = new System.Windows.Forms.CheckBox();
            this.ProgressBar = new System.Windows.Forms.ProgressBar();
            this.ChordPitch1 = new System.Windows.Forms.ComboBox();
            this.ChordPitch2 = new System.Windows.Forms.ComboBox();
            this.ChordPitch3 = new System.Windows.Forms.ComboBox();
            this.ChordPitch4 = new System.Windows.Forms.ComboBox();
            this.ChordThird1 = new System.Windows.Forms.ComboBox();
            this.ChordThird2 = new System.Windows.Forms.ComboBox();
            this.ChordThird3 = new System.Windows.Forms.ComboBox();
            this.ChordThird4 = new System.Windows.Forms.ComboBox();
            this.GroupBoxSettings = new System.Windows.Forms.GroupBox();
            this.BPMResetButton = new System.Windows.Forms.Button();
            this.ExitButton = new System.Windows.Forms.Button();
            this.BpmBar = new System.Windows.Forms.TrackBar();
            this.GroupBoxCurrent = new System.Windows.Forms.GroupBox();
            this.ExportWav = new System.Windows.Forms.Button();
            this.ExportMIDI = new System.Windows.Forms.Button();
            this.RandomiseLabel = new System.Windows.Forms.Label();
            this.RiffLabel = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.WrongChordWarning = new System.Windows.Forms.Label();
            this.PlayNewRiff = new System.Windows.Forms.Button();
            this.PlayCurrentRiff = new System.Windows.Forms.Button();
            this.PlayChord3 = new System.Windows.Forms.Button();
            this.StopPreview = new System.Windows.Forms.Button();
            this.RandAllButton = new System.Windows.Forms.Button();
            this.ChordsClearButton = new System.Windows.Forms.Button();
            this.Rand4Button = new System.Windows.Forms.Button();
            this.PlayChord4 = new System.Windows.Forms.Button();
            this.PlayChord1 = new System.Windows.Forms.Button();
            this.Rand3Button = new System.Windows.Forms.Button();
            this.PreviewChordsButton = new System.Windows.Forms.Button();
            this.PlayChord2 = new System.Windows.Forms.Button();
            this.Rand2Button = new System.Windows.Forms.Button();
            this.MyToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.Picture = new System.Windows.Forms.PictureBox();
            this.GroupBoxSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BpmBar)).BeginInit();
            this.GroupBoxCurrent.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Picture)).BeginInit();
            this.SuspendLayout();
            // 
            // bpmLabel
            // 
            this.bpmLabel.AutoSize = true;
            this.bpmLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bpmLabel.Location = new System.Drawing.Point(6, 32);
            this.bpmLabel.Name = "bpmLabel";
            this.bpmLabel.Size = new System.Drawing.Size(158, 16);
            this.bpmLabel.TabIndex = 28;
            this.bpmLabel.Text = "Beats Per Minute: (128)";
            // 
            // AutoPlay
            // 
            this.AutoPlay.AutoSize = true;
            this.AutoPlay.Checked = true;
            this.AutoPlay.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AutoPlay.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AutoPlay.Location = new System.Drawing.Point(9, 126);
            this.AutoPlay.Name = "AutoPlay";
            this.AutoPlay.Size = new System.Drawing.Size(162, 20);
            this.AutoPlay.TabIndex = 69;
            this.AutoPlay.Text = "Preview Automatically";
            this.MyToolTip.SetToolTip(this.AutoPlay, "Play preview as soon as one is created");
            this.AutoPlay.UseVisualStyleBackColor = true;
            // 
            // openFile
            // 
            this.openFile.FileName = "openFileDialog1";
            // 
            // KeyLabel
            // 
            this.KeyLabel.AutoSize = true;
            this.KeyLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.KeyLabel.Location = new System.Drawing.Point(174, 208);
            this.KeyLabel.Name = "KeyLabel";
            this.KeyLabel.Size = new System.Drawing.Size(0, 16);
            this.KeyLabel.TabIndex = 33;
            // 
            // ProgRhythmCheck
            // 
            this.ProgRhythmCheck.AutoSize = true;
            this.ProgRhythmCheck.Checked = true;
            this.ProgRhythmCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ProgRhythmCheck.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ProgRhythmCheck.Location = new System.Drawing.Point(9, 152);
            this.ProgRhythmCheck.Name = "ProgRhythmCheck";
            this.ProgRhythmCheck.Size = new System.Drawing.Size(154, 20);
            this.ProgRhythmCheck.TabIndex = 70;
            this.ProgRhythmCheck.Text = "Progressive Rhythm";
            this.MyToolTip.SetToolTip(this.ProgRhythmCheck, "Mutes hats and snares in the first half of the preview");
            this.ProgRhythmCheck.UseVisualStyleBackColor = true;
            // 
            // ProgressBar
            // 
            this.ProgressBar.Location = new System.Drawing.Point(12, 275);
            this.ProgressBar.Maximum = 256;
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(851, 23);
            this.ProgressBar.TabIndex = 70;
            this.ProgressBar.Visible = false;
            // 
            // ChordPitch1
            // 
            this.ChordPitch1.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.ChordPitch1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ChordPitch1.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChordPitch1.FormattingEnabled = true;
            this.ChordPitch1.Items.AddRange(new object[] {
            "C",
            "C#",
            "D",
            "D#",
            "E",
            "F",
            "F#",
            "G",
            "G#",
            "A",
            "A#",
            "B"});
            this.ChordPitch1.Location = new System.Drawing.Point(259, 96);
            this.ChordPitch1.Name = "ChordPitch1";
            this.ChordPitch1.Size = new System.Drawing.Size(50, 24);
            this.ChordPitch1.TabIndex = 18;
            this.ChordPitch1.SelectedIndexChanged += new System.EventHandler(this.ChordPitch1_SelectedIndexChanged);
            // 
            // ChordPitch2
            // 
            this.ChordPitch2.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.ChordPitch2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ChordPitch2.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChordPitch2.FormattingEnabled = true;
            this.ChordPitch2.Items.AddRange(new object[] {
            "C",
            "C#",
            "D",
            "D#",
            "E",
            "F",
            "F#",
            "G",
            "G#",
            "A",
            "A#",
            "B"});
            this.ChordPitch2.Location = new System.Drawing.Point(315, 96);
            this.ChordPitch2.Name = "ChordPitch2";
            this.ChordPitch2.Size = new System.Drawing.Size(50, 24);
            this.ChordPitch2.TabIndex = 19;
            this.ChordPitch2.SelectedIndexChanged += new System.EventHandler(this.ChordPitch2_SelectedIndexChanged);
            // 
            // ChordPitch3
            // 
            this.ChordPitch3.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.ChordPitch3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ChordPitch3.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChordPitch3.FormattingEnabled = true;
            this.ChordPitch3.Items.AddRange(new object[] {
            "C",
            "C#",
            "D",
            "D#",
            "E",
            "F",
            "F#",
            "G",
            "G#",
            "A",
            "A#",
            "B"});
            this.ChordPitch3.Location = new System.Drawing.Point(371, 96);
            this.ChordPitch3.Name = "ChordPitch3";
            this.ChordPitch3.Size = new System.Drawing.Size(50, 24);
            this.ChordPitch3.TabIndex = 20;
            this.ChordPitch3.SelectedIndexChanged += new System.EventHandler(this.ChordPitch3_SelectedIndexChanged);
            // 
            // ChordPitch4
            // 
            this.ChordPitch4.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.ChordPitch4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ChordPitch4.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChordPitch4.FormattingEnabled = true;
            this.ChordPitch4.Items.AddRange(new object[] {
            "C",
            "C#",
            "D",
            "D#",
            "E",
            "F",
            "F#",
            "G",
            "G#",
            "A",
            "A#",
            "B"});
            this.ChordPitch4.Location = new System.Drawing.Point(427, 96);
            this.ChordPitch4.Name = "ChordPitch4";
            this.ChordPitch4.Size = new System.Drawing.Size(50, 24);
            this.ChordPitch4.TabIndex = 48;
            this.ChordPitch4.SelectedIndexChanged += new System.EventHandler(this.ChordPitch4_SelectedIndexChanged);
            // 
            // ChordThird1
            // 
            this.ChordThird1.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.ChordThird1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ChordThird1.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChordThird1.FormattingEnabled = true;
            this.ChordThird1.Items.AddRange(new object[] {
            "maj",
            "min"});
            this.ChordThird1.Location = new System.Drawing.Point(259, 135);
            this.ChordThird1.Name = "ChordThird1";
            this.ChordThird1.Size = new System.Drawing.Size(50, 24);
            this.ChordThird1.TabIndex = 50;
            this.ChordThird1.SelectedIndexChanged += new System.EventHandler(this.ChordThird1_SelectedIndexChanged);
            // 
            // ChordThird2
            // 
            this.ChordThird2.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.ChordThird2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ChordThird2.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChordThird2.FormattingEnabled = true;
            this.ChordThird2.Items.AddRange(new object[] {
            "maj",
            "min"});
            this.ChordThird2.Location = new System.Drawing.Point(315, 135);
            this.ChordThird2.Name = "ChordThird2";
            this.ChordThird2.Size = new System.Drawing.Size(50, 24);
            this.ChordThird2.TabIndex = 51;
            this.ChordThird2.SelectedIndexChanged += new System.EventHandler(this.ChordThird2_SelectedIndexChanged);
            // 
            // ChordThird3
            // 
            this.ChordThird3.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.ChordThird3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ChordThird3.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChordThird3.FormattingEnabled = true;
            this.ChordThird3.Items.AddRange(new object[] {
            "maj",
            "min"});
            this.ChordThird3.Location = new System.Drawing.Point(371, 135);
            this.ChordThird3.Name = "ChordThird3";
            this.ChordThird3.Size = new System.Drawing.Size(50, 24);
            this.ChordThird3.TabIndex = 52;
            this.ChordThird3.SelectedIndexChanged += new System.EventHandler(this.ChordThird3_SelectedIndexChanged);
            // 
            // ChordThird4
            // 
            this.ChordThird4.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.ChordThird4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ChordThird4.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChordThird4.FormattingEnabled = true;
            this.ChordThird4.Items.AddRange(new object[] {
            "maj",
            "min"});
            this.ChordThird4.Location = new System.Drawing.Point(427, 135);
            this.ChordThird4.Name = "ChordThird4";
            this.ChordThird4.Size = new System.Drawing.Size(50, 24);
            this.ChordThird4.TabIndex = 53;
            this.ChordThird4.SelectedIndexChanged += new System.EventHandler(this.ChordThird4_SelectedIndexChanged);
            // 
            // GroupBoxSettings
            // 
            this.GroupBoxSettings.BackColor = System.Drawing.Color.Transparent;
            this.GroupBoxSettings.Controls.Add(this.BPMResetButton);
            this.GroupBoxSettings.Controls.Add(this.ExitButton);
            this.GroupBoxSettings.Controls.Add(this.BpmBar);
            this.GroupBoxSettings.Controls.Add(this.bpmLabel);
            this.GroupBoxSettings.Controls.Add(this.ProgRhythmCheck);
            this.GroupBoxSettings.Controls.Add(this.AutoPlay);
            this.GroupBoxSettings.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GroupBoxSettings.Location = new System.Drawing.Point(514, 35);
            this.GroupBoxSettings.Name = "GroupBoxSettings";
            this.GroupBoxSettings.Size = new System.Drawing.Size(191, 234);
            this.GroupBoxSettings.TabIndex = 76;
            this.GroupBoxSettings.TabStop = false;
            this.GroupBoxSettings.Text = "Settings";
            // 
            // BPMResetButton
            // 
            this.BPMResetButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.BPMResetButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BPMResetButton.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BPMResetButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(77)))), ((int)(((byte)(90)))));
            this.BPMResetButton.Location = new System.Drawing.Point(52, 85);
            this.BPMResetButton.Name = "BPMResetButton";
            this.BPMResetButton.Size = new System.Drawing.Size(90, 25);
            this.BPMResetButton.TabIndex = 68;
            this.BPMResetButton.Text = "Reset BPM";
            this.MyToolTip.SetToolTip(this.BPMResetButton, "Reset to 128 BPM");
            this.BPMResetButton.UseVisualStyleBackColor = false;
            this.BPMResetButton.Click += new System.EventHandler(this.BPMResetButton_Click);
            // 
            // ExitButton
            // 
            this.ExitButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.ExitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ExitButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ExitButton.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ExitButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(77)))), ((int)(((byte)(90)))));
            this.ExitButton.Location = new System.Drawing.Point(6, 193);
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.Size = new System.Drawing.Size(179, 35);
            this.ExitButton.TabIndex = 71;
            this.ExitButton.Text = "Exit Application";
            this.MyToolTip.SetToolTip(this.ExitButton, "Close programme");
            this.ExitButton.UseVisualStyleBackColor = false;
            this.ExitButton.Click += new System.EventHandler(this.ExitButton_Click);
            // 
            // BpmBar
            // 
            this.BpmBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.BpmBar.LargeChange = 10;
            this.BpmBar.Location = new System.Drawing.Point(6, 51);
            this.BpmBar.Maximum = 200;
            this.BpmBar.Minimum = 50;
            this.BpmBar.Name = "BpmBar";
            this.BpmBar.Size = new System.Drawing.Size(179, 45);
            this.BpmBar.TabIndex = 67;
            this.BpmBar.Value = 128;
            this.BpmBar.Scroll += new System.EventHandler(this.BpmBar_Scroll);
            // 
            // GroupBoxCurrent
            // 
            this.GroupBoxCurrent.BackColor = System.Drawing.Color.Transparent;
            this.GroupBoxCurrent.Controls.Add(this.ExportWav);
            this.GroupBoxCurrent.Controls.Add(this.ExportMIDI);
            this.GroupBoxCurrent.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GroupBoxCurrent.ForeColor = System.Drawing.Color.Black;
            this.GroupBoxCurrent.Location = new System.Drawing.Point(711, 35);
            this.GroupBoxCurrent.Name = "GroupBoxCurrent";
            this.GroupBoxCurrent.Size = new System.Drawing.Size(152, 234);
            this.GroupBoxCurrent.TabIndex = 77;
            this.GroupBoxCurrent.TabStop = false;
            this.GroupBoxCurrent.Text = "Current Riff Controls";
            this.GroupBoxCurrent.Visible = false;
            // 
            // ExportWav
            // 
            this.ExportWav.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.ExportWav.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ExportWav.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ExportWav.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ExportWav.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(77)))), ((int)(((byte)(90)))));
            this.ExportWav.Location = new System.Drawing.Point(6, 54);
            this.ExportWav.Name = "ExportWav";
            this.ExportWav.Size = new System.Drawing.Size(140, 35);
            this.ExportWav.TabIndex = 75;
            this.ExportWav.Text = "Export to WAV";
            this.MyToolTip.SetToolTip(this.ExportWav, "Export current riff and chords to MIDI files");
            this.ExportWav.UseVisualStyleBackColor = false;
            this.ExportWav.Click += new System.EventHandler(this.ExportWav_Click);
            // 
            // ExportMIDI
            // 
            this.ExportMIDI.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.ExportMIDI.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ExportMIDI.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ExportMIDI.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ExportMIDI.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(77)))), ((int)(((byte)(90)))));
            this.ExportMIDI.Location = new System.Drawing.Point(6, 95);
            this.ExportMIDI.Name = "ExportMIDI";
            this.ExportMIDI.Size = new System.Drawing.Size(140, 35);
            this.ExportMIDI.TabIndex = 74;
            this.ExportMIDI.Text = "Export to MIDI";
            this.MyToolTip.SetToolTip(this.ExportMIDI, "Export current riff and chords to MIDI files");
            this.ExportMIDI.UseVisualStyleBackColor = false;
            this.ExportMIDI.Click += new System.EventHandler(this.ExportMIDI_Click);
            // 
            // RandomiseLabel
            // 
            this.RandomiseLabel.AutoSize = true;
            this.RandomiseLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RandomiseLabel.Location = new System.Drawing.Point(256, 35);
            this.RandomiseLabel.Name = "RandomiseLabel";
            this.RandomiseLabel.Size = new System.Drawing.Size(133, 16);
            this.RandomiseLabel.TabIndex = 70;
            this.RandomiseLabel.Text = "Randomise Chords:";
            // 
            // RiffLabel
            // 
            this.RiffLabel.AutoSize = true;
            this.RiffLabel.BackColor = System.Drawing.Color.Transparent;
            this.RiffLabel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RiffLabel.Location = new System.Drawing.Point(12, 278);
            this.RiffLabel.Name = "RiffLabel";
            this.RiffLabel.Size = new System.Drawing.Size(85, 16);
            this.RiffLabel.TabIndex = 101;
            this.RiffLabel.Text = "Current Riff:";
            this.RiffLabel.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.WrongChordWarning);
            this.groupBox1.Controls.Add(this.KeyLabel);
            this.groupBox1.Controls.Add(this.PlayNewRiff);
            this.groupBox1.Controls.Add(this.PlayCurrentRiff);
            this.groupBox1.Controls.Add(this.ChordThird1);
            this.groupBox1.Controls.Add(this.PlayChord3);
            this.groupBox1.Controls.Add(this.ChordPitch2);
            this.groupBox1.Controls.Add(this.StopPreview);
            this.groupBox1.Controls.Add(this.RandAllButton);
            this.groupBox1.Controls.Add(this.ChordThird2);
            this.groupBox1.Controls.Add(this.ChordsClearButton);
            this.groupBox1.Controls.Add(this.ChordPitch3);
            this.groupBox1.Controls.Add(this.Rand4Button);
            this.groupBox1.Controls.Add(this.PlayChord4);
            this.groupBox1.Controls.Add(this.PlayChord1);
            this.groupBox1.Controls.Add(this.Rand3Button);
            this.groupBox1.Controls.Add(this.PreviewChordsButton);
            this.groupBox1.Controls.Add(this.PlayChord2);
            this.groupBox1.Controls.Add(this.Rand2Button);
            this.groupBox1.Controls.Add(this.ChordPitch1);
            this.groupBox1.Controls.Add(this.ChordPitch4);
            this.groupBox1.Controls.Add(this.RandomiseLabel);
            this.groupBox1.Controls.Add(this.ChordThird3);
            this.groupBox1.Controls.Add(this.ChordThird4);
            this.groupBox1.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.Black;
            this.groupBox1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.groupBox1.Location = new System.Drawing.Point(12, 35);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.groupBox1.Size = new System.Drawing.Size(496, 234);
            this.groupBox1.TabIndex = 102;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Main Controls";
            // 
            // WrongChordWarning
            // 
            this.WrongChordWarning.AutoSize = true;
            this.WrongChordWarning.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WrongChordWarning.Location = new System.Drawing.Point(254, 208);
            this.WrongChordWarning.Name = "WrongChordWarning";
            this.WrongChordWarning.Size = new System.Drawing.Size(227, 15);
            this.WrongChordWarning.TabIndex = 71;
            this.WrongChordWarning.Text = "WARNING - some chords may sound off";
            this.WrongChordWarning.Visible = false;
            // 
            // PlayNewRiff
            // 
            this.PlayNewRiff.AccessibleDescription = "Randomise All Chords";
            this.PlayNewRiff.AccessibleRole = System.Windows.Forms.AccessibleRole.ToolTip;
            this.PlayNewRiff.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.PlayNewRiff.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.PlayNewRiff.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.PlayNewRiff.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PlayNewRiff.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(77)))), ((int)(((byte)(90)))));
            this.PlayNewRiff.Location = new System.Drawing.Point(19, 54);
            this.PlayNewRiff.Name = "PlayNewRiff";
            this.PlayNewRiff.Size = new System.Drawing.Size(140, 50);
            this.PlayNewRiff.TabIndex = 0;
            this.PlayNewRiff.Text = "Play New Riff";
            this.MyToolTip.SetToolTip(this.PlayNewRiff, "Create new riff. If chords are filled out, riff is optimised for chords");
            this.PlayNewRiff.UseVisualStyleBackColor = false;
            this.PlayNewRiff.Click += new System.EventHandler(this.PlayNewRiff_Click);
            // 
            // PlayCurrentRiff
            // 
            this.PlayCurrentRiff.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.PlayCurrentRiff.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.PlayCurrentRiff.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PlayCurrentRiff.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(77)))), ((int)(((byte)(90)))));
            this.PlayCurrentRiff.Location = new System.Drawing.Point(19, 110);
            this.PlayCurrentRiff.Name = "PlayCurrentRiff";
            this.PlayCurrentRiff.Size = new System.Drawing.Size(140, 50);
            this.PlayCurrentRiff.TabIndex = 1;
            this.PlayCurrentRiff.Text = "Play Current Riff";
            this.MyToolTip.SetToolTip(this.PlayCurrentRiff, "Same riff, but updated settings and chords");
            this.PlayCurrentRiff.UseVisualStyleBackColor = false;
            this.PlayCurrentRiff.Visible = false;
            this.PlayCurrentRiff.Click += new System.EventHandler(this.PlayCurrentRiff_Click);
            // 
            // PlayChord3
            // 
            this.PlayChord3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.PlayChord3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.PlayChord3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.PlayChord3.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PlayChord3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(77)))), ((int)(((byte)(90)))));
            this.PlayChord3.Location = new System.Drawing.Point(371, 171);
            this.PlayChord3.Name = "PlayChord3";
            this.PlayChord3.Size = new System.Drawing.Size(50, 30);
            this.PlayChord3.TabIndex = 65;
            this.PlayChord3.Text = "Play";
            this.MyToolTip.SetToolTip(this.PlayChord3, "Preview third chord only");
            this.PlayChord3.UseVisualStyleBackColor = false;
            this.PlayChord3.Click += new System.EventHandler(this.PlayChord3_Click);
            // 
            // StopPreview
            // 
            this.StopPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.StopPreview.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.StopPreview.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.StopPreview.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StopPreview.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(77)))), ((int)(((byte)(90)))));
            this.StopPreview.Location = new System.Drawing.Point(19, 166);
            this.StopPreview.Name = "StopPreview";
            this.StopPreview.Size = new System.Drawing.Size(140, 35);
            this.StopPreview.TabIndex = 2;
            this.StopPreview.Text = "Stop Preview";
            this.MyToolTip.SetToolTip(this.StopPreview, "Stop preview playback");
            this.StopPreview.UseVisualStyleBackColor = false;
            this.StopPreview.Click += new System.EventHandler(this.StopPreview_Click);
            // 
            // RandAllButton
            // 
            this.RandAllButton.AccessibleRole = System.Windows.Forms.AccessibleRole.Animation;
            this.RandAllButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.RandAllButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.RandAllButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.RandAllButton.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RandAllButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(77)))), ((int)(((byte)(90)))));
            this.RandAllButton.Location = new System.Drawing.Point(259, 54);
            this.RandAllButton.Name = "RandAllButton";
            this.RandAllButton.Size = new System.Drawing.Size(50, 30);
            this.RandAllButton.TabIndex = 14;
            this.RandAllButton.Text = "All";
            this.MyToolTip.SetToolTip(this.RandAllButton, "Randomise all chords");
            this.RandAllButton.UseVisualStyleBackColor = false;
            this.RandAllButton.Click += new System.EventHandler(this.RandAllButton_Click);
            // 
            // ChordsClearButton
            // 
            this.ChordsClearButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.ChordsClearButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ChordsClearButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ChordsClearButton.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChordsClearButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(77)))), ((int)(((byte)(90)))));
            this.ChordsClearButton.Location = new System.Drawing.Point(177, 171);
            this.ChordsClearButton.Name = "ChordsClearButton";
            this.ChordsClearButton.Size = new System.Drawing.Size(76, 30);
            this.ChordsClearButton.TabIndex = 54;
            this.ChordsClearButton.Text = "Clear";
            this.MyToolTip.SetToolTip(this.ChordsClearButton, "Clear all chords");
            this.ChordsClearButton.UseVisualStyleBackColor = false;
            this.ChordsClearButton.Click += new System.EventHandler(this.ChordsClearButton_Click);
            // 
            // Rand4Button
            // 
            this.Rand4Button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.Rand4Button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Rand4Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Rand4Button.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Rand4Button.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(77)))), ((int)(((byte)(90)))));
            this.Rand4Button.Location = new System.Drawing.Point(427, 54);
            this.Rand4Button.Name = "Rand4Button";
            this.Rand4Button.Size = new System.Drawing.Size(50, 30);
            this.Rand4Button.TabIndex = 17;
            this.Rand4Button.Text = "4";
            this.MyToolTip.SetToolTip(this.Rand4Button, "Randomise fourth chord only");
            this.Rand4Button.UseVisualStyleBackColor = false;
            this.Rand4Button.Click += new System.EventHandler(this.Rand4Button_Click);
            // 
            // PlayChord4
            // 
            this.PlayChord4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.PlayChord4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.PlayChord4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.PlayChord4.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PlayChord4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(77)))), ((int)(((byte)(90)))));
            this.PlayChord4.Location = new System.Drawing.Point(427, 171);
            this.PlayChord4.Name = "PlayChord4";
            this.PlayChord4.Size = new System.Drawing.Size(50, 30);
            this.PlayChord4.TabIndex = 66;
            this.PlayChord4.Text = "Play";
            this.MyToolTip.SetToolTip(this.PlayChord4, "Preview fourth chord only");
            this.PlayChord4.UseVisualStyleBackColor = false;
            this.PlayChord4.Click += new System.EventHandler(this.PlayChord4_Click);
            // 
            // PlayChord1
            // 
            this.PlayChord1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.PlayChord1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.PlayChord1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.PlayChord1.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PlayChord1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(77)))), ((int)(((byte)(90)))));
            this.PlayChord1.Location = new System.Drawing.Point(259, 171);
            this.PlayChord1.Name = "PlayChord1";
            this.PlayChord1.Size = new System.Drawing.Size(50, 30);
            this.PlayChord1.TabIndex = 63;
            this.PlayChord1.Text = "Play";
            this.MyToolTip.SetToolTip(this.PlayChord1, "Preview first chord only");
            this.PlayChord1.UseVisualStyleBackColor = false;
            this.PlayChord1.Click += new System.EventHandler(this.PlayChord1_Click);
            // 
            // Rand3Button
            // 
            this.Rand3Button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.Rand3Button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Rand3Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Rand3Button.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Rand3Button.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(77)))), ((int)(((byte)(90)))));
            this.Rand3Button.Location = new System.Drawing.Point(371, 54);
            this.Rand3Button.Name = "Rand3Button";
            this.Rand3Button.Size = new System.Drawing.Size(50, 30);
            this.Rand3Button.TabIndex = 16;
            this.Rand3Button.Text = "3";
            this.MyToolTip.SetToolTip(this.Rand3Button, "Randomise third chord only");
            this.Rand3Button.UseVisualStyleBackColor = false;
            this.Rand3Button.Click += new System.EventHandler(this.Rand3Button_Click);
            // 
            // PreviewChordsButton
            // 
            this.PreviewChordsButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.PreviewChordsButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.PreviewChordsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.PreviewChordsButton.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PreviewChordsButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(77)))), ((int)(((byte)(90)))));
            this.PreviewChordsButton.Location = new System.Drawing.Point(177, 54);
            this.PreviewChordsButton.Name = "PreviewChordsButton";
            this.PreviewChordsButton.Size = new System.Drawing.Size(76, 111);
            this.PreviewChordsButton.TabIndex = 3;
            this.PreviewChordsButton.Text = "Preview All Chords";
            this.MyToolTip.SetToolTip(this.PreviewChordsButton, "Preview all chords");
            this.PreviewChordsButton.UseVisualStyleBackColor = false;
            this.PreviewChordsButton.Click += new System.EventHandler(this.PreviewChordsButton_Click);
            // 
            // PlayChord2
            // 
            this.PlayChord2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.PlayChord2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.PlayChord2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.PlayChord2.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PlayChord2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(77)))), ((int)(((byte)(90)))));
            this.PlayChord2.Location = new System.Drawing.Point(315, 171);
            this.PlayChord2.Name = "PlayChord2";
            this.PlayChord2.Size = new System.Drawing.Size(50, 30);
            this.PlayChord2.TabIndex = 64;
            this.PlayChord2.Text = "Play";
            this.MyToolTip.SetToolTip(this.PlayChord2, "Preview second chord only");
            this.PlayChord2.UseVisualStyleBackColor = false;
            this.PlayChord2.Click += new System.EventHandler(this.PlayChord2_Click);
            // 
            // Rand2Button
            // 
            this.Rand2Button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.Rand2Button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Rand2Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Rand2Button.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Rand2Button.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(77)))), ((int)(((byte)(90)))));
            this.Rand2Button.Location = new System.Drawing.Point(315, 54);
            this.Rand2Button.Name = "Rand2Button";
            this.Rand2Button.Size = new System.Drawing.Size(50, 30);
            this.Rand2Button.TabIndex = 15;
            this.Rand2Button.Text = "2";
            this.MyToolTip.SetToolTip(this.Rand2Button, "Randomise second chord only");
            this.Rand2Button.UseVisualStyleBackColor = false;
            this.Rand2Button.Click += new System.EventHandler(this.Rand2Button_Click);
            // 
            // Picture
            // 
            this.Picture.BackColor = System.Drawing.Color.Transparent;
            this.Picture.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Picture.Location = new System.Drawing.Point(12, 304);
            this.Picture.Name = "Picture";
            this.Picture.Size = new System.Drawing.Size(851, 160);
            this.Picture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Picture.TabIndex = 79;
            this.Picture.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.BackgroundImage = global::WAVToArray.Properties.Resources.backgroundmaterial;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(876, 475);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.Picture);
            this.Controls.Add(this.GroupBoxCurrent);
            this.Controls.Add(this.GroupBoxSettings);
            this.Controls.Add(this.ProgressBar);
            this.Controls.Add(this.RiffLabel);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "House Generator";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.GroupBoxSettings.ResumeLayout(false);
            this.GroupBoxSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BpmBar)).EndInit();
            this.GroupBoxCurrent.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Picture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button ExitButton;
        private System.Windows.Forms.Button StopPreview;
        private System.Windows.Forms.Label bpmLabel;
        private System.Windows.Forms.CheckBox AutoPlay;
        private System.Windows.Forms.OpenFileDialog openFile;
        private System.Windows.Forms.Label KeyLabel;
        private System.Windows.Forms.CheckBox ProgRhythmCheck;
        private System.Windows.Forms.ProgressBar ProgressBar;
        private System.Windows.Forms.ComboBox ChordPitch1;
        private System.Windows.Forms.ComboBox ChordPitch2;
        private System.Windows.Forms.ComboBox ChordPitch3;
        private System.Windows.Forms.ComboBox ChordPitch4;
        private System.Windows.Forms.Button RandAllButton;
        private System.Windows.Forms.ComboBox ChordThird1;
        private System.Windows.Forms.ComboBox ChordThird2;
        private System.Windows.Forms.ComboBox ChordThird3;
        private System.Windows.Forms.ComboBox ChordThird4;
        private System.Windows.Forms.Button PreviewChordsButton;
        private System.Windows.Forms.Button PlayChord1;
        private System.Windows.Forms.Button PlayChord2;
        private System.Windows.Forms.Button PlayChord3;
        private System.Windows.Forms.Button PlayChord4;
        private System.Windows.Forms.Button ExportMIDI;
        private System.Windows.Forms.Button PlayNewRiff;
        private System.Windows.Forms.GroupBox GroupBoxSettings;
        private System.Windows.Forms.Button PlayCurrentRiff;
        private System.Windows.Forms.GroupBox GroupBoxCurrent;
        private System.Windows.Forms.Button BPMResetButton;
        private System.Windows.Forms.Label RandomiseLabel;
        private System.Windows.Forms.Button Rand4Button;
        private System.Windows.Forms.Button Rand3Button;
        private System.Windows.Forms.Button Rand2Button;
        private System.Windows.Forms.PictureBox Picture;
        private System.Windows.Forms.Label RiffLabel;
        private System.Windows.Forms.Button ChordsClearButton;
        private System.Windows.Forms.TrackBar BpmBar;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ToolTip MyToolTip;
        private System.Windows.Forms.Label WrongChordWarning;
        private System.Windows.Forms.Button ExportWav;
    }
}

