using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using apz_pzpi_21_1_pershyna_anastasiia_task2.Data;
using apz_pzpi_21_1_pershyna_anastasiia_task2.DTO;
using apz_pzpi_21_1_pershyna_anastasiia_task2.Models;
using apz_pzpi_21_1_pershyna_anastasiia_task2.Models.Database;
using Microsoft.AspNetCore.Identity;

namespace apz_pzpi_21_1_pershyna_anastasiia_task2.Controllers
{
    // api/Workers
    [ApiController]
    [Route("api/[controller]")]
    public class WorkersController : ControllerBase
    {
        private readonly GreenGuardDbContext _context;
        private readonly ILogger<PlantTypesController> _logger;
        private readonly IPasswordHasher<Worker> _passwordHasher;
    }
}
