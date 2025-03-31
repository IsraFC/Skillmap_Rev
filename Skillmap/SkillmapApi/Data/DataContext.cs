using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using SkillmapLib1.Models;

namespace SkillmapApi.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options) 
        {
            
        }

        public DbSet<Role> Roles {  get; set; }
        public DbSet<ResourcesItem> Resources { get; set; }
    }
}
