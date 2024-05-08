﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnlineShop.Library.Common.Interfaces;
using OnlineShop.Library.Constants;
using OnlineShop.Library.Logging;

namespace OnlineShop.Library.Common.Repos
{
    public abstract class RepoControllerBase<T> : ControllerBase where T : class, IIdentifiable
    {
        protected readonly IRepo<T> EntitiesRepo;
        protected readonly ILogger<RepoControllerBase<T>> Logger;
        public RepoControllerBase(IRepo<T> entitiesRepo, ILogger<RepoControllerBase<T>> logger)
        {
            EntitiesRepo = entitiesRepo;
            Logger = logger;
        }

        [HttpPost(RepoActions.Add)]
        public async Task<IActionResult> Add([FromBody] T entity)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState.Values);
                }
                var articleId = await EntitiesRepo.AddAsync(entity);
                return Ok(articleId);
            }
            catch (Exception ex)
            {
                Logger.LogError(new LogEntry()
                    .WithClass(nameof(RepoControllerBase<T>))
                    .WithMethod(nameof(Add))
                    .WithComment(ex.ToString())
                    .WithOperation(RepoActions.Add)
                    .WithParametres($"{nameof(entity.Id)}: {entity.Id}")
                    .ToString()
                    );

                return StatusCode(500, LoggingConstants.InternalServerErrorMessage);
            }
        }

        [HttpPost(RepoActions.AddRange)]
        public async Task<IActionResult> Add([FromBody] IEnumerable<T> entities)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState.Values);
                }
                var articleIds = await EntitiesRepo.AddRangeAsync(entities);
                return Ok(articleIds);

            }
            catch (Exception ex)
            {
                Logger.LogError(new LogEntry()
                    .WithClass(nameof(RepoControllerBase<T>))
                    .WithMethod(nameof(Add))
                    .WithComment(ex.ToString())
                    .WithOperation(RepoActions.AddRange)
                    .WithParametres($"A collections of entities. Count: {entities.Count()}.")
                    .ToString()
                    );

                return StatusCode(500, LoggingConstants.InternalServerErrorMessage);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetOne(Guid id)
        {
            try
            {
                var article = await EntitiesRepo.GetOneAsync(id);
                if (article == null)
                {
                    return NotFound();
                }
                return Ok(article);
            }
            catch (Exception ex)
            {
                Logger.LogError(new LogEntry()
                    .WithClass(nameof(RepoControllerBase<T>))
                    .WithMethod(nameof(GetOne))
                    .WithComment(ex.ToString())
                    .WithParametres($"{nameof(id)}: {id}")
                    .ToString()
                    );

                return StatusCode(500, LoggingConstants.InternalServerErrorMessage);
            }
        }

        [HttpGet(RepoActions.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var articles = await EntitiesRepo.GetAllAsync();
                return Ok(articles);
            }
            catch (Exception ex)
            {
                Logger.LogError(new LogEntry()
                    .WithClass(nameof(RepoControllerBase<T>))
                    .WithMethod(nameof(GetAll))
                    .WithComment(ex.ToString())
                    .WithParametres(LoggingConstants.NoParameters)
                    .WithOperation(RepoActions.GetAll)
                    .ToString()
                    );

                return StatusCode(500, LoggingConstants.InternalServerErrorMessage);
            }
        }

        [HttpPost(RepoActions.Remove)]
        public virtual async Task<IActionResult> Remove([FromBody] Guid id)
        {
            try
            {
                await EntitiesRepo.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                Logger.LogError(new LogEntry()
                    .WithClass(nameof(RepoControllerBase<T>))
                    .WithMethod(nameof(Remove))
                    .WithComment(ex.ToString())
                    .WithParametres($"{nameof(id)}: {id}")
                    .WithOperation(RepoActions.Remove)
                    .ToString()
                    );

                return StatusCode(500, LoggingConstants.InternalServerErrorMessage);
            }
        }

        [HttpPost(RepoActions.RemoveRange)]
        public virtual async Task<IActionResult> Remove([FromBody] IEnumerable<Guid> ids)
        {
            try
            {
                await EntitiesRepo.DeleteRangeAsync(ids);
                return NoContent();
            }
            catch (Exception ex)
            {
                Logger.LogError(new LogEntry()
                    .WithClass(nameof(RepoControllerBase<T>))
                    .WithMethod(nameof(Remove))
                    .WithComment(ex.ToString())
                    .WithParametres($"A collections of ids. Count: {ids.Count()}.")
                    .WithOperation(RepoActions.RemoveRange)
                    .ToString()
                    );

                return StatusCode(500, LoggingConstants.InternalServerErrorMessage);
            }
        }

        [HttpPost(RepoActions.Update)]
        public virtual async Task<IActionResult> Update([FromBody] T entity)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState.Values);
                }

                var entityToBeUpdated = await EntitiesRepo.GetOneAsync(entity.Id);
                if (entityToBeUpdated == null)
                {
                    return BadRequest($"Entity with id = {entity.Id} was not found");
                }

                UpdateProperties(entity, entityToBeUpdated);
                await EntitiesRepo.SaveAsync(entityToBeUpdated);
                return Ok(entityToBeUpdated);
            }
            catch (Exception ex)
            {
                Logger.LogError(new LogEntry()
                    .WithClass(nameof(RepoControllerBase<T>))
                    .WithMethod(nameof(Update))
                    .WithComment(ex.ToString())
                    .WithParametres($"{nameof(entity.Id)}: {entity.Id}")
                    .WithOperation(RepoActions.Update)
                    .ToString()
                    );

                return StatusCode(500, LoggingConstants.InternalServerErrorMessage);
            }
        }

        protected abstract void UpdateProperties(T source, T destination);
    }
}
