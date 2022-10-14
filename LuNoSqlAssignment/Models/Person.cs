namespace LuNoSqlAssignment.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public string? Surname { get; set; }

        public Address? Address { get; set; }
    }
}
