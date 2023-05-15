using AdvancedCSharp.Task1;

string rootPath = string.Empty;

while (string.IsNullOrWhiteSpace(rootPath))
{
    Console.WriteLine("Please enter the root path: ");
    rootPath = Console.ReadLine() ?? String.Empty;
}

    Console.WriteLine("Please enter the file name or some phrases: ");
    string searchCriteria = Console.ReadLine() ?? String.Empty;

    Console.WriteLine("Please enter extension if it's posible: ");
    string searchExtension = Console.ReadLine() ?? String.Empty;



var fileSystemVisitor = new FileSystemVisitor(rootPath, file =>
{
    string extension = Path.GetExtension(file);
    string fileName = Path.GetFileName(file);

    // Если searchExtension и searchCriteria указаны
    if (!string.IsNullOrEmpty(searchExtension) && !string.IsNullOrEmpty(searchCriteria))
    {
        return (extension == searchExtension) && fileName.Contains(searchCriteria);
    }

    // Если только searchExtension указан
    if (!string.IsNullOrEmpty(searchExtension))
    {
        return extension == searchExtension;
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


foreach (string item in fileSystemVisitor)
{
    Console.WriteLine(item);
}
