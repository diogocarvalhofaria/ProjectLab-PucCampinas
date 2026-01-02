using ProjectLab.PucCampinas.Common.Models;
using ProjectLab.PucCampinas.Features.Laboratories.DTOs;
using ProjectLab.PucCampinas.Features.Laboratories.Model;

namespace ProjectLab.PucCampinas.Features.Laboratories.Service
{
    public interface ILaboratoryService
    {
        Task<PaginatedResult<LaboratoryResponse>> SearchLaboratories(SearchLaboratoryInput filter);
        Task<LaboratoryResponse?> GetLaboratoriesById(Guid id);
        Task<LaboratoryResponse> CreateLaboratories(LaboratoryRequest request);
        Task UpdateLaboratories(Guid id, LaboratoryRequest request);
        Task DeleteLaboratories(Guid id);
    }
}