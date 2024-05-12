using GreenGuard.Helpers;
using GreenGuard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace GreenGuard.Controllers.FeaturesControllers
{
    // api/Backups
    [ApiController]
    [Route("api/[controller]")]

    public class BackupsController : ControllerBase
    {
        private readonly BackupService _backupService;

        public BackupsController(BackupService backupService)
        {
            _backupService = backupService;
        }

        /// <summary>
        /// Get the list of available backup files.
        /// </summary>
        /// <returns>
        /// If the retrieval of backup files is successful, it will return a list of backup file names.
        /// If there is an error during the retrieval process, it will return a 500 Internal Server Error.
        /// If the user is not authenticated or does not have sufficient privileges, returns a 401 Unauthorized response.
        /// </returns>
        [Authorize(Roles = Roles.Administrator)]
        [HttpGet("backups")]
        public IActionResult GetBackups()
        {
            try
            {
                var backupFiles = _backupService.GetBackupFiles();
                return Ok(backupFiles);
            }
            catch (Exception ex)
            {
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
        [Authorize(Roles = Roles.Administrator)]
        [HttpPost("add")]
        public IActionResult CreateBackup()
        {
            try
            {
                var backupPath = _backupService.CreateBackup();
                return Ok($"Backup created successfully at {backupPath}");
            }
            catch (Exception ex)
            {
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
        [Authorize(Roles = Roles.Administrator)]
        [HttpPost("restore/{backupFileName}")]
        public IActionResult RestoreBackup(string backupFileName)
        {
            try
            {
                _backupService.RestoreBackup(backupFileName);
                return Ok("Database restored successfully");
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
