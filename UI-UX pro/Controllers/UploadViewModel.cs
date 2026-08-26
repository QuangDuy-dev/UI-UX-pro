using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace UI_UX_pro.Controllers;

public class UploadViewModel
{
    [Required]
    [StringLength(120, MinimumLength = 3)]
    public string Name { get; set; } = "";

    public string? CategorySlug { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(200)]
    public string? Tags { get; set; }

    [Required]
    public string Html { get; set; } = "";

    public string? Css { get; set; }

    public string? Js { get; set; }
}
