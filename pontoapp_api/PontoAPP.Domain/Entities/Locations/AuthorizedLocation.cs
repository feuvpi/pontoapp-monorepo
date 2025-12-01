using PontoAPP.Domain.Entities.Common;
using PontoAPP.Domain.Entities.Devices;
using PontoAPP.Domain.Entities.Tenants;

namespace PontoAPP.Domain.Entities.Locations;
public class AuthorizedLocation : BaseEntity
{
    public int TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public int RadiusInMeters { get; set; } = 100; // Raio do geofence
    public bool Active { get; set; } = true;
    public string Notes { get; set; } = string.Empty;
    
    // Relacionamentos
    public Tenant Tenant { get; set; } = null!;
    public ICollection<Device> Devices { get; set; } = new List<Device>();
}
