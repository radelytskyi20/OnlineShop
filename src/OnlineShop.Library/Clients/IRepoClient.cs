﻿using OnlineShop.Library.Common.Responses;

namespace OnlineShop.Library.Clients
{
    public interface IRepoClient<T> : IHttpClientContainer
    {
        Task<ServiceResponse<Guid>> Add(T entity);
        Task<ServiceResponse<IEnumerable<Guid>>> AddRange(IEnumerable<T> entities);
        Task<ServiceResponse<T>> Update(T entity);
        Task<ServiceResponse<object>> Remove(Guid entityId);
        Task<ServiceResponse<object>> RemoveRange(IEnumerable<Guid> entityIds);
        Task<ServiceResponse<T>> GetOne(Guid entityId);
        Task<ServiceResponse<IEnumerable<T>>> GetAll();
    }
}
