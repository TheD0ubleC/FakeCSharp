using System.Runtime.InteropServices;
using System.Text;
class Program
{
    static void Main(string[] args)
    {
        ConsoleProMax.WriteLine(new FakeString("1.Hello, World! Press any key to continue | 你好，世界！按下任意键继续"));
        ConsoleProMax.Read();
        ConsoleProMax.WriteLine(new FakeString("2.Enter the minimum value for random range | 请输入一个数作为最小值："));
        FakeString inputMin = ConsoleProMax.Read();
        FakeInt min = FakeInt.Parse(inputMin.ToFakeString());
        ConsoleProMax.WriteLine(new FakeString("3.Enter the maximum value for random range | 请输入一个数作为最大值："));
        FakeString inputMax = ConsoleProMax.Read();
        FakeInt max = FakeInt.Parse(inputMax.ToFakeString());
        FakeInt firstInt = FakeInt.RandomFakeInt(min, max);
        ConsoleProMax.WriteLine(new FakeString($"4.The minimum value <{inputMin}> and the maximum value <{inputMax}> the random value is <{firstInt.ToFakeString()}>"));
        ConsoleProMax.WriteLine(new FakeString($"4.最小值<{inputMin}>与最大值<{inputMax}>取出的随机值为<{firstInt.ToFakeString()}>"));
        ConsoleProMax.Read();
    }
}
public class ConsoleProMax
{
    public const int STD_INPUT_HANDLE = -10;
    [DllImport("Kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    public static extern bool WriteConsole(IntPtr hConsoleOutput, string lpBuffer, uint nNumberOfCharsToWrite, out uint lpNumberOfCharsWritten, IntPtr lpReserved);
    [DllImport("Kernel32.dll")]
    public static extern IntPtr GetStdHandle(int nStdHandle);
    [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool ReadConsole(IntPtr hConsoleInput,[Out] char[] lpBuffer,uint nNumberOfCharsToRead,out uint lpNumberOfCharsRead,IntPtr lpReserved);
    [DllImport("user32.dll")]
    public static extern short GetAsyncKeyState(int vKey);

    public const int STD_OUTPUT_HANDLE = -11;
    public static void WriteLine(FakeString fakeString)
    {
        IntPtr consoleHandle = GetStdHandle(STD_OUTPUT_HANDLE);
        string message = fakeString.ToFakeString();
        WriteConsole(consoleHandle, message + "\r\n", (uint)message.Length + 2, out uint charsWritten, IntPtr.Zero);
    }
    public static void WriteLine(FakeInt fakeInt)
    {
        IntPtr consoleHandle = GetStdHandle(STD_OUTPUT_HANDLE);
        string message = fakeInt.ToFakeString();
        WriteConsole(consoleHandle, message + "\r\n", (uint)message.Length + 2, out uint charsWritten, IntPtr.Zero);
    }
    public static void WriteLine(string message)
    {
        IntPtr consoleHandle = GetStdHandle(STD_OUTPUT_HANDLE);
        WriteConsole(consoleHandle, message + "\r\n", (uint)message.Length + 2, out uint charsWritten, IntPtr.Zero);
    }
    public static string Read()
    {
        IntPtr consoleInputHandle = GetStdHandle(STD_INPUT_HANDLE);
        char[] buffer = new char[256];
        if (ReadConsole(consoleInputHandle, buffer, 255, out uint charsRead, IntPtr.Zero))
        {
            FakeStringBuilder fakeStringBuilder = new FakeStringBuilder();
            fakeStringBuilder.Append(buffer, (int)charsRead);
            return fakeStringBuilder.ToString();
        }
        return string.Empty;
    }
    public static ConsoleKeyInfo ReadKey(bool intercept = false)
    {
        for (int i = 0; i < 255; i++)
        {
            short result = GetAsyncKeyState(i);
            if (result == 1 || result == -32767)
            {
                return new ConsoleKeyInfo((char)i, (ConsoleKey)i, false, false, false);
            }
        }
        return new ConsoleKeyInfo();
    }
}
public class FakeStringBuilder
{
    private StringBuilder _innerStringBuilder = new StringBuilder();
    public FakeStringBuilder()
    {
    }
    public void Append(char[] value, int length)
    {
        _innerStringBuilder.Append(value, 0, length);
    }
    public void Append(string str)
    {
        _innerStringBuilder.Append(str);
    }
    public override string ToString()
    {
        return _innerStringBuilder.ToString();
    }
}
public class FakeString
{
    private char[] characters;
    private StringBuilder _innerStringBuilder = new StringBuilder();
    public FakeString(string value)
    {
        characters = value.ToCharArray();
    }
    public static implicit operator FakeString(string value)
    {
        return new FakeString(value);
    }
    public int Length => characters.Length;
    public char this[int index]
    {
        get => characters[index];
        set => characters[index] = value;
    }
    public char[] ToCharArray()
    {
        return characters;
    }
    public void Append(char[] value, int length)
    {
        _innerStringBuilder.Append(value, 0, length);
    }
    public string ToFakeString()
    {
        return new string(characters);
    }
}
public class FakeInt
{
    private int value;
    private static Random random = new Random();
    public FakeInt(int value)
    {
        this.value = value;
    }
    public static implicit operator FakeInt(int value)
    {
        return new FakeInt(value);
    }
    public static implicit operator int(FakeInt fakeInt)
    {
        return fakeInt.value;
    }
    public static FakeInt operator +(FakeInt a, FakeInt b)
    {
        return new FakeInt(a.value + b.value);
    }
    public static FakeInt operator -(FakeInt a, FakeInt b)
    {
        return new FakeInt(a.value - b.value);
    }
    public static FakeInt operator *(FakeInt a, FakeInt b)
    {
        return new FakeInt(a.value * b.value);
    }
    public static FakeInt operator /(FakeInt a, FakeInt b)
    {
        if (b.value == 0)
        {
            throw new DivideByZeroException("cannot divide by zero");
        }
        return new FakeInt(a.value / b.value);
    }
    public string ToFakeString()
    {
        return value.ToString();
    }
    public static FakeInt Parse(string stringValue)
    {
        if (int.TryParse(stringValue, out int result))
        {
            return new FakeInt(result);
        }
        else
        {
            throw new FormatException("Input string was not in a correct format.");
        }
    }
    public static FakeInt RandomFakeInt(FakeInt min, FakeInt max)
    {
        int minValue = min;
        int maxValue = max;
        if (minValue > maxValue)
        {
            int temp = minValue;
            minValue = maxValue;
            maxValue = temp;
        }
        int randomValue = random.Next(minValue, maxValue + 1);
        return new FakeInt(randomValue);
    }
}