using System;
using System.IO;
using System.Threading;
using AdvancedCSharp.Task1;

//settings
string rootPath = string.Empty;
bool rootPathCheck = false;
string searchCriteria = String.Empty;
string searchExtension = String.Empty;
var fileSystemVisitor = new FileSystemVisitor(rootPath, searchExtension, searchCriteria);

string command = "start";
string commandForList = String.Empty;

while (!command.Equals("exit", StringComparison.OrdinalIgnoreCase))
{
    switch (command)
    {
        case "start":
            rootPath = string.Empty;
            rootPathCheck = false;

            while (string.IsNullOrWhiteSpace(rootPath) || !rootPathCheck)
            {
                Console.WriteLine("Please enter the root path: ");
                rootPath = Console.ReadLine() ?? String.Empty;
                rootPathCheck = Directory.Exists(rootPath);
            }

            Console.WriteLine("Please enter the file name or some phrases: ");
            searchCriteria = Console.ReadLine() ?? String.Empty;

            Console.WriteLine("Please enter extension if it's possible: ");
            searchExtension = Console.ReadLine() ?? String.Empty;


            fileSystemVisitor = new FileSystemVisitor(rootPath, searchExtension, searchCriteria, (file) =>
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
            break;
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
            fileSystemVisitor.ResetAbort();
            break;
        case "list":
            foreach (string item in fileSystemVisitor)
            {
                Console.WriteLine(item);

                if (!Console.KeyAvailable)
                {
                    Thread.Sleep(100);
                }
                else
                {
                    Console.WriteLine("Enter a command (abort or exit):");
                    commandForList = Console.ReadLine()?.Trim() ?? string.Empty;

                    if (commandForList == "abort")
                    {
                        fileSystemVisitor.Abort();
                        break;
                    }
                    if (commandForList == "exit")
                    {
                        command = "exit";
                        break;
                    }
                }
            }
            Console.WriteLine($"Search completed. Found {fileSystemVisitor.FoundFilesCount} files.");
            fileSystemVisitor.ResetAbort();
            break;
        default:
            Console.WriteLine("Invalid command.");
            break;
    }
    if(!command.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("Enter a command (exclude, abort, list, exit, start):");
        command = Console.ReadLine()?.Trim() ?? string.Empty;
    }
}
