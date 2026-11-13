using backend.Interfaces;
using backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase, IAccountController

{
    private readonly IAccountService _service;

    public AccountController(IAccountService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetAccounts()
    {
        return Ok(_service.GetAccounts());
    }

    [HttpGet("{id:int}")]
    public IActionResult GetAccountById(int id)
    {
        var account = _service.GetAccountById(id);
        if (account == null)
        {
            return NotFound(new { message = "Account not found." });
        }

        return Ok(account);
    }

    [HttpPost]
    public IActionResult CreateAccount([FromBody] Account account)
    {
        try
        {
            var createdAccount = _service.CreateAccount(account);
            return CreatedAtAction(nameof(GetAccountById), new { id = createdAccount.Id }, createdAccount);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("{id:int}")]
    public IActionResult UpdateAccount(int id, [FromBody] Account account)
    {
        try
        {
            var updatedAccount = _service.UpdateAccount(id, account);
            if (updatedAccount == null)
            {
                return NotFound(new { message = "Account not found." });
            }

            return Ok(updatedAccount);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeleteAccount(int id)
    {
        if (!_service.DeleteAccount(id))
        {
            return NotFound(new { message = "Account not found." });
        }

        return NoContent();
    }
}
