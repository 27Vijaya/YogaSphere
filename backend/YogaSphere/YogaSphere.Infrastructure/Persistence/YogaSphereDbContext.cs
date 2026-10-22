using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using YogaSphere.Domain.Entities;

namespace YogaSphere.Infrastructure.Persistence
{
    public class YogaSphereDbContext :DbContext
    {
        //DbContextOptions: supplies configuration, such as which database provider to use.
        public YogaSphereDbContext(DbContextOptions<YogaSphereDbContext> options) : base(options) 
        { }

        //represents the collection of users that EF Core can query and persist.
        //Set<TEntity>() provides the entity set without requiring separate property initialization.
        public DbSet<User> Users => Set<User>();
        public DbSet<CustomerProfile> CustomerProfiles => Set<CustomerProfile>();
        public DbSet<TrainerProfile> TrainerProfiles => Set<TrainerProfile>();

        public DbSet<YogaProgram> YogaPrograms => Set<YogaProgram>();

        public DbSet<YogaClass> YogaClasses => Set<YogaClass>();

        public DbSet<Schedule> Schedules => Set<Schedule>();
        public DbSet<Booking> Bookings => Set<Booking>();

        public DbSet<Payment> Payments => Set<Payment>();

        public DbSet<Review> Reviews => Set<Review>();

        public DbSet<Notification> Notifications => Set<Notification>();
    }
}
