using backend.Interfaces;
using backend.Models;

namespace backend.Services;

public class SkinService : ISkinService
{
    private readonly IShopRepository<Skin> _repository;

    public SkinService(IShopRepository<Skin> repository)
    {
        _repository = repository;
    }

    public IEnumerable<Skin> GetSkins()
    {
        return _repository.GetAllItems();
    }

    public Skin? GetSkinById(int id)
    {
        return _repository.GetById(id);
    }

    public Skin CreateSkin(Skin skin)
    {
        ValidateSkin(skin);

        skin.Id = 0;
        _repository.Add(skin);

        return skin;
    }

    public Skin? UpdateSkin(int id, Skin skin)
    {
        ValidateSkin(skin);

        var existingSkin = _repository.GetById(id);
        if (existingSkin == null)
        {
            return null;
        }

        existingSkin.Name = skin.Name;
        existingSkin.Price = skin.Price;

        _repository.Update(existingSkin);

        return existingSkin;
    }

    public bool DeleteSkin(int id)
    {
        var existingSkin = _repository.GetById(id);
        if (existingSkin == null)
        {
            return false;
        }

        _repository.Delete(existingSkin);

        return true;
    }

    private static void ValidateSkin(Skin skin)
    {
        if (string.IsNullOrWhiteSpace(skin.Name))
        {
            throw new ArgumentException("Skin name cannot be empty.");
        }

        if (skin.Price < 0)
        {
            throw new ArgumentException("Skin price cannot be negative.");
        }
    }
}
