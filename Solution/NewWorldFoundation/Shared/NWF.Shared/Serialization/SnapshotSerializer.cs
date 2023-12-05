using CsvHelper;
using K4os.Compression.LZ4.Streams;
using NWF.Shared.Types;
using System.Globalization;
using System.Text;

namespace NWF.Shared.Serialization
{
    public static class SnapshotSerializer
    {
        #region Methods
        public static void Save(string filepath, Entry[] entries, bool compressed = true)
        {
            string extension = Path.GetExtension(filepath);
            switch (extension)
            {
                case ".csv":
                    SaveCSV(filepath, entries);
                    break;
                case ".lz4":
                    SaveLZ4(filepath, entries, compressed);
                    break;
                default:
                    throw new ArgumentException($"Invalid extension: {extension}");
            }
        }
        public static Entry[] Load(string filepath, bool compressed = true)
        {
            string extension = Path.GetExtension(filepath);
            return extension switch
            {
                ".csv" => LoadCSV(filepath),
                ".lz4" => LoadLZ4(filepath, compressed),
                _ => throw new ArgumentException($"Invalid extension: {extension}"),
            };
        }
        #endregion

        #region Routines
        private static void SaveCSV(string filepath, Entry[] entries)
        {
            using StreamWriter writer = new(filepath);
            using CsvWriter csv = new(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(entries);
        }
        private static Entry[] LoadCSV(string filepath)
        {
            using StreamReader reader = new(filepath);
            using CsvReader csv = new(reader, CultureInfo.InvariantCulture);
            return csv.GetRecords<Entry>().ToArray();
        }
        private static void SaveLZ4(string filepath, Entry[] entries, bool compressed)
        {
            if (compressed)
            {
                using LZ4EncoderStream stream = LZ4Stream.Encode(File.Create(filepath));
                using BinaryWriter writer = new(stream, Encoding.UTF8, false);
                WriteToStream(writer, entries);
            }
            else
            {
                using FileStream stream = File.Open(filepath, FileMode.Create);
                using BinaryWriter writer = new(stream, Encoding.UTF8, false);
                WriteToStream(writer, entries);
            }
        }
        private static Entry[] LoadLZ4(string filepath, bool compressed)
        {
            if (compressed)
            {
                using LZ4DecoderStream source = LZ4Stream.Decode(File.OpenRead(filepath));
                using BinaryReader reader = new(source, Encoding.UTF8, false);
                return ReadFromStream(reader);
            }
            else
            {
                using FileStream stream = File.Open(filepath, FileMode.Open);
                using BinaryReader reader = new(stream, Encoding.UTF8, false);
                return ReadFromStream(reader);
            }
        }
        #endregion

        #region Secondary Routines
        private static void WriteToStream(BinaryWriter writer, Entry[] data)
        {
            writer.Write(data.Length);
            foreach (Entry d in data)
            {
                writer.Write(d.Size);
                writer.Write(d.DateModified.ToString("yyyy-MM-dd"));
                writer.Write(d.Filename);
                writer.Write(d.Path);
                writer.Write((byte)d.Type);
            }
        }
        private static Entry[] ReadFromStream(BinaryReader reader)
        {
            var count = reader.ReadInt32();
            Entry[] entries = new Entry[count];
            for (int i = 0; i < count; i++)
                entries[i] = new Entry(
                    reader.ReadInt64(),
                    DateTime.Parse(reader.ReadString()),
                    reader.ReadString(),
                    reader.ReadString(),
                    (EntryType)reader.ReadByte()
                );

            return entries;
        }
        #endregion
    }
}
