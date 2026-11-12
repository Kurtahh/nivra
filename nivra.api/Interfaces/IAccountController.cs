using backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend.Interfaces;

public interface IAccountController
{
    IActionResult GetAccounts();
    IActionResult GetAccountById(int id);
    IActionResult CreateAccount(Account account);
    IActionResult UpdateAccount(int id, Account account);
    IActionResult DeleteAccount(int id);
}
