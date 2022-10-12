namespace LuNoSqlAssignment.Services
{
    public interface IFileHandling
    {
        public string Directory { get; set; }
        string ReadFromDisk(string file);
        void WriteToDisk(string file, string input);
    }
}
