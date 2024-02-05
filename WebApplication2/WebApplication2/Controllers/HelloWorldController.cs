using Microsoft.AspNetCore.Mvc;

namespace WebApplication2.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class HelloWorldController : ControllerBase
    {
        [HttpGet(Name = "GetHelloWorld")]
        public Dictionary<string, string> Get()
        {
            return new Dictionary<string, string>() 
            {
                {"Hello", "World" }
            };
        }
    }
    
}
