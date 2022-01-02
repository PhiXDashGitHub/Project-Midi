public class MidiFile
{
    
}

/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public struct MidiEvent
{
    public enum Type
    {
        NoteOff,
        NoteOn,
        Other
    };

    public Type tEvent;
    public byte nKey;
    public byte nVelocity;
    public uint nWallTick;
    public uint nDeltaTick;
};

public struct MidiNote
{
    public byte nKey;
    public byte nVelocity;
    public uint nStartTime;
    public uint nDuration;
};

public struct MidiTrack
{
    #pragma warning disable IDE0060 // Nicht verwendete Parameter entfernen
    public MidiTrack(int i)
    {
        sName = "";
        sInstrument = "";
        midiEvents = new List<MidiEvent>();
        midiNotes = new List<MidiNote>();
        nMaxNote = 64;
        nMinNote = 64;
    }
    #pragma warning restore IDE0060 // Nicht verwendete Parameter entfernen

    public string sName;
    public string sInstrument;
    public List<MidiEvent> midiEvents;
    public List<MidiNote> midiNotes;
    public byte nMaxNote;
    public byte nMinNote;
};

public class MidiFile
{
    static FileStream fs;
    public List<MidiTrack> midiTracks;
    public int nTempo;
    public int nBPM;

    public MidiFile(string fileName = "")
    {
        midiTracks = new List<MidiTrack>();
        nTempo = 0;
        nBPM = 0;

        if (fileName != "" && fileName.EndsWith(".mid"))
        {
            ParseFile(fileName);
        }
    }

    [Flags]
    enum EventName : byte
    {
        VoiceNoteOff = 0x80,
        VoiceNoteOn = 0x90,
        VoiceAftertouch = 0xA0,
        VoiceControlChange = 0xB0,
        VoiceProgramChange = 0xC0,
        VoiceChannelPressure = 0xD0,
        VoicePitchBend = 0xE0,
        SystemExclusive = 0xF0
    };

    [Flags]
    enum MetaEventName : byte
    {
        MetaSequence = 0x00,
        MetaText = 0x01,
        MetaCopyright = 0x02,
        MetaTrackName = 0x03,
        MetaInstrumentName = 0x04,
        MetaLyrics = 0x05,
        MetaMarker = 0x06,
        MetaCuePoint = 0x07,
        MetaChannelPrefix = 0x20,
        MetaEndOfTrack = 0x2F,
        MetaSetTempo = 0x51,
        MetaSMPTEOffset = 0x54,
        MetaTimeSignature = 0x58,
        MetaKeySignature = 0x59,
        MetaSequencerSpecific = 0x7F
    }

    #pragma warning disable IDE0059 // Unnötige Zuweisung eines Werts.
    public bool ParseFile(string fileName)
    {
        fs = File.Open("Assets/" + fileName, FileMode.Open);

        if (!fs.CanRead || !fileName.EndsWith(".mid"))
        {
            Debug.LogError("ERROR - File is not in Midi-Format.");
            return false;
        }

        static dynamic Swap32(uint n)
        {
            return (((n >> 24) & 0xff) | ((n << 8) & 0xff0000) | ((n >> 8) & 0xff00) | ((n << 24) & 0xff000000));
        }

        static dynamic Swap16(ushort n)
        {
            return (ushort)((n >> 8) | (n << 8));
        }

        static dynamic ReadString(uint length)
        {
            byte[] s = new byte[length];
            for (uint i = 0; i < length; i++) s[i] = fs.Get();
            return System.Text.Encoding.Default.GetString(s);
        }

        static dynamic ReadValue(bool isByte = false)
        {
            uint nValue = 0;
            byte nByte = 0;

            //Read Byte
            nValue = fs.Get();

            //Check MSB, if set, more bytes need reading
            if ((nValue & 0x80) == 0x80)
            {
                //Extract bottom 7 bits of read byte
                nValue &= 0x7F;

                do
                {
                    //Read next byte
                    nByte = fs.Get();

                    //Construct value by setting bottom 7 bits, then shifting 7 bits
                    nValue = (nValue << 7) | (uint)(nByte & 0x7F);

                } while ((nByte & 0x80) == 0x80); //Loop whilst read byte MSB is 1
            }

            //Return final construction (always 32-bit unsigned integer internally)
            if (isByte)
            {
                return (byte)nValue;
            }
            else
            {
                return nValue;
            }
        }

        //PARSE MIDI FILE

        uint n32 = 0;
        ushort n16 = 0;

        //Read MIDI Header (Fixed Size)
        byte[] h = new byte[32];
        fs.Read(h, 0, sizeof(uint));
        n32 = BitConverter.ToUInt32(h, 0);
        uint nFileID = Swap32(n32);

        fs.Read(h, 0, sizeof(uint));
        n32 = BitConverter.ToUInt32(h, 0);
        uint nHeaderLength = Swap32(n32);

        fs.Read(h, 0, sizeof(ushort));
        n16 = BitConverter.ToUInt16(h, 0);
        ushort nFormat = Swap16(n16);

        fs.Read(h, 0, sizeof(ushort));
        n16 = BitConverter.ToUInt16(h, 0);
        ushort nTrackChunks = Swap16(n16);

        fs.Read(h, 0, sizeof(ushort));
        n16 = BitConverter.ToUInt16(h, 0);
        ushort nDivision = Swap16(n16);

        for (ushort nChunk = 0; nChunk < nTrackChunks; nChunk++)
        {
            Debug.Log("===== NEW TRACK");

            //Read Track Header
            fs.Read(h, 0, sizeof(uint));
            n32 = BitConverter.ToUInt32(h, 0);
            uint nTrackID = Swap32(n32);

            fs.Read(h, 0, sizeof(uint));
            n32 = BitConverter.ToUInt32(h, 0);
            uint nTrackLength = Swap32(n32);

            bool bEndOfTrack = false;
            byte nPreviousStatus = 0;

            MidiTrack mdTrack = new MidiTrack
            {
                midiEvents = new List<MidiEvent>(),
                midiNotes = new List<MidiNote>()
            };

            while (!fs.EndOfFile() && !bEndOfTrack)
            {
                //Fundamentally all MIDI Events contain a timecode, and a status byte*
                uint nStatusTimeDelta = 0;
                byte nStatus = 0;

                //Read Timecode
                nStatusTimeDelta = ReadValue();
                nStatus = fs.Get();

                if (nStatus < 0x80)
                {
                    nStatus = nPreviousStatus;
                    fs.Seek(-1, SeekOrigin.Current);
                }

                if ((EventName)(nStatus & 0xF0) == EventName.VoiceNoteOff)
                {
                    nPreviousStatus = nStatus;
                    byte nChannel = (byte)(nStatus & 0x0F);
                    byte nNoteID = fs.Get();
                    byte nNoteVelocity = fs.Get();

                    mdTrack.midiEvents.Add(new MidiEvent() { tEvent = MidiEvent.Type.NoteOff, nKey = nNoteID, nVelocity = nNoteVelocity, nDeltaTick = nStatusTimeDelta });
                }
                else if ((EventName)(nStatus & 0xF0) == EventName.VoiceNoteOn)
                {
                    nPreviousStatus = nStatus;
                    byte nChannel = (byte)(nStatus & 0x0F);
                    byte nNoteID = fs.Get();
                    byte nNoteVelocity = fs.Get();

                    if (nNoteVelocity == 0)
                    {
                        mdTrack.midiEvents.Add(new MidiEvent() { tEvent = MidiEvent.Type.NoteOff, nKey = nNoteID, nVelocity = nNoteVelocity, nDeltaTick = nStatusTimeDelta });
                    }
                    else
                    {
                        mdTrack.midiEvents.Add(new MidiEvent() { tEvent = MidiEvent.Type.NoteOn, nKey = nNoteID, nVelocity = nNoteVelocity, nDeltaTick = nStatusTimeDelta });
                    }
                }
                else if ((EventName)(nStatus & 0xF0) == EventName.VoiceAftertouch)
                {
                    nPreviousStatus = nStatus;
                    byte nChannel = (byte)(nStatus & 0x0F);
                    byte nNoteID = fs.Get();
                    byte nNoteVelocity = fs.Get();

                    mdTrack.midiEvents.Add(new MidiEvent() { tEvent = MidiEvent.Type.Other });
                }
                else if ((EventName)(nStatus & 0xF0) == EventName.VoiceControlChange)
                {
                    nPreviousStatus = nStatus;
                    byte nChannel = (byte)(nStatus & 0x0F);
                    byte nNoteID = fs.Get();
                    byte nNoteVelocity = fs.Get();

                    mdTrack.midiEvents.Add(new MidiEvent() { tEvent = MidiEvent.Type.Other });
                }
                else if ((EventName)(nStatus & 0xF0) == EventName.VoiceProgramChange)
                {
                    nPreviousStatus = nStatus;
                    byte nChannel = (byte)(nStatus & 0x0F);
                    byte nProgramID = fs.Get();

                    mdTrack.midiEvents.Add(new MidiEvent() { tEvent = MidiEvent.Type.Other });
                }
                else if ((EventName)(nStatus & 0xF0) == EventName.VoiceChannelPressure)
                {
                    nPreviousStatus = nStatus;
                    byte nChannel = (byte)(nStatus & 0x0F);
                    byte nChannelPressure = fs.Get();

                    mdTrack.midiEvents.Add(new MidiEvent() { tEvent = MidiEvent.Type.Other });
                }
                else if ((EventName)(nStatus & 0xF0) == EventName.VoicePitchBend)
                {
                    nPreviousStatus = nStatus;
                    byte nChannel = (byte)(nStatus & 0x0F);
                    byte nLS7B = fs.Get();
                    byte nMS7B = fs.Get();

                    mdTrack.midiEvents.Add(new MidiEvent() { tEvent = MidiEvent.Type.Other });
                }
                else if ((EventName)(nStatus & 0xF0) == EventName.SystemExclusive)
                {
                    nPreviousStatus = 0;

                    if (nStatus == 0xFF)
                    {
                        //Meta Message
                        byte nType = fs.Get();
                        byte nLength = ReadValue(true);

                        switch ((MetaEventName)nType)
                        {
                            case MetaEventName.MetaSequence:
                                Debug.Log("Sequence Number: " + fs.Get() + fs.Get());
                                break;
                            case MetaEventName.MetaText:
                                Debug.Log("Text: " + ReadString(nLength));
                                break;
                            case MetaEventName.MetaCopyright:
                                Debug.Log("Copyright: " + ReadString(nLength));
                                break;
                            case MetaEventName.MetaTrackName:
                                mdTrack.sName = ReadString(nLength);
                                Debug.Log("Track Name: " + mdTrack.sName);
                                break;
                            case MetaEventName.MetaInstrumentName:
                                mdTrack.sInstrument = ReadString(nLength);
                                Debug.Log("Instrument Name: " + mdTrack.sInstrument);
                                break;
                            case MetaEventName.MetaLyrics:
                                Debug.Log("Lyrics: " + ReadString(nLength));
                                break;
                            case MetaEventName.MetaMarker:
                                Debug.Log("Marker: " + ReadString(nLength));
                                break;
                            case MetaEventName.MetaCuePoint:
                                Debug.Log("Cue: " + ReadString(nLength));
                                break;
                            case MetaEventName.MetaChannelPrefix:
                                Debug.Log("Prefix: " + fs.Get());
                                break;
                            case MetaEventName.MetaEndOfTrack:
                                bEndOfTrack = true;
                                break;
                            case MetaEventName.MetaSetTempo:
                                //Tempo is in microseconds per quarter note
                                if (nTempo == 0)
                                {
                                    nTempo |= (fs.Get() << 16);
                                    nTempo |= (fs.Get() << 8);
                                    nTempo |= (fs.Get() << 0);
                                    nBPM = (60000000 / nTempo);
                                    Debug.Log("Tempo: " + nTempo + " (" + nBPM + " BPM)");
                                }
                                break;
                            case MetaEventName.MetaSMPTEOffset:
                                Debug.Log("SMPTE: H:" + fs.Get() + " M:" + fs.Get() + " S:" + fs.Get() + " FR:" + fs.Get() + " FF:" + fs.Get());
                                break;
                            case MetaEventName.MetaTimeSignature:
                                Debug.Log("Time Signature: " + fs.Get() + "/" + (2 + fs.Get()));
                                Debug.Log("ClocksPerTick: " + fs.Get());

                                //A MIDI "Beat" is 24 ticks, so specify how many 32nd notes constitude a neat
                                Debug.Log("32per24Clocks: " + fs.Get());
                                break;
                            case MetaEventName.MetaKeySignature:
                                Debug.Log("Key Signature: " + fs.Get());
                                Debug.Log("Minor Key: " + fs.Get());
                                break;
                            case MetaEventName.MetaSequencerSpecific:
                                Debug.Log("Sequencer Specific: " + ReadString(nLength));
                                break;
                            default:
                                Debug.LogError("ERROR - Unrecognised MetaEvent: " + nType);
                                break;
                        }
                    }

                    if (nStatus == 0xF0)
                    {
                        //System Exclusive Message Begin
                        Debug.Log("System Exclusive Begin: " + ReadString(ReadValue()));
                    }

                    if (nStatus == 0xF7)
                    {
                        //System Exclusive Message End
                        Debug.Log("System Exclusive End: " + ReadString(ReadValue()));
                    }
                }
                else
                {
                    Debug.LogError("ERROR - Unrecognised Status Byte: " + nStatus);
                }
            }

            midiTracks.Add(mdTrack);
        }

        //Convert Time Events to Notes
        for (int t = 0; t < midiTracks.Count; t++)
        {
            uint nWallTime = 0;

            MidiTrack track = midiTracks[t];
            List<MidiNote> notesBeingProcessed = new List<MidiNote>();

            foreach (MidiEvent midiEvent in track.midiEvents)
            {
                nWallTime += midiEvent.nDeltaTick;

                if (midiEvent.tEvent == MidiEvent.Type.NoteOn)
                {
                    //New Note
                    notesBeingProcessed.Add(new MidiNote() { nKey = midiEvent.nKey, nVelocity = midiEvent.nVelocity, nStartTime = nWallTime, nDuration = 0 });
                }
                
                if (midiEvent.tEvent == MidiEvent.Type.NoteOff)
                {
                    for (int n = 0; n < notesBeingProcessed.Count; n++)
                    {
                        MidiNote note = notesBeingProcessed[n];

                        if (notesBeingProcessed[n].nKey == midiEvent.nKey)
                        {
                            note.nDuration = nWallTime - note.nStartTime;
                            track.midiNotes.Add(note);
                            track.nMinNote = (byte)notesBeingProcessed.Min();
                            track.nMaxNote = (byte)notesBeingProcessed.Max();
                            notesBeingProcessed.Remove(note);
                            break;
                        }
                    }
                }
            }
        }

        //Delete empty Tracks
        for (int i = 0; i < midiTracks.Count; i++)
        {
            if (midiTracks[i].midiNotes.Count < 1)
            {
                midiTracks.RemoveAt(i);
            }
        }

        Debug.Log("MIDI File Loaded.");

        fs.Close();
        return true;
    }

    public List<string> ConvertToNoteData()
    {
        List<string> data = new List<string>();

        for (byte t = 0; t < midiTracks.Count; t++)
        {
            if (midiTracks[t].midiNotes.Count < 1)
            {
                continue;
            }

            for (int n = 0; n < midiTracks[t].midiNotes.Count; n++)
            {
                byte instrument = t;
                byte key = midiTracks[t].midiNotes[n].nKey;
                float startTime = midiTracks[t].midiNotes[n].nStartTime;
                float duration = midiTracks[t].midiNotes[n].nDuration;

                startTime += duration - 48;

                if (t == 10)
                {
                    data.Add($"[{0},{key - 36},{startTime / 24},{1}]");
                }
            }
        }

        return data;
    }

    #pragma warning restore IDE0059 // Unnötige Zuweisung eines Werts.
}

static class FSExtension
{
    public static bool EndOfFile(this FileStream fileStream)
    {
        return fileStream.Position >= fileStream.Length;
    }

    public static byte Get(this FileStream fileStream)
    {
        return (byte)fileStream.ReadByte();
    }

    public static int Min(this List<MidiNote> list)
    {
        int min = 9999;

        foreach (MidiNote note in list)
        {
            if (note.nKey < min)
            {
                min = note.nKey;
            }
        }

        return min;
    }

    public static int Max(this List<MidiNote> list)
    {
        int max = 0;

        foreach (MidiNote note in list)
        {
            if (note.nKey > max)
            {
                max = note.nKey;
            }
        }

        return max;
    }
}
*/