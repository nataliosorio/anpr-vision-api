﻿using Entity.Dtos;
using Entity.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IRepositoryData<T> where T : BaseModel
    {
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Obtener
        /// </summary>
        /// <returns></returns>
        /// 

        Task<IEnumerable<T>> GetAll(IDictionary<string, string?>? filters = null);

        /// <summary>
        /// Obtener por ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 

        Task<T> GetById(int id);


        /// <summary>
        /// Guardar
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<T> Save(T entity);
        

        /// <summary>
        /// Actualizar
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task Update(T entity);
        /// <summary>
        /// Eliminar Persistente
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> PermanentDelete(int id);
        /// <summary>
        /// Eliminar Lógico
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<int> Delete(int id);
        /// <summary>
        /// Obtiene una lista dinámica de entidades de tipo T, 
        /// incluyendo automáticamente las relaciones marcadas con el atributo <see cref="ForeignIncludeAttribute"/>.
        /// Las relaciones pueden incluir propiedades anidadas, y los resultados se devuelven como objetos dinámicos 
        /// con nombres de propiedades en PascalCase.
        /// </summary>
        /// <returns>Una lista de objetos dinámicos <see cref="ExpandoObject"/> que contiene el Id y las propiedades 
        /// seleccionadas desde las relaciones.</returns>
        Task<List<ExpandoObject>> GetAllDynamicAsync();

        Task<PagedResult<TDto>> GetAllPaginatedAsync<TDto>(QueryParameters query,Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null,CancellationToken cancellationToken = default);

        Task<bool> ExistsAsynca(string field, string value, int? currentId);

    }
}
