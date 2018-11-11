# Music-Generator
A C# Windows app that can randomly generate 16 bars of "House" music, complete with drums, main melody and a chord sequence. The app works by reading from a library of wav files (samples of drums or instrument notes), and combining them into a new wav file.
Features:
* Chords are generated using Scale Degrees, and so are guaranteed to sound musical to some degree.
* If the user does not like a chord, they can re-generate a single chord leaving the rest untouched, or they can specify the chord entirely manually.
* Melody is generated based on the chord sequence to ensure that there are no musical clashes.
* Export to .wav or .midi files for use in other programs.
* Graphical display to show the generated melody in a piano roll style.
* Drums are set to have a slight swing, giving the generated clip a classic "House" feeling.
* With the drums overlayed, the track will have "ducking" applied to it - a process in which the instruments' volume is quickly lowered and brought back up to make the kick drum stand out more.
