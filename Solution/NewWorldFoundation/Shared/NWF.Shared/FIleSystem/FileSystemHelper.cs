using EverythingService;
using NWF.Shared.Types;
using System.Linq;

namespace NWF.Shared.FIleSystem
{
    public static class FileSystemHelper
    {
        #region Main Methods
        public static IEnumerable<Entry> GetEntries(string folderPath)
        {
            return GetEntriesWithEverythingService(folderPath);
        }
        public static void Sync(string source, string destination)
        {

        }
        #endregion

        #region Routines
        public static IEnumerable<Entry> GetEntriesNative(string folderPath)
        {
            // Remark-cz: Not reliable
            // TODO: Compare with Everything service results
            IEnumerable<FileInfo> files = new DirectoryInfo(folderPath).EnumerateFiles("*", new EnumerationOptions()
            {
                IgnoreInaccessible = true,
                RecurseSubdirectories = true,
                AttributesToSkip = default,
                ReturnSpecialDirectories = true
            });
            IEnumerable<DirectoryInfo> folders = new DirectoryInfo(folderPath).EnumerateDirectories("*", new EnumerationOptions()
            {
                IgnoreInaccessible = true,
                RecurseSubdirectories = true,
                AttributesToSkip = default,
                ReturnSpecialDirectories = true
            });

            // Yield folders
            foreach (DirectoryInfo folder in folders)
            {
                long folderSize = files.Where(f => f.FullName.StartsWith(folder.FullName + Path.DirectorySeparatorChar)).Sum(f => f.Length);
                yield return new Entry(folderSize, folder.LastWriteTime, folder.Name, folder.FullName, EntryType.Folder);
            }
            // Yield files
            foreach (FileInfo file in files)
            {
                yield return new Entry(file.Length, file.LastWriteTime, file.Name, file.FullName, EntryType.File);
            }
        }
        /// <summary>
        /// Useful only for NTFS volumes and depends on configuration (e.g. exclusion and inclusion, and folder size indexing) of Everything service.
        /// </summary>
        public static IEnumerable<Entry> GetEntriesWithEverythingService(string folderPath)
        {
            return Everything.Search(folderPath.Contains(' ') ? $"\"{folderPath}\"" : folderPath)
                .Select(r => new Entry(r.Size, r.DateModified, r.Filename, r.Path, Enum.Parse<EntryType>(r.Type.ToString())));
        }
        #endregion

        #region Helper
        public static void GetMissingEntries(IEnumerable<Entry> left, IEnumerable<Entry> right, IEnumerable<Entry> missingFromLeft, IEnumerable<Entry> missingFromRight)
        {
            // Remark-cz: Just a quick and dirty implementation.
            // TODO: Can be optimized, especially for large number of files.
            var leftLookUp = left.ToDictionary(l => l.Path, l => l);
            var RightLookUp = right.ToDictionary(r => r.Path, r => r);

            string[] leftPaths = left.Select(l => l.Path).ToArray();
            string[] RightPaths = right.Select(r => r.Path).ToArray();

            IEnumerable<string> missingPathsFromLeft = leftPaths.Except(RightPaths);
            IEnumerable<string> missingPathsFromRight = RightPaths.Except(leftPaths);

            missingFromLeft = missingPathsFromLeft.Select(p => leftLookUp[p]);
            missingFromRight = missingPathsFromRight.Select(p => RightLookUp[p]);
        }
        #endregion
    }
}
