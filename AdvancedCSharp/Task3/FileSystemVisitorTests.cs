using AdvancedCSharp.Task1;
using NUnit.Framework;
using System;
using System.IO;

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
            testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(testDirectory);

            // Create some files and directories for testing
            File.WriteAllText(Path.Combine(testDirectory, "file1.txt"), "Test file 1");
            File.WriteAllText(Path.Combine(testDirectory, "file2.txt"), "Test file 2");
            Directory.CreateDirectory(Path.Combine(testDirectory, "subdirectory1"));
            Directory.CreateDirectory(Path.Combine(testDirectory, "subdirectory2"));
            File.WriteAllText(Path.Combine(testDirectory, "subdirectory1", "file3.txt"), "Test file 3");
            File.WriteAllText(Path.Combine(testDirectory, "subdirectory2", "file4.txt"), "Test file 4");
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
