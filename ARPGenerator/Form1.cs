using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using NAudio.Midi;

namespace MelodyGenerator
{
    public partial class Form1 : Form
    {
        private readonly Dictionary<string, int[]> scalesIntervals = new Dictionary<string, int[]>()
        {
            { "Phrygian Dominant", new[] { 0, 1, 4, 5, 7, 8, 11 } },
            { "Harmonic Minor", new[] { 0, 2, 3, 5, 7, 8, 11 } },
            { "Phrygian", new[] { 0, 1, 3, 5, 7, 8, 10 } },
            { "Dorian", new[] { 0, 2, 3, 5, 7, 9, 10 } },
            { "Mixolydian", new[] { 0, 2, 4, 5, 7, 9, 10 } },
            { "Lydian", new[] { 0, 2, 4, 6, 7, 9, 11 } },
            { "Locrian", new[] { 0, 1, 3, 5, 6, 8, 10 } },
            { "Whole Tone", new[] { 0, 2, 4, 6, 8, 10 } },
            { "Pentatonic Minor", new[] { 0, 3, 5, 7, 10 } },
            { "Pentatonic Major", new[] { 0, 2, 4, 7, 9 } },
            { "Blues", new[] { 0, 3, 5, 6, 7, 10 } },
            { "Hungarian Minor", new[] { 0, 2, 3, 6, 7, 8, 11 } },
            { "Spanish Phrygian", new[] { 0, 1, 4, 5, 7, 8, 10 } },
            { "Prometheus", new[] { 0, 2, 4, 6, 9, 10 } },
            { "Double Harmonic Major", new[] { 0, 1, 4, 5, 7, 8, 11 } },
            { "Melodic Minor", new[] { 0, 2, 3, 5, 7, 9, 11 } },
            { "Ionian", new[] { 0, 2, 4, 5, 7, 9, 11 } },
            { "Aeolian", new[] { 0, 2, 3, 5, 7, 8, 10 } },
        };

        private readonly Dictionary<string, int> noteOffsets = new Dictionary<string, int>()
        {
            { "C", 0 }, { "C#", 1 }, { "Db", 1 }, { "D", 2 }, { "D#", 3 }, { "Eb", 3 },
            { "E", 4 }, { "F", 5 }, { "F#", 6 }, { "Gb", 6 }, { "G", 7 }, { "G#", 8 },
            { "Ab", 8 }, { "A", 9 }, { "A#", 10 }, { "Bb", 10 }, { "B", 11 },
        };

        private List<List<int>> allArpeggioPatterns = new List<List<int>>();
        private string lastGeneratedMidiPath = string.Empty;
        private readonly Random random = new Random();

        // Ключ для XOR
        private byte xorKey = 0xAA;

        public Form1()
        {
            InitializeComponent();
            InitializeTonicaComboBox();
            InitializeScaleComboBox();
            InitializePlayButton();
            LoadArpeggioPatternsFromFile();
        }

        private void LoadArpeggioPatternsFromFile()
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(appDirectory, "Libraries\\database.db");

            if (!File.Exists(filePath))
            {
                MessageBox.Show("Файл database.db не найден. Поместите его рядом с исполняемым файлом.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            byte[] encryptedData = File.ReadAllBytes(filePath);
            byte[] decryptedData = XorDecryptData(encryptedData, xorKey);
            string content = System.Text.Encoding.UTF8.GetString(decryptedData);

            var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var parts = line.Split(':');
                if (parts.Length < 2) continue;

                string notesPart = parts[1].Trim();
                if (string.IsNullOrWhiteSpace(notesPart))
                    continue;

                var notes = notesPart.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                     .Select(n => n.Trim())
                                     .ToList();

                List<int> pattern = new List<int>();
                foreach (var noteName in notes)
                {
                    int midiNumber = NoteNameToMidi(noteName);
                    if (midiNumber < 60 || midiNumber > 83)
                    {
                        midiNumber = Clamp(midiNumber, 60, 83);
                    }
                    pattern.Add(midiNumber);
                }

                if (pattern.Count > 0)
                {
                    int baseNote = pattern[0];
                    var intervals = pattern.Select(n => n - baseNote).ToList();
                    allArpeggioPatterns.Add(intervals);
                }
            }

            if (allArpeggioPatterns.Count == 0)
            {
                MessageBox.Show("Не удалось загрузить ни одного паттерна из файла database.db", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private byte[] XorDecryptData(byte[] data, byte key)
        {
            byte[] result = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                result[i] = (byte)(data[i] ^ key);
            }
            return result;
        }

        private int NoteNameToMidi(string noteName)
        {
            int octave = 4;
            string notePart = noteName;
            for (int i = 0; i < noteName.Length; i++)
            {
                if (char.IsDigit(noteName[i]) || (noteName[i] == '-' && i > 0))
                {
                    notePart = noteName.Substring(0, i);
                    string octavePart = noteName.Substring(i);
                    octave = int.Parse(octavePart);
                    break;
                }
            }

            notePart = notePart.Replace("Db", "C#")
                               .Replace("Eb", "D#")
                               .Replace("Gb", "F#")
                               .Replace("Ab", "G#")
                               .Replace("Bb", "A#");

            if (!noteOffsets.TryGetValue(notePart, out int offset))
            {
                return 60; // fallback C4
            }

            int midi = (octave + 1) * 12 + offset;
            return midi;
        }

        private void InitializeTonicaComboBox()
        {
            cmbBoxTonica.Items.AddRange(new string[]
            {
                "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B",
                "Cm", "C#m", "Dm", "D#m", "Em", "Fm", "F#m", "Gm", "G#m", "Am", "A#m", "Bm"
            });
            cmbBoxTonica.SelectedIndex = 0;
        }

        private void InitializeScaleComboBox()
        {
            cmbBoxScale.Items.AddRange(scalesIntervals.Keys.ToArray());
            cmbBoxScale.SelectedIndex = 0;
        }

        private void InitializePlayButton()
        {
            btnPlayMidi.Click += BtnPlayMidi_Click;
            btnPlayMidi.Enabled = false;
        }

        private void TxtNumeric_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            string selectedTonica = cmbBoxTonica.SelectedItem.ToString();
            string selectedScale = cmbBoxScale.SelectedItem.ToString();
            int arpNotesCount;
            int uniqueMeasuresNumber;
            int repeatsNumber;

            if (!int.TryParse(txtArpNotesCount.Text, out arpNotesCount) || arpNotesCount <= 0)
            {
                MessageBox.Show("Введите корректное количество нот для арпеджио.", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(txtTactsNumber.Text, out uniqueMeasuresNumber) || uniqueMeasuresNumber <= 0)
            {
                MessageBox.Show("Введите корректное количество уникальных тактов.", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(txtRepeatsNumber.Text, out repeatsNumber) || repeatsNumber < 0)
            {
                MessageBox.Show("Введите корректное количество повторений.", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!scalesIntervals.ContainsKey(selectedScale))
            {
                MessageBox.Show($"Гамма '{selectedScale}' не поддерживается.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (repeatsNumber > uniqueMeasuresNumber)
            {
                MessageBox.Show("Количество повторений не может превышать количество уникальных тактов.", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (allArpeggioPatterns.Count == 0)
            {
                MessageBox.Show("Нет загруженных паттернов. Невозможно сгенерировать мелодию.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                int totalMeasuresNumber = uniqueMeasuresNumber + repeatsNumber;
                var uniqueMeasures = GenerateUniqueMeasures(selectedTonica, selectedScale, arpNotesCount, uniqueMeasuresNumber);
                var repeatedMeasures = GenerateRepeatedMeasures(uniqueMeasures, repeatsNumber);
                var finalMelody = CombineMeasures(uniqueMeasures, repeatedMeasures, totalMeasuresNumber);

                if (finalMelody.Count > 0)
                {
                    // В лог добавляем только уникальные ноты в каждом такте
                    txtProgression.Text = string.Join("\r\n", finalMelody.Select((measure, index) =>
                    {
                        var uniqueNotes = measure.Distinct().Select(n => MidiNumberToNoteName(n)).ToList();
                        return $"Такт {index + 1}: " + string.Join(", ", uniqueNotes);
                    }));

                    lastGeneratedMidiPath = SaveMelodyToMidi(finalMelody, selectedTonica, selectedScale, arpNotesCount);

                    btnPlayMidi.Enabled = true;
                    MessageBox.Show("MIDI файл мелодии успешно сохранен в папку 'Generated'.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Не удалось сгенерировать мелодию.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
private List<List<int>> GenerateUniqueMeasures(string tonica, string scale, int arpNotesCount, int uniqueMeasuresNumber)
{
    var uniqueMeasures = new List<List<int>>();
    var scaleNotes = GetScaleNotes(tonica, scale);

    if (scaleNotes.Count == 0)
    {
        throw new Exception("Гамма не содержит нот.");
    }

    for (int i = 0; i < uniqueMeasuresNumber; i++)
    {
        var motifPattern = GetRandomArpeggioPattern(allArpeggioPatterns, arpNotesCount);

        int baseNote = 69; // A4
        var motif = motifPattern.Select(interval => Clamp(baseNote + interval, 60, 83)).ToList();

        motif = MakeSimilarButNotIdentical(motif);

        // Изначально генерируем такт на 16 нот
        var measure = GenerateMeasure(motif, 16);

        // Теперь удлиняем такт, добавляя ещё 16 нот, тем самым общий размер будет 32 ноты (2 такта)
        measure.AddRange(GenerateMeasure(motif, 16));

        uniqueMeasures.Add(measure);
    }

    return uniqueMeasures;
}

        private List<int> GetRandomArpeggioPattern(List<List<int>> patterns, int arpNotesCount)
        {
            var filtered = patterns.Where(p => p.Count == arpNotesCount).ToList();
            if (filtered.Count == 0)
            {
                return GenerateRandomArpeggioPattern(arpNotesCount);
            }
            return filtered[random.Next(filtered.Count)];
        }

        private List<int> GenerateRandomArpeggioPattern(int arpNotesCount)
        {
            var pattern = new List<int>();
            for (int i = 0; i < arpNotesCount; i++)
            {
                pattern.Add(random.Next(-2, 8));
            }
            return pattern;
        }

        private List<int> MakeSimilarButNotIdentical(List<int> motif)
        {
            double probabilityShift = 0.2; // 20%
            for (int i = 0; i < motif.Count; i++)
            {
                if (random.NextDouble() < probabilityShift)
                {
                    int shift = random.NextDouble() < 0.5 ? 1 : -1;
                    int newNote = motif[i] + shift;
                    if (newNote >= 60 && newNote <= 83)
                    {
                        motif[i] = newNote;
                    }
                }
            }

            double probabilitySwap = 0.1;
            if (motif.Count > 1 && random.NextDouble() < probabilitySwap)
            {
                int idx1 = random.Next(motif.Count);
                int idx2 = random.Next(motif.Count);
                int temp = motif[idx1];
                motif[idx1] = motif[idx2];
                motif[idx2] = temp;
            }

            return motif;
        }

        private List<List<int>> GenerateRepeatedMeasures(List<List<int>> uniqueMeasures, int repeatsNumber)
        {
            var repeatedMeasures = new List<List<int>>();
            int uniqueCount = uniqueMeasures.Count;

            for (int i = 0; i < repeatsNumber; i++)
            {
                if (uniqueCount == 0)
                    break;

                int randomIndex = random.Next(uniqueCount);
                var measureToRepeat = uniqueMeasures[randomIndex];
                repeatedMeasures.Add(new List<int>(measureToRepeat));
            }

            return repeatedMeasures;
        }

        private List<List<int>> CombineMeasures(List<List<int>> uniqueMeasures, List<List<int>> repeatedMeasures, int totalMeasures)
        {
            var finalMelody = new List<List<int>>(uniqueMeasures);

            foreach (var repeat in repeatedMeasures)
            {
                int insertPosition = random.Next(0, finalMelody.Count + 1);
                finalMelody.Insert(insertPosition, repeat);
            }

            if (finalMelody.Count != totalMeasures)
            {
                throw new Exception($"Количество тактов после генерации ({finalMelody.Count}) не соответствует ожидаемому ({totalMeasures}).");
            }

            return finalMelody;
        }

        private List<int> GenerateMeasure(List<int> motif, int totalNotes)
        {
            var measure = new List<int>();
            int motifCount = totalNotes / motif.Count;
            int remainder = totalNotes % motif.Count;

            for (int i = 0; i < motifCount; i++)
            {
                measure.AddRange(motif);
            }

            if (remainder > 0)
            {
                measure.AddRange(motif.Take(remainder));
            }

            return measure;
        }

        private string SaveMelodyToMidi(List<List<int>> melody, string tonica, string scale, int arpNotesCount)
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string generatedDirectory = Path.Combine(appDirectory, "Generated");
            if (!Directory.Exists(generatedDirectory))
            {
                Directory.CreateDirectory(generatedDirectory);
            }

            string notesShort = string.Join("_", melody.SelectMany(m => m).Take(4).Select(n => MidiNumberToNoteName(n))).Replace("#", "s");
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                notesShort = notesShort.Replace(c, '_');
            }

            string fileName = $"{tonica}_{scale}_{notesShort}.mid";
            string fullPath = Path.Combine(generatedDirectory, fileName);

            var midiFile = new MidiEventCollection(1, 480);
            int bpm = 140;
            int microsecondsPerQuarterNote = 60000000 / bpm;
            midiFile.AddEvent(new TempoEvent(microsecondsPerQuarterNote, 0), 0);

            int melodyChannel = 1;
            midiFile.AddEvent(new PatchChangeEvent(0, melodyChannel, 81), 0); // Lead 2 (sawtooth)

            int currentTime = 0;
            int velocity = 100;

            // 1/16 нота = 120 тиков (как в примере)
            int sixteenthNoteTicks = 120;
            // 1/64 нота = 1/4 от 1/16 = 30 тиков
            int sixtyFourthNoteTicks = sixteenthNoteTicks / 4;

            Random rnd = new Random();

            foreach (var measure in melody)
            {
                foreach (var note in measure)
                {
                    // Случайная длительность от 1/64 до 1/16
                    int randomDuration = rnd.Next(sixtyFourthNoteTicks, sixteenthNoteTicks + 1);

                    int startNote = currentTime; // Нота начинается на чётком 1/16
                    int endNote = startNote + randomDuration;

                    int noteToPlay = note;
                    if (noteToPlay < 60 || noteToPlay > 83)
                    {
                        noteToPlay = Clamp(noteToPlay, 60, 83);
                    }

                    // Note On
                    midiFile.AddEvent(new NoteOnEvent(startNote, melodyChannel, noteToPlay, velocity, 0), 0);
                    // Note Off
                    midiFile.AddEvent(new NoteEvent(endNote, melodyChannel, MidiCommandCode.NoteOff, noteToPlay, 0), 0);

                    // Сдвигаем currentTime не на длину ноты, а ровно на 1/16 для следующей ноты
                    currentTime += sixteenthNoteTicks;
                }
            }

            midiFile.AddEvent(new MetaEvent(MetaEventType.EndTrack, 0, currentTime), 0);
            MidiFile.Export(fullPath, midiFile);
            return fullPath;
        }

        private string MidiNumberToNoteName(int midiNumber)
        {
            string[] notesSharp = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
            int octave = (midiNumber / 12) - 1;
            int noteIndex = midiNumber % 12;
            return $"{notesSharp[noteIndex]}{octave}";
        }

        private List<int> GetScaleNotes(string tonica, string scaleName)
        {
            var scaleIntervals = GetScaleIntervals(scaleName);
            if (scaleIntervals == null)
            {
                throw new Exception("Гамма не содержит нот.");
            }

            string baseNoteName = tonica.EndsWith("m") ? tonica.Substring(0, tonica.Length - 1) : tonica;
            if (!noteOffsets.ContainsKey(baseNoteName))
            {
                throw new Exception($"Нота '{baseNoteName}' не распознана.");
            }

            int noteOffsetValue = noteOffsets[baseNoteName];
            var scaleNotes = new List<int>();

            for (int octave = 4; octave <= 5; octave++)
            {
                int baseMidi = 12 * octave + noteOffsetValue;
                foreach (var interval in scaleIntervals)
                {
                    int note = baseMidi + interval;
                    if (note >= 60 && note <= 83)
                    {
                        scaleNotes.Add(note);
                    }
                }
            }

            return scaleNotes.OrderBy(n => n).ToList();
        }

        private int[] GetScaleIntervals(string scaleName)
        {
            if (scalesIntervals.ContainsKey(scaleName))
            {
                return scalesIntervals[scaleName];
            }
            else
            {
                throw new Exception($"Интервалы для гаммы '{scaleName}' не найдены.");
            }
        }

        private void BtnPlayMidi_Click(object sender, EventArgs e)
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

        private int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}
