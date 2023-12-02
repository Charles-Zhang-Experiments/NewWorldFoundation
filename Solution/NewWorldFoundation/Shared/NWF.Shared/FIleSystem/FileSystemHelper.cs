using EverythingService;
using NWF.Shared.Types;

namespace NWF.Shared.FIleSystem
{
    public static class FileSystemHelper
    {
        #region Main Methods
        public static IEnumerable<Entry> GetEntries(string query)
        {
            return Everything.Search(query)
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
