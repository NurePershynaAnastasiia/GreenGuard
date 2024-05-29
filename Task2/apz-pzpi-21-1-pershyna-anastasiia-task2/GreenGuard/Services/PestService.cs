using GreenGuard.Data;
using GreenGuard.Dto;
using GreenGuard.Models.Pest;
using Microsoft.EntityFrameworkCore;

public class PestService
{
    private readonly GreenGuardDbContext _context;
    private readonly ILogger<PestService> _logger;

    public PestService(GreenGuardDbContext context, ILogger<PestService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<PestDto>> GetPests()
    {
        return await _context.Pest.Select(data => new PestDto
        {
            PestId = data.PestId,
            PestName = data.PestName,
            PestDescription = data.PestDescription
        }).ToListAsync();
    }

    public async Task<AddPest> GetPestById(int pestId)
    {
        var pest = await _context.Pest.FindAsync(pestId);
        if (pest == null)
        {
            _logger.LogWarning($"Pest with ID {pestId} not found");
            return null;
        }

        return new AddPest
        {
            PestName = pest.PestName,
            PestDescription = pest.PestDescription
        };
    }

    public async Task AddPest(AddPest model)
    {
        if (await _context.Pest.AnyAsync(data => data.PestName == model.PestName))
        {
            _logger.LogError($"Pest with name '{model.PestName}' already exists");
            throw new InvalidOperationException("Pest with such name already exists");
        }

        var newPest = new PestDto
        {
            PestName = model.PestName,
            PestDescription = model.PestDescription
        };

        await _context.Pest.AddAsync(newPest);
        await _context.SaveChangesAsync();
    }

    public async Task AddPestToPlant(int plantId, int pestId)
    {
        var plant = await _context.Plant.FindAsync(plantId);
        var pest = await _context.Pest.FindAsync(pestId);

        if (plant == null)
        {
            _logger.LogError($"Plant with ID {plantId} not found");
        }

        if (pest == null)
        {
            _logger.LogError($"Pest with ID {pestId} not found");
        }

        var pestInPlant = new PestInPlantDto
        {
            PlantId = plantId,
            PestId = pestId
        };

        _context.Pest_in_Plant.Add(pestInPlant);
        await _context.SaveChangesAsync();
    }

    public async Task DeletePestFromPlant(int plantId, int pestId)
    {
        var pestInPlant = await _context.Pest_in_Plant.FirstOrDefaultAsync(pip => pip.PlantId == plantId && pip.PestId == pestId);


        _context.Pest_in_Plant.Remove(pestInPlant);
        await _context.SaveChangesAsync();
    }
}
