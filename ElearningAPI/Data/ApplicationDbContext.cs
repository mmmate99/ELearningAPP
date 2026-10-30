using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ElearningAPI.Models;

namespace ElearningAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<AnswerOption> AnswerOptions { get; set; }
        public DbSet<Progress> Progresses { get; set; }
        public DbSet<ForumPost> ForumPosts { get; set; }
        public DbSet<UserCourse> UserCourses { get; set; }
        public DbSet<QuizAttempt> QuizAttempts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ApplicationUser konfiguráció
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.CreatedCourses)
                .WithOne(c => c.Teacher)
                .HasForeignKey(c => c.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            // UserCourse - összetett kulcs
            builder.Entity<UserCourse>()
                .HasKey(uc => new { uc.UserId, uc.CourseId });

            builder.Entity<UserCourse>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.EnrolledCourses)
                .HasForeignKey(uc => uc.UserId);

            builder.Entity<UserCourse>()
                .HasOne(uc => uc.Course)
                .WithMany(c => c.UserCourses)
                .HasForeignKey(uc => uc.CourseId);

            // QuizAttempt
            builder.Entity<QuizAttempt>()
                .HasOne(qa => qa.User)
                .WithMany(u => u.QuizAttempts)
                .HasForeignKey(qa => qa.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<QuizAttempt>()
                .HasOne(qa => qa.Quiz)
                .WithMany(q => q.QuizAttempts)
                .HasForeignKey(qa => qa.QuizId)
                .OnDelete(DeleteBehavior.Restrict);

            // ForumPost önreferenciális kapcsolat
            builder.Entity<ForumPost>()
                .HasOne(fp => fp.ParentPost)
                .WithMany(fp => fp.Replies)
                .HasForeignKey(fp => fp.ParentPostId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexek
            builder.Entity<Course>()
                .HasIndex(c => c.TeacherId);

            builder.Entity<Progress>()
                .HasIndex(p => new { p.UserId, p.LessonId })
                .IsUnique();

            builder.Entity<QuizAttempt>()
                .HasIndex(qa => new { qa.UserId, qa.QuizId, qa.AttemptNumber })
                .IsUnique();

            // String mezők konfigurálása
            builder.Entity<Course>()
                .Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Entity<Course>()
                .Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Entity<Lesson>()
                .Property(l => l.Title)
                .IsRequired()
                .HasMaxLength(200);

            // Enum konverziók
            builder.Entity<Course>()
                .Property(c => c.Difficulty)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Entity<Lesson>()
                .Property(l => l.Type)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Entity<Question>()
                .Property(q => q.Type)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Entity<ApplicationUser>()
                .Property(u => u.Role)
                .HasConversion<string>()
                .HasMaxLength(20);
        }
    }
}