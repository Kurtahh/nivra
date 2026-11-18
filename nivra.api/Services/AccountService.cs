using backend.Interfaces;
using backend.Models;

namespace backend.Services;

public class AccountService : IAccountService
{
    private readonly IShopRepository<Account> _repository;

    public AccountService(IShopRepository<Account> repository)
    {
        _repository = repository;
    }

    public IEnumerable<Account> GetAccounts()
    {
        return _repository.GetAllItems();
    }

    public Account? GetAccountById(int id)
    {
        return _repository.GetById(id);
    }

    public Account CreateAccount(Account account)
    {
        ValidateAccount(account);

        account.Id = 0;
        _repository.Add(account);

        return account;
    }

    public Account? UpdateAccount(int id, Account account)
    {
        ValidateAccount(account);

        var existingAccount = _repository.GetById(id);
        if (existingAccount == null)
        {
            return null;
        }

        existingAccount.Name = account.Name;
        existingAccount.Balance = account.Balance;

        _repository.Update(existingAccount);

        return existingAccount;
    }

    public bool DeleteAccount(int id)
    {
        var existingAccount = _repository.GetById(id);
        if (existingAccount == null)
        {
            return false;
        }

        _repository.Delete(existingAccount);

        return true;
    }

    private static void ValidateAccount(Account account)
    {
        if (string.IsNullOrWhiteSpace(account.Name))
        {
            throw new ArgumentException("Account name cannot be empty.");
        }

        if (account.Balance < 0)
        {
            throw new ArgumentException("Account balance cannot be negative.");
        }
    }
}
