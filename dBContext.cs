using AcademiaSys.Models;
using Microsoft.EntityFrameworkCore;

namespace AcademiaSys
{
    public class dBContext : DbContext
    {
        public DbSet<Users> Users { get; set;}
        public DbSet<Students> Students { get; set;}
        public DbSet<Subjects> Subjects { get; set;}    

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("workstation id=AcademiaSys.mssql.somee.com;packet size=4096;user id=skanolkar_SQLLogin_1;pwd=zmbbmgdoe2;data source=AcademiaSys.mssql.somee.com;persist security info=False;initial catalog=AcademiaSys");

            //optionsBuilder.UseSqlServer("Server=SUJAY;Database=AcademiaSys;Integrated Security=true;TrustServerCertificate=true").EnableSensitiveDataLogging(); ;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Students>()
                .ToTable(tb => tb.HasTrigger("InsertSubjectsTrigger"));
        }
    }
}
