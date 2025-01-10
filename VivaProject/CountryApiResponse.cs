using VivaProject.Controllers;
namespace VivaProject
{
    public class CountryApiResponse
    {
        public CountryName name { get; set; }
        public List<string> capital { get; set; }
        public List<string> borders { get; set; }
    }
}
