using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace healthri_basket_api.Models;

public class Item
{
    [Key]
    public required string Id { get; init; }

    public Item()
    {
    }

    [SetsRequiredMembers]
    public Item(string id)
    {
        Id = id;
    }
}
