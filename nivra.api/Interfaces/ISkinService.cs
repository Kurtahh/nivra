using backend.Models;

namespace backend.Interfaces;

public interface ISkinService
{
    IEnumerable<Skin> GetSkins();
    Skin? GetSkinById(int id);
    Skin CreateSkin(Skin skin);
    Skin? UpdateSkin(int id, Skin skin);
    bool DeleteSkin(int id);
}
