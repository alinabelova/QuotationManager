using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using QuotationManager.Web.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuotationManager.DataAccess.Repository;
using QuotationManager.Models;
using QuotationManager.Models.Enums;
using QuotationManager.Web.Contracts;
using QuotationManager.Web.Models;

namespace QuotationManager.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/Quota")]
    [Authorize]
    public class QuotaController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Quota> _quotaRepository;
        private readonly IRepository<City> _cityRepository;
        private readonly IRepository<Contribution> _contributionRepository;

        public QuotaController(UserManager<ApplicationUser> userManager, IRepository<Quota> quotaRepository,
            IRepository<City> cityRepository, IRepository<Contribution> contributionRepository)
        {
            _userManager = userManager;
            _quotaRepository = quotaRepository;
            _cityRepository = cityRepository;
            _contributionRepository = contributionRepository;
        }

        /// <summary>
        /// Get all quota for current user
        /// </summary>
        /// <param name="contract">PaginationContract</param>
        /// <returns>List of ordered quotas for current page</returns>
        [Route("Get")]
        [HttpPost]
        public async Task<IActionResult> Get([FromBody]PaginationContract contract)
        {
            try
            {
                var usr = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);
                var quotas = _quotaRepository
                    .GetWithInclude(q => q.ClientId == usr.Id, q => q.AdditionalContributions, q => q.City);
                var pagination = contract.Pagination;
                switch (pagination.SortBy)
                {
                    case "city":
                        quotas = pagination.Descending
                            ? quotas.OrderByDescending(q => q.City.Name)
                            : quotas.OrderBy(q => q.City.Name);
                        break;
                    case "refinancingAmount":
                        quotas = pagination.Descending
                            ? quotas.OrderByDescending(q => q.RefinancingAmount)
                            : quotas.OrderBy(q => q.RefinancingAmount);
                        break;
                    case "interestRate":
                        quotas = pagination.Descending
                            ? quotas.OrderByDescending(q => q.InterestRate)
                            : quotas.OrderBy(q => q.InterestRate);
                        break;
                    default:
                        quotas = pagination.Descending
                            ? quotas.OrderByDescending(q => q.CreatedAt)
                            : quotas.OrderBy(q => q.CreatedAt);
                        break;
                }

                if (pagination.RowsPerPage > 0)
                    quotas = quotas.Skip((pagination.Page - 1) * pagination.RowsPerPage).Take(pagination.RowsPerPage);
                quotas = quotas.ToList();
                foreach (var quota in quotas)
                {
                    quota.City = null;
                    quota.AdditionalContributions?.ForEach(ac =>
                    {
                        ac.Quota = null;
                        ac.Contribution = null;
                    });
                }

                return Ok(quotas);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        /// <summary>
        /// Get quota total count for current user
        /// </summary>
        /// <returns>Total count</returns>
        [Route("GetTotalElements")]
        [HttpPost]
        public async Task<IActionResult> GetTotalElements()
        {
            try
            {
                var usr = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);
                int count = await _quotaRepository.TotalElementsAsync(q=>q.ClientId == usr.Id);
                return Ok(count);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        /// <summary>
        /// Get lookups for quotas
        /// </summary>
        /// <returns>Lookups</returns>
        [Route("GetLookups")]
        [HttpPost]
        public async Task<IActionResult> GetLookups()
        {
            try
            {
                var result = new QuotaLookup
                {
                    RefinancingTargets = Enum.GetValues(typeof(RefinancingTarget)).Cast<object>()
                        .ToDictionary(enumValue => ((RefinancingTarget)enumValue).GetAttribute<DisplayAttribute>().Name,
                            enumValue => (int)enumValue),
                    Cities = await _cityRepository.GetAsync(),
                    Contributions = await _contributionRepository.GetAsync()
                };
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Create new quota for current user
        /// </summary>
        /// <param name="contract">QuotaContract</param>
        /// <returns></returns>
        [Route("Create")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]QuotaContract contract)
        {
            try
            {
                var quota = contract.Quota;
                var curentUser = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);
                quota.ClientId = curentUser.Id;
                await _quotaRepository.CreateAsync(quota);
                return Ok(quota.Id);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        /// <summary>
        /// Update quota
        /// </summary>
        /// <param name="contract">QuotaContract</param>
        /// <returns></returns>
        [Route("Update")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody]QuotaContract contract)
        {
            try
            {
                var quota = contract.Quota;
                var quotaForUpdate = await _quotaRepository.FindByIdAsync(quota.Id);
                if (quotaForUpdate == null)
                    return BadRequest("Item not found");
                var curentUser = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);
                quotaForUpdate.ClientId = curentUser.Id;
                quotaForUpdate.AdditionalContributions = quota.AdditionalContributions;
                quotaForUpdate.CityId = quota.CityId;
                quotaForUpdate.Comment = quota.Comment;
                quotaForUpdate.CreatedAt = quota.CreatedAt;
                quotaForUpdate.InterestRate = quota.InterestRate;
                quotaForUpdate.ModifiedAt = quota.ModifiedAt;
                quotaForUpdate.RefinancingAmount = quota.RefinancingAmount;
                quotaForUpdate.RefinancingTarget = quota.RefinancingTarget;

                await _quotaRepository.UpdateAsync(quotaForUpdate);
                return Ok();
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        /// <summary>
        /// Remove quota
        /// </summary>
        /// <param name="id">Quota Id</param>
        /// <returns></returns>
        [Route("Remove")]
        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            try
            {
                var quotaForRemove = await _quotaRepository.FindByIdAsync(id);
                if (quotaForRemove == null)
                    return BadRequest("Item not found");
                await _quotaRepository.RemoveAsync(quotaForRemove);
                return Ok();
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
    }
}