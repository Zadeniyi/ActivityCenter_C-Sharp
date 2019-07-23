using Microsoft.EntityFrameworkCore;


namespace ActivityCenter.Models
{
    public class MyContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public MyContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users {get;set;}
        public DbSet<NewActivity> NewActivitys {get;set;}
        public DbSet<ActConnect> ActConnects {get;set;}

    }
}