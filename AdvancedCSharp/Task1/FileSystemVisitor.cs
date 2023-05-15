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

        public event EventHandler Start;
        public event EventHandler Finish;
        public event EventHandler<string>? FileFound;
        public event EventHandler<string>? DirectoryFound;
        public event EventHandler<string>? FilteredFileFound;
        public event EventHandler<string>? FilteredDirectoryFound;

        public FileSystemVisitor(string rootPath, Func<string, bool>? filter = null)
        {
            this.rootPath = rootPath;
            this.filter = filter;
            Start = null!;
            Finish = null!;
        }

        public IEnumerator<string> GetEnumerator()
        {
            OnStart();

            foreach (string item in TraverseDirectory(rootPath))
            {
                yield return item;
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
        }

        protected virtual void OnFilteredFileFound(string filePath)
        {
            FilteredFileFound?.Invoke(this, filePath);
        }

        protected virtual void OnFilteredDirectoryFound(string directoryPath)
        {
            FilteredDirectoryFound?.Invoke(this, directoryPath);
        }

        private IEnumerable<string> TraverseDirectory(string directory)
        {
            string[] files = Directory.GetFiles(directory);
            foreach (string file in files)
            {
                OnFileFound(file);

                if (filter == null || filter(file))
                {
                    OnFilteredFileFound(file);
                    yield return file;
                }
            }

            string[] subdirectories = Directory.GetDirectories(directory);
            foreach (string subdirectory in subdirectories)
            {
                OnDirectoryFound(subdirectory);

                foreach (string item in TraverseDirectory(subdirectory))
                {
                    yield return item;
                }
            }
        }
    }
}
