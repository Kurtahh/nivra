using System.ComponentModel.DataAnnotations.Schema;
using backend.Interfaces;

namespace backend.Models;

[Table("account")]
public class Account : IShopEntity
{
    [Column("id")]
    public int Id { get; set; }
    [Column("name")]
    public required string Name { get; set; }
    [Column("balance")]
    public int Balance { get; set; }
}
