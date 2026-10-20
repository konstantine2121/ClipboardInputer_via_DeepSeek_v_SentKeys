using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClipboardInputer_via_DeepSeek_v_SentKeys;

public partial class Form1 : Form
{
    // Символы, которые SendKeys интерпретирует как модификаторы или спецкоды.
    // Их нужно оборачивать в фигурные скобки при отправке.
    private const string SendKeysSpecialChars = "+^%~(){}[]";
    
    private const int MillisecondsDelayBeforeExitOnError = 3000;


    public Form1()
    {
        InitializeComponent();
        
        this.Load += async (s, e) => await ExecuteScenarioAsync();
    }

    // Prevent my window from getting activation and focus when shown
    protected override bool ShowWithoutActivation => true;

    private async Task ExecuteScenarioAsync()
    {
        // === Шаг 1: Считывание данных из буфера обмена ===
        Log("Чтение данных из буфера обмена...");

        if (!Clipboard.ContainsText())
        {
            Invoke(Activate);
            Log("Нет текстовых данных в буфере обмена.");


            await Task.Delay(MillisecondsDelayBeforeExitOnError);
            Application.Exit();
            return;
        }

        string clipboardText = Clipboard.GetText();

        if (string.IsNullOrWhiteSpace(clipboardText))
        {
            Invoke(Activate);
            Log("Данные в буфере пусты.");
            await Task.Delay(MillisecondsDelayBeforeExitOnError);
            Application.Exit();
            return;
        }

        Log($"Получен текст: \"{clipboardText}\"");

        // === Шаг 2: Распознавание и экранирование допустимых символов ===
        // Нормализуем переводы строк: \r\n и \r приводим к \n,
        // чтобы не получить двойной {ENTER}.
        string normalized = clipboardText.Replace("\r\n", "\n").Replace("\r", "\n");

        var filtered = new StringBuilder();

        foreach (char c in normalized)
        {
            // Пробел — печатный символ, разрешён напрямую.
            if (c == ' ')
            {
                filtered.Append(c);
                continue;
            }

            // Управляющие символы (перевод строки, табуляция и т.п.)
            // преобразуем в именованные коды SendKeys.
            if (char.IsControl(c))
            {
                string? code = GetSendKeysControlCode(c);
                if (code != null)
                {
                    filtered.Append(code);
                }
                // Если кода нет — безопасно пропустить символ.
                continue;
            }

            // Печатные символы: латиница, цифры, знаки пунктуации и спецсимволы.
            if (IsAllowedChar(c))
            {
                if (SendKeysSpecialChars.IndexOf(c) >= 0)
                {
                    // Экранируем спецсимвол SendKeys фигурными скобками.
                    filtered.Append('{').Append(c).Append('}');
                }
                else
                {
                    filtered.Append(c);
                }
            }
            // Остальные символы (не-ASCII и т.п.) игнорируются.
        }

        string keysToSend = filtered.ToString();

        if (string.IsNullOrEmpty(keysToSend))
        {
            Invoke(Activate);
            Log("Не найдено допустимых символов для ввода.");
            await Task.Delay(MillisecondsDelayBeforeExitOnError);
            Application.Exit();
            return;
        }

        Log($"Подготовлено к вводу: \"{keysToSend}\"");

        // === Шаг 3: Эмуляция последовательного нажатия ===
        Log("Подготовка к эмуляции ввода через 1 секунду...");
        await Task.Delay(1000);

        try
        {
            SendKeys.SendWait(keysToSend);
            Log("Эмуляция ввода завершена.");
        }
        catch (Exception ex)
        {
            Invoke(Activate);
            Log($"Ошибка эмуляции: {ex.Message}");
        }

        // === Шаг 4: Завершение через 5 секунд ===
        Log("Завершение работы через 5 секунд...");
        await Task.Delay(5000);
        Application.Exit();
    }

    /// <summary>
    /// Разрешены: буквы латинского алфавита, цифры и все печатные
    /// знаки пунктуации/символы ASCII (коды 33..126).
    /// </summary>
    private static bool IsAllowedChar(char c)
    {
        // Латиница
        if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z')) return true;

        // Цифры
        if (c >= '0' && c <= '9') return true;

        // Печатные символы ASCII — все знаки пунктуации и спецсимволы.
        if (c >= 33 && c <= 126) return true;

        return false;
    }

    /// <summary>
    /// Возвращает код SendKeys для управляющего символа.
    /// Если символ не имеет безопасного представления — возвращает null.
    /// </summary>
    private static string? GetSendKeysControlCode(char c)
    {
        return c switch
        {
            '\r' => "{ENTER}",
            '\n' => "{ENTER}",   // LF → Enter
            '\t' => "{TAB}",
            '\b' => "{BACKSPACE}",
            '\f' => null,        // Form feed — пропускаем
            '\v' => null,        // Vertical tab — пропускаем
            '\0' => null,        // NUL — пропускаем
            '\u001b' => "{ESC}",     // ESC
            '\u007f' => "{DELETE}",  // DEL
            _ => null            // остальные управляющие — пропускаем
        };
    }

    private void Log(string message)
    {
        if (logOutput.InvokeRequired)
        {
            logOutput.Invoke(new Action(() => Log(message)));
            return;
        }
        logOutput.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
        logOutput.SelectionStart = logOutput.Text.Length;
        logOutput.ScrollToCaret();
    }
}
