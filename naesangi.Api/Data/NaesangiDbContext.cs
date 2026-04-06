using Microsoft.EntityFrameworkCore;

namespace naesangi.Api.Data;

public sealed class NaesangiDbContext : DbContext
{
    public NaesangiDbContext(DbContextOptions<NaesangiDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        //
    }
}
