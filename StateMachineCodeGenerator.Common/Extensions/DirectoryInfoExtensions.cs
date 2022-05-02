using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachineCodeGenerator.Common.Extensions
{
    public static class DirectoryInfoExtensions
    {
        public static IEnumerable<DirectoryInfo> GetSiblings(this DirectoryInfo startFolder) {
            var parent = startFolder.Parent;
            var siblings = parent?.EnumerateDirectories();
            return siblings;
        }

        public static FileInfo[] GetFiles(this DirectoryInfo searchFolder, string fileNamePattern) {
            var result = searchFolder.GetFiles(fileNamePattern, searchOption: SearchOption.TopDirectoryOnly);

            return result;
        }

        public static List<FileInfo> GetFilesInSiblings(this DirectoryInfo startFolder, string fileNamePattern) {
            var files = new List<FileInfo>();
            foreach (var dir in startFolder.GetSiblings()) {
                files.AddRange(dir.GetFiles(fileNamePattern));
            }

            return files;
        }

        public static List<FileInfo> FindFirstFilesInAncestors(this DirectoryInfo startFolder, string fileNamePattern, int levelsUp = 3) {
            if (startFolder == null) { throw new Exception(nameof(startFolder)); }
            var files = new List<FileInfo>();
            var parent = startFolder;

            for (int i = 0; i < levelsUp; i++) {
                parent = parent.Parent;
                if (parent == null) { throw new Exception(nameof(startFolder)); }
                files.AddRange(parent.GetFilesInSiblings(fileNamePattern));
                if (files.Any()) { break; }
            }

            return files;
        }

    }
}
