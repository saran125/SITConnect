using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace SITConnect.Models
{
    public class SITConnectDBcontext : DbContext
    {

        private readonly IConfiguration _config;
        public SITConnectDBcontext(IConfiguration configuration)
        {
            _config = configuration;
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = _config.GetConnectionString("MyConn");
            optionsBuilder.UseSqlServer(connectionString);
        }
        public DbSet<User> Users { get; set; }

    }
}
