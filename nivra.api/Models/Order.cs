using System.ComponentModel.DataAnnotations.Schema;
using backend.Interfaces;

namespace backend.Models;

[Table("order")]
public class Order : IShopEntity
{
    [Column("id")]
    public int Id { get; set; }
    [Column("name")]
    public required string Name { get; set; }
    [Column("price")]
    public int Price { get; set; }
}
