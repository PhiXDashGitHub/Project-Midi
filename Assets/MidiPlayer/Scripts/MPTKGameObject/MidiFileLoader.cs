
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System;
using UnityEngine.Events;
using MEC;

namespace MidiPlayerTK
{
    /// <summary>
    /// Script for the prefab MidiFilePlayer. 
    /// Play a selected midi file. 
    /// List of Midi file must be defined with Midi Player Setup (see Unity menu MPTK).
    /// </summary>
    public class MidiFileLoader : MonoBehaviour
    {
        /// <summary>
        /// Midi name to play. Use the exact name defined in Unity resources folder MidiDB without any path or extension.
        /// Tips: Add Midi files to your project with the Unity menu MPTK or add it directly in the ressource folder and open Midi File Setup to automatically integrate Midi in MPTK.
        //! @code
        //! midiFilePlayer.MPTK_MidiName = "Albinoni - Adagio";
        //! @endcode
        /// </summary>
        public virtual string MPTK_MidiName
        {
            get
            {
                //Debug.Log("MPTK_MidiName get " + midiNameToPlay);
                return midiNameToPlay;
            }
            set
            {
                //Debug.Log("MPTK_MidiName set " + value);
                midiIndexToPlay = MidiPlayerGlobal.MPTK_FindMidi(value);
                //Debug.Log("MPTK_MidiName set index= " + midiIndexToPlay);
                midiNameToPlay = value;
            }
        }
        [SerializeField]
        [HideInInspector]
        private string midiNameToPlay;

        /// <summary>
        /// Index Midi. Find the Index of Midi file from the popup in MidiFilePlayer inspector.
        /// Tips: Add Midi files to your project with the Unity menu MPTK or add it directly in the ressource folder and open Midi File Setup to automatically integrate Midi in MPTK.
        /// return -1 if not found
        /// </summary>
        /// <param name="index"></param>
        public virtual int MPTK_MidiIndex
        {
            get
            {
                try
                {
                    //int index = MidiPlayerGlobal.MPTK_FindMidi(MPTK_MidiName);
                    //Debug.Log("MPTK_MidiIndex get " + midiIndexToPlay);
                    return midiIndexToPlay;
                }
                catch (System.Exception ex)
                {
                    MidiPlayerGlobal.ErrorDetail(ex);
                }
                return -1;
            }
            set
            {
                //! @code
                //! midiFilePlayer.MPTK_MidiIndex = 1;
                //! @endcode
                try
                {
                    //Debug.Log("MPTK_MidiIndex set " + value);
                    if (value >= 0 && value < MidiPlayerGlobal.CurrentMidiSet.MidiFiles.Count)
                    {
                        MPTK_MidiName = MidiPlayerGlobal.CurrentMidiSet.MidiFiles[value];
                        // useless, set when set midi name : 
                        midiIndexToPlay = value;
                    }
                    else
                        Debug.LogWarning("MidiFilePlayer - Set MidiIndex value not valid : " + value);
                }
                catch (System.Exception ex)
                {
                    MidiPlayerGlobal.ErrorDetail(ex);
                }
            }
        }

        [SerializeField]
        [HideInInspector]
        private int midiIndexToPlay;

        /// <summary>
        /// Get duration of current Midi with current tempo
        /// </summary>
        public virtual TimeSpan MPTK_Duration { get { try { if (midiLoaded != null) return midiLoaded.MPTK_Duration; } catch (System.Exception ex) { MidiPlayerGlobal.ErrorDetail(ex); } return TimeSpan.Zero; } }

        /// <summary>
        /// Last tick position in Midi: Value of the tick for the last midi event in sequence expressed in number of "ticks". MPTK_TickLast / MPTK_DeltaTicksPerQuarterNote equal the duration time of a quarter-note regardless the defined tempo.
        /// </summary>
        public virtual long MPTK_TickLast { get { return midiLoaded != null ? midiLoaded.MPTK_TickLast : 0; } }

        /// <summary>
        /// Lenght in millisecond of a quarter
        /// </summary>
        public virtual double MPTK_PulseLenght { get { try { if (midiLoaded != null) return midiLoaded.TickLengthMs; } catch (System.Exception ex) { MidiPlayerGlobal.ErrorDetail(ex); } return 0d; } }

        /// <summary>
        /// Updated only when playing in Unity (for inspector refresh)
        /// </summary>
        //! @cond NODOC
        public string playTimeEditorModeOnly;
        //! @endcond

        /// <summary>
        /// Log midi events
        /// </summary>
        public virtual bool MPTK_LogEvents
        {
            get { return logEvents; }
            set { logEvents = value; }
        }

        /// <summary>
        /// Should keep note off event Events ? 
        /// </summary>
        public virtual bool MPTK_KeepNoteOff
        {
            get { return keepNoteOff; }
            set { keepNoteOff = value; }
        }

        /// <summary>
        /// Level of quantization : 
        ///! @li @c     0 = None 
        ///! @li @c     1 = Quarter Note
        ///! @li @c     2 = Eighth Note
        ///! @li @c     3 = 16th Note
        ///! @li @c     4 = 32th Note
        ///! @li @c     5 = 64th Note
        /// </summary>
        public virtual int MPTK_Quantization
        {
            get { return quantization; }
            set
            {
                try
                {
                    if (value >= 0 && value <= 5)
                    {
                        quantization = value;
                        if (midiLoaded == null)
                        {
                            NoMidiLoaded("MPTK_Quantization");
                        }
                        midiLoaded.ChangeQuantization(quantization);
                    }
                    else
                        Debug.LogWarning("MidiFilePlayer - Set Quantization value not valid : " + value);
                }
                catch (System.Exception ex)
                {
                    MidiPlayerGlobal.ErrorDetail(ex);
                }
            }
        }
        [SerializeField]
        [HideInInspector]
        private int quantization = 0;


        [SerializeField]
        [HideInInspector]
        private bool logEvents = false, keepNoteOff = false;

        private MidiLoad midiLoaded;

        /// <summary>
        /// Get all the raw midi events available in the midi file
        /// [DEPRECATED] use rather MPTK_Load then midiloaded.MPTK_ReadMidiEvents();
        /// </summary>
        public virtual List<TrackMidiEvent> MPTK_MidiEvents
        {
            get
            {
                List<TrackMidiEvent> tme = null;
                try
                {
                    if (midiLoaded == null)
                    {
                        NoMidiLoaded("MPTK_MidiEvents");
                    }
                    else
                        tme = midiLoaded.MidiSorted;
                }
                catch (System.Exception ex)
                {
                    MidiPlayerGlobal.ErrorDetail(ex);
                }
                return tme;
            }
        }

        /// <summary>
        /// Delta Ticks Per Quarter Note. Indicate the duration time in "ticks" which make up a quarter-note. For instance, if 96, then a duration of an eighth-note in the file would be 48.
        /// </summary>
        public virtual int MPTK_DeltaTicksPerQuarterNote
        {
            get
            {
                int DeltaTicksPerQuarterNote = 0;
                try
                {
                    if (midiLoaded == null)
                    {
                        NoMidiLoaded("MPTK_DeltaTicksPerQuarterNote");
                    }
                    else
                        DeltaTicksPerQuarterNote = midiLoaded.MPTK_DeltaTicksPerQuarterNote;
                }
                catch (System.Exception ex)
                {
                    MidiPlayerGlobal.ErrorDetail(ex);
                }
                return DeltaTicksPerQuarterNote;
            }
        }

        void Awake()
        {
            //Debug.Log("Awake MidiFilePlayer midiIsPlaying:" + midiIsPlaying);
        }

        void Start()
        {
            //Debug.Log("Start MidiFilePlayer midiIsPlaying:" + midiIsPlaying + " MPTK_PlayOnStart:" + MPTK_PlayOnStart);
        }

        /// <summary>
        /// Load the midi file defined with MPTK_MidiName or MPTK_MidiIndex or from a array of bytes
        /// </summary>
        /// <param name="midiBytesToLoad"></param>
        public virtual void MPTK_Load(byte[] midiBytesToLoad = null)
        {
            try
            {
                // Load description of available soundfont
                //if (MidiPlayerGlobal.ImSFCurrent != null && MidiPlayerGlobal.CurrentMidiSet != null && MidiPlayerGlobal.CurrentMidiSet.MidiFiles != null && MidiPlayerGlobal.CurrentMidiSet.MidiFiles.Count > 0)
                {
                    if (string.IsNullOrEmpty(MPTK_MidiName))
                        MPTK_MidiName = MidiPlayerGlobal.CurrentMidiSet.MidiFiles[0];
                    int selectedMidi = MidiPlayerGlobal.CurrentMidiSet.MidiFiles.FindIndex(s => s == MPTK_MidiName);
                    if (selectedMidi < 0)
                    {
                        Debug.LogWarning("MidiFilePlayer - MidiFile " + MPTK_MidiName + " not found. Try with the first in list.");
                        selectedMidi = 0;
                        MPTK_MidiName = MidiPlayerGlobal.CurrentMidiSet.MidiFiles[0];
                    }

                    try
                    {
                        midiLoaded = new MidiLoad();

                        // No midi byte array, try to load from MidiFile from resource
                        if (midiBytesToLoad == null || midiBytesToLoad.Length == 0)
                        {
                            TextAsset mididata = Resources.Load<TextAsset>(Path.Combine(MidiPlayerGlobal.MidiFilesDB, MPTK_MidiName));
                            midiBytesToLoad = mididata.bytes;
                        }

                        midiLoaded.KeepNoteOff = MPTK_KeepNoteOff;
                        midiLoaded.MPTK_Load(midiBytesToLoad);
                    }
                    catch (System.Exception ex)
                    {
                        MidiPlayerGlobal.ErrorDetail(ex);
                    }
                }
                //else
                //    Debug.LogWarning(MidiPlayerGlobal.ErrorNoMidiFile);
            }
            catch (System.Exception ex)
            {
                MidiPlayerGlobal.ErrorDetail(ex);
            }
        }

        /// <summary>
        /// Read the list of midi events available in the Midi from a ticks position to an end position.
        /// </summary>
        /// <param name="fromTicks">ticks start</param>
        /// <param name="toTicks">ticks end</param>
        /// <returns></returns>
        public List<MPTKEvent> MPTK_ReadMidiEvents(long fromTicks = 0, long toTicks = long.MaxValue)
        {
            if (midiLoaded == null)
            {
                NoMidiLoaded("MPTK_ReadMidiEvents");
                return null;
            }
            midiLoaded.LogEvents = logEvents;
            midiLoaded.KeepNoteOff = keepNoteOff;
            return midiLoaded.MPTK_ReadMidiEvents(fromTicks, toTicks);
        }

        private void NoMidiLoaded(string action)
        {
            Debug.LogWarning(string.Format("No Midi loaded, {0} canceled", action));
        }
        /// <summary>
        /// Play next Midi from the list of midi defined in MPTK (see Unity menu Midi)
        /// </summary>
        public virtual void MPTK_Next()
        {
            try
            {
                if (MidiPlayerGlobal.CurrentMidiSet.MidiFiles != null && MidiPlayerGlobal.CurrentMidiSet.MidiFiles.Count > 0)
                {
                    int selectedMidi = 0;
                    //Debug.Log("Next search " + MPTK_MidiName);
                    if (!string.IsNullOrEmpty(MPTK_MidiName))
                        selectedMidi = MidiPlayerGlobal.CurrentMidiSet.MidiFiles.FindIndex(s => s == MPTK_MidiName);
                    if (selectedMidi >= 0)
                    {
                        selectedMidi++;
                        if (selectedMidi >= MidiPlayerGlobal.CurrentMidiSet.MidiFiles.Count)
                            selectedMidi = 0;
                        MPTK_MidiName = MidiPlayerGlobal.CurrentMidiSet.MidiFiles[selectedMidi];
                        //Debug.Log("Next found " + MPTK_MidiName);
                    }
                }
                else
                    Debug.LogWarning(MidiPlayerGlobal.ErrorNoMidiFile);
            }
            catch (System.Exception ex)
            {
                MidiPlayerGlobal.ErrorDetail(ex);
            }
        }

        /// <summary>
        /// Play previous Midi from the list of midi defined in MPTK (see Unity menu Midi)
        /// </summary>
        public virtual void MPTK_Previous()
        {
            try
            {
                if (MidiPlayerGlobal.CurrentMidiSet.MidiFiles != null && MidiPlayerGlobal.CurrentMidiSet.MidiFiles.Count > 0)
                {
                    int selectedMidi = 0;
                    if (!string.IsNullOrEmpty(MPTK_MidiName))
                        selectedMidi = MidiPlayerGlobal.CurrentMidiSet.MidiFiles.FindIndex(s => s == MPTK_MidiName);
                    if (selectedMidi >= 0)
                    {
                        selectedMidi--;
                        if (selectedMidi < 0)
                            selectedMidi = MidiPlayerGlobal.CurrentMidiSet.MidiFiles.Count - 1;
                        MPTK_MidiName = MidiPlayerGlobal.CurrentMidiSet.MidiFiles[selectedMidi];
                    }
                }
                else
                    Debug.LogWarning(MidiPlayerGlobal.ErrorNoMidiFile);
            }
            catch (System.Exception ex)
            {
                MidiPlayerGlobal.ErrorDetail(ex);
            }
        }

        /// <summary>
        /// Return note length as https://en.wikipedia.org/wiki/Note_value 
        /// </summary>
        /// <param name="note"></param>
        /// <returns>MPTKEvent.EnumLength</returns>
        public MPTKEvent.EnumLength MPTK_NoteLength(MPTKEvent note)
        {
            if (midiLoaded != null)
                return midiLoaded.NoteLength(note);
            else
                NoMidiLoaded("MPTK_NoteLength");
            return MPTKEvent.EnumLength.Sixteenth;
        }
    }
}

