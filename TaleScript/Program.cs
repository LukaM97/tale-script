using System.Runtime.InteropServices;
using TaleScript;

if (args.Length > 1)
{
    Environment.Exit(64);
}
else if (args.Length == 1)
{
    RunFile(args[0]);
}
else
{
    RunPrompt();
}

static void RunFile(string filePath)
{
    string content = File.ReadAllText(Path.GetFullPath(filePath));
    run(content);
}

static void RunPrompt()
{
    for (; ; )
    {
        Console.Write("> ");
        string? line = Console.ReadLine();
        if (line == null) break;
        run(line);
    }
}

static void run(string source)
{
    Scanner scanner = new Scanner(source);
    List<Token> tokens = scanner.ScanTokens();

    tokens.ForEach(token => Console.WriteLine(token));
}