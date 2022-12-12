using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Recursos_Humanos.Models;

namespace Recursos_Humanos.Controllers
{
    public class ColaboradorController : Controller
    {
        private readonly AppDbContext _context;
        public ColaboradorController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> ColaboradorIndex(string Filtro, int DepartamentoId)
        {
            List<Colaborador> colaboradores =
                await _context.Colaboradores.Include(c => c.Departamento).ToListAsync();
            if (Filtro != null && DepartamentoId == 0)
            {
                //Filtro solo por Palabra
                colaboradores = await _context.Colaboradores
                    .Where(c => c.Rut.Contains(Filtro) 
                    || c.Nombres.Contains(Filtro) 
                    || c.Apellidos.Contains(Filtro)
                    || c.Correo.Contains(Filtro)
                    || c.Telefono.Contains(Filtro)
                    || c.Direccion.Contains(Filtro)
                    || c.Comuna.Contains(Filtro))
                    .Include(c => c.Departamento).ToListAsync();
            }
            if(Filtro != null && DepartamentoId > 0)
            {
                //Filtro por Palabra y Dpto
                colaboradores = await _context.Colaboradores
                    .Where(c => (c.Rut.Contains(Filtro)
                    || c.Nombres.Contains(Filtro)
                    || c.Apellidos.Contains(Filtro)
                    || c.Correo.Contains(Filtro)
                    || c.Telefono.Contains(Filtro)
                    || c.Direccion.Contains(Filtro)
                    || c.Comuna.Contains(Filtro)) && (c.DepartamentoId == DepartamentoId))
                    .Include(c => c.Departamento).ToListAsync();
            }
            if(Filtro == null && DepartamentoId > 0)
            {
                //Filtro solo por Dpto
                colaboradores = await _context.Colaboradores
                    .Where(c => c.DepartamentoId == DepartamentoId)
                    .Include(c => c.Departamento).ToListAsync();
            }

            ViewData["Departamentos"] = new SelectList(await _context.Departamentos.ToListAsync(), "DepartamentoId", "NombreDepartamento");
            return View(colaboradores);
        }

        public async Task<IActionResult> ColaboradorCreate()
        {
            ViewData["Departamentos"] = new SelectList(await _context.Departamentos.ToListAsync(), "DepartamentoId", "NombreDepartamento");
            return View();
        }

        public async Task<IActionResult> ColaboradorEdit(string Rut)
        {
            if(Rut == null) { return View(); } else
            {
                var colaborador = await _context.Colaboradores.FirstOrDefaultAsync(x => x.Rut == Rut);
                ViewData["Departamentos"] = new SelectList(await _context.Departamentos.ToListAsync(), "DepartamentoId", "NombreDepartamento");
                return View(colaborador);
            }
        }

        public async Task<IActionResult> ColaboradorDelete(string Rut)
        {
            if (Rut == null) { return NotFound(); }
            else
            {
                var colaborador = await _context.Colaboradores.FirstOrDefaultAsync(x => x.Rut == Rut);
                if (colaborador == null) { return NotFound(); } 
                else
                {
                    _context.Remove(colaborador);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("ColaboradorIndex");
                }
                
            }
        }


        [HttpPost]
        public async Task<IActionResult> ColaboradorCreate(Colaborador C)
        {
            if (ModelState.IsValid)
            {
                var colaborador = await _context.Colaboradores.FirstOrDefaultAsync(x => x.Rut == C.Rut);
                if (colaborador == null)
                {
                    _context.Add(C);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("ColaboradorIndex");
                }
                else
                {
                    ModelState.AddModelError("", "Ya existe un colaborador con ese Rut");
                    return View(C);
                }
            }
            else
            {
                ViewData["Departamentos"] = new SelectList(await _context.Departamentos.ToListAsync(), "DepartamentoId", "NombreDepartamento");
                ModelState.AddModelError("", "Error al Guardar");
                return View(C);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ColaboradorEdit(Colaborador C)
        {
            if (ModelState.IsValid)
            {
                _context.Update(C);
                await _context.SaveChangesAsync();
                return RedirectToAction("ColaboradorIndex");
            }
            else
            {
                ViewData["Departamentos"] = new SelectList(await _context.Departamentos.ToListAsync(), "DepartamentoId", "NombreDepartamento");
                ModelState.AddModelError("", "Error al Guardar");
                return View(C);
            }
        }
    }
}
