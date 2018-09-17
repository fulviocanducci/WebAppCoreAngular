using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppCoreAngular.Models;

namespace WebAppCoreAngular.Controllers
{
    public class PeopleController : Controller
    {
        private readonly DatabaseContext _context;

        public PeopleController(DatabaseContext context)
        {
            _context = context;
        }
        
        public async Task<IActionResult> Index()
        {
            return View(await _context.People.ToListAsync());
        }
        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var people = await _context.People
                .FirstOrDefaultAsync(m => m.Id == id);
            if (people == null)
            {
                return NotFound();
            }

            return View(people);
        }
       
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Phones")] People people)
        {           
            if (ModelState.IsValid)
            {
                _context.Add(people);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(people);
        }
       
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var people = await _context.People.FindAsync(id);               
            if (people == null)
            {
                return NotFound();
            }
            var phones = _context.Phone.Where(x => x.PeopleId == id)
                    .Select(x => new
                    {
                        id = x.Id, 
                        ddd = x.Ddd,
                        number = x.Number,
                        peopleId = x.PeopleId,
                        status = 'S'
                    })
                    .ToList();
            ViewBag.Phones = "[]";
            if (phones != null)
            {
                ViewBag.Phones = Newtonsoft.Json.JsonConvert.SerializeObject(phones);
            }
            return View(people);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] People people, IList<PhonesViewModel> phonesViewModel)
        {
            if (id != people.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(people);
                    IList<PhonesViewModel> phs = phonesViewModel.Where(x => x.Status != "S").ToList();
                    foreach (var ph in phs)
                    {
                        if (!string.IsNullOrEmpty(ph.Ddd) && !string.IsNullOrEmpty(ph.Number))
                        {
                            if (ph.Status == "D" && ph.PeopleId == id && ph.Id > 0)
                            {
                                Phone _d = ph;                                
                                _context.Phone.Remove(_d);                                
                            }
                            else if(ph.Ddd.Length == 2 && ph.Number.Length >= 8 && ph.PeopleId == id)
                            {
                                if (ph.Status == "N" && ph.Id == 0)
                                {
                                    Phone _n = ph;
                                    _context.Phone.Add(_n);                                   
                                }
                                else if (ph.Status == "A" && ph.Id > 0)
                                {
                                    Phone _u = ph;
                                    _context.Phone.Update(_u);
                                }
                            }
                        }
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PeopleExists(people.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Edit), id);
            }
            return View(people);
        }
               
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var people = await _context.People
                .FirstOrDefaultAsync(m => m.Id == id);
            if (people == null)
            {
                return NotFound();
            }

            return View(people);
        }
                
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var people = await _context.People.FindAsync(id);
            _context.People.Remove(people);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PeopleExists(int id)
        {
            return _context.People.Any(e => e.Id == id);
        }

    }
}
