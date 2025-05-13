using KOL1.Models.DTOs;

namespace KOL1.Services;

public interface IDbService
{
    Task<DeliverDTO> GetDelivery(int deliveryId);
    Task AddDelivery(DeliveryDTO delivery);
}