using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GreenGuard.Data;
using GreenGuard.DTO;
using GreenGuard.Models;
using GreenGuard.Models.Database;


namespace GreenGuard.Controllers
{
    // api/Fertilizers
    [ApiController]
    [Route("api/[controller]")]
    public class WateringController : ControllerBase
    {

        private readonly GreenGuardDbContext _context;
        private readonly ILogger<WateringController> _logger;

        public WateringController(GreenGuardDbContext context, ILogger<WateringController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Tasks/all-tasks
        [HttpGet("all-task")]
        public async Task<IActionResult> CalculateNextWatering(int plantId)
        {
            var plant = _context.Plant
                .Include(p => p.PlantTypeId)
                .FirstOrDefault(p => p.PlantId == plantId);

            if (plant == null)
            {
                // Рослина не знайдена
                return null;
            }

            // Рекомендовані дані для типу рослини
            var recommendedData = plant.PlantTypeId;

            // Пошук останнього Task типу "water" для цієї рослини
            var lastWateringTask = _context.Plant_in_Task
                .Where(pit => pit.PlantId == plantId)
                .OrderByDescending(pit => pit.Task.TaskDate)
                .Select(pit => pit.Task)
                .FirstOrDefault(task => task.TaskType == "watering");

            // Якщо не було жодного поливу раніше або від останнього поливу пройшло більше днів, ніж рекомендована частота поливу
            if (lastWateringTask == null || (DateTime.Now - lastWateringTask.TaskDate).TotalDays >= recommendedData.WaterFreq)
            {
                // Розрахунок інтервалу до наступного поливу в залежності від різниці між поточними та оптимальними значеннями вологості та температури
                var humidityDifference = recommendedData.OptHumidity - plant.Humidity;
                var tempDifference = plant.Temp - recommendedData.OptTemp;

                // Розрахунок коефіцієнтів для визначення інтервалу
                var humidityCoefficient = humidityDifference >= 0 ? 1 / (1 + humidityDifference) : 1 + Math.Abs(humidityDifference);
                var tempCoefficient = tempDifference >= 0 ? 1 / (1 + tempDifference) : 1 + Math.Abs(tempDifference);

                // Розрахунок інтервалу в днях
                var interval = (int)Math.Round((humidityCoefficient + tempCoefficient) * recommendedData.WaterFreq);

                // Перевірка, чи інтервал не менше 1 дня
                interval = Math.Max(1, interval);

                // Додавання інтервалу до дати останнього поливу для отримання дати наступного поливу
                var nextWateringDate = lastWateringTask == null ? DateTime.Now : lastWateringTask.TaskDate.AddDays(interval);

                // Повертаємо об'єкт PlantWateringSchedule з датою наступного поливу
                return new PlantWateringSchedule { Date = nextWateringDate, ActionType = "Watering" };
            }
            else
            {
                return null;
            }
        }

    }
}
