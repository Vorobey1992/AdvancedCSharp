using AdvancedCSharp.Task1;

string rootPath = string.Empty;
bool rootPathCheck = false;

while (string.IsNullOrWhiteSpace(rootPath) || !rootPathCheck)
{
    Console.WriteLine("Please enter the root path: ");
    rootPath = Console.ReadLine() ?? String.Empty;
    rootPathCheck = Directory.Exists(rootPath);
}

    Console.WriteLine("Please enter the file name or some phrases: ");
    string searchCriteria = Console.ReadLine() ?? String.Empty;

    Console.WriteLine("Please enter extension if it's possible: ");
    string searchExtension = Console.ReadLine() ?? String.Empty;


var fileSystemVisitor = new FileSystemVisitor(rootPath, searchExtension, searchCriteria, (file) =>
{
    string extension = Path.GetExtension(file);
    string fileName = Path.GetFileName(file);

    // Отладочный вывод для отслеживания значений
    //Console.WriteLine($"File: {file}, Extension: {extension}, FileName: {fileName}");

    // Если searchExtension и searchCriteria указаны
    if (!string.IsNullOrEmpty(searchExtension) && !string.IsNullOrEmpty(searchCriteria))
    {
        return (extension == searchExtension) && fileName.Contains(searchCriteria);
    }

    // Если только searchExtension указан
    if (!string.IsNullOrEmpty(searchExtension))
    {
        return (extension == searchExtension);
    }

    // Если только searchCriteria указан
    if (!string.IsNullOrEmpty(searchCriteria))
    {
        return fileName.Contains(searchCriteria);
    }

    // Если ни searchExtension, ни searchCriteria не указаны, фильтруем все файлы
    return true;
});

fileSystemVisitor.Start += (sender, e) =>
{
    Console.WriteLine("Search started...");
};

fileSystemVisitor.Finish += (sender, e) =>
{
    Console.WriteLine("Search finished.");
};

fileSystemVisitor.FileFound += (sender, filePath) =>
{
    Console.WriteLine($"Found file: {filePath}");
};

fileSystemVisitor.DirectoryFound += (sender, directoryPath) =>
{
    Console.WriteLine($"Found directory: {directoryPath}");
};

fileSystemVisitor.FilteredFileFound += (sender, filePath) =>
{
    Console.WriteLine($"Filtered file found: {filePath}");
};

fileSystemVisitor.FilteredDirectoryFound += (sender, directoryPath) =>
{
    Console.WriteLine($"Filtered directory found: {directoryPath}");
};

Console.WriteLine("Enter a command (exclude, abort, list, exit):");
string command = Console.ReadLine()?.Trim() ?? string.Empty;

while (!command.Equals("exit", StringComparison.OrdinalIgnoreCase))
{
    switch (command)
    {
        case "exclude":
            Console.WriteLine("Enter paths to exclude (separated by commas):");
            string pathsToExclude = Console.ReadLine()?.Trim() ?? string.Empty;
            if (!string.IsNullOrEmpty(pathsToExclude))
            {
                string[] paths = pathsToExclude.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string path in paths)
                {
                    fileSystemVisitor.ExcludeItem(path.Trim());
                }
            }
            break;
        case "abort":
            fileSystemVisitor.Abort();
            Console.WriteLine("Search aborted.");
            break;
        case "list":
            foreach (string item in fileSystemVisitor)
            {
                Console.WriteLine(item);
            }
            break;
        default:
            Console.WriteLine("Invalid command.");
            break;
    }

    Console.WriteLine("Enter a command (exclude, abort, list, exit):");
    command = Console.ReadLine()?.Trim() ?? string.Empty;
}
