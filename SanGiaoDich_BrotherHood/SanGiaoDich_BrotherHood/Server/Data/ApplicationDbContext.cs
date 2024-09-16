using Microsoft.EntityFrameworkCore;

namespace SanGiaoDich_BrotherHood.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }
    }
}
