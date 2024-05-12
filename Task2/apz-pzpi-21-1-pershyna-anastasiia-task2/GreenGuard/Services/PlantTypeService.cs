using GreenGuard.Data;
using GreenGuard.Dto;
using GreenGuard.Models.PlantType;
using Microsoft.EntityFrameworkCore;

namespace GreenGuard.Services
{
    public class PlantTypeService
    {
        private readonly GreenGuardDbContext _context;

        public PlantTypeService(GreenGuardDbContext context)
        {
            _context = context;
        }

        public async Task<List<PlantTypeDto>> GetPlantTypes()
        {
            var plantTypes = await _context.Plant_type.Select(data => new PlantTypeDto
            {
                PlantTypeId = data.PlantTypeId,
                PlantTypeName = data.PlantTypeName,
                PlantTypeDescription = data.PlantTypeDescription,
                OptHumidity = data.OptHumidity,
                OptTemp = data.OptTemp,
                OptLight = data.OptLight,
                WaterFreq = data.WaterFreq,
            }).ToListAsync();
            return plantTypes;
        }

        public async Task AddPlantType(AddPlantType model)
        {
            if (await _context.Plant_type.AnyAsync(data => data.PlantTypeName == model.PlantTypeName))
            {
                throw new InvalidOperationException("Plant type with such name already exists");
            }

            var newPlantType = new PlantTypeDto
            {
                PlantTypeName = model.PlantTypeName,
                PlantTypeDescription = model.PlantTypeDescription,
                OptHumidity = model.OptHumidity,
                OptTemp = model.OptTemp,
                OptLight = model.OptLight,
                WaterFreq = model.WaterFreq,
            };

            await _context.Plant_type.AddAsync(newPlantType);
            await _context.SaveChangesAsync();
        }
    }
}

