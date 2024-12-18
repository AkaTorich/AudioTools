using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace ImageResizer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // Обработчик кнопки "Выбрать исходную папку"
        private void btnSelectSource_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Выберите исходную папку с изображениями";
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    txtSource.Text = fbd.SelectedPath;
                }
            }
        }

        // Обработчик кнопки "Выбрать целевую папку"
        private void btnSelectTarget_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Выберите целевую папку для сохранения изображений";
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    txtTarget.Text = fbd.SelectedPath;
                }
            }
        }

        // Обработчик кнопки "Начать обработку"
        private void btnStartProcessing_Click(object sender, EventArgs e)
        {
            string sourceDirectory = txtSource.Text;
            string targetDirectory = txtTarget.Text;

            if (string.IsNullOrEmpty(sourceDirectory) || !Directory.Exists(sourceDirectory))
            {
                MessageBox.Show("Пожалуйста, выберите корректную исходную папку.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(targetDirectory))
            {
                MessageBox.Show("Пожалуйста, выберите целевую папку.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Получаем и проверяем значения ширины и высоты из текстовых полей
            if (!int.TryParse(txtWidth.Text, out int maxWidth) || maxWidth <= 0)
            {
                MessageBox.Show("Пожалуйста, введите корректную ширину изображения (положительное целое число).", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(txtHeight.Text, out int maxHeight) || maxHeight <= 0)
            {
                MessageBox.Show("Пожалуйста, введите корректную высоту изображения (положительное целое число).", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Отключаем кнопку, чтобы предотвратить повторные нажатия
            btnStartProcessing.Enabled = false;

            // Запускаем обработку в отдельном потоке, чтобы не блокировать UI
            System.Threading.Tasks.Task.Run(() => ProcessImages(sourceDirectory, targetDirectory, maxWidth, maxHeight))
                .ContinueWith(t =>
                {
                    // Включаем кнопку после завершения
                    this.Invoke(new Action(() =>
                    {
                        btnStartProcessing.Enabled = true;
                        if (t.Exception != null)
                        {
                            MessageBox.Show($"Произошла ошибка: {t.Exception.InnerException.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            MessageBox.Show("Обработка завершена.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }));
                });
        }

        private void ProcessImages(string sourceDirectory, string targetDirectory, int maxWidth, int maxHeight)
        {
            // Получаем текст водяного знака из TextBox
            string watermarkText = txtWatermark.InvokeRequired
                ? (string)txtWatermark.Invoke(new Func<string>(() => txtWatermark.Text))
                : txtWatermark.Text;

            if (string.IsNullOrEmpty(watermarkText))
            {
                watermarkText = "PsyShout @ Copyrights"; // Значение по умолчанию
            }

            AppendLog("Начинаем обработку изображений...");

            // Поддерживаемые форматы изображений
            string[] supportedExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };

            // Получаем все подкаталоги в исходной папке
            var directories = Directory.GetDirectories(sourceDirectory, "*", SearchOption.AllDirectories);

            // Добавляем саму исходную папку в список для обработки
            var allDirectories = new System.Collections.Generic.List<string>(directories);
            allDirectories.Insert(0, sourceDirectory);

            int totalImages = 0;
            int processedImages = 0;
            int skippedImages = 0;

            foreach (var dir in allDirectories)
            {
                // Получаем относительный путь для сохранения структуры
                string relativePath = GetRelativePath(sourceDirectory, dir);
                string targetSubDir = Path.Combine(targetDirectory, relativePath);

                // Создаем подкаталог в целевой папке, если его нет
                if (!Directory.Exists(targetSubDir))
                {
                    Directory.CreateDirectory(targetSubDir);
                    AppendLog($"Создан подкаталог: {targetSubDir}");
                }

                // Получаем все файлы в текущем подкаталоге
                var files = Directory.GetFiles(dir);
                foreach (var file in files)
                {
                    string extension = Path.GetExtension(file).ToLower();

                    if (Array.IndexOf(supportedExtensions, extension) < 0)
                    {
                        skippedImages++;
                        AppendLog($"Пропущено (неподдерживаемый формат): {file}");
                        continue; // Пропускаем неподдерживаемые форматы
                    }

                    totalImages++;
                    string fileName = Path.GetFileName(file);
                    string targetFilePath = Path.Combine(targetSubDir, fileName);

                    try
                    {
                        using (Image image = Image.FromFile(file))
                        {
                            // Изменяем размер изображения с использованием заданных размеров
                            using (Image resizedImage = ResizeImage(image, maxWidth, maxHeight))
                            {
                                // Добавляем водяной знак
                                using (Image watermarkedImage = AddWatermark(resizedImage, watermarkText))
                                {
                                    // Сохраняем изображение в целевой папке
                                    watermarkedImage.Save(targetFilePath, GetImageFormat(extension));
                                    processedImages++;
                                    AppendLog($"Обработано: {Path.Combine(relativePath, fileName)}");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        skippedImages++;
                        AppendLog($"Ошибка при обработке файла {file}: {ex.Message}");
                    }
                }
            }

            AppendLog("=== Завершено ===");
            AppendLog($"Всего изображений: {totalImages}");
            AppendLog($"Обработано: {processedImages}");
            AppendLog($"Пропущено: {skippedImages}");
        }

        /// <summary>
        /// Изменяет размер изображения до указанных размеров, сохраняя пропорции.
        /// </summary>
        /// <param name="image">Исходное изображение.</param>
        /// <param name="maxWidth">Максимальная ширина.</param>
        /// <param name="maxHeight">Максимальная высота.</param>
        /// <returns>Изменённое изображение.</returns>
        private static Image ResizeImage(Image image, int maxWidth, int maxHeight)
        {
            // Вычисляем соотношение сторон
            double ratioX = (double)maxWidth / image.Width;
            double ratioY = (double)maxHeight / image.Height;
            double ratio = Math.Min(ratioX, ratioY);

            // Вычисляем новые размеры
            int newWidth = (int)(image.Width * ratio);
            int newHeight = (int)(image.Height * ratio);

            // Создаём новое изображение с новыми размерами
            Bitmap resizedImage = new Bitmap(newWidth, newHeight);
            using (Graphics graphics = Graphics.FromImage(resizedImage))
            {
                // Настройки качества
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                // Рисуем исходное изображение в новом размере
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return resizedImage;
        }

        /// <summary>
        /// Добавляет текстовый водяной знак на изображение в центр.
        /// </summary>
        /// <param name="image">Исходное изображение.</param>
        /// <param name="watermarkText">Текст водяного знака.</param>
        /// <returns>Изображение с водяным знаком.</returns>
        private static Image AddWatermark(Image image, string watermarkText)
        {
            Bitmap bmp = new Bitmap(image.Width, image.Height);
            using (Graphics graphics = Graphics.FromImage(bmp))
            {
                graphics.DrawImage(image, 0, 0, image.Width, image.Height);

                // Настройки водяного знака
                // Выбираем шрифт и размер (увеличиваем размер)
                float fontSize = image.Width / 15f; // Размер шрифта зависит от ширины изображения
                Font font = new Font("Arial", fontSize, FontStyle.Bold, GraphicsUnit.Pixel);

                // Вычисляем размер текста
                SizeF textSize = graphics.MeasureString(watermarkText, font);

                // Позиция водяного знака (центр)
                float xPosition = (image.Width - textSize.Width) / 2;
                float yPosition = (image.Height - textSize.Height) / 2;

                // Полупрозрачный белый цвет
                SolidBrush brush = new SolidBrush(Color.FromArgb(128, 255, 255, 255));

                // Добавляем тень для лучшей читаемости (опционально)
                graphics.DrawString(watermarkText, font, Brushes.Black, xPosition + 2, yPosition + 2);

                // Рисуем сам водяной знак
                graphics.DrawString(watermarkText, font, brush, xPosition, yPosition);
            }
            return bmp;
        }

        /// <summary>
        /// Получает соответствующий формат изображения по расширению файла.
        /// </summary>
        /// <param name="extension">Расширение файла.</param>
        /// <returns>Формат изображения.</returns>
        private static ImageFormat GetImageFormat(string extension)
        {
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    return ImageFormat.Jpeg;
                case ".png":
                    return ImageFormat.Png;
                case ".bmp":
                    return ImageFormat.Bmp;
                case ".gif":
                    return ImageFormat.Gif;
                default:
                    return ImageFormat.Png; // По умолчанию PNG
            }
        }

        /// <summary>
        /// Получает относительный путь между двумя абсолютными путями.
        /// </summary>
        /// <param name="basePath">Базовый путь.</param>
        /// <param name="fullPath">Полный путь.</param>
        /// <returns>Относительный путь.</returns>
        private static string GetRelativePath(string basePath, string fullPath)
        {
            // Добавляем символ разделителя, если его нет
            if (!basePath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                basePath += Path.DirectorySeparatorChar;
            }

            Uri baseUri = new Uri(basePath);
            Uri fullUri = new Uri(fullPath);

            Uri relativeUri = baseUri.MakeRelativeUri(fullUri);
            string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            return relativePath.Replace('/', Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Добавляет символ разделителя каталога к пути, если его нет.
        /// </summary>
        /// <param name="path">Путь.</param>
        /// <returns>Путь с символом разделителя.</returns>
        private static string AppendDirectorySeparatorChar(string path)
        {
            if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                return path + Path.DirectorySeparatorChar;
            }
            return path;
        }

        /// <summary>
        /// Добавляет текст в лог, безопасно из любого потока.
        /// </summary>
        /// <param name="message">Сообщение для добавления.</param>
        private void AppendLog(string message)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action<string>(AppendLog), message);
            }
            else
            {
                txtLog.AppendText($"{DateTime.Now}: {message}{Environment.NewLine}");
            }
        }
    }
}
