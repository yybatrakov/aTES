using AuthorizationServer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PopugAccounting.Logic;
using PopugTaskTracker;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
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
        private string GetUserId()
        {
            return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
        }

        [HttpGet]
        [Route("balance")]
        [Authorize(AuthenticationSchemes = PopugTokenScheme.SchemeName)]
        public async Task<BalanceDb> GetBalance()
        {
            return await AccountingLogic.GetBalance(GetUserId());
        }
        [HttpGet]
        [Route("balance_transactions")]
        [Authorize(AuthenticationSchemes = PopugTokenScheme.SchemeName)]
        public async Task<List<BalanceTransactionDb>> GetBalanceTransctions()
        {
            return await AccountingLogic.GetBalanceTransactions(GetUserId());
        }

        [HttpGet]
        [Route("process_payment")]
        [Authorize(AuthenticationSchemes = PopugTokenScheme.SchemeName, Roles = "Admin")]
        public async Task ProcessPayment()
        {
            await AccountingLogic.ProcessPayment();
        }

        [HttpGet]
        [Route("top_management_money_statistics")]
        [Authorize(AuthenticationSchemes = PopugTokenScheme.SchemeName, Roles = "Admin,Accounter")]
        public async Task<List<TopManagementStatisticResponse>> GetTopManagementMoneyStatistics()
        {
            return await AccountingLogic.GetTopManagementMoneyStatistics();
        }
    }
    public class TopManagementStatisticResponse
    {
        public DateTime Date { get; set; }
        public int Money { get; set; }
    }
}
