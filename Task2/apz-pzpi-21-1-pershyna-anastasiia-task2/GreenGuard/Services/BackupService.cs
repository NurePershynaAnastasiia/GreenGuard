using Microsoft.Data.SqlClient;

namespace GreenGuard.Services
{
    public class BackupService
    {
        private readonly ILogger<BackupService> _logger;
        private readonly string _connectionString;
        private readonly string _backupDirectory = "D:\\";

        public BackupService(ILogger<BackupService> logger)
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

        public IEnumerable<string> GetBackupFiles()
        {
            try
            {
                var backupFiles = Directory.GetFiles(_backupDirectory, "*.bak")
                    .Select(Path.GetFileName)
                    .ToList();

                return backupFiles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching backup files");
                throw;
            }
        }

        public string CreateBackup()
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

                    return backupPath;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a backup");
                throw;
            }
        }

        public void RestoreBackup(string backupFileName)
        {
            try
            {
                using (var sqlConnection = CreateAndOpenConnection())
                {
                    sqlConnection.ChangeDatabase("master");

                    string backupFilePath = Path.Combine(_backupDirectory, backupFileName);

                    if (!System.IO.File.Exists(backupFilePath))
                    {
                        throw new FileNotFoundException("Backup file not found");
                    }

                    string restoreQuery = $"RESTORE DATABASE GreenGuard FROM DISK = '{backupFilePath}' WITH REPLACE;";
                    using (SqlCommand sqlCommand = new SqlCommand(restoreQuery, sqlConnection))
                    {
                        sqlCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while restoring the database");
                throw;
            }
        }
    }

}
