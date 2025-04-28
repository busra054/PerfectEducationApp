using WebApplication_Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace WebApplication_Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ApplicationDbContext()
        {

        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=EducationDB2;User=SA;Password=reallyStrongPwd123;TrustServerCertificate=True", b => b.MigrationsAssembly("WebApplication_Deneme"));
        }

        public DbSet<Branch> Branches { get; set; }
        public DbSet<TeacherBranch> TeacherBranches { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseEnrollment> CourseEnrollments { get; set; } // PascalCase

        public DbSet<CourseMaterial> CourseMaterials { get; set; } // PascalCase
        public DbSet<Student> Students { get; set; }
        public DbSet<TeacherRequest> TeacherRequests { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<AssignmentSubmission> AssignmentSubmissions { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Course ilişkileri
            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasOne(c => c.Teacher)
                    .WithMany(t => t.Courses)
                    .HasForeignKey(c => c.TeacherId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Package)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(c => c.PackageId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CourseEnrollment>(entity =>
            {
                entity.HasKey(ce => ce.Id);

                entity.HasOne(ce => ce.Student)
                    .WithMany(s => s.Enrollments)
                    .HasForeignKey(ce => ce.StudentId)
                    .OnDelete(DeleteBehavior.Cascade); // Öğrenci silinirse kayıtlar da silinsin

                entity.HasOne(ce => ce.Course)
                    .WithMany(c => c.EnrolledStudents)
                    .HasForeignKey(ce => ce.CourseId)
                    .OnDelete(DeleteBehavior.Restrict); // Kurs silinirse kayıtlar kalsın
            });

            // CourseMaterial ilişkileri
            modelBuilder.Entity<CourseMaterial>(entity =>
            {
                entity.HasKey(cm => cm.Id);

                // Course ilişkisi
                entity.HasOne(cm => cm.Course)
                    .WithMany(c => c.CourseMaterials)
                    .HasForeignKey(cm => cm.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);

                // UploadedBy ilişkisi (User)
                entity.HasOne(cm => cm.UploadedBy)
                    .WithMany(u => u.UploadedMaterials)
                    .HasForeignKey(cm => cm.UploadedById)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Teacher-User İlişkisi
            modelBuilder.Entity<Teacher>()
                .HasOne(t => t.User)
                .WithOne(u => u.TeacherProfile)
                .HasForeignKey<Teacher>(t => t.UserId);

            // Student-User İlişkisi
            modelBuilder.Entity<Student>()
                .HasOne(s => s.User)
                .WithOne(u => u.StudentProfile)
                .HasForeignKey<Student>(s => s.UserId);

            modelBuilder.Entity<Branch>()
                .HasKey(b => b.Id);

            modelBuilder.Entity<TeacherBranch>(entity =>
            {
                entity.HasKey(tb => tb.Id);

                entity.HasOne(tb => tb.Teacher)
                    .WithMany(t => t.TeacherBranches)
                    .HasForeignKey(tb => tb.TeacherId)
                    .OnDelete(DeleteBehavior.Restrict); // Teachers tablosuna bağlı

                entity.HasOne(tb => tb.Branch)
                    .WithMany(b => b.TeacherBranches)
                    .HasForeignKey(tb => tb.BranchId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TeacherRequest>()
                .HasOne(tr => tr.Branch)
                .WithMany()
                .HasForeignKey(tr => tr.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(a => a.Id);

                // Student ilişkisi
                entity.HasOne(a => a.Student)
                    .WithMany(s => s.AppointmentsAsStudent)
                    .HasForeignKey(a => a.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Teacher ilişkisi
                entity.HasOne(a => a.Teacher)
                    .WithMany(t => t.AppointmentsAsTeacher)
                    .HasForeignKey(a => a.TeacherId)
                    .OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<Package>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(p => p.Id);

                // User ilişkisini kaldır, Student ilişkisini ekle
                entity.HasOne(p => p.Student)
                    .WithMany(s => s.PurchasedPackages)
                    .HasForeignKey(p => p.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Package)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(p => p.PackageId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(p => p.Amount)
                    .HasColumnType("decimal(18,2)");
            }); // User silinirse Payment silinmesin

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Package)
                .WithMany(p => p.Payments)
                .HasForeignKey(p => p.PackageId)
                .OnDelete(DeleteBehavior.Restrict); // Package silinirse Payment silinmesin

            modelBuilder.Entity<Message>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<Message>()
                .HasOne<User>(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Message>()
                .HasOne<User>(m => m.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.NoAction);
            
            modelBuilder.Entity<Package>(entity =>
            {
                entity.Property(p => p.Price)
                    .HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasMany(s => s.PurchasedPackages)
                    .WithOne(p => p.Student)
                    .HasForeignKey(p => p.StudentId);
            });

            // Assignment ilişkileri
            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.HasKey(a => a.Id);

                entity.HasOne(a => a.Course)
                    .WithMany(c => c.Assignments)
                    .HasForeignKey(a => a.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<AssignmentSubmission>(entity =>
            {
                entity.HasKey(s => s.Id);

                entity.HasOne(s => s.Assignment)
                    .WithMany(a => a.Submissions)
                    .HasForeignKey(s => s.AssignmentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(s => s.Student)
                    .WithMany()
                    .HasForeignKey(s => s.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);

             
            });


            modelBuilder.Entity<Branch>().HasData(
                new Branch { Id = 1, Name = "Matematik" },
                new Branch { Id = 2, Name = "Fizik" },
                new Branch { Id = 3, Name = "Kimya" },
                new Branch { Id = 4, Name = "Biyoloji" },
                new Branch { Id = 5, Name = "Türkçe" },
                new Branch { Id = 6, Name = "İngilizce" },
                new Branch { Id = 7, Name = "Tarih" },
                new Branch { Id = 8, Name = "Coğrafya" },
                new Branch { Id = 9, Name = "İspanyolca" },
                new Branch { Id = 10, Name = "Korece" }
            );

            modelBuilder.Entity<Package>().HasData(
               new Package
               {
                   Id = 1,
                   Name = "TYT-Eğitimi Full Paket",
                   Description = "TYT genel tüm dersler temelden gelişmişe adım adım Eğitim",
                   Price = 14000.0m,
               },
new Package
{
    Id = 2,
    Name = "AYT-Sayısal Eğitimi Full Paket",
    Description = "AYT temelden gelişmişe Matematik ,Fizik, Kimya, Biyoloji Dersleri Eğitimi",
    Price = 18000.0m,
},
new Package
{
    Id = 3,
    Name = "AYT-Eşit Ağırlık Eğitimi",
    Description = "AYT temelden gelişmişe Matematik ,Tarih, Coğrafya, Edebiyat Dersleri eğitimi",
    Price = 14000.0m,
},
new Package
{
    Id = 4,
    Name = "KPSS Kapsamlı Eğitimi ",
    Description = "KPSS temelden gelişmişe full hazırlık paketi dersleri",
    Price = 23000.0m,
},
new Package
{
    Id = 5,
    Name = "DGS Kapsamlı Eğitimi",
    Description = "DGS temelden gelişmişe full hazırlık paketi dersleri",
    Price = 13000.0m,
},
new Package
{
    Id = 6,
    Name = "İngilizce A1-A2 Temel Seviye Eğitim",
    Description = "İngilizce temelden gelişmişe okuma,yazma,konuşma ve dinleme becerileri geliştirme dersleri",
    Price = 15000.0m,
},
new Package
{
    Id = 7,
    Name = "İspanyolca A1-A2 Temel Seviye Eğitim",
    Description = "İspanyolca temelden gelişmişe okuma,yazma,konuşma ve dinleme becerileri geliştirme dersleri",
    Price = 12000.0m,
},
new Package
{
    Id = 8,
    Name = "Korece A1-A2 Temel Seviye Eğitim",
    Description = "Korece temelden gelişmişe okuma,yazma,konuşma ve dinleme becerileri geliştirme dersleri",
    Price = 16000.0m,
},
new Package
{
    Id = 9,
    Name = "Almanca A1-B2 Temelden Orta Düzeye(İntermediate) Seviye Eğitim",
    Description = "Almanca temelden gelişmişe okuma,yazma,konuşma ve dinleme becerileri geliştirme dersleri",
    Price = 17000.0m,
}

                );
        }

        public DbSet<WebApplication_Domain.Entities.Course> Course { get; set; } = default!;

    }
}
