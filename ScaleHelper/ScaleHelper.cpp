// ScaleSelector_RU.cpp : ���������� ����� ����� ��� ����������.
//
#include <windows.h>
#include <shellapi.h>
#include <string>
#include <vector>
#include <map>
#include <richedit.h>
#include "resource.h"  // <-- ��������� � #define IDI_MYICON

//-----------------------------------------------------
// ���������� ���������� � ���������
//-----------------------------------------------------
const wchar_t CLASS_NAME[] = L"ScaleSelectorWindowClass";
const UINT WM_TRAYICON = WM_USER + 1;
const UINT HOTKEY_ID_PLUS = 1;
const UINT HOTKEY_ID_F1 = 2;
HINSTANCE hInst = NULL;
HFONT hFont = NULL;

// �������� ������� ������� F1?
const bool ENABLE_F1_HOTKEY = true;

// ������������� Rich Edit
#define IDC_RICHEDIT1 104

//-----------------------------------------------------
// ������������� ����� (12-�������)
//-----------------------------------------------------
const std::vector<std::wstring> chromatic_scale = {
	L"C",  L"C#",  L"D",  L"D#",  L"E",  L"F",
	L"F#", L"G",   L"G#", L"A",   L"A#", L"B"
};

// ������ (12 ����)
const std::vector<std::wstring> tonics = {
	L"C",  L"C#",  L"D",  L"D#",  L"E",  L"F",
	L"F#", L"G",   L"G#", L"A",   L"A#", L"B"
};

//-----------------------------------------------------
// ������� ������ ������� (��� ����������)
// (������������, ������������, ��������)
//-----------------------------------------------------
const std::map<std::wstring, std::vector<int>> big_scale_patterns = {

	// --- ������������/�������� � �������� ---
	{L"����� (Ionian)",               {2, 2, 1, 2, 2, 2, 1}},
	{L"����������� ����� (Aeolian)",  {2, 1, 2, 2, 1, 2, 2}},
	{L"������������� �����",          {2, 1, 2, 2, 1, 3, 1}},
	{L"������������ ����� (Asc.)",    {2, 1, 2, 2, 2, 2, 1}},
	{L"��������� (Dorian)",           {2, 1, 2, 2, 2, 1, 2}},
	{L"���������� (Phrygian)",        {1, 2, 2, 2, 1, 2, 2}},
	{L"��������� (Lydian)",           {2, 2, 2, 1, 2, 2, 1}},
	{L"�������������� (Mixolyd.)",    {2, 2, 1, 2, 2, 1, 2}},
	{L"���������� (Locrian)",         {1, 2, 2, 1, 2, 2, 2}},

	// ����������� � ����
	{L"�������� �����������",         {2, 2, 3, 2, 3}},
	{L"�������� �����������",         {3, 2, 2, 3, 2}},
	{L"�������� (Blues)",             {3, 2, 1, 1, 3, 2}},

	// �����
	{L"�������� �����",               {2, 2, 1, 2, 2, 1, 1, 2}},
	{L"�������� �����",               {2, 1, 2, 2, 1, 1, 2, 2}},
	{L"������������ �����",           {2, 2, 1, 2, 1, 1, 2, 2}},

	// ��������������
	{L"�������������� �����",        {1, 2, 2, 2, 2, 2, 1}},
	{L"�������������� �����",        {1, 2, 2, 2, 1, 2, 2}},

	// ����������, �������������
	{L"���������� (Whole Tone)",      {2, 2, 2, 2, 2, 2}},
	{L"������������� (12 Half)",      {1,1,1,1,1,1,1,1,1,1,1,1}},

	// --- ������� �������������, ���������� ---
	{L"������� ������������� (Double Harm.)", {1, 3, 1, 2, 1, 3, 1}},
	{L"���������� ����� (Hungarian Minor)",   {2, 1, 3, 1, 1, 3, 1}},
	{L"���������� ����� (Hungarian Major)",   {3, 1, 2, 1, 2, 1, 2}},

	// --- ������������ (��������, ���������) ---
	{L"�������� (Bhairav)",            {1, 3, 1, 2, 1, 2, 2}},
	{L"��������� (Todi)",              {1, 2, 2, 2, 2, 2, 1}},
	{L"���������� (Egyptian)",         {2, 1, 2, 2, 2, 2, 3}},
	{L"���������� (Persian)",          {1, 3, 1, 2, 1, 2, 2}},
	{L"�������� (Hijaz)",              {1, 3, 1, 2, 1, 3, 1}},
	{L"��������� (Sufi)",              {1, 3, 1, 2, 1, 2, 2}},

	// --- �������� (���������) ---
	{L"����� (Trance)",                {3, 1, 2, 2, 2, 1, 1}},
	{L"�������� (PsyTrance)",          {3, 2, 1, 2, 1, 2, 1}},
	{L"��������� (Cinematic)",         {2, 2, 1, 3, 1, 2, 1}},
	{L"������� (Ambient)",            {2, 1, 2, 1, 2, 2, 2}},
	{L"����-�-���� (DnB)",            {2, 2, 1, 2, 1, 2, 2}},
	{L"������ (Saiomy)",              {3, 1, 1, 3, 1, 2, 1}}
};

//-----------------------------------------------------
// ���������/������� ������ � ��������� ����
//-----------------------------------------------------
void AddTrayIcon(HWND hwnd) {
	NOTIFYICONDATA nid = {};
	nid.cbSize = sizeof(NOTIFYICONDATA);
	nid.hWnd = hwnd;
	nid.uID = 1;
	nid.uFlags = NIF_ICON | NIF_MESSAGE | NIF_TIP;
	nid.uCallbackMessage = WM_TRAYICON;

	// ��������� ������ �� �������� (IDI_MYICON)
	nid.hIcon = LoadIcon(hInst, MAKEINTRESOURCE(IDI_MYICON));

	wcscpy_s(nid.szTip, L"����� �����");
	Shell_NotifyIcon(NIM_ADD, &nid);
}

void RemoveTrayIcon(HWND hwnd) {
	NOTIFYICONDATA nid = {};
	nid.cbSize = sizeof(NOTIFYICONDATA);
	nid.hWnd = hwnd;
	nid.uID = 1;
	Shell_NotifyIcon(NIM_DELETE, &nid);
}

//-----------------------------------------------------
// ������� ��� ����������� ���� ��� ����� � ����
//-----------------------------------------------------
void ShowTrayMenu(HWND hwnd) {
	POINT pt;
	GetCursorPos(&pt);
	HMENU hMenu = CreatePopupMenu();
	if (hMenu) {
		InsertMenu(hMenu, -1, MF_BYPOSITION, 1, L"�������");
		InsertMenu(hMenu, -1, MF_BYPOSITION, 2, L"������");
		InsertMenu(hMenu, -1, MF_BYPOSITION, 3, L"�����");
		SetForegroundWindow(hwnd);
		UINT clicked = TrackPopupMenu(
			hMenu,
			TPM_BOTTOMALIGN | TPM_LEFTALIGN | TPM_RETURNCMD,
			pt.x, pt.y,
			0, hwnd, NULL
		);
		if (clicked == 1) {
			// "�������"
			ShowWindow(hwnd, SW_SHOW);
		}
		else if (clicked == 2) {
			// "������"
			ShowWindow(hwnd, SW_HIDE);
		}
		else if (clicked == 3) {
			// "�����" � ��������� �������
			PostMessage(hwnd, WM_DESTROY, 0, 0);
		}
		DestroyMenu(hMenu);
	}
}

//-----------------------------------------------------
// ������� �������
//-----------------------------------------------------
bool RegisterHotkeys(HWND hwnd) {
	if (!RegisterHotKey(hwnd, HOTKEY_ID_PLUS, MOD_NOREPEAT, VK_ADD)) {
		return false;
	}
	if (ENABLE_F1_HOTKEY) {
		RegisterHotKey(hwnd, HOTKEY_ID_F1, MOD_NOREPEAT, VK_F1);
	}
	return true;
}
void UnregisterHotkeys() {
	UnregisterHotKey(NULL, HOTKEY_ID_PLUS);
	if (ENABLE_F1_HOTKEY) {
		UnregisterHotKey(NULL, HOTKEY_ID_F1);
	}
}

//-----------------------------------------------------
// ���������� �����
//-----------------------------------------------------
std::vector<std::wstring> CalculateScale(const std::wstring& tonic,
	const std::vector<int>& pattern)
{
	std::vector<std::wstring> scale_notes;
	int index = -1;
	for (size_t i = 0; i < chromatic_scale.size(); ++i) {
		if (chromatic_scale[i] == tonic) {
			index = static_cast<int>(i);
			break;
		}
	}
	if (index == -1) {
		return scale_notes;
	}
	// ��������� ������ ���� (������)
	scale_notes.push_back(chromatic_scale[index]);

	// ��������� ���������
	for (int interval : pattern) {
		index = (index + interval) % chromatic_scale.size();
		scale_notes.push_back(chromatic_scale[index]);
	}
	return scale_notes;
}

//-----------------------------------------------------
// �������� ComboBox (������)
//-----------------------------------------------------
HWND CreateComboBoxControl(HWND hwndParent, int id,
	int x, int y, int width, int height,
	const std::vector<std::wstring>& items)
{
	HWND hwndCombo = CreateWindowEx(
		0, L"COMBOBOX", NULL,
		CBS_DROPDOWNLIST | CBS_HASSTRINGS | WS_CHILD | WS_VISIBLE
		| WS_VSCROLL | CBS_DISABLENOSCROLL,
		x, y, width, height,
		hwndParent, (HMENU)(INT_PTR)id, hInst, NULL
	);

	for (const auto& item : items) {
		SendMessage(hwndCombo, CB_ADDSTRING, 0, (LPARAM)item.c_str());
	}
	// �� ��������� �������� ������ �������
	SendMessage(hwndCombo, CB_SETCURSEL, 0, 0);

	return hwndCombo;
}

//-----------------------------------------------------
// ������� ������ � Rich Edit (� ������)
//-----------------------------------------------------
void InsertColoredText(HWND hRichEdit, const std::wstring& text, COLORREF color) {
	SendMessage(hRichEdit, EM_SETSEL, (WPARAM)-1, (LPARAM)-1);

	CHARFORMAT2 cf = {};
	cf.cbSize = sizeof(CHARFORMAT2);
	cf.dwMask = CFM_COLOR;
	cf.crTextColor = color;
	SendMessage(hRichEdit, EM_SETCHARFORMAT, SCF_SELECTION, (LPARAM)&cf);

	SendMessage(hRichEdit, EM_REPLACESEL, FALSE, (LPARAM)text.c_str());
}

//-----------------------------------------------------
// ������� ���������
//-----------------------------------------------------
LRESULT CALLBACK WindowProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam) {
	static HWND hwndScale, hwndTonic, hwndButton, hwndRichEdit;
	static HMODULE msftEdit = 0;

	switch (uMsg) {
	case WM_CREATE:
	{
		// ��������� ���������� Rich Edit
		msftEdit = LoadLibrary(TEXT("Msftedit.dll"));

		AddTrayIcon(hwnd);
		RegisterHotkeys(hwnd);

		// ������� "�������� �����:"
		CreateWindow(
			L"STATIC",
			L"�������� �����:",
			WS_VISIBLE | WS_CHILD,
			20, 30, 120, 20,
			hwnd,
			NULL,
			hInst,
			NULL
		);

		// ��������� ComboBox ��� �������
		{
			std::vector<std::wstring> scaleNames;
			scaleNames.reserve(big_scale_patterns.size());
			for (auto& kv : big_scale_patterns) {
				scaleNames.push_back(kv.first);
			}
			hwndScale = CreateComboBoxControl(hwnd, 101, 150, 25, 220, 200, scaleNames);
		}

		// ������� "�������� ������:"
		CreateWindow(
			L"STATIC",
			L"�������� ������:",
			WS_VISIBLE | WS_CHILD,
			20, 80, 120, 20,
			hwnd,
			NULL,
			hInst,
			NULL
		);
		hwndTonic = CreateComboBoxControl(hwnd, 102, 150, 75, 220, 200, tonics);

		// ������ "�������� ����"
		hwndButton = CreateWindow(
			L"BUTTON",
			L"�������� ����",
			WS_TABSTOP | WS_VISIBLE | WS_CHILD | BS_DEFPUSHBUTTON,
			170, 130, 120, 30,
			hwnd,
			(HMENU)(INT_PTR)103,
			hInst,
			NULL
		);

		// Rich Edit
		hwndRichEdit = CreateWindowEx(
			0, MSFTEDIT_CLASS, NULL,
			WS_CHILD | WS_VISIBLE | ES_MULTILINE | ES_READONLY
			| WS_VSCROLL | WS_BORDER,
			20, 180, 440, 60,
			hwnd,
			(HMENU)(INT_PTR)IDC_RICHEDIT1,
			hInst,
			NULL
		);

		// ������������� ����� ��� Rich Edit (�������� � 4 ����)
		NONCLIENTMETRICS ncm = { sizeof(NONCLIENTMETRICS) };
		SystemParametersInfo(SPI_GETNONCLIENTMETRICS, sizeof(NONCLIENTMETRICS), &ncm, 0);

		LOGFONT lf = ncm.lfMessageFont;
		//lf.lfHeight = 16;    // ���� ���� 16
		lf.lfHeight = 16 * 4; // ����������� � 4 ����
		hFont = CreateFontIndirect(&lf);
		SendMessage(hwndRichEdit, WM_SETFONT, (WPARAM)hFont, TRUE);
	}
	break;

	case WM_TRAYICON:
		if (lParam == WM_RBUTTONUP) {
			ShowTrayMenu(hwnd);
		}
		break;

	case WM_HOTKEY:
	{
		if (wParam == HOTKEY_ID_PLUS) {
			// NumPad '+'
			if (IsWindowVisible(hwnd)) {
				ShowWindow(hwnd, SW_HIDE);
			}
			else {
				ShowWindow(hwnd, SW_SHOW);
				SetForegroundWindow(hwnd);
			}
		}
		else if (ENABLE_F1_HOTKEY && wParam == HOTKEY_ID_F1) {
			// F1
			if (IsWindowVisible(hwnd)) {
				ShowWindow(hwnd, SW_HIDE);
			}
			else {
				ShowWindow(hwnd, SW_SHOW);
				SetForegroundWindow(hwnd);
			}
		}
	}
	break;

	case WM_COMMAND:
	{
		int wmId = LOWORD(wParam);
		switch (wmId) {
		case 103: // ������ ������ "�������� ����"
		{
			// 1) �������� ��������� �����
			HWND hwndScaleCB = GetDlgItem(hwnd, 101);
			LRESULT scaleResult = SendMessage(hwndScaleCB, CB_GETCURSEL, 0, 0);
			int scaleIndex = (scaleResult != CB_ERR) ? static_cast<int>(scaleResult) : -1;

			wchar_t scaleName[200];
			scaleName[0] = L'\0';
			if (scaleIndex != -1) {
				SendMessage(hwndScaleCB, CB_GETLBTEXT, scaleIndex, (LPARAM)scaleName);
			}

			// 2) �������� ��������� ������
			HWND hwndTonicCB = GetDlgItem(hwnd, 102);
			LRESULT tonicResult = SendMessage(hwndTonicCB, CB_GETCURSEL, 0, 0);
			int tonicIndex = (tonicResult != CB_ERR) ? static_cast<int>(tonicResult) : -1;

			wchar_t tonicName[10];
			tonicName[0] = L'\0';
			if (tonicIndex != -1) {
				SendMessage(hwndTonicCB, CB_GETLBTEXT, tonicIndex, (LPARAM)tonicName);
			}

			if (scaleIndex == -1 || tonicIndex == -1) {
				MessageBox(hwnd,
					L"����������, �������� ����� � ������.",
					L"��������������",
					MB_OK | MB_ICONWARNING);
				break;
			}

			// ���� �������
			std::wstring selectedScale(scaleName);
			auto it = big_scale_patterns.find(selectedScale);
			if (it != big_scale_patterns.end()) {
				// ��������� ����
				std::vector<std::wstring> notes = CalculateScale(tonicName, it->second);
				if (!notes.empty()) {
					// ������� Rich Edit
					SendMessage(hwndRichEdit, EM_SETSEL, 0, -1);
					SendMessage(hwndRichEdit, EM_REPLACESEL, FALSE, (LPARAM)L"");

					// -- ������� ���������. 
					//   ������ ����:
					//   std::wstring header = L"���� � ����� \"" + selectedScale + ...;
					//   InsertColoredText(...);

					// ������ ����� ������ ������� ����:
					for (size_t i = 0; i < notes.size(); ++i) {
						std::wstring note = notes[i] + L" ";
						if (((i + 1) % 2) != 0) {
							InsertColoredText(hwndRichEdit, note, RGB(0, 128, 0)); // �������� � ������
						}
						else {
							InsertColoredText(hwndRichEdit, note, RGB(255, 0, 0)); // ׸���� � �������
						}
					}
				}
				else {
					MessageBox(hwnd,
						L"������� ������������ ������.",
						L"������",
						MB_OK | MB_ICONERROR);
				}
			}
			else {
				MessageBox(hwnd,
					L"������ ����� �� ������.",
					L"������",
					MB_OK | MB_ICONERROR);
			}
		}
		break;

		default:
			break;
		}
	}
	break;

	case WM_DESTROY:
	{
		RemoveTrayIcon(hwnd);
		UnregisterHotkeys();
		if (hFont) {
			DeleteObject(hFont);
		}
		if (msftEdit) {
			FreeLibrary(msftEdit);
		}
		PostQuitMessage(0);
	}
	break;

	case WM_CLOSE:
		// ���� ������������ ������ ���� (����. Alt+F4),
		// ����� ������ ��������:
		ShowWindow(hwnd, SW_HIDE);
		break;

	default:
		return DefWindowProc(hwnd, uMsg, wParam, lParam);
	}
	return 0;
}

//-----------------------------------------------------
// ����� �����
//-----------------------------------------------------
#include <sal.h>  // ��� _In_ / _In_opt_ ��������� (�������������)

int WINAPI wWinMain(
	_In_     HINSTANCE hInstance,
	_In_opt_ HINSTANCE hPrevInstance,
	_In_     PWSTR     lpCmdLine,
	_In_     int       nCmdShow
)
{
	hInst = hInstance;

	// ������������ ����� ����
	WNDCLASS wc = {};
	wc.lpfnWndProc = WindowProc;
	wc.hInstance = hInstance;

	// <-- ����������� ������ ���������� (����)
	wc.hIcon = LoadIcon(hInstance, MAKEINTRESOURCE(IDI_MYICON));

	wc.hCursor = LoadCursor(NULL, IDC_ARROW);
	wc.hbrBackground = (HBRUSH)(COLOR_WINDOW + 1);
	wc.lpszClassName = CLASS_NAME;
	RegisterClass(&wc);

	// ������ ����
	HWND hwnd = CreateWindowEx(
		0,
		CLASS_NAME,
		L"����� ����� (��������� ������)",
		WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_MINIMIZEBOX,
		CW_USEDEFAULT, CW_USEDEFAULT, 500, 315,
		NULL,
		NULL,
		hInstance,
		NULL
	);

	if (!hwnd) {
		return 0;
	}

	// ���������� ����
	ShowWindow(hwnd, nCmdShow);
	UpdateWindow(hwnd);

	// ���� ���������
	MSG msg = {};
	while (GetMessage(&msg, NULL, 0, 0)) {
		TranslateMessage(&msg);
		DispatchMessage(&msg);
	}
	return 0;
}
