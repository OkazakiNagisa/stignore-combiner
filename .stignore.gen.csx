using System.Text.RegularExpressions;

var records = new List<string>();

async Task main()
{
    // if (!File.Exists(".stignore"))
    {
        try
        {
            using var file = new StreamWriter(".stignore", false);
            await file.WriteAsync("#include .stignore.gen");
        }
        catch (SystemException e)
        {
            Console.Error.WriteLine(e.Message);
            Console.ReadKey();
        }
    }

    Console.WriteLine($"Working dir: {Environment.CurrentDirectory}\nContinue?");
    Console.ReadKey();

    await WalkDirectoryTree(new DirectoryInfo(Environment.CurrentDirectory), true);
    try
    {
        using var file = new StreamWriter(".stignore.gen", false);

        foreach (string line in records)
            await file.WriteLineAsync(line);
    }
    catch (SystemException e)
    {
        Console.Error.WriteLine(e.Message);
    }

    Console.WriteLine("Finished, any key to exit.");
    Console.ReadKey();
}

async Task WalkDirectoryTree(DirectoryInfo dir, bool root)
{
    FileInfo[] files = { };
    DirectoryInfo[] subDirs = { };

    if (!root)
    {
        try
        {
            files = dir.GetFiles(".stignore");
        }
        catch (UnauthorizedAccessException e)
        {
            Console.Error.WriteLine(e.Message);
        }
        catch (DirectoryNotFoundException e)
        {
            Console.Error.WriteLine(e.Message);
        }

        if (files is not null)
            foreach (var file in files)
                await ParseFile(file);
    }

    // Now find all the subdirectories under this directory.
    subDirs = dir.GetDirectories();
    foreach (var dirInfo in subDirs)
        await WalkDirectoryTree(dirInfo, false);
}

var regex = new Regex(@"^(#include\s|((!|(\(\?[di]\))){0,3}/))(?!/)", RegexOptions.Singleline);

async Task ParseFile(FileInfo file)
{
    records.Add($"/// {file.FullName.Replace('\\', '/').Substring(Environment.CurrentDirectory.Length + 1)}");
    try
    {
        string line = null;
        var reader = file.OpenText();
        while ((line = await reader.ReadLineAsync()) != null)
        {
            line = line.Trim();
            line = regex.Replace(line.Trim(),
                m => m.Value + file.DirectoryName!.Replace('\\', '/')
                                                  .Substring(Environment.CurrentDirectory.Length + 1) + "/");
            records.Add(line);
        }
    }
    catch (UnauthorizedAccessException e)
    {
        Console.Error.WriteLine(e.Message);
    }
    catch (FileNotFoundException e)
    {
        Console.Error.WriteLine(e.Message);
    }
    records.Add(string.Empty);
}

await main();