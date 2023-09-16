using System.ComponentModel.DataAnnotations;
using System;
using System.Text.Json.Serialization;

public abstract class BaseTaskDto
{
    private Guid locationId;
    private Guid? majorId;
    private Guid npcId;
    private Guid? itemId;
    private string name;
    private string type;
    private string status;
    private DateTime createdAt;

    [Required]
    public Guid LocationId
    {
        get { return locationId; }
        set { locationId = value; }
    }

    public Guid? MajorId
    {
        get { return majorId; }
        set { majorId = value; }
    }

    [Required]
    public Guid NpcId
    {
        get { return npcId; }
        set { npcId = value; }
    }

    public Guid? ItemId
    {
        get { return itemId; }
        set { itemId = value; }
    }

    [Required]
    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    [Required]
    public string Type
    {
        get { return type; }
        set { type = value; }
    }

    [Required]
    [RegularExpression("^(INACTIVE|ACTIVE)$", ErrorMessage = "Status must be 'INACTIVE' or 'ACTIVE'.")]
    public string Status
    {
        get { return status; }
        set { status = value; }
    }

    [JsonIgnore]

    public DateTime CreatedAt
    {
        get { return createdAt; }
        set { createdAt = value; }
    }
}
