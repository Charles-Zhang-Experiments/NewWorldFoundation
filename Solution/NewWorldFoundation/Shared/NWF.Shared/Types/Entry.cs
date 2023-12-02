namespace NWF.Shared.Types
{
    public enum EntryType
    {
        File,
        Folder
    }
    public record Entry(long Size, DateTime DateModified, string Filename, string Path, EntryType Type)
    {
        public bool IsFolder
            => Type == EntryType.Folder;
        public bool IsFile
            => Type == EntryType.File;

        public override string ToString()
            => $"Name: {Filename}\tSize (B): {(IsFolder ? "(Folder)" : Size)}\tModified: {DateModified:d}\tPath: {Path.Substring(0, Math.Min(Path.Length, 15))}...";
    }
}
