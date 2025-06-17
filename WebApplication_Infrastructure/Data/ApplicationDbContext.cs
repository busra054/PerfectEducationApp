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

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseSqlServer("Server=localhost,1433;Database=EducationDB2;User=SA;Password=reallyStrongPwd123;TrustServerCertificate=True",
        //            b => b.MigrationsAssembly("WebApplication_Deneme"));
        //    }
        //}


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

                entity.HasOne(a => a.Student)
                      .WithMany(s => s.AppointmentsAsStudent)
                      .HasForeignKey(a => a.StudentId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Teacher)
                      .WithMany(t => t.AppointmentsAsTeacher)
                      .HasForeignKey(a => a.TeacherId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Package)
                      .WithMany()    // isterseniz Package içindeki ICollection<Appointment> ile .WithMany(p=>p.Appointments) yapabilirsiniz
                      .HasForeignKey(a => a.PackageId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Course)
                      .WithMany()    // isterseniz Course içindeki ICollection<Appointment> ile .WithMany(c=>c.Appointments) yapabilirsiniz
                      .HasForeignKey(a => a.CourseId)
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
                Description = "TYT ve AYT tüm konuları kapsayan, eğitim, canlı yayınlar ve deneme sınavlarıyla eksiksiz hazırlık.",
                OriginalPrice = 35000.00m,
                DiscountRate = 30,
                Price = 24500.00m,
                BannerText = "Her Şey Dahil",
                Feature1 = "100 Soru Sorma Hakkı",
                Feature2 = "PDF Özet Notlar",
                Feature3 = "Videolu Deneme Sınavları",
                Feature4 = "Yapay Zeka Asistanı",
                CoverImagePath = "/img/1.jpg"
            },
            new Package
            {
                Id = 2,
                Name = "AYT-Sayısal Eğitimi Full Paket",
                Description = "TYT konularına hızlı tekrar ve yoğun soru çözümü içeren pratik paket.",
                OriginalPrice = 18000.00m,
                DiscountRate = 20,
                Price = 14400.00m,
                BannerText = "Kampanyalı",
                Feature1 = "50 Canlı Soru Saati",
                Feature2 = "40 Test Denemesi",
                Feature3 = "PDF Özet Notlar",
                Feature4 = "Yapay Zeka Asistanı",
                CoverImagePath = "/img/2.jpg"
            },
            new Package
            {
                Id = 3,
                Name = "AYT-Eşit Ağırlık Eğitimi",
                Description = "Matematik, Fizik, Kimya ve Biyoloji konularını içeren kapsamlı sayısal paket.",
                OriginalPrice = 30000.00m,
                DiscountRate = 25,
                Price = 22500.00m,
                BannerText = "İndirimli",
                Feature1 = "80 Deneme Sınavı",
                Feature2 = "Canlı Soru Çözümü",
                Feature3 = "PDF Özet Notlar",
                Feature4 = "Yapay Zeka Asistanı",
                CoverImagePath = "/img/3.jpg"
            },
            new Package
            {
                Id = 4,
                Name = "KPSS Kapsamlı Eğitimi",
                Description = "Matematik, Edebiyat, Tarih ve Coğrafya derslerini kapsayan eşit ağırlık paketi.",
                OriginalPrice = 28000.00m,
                DiscountRate = 25,
                Price = 21000.00m,
                BannerText = "Kampanyalı",
                Feature1 = "75 Deneme Sınavı",
                Feature2 = "Video Çözümler",
                Feature3 = "PDF Özet Notlar",
                Feature4 = "Yapay Zeka Asistanı",
                CoverImagePath = "/img/4.jpg"
            },
            new Package
            {
                Id = 5,
                Name = "DGS Kapsamlı Eğitimi",
                Description = "Genel Yetenek ve Genel Kültür derslerinden oluşan KPSS hazırlık paketi.",
                OriginalPrice = 25000.00m,
                DiscountRate = 28,
                Price = 18000.00m,
                BannerText = "Her Şey Dahil",
                Feature1 = "60 Deneme Sınavı",
                Feature2 = "Video Anlatım",
                Feature3 = "PDF Özet Notlar",
                Feature4 = "Yapay Zeka Asistanı",
                CoverImagePath = "/img/5.jpg"
            },
            new Package
            {
                Id = 6,
                Name = "İngilizce A1-A2 Temel Seviye Eğitim",
                Description = "Dikey Geçiş Sınavı için tüm başlıkları kapsayan hazırlık paketi.",
                OriginalPrice = 20000.00m,
                DiscountRate = 20,
                Price = 16000.00m,
                BannerText = "Kampanyalı",
                Feature1 = "50 Deneme Sınavı",
                Feature2 = "PDF Özet Notlar",
                Feature3 = "Video Çözümler",
                Feature4 = "Yapay Zeka Asistanı",
                CoverImagePath = "/img/6.jpg"
            },
            new Package
            {
                Id = 7,
                Name = "İspanyolca A1-A2 Temel Seviye Eğitim",
                Description = "Okuma, yazma, konuşma ve dinleme pratiği ile A1-A2 seviyesine hazırlık.",
                OriginalPrice = 15000.00m,
                DiscountRate = 15,
                Price = 12750.00m,
                BannerText = "İndirimli",
                Feature1 = "Canlı Konuşma Seansları",
                Feature2 = "PDF Eğitim Materyalleri",
                Feature3 = "Kısa Video Dersler",
                Feature4 = "Yapay Zeka Asistanı",
                CoverImagePath = "/img/7.jpg"
            },
            new Package
            {
                Id = 8,
                Name = "Korece A1-A2 Temel Seviye Eğitim",
                Description = "Temel Korece becerilerini geliştiren A1-A2 paketi.",
                OriginalPrice = 14000.00m,
                DiscountRate = 15,
                Price = 11900.00m,
                BannerText = "Kampanyalı",
                Feature1 = "Konuşma Kulübü",
                Feature2 = "PDF Notlar",
                Feature3 = "Video Dersler",
                Feature4 = "Yapay Zeka Asistanı",
                CoverImagePath = "/img/8.jpg"
            },
            new Package
            {
                Id = 9,
                Name = "Almanca A1-B2 Temelden Orta Düzeye(İntermediate) Seviye Eğitim",
                Description = "İlk seviye Korece pratik ve temel dil bilgisi paketi.",
                OriginalPrice = 16000.00m,
                DiscountRate = 15,
                Price = 13600.00m,
                BannerText = "Her Şey Dahil",
                Feature1 = "Karakter Tanıma Dersleri",
                Feature2 = "PDF Eğitim Seti",
                Feature3 = "Video Anlatım",
                Feature4 = "Yapay Zeka Asistanı",
                CoverImagePath = "/img/9.jpg"
            }
        );

        }

        public DbSet<WebApplication_Domain.Entities.Course> Course { get; set; } = default!;

    }
}
