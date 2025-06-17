using Microsoft.AspNetCore.Mvc.Rendering;

public class AppointmentCreateViewModel
{
    // Seçilen değerler
    public int PackageId { get; set; }
    public int CourseId { get; set; }
    public int StudentId { get; set; }
    public DateTime Date { get; set; }
    public string Type { get; set; }
    public string ZoomLink { get; set; }

    // Dropdown listeleri
    public List<SelectListItem> Packages { get; set; }
    public List<SelectListItem> Students { get; set; }

    // Burada pakete göre kursları tutabilmek için Dictionary ekliyoruz
    public List<SelectListItem> Courses { get; set; }

}
