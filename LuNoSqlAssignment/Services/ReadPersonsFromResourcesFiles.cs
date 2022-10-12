using LuNoSqlAssignment.Models;

using LuNoSqlAssignment.Resources;
using System.Collections;
using System.Globalization;
using System.Resources;

namespace LuNoSqlAssignment.Services
{
    public class PersonsFromResourcesFiles
    {

        public IList<Person> Read()
        {
            List<Person> persons = new List<Person>();

            ResourceSet? resourceSet = ResourcesNames.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);

            if (resourceSet != null)
            {
                foreach (DictionaryEntry item in resourceSet)
                {
                    persons.Add(new Person { Name = (string?)item.Key, Surname = (string?)item.Value });
                }
            }

            ResourceSet? resourceSetAddress = ResourcesAddresses.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);

            if (resourceSetAddress != null)
            {
                int i = 0;

                foreach (DictionaryEntry item in resourceSetAddress)
                {
                    persons[i++].Address = new Address { HouseNumber = (string?)item.Key, Street = (string?)item.Value };
                }
            }

            ResourceSet? resourceSetAddress2 = ResourcesAddresses2.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);

            if (resourceSetAddress2 != null)
            {
                int i = 0;

                foreach (DictionaryEntry item in resourceSetAddress2)
                {
                    if (persons[i].Address != null)
                    {
                        persons[i].Address.City = (string?)item.Key;
                        persons[i++].Address.PostalCode = (string?)item.Value;
                    }
                }
            }

            return persons;
        }
    }
}
