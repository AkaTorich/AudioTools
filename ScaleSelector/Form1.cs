using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq; // Для сортировки
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ScaleSelector
{
    public partial class ScaleSelectorForm : Form
    {
        // Параметры горячих клавиш
        private const int HOTKEY_ID_PLUS = 1;
        private const int HOTKEY_ID_F1 = 2;
        private const uint MOD_NOREPEAT = 0x4000;
        private const uint WM_HOTKEY = 0x0312;

        // Данные
        private readonly List<string> chromaticScale = new List<string>
        {
            "C", "C#", "D", "D#", "E", "F",
            "F#", "G", "G#", "A", "A#", "B"
        };

        private readonly List<string> tonics = new List<string>
        {
            "C", "C#", "D", "D#", "E", "F",
            "F#", "G", "G#", "A", "A#", "B"
        };

        private readonly Dictionary<string, List<int>> bigScalePatterns = new Dictionary<string, List<int>>
        {
            // Классические/Западные и Джазовые
            {"Мажор (Ionian)", new List<int> {2, 2, 1, 2, 2, 2, 1}},
            {"Натуральный минор (Aeolian)", new List<int> {2, 1, 2, 2, 1, 2, 2}},
            {"Гармонический минор", new List<int> {2, 1, 2, 2, 1, 3, 1}},
            {"Мелодический минор (Asc.)", new List<int> {2, 1, 2, 2, 2, 2, 1}},
            {"Дорийский (Dorian)", new List<int> {2, 1, 2, 2, 2, 1, 2}},
            {"Фригийский (Phrygian)", new List<int> {1, 2, 2, 2, 1, 2, 2}},
            {"Лидийский (Lydian)", new List<int> {2, 2, 2, 1, 2, 2, 1}},
            {"Миксолидийский (Mixolyd.)", new List<int> {2, 2, 1, 2, 2, 1, 2}},
            {"Локрийский (Locrian)", new List<int> {1, 2, 2, 1, 2, 2, 2}},
            // Пентатоника и Блюз
            {"Мажорная пентатоника", new List<int> {2, 2, 3, 2, 3}},
            {"Минорная пентатоника", new List<int> {3, 2, 2, 3, 2}},
            {"Блюзовая (Blues)", new List<int> {3, 2, 1, 1, 3, 2}},
            // Бибоп
            {"Мажорный бибоп", new List<int> {2, 2, 1, 2, 2, 1, 1, 2}},
            {"Минорный бибоп", new List<int> {2, 1, 2, 2, 1, 1, 2, 2}},
            {"Доминиантный бибоп", new List<int> {2, 2, 1, 2, 1, 1, 2, 2}},
            // Неаполитанские
            {"Неаполитанский мажор", new List<int> {1, 2, 2, 2, 2, 2, 1}},
            {"Неаполитанский минор", new List<int> {1, 2, 2, 2, 1, 2, 2}},
            // Целотонный, Хроматический
            {"Целотонный (Whole Tone)", new List<int> {2, 2, 2, 2, 2, 2}},
            {"Хроматический (12 Half)", new List<int> {1,1,1,1,1,1,1,1,1,1,1,1}},
            // Двойной гармонический, Венгерский
            {"Двойной гармонический (Double Harm.)", new List<int> {1, 3, 1, 2, 1, 3, 1}},
            {"Венгерский минор (Hungarian Minor)", new List<int> {2, 1, 3, 1, 1, 3, 1}},
            {"Венгерский мажор (Hungarian Major)", new List<int> {3, 1, 2, 1, 2, 1, 2}},
            // Экзотические (арабские, индийские…)
            {"Арабский (Bhairav)", new List<int> {1, 3, 1, 2, 1, 2, 2}},
            {"Индийская (Todi)", new List<int> {1, 2, 2, 2, 2, 2, 1}},
            {"Египетская (Egyptian)", new List<int> {2, 1, 2, 2, 2, 2, 3}},
            {"Персидская (Persian)", new List<int> {1, 3, 1, 2, 1, 2, 2}},
            {"Арабская (Hijaz)", new List<int> {1, 3, 1, 2, 1, 3, 1}},
            {"Суфийская (Sufi)", new List<int> {1, 3, 1, 2, 1, 2, 2}},
            // Жанровые (фиктивные)
            {"Транс (Trance)", new List<int> {3, 1, 2, 2, 2, 1, 1}},
            {"Пситранс (PsyTrance)", new List<int> {3, 2, 1, 2, 1, 2, 1}},
            {"Синематик (Cinematic)", new List<int> {2, 2, 1, 3, 1, 2, 1}},
            {"Эмбиент (Ambient)", new List<int> {2, 1, 2, 1, 2, 2, 2}},
            {"Драм-н-бейс (DnB)", new List<int> {2, 2, 1, 2, 1, 2, 2}},
            {"Сайоми (Saiomy)", new List<int> {3, 1, 1, 3, 1, 2, 1}}
        };

        public ScaleSelectorForm()
        {
            InitializeComponent();
            InitializeTrayIcon();
            RegisterHotkeys();
            PopulateComboBoxes();

            // Дополнительно устанавливаем DropDownStyle программно (на всякий случай)
            comboBoxScale.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxTonic.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void PopulateComboBoxes()
        {
            // Сортируем гаммы по названию
            var sortedScales = bigScalePatterns.Keys.OrderBy(s => s).ToList();

            foreach (var scale in sortedScales)
            {
                comboBoxScale.Items.Add(scale);
            }
            if (comboBoxScale.Items.Count > 0)
                comboBoxScale.SelectedIndex = 0;

            // Сортируем тоники по названию (опционально)
            var sortedTonics = tonics.OrderBy(t => t).ToList();

            foreach (var tonic in sortedTonics)
            {
                comboBoxTonic.Items.Add(tonic);
            }
            if (comboBoxTonic.Items.Count > 0)
                comboBoxTonic.SelectedIndex = 0;
        }

        private void InitializeTrayIcon()
        {
            // Создание контекстного меню для трея
            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Открыть", null, TrayMenu_Open_Click);
            trayMenu.Items.Add("Скрыть", null, TrayMenu_Hide_Click);
            trayMenu.Items.Add("Выход", null, TrayMenu_Exit_Click);

            // Настройка иконки в трее
            trayIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application, // Замените на вашу иконку
                ContextMenuStrip = trayMenu,
                Text = "Выбор гаммы",
                Visible = true
            };
            trayIcon.DoubleClick += TrayIcon_DoubleClick;
        }

        private void RegisterHotkeys()
        {
            // Регистрируем горячую клавишу Numpad '+' (VK_ADD)
            RegisterHotKey(this.Handle, HOTKEY_ID_PLUS, MOD_NOREPEAT, (uint)Keys.Add);

            // Регистрируем горячую клавишу F1
            RegisterHotKey(this.Handle, HOTKEY_ID_F1, MOD_NOREPEAT, (uint)Keys.F1);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                int id = m.WParam.ToInt32();
                if (id == HOTKEY_ID_PLUS || id == HOTKEY_ID_F1)
                {
                    ToggleVisibility();
                }
            }
            base.WndProc(ref m);
        }

        private void ToggleVisibility()
        {
            if (this.Visible)
            {
                this.Hide();
            }
            else
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.BringToFront();
            }
        }

        private void ButtonShowNotes_Click(object sender, EventArgs e)
        {
            if (comboBoxScale.SelectedIndex == -1 || comboBoxTonic.SelectedIndex == -1)
            {
                MessageBox.Show("Пожалуйста, выберите гамму и тонику.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedScale = comboBoxScale.SelectedItem.ToString();
            string selectedTonic = comboBoxTonic.SelectedItem.ToString();

            if (bigScalePatterns.TryGetValue(selectedScale, out List<int> pattern))
            {
                List<string> notes = CalculateScale(selectedTonic, pattern);
                if (notes.Count > 0)
                {
                    richTextBoxNotes.Clear();
                    for (int i = 0; i < notes.Count; i++)
                    {
                        string note = notes[i] + " ";
                        richTextBoxNotes.SelectionStart = richTextBoxNotes.TextLength;
                        richTextBoxNotes.SelectionLength = 0;
                        richTextBoxNotes.SelectionColor = (i % 2 == 0) ? Color.Green : Color.Red;
                        richTextBoxNotes.AppendText(note);
                    }
                }
                else
                {
                    MessageBox.Show("Выбрана некорректная тоника.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Шаблон гаммы не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<string> CalculateScale(string tonic, List<int> pattern)
        {
            List<string> scaleNotes = new List<string>();
            int index = chromaticScale.IndexOf(tonic);
            if (index == -1)
                return scaleNotes;

            scaleNotes.Add(chromaticScale[index]);

            foreach (int interval in pattern)
            {
                index = (index + interval) % chromaticScale.Count;
                scaleNotes.Add(chromaticScale[index]);
            }

            return scaleNotes;
        }

        private void TrayIcon_DoubleClick(object sender, EventArgs e)
        {
            ToggleVisibility();
        }

        private void TrayMenu_Open_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.BringToFront();
        }

        private void TrayMenu_Hide_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void TrayMenu_Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ScaleSelectorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Скрываем форму вместо закрытия
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // Отмена регистрации горячих клавиш и удаление иконки из трея
            UnregisterHotKey(this.Handle, HOTKEY_ID_PLUS);
            UnregisterHotKey(this.Handle, HOTKEY_ID_F1);
            trayIcon.Visible = false;
            base.OnFormClosed(e);
        }

        // Импорт функций для работы с горячими клавишами
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    }
}
