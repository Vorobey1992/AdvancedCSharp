using AdvancedCSharp.Task1;
using NUnit.Engine.Internal.Backports;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using static System.Net.WebRequestMethods;

namespace AdvancedCSharp.Task3
{
    [TestFixture]
    public class FileSystemVisitorTests
    {
        private string testDirectory = string.Empty;

        [SetUp]
        public void Setup()
        {
            // Create a temporary test directory
            testDirectory = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(testDirectory);

            // Create some files and directories for testing
            System.IO.File.WriteAllText(System.IO.Path.Combine(testDirectory, "file1.txt"), "Test file 1");
            System.IO.File.WriteAllText(System.IO.Path.Combine(testDirectory, "file2.txt"), "Test file 2");
            Directory.CreateDirectory(System.IO.Path.Combine(testDirectory, "subdirectory1"));
            Directory.CreateDirectory(System.IO.Path.Combine(testDirectory, "subdirectory2"));
            System.IO.File.WriteAllText(System.IO.Path.Combine(testDirectory, "subdirectory1", "file3.txt"), "Test file 3");
            System.IO.File.WriteAllText(System.IO.Path.Combine(testDirectory, "subdirectory2", "file4.txt"), "Test file 4");
        }

        [TearDown]
        public void TearDown()
        {
            // Delete the test directory and its contents
            if (!string.IsNullOrEmpty(testDirectory))
            {
                Directory.Delete(testDirectory, true);
            }
        }

        [Test]
        public void FileSystemVisitor_ReturnsAllFilesAndDirectories()
        {
            var fileSystemVisitor = new FileSystemVisitor(testDirectory, string.Empty, string.Empty);

            var results = fileSystemVisitor.GetEnumerator();

            // Verify that all files and directories are returned
            Assert.That(GetCount(results), Is.EqualTo(4));
        }

        [Test]
        public void FileSystemVisitor_FiltersByExtension()
        {
            var fileSystemVisitor = new FileSystemVisitor(testDirectory, ".txt", string.Empty);

            var results = fileSystemVisitor.GetEnumerator();

            // Verify that only files with .txt extension are returned
            Assert.That(GetCount(results), Is.EqualTo(4));
        }

        [Test]
        public void FileSystemVisitor_FiltersByName()
        {
            var fileSystemVisitor = new FileSystemVisitor(testDirectory, string.Empty, "file1.txt");

            var results = fileSystemVisitor.GetEnumerator();

            // Verify that only files with the specified name are returned
            Assert.That(GetCount(results), Is.EqualTo(1));
        }

        [Test]
        public void FileSystemVisitor_FiltersByExtensionAndName()
        {
            var fileSystemVisitor = new FileSystemVisitor(testDirectory, ".txt", "file1.txt");

            var results = fileSystemVisitor.GetEnumerator();

            // Verify that only files with the specified extension and name are returned
            Assert.That(GetCount(results), Is.EqualTo(1));
        }

        [Test]
        public void FileSystemVisitor_ExcludeFile()
        {
            string exclude = string.Concat(testDirectory,"\\", "file1.txt");

            var fileSystemVisitor = new FileSystemVisitor(testDirectory, string.Empty, string.Empty);

            fileSystemVisitor.ExcludeItem(exclude);

            var results = fileSystemVisitor.GetEnumerator();

            Assert.That(GetCount(results), Is.EqualTo(3));
        }

        [Test]
        public void FileSystemVisitor_ExcludeFolder()
        {
            string exclude = string.Concat(testDirectory,"\\","subdirectory1");

            var fileSystemVisitor = new FileSystemVisitor(testDirectory, string.Empty, string.Empty);

            fileSystemVisitor.ExcludeItem(exclude);

            var results = fileSystemVisitor.GetEnumerator();

            Assert.That(GetCount(results), Is.EqualTo(3));
        }

        [Test]
        public void FileSystemVisitor_Abort()
        {
            var fileSystemVisitor = new FileSystemVisitor(testDirectory, ".txt", "file1.txt");

            fileSystemVisitor.FileFound += (sender, path) => { fileSystemVisitor.Abort(); };

            var results = fileSystemVisitor.GetEnumerator();

            Assert.That(GetCount(results), Is.EqualTo(1));
        }

        private static int GetCount(IEnumerator<string> enumerator)
        {
            int count = 0;
            while (enumerator.MoveNext())
            {
                count++;
            }
            return count;
        }

    }
}
