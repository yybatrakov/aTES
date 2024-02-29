using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PopugTaskTracker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaskController : ControllerBase
    {
        public TaskController()
        {
        }

        [HttpGet]
        public IEnumerable<Task> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new Task
            {
                TaskId = rng.Next(-20, 55),
                Description = $"Do this: {rng.Next(-20, 55)}"
            });
        }
    }
}
