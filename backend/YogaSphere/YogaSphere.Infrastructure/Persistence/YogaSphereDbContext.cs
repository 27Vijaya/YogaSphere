using Microsoft.EntityFrameworkCore;
using YogaSphere.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

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


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(user => user.Email)
                .IsUnique();

            // Configure a one-to-one relationship between User and CustomerProfile.
            modelBuilder.Entity<CustomerProfile>()
                .HasOne(profile => profile.User)
                .WithOne()
                .HasForeignKey<CustomerProfile>(profile => profile.UserId);

            // Configure the one-to-one relationship between TrainerProfile and User.
            modelBuilder.Entity<TrainerProfile>()
                .HasOne(profile => profile.User)
                .WithOne()
                .HasForeignKey<TrainerProfile>(profile => profile.UserId);

            // Configure the relationship between a trainer profile and its yoga programs.
            modelBuilder.Entity<YogaProgram>()
                .HasOne(program => program.Trainer)
                .WithMany()
                .HasForeignKey(program => program.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the precision for yoga program prices.
            modelBuilder.Entity<YogaProgram>()
                .Property(program => program.Price)
                .HasPrecision(18, 2);

            // Configure the precision for payment amounts.
            modelBuilder.Entity<Payment>()
                .Property(payment => payment.Amount)
                .HasPrecision(18, 2);

            // Configure the relationship between a yoga program and its classes.
            modelBuilder.Entity<YogaClass>()
                .HasOne(yogaClass => yogaClass.YogaProgram)
                .WithMany()
                .HasForeignKey(yogaClass => yogaClass.YogaProgramId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the relationship between a yoga class and its schedules.
            modelBuilder.Entity<Schedule>()
                .HasOne(schedule => schedule.YogaClass)
                .WithMany()
                .HasForeignKey(schedule => schedule.YogaClassId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the relationship between a booking and a customer profile.
            modelBuilder.Entity<Booking>()
                .HasOne(booking => booking.Customer)
                .WithMany()
                .HasForeignKey(booking => booking.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the relationship between a booking and a schedule.
            modelBuilder.Entity<Booking>()
                .HasOne(booking => booking.Schedule)
                .WithMany()
                .HasForeignKey(booking => booking.ScheduleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the relationship between a payment and its booking.
            modelBuilder.Entity<Payment>()
                .HasOne(payment => payment.Booking)
                .WithMany()
                .HasForeignKey(payment => payment.BookingId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
