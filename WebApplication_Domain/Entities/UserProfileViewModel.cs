using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication_Domain.Entities
{
    public class UserProfileViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Ad Soyad")]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        public string Role { get; set; }

        [Display(Name = "Biyografi")]
        public string? Biography { get; set; }

        [Display(Name = "Sertifikalar")]
        // var olan dosya yolu
        public string? ExistingCertificationPath { get; set; }
        // yeni belge yükleme
        public IFormFile? CertificationFile { get; set; }

        [Display(Name = "Yeni Şifre")]
        [MinLength(6)]
        public string? NewPassword { get; set; }

        [Display(Name = "Profil Resmi")]
        public IFormFile? ProfileImage { get; set; }

        public string? ExistingProfileImagePath { get; set; }

        [Url, Display(Name = "Instagram URL")]
        public string? InstagramUrl { get; set; }
        [Url, Display(Name = "Twitter (X) URL")]
        public string? TwitterUrl { get; set; }
        [Url, Display(Name = "Facebook URL")]
        public string? FacebookUrl { get; set; }
    }
}