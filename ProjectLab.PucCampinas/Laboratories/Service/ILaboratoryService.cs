using ProjectLab.PucCampinas.Common.Models;
using ProjectLab.PucCampinas.Laboratories.DTOs;
using ProjectLab.PucCampinas.Laboratories.Model;

namespace ProjectLab.PucCampinas.Laboratories.Service
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