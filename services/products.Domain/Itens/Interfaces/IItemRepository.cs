using products.Domain.Itens.DTOs;
using products.Domain.Itens.Entities;
using products.Domain.Shared;

namespace products.Domain.Itens.Interfaces;

public interface IItemRepository
{
    Task<List<Item>> GetAllAsync();
    Task<NotificationResult> GetByIdAsync(int id);
    Task<NotificationResult> CreateAsync(NewItem item);
    Task<NotificationResult> UpdateAsync(int id, Item item);
    Task<NotificationResult> DeleteAsync(int id);
}