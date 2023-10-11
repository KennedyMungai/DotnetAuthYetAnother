using System.ComponentModel.DataAnnotations;

namespace DotnetAuthYetAnother.Api.Models;


public class TeamModel
{
    [Required(ErrorMessage = "Id is required")]
    public int Id { get; set; }
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = string.Empty;
    [Required(ErrorMessage = "Country is required")]
    public string Country { get; set; } = string.Empty;
    [Required(ErrorMessage = "Team principle is required")]
    public string TeamPrinciple { get; set; } = string.Empty;
}