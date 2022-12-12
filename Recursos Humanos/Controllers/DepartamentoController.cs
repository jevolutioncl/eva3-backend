using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recursos_Humanos.Models;

namespace Recursos_Humanos.Controllers
{
    public class DepartamentoController : Controller
    {
        private readonly AppDbContext _context;
        public DepartamentoController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> DepartamentoIndex(string Filtro)
        {
            var departamentos = await _context.Departamentos.ToListAsync();
            if (Filtro != null)
            {
                departamentos = await _context.Departamentos
                    .Where(d=>d.NombreDepartamento.Contains(Filtro) || d.AreaDepartamento.Contains(Filtro))
                    .ToListAsync();
            }
            return View(departamentos);
        }

        public IActionResult DepartamentoCreate()
        {
            return View();
        }

        public async Task<IActionResult> DepartamentoEdit(int DepartamentoId)
        {
            if (DepartamentoId == 0) { return View(); }
            else
            {
                var departamento = await _context.Departamentos.FirstOrDefaultAsync(x => x.DepartamentoId == DepartamentoId);
                return View(departamento);
            }
        }

        public async Task<IActionResult> DepartamentoDelete(int DepartamentoId)
        {
            if (DepartamentoId == 0) { return NotFound(); }
            else
            {
                var departamento = await _context.Departamentos.FirstOrDefaultAsync(x => x.DepartamentoId == DepartamentoId);
                if (departamento == null) { return NotFound(); }
                else
                {
                    _context.Remove(departamento);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("DepartamentoIndex");
                }

            }
        }


        [HttpPost]
        public async Task<IActionResult> DepartamentoCreate(Departamento D)
        {
            if (ModelState.IsValid)
            {
                var departamento = _context.Departamentos.FirstOrDefault(x => x.NombreDepartamento == D.NombreDepartamento);
                if (departamento == null)
                {
                    _context.Add(D);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("DepartamentoIndex");
                }
                else
                {
                    ModelState.AddModelError("", "Ya existe un Departamento con ese Nombre");
                    return View(D);
                }
            }
            else
            {
                ModelState.AddModelError("", "Error al Guardar");
                return View(D);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DepartamentoEdit(Departamento D)
        {
            if (ModelState.IsValid)
            {
                _context.Update(D);
                await _context.SaveChangesAsync();
                return RedirectToAction("DepartamentoIndex");
            }
            else
            {
                ModelState.AddModelError("", "Error al Guardar");
                return View(D);
            }
        }

    }
}
