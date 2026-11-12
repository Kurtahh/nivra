using System.ComponentModel.DataAnnotations.Schema;
using backend.Interfaces;

namespace backend.Models;
[Table("account")]
public class Account : IShopEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Balance { get; set; }
}