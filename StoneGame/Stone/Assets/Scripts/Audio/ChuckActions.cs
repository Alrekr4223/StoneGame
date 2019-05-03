using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChuckActions : MonoBehaviour {
    

    public void ChunityStrum(float note, float length) //TODO, send in color value to SelectionFX().
    {


        //Debug.Log("Note: " + note + ", length: " + length);
        //Chuck + Unity -> Chunity, Interactive Audio
        this.GetComponent<ChuckSubInstance>().RunCode(string.Format(@"

            Mandolin m => dac;
            
            //Dorian Scale
            [0,0,2,3,3,5,5,7,7,9,9,10] @=> int Dorian[];

            //Lydian
            [0,2,4,6,7,9,11,12,14,16,18,19]  @=> int Lydian[];
            
            {0} $ int => int MidiCasted;
            MidiCasted % 12 => int MidiModulus;
            MidiCasted / 12 => int octave;
            
            Lydian[MidiModulus] => int ScaleNote;
            ScaleNote + (12*octave) => int OctiveNote;

            Std.mtof(OctiveNote) => float Freq;
            Freq $ int => int NoteToPlay;

            function void strumMandolin ( float freq, float detune, float body, float pluckpos, float length )
            {{
                freq => m.freq;
                detune => m.stringDetune;
                body => m.bodySize;
                pluckpos => m.pluckPos;  //good to change

                0.4 => m.noteOn;

                length::second => now;
            }}

            function void SelectionFX(float masterNote, float length){{

                strumMandolin (masterNote, 0, 0.25, 0.5, length);
            }}


            SelectionFX(NoteToPlay, {1});

        ", note, length));
    }



    public void ChunityFAIL() //TODO, send in color value to SelectionFX().
    {
        //Chuck + Unity -> Chunity, Interactive Audio
        this.GetComponent<ChuckSubInstance>().RunCode(@"

            Mandolin m => dac;
            

            function void strumMandolin ( float freq, float detune, float body, float pluckpos, float length )
            {{
                freq => m.freq;
                detune => m.stringDetune;
                body => m.bodySize;
                pluckpos => m.pluckPos;  //good to change

                1.0 => m.noteOn;

                length::second => now;
            }}

            function void SelectionFX(float masterNote, float length){{

                strumMandolin (masterNote, 1, 0.25, 0.5, length);
                strumMandolin (masterNote - 19, 1, 0.25, 0.5, length);
                strumMandolin (masterNote - 61, 1, 0.25, 0.5, length + 0.8);
            }}


            SelectionFX(220, 0.3);

        ");
    }

    public void ChunityWIN()
    {
        this.GetComponent<ChuckSubInstance>().RunCode(@"

            Mandolin m => dac;
            

            function void strumMandolin ( float freq, float detune, float body, float pluckpos, float length )
            {{
                freq => m.freq;
                detune => m.stringDetune;
                body => m.bodySize;
                pluckpos => m.pluckPos;  //good to change

                1.0 => m.noteOn;

                length::second => now;
            }}

            function void SelectionFX(float masterNote, float length){{

                strumMandolin (masterNote, 1, 0.25, 0.5, length);
                strumMandolin (masterNote + 20, 1, 0.25, 0.5, length);
                strumMandolin (masterNote + 60, 1, 0.25, 0.5, length + 0.8);
            }}


            SelectionFX(440, 0.3);

        ");
    }
}
