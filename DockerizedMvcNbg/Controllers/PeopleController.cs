using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DockerizedMvcNbg.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text;
using System.Text.Json;

namespace DockerizedMvcNbg.Controllers;
 
    public class PeopleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDistributedCache _cache;
        private readonly ILogger<PeopleController> _logger;

        public PeopleController(ApplicationDbContext context, IDistributedCache cache,
            ILogger<PeopleController> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        // GET: People
        public async Task<IActionResult> Index()
        {
        List<Person>? persons = null;
        var cachedData = await _cache.GetAsync($"person-all");
        if (cachedData != null)
        {
            var json = Encoding.UTF8.GetString(cachedData);
            persons = JsonSerializer.Deserialize<List<Person>>(json);
            _logger.LogInformation($"cache was used by index method");
        }
        if (persons == null)
        {
            persons = await _context.Persons.Take(50).ToListAsync();
            _logger.LogInformation($"db was used");
            var jsonData = JsonSerializer.Serialize(persons);
            var encodedData = Encoding.UTF8.GetBytes(jsonData);
            await _cache.SetAsync($"person-all", encodedData);
        }

     




        return View(persons);
        }

        // GET: People/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

        ///////////////
            Person? person=null;
            var cachedData = await _cache.GetAsync($"person-{id}");
        if (cachedData != null)
        {
            var json = Encoding.UTF8.GetString(cachedData);
            person = JsonSerializer.Deserialize<Person>(json);
            _logger.LogInformation($"cache was used");
        }
        if (person==null)
        {
             person = await _context.Persons .FirstOrDefaultAsync(m => m.Id == id);
            _logger.LogInformation($"db was used");
            var jsonData = JsonSerializer.Serialize(person);
            var encodedData = Encoding.UTF8.GetBytes(jsonData);
            await _cache.SetAsync($"person-{id}", encodedData);
        }

        if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // GET: People/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: People/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Person person)
        {
            if (ModelState.IsValid)
            {
                _context.Add(person);
                await _context.SaveChangesAsync();
                await _cache.RemoveAsync($"person-all");
                return RedirectToAction(nameof(Index));
            }
            return View(person);
        }

        // GET: People/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Persons.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }
            return View(person);
        }

        // POST: People/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Person person)
        {
            if (id != person.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(person);
                    await _context.SaveChangesAsync();
                //     await _cache.RemoveAsync($"products-{id}");
              await _cache.RemoveAsync($"person-all");
                var jsonData = JsonSerializer.Serialize(person);
                var encodedData = Encoding.UTF8.GetBytes(jsonData);
                await _cache.SetAsync($"person-{id}", encodedData);


              




            }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonExists(person.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(person);
        }

        // GET: People/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Persons
                .FirstOrDefaultAsync(m => m.Id == id);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // POST: People/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var person = await _context.Persons.FindAsync(id);
            if (person != null)
            {
                _context.Persons.Remove(person);
            }

            await _context.SaveChangesAsync();
            await _cache.RemoveAsync($"person-all");
            await _cache.RemoveAsync($"person-{id}");


        return RedirectToAction(nameof(Index));
        }

        private bool PersonExists(int id)
        {
            return _context.Persons.Any(e => e.Id == id);
        }
    }
 
