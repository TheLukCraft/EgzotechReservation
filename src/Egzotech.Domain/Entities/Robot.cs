namespace Egzotech.Domain.Entities;
public class Robot
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Model { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    public Robot(){}

    public Robot(string name, string model)
    {
        Id = Guid.NewGuid();
        Name = name;
        Model = model;
        IsActive = true;
    }
}