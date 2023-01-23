using Microsoft.AspNetCore.Mvc;
using PhoneBook.Contact.Dtos.Enums;
using PhoneBook.Contact.Dtos.Model.Contact;
using PhoneBook.Contact.Entities.Db;
using PhoneBook.Contact.Repositories.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PhoneBook.Contact.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        #region Ctor&Fields

        private readonly IContactInfoRepository _contactInfoRepository;
        private readonly IContactDetailRepository _contactDetailRepository;

        public ContactsController(IContactInfoRepository contactInfoRepository, IContactDetailRepository contactDetailRepository)
        {
            _contactInfoRepository = contactInfoRepository;
            _contactDetailRepository = contactDetailRepository;
        }

        #endregion

        #region Add

        [HttpPost("Add")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        public async Task<IActionResult> AddAsync(ContactModel model)
        {
            await _contactInfoRepository.AddAsync(new ContactInfo()
            {
                Company = model.ContactCompany,
                CreateDate = DateTime.Now,
                IsActive = true,
                LastName = model.ContactLastName,
                Name = model.ContactName
            });

            await _contactInfoRepository.SaveChangeAsync();

            return Ok();
        }

        #endregion

        #region GetAll

        [HttpPost("GetAll")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<ContactListModel>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAllAsync(ContactInfoCustomSearchModel model)
        {
            var getAllData = await _contactInfoRepository.CustomSearchAsync(model);

            var responseModel = getAllData.Select(i => new ContactListModel()
            {
                ContactId = i.Id,
                ContactCompany = i.Company,
                ContactLastName = i.LastName,
                ContactName = i.Name
            });

            return Ok(responseModel);
        }

        #endregion

        #region GetAllContactByLocation

        [HttpGet("GetAllContactByLocation/{Location}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ContactReportModel))]
        public async Task<IActionResult> GetAllContactByLocationAsync(string Location)
        {
            ContactReportModel reportModel = new();
            reportModel.Location = Location;

            var getAllContactDetailData = await _contactDetailRepository.GetListAsync(i => i.IsActive && i.ContactTypeId == (int)StaticContactTypeEnm.Location && i.Text == Location);
            if (getAllContactDetailData.Any())
            {
                var onlyContactIds = getAllContactDetailData.Select(i => i.ContactId).ToList();

                var getAllContactData = await _contactInfoRepository.GetListAsync(i=>i.IsActive&&onlyContactIds.Contains(i.Id));

                reportModel.TotalContactCount = getAllContactData.Count();

                var getAllContactDetailForPhone = await _contactDetailRepository.GetListAsync(i=>i.IsActive&&onlyContactIds.Contains(i.ContactId)&& i.ContactTypeId == (int)StaticContactTypeEnm.PhoneNumber);

                reportModel.TotalPhoneNumberCount = getAllContactDetailForPhone.Count;

            }

            return Ok(reportModel);
        }

        #endregion

        #region Delete

        [HttpDelete("Delete")]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteAsync(Guid Id)
        {
            var getOldData = await _contactInfoRepository.GetAsync(i => i.IsActive && i.Id == Id);
            if (getOldData == default)
            {
                return NotFound();
            }

            getOldData.UpdateDate = DateTime.Now;
            getOldData.IsActive = false;

            await _contactInfoRepository.UpdateAsync(getOldData);
            await _contactInfoRepository.SaveChangeAsync();

            return Ok();
        }

        #endregion

    }
}
