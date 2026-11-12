using System.ComponentModel.DataAnnotations.Schema;
using backend.Interfaces;

namespace backend.Models;

[Table("skin")]
public class Skin : IShopEntity
{
    public int Id { get; set; }
    public int Name { get; set; }
    public int Price { get; set; }
}