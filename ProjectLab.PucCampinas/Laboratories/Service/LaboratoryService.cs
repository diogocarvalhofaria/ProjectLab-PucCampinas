using Microsoft.EntityFrameworkCore;

using ProjectLab.PucCampinas.Common.Models;
using ProjectLab.PucCampinas.Data;
using ProjectLab.PucCampinas.Laboratories.DTOs;
using ProjectLab.PucCampinas.Laboratories.Model;
using ProjectLab.PucCampinas.shared.Service;

namespace ProjectLab.PucCampinas.Laboratories.Service
{
    public class LaboratoryService : ILaboratoryService
    {
        private readonly AppDbContext _context;

        public LaboratoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Laboratory>> GetLaboratories()
           => await _context.Laboratories.ToListAsync();

        public async Task<Laboratory?> GetLaboratoriesById(Guid id)
           => await _context.Laboratories.FindAsync(id);


        public async Task CreateLaboratories(Laboratory laboratory)
        { 
             _context.Laboratories.Add(laboratory);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteLaboratories(Guid id)
        {
            await _context.Laboratories
                .Where(lab => lab.Id == id)
                .ExecuteDeleteAsync();
        }

        public async Task UpdateLaboratories(Laboratory laboratory)
        {

            _context.Laboratories.Update(laboratory);
            await _context.SaveChangesAsync();
        }

        public async Task<PaginatedResult<Laboratory>> SearchLaboratories(SearchLaboratoryInput filter)
        {
            var entity = _context.Laboratories.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
                entity = entity.Where(x => x.Name.Contains(filter.Keyword));

            entity = filter.Order.ToUpper() == "ASC"
                ? entity.OrderBy(x => x.Name)
                : entity.OrderByDescending(x => x.Name);

            return await entity.ToPaginatedResultAsync(filter.Page, filter.Size);

        }
    }
}
