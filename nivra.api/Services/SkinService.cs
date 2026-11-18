using backend.Interfaces;
using backend.Models;

namespace backend.Services;

public class SkinService : ISkinService
{
    private readonly IShopRepository<Skin> _repository;
    private readonly IShopRepository<Order> _orderRepository;
    public SkinService(IShopRepository<Skin> repository, IShopRepository<Order> orderRepository)
    {
        _repository = repository;
        _orderRepository = orderRepository;
    }

    public IEnumerable<Skin> GetSkins()
    {
        return _repository.GetAllItems();
    }

    public Skin? GetSkinById(int id)
    {
        return _repository.GetByIdNullable(id);
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

        var existingSkin = _repository.GetByIdNullable(id);
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
        var existingSkin = _repository.GetByIdNullable(id);
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

    public string GetEquippedSkinName(long id)
    {
        Order? latestEquip = _orderRepository.GetByPrefix($"equip:{id}:");
        int skinId;
        if (latestEquip is not null){
            int indexOfLastColon = latestEquip.Name.LastIndexOf(':');
            string textSkinId = latestEquip.Name.Substring(indexOfLastColon+1);
            skinId = int.Parse(textSkinId);
        }
        else
            skinId = 3; //default skin

        return _repository.GetById(skinId).Name;
    }
}
