using GreenGuard.Data;
using GreenGuard.Dto;
using GreenGuard.Models.Fertilizer;
using Microsoft.EntityFrameworkCore;

public class FertilizerService
{
    private readonly GreenGuardDbContext _context;

    public FertilizerService(GreenGuardDbContext context)
    {
        _context = context;
    }

    public async Task<List<FertilizerDto>> GetFertilizers()
    {
        var fertilizers = await _context.Fertilizer.Select(data => new FertilizerDto
        {
            FertilizerId = data.FertilizerId,
            FertilizerName = data.FertilizerName,
            FertilizerQuantity = data.FertilizerQuantity
        }).ToListAsync();
        return fertilizers;
    }

    public async Task<AddFertilizer> GetFertilizerById(int id)
    {
        var fertilizer = await _context.Fertilizer.FindAsync(id);
        if (fertilizer == null)
        {
            return null;
        }

        return new AddFertilizer
        {
            FertilizerName = fertilizer.FertilizerName,
            FertilizerQuantity = fertilizer.FertilizerQuantity
        };
    }

    public async Task AddFertilizer(AddFertilizer model)
    {
        var newFertilizer = new FertilizerDto
        {
            FertilizerName = model.FertilizerName,
            FertilizerQuantity = model.FertilizerQuantity
        };

        _context.Add(newFertilizer);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateFertilizerQuantity(int id, UpdateFertilizerQuantity model)
    {
        var fertilizer = await _context.Fertilizer.FindAsync(id);
        if (fertilizer != null)
        {
            fertilizer.FertilizerQuantity = model.FertilizerQuantity;
            _context.Update(fertilizer);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteFertilizer(int id)
    {
        var fertilizer = await _context.Fertilizer.FindAsync(id);
        if (fertilizer != null)
        {
            _context.Fertilizer.Remove(fertilizer);
            await _context.SaveChangesAsync();
        }
    }
}
