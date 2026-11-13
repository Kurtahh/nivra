using backend.Models;

namespace backend.Interfaces;

public interface IAccountService
{
    IEnumerable<Account> GetAccounts();
    Account? GetAccountById(int id);
    Account CreateAccount(Account account);
    Account? UpdateAccount(int id, Account account);
    bool DeleteAccount(int id);
}
