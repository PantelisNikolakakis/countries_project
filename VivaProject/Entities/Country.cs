namespace VivaProject.Entities
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Capital { get; set; }
        public string Borders { get; set; } // Stored as a comma-separated string
    }
}
