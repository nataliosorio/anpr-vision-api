using Entity.Dtos;
using Entity.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IRepositoryBusiness<T, D> where T : BaseModel where D : BaseDto
    {
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// Obtener 
        /// </summary>
        /// <returns></returns>
        Task<List<D>> GetAll(IDictionary<string, string?>? filters = null);
        /// <summary>
        /// Obtener por ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<D> GetById(int id);
        /// <summary>
        /// Guardar
        /// </summary>
        /// <param name="entityDto"></param>
        /// <returns></returns>
        Task<D> Save(D entityDto);
        /// <summary>
        /// Actualizar
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entityDto"></param>
        /// <returns></returns>
        Task Update(D entityDto);
        /// <summary>
        /// Eliminar Logico
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<int> Delete(int id);
        /// <summary>
        /// Eliminar Consistente
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> PermanentDelete(int id);

        /// <summary>
        /// Obtiene una lista dinámica de entidades con relaciones incluidas automáticamente.
        /// </summary>
        /// <returns>Lista de objetos dinámicos con propiedades en PascalCase.</returns>
        Task<List<ExpandoObject>> GetAllDynamicAsync();

        Task<PagedResult<D>> GetAllPaginatedAsync(QueryParameters query, Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IQueryable<T>>? include = null, CancellationToken cancellationToken = default);

        Task<bool> ExistsAsynca(string field, string value, int? currentId);
    }

}
