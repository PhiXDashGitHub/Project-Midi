using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text;

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
    public Sprite[] playButtonSprites;
    public TextMeshProUGUI bpmText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI countdownText;
    public ControlKnob volumeKnob;
    public ControlKnob reverbKnob;
    public Button[] instrumentButtons;

    [Header("Settings")]
    public int keyRange = 72;
    public static int s_keyRange;

    public float bpm = 140f;
    public static float s_bpm;

    public float gridSize = 1f;
    public static float s_gridSize;

    public GameObject audioSourcePrefab;

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
    public static float lastNoteLength;

    public float songLength;

    string filePath;
    string fileType;

    [Header("Online")]
    public Object votingScene;
    public bool isOnline = true;

    void Start()
    {
        noteEditor = this;

        timer = 0;
        s_bpm = bpm;
        s_keyRange = keyRange;
        s_gridSize = gridSize;
        bpmOffset = 15 / bpm;

        gameTimer = 300f;
        lastNoteLength = 1f;

        playBack = false;
        pause = false;
        recording = false;

        instrumentVolumes = new float[instruments.Length];
        instrumentReverbs = new float[instruments.Length];

        filePath = Application.persistentDataPath + "/Songs/";
        fileType = ".ndf";

        for (int i = 0; i < instruments.Length; i++)
        {
            instrumentVolumes[i] = 0.75f;
            instrumentReverbs[i] = 0.25f;
        }

        if (volumeKnob)
        {
            volumeKnob.value = instrumentVolumes[selectedInstrument];
            reverbKnob.value = instrumentReverbs[selectedInstrument];
        }

        EncodeSoundData();
    }

    void Update()
    {
        if (!enabled)
        {
            return;
        }

        //Update Timer
        if (playBack && !pause)
        {
            timer += Time.deltaTime;
        }

        if (gameTimer > 0)
        {
            gameTimer -= Time.deltaTime;
        }
        else if (isOnline)
        {
            GameEnd();
            return;
        }

        //UI Update
        if (timerText)
        {
            float minutes = (int)gameTimer / 60;
            float seconds = gameTimer % 60;

            float timerMinutes = (int)timer / 59;
            float timerSeconds = timer % 59;
            float timerMilliseconds = Mathf.Repeat(timer, 1.0f);

            timerText.text = timerMinutes.ToString("00") + ":" + timerSeconds.ToString("00") + ":" + timerMilliseconds.ToString(".000").Replace(',', ' ').Trim();
            countdownText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
        }
        

        //Debug Input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (playBack)
            {
                StopButtonPressed();
            }
            else
            {
                PlayButtonPressed();
            }
        }

        //Place Notes
        if (editMode == EditMode.Place && Input.GetMouseButtonDown(0) && InsideScrollviewBounds() && !OverAnyNote())
        {
            Note newNote = Instantiate(notePrefab, noteParent).GetComponent<Note>();
            newNote.transform.localPosition = newNote.transform.InverseTransformPoint(Input.mousePosition) + Vector3Int.up;
            newNote.instrument = selectedInstrument;
            newNote.length = lastNoteLength;

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

    void LateUpdate()
    {
        //Audio Updates
        if (!volumeKnob || !enabled)
        {
            return;
        }

        instrumentVolumes[selectedInstrument] = volumeKnob.value;
        instrumentReverbs[selectedInstrument] = reverbKnob.value;
        audioMixerGroups[selectedInstrument].audioMixer.SetFloat(selectedInstrument + "_Reverb", instrumentReverbs[selectedInstrument] * 10f);
    }

    //Encodes the Notes of the Editor into the Sounddata
    public void EncodeSoundData()
    {
        bpmOffset = 15 / bpm;
        noteData = new List<string>();

        foreach (Note note in noteParent.GetComponentsInChildren<Note>())
        {
            string output = "[" + note.instrument + "," + note.value + "," + note.pos.ToString().Replace(',', '.') + "," + note.length.ToString().Replace(',', '.') + "]";
            noteData.Add(output);
        }
    }

    //Decodes the Soundata into the Notes of the Editor
    public void DecodeSoundData()
    {
        foreach (Transform note in noteParent)
        {
            Destroy(note.gameObject);
        }

        foreach (string input in noteData)
        {
            string[] values = input.Replace('[', ' ').Replace(']', ' ').Split(',');

            int instr = int.Parse(values[0]);
            int note = int.Parse(values[1]);
            float stTime = float.Parse(values[2], CultureInfo.InvariantCulture);
            float dur = float.Parse(values[3], CultureInfo.InvariantCulture);

            Note newNote = Instantiate(notePrefab, noteParent).GetComponent<Note>();
            newNote.transform.localPosition = new Vector2(stTime, note);
            newNote.instrument = instr;
            newNote.length = dur;

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

    //Sorts the Sounddata after Playtime to minimize Delay
    public void SortSoundData()
    {
        List<string> newData = new List<string>();

        for (float c = 0; c < 332; c+=0.25f)
        {
            for (int i = 0; i < noteData.Count; i++)
            {
                if (noteData[i].Length < 4)
                {
                    continue;
                }

                string[] values = noteData[i].Replace('[', ' ').Replace(']', ' ').Split(',');
                float pos = float.Parse(values[2]);

                if (pos == c && !newData.Contains(noteData[i]))
                {
                    newData.Add(noteData[i]);
                    if (i == noteData.Count-1)
                    {
                        songLength =  (pos + float.Parse(values[3], CultureInfo.InvariantCulture)) * bpmOffset;
                    }
                }
            }
        }

        noteData = newData;
    }

    //Checks if Song Directory Exists, if not, creates it
    public void CheckSongDirectory()
    {
        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }
    }

    //Saves the NoteData to File
    public bool SaveToFile(string name, bool overrideFile = false)
    {
        CheckSongDirectory();
        EncodeSoundData();
        SortSoundData();

        string file = filePath + name + fileType;
        FileStream fileStream;

        if (overrideFile)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }

            fileStream = File.Create(file);
        }
        else
        {
            if (File.Exists(file))
            {
                Debug.LogError("File already exists.");
                return false;
            }

            fileStream = File.Create(file);
        }

        string content = bpm + "\n" + "\n";

        //Write InstrumentInfo
        foreach (Instrument instrument in instruments)
        {
            content += instrument.name + "\n";
        }

        content += "\n";

        //Write Instrument Volumes
        foreach (float value in instrumentVolumes)
        {
            content += value + "\n";
        }

        content += "\n";

        //Write Instrument Reverbs
        foreach (float value in instrumentReverbs)
        {
            content += value + "\n";
        }

        content += "\n";

        //Write NoteData
        foreach (string note in noteData)
        {
            content += note + "\n";
        }

        fileStream.Write(Encoding.ASCII.GetBytes(content), 0, Encoding.ASCII.GetByteCount(content));
        fileStream.Close();

        Debug.Log("File saved successfully.");
        return true;
    }

    //Loads the NoteData from File
    public bool LoadFromFile(string name)
    {
        string file = filePath + name + fileType;

        Instrument[] initInstruments = instruments;
        float[] initVolumes = instrumentVolumes;
        float[] initReverbs = instrumentReverbs;
        List<string> initNoteData = noteData;

        if (!File.Exists(file))
        {
            Debug.LogError("File not found.");
            return false;
        }

        try
        {
            string[] content = File.ReadAllLines(file);

            //Load BPM
            AddBPM(float.Parse(content[0]) - bpm);

            //Load Instruments
            string instr = "";
            for (int i = 2; i < 2 + instruments.Length; i++)
            {
                instr += content[i] + ";";
            }
            StringToInstruments(instr);

            //Load Instrument Volumes
            string instrVol = "";
            for (int i = instruments.Length + 3; i < instruments.Length * 2 + 3; i++)
            {
                instrVol += content[i] + ";";
            }
            StringToVolume(instrVol);

            //Load Instrument Reverbs
            string instrRev = "";
            for (int i = instruments.Length * 2 + 4; i < instruments.Length * 3 + 4; i++)
            {
                instrRev += content[i] + ";";
            }
            StringToReverb(instrRev);

            //Load NoteData
            noteData = new List<string>();
            for (int i = instruments.Length * 3 + 5; i < content.Length; i++)
            {
                noteData.Add(content[i]);
            }
            SortSoundData();
            DecodeSoundData();

            Debug.Log("File loaded successfully.");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("File corrupted: " + e);

            instruments = initInstruments;
            instrumentVolumes = initVolumes;
            instrumentReverbs = initReverbs;
            noteData = initNoteData;

            return false;
        }
    }

    //Converts the NoteData into a single string
    public string NoteDataToString()
    {
        string output = "";

        for (int i = 0; i < noteData.Count; i++)
        {
            output += noteData[i];

            if (i < noteData.Count - 1)
            {
                output += ";";
            }
        }

        return output;
    }

    //Converts a single string into NoteData
    public void StringToNoteData(string input)
    {
        if (input == null || input.Length < 4)
        {
            Debug.LogError("Note String is null");
            return;
        }

        noteData = new List<string>();
        string[] splitInput = input.Split(';');

        for (int i = 0; i < splitInput.Length; i++)
        {
            noteData.Add(splitInput[i]);
        }
    }

    //Converts a single string into InstrumentData
    public bool StringToInstruments(string input)
    {
        if (input == null || input.Length < 4)
        {
            Debug.LogError("Instrument String is null");
            return false;
        }

        instruments = new Instrument[6];
        string[] splitInput = input.Replace('[', ' ').Replace(']', ' ').Trim().Split(';');
        Instrument[] allInstruments = Resources.LoadAll<Instrument>("Instruments/");

        for (int i = 0; i < splitInput.Length; i++)
        {
            for (int r = 0; r < allInstruments.Length; r++)
            {
                if (allInstruments[r].name == splitInput[i] && splitInput[i].Length >1)
                {
                    instruments[i] = allInstruments[r];
                    instrumentButtons[i].gameObject.SetActive(true);
                }
            }
        }

        return true;
    }

    //Converts the VolumeData into a single string
    public string VolumeToString()
    {
        string output = "[";

        for (int i = 0; i < instrumentVolumes.Length; i++)
        {
            output += instrumentVolumes[i] + (i < instrumentVolumes.Length - 1 ? ";" : "]");
        }

        return output;
    }

    //Converts a single string into VolumeData
    public void StringToVolume(string input)
    {
        if (input == null || input.Length < 4)
        {
            Debug.LogError("Volume String is null");
            return;
        }

        string[] stringVolumes = input.Replace('[', ' ').Replace(']', ' ').Split(';');

        for (int i = 0; i < stringVolumes.Length; i++)
        {
            if (stringVolumes[i] == "")
            {
                continue;
            }

            instrumentVolumes[i] = float.Parse(stringVolumes[i]);
        }
    }

    //Converts the ReverbData into a single string
    public string ReverbToString()
    {
        string output = "[";

        for (int i = 0; i < instrumentReverbs.Length; i++)
        {
            output += instrumentReverbs[i] + (i < instrumentReverbs.Length - 1 ? ";" : "]");
        }

        return output;
    }

    //Converts a single string into ReverbData
    public void StringToReverb(string input)
    {
        if (input == null || input.Length < 4)
        {
            Debug.LogError("Reverb String is null");
            return;
        }

        string[] stringReverbs = input.Replace('[', ' ').Replace(']', ' ').Split(';');

        for (int i = 0; i < stringReverbs.Length; i++)
        {
            if (stringReverbs[i] == "")
            {
                continue;
            }

            instrumentReverbs[i] = float.Parse(stringReverbs[i]);
            audioMixerGroups[i].audioMixer.SetFloat(i + "_Reverb", instrumentReverbs[i] * 10f);
        }
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
        AudioClip sample = instruments[instrument].samples[0];

        float pitch = 1;

        if (instruments[instrument].affectedByPitch)
        {
            int index = Mathf.RoundToInt((float)note / 12);                 //Calculate correct Sample Index
            index = Mathf.Clamp(index, 0, keyRange / 12);                   //Clamps the Sample Index to prevent OutOfBounds-Error                    
            int octave = 12 * index;                                        //Calculates the current octave the note is in
            sample = instruments[instrument].samples[index];                //Selects the correct Audio Sample
            pitch = Mathf.Pow(pitchOffset, note - octave);                  //Calculates the correct pitch
        }
        else
        {
            sample = note < instruments[instrument].samples.Length ? instruments[instrument].samples[note] : null;
        }

        audioSource.clip = sample;
        audioSource.pitch = pitch;
        audioSource.volume = instrumentVolumes[instrument];
        audioSource.outputAudioMixerGroup = audioMixerGroups[instrument];

        audioSource.GetComponent<AudioPlayer>().PlayForSeconds(startTime, duration, useRealTime, instruments[instrument].decay);
    }

    //Plays a sound via the Keyboard
    public void PlayKeyboardSound(int note)
    {
        PlaySound(selectedInstrument, note, Time.time, 1, true);
    }

    //Checks if Mouse Cursor is inside the Scrollview
    public bool InsideScrollviewBounds()
    {
        if (!scrollView)
        {
            return false;
        }

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
        bpm = Mathf.Clamp(bpm, 80, 220);
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
        if (SceneManager.GetActiveScene().name == votingScene.name)
        {
            return;
        }

        Cursor.SetCursor(null, cursorOffsetErase, CursorMode.ForceSoftware);
        EncodeSoundData();

        GetComponent<SendSong>().bpm = (int)bpm;
        GetComponent<SendSong>().reverb = ReverbToString();
        GetComponent<SendSong>().volume = VolumeToString();
        GetComponent<SendSong>().votingscene = votingScene;
        GetComponent<SendSong>().Send(NoteDataToString());
        enabled = false;
    }
}
