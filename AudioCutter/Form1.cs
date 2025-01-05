using System;
using System.IO;
using System.Windows.Forms;
using NAudio.Wave;
using NAudio.Lame; // Для MP3 кодирования (требуется установка NAudio.Lame через NuGet)
using NAudio.Wave.SampleProviders; // Для OffsetSampleProvider

namespace AudioCutter
{
    public partial class Form1 : Form
    {
        private string inputFilePath;
        private AudioFileReader audioFileReader;
        private TimeSpan totalTime;

        public Form1()
        {
            InitializeComponent();
            cmbFormat.Items.AddRange(new string[] { "WAV", "MP3", "AIFF" });
            cmbFormat.SelectedIndex = 0;

            // Подключение обработчиков событий ValueChanged
            trackBarStart.ValueChanged += trackBarStart_ValueChanged;
            trackBarEnd.ValueChanged += trackBarEnd_ValueChanged;
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Audio Files|*.wav;*.mp3;*.aiff";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    inputFilePath = ofd.FileName;
                    audioFileReader = new AudioFileReader(inputFilePath);
                    totalTime = audioFileReader.TotalTime;

                    trackBarStart.Minimum = 0;
                    trackBarStart.Maximum = (int)Math.Floor(totalTime.TotalSeconds);
                    trackBarStart.Value = 0;
                    lblStart.Text = "Начало: 0 сек";

                    trackBarEnd.Minimum = 0;
                    trackBarEnd.Maximum = (int)Math.Floor(totalTime.TotalSeconds);
                    trackBarEnd.Value = (int)Math.Floor(totalTime.TotalSeconds);
                    lblEnd.Text = $"Конец: {trackBarEnd.Value} сек";

                    // Дополнительная метка для общей длительности (опционально)
                    lblTotalTime.Text = $"Общая длительность: {totalTime.Minutes} мин {totalTime.Seconds} сек";

                    // Обновление метки длительности вырезки
                    UpdateCutDuration();
                }
            }
        }

        private void trackBarStart_ValueChanged(object sender, EventArgs e)
        {
            // Обеспечиваем, что начало не превышает конец
            if (trackBarStart.Value >= trackBarEnd.Value)
            {
                if (trackBarStart.Value + 1 <= trackBarEnd.Maximum)
                    trackBarEnd.Value = trackBarStart.Value + 1;
                else
                    trackBarEnd.Value = trackBarStart.Value;
                lblEnd.Text = $"Конец: {trackBarEnd.Value} сек";
            }
            lblStart.Text = $"Начало: {trackBarStart.Value} сек";

            // Обновляем длительность вырезки
            UpdateCutDuration();
        }

        private void trackBarEnd_ValueChanged(object sender, EventArgs e)
        {
            // Обеспечиваем, что конец не меньше начала
            if (trackBarEnd.Value <= trackBarStart.Value)
            {
                if (trackBarEnd.Value - 1 >= trackBarStart.Minimum)
                    trackBarStart.Value = trackBarEnd.Value - 1;
                else
                    trackBarStart.Value = trackBarEnd.Value;
                lblStart.Text = $"Начало: {trackBarStart.Value} сек";
            }
            lblEnd.Text = $"Конец: {trackBarEnd.Value} сек";

            // Обновляем длительность вырезки
            UpdateCutDuration();
        }

        /// <summary>
        /// Обновляет метку lblCutDuration с текущей длительностью выбранного отрезка.
        /// </summary>
        private void UpdateCutDuration()
        {
            int durationSeconds = trackBarEnd.Value - trackBarStart.Value;
            lblCutDuration.Text = $"Длительность: {durationSeconds} сек";
        }

        private void btnCut_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(inputFilePath))
            {
                MessageBox.Show("Пожалуйста, загрузите аудиофайл.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int startSeconds = trackBarStart.Value;
            int endSeconds = trackBarEnd.Value;

            if (endSeconds <= startSeconds)
            {
                MessageBox.Show("Конец должен быть позже начала.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                string selectedFormat = cmbFormat.SelectedItem.ToString();

                // Используем switch-case вместо switch expression
                switch (selectedFormat)
                {
                    case "WAV":
                        sfd.Filter = "WAV Files|*.wav";
                        break;
                    case "MP3":
                        sfd.Filter = "MP3 Files|*.mp3";
                        break;
                    case "AIFF":
                        sfd.Filter = "AIFF Files|*.aiff";
                        break;
                    default:
                        sfd.Filter = "All Files|*.*";
                        break;
                }

                sfd.FileName = Path.GetFileNameWithoutExtension(inputFilePath) + "_cut." + selectedFormat.ToLower();

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var startTime = TimeSpan.FromSeconds(startSeconds);
                        var duration = TimeSpan.FromSeconds(endSeconds - startSeconds);

                        string outputPath = sfd.FileName;

                        switch (selectedFormat)
                        {
                            case "WAV":
                                // Используем CreateWaveFile16 напрямую без вложенного using (WaveFileWriter)
                                var cutterWav = new OffsetSampleProvider(audioFileReader)
                                {
                                    SkipOver = startTime,
                                    Take = duration
                                };
                                WaveFileWriter.CreateWaveFile16(outputPath, cutterWav);
                                break;

                            case "MP3":
                                using (var reader = new AudioFileReader(inputFilePath))
                                {
                                    // Удаляем строку, устанавливающую CurrentTime, чтобы не конфликтовать с OffsetSampleProvider
                                    // reader.CurrentTime = startTime;

                                    var cutter = new OffsetSampleProvider(reader)
                                    {
                                        SkipOver = startTime,
                                        Take = duration
                                    };
                                    var waveProvider = cutter.ToWaveProvider16();

                                    using (var writer = new LameMP3FileWriter(outputPath, waveProvider.WaveFormat, 128))
                                    {
                                        byte[] buffer = new byte[waveProvider.WaveFormat.AverageBytesPerSecond];
                                        int bytesRead;
                                        while ((bytesRead = waveProvider.Read(buffer, 0, buffer.Length)) > 0)
                                        {
                                            writer.Write(buffer, 0, bytesRead);
                                        }
                                    }
                                }
                                break;

                            case "AIFF":
                                // Исправление процесса записи AIFF файла
                                var cutterAiff = new OffsetSampleProvider(audioFileReader)
                                {
                                    SkipOver = startTime,
                                    Take = duration
                                };
                                var waveProviderAiff = cutterAiff.ToWaveProvider16();

                                using (var aiffWriter = new AiffFileWriter(outputPath, waveProviderAiff.WaveFormat))
                                {
                                    byte[] bufferAiff = new byte[waveProviderAiff.WaveFormat.AverageBytesPerSecond];
                                    int bytesReadAiff;
                                    while ((bytesReadAiff = waveProviderAiff.Read(bufferAiff, 0, bufferAiff.Length)) > 0)
                                    {
                                        aiffWriter.Write(bufferAiff, 0, bytesReadAiff);
                                    }
                                }
                                break;
                        }

                        MessageBox.Show("Файл успешно сохранён!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
