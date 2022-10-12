using LuNoSqlAssignment.Models;
using Newtonsoft.Json;

namespace LuNoSqlAssignment.Services
{
    public class PersonsFileAccess : IPersonsFileAccess
    {
        private readonly IFileHandling _fileHandling;

        public PersonsFileAccess(IFileHandling fileHandling)
        {
            _fileHandling = fileHandling;
        }

        public void SavePersons(IList<Person> persons, string fileNameWrite)
        {
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string jsonPersonsToFile = JsonConvert.SerializeObject(persons, Formatting.None);

            _fileHandling.Directory = docPath;
            _fileHandling.WriteToDisk(fileNameWrite, jsonPersonsToFile);
        }

        public IList<Person> OpenPersons(string fileName, string path = "")
        {
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            _fileHandling.Directory = path == "" ? docPath : path;

            try
            {
                string jsonPersonsFromFile = _fileHandling.ReadFromDisk(fileName);
                IList<Person>? personsFromFile = JsonConvert.DeserializeObject<List<Person>>(jsonPersonsFromFile);

                if (personsFromFile == null)
                {
                    return null;
                }

                return personsFromFile;
            }
            catch (Exception e)
            {
                throw new FileNotFoundException(e.ToString());
            }
        }
    }
}
