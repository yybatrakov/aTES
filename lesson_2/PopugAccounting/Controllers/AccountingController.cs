using AuthorizationServer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PopugAccounting.Logic;
using PopugTaskTracker;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PopugAccounting.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountingController : ControllerBase
    {
        public AccountingLogic AccountingLogic { get; }

        public AccountingController(AccountingLogic accountingLogic)
        {
            AccountingLogic = accountingLogic;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = PopugTokenScheme.SchemeName, Roles = "Admin")]
        public async Task ProcessPayment()
        {
            await AccountingLogic.ProcessPayment();
        }


    }
}
