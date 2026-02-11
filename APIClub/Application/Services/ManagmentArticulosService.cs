using APIClub.Application.Common;
using APIClub.Application.Dtos.Articulos;
using APIClub.Domain.AlquilerArticulos.Models;
using APIClub.Domain.AlquilerArticulos.Repositories;
using APIClub.Domain.AlquilerArticulos.UseCases;

namespace APIClub.Application.Services
{
    public class ManagmentArticulosService : IManagmentArticulosService
    {
        private readonly IArticuloRepository _articuloRepository;

        public ManagmentArticulosService(IArticuloRepository articuloRepository)
        {
            _articuloRepository = articuloRepository;
        }

        public async Task<Result<ArticuloDto>> CargarArticulo(CreateArticuloDto dto)
        {
            var exists = await _articuloRepository.ArticuloExistsByNombre(dto.Nombre);

            if (exists)
            {
                return Result<ArticuloDto>.Error("Ya existe un artículo con ese nombre", 400);
            }

            var articulo = new Articulo
            {
                Nombre = dto.Nombre,
                PrecioAlquiler = dto.PrecioAlquiler
            };

            await _articuloRepository.CargarArticulo(articulo);

            var articuloDto = new ArticuloDto
            {
                Id = articulo.Id,
                Nombre = articulo.Nombre,
                PrecioAlquiler = articulo.PrecioAlquiler
            };

            return Result<ArticuloDto>.Exito(articuloDto);
        }

        public async Task<Result<List<ArticuloDto>>> GetAllArticulos()
        {
            var articulos = await _articuloRepository.GetAllArticulos();

            var articulosDto = articulos.Select(a => new ArticuloDto
            {
                Id = a.Id,
                Nombre = a.Nombre,
                PrecioAlquiler = a.PrecioAlquiler
            }).ToList();

            return Result<List<ArticuloDto>>.Exito(articulosDto);
        }

        public async Task<Result<ArticuloDto>> UpdatePrecioArticulo(int id, int nuevoPrecio)
        {
            if (id <= 0)
                return Result<ArticuloDto>.Error("El ID proporcionado no es válido", 400);
            

            if (nuevoPrecio <= 0)
                return Result<ArticuloDto>.Error("El precio debe ser mayor a 0", 400);

            var articulo = await _articuloRepository.GetArticuloById(id);

            if (articulo is null)
                return Result<ArticuloDto>.Error("No se encontró un artículo con ese ID", 404);

            articulo.PrecioAlquiler = nuevoPrecio;

            await _articuloRepository.UpdateArticulo(articulo);

            var articuloDto = new ArticuloDto
            {
                Id = articulo.Id,
                Nombre = articulo.Nombre,
                PrecioAlquiler = articulo.PrecioAlquiler
            };

            return Result<ArticuloDto>.Exito(articuloDto);
        }
    }
}
