using EverythingService;
using NWF.Shared.Types;

namespace NWF.Shared.FIleSystem
{
    public static class FileSystemHelper
    {
        #region Main Methods
        public static IEnumerable<Entry> GetEntries(string folderPath)
        {
            return Everything.Search(folderPath.Contains(' ') ? $"\"{folderPath}\"": folderPath)
                .Select(r => new Entry(r.Size, r.DateModified, r.Filename, r.Path, Enum.Parse<EntryType>(r.Type.ToString())));
        }
        public static void Sync(string source, string destination)
        {

        }
        #endregion

        #region Routines

        #endregion
    }
}
