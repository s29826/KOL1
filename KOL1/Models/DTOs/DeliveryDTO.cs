namespace KOL1.Models.DTOs;

public class DeliveryDTO
{
    public int deliveryId { get; set; }
    public int countromerId { get; set; }
    public string licenceNumber { get; set; }
    public List<ProductDTO> products { get; set; }
}