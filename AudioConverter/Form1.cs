using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Lame;
using NAudio.Wave;

namespace WAVConverter
{
    public partial class Form1 : Form
    {
        private List<string> inputFilePaths = new List<string>();

        public Form1()
        {
            InitializeComponent();
            comboFormat.SelectedIndex = 0; // Например, выбираем второй элемент (индекс 1)

        }

        // Открытие файлов WAV/MP3/AIFF
        private void openWAVFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Audio Files (*.wav;*.mp3;*.aiff)|*.wav;*.mp3;*.aiff|All Files (*.*)|*.*";
                openFileDialog.Multiselect = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    inputFilePaths.AddRange(openFileDialog.FileNames);
                    foreach (var file in openFileDialog.FileNames)
                    {
                        string fileName = Path.GetFileName(file);
                        txtLog.AppendText($"Добавлен файл: {fileName}{Environment.NewLine}");

                        // Запись в лог-файл
                        string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");
                        File.AppendAllText(logPath, $"Добавлен файл: {fileName}{Environment.NewLine}");
                    }
                }
            }
        }

        // Конвертация файлов
        private async void convertFiles_Click(object sender, EventArgs e)
        {
            if (inputFilePaths.Count == 0)
            {
                MessageBox.Show("Пожалуйста, добавьте хотя бы один аудиофайл.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string outputFolder = folderDialog.SelectedPath;

                    ToggleControls(false);

                    try
                    {
                        foreach (var inputFilePath in inputFilePaths)
                        {
                            string outputExtension = GetSelectedOutputFormat();
                            string outputFileName = Path.ChangeExtension(Path.GetFileName(inputFilePath), outputExtension);
                            string outputPath = Path.Combine(outputFolder, outputFileName);

                            txtLog.AppendText($"Начинается обработка: {Path.GetFileName(inputFilePath)}{Environment.NewLine}");

                            await Task.Run(() => ConvertAudio(inputFilePath, outputPath, outputExtension));

                            txtLog.AppendText($"Файл успешно сохранен: {outputFileName}{Environment.NewLine}");
                        }

                        MessageBox.Show("Конвертация завершена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        ToggleControls(true);
                        inputFilePaths.Clear();
                    }
                }
            }
        }

        // Метод для конвертации файлов
        private void ConvertAudio(string inputFilePath, string outputPath, string outputFormat)
        {
            using (var reader = CreateAudioReader(inputFilePath))
            {
                if (outputFormat == ".mp3")
                {
                    using (var resampler = new MediaFoundationResampler(reader, new WaveFormat(44100, 16, 2)))
                    {
                        resampler.ResamplerQuality = 60;
                        using (var writer = new LameMP3FileWriter(outputPath, resampler.WaveFormat, 320))
                        {
                            byte[] buffer = new byte[8192];
                            int bytesRead;
                            while ((bytesRead = resampler.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                writer.Write(buffer, 0, bytesRead);
                            }
                        }
                    }
                }
                else if (outputFormat == ".wav")
                {
                    WaveFileWriter.CreateWaveFile(outputPath, reader);
                }
                else if (outputFormat == ".aiff")
                {
                    using (var resampler = new MediaFoundationResampler(reader, new WaveFormat(44100, 16, 2)))
                    {
                        resampler.ResamplerQuality = 60;
                        using (var writer = new AiffFileWriter(outputPath, resampler.WaveFormat))
                        {
                            byte[] buffer = new byte[8192];
                            int bytesRead;
                            while ((bytesRead = resampler.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                writer.Write(buffer, 0, bytesRead);
                            }
                        }
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Формат {outputFormat} не поддерживается.");
                }
            }
        }

        // Создание читателя для различных аудиоформатов
        private WaveStream CreateAudioReader(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            switch (extension)
            {
                case ".wav":
                    return new WaveFileReader(filePath);
                case ".mp3":
                    return new Mp3FileReader(filePath);
                case ".aiff":
                    return new AiffFileReader(filePath);
                default:
                    throw new InvalidOperationException("Формат файла не поддерживается.");
            }
        }

        // Метод для получения выбранного формата
        private string GetSelectedOutputFormat()
        {
            if (comboFormat.SelectedIndex == 0) // Если выбран дефолтный текст
            {
                throw new InvalidOperationException("Пожалуйста, выберите формат для конвертации.");
            }

            switch (comboFormat.SelectedItem?.ToString())
            {
                case "Конвертировать в MP3":
                    return ".mp3";
                case "Конвертировать в AIFF":
                    return ".aiff";
                case "Конвертировать в WAV":
                    return ".wav";
                default:
                    throw new InvalidOperationException("Формат не поддерживается.");
            }
        }
        // Переключение активности кнопок
        private void ToggleControls(bool enable)
        {
            openWAVFile.Enabled = enable;
            convertToMP3.Enabled = enable;
        }
    }
}
