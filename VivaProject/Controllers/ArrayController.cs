using Microsoft.AspNetCore.Mvc;

namespace VivaProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArrayController : ControllerBase
    {
        // POST: api/array
        [HttpPost]
        public ActionResult<int> GetSecondLargest([FromBody] RequestObj request)
        {
            // Validate input
            if (request?.RequestArrayObj == null || !request.RequestArrayObj.Any())
            {
                return BadRequest("Input array is null or empty.");
            }

            // Find second largest number
            int max = request.RequestArrayObj.Max();
            int? secondLargest = request.RequestArrayObj.Where(x => x < max).DefaultIfEmpty().Max();

            if (secondLargest == null || secondLargest == max)
            {
                return BadRequest("Array must contain at least two distinct integers.");
            }

            return Ok(secondLargest);
        }
    }
}
