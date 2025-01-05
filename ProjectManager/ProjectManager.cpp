

//
// ProjectManager.cpp
// - Рекурсивный поиск DAW-проектов в выбранной папке.
// - Отображение в ListView с системными иконками, сортировкой по дате и размером папки.
// - Контекстное меню: Открыть, Создать ZIP (файл), Открыть папку, Создать ZIP папки проекта.
// - Архивирование через PowerShell с использованием Compress-Archive.
//
#define WINVER       0x0601     // Windows 7
#define _WIN32_WINNT 0x0601     // Windows 7
#define _WIN32_IE    0x0800     // IE 8
#define _CRT_SECURE_NO_WARNINGS
#include <windows.h>
#include <commctrl.h>    // ListView, InitCommonControlsEx
#include <shellapi.h>    // ShellExecuteW
#include <shlobj.h>      // SHBrowseForFolder, BROWSEINFO
#include <shlwapi.h>     // PathFindExtensionW
#include <knownfolders.h> // FOLDERID_Desktop
#include <tchar.h>

#include <string>
#include <vector>
#include <map>
#include <filesystem>     // C++17
#include <algorithm>
#include <sstream>
#include <iomanip>        // Для форматирования размера

#pragma comment(lib, "comctl32.lib")
#pragma comment(lib, "shell32.lib")
#pragma comment(lib, "shlwapi.lib")

//-------------------------------------------------------------------
// Глобальные переменные
//-------------------------------------------------------------------
HINSTANCE g_hInst = nullptr;
HWND      g_hListView = nullptr;

// Папка, выбранная пользователем
std::wstring g_chosenFolder;

// Список найденных проектов (полные пути)
std::vector<std::wstring> g_Files;

// Индекс элемента, по которому кликнули правой кнопкой
int g_fileIndexOnContextClick = -1;

// Словарь: расширение -> название DAW
static const std::map<std::wstring, std::wstring> g_extToDaw = {
	{L".cpr",       L"Cubase"},
	{L".npr",       L"Nuendo"},
	{L".mon",       L"WaveLab"},
	{L".bwproject", L"Bitwig"},
	{L".flp",       L"FL Studio"},
	{L".flm",       L"FL Studio Mobile"},
	{L".rpp",       L"Reaper"},
	{L".ptx",       L"Pro Tools"},
	{L".ptf",       L"Pro Tools (old)"},
	{L".cwp",       L"Cakewalk / Sonar"},
	{L".sng",       L"Sonar (old)"},
	{L".song",      L"Sonar (old)"},
	{L".logicx",    L"Logic Pro"},
	{L".dpdoc",     L"Digital Performer"},
	{L".ardour",    L"Ardour"},
	{L".reason",    L"Reason"},
	{L".alp",       L"Ableton Live"},
	{L".als",       L"Ableton Live"},
	{L".ejay",      L"eJay"},
	{L".mmp",       L"LMMS"},
	{L".mmpz",      L"LMMS"},
	{L".studioone", L"Studio One"},
	{L".s1p",       L"Studio One"},
	{L".mas",       L"Maschine"},
	{L".mcproject", L"Mixcraft"},
	{L".mixbus",    L"Harrison Mixbus"},
	{L".sequoia",   L"Magix Sequoia"},
	{L".samplitude", L"Samplitude"},
	{L".waveform",  L"Tracktion Waveform"},
	{L".ntrk",      L"n-Track Studio"},
	{L".pod",       L"Podium by Zynewave"},
	// Добавьте другие расширения по необходимости
};

// ID пунктов контекстного меню
enum {
	ID_MENU_OPEN = 1001,
	ID_MENU_ZIP_FILE,
	ID_MENU_OPEN_FOLDER,
	ID_MENU_ZIP_FOLDER
};

//-------------------------------------------------------------------
// Получить путь к рабочему столу (Desktop) текущего пользователя
//-------------------------------------------------------------------
std::wstring GetDesktopPath()
{
	PWSTR path = nullptr;
	if (SUCCEEDED(SHGetKnownFolderPath(FOLDERID_Desktop, 0, nullptr, &path)))
	{
		std::wstring result(path);
		CoTaskMemFree(path);
		return result;
	}
	return L"";
}

//-------------------------------------------------------------------
// Определить название DAW по расширению
//-------------------------------------------------------------------
std::wstring GetDawName(const std::wstring& ext)
{
	std::wstring lower = ext;
	std::transform(lower.begin(), lower.end(), lower.begin(), ::towlower);

	auto it = g_extToDaw.find(lower);
	if (it != g_extToDaw.end())
		return it->second;

	return L"";
}

//-------------------------------------------------------------------
// Диалог выбора папки
//-------------------------------------------------------------------
std::wstring BrowseForFolder(HWND owner)
{
	BROWSEINFO bi = { 0 };
	bi.hwndOwner = owner;
	bi.lpszTitle = L"Выберите папку, где искать проекты";
	bi.ulFlags = BIF_RETURNONLYFSDIRS | BIF_NEWDIALOGSTYLE;

	LPITEMIDLIST pidl = SHBrowseForFolder(&bi);
	if (pidl)
	{
		WCHAR buffer[MAX_PATH];
		if (SHGetPathFromIDListW(pidl, buffer))
		{
			IMalloc* pMalloc = nullptr;
			if (SUCCEEDED(SHGetMalloc(&pMalloc)) && pMalloc)
			{
				pMalloc->Free(pidl);
				pMalloc->Release();
			}
			return buffer;
		}
	}
	return L""; // Нажали "Отмена"
}

//-------------------------------------------------------------------
// Проверка, является ли файл проектным
//-------------------------------------------------------------------
bool IsProjectFile(const std::wstring& path)
{
	std::wstring ext = PathFindExtensionW(path.c_str());
	std::transform(ext.begin(), ext.end(), ext.begin(), ::towlower);

	return (g_extToDaw.find(ext) != g_extToDaw.end());
}

//-------------------------------------------------------------------
// Рекурсивный сбор всех проектов
//-------------------------------------------------------------------
void CollectProjectFiles(const std::wstring& folder)
{
	namespace fs = std::filesystem;

	try
	{
		for (auto& entry : fs::recursive_directory_iterator(folder))
		{
			if (entry.is_regular_file())
			{
				auto fullpath = entry.path().wstring();
				if (IsProjectFile(fullpath))
				{
					g_Files.push_back(fullpath);
				}
			}
		}
	}
	catch (...)
	{
		// Игнорируем ошибки доступа
	}
}

//-------------------------------------------------------------------
// Функция для подсчёта размера папки
//-------------------------------------------------------------------
uintmax_t GetFolderSize(const std::wstring& folderPath)
{
	namespace fs = std::filesystem;
	uintmax_t size = 0;

	try
	{
		for (auto& p : fs::recursive_directory_iterator(folderPath, fs::directory_options::skip_permission_denied))
		{
			if (p.is_regular_file())
			{
				size += p.file_size();
			}
		}
	}
	catch (...)
	{
		// Игнорируем ошибки доступа
	}

	return size;
}

//-------------------------------------------------------------------
// Форматирование размера в удобочитаемый вид
//-------------------------------------------------------------------
std::wstring FormatSize(uintmax_t size)
{
	std::wstringstream ss;
	ss << std::fixed << std::setprecision(2);
	if (size < 1024)
		ss << size << L" B";
	else if (size < 1024 * 1024)
		ss << (size / 1024.0) << L" KB";
	else if (size < 1024 * 1024 * 1024)
		ss << (size / (1024.0 * 1024.0)) << L" MB";
	else
		ss << (size / (1024.0 * 1024.0 * 1024.0)) << L" GB";
	return ss.str();
}

//-------------------------------------------------------------------
// Сортируем g_Files по дате последнего изменения (от новых к старым)
//-------------------------------------------------------------------
void SortProjectFiles()
{
	namespace fs = std::filesystem;

	std::sort(g_Files.begin(), g_Files.end(),
		[](const std::wstring& a, const std::wstring& b) -> bool
		{
			try
			{
				auto timeA = fs::last_write_time(a);
				auto timeB = fs::last_write_time(b);
				return timeA > timeB; // Для сортировки от новых к старым
			}
			catch (...)
			{
				// В случае ошибки (например, файл не существует), сортируем по имени
				fs::path pa(a), pb(b);
				auto fa = pa.filename().wstring();
				auto fb = pb.filename().wstring();

				std::transform(fa.begin(), fa.end(), fa.begin(), ::towlower);
				std::transform(fb.begin(), fb.end(), fb.begin(), ::towlower);

				return fa < fb;
			}
		});
}

//-------------------------------------------------------------------
// Запуск PowerShell-скрипта синхронно и получение вывода
//-------------------------------------------------------------------
bool RunPowerShellCommand(const std::wstring& command, std::wstring& output)
{
	// Формируем командную строку: powershell.exe -NoProfile -Command "<command>"
	std::wstring cmd = L"powershell.exe -NoProfile -Command \"";
	cmd += command;
	cmd += L"\"";

	// Структуры для CreateProcess
	STARTUPINFOW si = { 0 };
	si.cb = sizeof(si);
	PROCESS_INFORMATION pi = { 0 };

	// Создаём pipe для захвата вывода
	HANDLE hReadPipe, hWritePipe;
	SECURITY_ATTRIBUTES sa = { sizeof(SECURITY_ATTRIBUTES), NULL, TRUE };
	if (!CreatePipe(&hReadPipe, &hWritePipe, &sa, 0))
	{
		MessageBoxW(NULL, L"Не удалось создать pipe для захвата вывода PowerShell.", L"Ошибка", MB_ICONERROR);
		return false;
	}

	si.dwFlags = STARTF_USESTDHANDLES;
	si.hStdOutput = hWritePipe;
	si.hStdError = hWritePipe;

	// Запускаем процесс
	BOOL ok = CreateProcessW(
		NULL,                                   // lpApplicationName
		const_cast<LPWSTR>(cmd.c_str()),        // lpCommandLine
		NULL, NULL, FALSE,
		CREATE_NEW_CONSOLE,                     // Создаём новое окно консоли для PowerShell
		NULL, NULL,
		&si, &pi
	);
	if (!ok)
	{
		// Ошибка создания процесса
		MessageBoxW(NULL, L"Не удалось запустить PowerShell.", L"Ошибка", MB_ICONERROR);
		CloseHandle(hWritePipe);
		CloseHandle(hReadPipe);
		return false;
	}

	CloseHandle(hWritePipe);

	// Читаем вывод
	WCHAR buffer[4096];
	DWORD bytesRead;
	while (ReadFile(hReadPipe, buffer, sizeof(buffer) / sizeof(WCHAR) - 1, &bytesRead, NULL) && bytesRead > 0)
	{
		buffer[bytesRead / sizeof(WCHAR)] = L'\0';
		output += buffer;
	}

	CloseHandle(hReadPipe);

	// Ждём завершения процесса
	WaitForSingleObject(pi.hProcess, INFINITE);

	// Получаем код возврата
	DWORD exitCode = 0;
	GetExitCodeProcess(pi.hProcess, &exitCode);

	// Закрываем хендлы
	CloseHandle(pi.hThread);
	CloseHandle(pi.hProcess);

	// Если код возврата 0, считаем успех
	bool success = (exitCode == 0);

	return success;
}

//-------------------------------------------------------------------
// Создать ZIP (отдельного файла) в той же папке через Compress-Archive
//-------------------------------------------------------------------
bool CreateZipArchiveFile(int index)
{
	if (index < 0 || index >= static_cast<int>(g_Files.size()))
		return false;

	// Путь к файлу
	const std::wstring& filePath = g_Files[index];
	std::filesystem::path p(filePath);
	std::wstring folderPath = p.parent_path().wstring();

	// Имя ZIP: "<filename_without_ext>.zip"
	std::wstring zipName = p.stem().wstring() + L".zip";
	std::wstring destZip = folderPath + L"\\" + zipName;

	// Проверяем, существует ли ZIP-архив
	if (std::filesystem::exists(destZip))
	{
		std::wstring msg = std::wstring(L"ZIP-архив уже существует: ") + destZip + L"\nПерезаписать?";
		if (MessageBoxW(NULL, msg.c_str(), L"Подтверждение", MB_YESNO | MB_ICONQUESTION) != IDYES)
			return false;
	}

	// Формируем команду Compress-Archive
	std::wstring command = L"Compress-Archive -Path '" + filePath + L"' -DestinationPath '" + destZip + L"' -CompressionLevel Optimal -Force";

	// Запускаем PowerShell-скрипт синхронно и получаем вывод
	std::wstring output;
	bool success = RunPowerShellCommand(command, output);

	if (success)
	{
		// Показываем вывод PowerShell
		std::wstring successMsg = std::wstring(L"ZIP-архив успешно создан в той же папке проекта.\n\nВывод PowerShell:\n") + output;
		MessageBoxW(NULL, successMsg.c_str(), L"Успех", MB_ICONINFORMATION);
	}
	else
	{
		// Показываем вывод PowerShell для диагностики
		std::wstring errorMsg = std::wstring(L"Ошибка при создании ZIP-архива.\n\nВывод PowerShell:\n") + output;
		MessageBoxW(NULL, errorMsg.c_str(), L"Ошибка", MB_ICONERROR);
	}

	return success;
}

//-------------------------------------------------------------------
// Создать ZIP папки проекта через Compress-Archive
//-------------------------------------------------------------------
bool CreateZipArchiveProjectFolder(int index)
{
	if (index < 0 || index >= static_cast<int>(g_Files.size()))
		return false;

	// Путь к файлу
	const std::wstring& filePath = g_Files[index];
	std::filesystem::path p(filePath);
	std::wstring projectFolder = p.parent_path().wstring();

	// Проверка существования папки
	if (!std::filesystem::exists(projectFolder))
	{
		std::wstring errorMsg = std::wstring(L"Папка проекта не найдена: ") + projectFolder;
		MessageBoxW(NULL, errorMsg.c_str(), L"Ошибка", MB_ICONERROR);
		return false;
	}

	// Имя ZIP: "<название_папки>.zip"
	std::filesystem::path folderPath(projectFolder);
	std::wstring folderName = folderPath.filename().wstring();
	if (folderName.empty()) folderName = L"MyProjectFolder";

	std::wstring destZip = projectFolder + L"\\" + folderName + L".zip";

	// Проверяем, существует ли ZIP-архив
	if (std::filesystem::exists(destZip))
	{
		std::wstring msg = std::wstring(L"ZIP-архив уже существует: ") + destZip + L"\nПерезаписать?";
		if (MessageBoxW(NULL, msg.c_str(), L"Подтверждение", MB_YESNO | MB_ICONQUESTION) != IDYES)
			return false;
	}

	// Формируем команду Compress-Archive
	std::wstring command = L"Compress-Archive -Path '" + projectFolder + L"' -DestinationPath '" + destZip + L"' -CompressionLevel Optimal -Force";

	// Запускаем PowerShell-скрипт синхронно и получаем вывод
	std::wstring output;
	bool success = RunPowerShellCommand(command, output);

	if (success)
	{
		// Показываем вывод PowerShell
		std::wstring successMsg = std::wstring(L"ZIP-архив папки проекта успешно создан в той же папке проекта.\n\nВывод PowerShell:\n") + output;
		MessageBoxW(NULL, successMsg.c_str(), L"Успех", MB_ICONINFORMATION);
	}
	else
	{
		// Показываем вывод PowerShell для диагностики
		std::wstring errorMsg = std::wstring(L"Ошибка при создании ZIP-архива папки проекта.\n\nВывод PowerShell:\n") + output;
		MessageBoxW(NULL, errorMsg.c_str(), L"Ошибка", MB_ICONERROR);
	}

	return success;
}

//-------------------------------------------------------------------
// Открыть проект (ShellExecute)
//-------------------------------------------------------------------
void OpenProject(int index)
{
	if (index < 0 || index >= static_cast<int>(g_Files.size()))
		return;

	ShellExecuteW(NULL, L"open", g_Files[index].c_str(), NULL, NULL, SW_SHOWNORMAL);
}

//-------------------------------------------------------------------
// Открыть папку проекта (Проводник)
//-------------------------------------------------------------------
void OpenProjectFolder(int index)
{
	if (index < 0 || index >= static_cast<int>(g_Files.size()))
		return;

	std::filesystem::path p(g_Files[index]);
	auto folder = p.parent_path().wstring();

	ShellExecuteW(NULL, L"open", folder.c_str(), NULL, NULL, SW_SHOWNORMAL);
}

//-------------------------------------------------------------------
// Показать контекстное меню по правому клику
//-------------------------------------------------------------------
void ShowContextMenu(HWND hwnd, POINT pt, int indexInFiles)
{
	HMENU hMenu = CreatePopupMenu();
	if (!hMenu) return;

	// Добавляем пункты меню
	AppendMenuW(hMenu, MF_STRING, ID_MENU_OPEN, L"Открыть");
	AppendMenuW(hMenu, MF_STRING, ID_MENU_ZIP_FILE, L"Создать ZIP (файл)");
	AppendMenuW(hMenu, MF_STRING, ID_MENU_OPEN_FOLDER, L"Открыть папку проекта");
	AppendMenuW(hMenu, MF_STRING, ID_MENU_ZIP_FOLDER, L"Создать ZIP папки проекта");

	// Запоминаем индекс элемента
	g_fileIndexOnContextClick = indexInFiles;

	// Показываем меню
	TrackPopupMenu(hMenu, TPM_RIGHTBUTTON, pt.x, pt.y, 0, hwnd, NULL);

	// Уничтожаем меню после использования
	DestroyMenu(hMenu);
}

//-------------------------------------------------------------------
// Заполнить ListView
//-------------------------------------------------------------------
void PopulateListView(HWND hList)
{
	// Очистим ListView
	ListView_DeleteAllItems(hList);
	// Удалим все колонки (на случай повторного вызова)
	while (ListView_DeleteColumn(hList, 0));

	// Добавим три колонки
	LVCOLUMNW col = { 0 };

	// Первая колонка: Проекты (DAW)
	col.mask = LVCF_WIDTH | LVCF_TEXT;
	col.cx = 400; // Ширина первой колонки
	col.pszText = (LPWSTR)L"Проекты (DAW)";
	ListView_InsertColumn(hList, 0, &col);

	// Вторая колонка: Размер папки
	col.mask = LVCF_WIDTH | LVCF_TEXT;
	col.cx = 150; // Ширина второй колонки
	col.pszText = (LPWSTR)L"Размер папки";
	ListView_InsertColumn(hList, 1, &col);

	// Третья колонка: Дата последнего изменения
	col.mask = LVCF_WIDTH | LVCF_TEXT;
	col.cx = 200; // Ширина третьей колонки
	col.pszText = (LPWSTR)L"Дата последнего изменения";
	ListView_InsertColumn(hList, 2, &col);

	// Привяжем системный ImageList (иконки файлов)
	SHFILEINFOW sfi = { 0 };
	HIMAGELIST himl = (HIMAGELIST)SHGetFileInfoW(
		L"C:\\Windows",
		0,
		&sfi,
		sizeof(sfi),
		SHGFI_SYSICONINDEX | SHGFI_SMALLICON
	);
	ListView_SetImageList(hList, himl, LVSIL_SMALL);

	// Добавим элементы в ListView
	for (size_t i = 0; i < g_Files.size(); i++)
	{
		const auto& fullPath = g_Files[i];
		std::filesystem::path p(fullPath);

		std::wstring filename = p.filename().wstring();
		std::wstring ext = p.extension().wstring();
		std::wstring dawName = GetDawName(ext);

		// Формируем отображаемое имя: "(DAW) filename.ext"
		std::wstring displayName;
		if (!dawName.empty())
			displayName = L"(" + dawName + L") " + filename;
		else
			displayName = filename;

		// Получаем индекс иконки
		SHFILEINFOW sfiItem = { 0 };
		SHGetFileInfoW(
			fullPath.c_str(),
			0,
			&sfiItem,
			sizeof(sfiItem),
			SHGFI_SYSICONINDEX | SHGFI_SMALLICON | SHGFI_USEFILEATTRIBUTES
		);
		int iIcon = sfiItem.iIcon;

		// Получаем путь к папке проекта
		std::wstring folderPath = p.parent_path().wstring();

		// Подсчитываем размер папки
		uintmax_t folderSizeBytes = GetFolderSize(folderPath);
		std::wstring formattedSize = FormatSize(folderSizeBytes);

		// Получаем дату последнего изменения
		std::wstring formattedDate;
		try
		{
			auto ftime = std::filesystem::last_write_time(fullPath);
			auto sctp = std::chrono::time_point_cast<std::chrono::system_clock::duration>(
				ftime - std::filesystem::file_time_type::clock::now()
				+ std::chrono::system_clock::now()
			);
			std::time_t cftime = std::chrono::system_clock::to_time_t(sctp);
			wchar_t dateBuffer[100];
			if (std::wcsftime(dateBuffer, sizeof(dateBuffer) / sizeof(wchar_t), L"%Y-%m-%d %H:%M:%S", std::localtime(&cftime)))
			{
				formattedDate = dateBuffer;
			}
			else
			{
				formattedDate = L"Неизвестно";
			}
		}
		catch (...)
		{
			formattedDate = L"Неизвестно";
		}

		// Формируем LVITEM для первой колонки
		LVITEMW lv = { 0 };
		lv.mask = LVIF_TEXT | LVIF_PARAM | LVIF_IMAGE;
		lv.iItem = static_cast<int>(i);
		lv.iSubItem = 0; // Первая колонка
		lv.pszText = const_cast<LPWSTR>(displayName.c_str());
		lv.lParam = static_cast<LPARAM>(i); // индекс в g_Files
		lv.iImage = iIcon;

		int itemIndex = ListView_InsertItem(hList, &lv);

		// Устанавливаем значение во вторую колонку (Размер папки)
		ListView_SetItemText(hList, itemIndex, 1, const_cast<LPWSTR>(formattedSize.c_str()));

		// Устанавливаем значение в третью колонку (Дата последнего изменения)
		ListView_SetItemText(hList, itemIndex, 2, const_cast<LPWSTR>(formattedDate.c_str()));
	}
}

//-------------------------------------------------------------------
// Обработка двойного клика
//-------------------------------------------------------------------
void OnListViewDblClick(HWND hList)
{
	int iSel = ListView_GetNextItem(hList, -1, LVNI_SELECTED);
	if (iSel == -1) return;

	LVITEMW lvi = { 0 };
	lvi.mask = LVIF_PARAM;
	lvi.iItem = iSel;
	if (ListView_GetItem(hList, &lvi))
	{
		int idx = static_cast<int>(lvi.lParam);
		OpenProject(idx);
	}
}

//-------------------------------------------------------------------
// Обработка правого клика
//-------------------------------------------------------------------
void OnListViewRightClick(HWND hList, LPNMITEMACTIVATE pnmitem)
{
	int iItem = pnmitem->iItem;
	if (iItem < 0) iItem = -1; // Клик по пустой области

	POINT pt = pnmitem->ptAction;
	ClientToScreen(hList, &pt);

	int idx = -1;
	if (iItem >= 0)
	{
		LVITEMW lvi = { 0 };
		lvi.mask = LVIF_PARAM;
		lvi.iItem = iItem;
		if (ListView_GetItem(hList, &lvi))
		{
			idx = static_cast<int>(lvi.lParam);
		}
	}

	ShowContextMenu(GetParent(hList), pt, idx);
}

//-------------------------------------------------------------------
// Оконная процедура
//-------------------------------------------------------------------
LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	switch (message)
	{
	case WM_CREATE:
	{
		// Инициализация Common Controls (ListView)
		INITCOMMONCONTROLSEX icex;
		icex.dwSize = sizeof(icex);
		icex.dwICC = ICC_LISTVIEW_CLASSES;
		InitCommonControlsEx(&icex);

		// Создаём ListView
		RECT rc;
		GetClientRect(hWnd, &rc);
		g_hListView = CreateWindowExW(
			0,
			WC_LISTVIEWW,
			L"",
			WS_VISIBLE | WS_CHILD | LVS_REPORT | LVS_SINGLESEL,
			0, 0,
			rc.right, rc.bottom,
			hWnd,
			(HMENU)1,
			g_hInst,
			NULL
		);

		// Диалог выбора папки
		std::wstring folder = BrowseForFolder(hWnd);
		if (!folder.empty())
		{
			g_chosenFolder = folder;

			// Сбор проектов
			CollectProjectFiles(folder);
			// Сортировка
			SortProjectFiles();
			// Заполнение ListView
			PopulateListView(g_hListView);
		}
		else
		{
			// Если пользователь отменил выбор папки, закрываем приложение
			PostQuitMessage(0);
		}
	}
	break;

	case WM_SIZE:
	{
		int w = LOWORD(lParam);
		int h = HIWORD(lParam);
		SetWindowPos(g_hListView, NULL, 0, 0, w, h, SWP_NOZORDER);
	}
	break;

	case WM_NOTIFY:
	{
		LPNMHDR pnmh = (LPNMHDR)lParam;
		if (pnmh->hwndFrom == g_hListView)
		{
			switch (pnmh->code)
			{
			case NM_DBLCLK:
				OnListViewDblClick(g_hListView);
				break;
			case NM_RCLICK:
			{
				LPNMITEMACTIVATE pnmitem = (LPNMITEMACTIVATE)lParam;
				OnListViewRightClick(g_hListView, pnmitem);
			}
			break;
			}
		}
	}
	break;

	case WM_COMMAND:
	{
		switch (LOWORD(wParam))
		{
		case ID_MENU_OPEN:
		{
			OpenProject(g_fileIndexOnContextClick);
		}
		break;
		case ID_MENU_ZIP_FILE:
		{
			CreateZipArchiveFile(g_fileIndexOnContextClick);
		}
		break;
		case ID_MENU_OPEN_FOLDER:
		{
			OpenProjectFolder(g_fileIndexOnContextClick);
		}
		break;
		case ID_MENU_ZIP_FOLDER:
		{
			CreateZipArchiveProjectFolder(g_fileIndexOnContextClick);
		}
		break;
		}
	}
	break;

	case WM_DESTROY:
		PostQuitMessage(0);
		break;

	default:
		return DefWindowProcW(hWnd, message, wParam, lParam);
	}
	return 0;
}

//-------------------------------------------------------------------
// Точка входа
//-------------------------------------------------------------------
int APIENTRY wWinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPWSTR lpCmdLine, int nCmdShow)
{
	g_hInst = hInstance;

	// Регистрация класса окна
	WNDCLASSEXW wcex = { 0 };
	wcex.cbSize = sizeof(wcex);
	wcex.style = CS_HREDRAW | CS_VREDRAW;
	wcex.lpfnWndProc = WndProc;
	wcex.hInstance = hInstance;
	wcex.hIcon = LoadIcon(NULL, IDI_APPLICATION);
	wcex.hCursor = LoadCursor(NULL, IDC_ARROW);
	wcex.hbrBackground = (HBRUSH)(COLOR_WINDOW + 1);
	wcex.lpszClassName = L"ProjectManagerClass";
	wcex.hIconSm = LoadIcon(NULL, IDI_APPLICATION);

	if (!RegisterClassExW(&wcex))
		return 0;

	// Создание окна
	HWND hWnd = CreateWindowW(
		wcex.lpszClassName,
		L"Project Manager: Архивирование проектов",
		WS_OVERLAPPEDWINDOW,
		CW_USEDEFAULT, 0,
		800, 600,
		NULL,
		NULL,
		hInstance,
		NULL
	);
	if (!hWnd)
		return 0;

	ShowWindow(hWnd, nCmdShow);
	UpdateWindow(hWnd);

	// Цикл сообщений
	MSG msg;
	while (GetMessage(&msg, NULL, 0, 0))
	{
		TranslateMessage(&msg);
		DispatchMessage(&msg);
	}
	return static_cast<int>(msg.wParam);
}
