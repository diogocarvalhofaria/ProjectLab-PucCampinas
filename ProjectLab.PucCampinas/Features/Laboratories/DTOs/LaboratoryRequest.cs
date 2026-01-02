using System.ComponentModel.DataAnnotations;

namespace ProjectLab.PucCampinas.Features.Laboratories.DTOs
{
    public class LaboratoryRequest
    {
        public string Name { get; set; } = string.Empty;

        public string Building { get; set; } = string.Empty;

        public int Capacity { get; set; }
    }
}
