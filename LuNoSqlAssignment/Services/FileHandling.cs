using System.IO;
using System.Text.Json;

namespace LuNoSqlAssignment.Services
{  
    public class FileHandling : IFileHandling
    {
        public string Directory { get; set; } = String.Empty;
                       
        public string ReadFromDisk(string file)
        {
            string readStream = string.Empty;

            try
            {
                using (var sr = new StreamReader(Path.Combine(Directory, file)))
                {
                    readStream = sr.ReadToEnd();
                }
            }
            catch (IOException e)
            {
                throw new Exception(e);
            }

            return readStream;
        }
        
        public void WriteToDisk(string file, string input)
        {
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(Directory, file)))
            {
                outputFile.WriteLine(input);
            }
        }
    }
}
