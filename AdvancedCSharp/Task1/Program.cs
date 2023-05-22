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

    Console.WriteLine("Do you want to display all files and folders? (Y/N)");
    string displayOption = Console.ReadLine()?.Trim() ?? String.Empty;

    bool showAllItems = displayOption?.Equals("Y", StringComparison.OrdinalIgnoreCase) ?? true;




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


fileSystemVisitor.AbortSearch += (sender, e) =>
{
    // Обработчик события AbortSearch
    fileSystemVisitor.Abort(); // Установка флага shouldAbortSearch в true
};

fileSystemVisitor.ExcludeFromList += (sender, path) =>
{
    // Обработчик события ExcludeFromList
    fileSystemVisitor.ExcludeItem(path); // Добавление пути в список исключенных элементов
};


foreach (string item in fileSystemVisitor)
{
    Console.WriteLine(item);
}
