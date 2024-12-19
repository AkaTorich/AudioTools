import os
import shutil

# Укажите путь к папке, в которой находятся файлы
source_directory = '.'  # Текущая папка
libraries_directory = os.path.join(source_directory, 'Libraries')

# Создаём папку Libraries, если её ещё нет
os.makedirs(libraries_directory, exist_ok=True)

# Проходимся по всем файлам в директории
for filename in os.listdir(source_directory):
    filepath = os.path.join(source_directory, filename)

    # Проверяем, что это файл, и его расширение не .exe
    if os.path.isfile(filepath) and not filename.lower().endswith('.exe'):
        destination_path = os.path.join(libraries_directory, filename)
        try:
            if os.path.exists(destination_path):
                # Копируем файл с заменой, если он существует в целевой папке
                shutil.copy2(filepath, destination_path)
                print(f"Файл {filename} уже существовал. Скопирован с заменой.")
            else:
                # Перемещаем файл, если его нет в целевой папке
                shutil.move(filepath, destination_path)
                print(f"Файл {filename} перемещён.")
        except Exception as e:
            print(f"Ошибка при обработке {filename}: {e}")

# Копируем файл ../../database.db в папку Libraries с заменой
database_path = os.path.abspath(os.path.join(source_directory, '../../database.db'))
destination_path = os.path.join(libraries_directory, 'database.db')
if os.path.exists(database_path):
    try:
        shutil.copy2(database_path, destination_path)
        print("Файл database.db скопирован в папку Libraries.")
    except Exception as e:
        print(f"Ошибка при копировании database.db: {e}")
else:
    print("Файл database.db не найден.")

print("Обработка файлов завершена.")
