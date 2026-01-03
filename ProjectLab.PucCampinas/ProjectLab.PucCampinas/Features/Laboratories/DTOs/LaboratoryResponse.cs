namespace ProjectLab.PucCampinas.Features.Laboratories.DTOs
{
    public class LaboratoryResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Building { get; set; } = string.Empty;
        public int Capacity { get; set; }
    }
}
