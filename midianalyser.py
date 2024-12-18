from mido import MidiFile
import pandas as pd
import os

# Укажите путь к папке с MIDI-файлами
midi_directory = "C:/Users/Admin/Desktop/AudioTools/MIDI_FILES"

# Найдите все файлы с расширением .mid или .midi
midi_files = [os.path.join(midi_directory, file) for file in os.listdir(midi_directory) if file.endswith(('.mid', '.midi'))]

# Файл для записи логов
log_file = "midi_analysis_log.txt"

# Очищаем содержимое файла лога и записываем заголовок
with open(log_file, 'w', encoding='utf-8') as log:
    log.write("MIDI Analysis Log\n")
    log.write("=" * 40 + "\n\n")

# Функция для преобразования MIDI-номера ноты в название ноты
def note_number_to_name(note_number):
    names = ['C', 'C#', 'D', 'D#', 'E', 'F', 
             'F#', 'G', 'G#', 'A', 'A#', 'B']
    octave = (note_number // 12) - 1
    note = names[note_number % 12]
    return f"{note}{octave}"

# Функция для анализа одного MIDI-файла
def analyze_midi_file(file_path):
    try:
        midi = MidiFile(file_path)
        note_names = set()  # Используем set для уникальных названий нот

        for track in midi.tracks:
            for msg in track:
                if msg.type == 'note_on' and msg.velocity > 0:
                    note_name = note_number_to_name(msg.note)
                    note_names.add(note_name)
        
        # Преобразуем set в отсортированный список
        sorted_notes = sorted(note_names, key=lambda x: (int(x[-1]), x[:-1]))
        
        # Записываем в лог только название файла и список нот
        with open(log_file, 'a', encoding='utf-8') as log:
            file_name = os.path.basename(file_path)
            notes_str = ', '.join(sorted_notes)
            log.write(f"{file_name}: {notes_str}\n")
        
        return {
            'file_name': file_name,
            'notes': sorted_notes
        }
    except Exception as e:
        with open(log_file, 'a', encoding='utf-8') as log:
            file_name = os.path.basename(file_path)
            log.write(f"{file_name}: Error - {str(e)}\n")
        return {
            'file_name': file_name,
            'error': str(e)
        }

# Анализируем все MIDI-файлы
if midi_files:
    analysis_results = [analyze_midi_file(file) for file in midi_files]

    # Преобразуем результаты в DataFrame для лучшей читаемости (опционально)
    df_results = pd.DataFrame(analysis_results)
    print(df_results)
else:
    print("В указанной директории не найдено MIDI-файлов.")
