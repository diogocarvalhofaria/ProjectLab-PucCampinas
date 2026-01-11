using Microsoft.EntityFrameworkCore;
using ProjectLab.PucCampinas.Common.Models;
using ProjectLab.PucCampinas.Common.Services;
using ProjectLab.PucCampinas.Features.Laboratories.DTOs;
using ProjectLab.PucCampinas.Features.Laboratories.Model;
using ProjectLab.PucCampinas.Infrastructure.Data;
using ProjectLab.PucCampinas.shared.Service;

namespace ProjectLab.PucCampinas.Features.Laboratories.Service
{
    public class LaboratoryService : BaseService, ILaboratoryService
    {
        private readonly AppDbContext _context;

        public LaboratoryService(AppDbContext context, ICustomErrorHandler errorHandler)
             : base(errorHandler)
        {
            _context = context;
        }

        public async Task<List<Laboratory>> GetLaboratories()
        {
            try
            {
                return await _context.Laboratories.ToListAsync();
            }
            catch (Exception ex)
            {
                OnError(ex, 500);
                throw;
            }
        }

        public async Task<LaboratoryResponse?> GetLaboratoriesById(Guid id)
        {
            try
            {
                var lab = await _context.Laboratories.FindAsync(id);
                if (lab == null) return null;

                return new LaboratoryResponse
                {
                    Id = lab.Id,
                    Name = lab.Name,
                    Building = lab.Building,
                    Capacity = lab.Capacity,
                    Room = lab.Room

                };
            }
            catch (Exception ex)
            {
                OnError(ex, 500);
                throw;
            }
        }


        public async Task<LaboratoryResponse> CreateLaboratories(LaboratoryRequest request)
        {
            try
            {
                var lab = new Laboratory
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Building = request.Building,
                    Capacity = request.Capacity,
                    Room = request.Room
                };

                _context.Laboratories.Add(lab);
                await _context.SaveChangesAsync();

                return new LaboratoryResponse
                {
                    Id = lab.Id,
                    Name = lab.Name,
                    Building = lab.Building,
                    Capacity = lab.Capacity,
                    Room = lab.Room
                };
            }
            catch (Exception ex)
            {
                OnError(ex, 500);
                throw;
            }
        }

        public async Task DeleteLaboratories(Guid id)
        {
            try
            {
                await _context.Laboratories
                    .Where(lab => lab.Id == id)
                    .ExecuteDeleteAsync();
            }
            catch (Exception ex)
            {
                OnError(ex, 500);
                throw;
            }
        }

        public async Task UpdateLaboratories(Guid id, LaboratoryRequest request)
        {
            try
            {
                var lab = await _context.Laboratories.FirstOrDefaultAsync(l => l.Id == id);

                if (lab == null) throw new Exception("Laboratório não encontrado");

                lab.Name = request.Name;
                lab.Building = request.Building;
                lab.Capacity = request.Capacity;
                lab.Room = request.Room;

                _context.Laboratories.Update(lab);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                OnError(ex, 500);
                throw;
            }
        }

        public async Task<PaginatedResult<LaboratoryResponse>> SearchLaboratories(SearchLaboratoryInput filter)
        {
            try
            {
                var query = _context.Laboratories.AsQueryable();

                if (!string.IsNullOrWhiteSpace(filter.Keyword))
                {
                    query = query.Where(x => x.Name.Contains(filter.Keyword) ||
                                             x.Building.Contains(filter.Keyword));
                }

                query = filter.Order.ToUpper() == "ASC"
                    ? query.OrderBy(x => x.Name)
                    : query.OrderByDescending(x => x.Name);

                var responseQuery = query.Select(lab => new LaboratoryResponse
                {
                    Id = lab.Id,
                    Name = lab.Name,
                    Building = lab.Building,
                    Capacity = lab.Capacity,
                    Room = lab.Room
                });

                return await responseQuery.ToPaginatedResultAsync(filter.Page, filter.Size);
            }
            catch (Exception ex)
            {
                OnError(ex, 500);
                throw;
            }
        }
    }
}
