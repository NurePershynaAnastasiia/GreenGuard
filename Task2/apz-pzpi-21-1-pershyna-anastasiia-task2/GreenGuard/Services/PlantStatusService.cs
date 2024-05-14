using GreenGuard.Data;
using GreenGuard.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GreenGuard.Services
{
    public class PlantStatusService
    {
        private readonly GreenGuardDbContext _context;

        public PlantStatusService(GreenGuardDbContext context)
        {
            _context = context;
        }

        public async Task UpdatePlantStatus()
        {
            int tempDeviation = 3;
            int lightDeviation = 100;
            int humidityDeviation = 5;

            var plants = await _context.Plant.ToListAsync();

            foreach (var plant in plants)
            {
                var plantType = await _context.Plant_type.FirstOrDefaultAsync(pt => pt.PlantTypeId == plant.PlantTypeId);

                if (plantType != null)
                {
                    string status = "";

                    if (plant.Temp + tempDeviation < plantType.OptTemp)
                        status = "Температура занизька";
                    else if (plant.Temp - tempDeviation > plantType.OptTemp)
                        status = "Температура зависока";
                    else 
                        status = "Температура в нормі";

                    if (plant.Light + lightDeviation < plantType.OptLight)
                        status += ", Рівень освітлення занизький";
                    else if (plant.Light - lightDeviation > plantType.OptLight)
                        status += ", Рівень освітлення зависокий";
                    else 
                        status += ", Рівень освітлення в нормі";

                    if (plant.Humidity + humidityDeviation < plantType.OptHumidity)
                        status += ", Вологість занизька";
                    else if (plant.Humidity - humidityDeviation > plantType.OptHumidity)
                        status += ", Вологість зависока";
                    else
                        status += ", Вологість в нормі";

                    plant.PlantState = status;
                }
                else
                {
                    plant.PlantState = "Не знайдено рекомендацій для цього типу рослини";
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
