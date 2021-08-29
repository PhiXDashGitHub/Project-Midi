using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class NoteEditor : MonoBehaviour
{
    static NoteEditor noteEditor;

    public enum EditMode { None, Place, Erase };
    public static EditMode editMode;

    [Header("Cursor")]
    public Texture2D cursorTexPlace, cursorTexErase;
    public Vector2 cursorOffsetPlace, cursorOffsetErase;

    [Header("UI")]
    public GameObject notePrefab;
    public Transform noteParent;
    public RectTransform scrollView;
    public Image playButton;
    public Image recordButton;
    [Tooltip("0 = Play, 1 = Pause")] public Sprite[] playButtonSprites;
    public TextMeshProUGUI bpmText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI countdownText;
    public ControlKnob volumeKnob;
    public ControlKnob reverbKnob;

    [Header("Settings")]
    public int keyRange = 72;
    public static int s_keyRange;

    public float bpm = 140f;
    public static float s_bpm;

    public float gridSize = 1f;
    public static float s_gridSize;

    public GameObject audioSourcePrefab;

    //public string instrumentData;
    public List<string> noteData;

    public Instrument[] instruments;
    public int selectedInstrument;
    float[] instrumentVolumes;
    float[] instrumentReverbs;
    public AudioMixerGroup[] audioMixerGroups;

    const float pitchOffset = 1.05946f;
    public static float bpmOffset;
    public static float timer;
    float gameTimer;
    bool playBack, pause;

    public static bool recording;

    void Start()
    {
        noteEditor = this;

        timer = 0;
        s_bpm = bpm;
        s_keyRange = keyRange;
        s_gridSize = gridSize;
        bpmOffset = 15 / bpm;

        gameTimer = 600f;

        playBack = false;
        pause = false;
        recording = false;

        instrumentVolumes = new float[instruments.Length];
        instrumentReverbs = new float[instruments.Length];

        for (int i = 0; i < instruments.Length; i++)
        {
            instrumentVolumes[i] = 0.75f;
            instrumentReverbs[i] = 0.25f;

            audioMixerGroups[i].name = instruments[i].name;
        }

        volumeKnob.value = instrumentVolumes[selectedInstrument];
        reverbKnob.value = instrumentReverbs[selectedInstrument];
    }

    void Update()
    {
        //Update Timer
        if (playBack && !pause)
        {
            timer += Time.deltaTime;
        }

        if (gameTimer > 0)
        {
            gameTimer -= Time.deltaTime;
        }
        else
        {
            GameEnd();
            return;
        }

        float minutes = ((int)gameTimer / 60);
        float seconds = (gameTimer % 60);

        //UI Update
        timerText.text = timer.ToString("00:00.000").Replace(',', ':');
        countdownText.text = minutes.ToString("00") + ":" + seconds.ToString("00");

        //Debug Input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayButtonPressed();
        }

        //Audio Updates
        instrumentVolumes[selectedInstrument] = volumeKnob.value;
        instrumentReverbs[selectedInstrument] = reverbKnob.value * 10f;
        audioMixerGroups[selectedInstrument].audioMixer.SetFloat(selectedInstrument + "_Reverb", instrumentReverbs[selectedInstrument]);

        //Place Notes
        if (editMode == EditMode.Place && Input.GetMouseButtonDown(0) && InsideScrollviewBounds() && !OverAnyNote())
        {
            Note newNote = Instantiate(notePrefab, noteParent).GetComponent<Note>();
            newNote.transform.localPosition = newNote.transform.InverseTransformPoint(Input.mousePosition) + Vector3Int.up;
            newNote.instrument = selectedInstrument;

            if (newNote.transform.localPosition.x < -1)
            {
                Destroy(newNote.gameObject);
            }
            else
            {
                newNote.UpdatePosition();
            }
        }
    }

    //Encodes the Notes of the Editor into the Sounddata
    public void EncodeSoundData()
    {
        bpmOffset = 15 / bpm;
        noteData = new List<string>();

        foreach (Note note in noteParent.GetComponentsInChildren<Note>())
        {
            string output = "[" + note.instrument + "," + note.value + "," + note.pos + "," + note.length.ToString().Replace(',', '.') + "]";
            noteData.Add(output);
        }
    }

    //Sorts the Sounddata after Playtime to minimize Delay
    public void SortSoundData()
    {
        List<string> newData = new List<string>();

        for (int c = 0; c < 32; c++)
        {
            for (int i = 0; i < noteData.Count; i++)
            {
                if (noteData[i] == "")
                {
                    continue;
                }

                string[] values = noteData[i].Replace('[', ' ').Replace(']', ' ').Split(',');
                int note = int.Parse(values[2]);

                if (note == c && !newData.Contains(noteData[i]))
                {
                    newData.Add(noteData[i]);
                }
            }
        }

        noteData = newData;
    }

    //Decodes the Sounddata into samples for the AudioSources to play all at once
    public void PlaySoundData(float delay = 0, float position = 0)
    {
        SortSoundData();

        for (int i = 0; i < noteData.Count; i++)
        {
            string[] values = noteData[i].Replace('[', ' ').Replace(']', ' ').Split(',');

            int instr = int.Parse(values[0]);
            int note = int.Parse(values[1]);
            float stTime = float.Parse(values[2], CultureInfo.InvariantCulture) + delay;
            float dur = float.Parse(values[3], CultureInfo.InvariantCulture);

            if (stTime * bpmOffset >= position)
            {
                PlaySound(instr, note, stTime * bpmOffset, dur * bpmOffset);
            }
        }
    }

    //Instantiates a new AudioSource and plays the correct sample at a given point
    public void PlaySound(int instrument, int note, float startTime, float duration, bool useRealTime = false)
    {
        AudioSource audioSource = Instantiate(audioSourcePrefab).GetComponent<AudioSource>();

        int index = Mathf.RoundToInt((float)note / 12);                 //Calculate correct Sample Index
        index = Mathf.Clamp(index, 0, keyRange / 12);                   //Clamps the Sample Index to prevent OutOfBounds-Error                    
        int octave = 12 * index;                                        //Calculates the current octave the note is in
        AudioClip sample = instruments[instrument].samples[index];      //Selects the correct Audio Sample
        float pitch = 1;                                                //Calculates the correct pitch

        if (instruments[instrument].affectedByPitch)
        {
            pitch = Mathf.Pow(pitchOffset, note - octave);
        }

        audioSource.clip = sample;
        audioSource.pitch = pitch;
        audioSource.volume = instrumentVolumes[selectedInstrument];
        audioSource.outputAudioMixerGroup = audioMixerGroups[instrument];

        audioSource.GetComponent<AudioPlayer>().PlayForSeconds(startTime, duration, useRealTime);
    }

    //Plays a sound via the Keyboard
    public void PlayKeyboardSound(int note)
    {
        PlaySound(selectedInstrument, note, Time.time, 1, true);
    }

    //Checks if Mouse Cursor is inside the Scrollview
    public bool InsideScrollviewBounds()
    {
        Vector3 relativeMousePosition = scrollView.InverseTransformPoint(Input.mousePosition);
        bool insideHorizontal = relativeMousePosition.x > scrollView.rect.xMin && relativeMousePosition.x < scrollView.rect.xMax;
        bool insideVertical = relativeMousePosition.y > scrollView.rect.yMin && relativeMousePosition.y < scrollView.rect.yMax;

        return insideHorizontal && insideVertical;
    }

    //Checks if Mouse Cursor is over any placed Note
    public bool OverAnyNote()
    {
        for (int i = 0; i < noteParent.childCount; i++)
        {
            if (noteParent.GetComponentsInChildren<Note>()[i].mouseOver)
            {
                return true;
            }
        }

        return false;
    }

    //Changes the Edit Mode to Placing
    public void PlaceButtonPressed()
    {
        if (editMode == EditMode.Place)
        {
            editMode = EditMode.None;
            Cursor.SetCursor(null, cursorOffsetPlace, CursorMode.ForceSoftware);
        }
        else
        {
            editMode = EditMode.Place;
            Cursor.SetCursor(cursorTexPlace, cursorOffsetPlace, CursorMode.ForceSoftware);
        }
    }

    //Changes the Edit Mode to Erasing
    public void EraseButtonPressed()
    {
        if (editMode == EditMode.Erase)
        {
            editMode = EditMode.None;
            Cursor.SetCursor(null, cursorOffsetErase, CursorMode.ForceSoftware);
        }
        else
        {
            editMode = EditMode.Erase;
            Cursor.SetCursor(cursorTexErase, cursorOffsetErase, CursorMode.ForceSoftware);
        }
    }

    //Handles the Audio Play, Pause and Resume Functions
    public void PlayButtonPressed()
    {
        EncodeSoundData();

        if (playBack)
        {
            pause = !pause;

            if (pause)
            {
                playButton.sprite = playButtonSprites[0];

                StopAllAudioPlayback();
            }
            else
            {
                playButton.sprite = playButtonSprites[1];

                PlaySoundData(0, timer);
                playBack = true;
            }
        }
        else
        {
            playButton.sprite = playButtonSprites[1];

            PlaySoundData();
            playBack = true;
            pause = false;
        }
    }

    //Handles the Note Recording State
    public void RecordButtonPressed()
    {
        recording = !recording;

        recordButton.color = recording ? Color.red : Color.white;
    }

    //Handles the Audio Stop Function
    public void StopButtonPressed()
    {
        playBack = false;
        pause = false;
        timer = 0;

        playButton.sprite = playButtonSprites[0];

        StopAllAudioPlayback();
    }

    //Changes the selected Instrument
    public void ChangeInstrument(int value)
    {
        selectedInstrument = value;
        volumeKnob.value = instrumentVolumes[value];
        reverbKnob.value = instrumentReverbs[value];
    }

    //Stops all AudioSources from playing
    void StopAllAudioPlayback()
    {
        foreach (AudioPlayer audioPlayer in FindObjectsOfType<AudioPlayer>())
        {
            audioPlayer.StopAllCoroutines();
            Destroy(audioPlayer.gameObject);
        }
    }

    //Adds BPM by amount and Updates the BPM UI Text
    public void AddBPM(float amount)
    {
        bpm += amount;
        bpm = Mathf.Clamp(bpm, 10, 250);
        bpmText.text = bpm + " BPM";

        s_bpm = bpm;
    }

    //Changes the Size of the Grid
    public void ChangeGridSize(float value)
    {
        gridSize = value;
        s_gridSize = gridSize;
    }

    //Gets the recorded Keyboard Notedata
    public static void SendRecordData(int note, float time, float duration)
    {
        noteEditor.ProcessRecordData(note, time, duration);
    }

    //Processes the recorded Keyboard Notedata
    public void ProcessRecordData(int note, float time, float duration)
    {
        Note newNote = Instantiate(notePrefab, noteParent).GetComponent<Note>();
        newNote.transform.localPosition = new Vector3(time / bpmOffset, note, 0);
        newNote.instrument = selectedInstrument;
        newNote.value = note;
        newNote.pos = time / bpmOffset;
        newNote.length = duration / bpmOffset;
    }

    //When the Gametimer reached zero
    public void GameEnd()
    {

    }
}