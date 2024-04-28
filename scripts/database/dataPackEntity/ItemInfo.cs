using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ColdMint.scripts.inventory;
using Godot;

namespace ColdMint.scripts.database.dataPackEntity;

public class ItemInfo
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Index { get; set; }

    public string Id { get; set; }
    public int Quantity { get; set; }
    public int MaxStackQuantity { get; set; }
    public string? Icon { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string ZipFileName { get; set; }
    
    public string Namespace { get; set; }

    public DateTime CrateTime { get; set; }
}