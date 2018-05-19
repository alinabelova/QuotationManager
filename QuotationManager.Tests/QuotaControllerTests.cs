using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using QuotationManager.DataAccess.Repository;
using QuotationManager.Models;
using QuotationManager.Models.Enums;
using QuotationManager.Web.Contracts;
using QuotationManager.Web.Controllers;
using Xunit;

namespace QuotationManager.Tests
{
    public class QuotaControllerTests
    {
        private readonly Mock<IRepository<Quota>> _quotaRepository;
        private readonly Mock<FakeUserManager> _fakeUserManager;

        public QuotaControllerTests()
        {
            var users = new List<ApplicationUser>
            {
                new ApplicationUser()
                {
                    UserName = "test@test.it",
                    Id = "dddc9f1e-63c5-4715-b60f-1cf2f7fe93f2",
                    Email = "test@test.it"
                },
                new ApplicationUser()
                {
                    UserName = "user@user.it",
                    Id = "aaac9f1e-63c5-4715-b60f-1cf2f7fe93g5",
                    Email = "user@user.it"
                },
                new ApplicationUser()
                {
                    UserName = "new@new.it",
                    Id = "abcd9f1e-63c5-4715-b60f-1cf2f7fe93m8",
                    Email = "new@new.it"
                }
            }.AsQueryable();

            _quotaRepository = new Mock<IRepository<Quota>>();
            _quotaRepository.Setup(repo => repo.GetAsync()).ReturnsAsync(GetTestQuotas());
            _fakeUserManager = new Mock<FakeUserManager>();

            _fakeUserManager.Setup(x => x.Users)
                .Returns(users);
        }

        [Theory]
        [InlineData("test@test.it", "dddc9f1e-63c5-4715-b60f-1cf2f7fe93f2", 3)]
        [InlineData("user@user.it", "aaac9f1e-63c5-4715-b60f-1cf2f7fe93g5", 1)]
        public void GetTotalElementsReturnsValidCount(string username, string userId, int count)
        {
            _fakeUserManager.Setup(x => x.FindByEmailAsync(username)).ReturnsAsync(_fakeUserManager.Object.Users.FirstOrDefault(u => u.Id == userId));

            _quotaRepository.Setup(repo => repo.TotalElementsAsync(It.IsAny<Expression<Func<Quota, bool>>>())).ReturnsAsync(GetTestQuotas().Count(q => q.ClientId == userId));
            var controller = new QuotaController(_fakeUserManager.Object, _quotaRepository.Object, null, null);
            controller.ControllerContext = MockContext(username);
            var result = controller.GetTotalElements().GetAwaiter().GetResult();
            Assert.Equal(count, (result as OkObjectResult)?.Value);
        }

        [Fact]
        public void GetElementsReturnsValidList()
        {
            string username = "test@test.it";
            string userId = "dddc9f1e-63c5-4715-b60f-1cf2f7fe93f2";

            _fakeUserManager.Setup(x => x.FindByEmailAsync(username)).ReturnsAsync(_fakeUserManager.Object.Users.FirstOrDefault(u => u.Id == userId));
            _quotaRepository.Setup(repo => repo.GetWithInclude(It.IsAny<Func<Quota, bool>>(), It.IsAny<Expression<Func<Quota, object>>[]>())).Returns(GetTestQuotas().Where(q => q.ClientId == userId));

            var controller = new QuotaController(_fakeUserManager.Object, _quotaRepository.Object, null, null);
            controller.ControllerContext = MockContext(username);
            var pagination = new PaginationContract()
            {
                Pagination = new Pagination() {Descending = true, Page = 1, RowsPerPage = 2, SortBy = "interestRate"}
            };
            var result = controller.Get(pagination).GetAwaiter().GetResult() as OkObjectResult;
            var lastOnPage = (result?.Value as List<Quota>)?.LastOrDefault();
            Assert.Equal(new decimal(12.6), lastOnPage?.InterestRate);
        }

        [Fact]
        public void CreateReturnsNewQuota()
        {
            string username = "test@test.it";
            string userId = "dddc9f1e-63c5-4715-b60f-1cf2f7fe93f2";
            var quotas = GetTestQuotas();
            _fakeUserManager.Setup(x => x.FindByEmailAsync(username)).ReturnsAsync(_fakeUserManager.Object.Users.FirstOrDefault(u => u.Id == userId));
            _quotaRepository.Setup(repo => repo.CreateAsync(It.IsAny<Quota>()))
                .Callback((Quota quota) => { quotas.Add(quota); }).Returns(Task.FromResult(0));

            var newQuota = new Quota
            {
                Id = 5,
                InterestRate = new decimal(17.3),
                RefinancingAmount = new decimal(53255.0),
                RefinancingTarget = RefinancingTarget.ConsumerLoan,
                CityId = 5
            };

            var controller =
                new QuotaController(_fakeUserManager.Object, _quotaRepository.Object, null, null)
                {
                    ControllerContext = MockContext(username)
                };
            controller.Create(new QuotaContract(){Quota = newQuota}).GetAwaiter().GetResult();
            Assert.Equal(5, quotas.LastOrDefault()?.Id);
        }

        [Fact]
        public void DeleteReturnsDeletedQuota()
        {
            string username = "test@test.it";
            string userId = "dddc9f1e-63c5-4715-b60f-1cf2f7fe93f2";
            var quotas = GetTestQuotas();
            _fakeUserManager.Setup(x => x.FindByEmailAsync(username))
                .ReturnsAsync(_fakeUserManager.Object.Users.FirstOrDefault(u => u.Id == userId));

            _quotaRepository.Setup(repo => repo.FindByIdAsync(It.IsAny<int>())).ReturnsAsync(quotas[1]);
            _quotaRepository.Setup(repo => repo.RemoveAsync(It.IsAny<Quota>()))
                .Callback((Quota quota) => { quotas.Remove(quota); }).Returns(Task.FromResult(0));

            var controller =
                new QuotaController(_fakeUserManager.Object, _quotaRepository.Object, null, null)
                {
                    ControllerContext = MockContext(username)
                };
            controller.Remove(quotas[1].Id).GetAwaiter().GetResult();
            Assert.Equal(3, quotas.Count);
        }

        [Fact]
        public void UpdateReturnsUpdatedQuota()
        {
            string username = "test@test.it";
            string userId = "dddc9f1e-63c5-4715-b60f-1cf2f7fe93f2";
            string comment = "test comment";
            var quotas = GetTestQuotas();
            _fakeUserManager.Setup(x => x.FindByEmailAsync(username))
                .ReturnsAsync(_fakeUserManager.Object.Users.FirstOrDefault(u => u.Id == userId));

            _quotaRepository.Setup(repo => repo.FindByIdAsync(It.IsAny<int>())).ReturnsAsync(quotas[1]);
            _quotaRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Quota>()))
                .Callback((Quota quota) =>
                {
                    quotas[1].Comment = quota.Comment;
                    quotas[1].RefinancingTarget = quota.RefinancingTarget;
                    quotas[1].InterestRate = quota.InterestRate;
                    quotas[1].CityId = quota.CityId;
                }).Returns(Task.FromResult(0));

            var controller =
                new QuotaController(_fakeUserManager.Object, _quotaRepository.Object, null, null)
                {
                    ControllerContext = MockContext(username)
                };

            quotas[1].Comment = comment;
            quotas[1].RefinancingTarget = RefinancingTarget.Mortgage;
            quotas[1].InterestRate = new decimal(25.0);
            quotas[1].CityId = 1;

            controller.Update(new QuotaContract(){Quota = quotas[1]}).GetAwaiter().GetResult();
            Assert.Equal(comment, quotas[1].Comment);
        }

        private static List<Quota> GetTestQuotas()
        {
            string client1Id = "dddc9f1e-63c5-4715-b60f-1cf2f7fe93f2";
            string client2Id = "aaac9f1e-63c5-4715-b60f-1cf2f7fe93g5";
            var quotas = new List<Quota>
            {
                new Quota { Id=1, ClientId = client1Id, InterestRate = new decimal(12.6), RefinancingAmount = new decimal(156789.5), CreatedAt = new DateTime(2018, 5, 18, 9, 34, 0)},
                new Quota { Id=2, ClientId = client1Id, InterestRate = new decimal(14.9), RefinancingAmount = new decimal(15000.0), CreatedAt = new DateTime(2018, 4, 14, 19, 34, 0)},
                new Quota { Id=3, ClientId = client2Id, InterestRate = new decimal(8.7), RefinancingAmount = new decimal(56897.5), CreatedAt = new DateTime(2018, 5, 1, 9, 14, 0)},
                new Quota { Id=4, ClientId = client1Id, InterestRate = new decimal(2.4), RefinancingAmount = new decimal(345671.0), CreatedAt = new DateTime(2018, 3, 18, 9, 34, 0)}
            };
            return quotas;
        }
        private static ControllerContext MockContext(string userName)
        {
            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, userName)
                    }, null))
                }
            };
            return context;
        }
    }
}
