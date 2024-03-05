using AuthorizationServer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PopugTaskTracker;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PopugAnalitics.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnaliticsController : ControllerBase
    {
        public AnaliticsController()
        {
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = PopugTokenScheme.SchemeName, Roles = "Admin")]
        public async Task<List<PopugTask>> Get()
        {
            return new List<PopugTask> { };
        }
    }
}
