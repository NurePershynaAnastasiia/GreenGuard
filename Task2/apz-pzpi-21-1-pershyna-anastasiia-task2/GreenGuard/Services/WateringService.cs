using GreenGuard.Data;
using GreenGuard.Models.Watering;

public class WateringService
{
    private readonly GreenGuardDbContext _context;
    private readonly ILogger<WateringService> _logger;

    public WateringService(GreenGuardDbContext context, ILogger<WateringService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<WateringSchedule> CalculateNextWateringAsync(int plantId)
    {
        try
        {
            var plant = _context.Plant
            .FirstOrDefault(p => p.PlantId == plantId);

            if (plant == null)
            {
                return null;
            }

            var recommendedData = await _context.Plant_type.FindAsync(plant.PlantTypeId);

            var lastWateringTask = _context.Plant_in_Task
            .Where(pit => pit.PlantId == plantId)
            .Select(pit => pit.TaskId)
            .Distinct()
            .Select(taskId => _context.Task
                .Where(t => t.TaskId == taskId && t.TaskType == "watering")
                .OrderByDescending(t => t.TaskDate)
                .FirstOrDefault())
            .FirstOrDefault();

            if (lastWateringTask == null) {
                return (new WateringSchedule { Date = DateTime.Now, PlantId = plantId });
            }

            var humidityDifference = recommendedData.OptHumidity - plant.Humidity;
            var tempDifference = plant.Temp - recommendedData.OptTemp;
            var daysSinceLastWatering = (DateTime.Now - lastWateringTask.TaskDate).TotalDays;

            var nextWateringDate = lastWateringTask.TaskDate.AddDays(recommendedData.WaterFreq);

            if (daysSinceLastWatering >= recommendedData.WaterFreq || humidityDifference > 0)
            {
                var humidityCoefficient = humidityDifference / recommendedData.OptHumidity * -1;
                var tempCoefficient = tempDifference / recommendedData.OptTemp * -1;

                var interval = (int)Math.Round((decimal)(humidityCoefficient + tempCoefficient) * recommendedData.WaterFreq);

                interval = Math.Max(1, interval);
                nextWateringDate = lastWateringTask == null ? DateTime.Now : lastWateringTask.TaskDate.AddDays(interval);
                nextWateringDate = (nextWateringDate < DateTime.Now)? DateTime.Now : nextWateringDate;
            }
            return (new WateringSchedule { Date = nextWateringDate, PlantId = plantId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during calculating watering schedule");
            throw;
        }
    }
}
