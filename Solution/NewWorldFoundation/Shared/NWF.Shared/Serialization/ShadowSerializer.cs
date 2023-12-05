using NWF.Shared.FIleSystem;
using NWF.Shared.Types;

namespace NWF.Shared.Serialization
{
    /// <summary>
    /// Creates a shadow copy of source folder;
    /// Caller should check whether destination is in clean state before invoking this.
    /// </summary>
    public static class ShadowSerializer
    {
        #region Methods
        /// <summary>
        /// Returns created entries
        /// </summary>
        public static Entry[] MakeShadowCopy(string sourceFolder, string destinationFolder, string[] fileTypesFilter = null, int fileSizeThresholdInMb = 1)
        {
            if (!Directory.Exists(sourceFolder))
                throw new ArgumentException($"Invalid directory: {sourceFolder}");

            var sourceEntries = FileSystemHelper.GetEntries(sourceFolder);
            var destinationEntries = FileSystemHelper.GetEntries(destinationFolder);
            fileTypesFilter ??= [".txt", ".md", ".cs"];
            List<Entry> created = [];

            // Duplicate folder structure
            foreach (Entry directory in sourceEntries.Where(e => e.IsFolder)) // TODO: Why on earth this iteration is not working?
            {
                string relativePath = Path.GetRelativePath(sourceFolder, directory.Path);
                if (!destinationEntries.Any(e => Path.GetRelativePath(destinationFolder, e.Path) == relativePath))
                {
                    var newDirectory = Path.Combine(destinationFolder, relativePath);
                    Directory.CreateDirectory(newDirectory);
                    created.Add(new Entry(0, DateTime.Now, Path.GetFileName(directory.Filename), newDirectory, EntryType.Folder));
                }
                Console.WriteLine(relativePath);
            }

            // Duplicate files
            foreach (Entry file in sourceEntries.Where(e => e.IsFile && fileTypesFilter.Contains(Path.GetExtension(e.Filename).ToLower())))
            {
                string relativePath = Path.GetRelativePath(sourceFolder, file.Path);
                var newPath = Path.Combine(destinationFolder, relativePath);

                File.Copy(file.Path, newPath);
                created.Add(new Entry(file.Size, file.DateModified, file.Filename, newPath, EntryType.File));
            }

            return created.ToArray();
        }
        #endregion
    }
}
