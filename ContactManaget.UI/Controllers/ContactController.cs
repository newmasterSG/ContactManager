using ContactManager.Application.DTO;
using ContactManager.Application.InterfacesServices;
using ContactManager.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ContactManaget.UI.Controllers
{
    public class ContactController : Controller
    {
        private readonly IContactService _contactService;
        private readonly UserManager<UserEntity> _userManager;
        private readonly ICsvService _csvService;

        public ContactController(IContactService contactService,
            UserManager<UserEntity> userManager,
            ICsvService csvService)
        {
            _contactService = contactService;
            _userManager = userManager;
            _csvService = csvService;
        }

        public async Task<IActionResult> Table()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var contacts = _contactService.GetByUserId(user.Id);

            return View(contacts);
        }

        [HttpPost]
        public async Task<IActionResult> Add(IFormFile file)
        {

            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("File", "Please select a valid CSV file.");
                return View("Table", _contactService.GetByUserId(user.Id));
            }


            var contacts = await _csvService.ReadContactsFromCsv(file);

            foreach (var contact in contacts)
            {
                contact.User = user;
                await _contactService.CreateAsync(contact);
            }

            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContactEntity>>> GetAll()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var contacts = _contactService.GetByUserId(user.Id);

            return Ok(contacts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContactEntity>> GetById(int id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var contacts = await _contactService.GetByIdAsync(id);
            if (contacts == null)
                return NotFound();

            return Ok(contacts);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteById(int id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var contact = await _contactService.GetByIdAsync(id);
            if (contact == null)
                return NotFound();

            await _contactService.DeleteAsync(contact.Id);

            return Ok();
        }

        public async Task<IActionResult> Update([FromBody] ContactDTO contact)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            contact.User = user;

            await _contactService.UpdateUrlAsync(contact);

            return Ok();
        }
    }
}
