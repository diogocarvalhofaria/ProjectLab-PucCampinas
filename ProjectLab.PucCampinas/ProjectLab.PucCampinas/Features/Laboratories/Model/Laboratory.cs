using System.ComponentModel.DataAnnotations;

namespace ProjectLab.PucCampinas.Features.Laboratories.Model
{
    public class Laboratory
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "O nome do laboratório é obrigatório")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "O bloco/prédio deve ser informado")]
        public string Building { get; set; } = string.Empty;
        public int Capacity { get; set; }
    }
}
