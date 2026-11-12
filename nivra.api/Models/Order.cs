using System.ComponentModel.DataAnnotations.Schema;
using backend.Interfaces;

namespace backend.Models;
[Table("order")]
public class Order : IShopEntity
{
    public int Id { get; set; }
    public int Name { get; set; }
    public int Price { get; set; }
}