using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FerramentariaTest.DAL;
using FerramentariaTest.Entities;
using AutoMapper;
using FerramentariaTest.Models;
using FerramentariaTest.Helpers;
using X.PagedList;
using FerramentariaTest.EntitiesBS;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Globalization;
using SelectPdf;

namespace FerramentariaTest.Controllers
{
    public class ObrasController : Controller
    {
        private readonly ContextoBanco _context;
        private readonly ContextoBancoBS _contextBS;
        protected IHttpContextAccessor httpContextAccessor;
        private readonly IConfiguration _configuration;
        private static string pagina = "thObra.aspx";
        private MapperConfiguration mapper;
        private readonly IMapper _mapper;

        private const string SessionKeyLoggedUserInformation = "LoggedUserInformation";


        public ObrasController(ContextoBanco context, ContextoBancoBS contextBS, IHttpContextAccessor httpCA, IConfiguration configuration)
        {
            _context = context;
            _contextBS = contextBS;
            httpContextAccessor = httpCA;
            _configuration = configuration;
            mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Obra, ObraViewModel>();
                cfg.CreateMap<ObraViewModel, Obra>()
                    .ForMember(dest => dest.DataRegistro, opt => opt.MapFrom(src => DateTime.Now));
            });
            _mapper = mapper.CreateMapper();
        }


        public IActionResult Index(int? page)
        {
            Log log = new Log();
            log.LogWhat = pagina + "/Index";
            log.LogWhere = pagina;
            Auxiliar auxiliar = new Auxiliar(_context, _contextBS, httpContextAccessor, _configuration);

            try
            {
                LoggedUserData? loggedUser = HttpContext.Session.GetObject<LoggedUserData>(SessionKeyLoggedUserInformation) ?? new LoggedUserData();
                if (loggedUser != null)
                {
                    log.LogWho = loggedUser.Id.ToString();
                    PermissionAccessModel? checkPermission = loggedUser?.ListOfPermissionAccess?.FirstOrDefault(i => i.Pagina == pagina);
                    if (checkPermission != null)
                    {
                        if (checkPermission.Visualizar == 1)
                        {
                            if (TempData.ContainsKey("ErrorMessage"))
                            {
                                ViewBag.Error = TempData["ErrorMessage"]?.ToString();
                                TempData.Remove("ErrorMessage"); // Remove it from TempData to avoid displaying it again
                            }

                            if (TempData.ContainsKey("ShowSuccessAlert"))
                            {
                                ViewBag.ShowSuccessAlert = TempData["ShowSuccessAlert"]?.ToString();
                                TempData.Remove("ShowSuccessAlert"); // Remove it from TempData to avoid displaying it again
                            }

                            if (TempData.ContainsKey("SuccessAlertObra"))
                            {
                                ViewBag.SuccessAlertNew = TempData["SuccessAlertObra"]?.ToString();
                                TempData.Remove("SuccessAlertObra"); // Remove it from TempData to avoid displaying it again
                            }


                            return View();
                        }
                        else
                        {
                            log.LogWhy = $"No Permission for Page:{pagina}";
                            auxiliar.GravaLogErro(log);
                            return RedirectToAction("PreserveActionError", "Home", new { Error = $"No Permission for Page:{pagina}" });
                        }
                    }
                    else
                    {
                        log.LogWhy = "Permission is Empty";
                        auxiliar.GravaLogErro(log);
                        return RedirectToAction("PreserveActionError", "Home", new { Error = "Permission is Empty" });
                    }
                }
                else
                {
                    log.LogWhy = "Session Expired";
                    auxiliar.GravaLogErro(log);
                    return RedirectToAction("PreserveActionError", "Home", new { Error = "Session Expired" });
                }          
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }


        [HttpPost]
        public IActionResult GetObraList(string? filter, int? status, int? pagination, int page)
        {

            try
            {
                int pageSize = pagination ?? 10;

                Obra[]? result = ObraListResult(filter, status, pagination);
                if (result.Length > 0)
                {

                    int totalItems = result.Count();
                    result = result.Skip(page * pageSize).Take(pageSize).ToArray();


                    return Json(new { success = true, result, totalItems });
                }
                else
                {
                    return Json(new { success = false, message = "Nenhum resultado encontrado." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"GetObraList Error: {ex.Message}" });
            }

       
            //return Json(new { success = false, message = "Unexpected error occurred." });
        }

        public Obra[] ObraListResult(string? filter, int? status,int? pagination)
        {
            Obra[]? ObraResult = _context.Obra.Where(i =>
                                        (status == null || i.Ativo == status)
                                        && (filter == null || i.Nome.Contains(filter) || i.Codigo.Contains(filter))
                                         ).ToArray() ?? new Obra[0];

            return ObraResult;
        }

        [HttpGet]
        public IActionResult checkCodigoDuplicate(string? codigo)
        {
            try
            {
                bool isDuplicate = _context.Obra.Any(m => m.Codigo == codigo);

                if (!isDuplicate)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = $"{codigo} duplicado." });
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"checkCodigoDuplicate Error: {ex.Message}" });
            }
        }

        [HttpGet]
        public IActionResult checkNomeDuplicate(string? nome)
        {
            try
            {
                bool isDuplicate = _context.Obra.Any(m => m.Nome == nome);

                if (!isDuplicate)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = $"{nome} duplicado." });
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"checkCodigoDuplicate Error: {ex.Message}" });
            }
        }

        [HttpGet]
        public IActionResult checkCodigoDuplicateEdit(string? codigo, int? id)
        {
            try
            {
                bool isDuplicate = _context.Obra.Any(m => m.Id != id && m.Codigo == codigo);


                if (!isDuplicate)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = $"{codigo} duplicado." });
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"checkCodigoDuplicate Error: {ex.Message}" });
            }
        }

        [HttpGet]
        public IActionResult checkNomeDuplicateEdit(string? nome, int? id)
        {
            try
            {
                bool isDuplicate = _context.Obra.Any(m => m.Id != id && m.Nome == nome);


                if (!isDuplicate)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = $"{nome} duplicado." });
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"checkCodigoDuplicate Error: {ex.Message}" });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ObraViewModel obra)
        {
            Log log = new Log();
            log.LogWhat = pagina + "/Create";
            log.LogWhere = pagina;
            log.LogWhy = "Insert Obra";
            Auxiliar auxiliar = new Auxiliar(_context, _contextBS, httpContextAccessor, _configuration);

            try
            {
                LoggedUserData? loggedUser = HttpContext.Session.GetObject<LoggedUserData>(SessionKeyLoggedUserInformation) ?? new LoggedUserData();
                if (loggedUser != null)
                {
                    log.LogWho = loggedUser.Id.ToString();
                    PermissionAccessModel? checkPermission = loggedUser?.ListOfPermissionAccess?.FirstOrDefault(i => i.Pagina == pagina);
                    if (checkPermission != null)
                    {
                        if (checkPermission.Inserir == 1)
                        {
                            using (var transaction = _context.Database.BeginTransaction())
                            {
                                try
                                {
                                    Obra entity = _mapper.Map<Obra>(obra);
                                    entity.Ativo = 1;
                                    _context.Obra.Add(entity);
                                    _context.SaveChanges();
                                 

                                    log.LogWho = loggedUser.Id.ToString();
                                    log.LogHowMuch = entity.Id.ToString();
                                    LogTransaction(log);

                                    transaction.Commit();

                                    //ViewBag.SuccessAlertNew = "Cadastro Novo Obra Successo!";

                                    TempData["SuccessAlertObra"] = "Cadastro Novo Obra Successo!";
                                    return RedirectToAction(nameof(Index));
                                    //return View(nameof(Index));
                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback();
                                    ViewBag.Error = $"SERVER ERROR: {ex.Message}";
                                    return View(nameof(Index));
                                }
                            }                              
                        }
                        else
                        {
                            log.LogWhy = $"No Permission for Page:{pagina}";
                            auxiliar.GravaLogErro(log);
                            return RedirectToAction("PreserveActionError", "Home", new { Error = $"No Permission for Page:{pagina}" });
                        }
                    }
                    else
                    {
                        log.LogWhy = "Permission is Empty";
                        auxiliar.GravaLogErro(log);
                        return RedirectToAction("PreserveActionError", "Home", new { Error = "Permission is Empty" });
                    }
                }
                else
                {
                    log.LogWhy = "Session Expired";
                    auxiliar.GravaLogErro(log);
                    return RedirectToAction("PreserveActionError", "Home", new { Error = "Session Expired" });
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditObraChange(ObraViewModel obra)
        {
            Log log = new Log();
            log.LogWhat = pagina + "/EditObraChange";
            log.LogWhere = pagina;
            log.LogWhy = "Edit Obra";
            Auxiliar auxiliar = new Auxiliar(_context, _contextBS, httpContextAccessor, _configuration);

            try
            {
                LoggedUserData? loggedUser = HttpContext.Session.GetObject<LoggedUserData>(SessionKeyLoggedUserInformation) ?? new LoggedUserData();
                if (loggedUser != null)
                {
                    log.LogWho = loggedUser.Id.ToString();
                    PermissionAccessModel? checkPermission = loggedUser?.ListOfPermissionAccess?.FirstOrDefault(i => i.Pagina == pagina);
                    if (checkPermission != null)
                    {
                        if (checkPermission.Editar == 1)
                        {
                            using (var transaction = _context.Database.BeginTransaction())
                            {
                                try
                                {
                                    Obra? entityToEdit = _context.Obra.FirstOrDefault(i => i.Id == obra.Id);
                                    if (entityToEdit != null)
                                    {
                                        entityToEdit.Codigo = obra.Codigo;
                                        entityToEdit.Nome = obra.Nome;
                                        _context.Update(entityToEdit);
                                        _context.SaveChanges();

                                        log.LogWho = loggedUser.Id.ToString();
                                        log.LogHowMuch = entityToEdit.Id.ToString();
                                        LogTransaction(log);

                                        transaction.Commit();

                                        TempData["SuccessAlertObra"] = $"Editar Obra {entityToEdit.Id} Successo!";
                                        return RedirectToAction(nameof(Index));
                                    }
                                    else
                                    {
                                        throw new Exception($"Entity with ID:{obra.Id} not found");
                                    }


                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback();
                                    ViewBag.Error = $"SERVER ERROR: {ex.Message}";
                                    return View(nameof(Index));
                                }
                            }
                        }
                        else
                        {
                            log.LogWhy = $"No Permission for Page:{pagina}";
                            auxiliar.GravaLogErro(log);
                            return RedirectToAction("PreserveActionError", "Home", new { Error = $"No Permission for Page:{pagina}" });
                        }
                    }
                    else
                    {
                        log.LogWhy = "Permission is Empty";
                        auxiliar.GravaLogErro(log);
                        return RedirectToAction("PreserveActionError", "Home", new { Error = "Permission is Empty" });
                    }
                }
                else
                {
                    log.LogWhy = "Session Expired";
                    auxiliar.GravaLogErro(log);
                    return RedirectToAction("PreserveActionError", "Home", new { Error = "Session Expired" });
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(nameof(Index));
            }
        }




        public void LogTransaction(Log log)
        {
            log.LogHow = "SUCESSO";
            log.LogWhen = DateTime.Now.ToString("u", DateTimeFormatInfo.InvariantInfo);
            _context.Add(log);
            _context.SaveChanges();
        }






    }
}
