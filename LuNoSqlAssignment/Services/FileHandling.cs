using System.IO;
using System.Text.Json;

namespace LuNoSqlAssignment.Services
{
    /// <summary>
    /// Provides file handling, i.e. writing and reading.
    /// </summary>
    public class FileHandling : IFileHandling
    {
        /// <summary>
        /// Gets directory for file access.
        /// </summary>
        public string Directory { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileHandling"/> class.
        /// </summary>
        /// <param name="directory">Directory for file access.</param>
        public FileHandling()
        {
            //Directory = directory;
        }

        /// <summary>
        /// Reads file from disk.
        /// </summary>
        /// <param name="file">File name.</param>
        /// <returns>Returns file content.</returns>
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

            }

            return readStream;
        }

        /// <summary>
        /// Writes file to disk.
        /// </summary>
        /// <param name="file">File name.</param>       
        public void WriteToDisk(string file, string input)
        {
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(Directory, file)))
            {
                outputFile.WriteLine(input);
            }
        }
    }
}
