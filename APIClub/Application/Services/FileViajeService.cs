using APIClub.Application.Dtos.Viajes.FileViaje;
using APIClub.Domain.ModuloGestionViajes.Models;
using APIClub.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace APIClub.Application.Services
{
    public class FileViajeService
    {
        private readonly AppDbcontext _context;

        public FileViajeService(AppDbcontext context)
        {
            _context = context;
        }

        public async Task<bool> AddInscriptoToFile(AddInscriptoToFileDto dto)
        {
            var inscripto = await _context.Inscriptos
                .Include(i => i.Variante)
                .FirstOrDefaultAsync(i => i.Id == dto.InscriptoId);

            if (inscripto == null)
                return false;

            var numeroFile = dto.NumeroFile.Trim();

            if (string.IsNullOrWhiteSpace(numeroFile))
                return false;

            var viajeId = inscripto.Variante.IdViaje;

            var file = await _context.FileViajes
                .FirstOrDefaultAsync(f =>
                    f.NumeroFile == numeroFile &&
                    f.ViajeId == viajeId);

            if (file == null)
            {
                file = new FileViaje
                {
                    NumeroFile = numeroFile,
                    ViajeId = viajeId
                };

                _context.FileViajes.Add(file);
            }

            inscripto.FileViaje = file;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<FileViajeViewDto>> GetFilesByViaje(int viajeId)
        {
            return await _context.FileViajes
                .Where(f => f.ViajeId == viajeId)
                .Where(f => f.Inscriptos.Any(i => !i.cancelado))
                .Select(f => new FileViajeViewDto
                {
                    Id = f.Id,
                    NumeroFile = f.NumeroFile,
                    ViajeId = f.ViajeId,
                    InscriptosIds = f.Inscriptos
                        .Where(i => !i.cancelado)
                        .Select(i => i.Id)
                        .ToList()
                })
                .ToListAsync();
        }
    }
}
