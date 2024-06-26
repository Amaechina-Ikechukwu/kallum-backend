using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kallum.Data;
using Kallum.DTOS.Bank;
using Kallum.Extensions;
using Kallum.Helper;
using Kallum.Interfaces;
using Kallum.Models;
using Kallum.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Kallum.Controllers
{
    [ApiController]
    [Route("api/bankoperations")]
    public class BankAccountController : ControllerBase
    {
        public readonly ApplicationDBContext _context;
        public readonly UserManager<AppUser> _userManager;
        public readonly IBankOperationRepository _bankRepository;

        public BankAccountController(ApplicationDBContext context, UserManager<AppUser> userManager, IBankOperationRepository bankOperationnRepository)
        {
            _context = context;
            _userManager = userManager;
            _bankRepository = bankOperationnRepository;
        }
        [HttpGet("accountdetails")]
        [Authorize]
        public async Task<IActionResult> BankDetails()
        {
            try
            {
                var username = User.GetUsername();
                var userInfo = await _userManager.FindByNameAsync(username);
                var userId = userInfo?.Id;
                if (userId == null) return BadRequest("User not found");

                var bankDetails = await _bankRepository.CreateBankAccount(userId);
                return Ok(bankDetails);
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }

        }

        [HttpGet("balance")]
        [Authorize]
        public async Task<IActionResult> GetBankDetails()
        {
            try
            {
                var username = User.GetUsername();
                var bankDetailsResult = await _bankRepository.GetBalanceDetails(username);
                if (bankDetailsResult is null)
                {
                    return BadRequest();
                }
                return Ok(bankDetailsResult);
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }
        [HttpGet("accountdetails/{bankid}")]
        [Authorize]
        public async Task<IActionResult> GetAccountDetail([FromRoute] string bankid)
        {
            try
            {

                var bankDetailsResult = await _bankRepository.GetBankAccountAsync(bankid);
                if (bankDetailsResult is null)
                {
                    return NotFound(new { message = "Bank account not found." });
                }
                return Ok(bankDetailsResult);
            }
            catch (Exception e)
            {
                // Log the exception (not shown here for brevity)
                return StatusCode(500, new { message = "An error occurred while processing your request.", details = e.Message });
            }
        }
        [HttpGet("findkallumuser")]
        [Authorize]
        public async Task<IActionResult> FindUser([FromQuery] FinanceCircleQueryObject query)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var queriedUser = await _bankRepository.FindBankUser(query);
                if (queriedUser is null)
                {
                    return BadRequest();
                }
                return Ok(queriedUser);
            }
            catch (Exception e)
            {
                // Log the exception (not shown here for brevity)
                return StatusCode(500, new { message = "An error occurred while processing your request.", details = e.Message });
            }
        }

    }
}