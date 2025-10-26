﻿using Data.Interfaces;
using Entity.Dtos;
using Entity.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Data.Implementations
{
    public abstract class ARepositoryData<T> : IRepositoryData<T> where T : BaseModel
    {
        public abstract Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Obtener
        /// </summary>
        /// <returns></returns>
        /// 

        public abstract Task<IEnumerable<T>> GetAll(IDictionary<string, string?>? filters = null);

        /// <summary>
        /// Obtener por ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 

        public abstract Task<T> GetById(int id);


        /// <summary>
        /// Guardar
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public abstract Task<T> Save(T entity);


        /// <summary>
        /// Actualizar
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public abstract Task Update(T entity);
        /// <summary>
        /// Eliminar Persistente
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public abstract Task<bool> PermanentDelete(int id);
        /// <summary>
        /// Eliminar Lógico
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public abstract Task<int> Delete(int id);

        public abstract Task<List<ExpandoObject>> GetAllDynamicAsync();

        public abstract Task<PagedResult<TDto>> GetAllPaginatedAsync<TDto>(QueryParameters query, Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null, CancellationToken cancellationToken = default);
        public abstract Task<bool> ExistsAsynca(string field, string value, int? currentId);

    }
}
