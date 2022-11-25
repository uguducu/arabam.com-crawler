namespace crawler.Models;

public class Cars
{
    public int Id { get; set; }
    public string? Brand { get; set; }
    public string? Series { get; set; }
    public string? Model { get; set; }
    public int? Year { get; set; }
    public int? Kilometer { get; set; }
    public string? GearType { get; set; }
    public string? FuelType { get; set; }
    public string? CaseType { get; set; }
    public string? EngineCapacity{ get; set; }
    public string? MotorPower{ get; set; }
    public string? Colour { get; set; }
    public decimal? Price { get; set; }
    public string? Link { get; set; }
    public string? Source { get; set; }
    public DateTime CreateDate { get; set; }
    public string? AdvertId { get; set; }
}