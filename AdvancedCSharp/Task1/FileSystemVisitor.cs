using NUnit.Engine.Internal.Backports;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace AdvancedCSharp.Task1
{
    public class FileSystemVisitor : IEnumerable<string>
    {
        private readonly string rootPath;
        private readonly Func<string, bool>? filter;
        private readonly string searchExtension;
        private readonly string searchCriteria;

        private bool shouldAbortSearch;
        private readonly HashSet<string> excludedItems;

        public event EventHandler Start;
        public event EventHandler Finish;
        public event EventHandler<string>? FileFound;
        public event EventHandler<string>? DirectoryFound;
        public event EventHandler<string>? FilteredFileFound;
        public event EventHandler<string>? FilteredDirectoryFound;

        public int FoundFilesCount { get; private set; }

        public FileSystemVisitor(string rootPath, string searchExtension, string searchCriteria, Func<string, bool>? filter = null)
        {
            this.rootPath = rootPath;
            this.filter = filter;
            this.searchExtension = searchExtension;
            this.searchCriteria = searchCriteria;
            excludedItems = new HashSet<string>();
            Start = null!;
            Finish = null!;
        }

        public IEnumerator<string> GetEnumerator()
        {
            OnStart();

            foreach (string item in TraverseDirectory(rootPath, searchExtension, searchCriteria))
            {
                if (ShouldExcludeItem(item))
                {
                    continue;
                }

                yield return item;

                if (ShouldAbortSearch())
                {
                    yield break;
                }
            }

            OnFinish();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected virtual void OnStart()
        {
            Start?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnFinish()
        {
            Finish?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnFileFound(string filePath)
        {
            FileFound?.Invoke(this, filePath);
        }

        protected virtual void OnDirectoryFound(string directoryPath)
        {
            DirectoryFound?.Invoke(this, directoryPath);
            if (filter != null)
            {
                bool filtered = filter(directoryPath);

                if (filtered)
                {
                    OnFilteredDirectoryFound(directoryPath);
                }
            }
        }

        protected virtual void OnFilteredFileFound(string filePath)
        {
            FilteredFileFound?.Invoke(this, filePath);
            FoundFilesCount++;
        }

        protected virtual void OnFilteredDirectoryFound(string directoryPath)
        {
            FilteredDirectoryFound?.Invoke(this, directoryPath);
        }

        private IEnumerable<string> TraverseDirectory(string directory, string searchExtension, string searchCriteria)
        {
            string[] files = Directory.GetFiles(directory);
            foreach (string file in files)
            {
                string extension = System.IO.Path.GetExtension(file);
                string fileName = System.IO.Path.GetFileName(file);

                bool matchExtension = string.IsNullOrEmpty(searchExtension) || extension == searchExtension;
                bool matchCriteria = string.IsNullOrEmpty(searchCriteria) || fileName.Contains(searchCriteria);

                bool shouldExclude = ShouldExcludeItem(file);

                if (matchExtension && matchCriteria && !shouldExclude)
                {
                    OnFilteredFileFound(file);
                    yield return file;
                }
            }

            string[] subdirectories = Directory.GetDirectories(directory);
            foreach (string subdirectory in subdirectories)
            {
                bool shouldExclude = ShouldExcludeItem(subdirectory);
                
                if (!shouldExclude)
                {
                    OnDirectoryFound(subdirectory);

                    if (Console.KeyAvailable)
                    {
                        yield return subdirectory;
                    }

                        foreach (string item in TraverseDirectory(subdirectory, searchExtension, searchCriteria))
                    {
                        yield return item;
                    }
                }
            }
        }

        private bool ShouldAbortSearch()
        {
            return shouldAbortSearch;
        }

        private bool ShouldExcludeItem(string path)
        {
            if (excludedItems.Contains(path))
            {
                return true;
            }
            return false;
        }

        public void Abort()
        {
            shouldAbortSearch = true;
        }
        public void ResetAbort()
        {
            shouldAbortSearch = false;
        }

        public void ExcludeItem(string path)
        {
            excludedItems.Add(path);
        }
    }
}
