using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace GreenGuard.Controllers.FeaturesControllers
{
    // api/Backups
    [ApiController]
    [Route("api/[controller]")]

    public class BackupsController : ControllerBase
    {
        private readonly ILogger<BackupsController> _logger;
        private readonly string _connectionString;
        private readonly string _backupDirectory = "B:\\";

        public BackupsController(ILogger<BackupsController> logger)
        {
            _logger = logger;
            _connectionString = "Server=DESKTOP-D0GBIS9;Database=GreenGuard;Trusted_Connection=True;TrustServerCertificate=True;";
        }

        private SqlConnection CreateAndOpenConnection()
        {
            var sqlConnection = new SqlConnection(_connectionString);
            sqlConnection.Open();
            return sqlConnection;
        }

        private string GetBackupFilePath(string databaseName)
        {
            return Path.Combine(_backupDirectory, $"{databaseName}_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.bak");
        }

        /// <summary>
        /// Get the list of available backup files.
        /// </summary>
        /// <returns>
        /// If the retrieval of backup files is successful, it will return a list of backup file names.
        /// If there is an error during the retrieval process, it will return a 500 Internal Server Error.
        /// </returns>
        [HttpGet("backups")]
        public IActionResult GetBackups()
        {
            try
            {
                var backupFiles = Directory.GetFiles(_backupDirectory, "*.bak")
                    .Select(Path.GetFileName)
                    .ToList();

                return Ok(backupFiles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching backup files");
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Create a backup of the database.
        /// </summary>
        /// <returns>
        /// If the backup is successful, it will return a message confirming the backup creation and the file path where the backup is saved.
        /// If there is an error during the backup process, it will return a 500 Internal Server Error.
        /// </returns>
        [HttpPost("add")]
        public IActionResult CreateBackup()
        {
            try
            {
                using (var sqlConnection = CreateAndOpenConnection())
                {
                    string databaseName = sqlConnection.Database;
                    string backupPath = GetBackupFilePath(databaseName);

                    using (var sqlCommand = new SqlCommand($"BACKUP DATABASE [{databaseName}] TO DISK='{backupPath}' WITH FORMAT", sqlConnection))
                    {
                        sqlCommand.ExecuteNonQuery();
                    }

                    return Ok($"Backup created successfully at {backupPath}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a backup");
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Restore the database from a specified backup file.
        /// </summary>
        /// <param name="backupFileName">The name of the backup file to restore.</param>
        /// <returns>
        /// If the restoration is successful, it will return a message confirming the database restoration from the specified backup file.
        /// If the specified backup file is not found, it will return a 404 Not Found error.
        /// If there is an error during the restoration process, it will return a 500 Internal Server Error.
        /// </returns>
        [HttpPost("restore/{backupFileName}")]
        public IActionResult RestoreBackup(string backupFileName)
        {
            try
            {
                using (var sqlConnection = CreateAndOpenConnection())
                {
                    string backupFilePath = Path.Combine(_backupDirectory, backupFileName);

                    if (!System.IO.File.Exists(backupFilePath))
                    {
                        return NotFound("Backup file not found");
                    }

                    string restoreQuery = $"USE master; RESTORE DATABASE VetCare_db FROM DISK = '{backupFilePath}' WITH REPLACE;";

                    using (SqlCommand sqlCommand = new SqlCommand(restoreQuery, sqlConnection))
                    {
                        sqlCommand.ExecuteNonQuery();
                    }

                    return Ok($"Database restored successfully from {backupFilePath}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while restoring the database");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
