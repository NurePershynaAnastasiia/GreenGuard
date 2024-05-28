using GreenGuard.Data;
using GreenGuard.Dto;
using GreenGuard.Models.Plant;
using Microsoft.EntityFrameworkCore;

namespace GreenGuard.Services
{
    public class PlantService
    {
        private readonly GreenGuardDbContext _context;
        private readonly ILogger<PlantService> _logger;

        public PlantService(GreenGuardDbContext context, ILogger<PlantService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<PlantFull>> GetPlants()
        {
            try
            {
                var plants = await _context.Plant.Select(data => new PlantFull
                {
                    PlantId = data.PlantId,
                    PlantTypeName = _context.Plant_type
                        .Where(pt => pt.PlantTypeId == data.PlantTypeId)
                        .Select(pt => pt.PlantTypeName)
                        .FirstOrDefault(),
                    PlantLocation = data.PlantLocation,
                    Humidity = data.Humidity,
                    Temp = data.Temp,
                    Light = data.Light,
                    AdditionalInfo = data.AdditionalInfo,
                    PlantState = data.PlantState,
                    Pests = _context.Pest_in_Plant
                        .Where(pip => pip.PlantId == data.PlantId)
                        .Select(pip => _context.Pest.FirstOrDefault(p => p.PestId == pip.PestId).PestName)
                        .ToList()
                }).ToListAsync();

                return plants;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting plants");
                throw;
            }
        }

        public async Task AddNewPlant(AddPlant model)
        {
            try
            {
                var newPlant = new PlantDto
                {
                    PlantTypeId = model.PlantTypeId,
                    PlantLocation = model.PlantLocation,
                    Humidity = model.Humidity,
                    Light = model.Light,
                    Temp = model.Temp,
                    AdditionalInfo = model.AdditionalInfo,
                };

                _context.Add(newPlant);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new plant");
                throw;
            }
        }
        public async Task UpdatePlant(int id, UpdatePlant model)
        {
            try
            {
                var plant = await _context.Plant.FindAsync(id);
                if (plant == null)
                {
                    _logger.LogError($"Plant with ID {id} not found");
                    throw new Exception($"Plant with ID {id} not found");
                }

                plant.PlantLocation = model.PlantLocation;
                plant.Humidity = model.Humidity;
                plant.Light = model.Light;
                plant.Temp = model.Temp;
                plant.AdditionalInfo = model.AdditionalInfo;
                plant.PlantState = model.PlantState;

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating plant");
                throw;
            }
        }

        public async Task UpdatePlantState(int id, UpdatePlantState model)
        {
            try
            {
                var plant = await _context.Plant.FindAsync(id);
                if (plant == null)
                {
                    _logger.LogError($"Plant with ID {id} not found");
                    throw new Exception($"Plant with ID {id} not found");
                }

                plant.PlantState = model.PlantState;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating plant state");
                throw;
            }
        }

        public async Task DeletePlant(int id)
        {
            try
            {
                var plant = await _context.Plant.FindAsync(id);
                if (plant == null)
                {
                    _logger.LogError($"Plant with ID {id} not found");
                    throw new Exception($"Plant with ID {id} not found");
                }

                _context.Plant.Remove(plant);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting plant");
                throw;
            }
        }
    }
}
