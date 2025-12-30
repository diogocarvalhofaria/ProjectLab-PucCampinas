using ProjectLab.PucCampinas.Common.Models;
using ProjectLab.PucCampinas.Features.Laboratories.DTOs;
using ProjectLab.PucCampinas.Features.Laboratories.Model;

namespace ProjectLab.PucCampinas.Features.Laboratories.Service
{
    public interface ILaboratoryService
    {
        Task<List<Laboratory>> GetLaboratories();
        Task<Laboratory?> GetLaboratoriesById(Guid id);
        Task CreateLaboratories(Laboratory laboratory);
        Task DeleteLaboratories(Guid id);
        Task UpdateLaboratories(Laboratory laboratory);
        Task<PaginatedResult<Laboratory>> SearchLaboratories(SearchLaboratoryInput filter);
    }
}