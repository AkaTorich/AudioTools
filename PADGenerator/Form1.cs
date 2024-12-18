using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using NAudio.Midi;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace MIDIGenerator
{
    public partial class Form1 : Form
    {
        // Вспомогательный класс для представления скейла с его стилем
        public class ScaleItem
        {
            public string Name { get; set; }
            public string Style { get; set; }

            public override string ToString()
            {
                // Отображение скейла с его стилем
                return $"{Name} ({Style})";
            }
        }

        // Словарь скейлов с их ступенями и качествами
        private readonly Dictionary<string, Tuple<string[], string[]>> scalesChords = new Dictionary<string, Tuple<string[], string[]>>()
        {
            { "Major", Tuple.Create(
                new string[] { "I", "ii", "iii", "IV", "V", "vi", "vii°" },
                new string[] { "maj", "min", "min", "maj", "maj", "min", "dim" }
            ) },
            { "Minor", Tuple.Create(
                new string[] { "i", "ii°", "III", "iv", "v", "VI", "VII" },
                new string[] { "min", "dim", "maj", "min", "min", "maj", "maj" }
            ) },
            { "Harmonic Minor", Tuple.Create(
                new string[] { "i", "ii°", "III+", "iv", "V", "VI", "vii°" },
                new string[] { "min", "dim", "aug", "min", "maj", "maj", "dim" }
            ) },
            { "Melodic Minor", Tuple.Create(
                new string[] { "i", "ii", "III+", "IV", "V", "vi°", "vii°" },
                new string[] { "min", "min", "aug", "maj", "maj", "dim", "dim" }
            ) },
            { "Dorian", Tuple.Create(
                new string[] { "i", "ii", "III", "IV", "v", "vi°", "VII" },
                new string[] { "min", "min", "maj", "maj", "min", "dim", "maj" }
            ) },
            { "Phrygian", Tuple.Create(
                new string[] { "i", "II", "III", "iv", "v°", "VI", "vii" },
                new string[] { "min", "maj", "maj", "min", "dim", "maj", "maj" }
            ) },
            { "Lydian", Tuple.Create(
                new string[] { "I", "II", "iii", "#iv°", "V", "vi", "vii" },
                new string[] { "maj", "maj", "min", "dim", "maj", "min", "dim" }
            ) },
            { "Mixolydian", Tuple.Create(
                new string[] { "I", "ii", "iii°", "IV", "v", "vi", "VII" },
                new string[] { "maj", "min", "dim", "maj", "min", "min", "maj" }
            ) },
            { "Locrian", Tuple.Create(
                new string[] { "i°", "II", "iii", "iv", "V", "VI", "vii" },
                new string[] { "dim", "maj", "min", "min", "maj", "maj", "min" }
            ) },
            { "Pentatonic Major", Tuple.Create(
                new string[] { "I", "II", "III", "V", "VI" },
                new string[] { "maj", "maj", "maj", "maj", "maj" }
            ) },
            { "Pentatonic Minor", Tuple.Create(
                new string[] { "i", "III", "IV", "V", "VII" },
                new string[] { "min", "min", "min", "min", "min" }
            ) },
            { "Blues", Tuple.Create(
                new string[] { "i", "III", "IV", "bV", "V", "VII" },
                new string[] { "min", "maj", "min", "dim", "min", "maj" }
            ) },
            { "Hungarian Minor", Tuple.Create(
                new string[] { "i", "#ii", "III", "#iv", "V", "VI", "vii°" },
                new string[] { "min", "dim", "maj", "dim", "maj", "maj", "dim" }
            ) },
            { "Spanish Phrygian", Tuple.Create(
                new string[] { "i", "II", "III", "iv°", "v", "VI", "VII" },
                new string[] { "min", "maj", "maj", "dim", "min", "maj", "maj" }
            ) },
            { "Whole Tone", Tuple.Create(
                new string[] { "I+", "II+", "III+", "#iv°", "#V+", "#vi+" },
                new string[] { "aug", "aug", "aug", "dim", "aug", "aug" }
            ) },
            { "Phrygian Dominant", Tuple.Create(
                new string[] { "i", "II", "III+", "iv", "v°", "VI", "vii" },
                new string[] { "min", "maj", "aug", "min", "dim", "maj", "min" }
            ) },
            { "Hijaz", Tuple.Create(
                new string[] { "i", "II", "III+", "iv", "v", "VI", "vii°" },
                new string[] { "min", "maj", "aug", "min", "min", "maj", "dim" }
            ) },
            { "Hirajoshi", Tuple.Create(
                new string[] { "i", "II", "iii", "v", "VI" },
                new string[] { "min", "maj", "min", "min", "maj" }
            ) },
            { "In Sen", Tuple.Create(
                new string[] { "i", "II", "iv", "V", "vii" },
                new string[] { "min", "maj", "min", "maj", "min" }
            ) },
            { "Kumoi", Tuple.Create(
                new string[] { "i", "ii", "IV", "V", "vi" },
                new string[] { "min", "min", "maj", "maj", "min" }
            ) },
            { "Blues Hexatonic", Tuple.Create(
                new string[] { "i", "III", "IV", "V", "VI", "VII" },
                new string[] { "min", "maj", "maj", "maj", "maj", "maj" }
            ) },
            { "Prometheus", Tuple.Create(
                new string[] { "I", "II", "iii", "#iv", "vi", "bVII" },
                new string[] { "maj", "maj", "min", "aug", "min", "maj" }
            ) },
            { "Double Harmonic Major", Tuple.Create(
                new string[] { "I", "bII", "iii", "IV", "V", "bVI", "vii°" },
                new string[] { "maj", "maj", "min", "maj", "maj", "maj", "dim" }
            ) },
        };

        private readonly HashSet<string> psytranceScales = new HashSet<string>()
        {
            "Harmonic Minor",
            "Phrygian",
            "Hirajoshi",
            "In Sen",
            "Phrygian Dominant",
            "Double Harmonic Major"
        };

        private readonly HashSet<string> tranceScales = new HashSet<string>()
        {
            "Major",
            "Minor",
            "Dorian",
            "Lydian",
            "Mixolydian",
            "Melodic Minor",
            "Pentatonic Major",
            "Pentatonic Minor",
            "Blues",
            "Hungarian Minor",
            "Spanish Phrygian",
            "Whole Tone",
            "Locrian",
            "Blues Hexatonic",
            "Prometheus"
        };

        private readonly Dictionary<string, int[]> chordFormulas = new Dictionary<string, int[]>()
        {
            { "", new[] { 0, 4, 7 } },
            { "maj", new[] { 0, 4, 7 } },
            { "m", new[] { 0, 3, 7 } },
            { "dim", new[] { 0, 3, 6 } },
            { "aug", new[] { 0, 4, 8 } },
            { "add9", new[] { 0, 4, 7, 14 } },
            { "sus4", new[] { 0, 5, 7 } },
            { "7", new[] { 0, 4, 7, 10 } },
            { "maj7", new[] { 0, 4, 7, 11 } },
            { "m7", new[] { 0, 3, 7, 10 } },
            { "dim7", new[] { 0, 3, 6, 9 } },
            { "m7b5", new[] { 0, 3, 6, 10 } },
            { "m9", new[] { 0, 3, 7, 10, 14 } },
            { "maj9", new[] { 0, 4, 7, 11, 14 } },
            { "9", new[] { 0, 4, 7, 10, 14 } },
            { "aug7", new[] { 0, 4, 8, 10 } },
            { "5", new[] { 0, 7 } },
            { "+", new[] { 0, 4, 8 } },
            { "°", new[] { 0, 3, 6 } },
        };

        private readonly Random random = new Random();
        private string lastGeneratedMidiPath = string.Empty;

        public Form1()
        {
            InitializeComponent();
            InitializeTonalityComboBox();
            InitializeScaleComboBox();

            cmbBoxTonality.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbBoxScale.DropDownStyle = ComboBoxStyle.DropDownList;

            foreach (var scale in scalesChords)
            {
                if (scale.Value.Item1.Length != scale.Value.Item2.Length)
                {
                    Log($"Несоответствие количества ступеней и качеств в скейле: {scale.Key}");
                }
            }
        }

        private void InitializeTonalityComboBox()
        {
            cmbBoxTonality.Items.AddRange(new string[]
            {
                "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B",
                "Cm", "C#m", "Dm", "D#m", "Em", "Fm", "F#m", "Gm", "G#m", "Am", "A#m", "Bm"
            });
            cmbBoxTonality.SelectedIndex = 0;
        }

        private void InitializeScaleComboBox()
        {
            cmbBoxScale.Items.Clear();

            foreach (var scale in scalesChords.Keys)
            {
                string style = "None";
                bool isPsytrance = psytranceScales.Contains(scale);
                bool isTrance = tranceScales.Contains(scale);

                if (isPsytrance && isTrance)
                {
                    style = "Psytrance, Trance";
                }
                else if (isPsytrance)
                {
                    style = "Psytrance";
                }
                else if (isTrance)
                {
                    style = "Trance";
                }

                cmbBoxScale.Items.Add(new ScaleItem { Name = scale, Style = style });
            }

            if (cmbBoxScale.Items.Count > 0)
            {
                cmbBoxScale.SelectedIndex = 0;
            }

            cmbBoxScale.DisplayMember = "ToString";
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            string selectedTonality = cmbBoxTonality.SelectedItem.ToString();
            string selectedScale = "";

            if (cmbBoxScale.SelectedItem is ScaleItem selectedScaleItem)
            {
                selectedScale = selectedScaleItem.Name;
            }
            else
            {
                MessageBox.Show("Выберите скейл из списка.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int maxUniqueChords;
            int numberOfChords;

            // txtNumberOfChords - количество уникальных аккордов
            if (!int.TryParse(txtNumberOfChords.Text, out maxUniqueChords) || maxUniqueChords <= 0)
            {
                MessageBox.Show("Введите корректное количество уникальных аккордов.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // txtProgressionLength - общее количество аккордов
            if (!int.TryParse(txtProgressionLength.Text, out numberOfChords) || numberOfChords <= 0)
            {
                MessageBox.Show("Введите корректное общее количество аккордов в прогрессии.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (numberOfChords < maxUniqueChords)
            {
                MessageBox.Show("Общее количество аккордов не может быть меньше количества уникальных аккордов.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!scalesChords.ContainsKey(selectedScale))
            {
                MessageBox.Show($"Скейл '{selectedScale}' не поддерживается.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                var chordProgression = GenerateChordProgression(selectedTonality, selectedScale, maxUniqueChords, numberOfChords);

                if (chordProgression.Count > 0)
                {
                    txtProgression.Text = string.Join(" - ", chordProgression);

                    lastGeneratedMidiPath = SaveChordProgressionToMidi(chordProgression, selectedTonality, selectedScale);

                    btnPlayMIDI.Enabled = true;

                    MessageBox.Show("Прогрессия успешно сохранена в папку 'Generated'.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Не удалось сгенерировать прогрессию аккордов.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<string> GenerateChordProgression(string tonality, string scale, int maxUniqueChords, int numberOfChords)
        {
            var progression = new List<string>();
            var scaleData = scalesChords[scale];
            var degrees = scaleData.Item1;
            var qualities = scaleData.Item2;

            var allChords = new List<string>();
            for (int i = 0; i < degrees.Length; i++)
            {
                string degree = degrees[i];
                string quality = qualities[i];

                string chordName = GetChordName(tonality, degree, quality);

                if (!string.IsNullOrEmpty(chordName))
                {
                    allChords.Add(chordName);
                }
                else
                {
                    Log($"Не удалось получить имя аккорда для ступени: {degree}");
                }
            }

            if (allChords.Count == 0)
            {
                MessageBox.Show("Не удалось получить список аккордов для выбранного скейла.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return progression;
            }

            if (maxUniqueChords > allChords.Count)
            {
                maxUniqueChords = allChords.Count;
            }

            var selectedChordsSet = new HashSet<string>();
            while (selectedChordsSet.Count < maxUniqueChords)
            {
                int index = random.Next(allChords.Count);
                selectedChordsSet.Add(allChords[index]);
            }

            var selectedChords = selectedChordsSet.ToList();

            for (int i = 0; i < numberOfChords; i++)
            {
                string nextChord;
                int attempts = 0;
                do
                {
                    int index = random.Next(selectedChords.Count);
                    nextChord = selectedChords[index];
                    attempts++;
                    if (attempts > 100)
                    {
                        MessageBox.Show("Не удалось сгенерировать прогрессию с заданными ограничениями.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return progression;
                    }
                }
                while (i >= 2 && progression[i - 1] == nextChord && progression[i - 2] == nextChord);

                progression.Add(nextChord);
            }

            return progression;
        }
        private string SaveChordProgressionToMidi(List<string> chordProgression, string tonality, string scale)
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string generatedDirectory = Path.Combine(appDirectory, "Generated");
            if (!Directory.Exists(generatedDirectory))
            {
                Directory.CreateDirectory(generatedDirectory);
            }

            string chordsShort = string.Join("_", chordProgression.Take(4)).Replace("-", "").Replace(" ", "");
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                chordsShort = chordsShort.Replace(c, '_');
            }

            string fileName = $"{tonality}_{scale}_{chordsShort}.mid";
            string fullPath = Path.Combine(generatedDirectory, fileName);

            var midiFile = new MidiEventCollection(0, 480);

            // Устанавливаем темп (120 BPM для примера)
            midiFile.AddEvent(new TempoEvent(500000, 0), 0);

            int chordChannel = 1;
            midiFile.AddEvent(new PatchChangeEvent(0, chordChannel, 0), 0); // Acoustic Grand Piano, при необходимости замените на другой тембр

            int ticksPerQuarter = 480;
            int beatsPerMeasure = 4;
            int measuresPerChord = 8; // Продление такта на два, как вы хотели
            int noteDuration = ticksPerQuarter * beatsPerMeasure * measuresPerChord;
            // Это будет длительность одного аккорда (например, 8 тактов по 4/4 при 120BPM)

            int currentTime = 0;
            int velocity = 100;

            // Логика пэдов:
            // Для каждого аккорда — один NoteOn в начале и один NoteOff в конце большой длительности.

            foreach (var chord in chordProgression)
            {
                var notes = GetMidiNotesForChord(chord);

                // Включаем все ноты аккорда на currentTime
                foreach (var note in notes)
                {
                    midiFile.AddEvent(new NoteOnEvent(currentTime, chordChannel, note, velocity, 0), 0);
                }

                // Выключаем все ноты аккорда после noteDuration
                int noteOffTime = currentTime + noteDuration;
                foreach (var note in notes)
                {
                    midiFile.AddEvent(new NoteEvent(noteOffTime, chordChannel, MidiCommandCode.NoteOff, note, 0), 0);
                }

                // Переходим к следующему аккорду после noteDuration
                currentTime += noteDuration;
            }

            midiFile.AddEvent(new MetaEvent(MetaEventType.EndTrack, 0, currentTime), 0);
            MidiFile.Export(fullPath, midiFile);

            return fullPath;
        }

        private int[] GetMidiNotesForChord(string chord)
        {
            if (string.IsNullOrEmpty(chord))
            {
                Log("Название аккорда пусто или null");
                return new int[0];
            }

            string rootNote = "";
            string chordType = "";

            if (char.IsLetter(chord[0]))
            {
                rootNote += chord[0];
                if (chord.Length > 1 && (chord[1] == '#' || chord[1] == 'b'))
                {
                    rootNote += chord[1];
                    if (chord.Length > 2)
                    {
                        chordType = chord.Substring(2).ToLower();
                    }
                }
                else
                {
                    if (chord.Length > 1)
                    {
                        chordType = chord.Substring(1).ToLower();
                    }
                }
            }
            else
            {
                Log($"Неизвестный формат аккорда: {chord}");
                return new int[0];
            }

            Log($"Разбор аккорда: {chord}, корень: {rootNote}, тип: {chordType}");

            int rootMidi = GetMidiNumber(rootNote);
            if (rootMidi == -1)
            {
                Log($"Неизвестная корневая нота: {rootNote}");
                return new int[0];
            }

            if (!chordFormulas.ContainsKey(chordType))
            {
                Log($"Неизвестный тип аккорда: {chordType}");
                return new int[0];
            }

            int[] intervals = chordFormulas[chordType];

            List<int> midiNotes = new List<int>();
            foreach (var interval in intervals)
            {
                int note = rootMidi + interval;

                int lowerLimit = 60;
                int upperLimit = lowerLimit + 24;

                while (note < lowerLimit)
                {
                    note += 12;
                }
                while (note >= upperLimit)
                {
                    note -= 12;
                }

                midiNotes.Add(note);
            }

            Log($"Аккорд {chord} MIDI ноты: {string.Join(", ", midiNotes)}");
            return midiNotes.ToArray();
        }

        private int GetMidiNumber(string note)
        {
            Dictionary<string, int> noteToMidi = new Dictionary<string, int>()
            {
                { "C", 60 },
                { "C#", 61 },
                { "Db", 61 },
                { "D", 62 },
                { "D#", 63 },
                { "Eb", 63 },
                { "E", 64 },
                { "F", 65 },
                { "F#", 66 },
                { "Gb", 66 },
                { "G", 67 },
                { "G#", 68 },
                { "Ab", 68 },
                { "A", 69 },
                { "A#", 70 },
                { "Bb", 70 },
                { "B", 71 }
            };

            if (noteToMidi.ContainsKey(note))
            {
                return noteToMidi[note];
            }
            else
            {
                return -1;
            }
        }

        private string GetChordName(string tonality, string degree, string quality)
        {
            string root = tonality;
            bool isMinorTonality = false;

            if (tonality.EndsWith("m"))
            {
                root = tonality.Substring(0, tonality.Length - 1);
                isMinorTonality = true;
                Log($"Тональность минорная. Корень аккорда: {root}");
            }
            else
            {
                Log($"Тональность мажорная. Корень аккорда: {root}");
            }

            int semitones = GetSemitonesFromDegree(degree);
            if (semitones == -1)
            {
                Log($"Не удалось определить количество полутонов для ступени: {degree}");
                return null;
            }

            string chordRoot = Transpose(root, semitones);
            if (string.IsNullOrEmpty(chordRoot))
            {
                Log($"Не удалось транспонировать корень: {root} на {semitones} полутонов");
                return null;
            }

            string chordName = chordRoot;

            switch (quality)
            {
                case "maj":
                    break;
                case "min":
                    chordName += "m";
                    break;
                case "dim":
                    chordName += "dim";
                    break;
                case "aug":
                    chordName += "aug";
                    break;
                case "sus4":
                    chordName += "sus4";
                    break;
                case "add9":
                    chordName += "add9";
                    break;
                case "7":
                    chordName += "7";
                    break;
                case "maj7":
                    chordName += "maj7";
                    break;
                case "m7":
                    chordName += "m7";
                    break;
                case "dim7":
                    chordName += "dim7";
                    break;
                case "m7b5":
                    chordName += "m7b5";
                    break;
                case "m9":
                    chordName += "m9";
                    break;
                case "maj9":
                    chordName += "maj9";
                    break;
                case "9":
                    chordName += "9";
                    break;
                case "aug7":
                    chordName += "aug7";
                    break;
                case "5":
                    chordName += "5";
                    break;
                case "+":
                    chordName += "+";
                    break;
                case "°":
                    chordName += "°";
                    break;
                default:
                    chordName += quality;
                    break;
            }

            Log($"Сгенерирован аккорд: {chordName}");
            return chordName;
        }

        private int GetSemitonesFromDegree(string degree)
        {
            int semitones = 0;
            int accidental = 0;

            string degreeWithoutSuffix = degree.TrimEnd('°', '+');

            string pattern = @"^([b#]?)(i{1,3}|v{1,3}|iv|vi{0,2}|vii)$";
            var match = Regex.Match(degreeWithoutSuffix.ToLower(), pattern);

            if (!match.Success)
            {
                Log($"Неизвестная ступень: {degree}");
                return -1;
            }

            string accidentalSymbol = match.Groups[1].Value;
            string romanNumeral = match.Groups[2].Value;

            if (accidentalSymbol == "b")
                accidental = -1;
            else if (accidentalSymbol == "#")
                accidental = +1;

            switch (romanNumeral.ToLower())
            {
                case "i":
                    semitones = 0;
                    break;
                case "ii":
                    semitones = 2;
                    break;
                case "iii":
                    semitones = 4;
                    break;
                case "iv":
                    semitones = 5;
                    break;
                case "v":
                    semitones = 7;
                    break;
                case "vi":
                    semitones = 9;
                    break;
                case "vii":
                    semitones = 11;
                    break;
                default:
                    Log($"Неизвестная ступень: {degree}");
                    return -1;
            }

            semitones += accidental;
            semitones = (semitones + 12) % 12;

            return semitones;
        }

        private string Transpose(string note, int semitones)
        {
            string[] notesSharp = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
            string[] notesFlat = { "C", "Db", "D", "Eb", "E", "F", "Gb", "G", "Ab", "A", "Bb", "B" };

            int index = Array.IndexOf(notesSharp, note);
            bool isSharp = true;
            if (index == -1)
            {
                index = Array.IndexOf(notesFlat, note);
                isSharp = false;
                if (index == -1)
                {
                    Log($"Неизвестная нота для транспонирования: {note}");
                    return "C";
                }
            }

            int newIndex = (index + semitones) % 12;
            if (newIndex < 0) newIndex += 12;

            return isSharp ? notesSharp[newIndex] : notesFlat[newIndex];
        }

        private void Log(string message)
        {
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");
            string logMessage = $"{DateTime.Now}: {message}{Environment.NewLine}";
            try
            {
                File.AppendAllText(logPath, logMessage, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось записать лог: {ex.Message}", "Ошибка логирования", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPlayMIDI_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(lastGeneratedMidiPath) || !File.Exists(lastGeneratedMidiPath))
            {
                MessageBox.Show("Нет сгенерированного MIDI-файла для воспроизведения.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = lastGeneratedMidiPath,
                    UseShellExecute = true
                };
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось воспроизвести MIDI-файл: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
