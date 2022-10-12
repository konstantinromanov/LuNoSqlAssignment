using LuNoSqlAssignment.Models;

namespace LuNoSqlAssignment.Services
{
    public interface IPersonsFileAccess
    {
        IList<Person> OpenPersons(string fileName);
        void SavePersons(IList<Person> persons, string fileNameWrite);
    }
}
