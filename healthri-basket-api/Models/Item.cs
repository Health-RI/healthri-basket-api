using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace healthri_basket_api.Models;

public class Item
{
    [Key]
    public required string Id { get; init; }

    // For EF Core materialization only.
    private Item()
    {
    }

    [SetsRequiredMembers]
    public Item(string id)
    {
        Id = id;
    }
}
