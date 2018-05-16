using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using QuotationManager.Web.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuotationManager.DataAccess.Repository;
using QuotationManager.Models;
using QuotationManager.Models.Enums;
using QuotationManager.Web.Models;

namespace QuotationManager.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/Quota")]
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
        /// <returns>List of quotas</returns>
        [Route("Get")]
        [HttpPost]
        public async Task<IActionResult> Get()
        {
            try
            {
                var usr = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);
                var quotas = _quotaRepository.GetWithInclude(q => q.ClientId == usr.Id, q => q.AdditionalContributions, q=>q.City).ToList();
                return Ok(quotas);
            }
            catch (Exception exception)
            {
                //Log.Instance.Trace(exception, logToken);
                return BadRequest(exception.Message);
            }
        }

        [Route("GetLookups")]
        [HttpPost]
        public async Task<IActionResult> GetLookups()
        {
            var result = new QuotaLookup
            {
                RefinancingTargets = Enum.GetValues(typeof(RefinancingTarget)).Cast<object>()
                    .ToDictionary(enumValue => ((RefinancingTarget) enumValue).GetAttribute<DisplayAttribute>().Name,
                        enumValue => (int) enumValue),
                Cities = await _cityRepository.GetAsync(),
                Contributions = await _contributionRepository.GetAsync()
            };
            return Ok(result);
        }
    }
}