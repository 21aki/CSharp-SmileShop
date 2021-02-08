using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmileShop.Data;
using SmileShop.DTOs;
using SmileShop.Helpers;
using SmileShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SmileShop.Services
{
    public class ServiceBase
    {
        private readonly AppDBContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContext;

        public ServiceBase(AppDBContext dbContext, IMapper mapper, IHttpContextAccessor httpContext)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _httpContext = httpContext;
        }

        public static IOrderedQueryable<T> ApplyOrder<T>(
            IQueryable<T> source,
            string property,
            string methodName)
        {
            string[] props = property.Split('.');
            Type type = typeof(T);
            ParameterExpression arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            foreach (string prop in props)
            {
                // use reflection (not ComponentModel) to mirror LINQ
                PropertyInfo pi = type.GetProperty(prop);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

            object result = typeof(Queryable).GetMethods().Single(
                    method => method.Name == methodName
                            && method.IsGenericMethodDefinition
                            && method.GetGenericArguments().Length == 2
                            && method.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(T), type)
                    .Invoke(null, new object[] { source, lambda });
            return (IOrderedQueryable<T>)result;
        }

        public Func<DateTime> Now { get; private set; } = () => DateTime.Now;

        public void SetNow(DateTime now) => Now = () => now;

        public void ResetNow() => Now = () => DateTime.Now;

        public string GetUserId() => _httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        public string GetUsername() => _httpContext.HttpContext.User.FindFirstValue(ClaimTypes.Name);

        public string GetPersonId() => _httpContext.HttpContext.User.FindFirstValue("PersonId");

        public IEnumerable<string> GetUserRoles()
        {
            return _httpContext.HttpContext.User.FindAll(ClaimTypes.Role).Select(x => x.Value).ToArray();
        }

        protected async Task<List<TDTO>> Get<TEntity, TDTO>() where TEntity : class
        {
            var entities = await _dbContext.Set<TEntity>().AsNoTracking().ToListAsync();
            var dtos = _mapper.Map<List<TDTO>>(entities);
            return dtos;
        }

        protected async Task<List<TDTO>> Get<TEntity, TDTO>(PaginationDto paginationDto) where TEntity : class
        {
            var queryable = _dbContext.Set<TEntity>().AsNoTracking().AsQueryable();
            await _httpContext.HttpContext.InsertPaginationParametersInResponse(queryable, paginationDto.RecordsPerPage, paginationDto.Page);
            var entities = await queryable.Paginate(paginationDto).ToListAsync();
            var dtos = _mapper.Map<List<TDTO>>(entities);
            return dtos;
        }

        protected async Task<TDTO> Get<TEntity, TDTO>(string id) where TEntity : class, IId
        {
            var guid = Guid.Parse(id);
            var entity = await _dbContext.Set<TEntity>().FindAsync(guid);
            var dto = _mapper.Map<TDTO>(entity);
            return dto;
        }

        protected async Task<TDTO> Post<TAdd, TEntity, TDTO>(TAdd newItem) where TEntity : class, IId
        {
            var entity = _mapper.Map<TEntity>(newItem);
            entity.Id = new Guid();
            _dbContext.Set<TEntity>().Add(entity);
            await _dbContext.SaveChangesAsync();
            var dto = _mapper.Map<TDTO>(entity);
            return dto;
        }

        protected async Task<TDTO> Put<TUpdate, TEntity, TDTO>(string id, TUpdate newItem) where TEntity : class, IId
        {
            var guid = Guid.Parse(id);
            var entity = await _dbContext.Set<TEntity>().FindAsync(guid);

            entity = _mapper.Map(newItem, entity);

            _dbContext.Set<TEntity>().Update(entity);
            await _dbContext.SaveChangesAsync();

            var dto = _mapper.Map<TDTO>(entity);

            return dto;
        }

        protected async Task<TDTO> Delete<TEntity, TDTO>(string id) where TEntity : class, IId, new()
        {
            var guid = Guid.Parse(id);
            //var entity = await _dbContext.Set<TEntity>().FindAsync(guid);
            var entity = new TEntity() { Id = guid };
            _dbContext.Set<TEntity>().Remove(entity);
            await _dbContext.SaveChangesAsync();

            var dto = _mapper.Map<TDTO>(entity);

            return dto;
        }
    }
}