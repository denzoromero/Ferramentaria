using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FerramentariaTest.DAL;
using FerramentariaTest.Entities;
using FerramentariaTest.Helpers;
using FerramentariaTest.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using AutoMapper;
using X.PagedList;
using FerramentariaTest.EntitiesBS;
using System.Globalization;

namespace FerramentariaTest.Controllers
{
    public class MarcasController : Controller
    {
        private readonly ContextoBanco _context;
        private readonly ContextoBancoBS _contextBS;
        protected IHttpContextAccessor httpContextAccessor;
        private readonly IConfiguration _configuration;
        private static string pagina = "Generico.aspx?TOOLHOUSE.Marca";
        private MapperConfiguration mapper;
        private readonly IMapper _mapper;

        private const string SessionKeyLoggedUserInformation = "LoggedUserInformation";

        public MarcasController(ContextoBanco context, ContextoBancoBS contextBS, IHttpContextAccessor httpCA, IConfiguration configuration)
        {
            _context = context;
            _contextBS = contextBS;
            httpContextAccessor = httpCA;
            _configuration = configuration;
            mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Marca, MarcaViewModel>();
                cfg.CreateMap<MarcaViewModel, Marca>()
                    .ForMember(dest => dest.DataRegistro, opt => opt.MapFrom(src => DateTime.Now));
            });
            _mapper = mapper.CreateMapper();
        }

        public IActionResult Index(string filter, int status, int? page)
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

                            if (TempData.ContainsKey("SuccessAlertMarca"))
                            {
                                ViewBag.SuccessAlertNew = TempData["SuccessAlertMarca"]?.ToString();
                                TempData.Remove("SuccessAlertMarca"); // Remove it from TempData to avoid displaying it again
                            }

                            log.LogWhy = "Acesso Permitido";
                            auxiliar.GravaLogSucesso(log);

                            return View();
                        }
                        else
                        {
                            return RedirectToAction("PreserveActionError", "Home", new { Error = $"No Permission for Page:{pagina}" });
                        }
                    }
                    else
                    {
                        log.LogWhy = "Permission is Empty";
                        return RedirectToAction("PreserveActionError", "Home", new { Error = "Permission is Empty" });
                    }
                }
                else
                {
                    log.LogWhy = "Session Expired";
                    return RedirectToAction("PreserveActionError", "Home", new { Error = "Session Expired" });
                }          
            }
            catch (Exception ex)
            {
                log.LogWhy = ex.Message;
                ErrorViewModel erro = new ErrorViewModel();
                erro.Tela = log.LogWhere;
                erro.Descricao = log.LogWhy;
                erro.Mensagem = log.LogWhat;
                erro.IdLog = auxiliar.GravaLogRetornoErro(log);
                return View();
            }
        }

        [HttpGet]
        public IActionResult GetMarcaList(string? filter, int? status, int? pagination, int page)
        {
            try
            {
                int pageSize = pagination ?? 10;

                Marca[]? result = MarcaListResult(filter, status, pagination);
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
                return Json(new { success = false, message = $"GetMarcaList Error: {ex.Message}" });
            }
        }

        public Marca[] MarcaListResult(string? filter, int? status, int? pagination)
        {
            Marca[]? ObraResult = _context.Marca.Where(i =>
                                        (status == null || i.Ativo == status)
                                        && (filter == null || i.Nome.Contains(filter))
                                         ).ToArray() ?? new Marca[0];

            return ObraResult;
        }

        [HttpGet]
        public IActionResult checkNomeDuplicate(string? nome,int? idMarca)
        {
            try
            {
                bool isDuplicate = _context.Marca.Any(m => m.Nome == nome && (idMarca == null || m.Id != idMarca));

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
        public IActionResult CreateMarca([Bind("Nome")] MarcaViewModel marca)
        {
            Log log = new Log();
            log.LogWhat = pagina + "/CreateMarca";
            log.LogWhere = pagina;
            Auxiliar auxiliar = new Auxiliar(_context, _contextBS, httpContextAccessor, _configuration);

            try
            {
                LoggedUserData? loggedUser = HttpContext.Session.GetObject<LoggedUserData>(SessionKeyLoggedUserInformation) ?? new LoggedUserData();
                if (loggedUser != null)
                {
                    PermissionAccessModel? checkPermission = loggedUser?.ListOfPermissionAccess?.FirstOrDefault(i => i.Pagina == pagina);
                    if (checkPermission != null)
                    {
                        if (checkPermission.Inserir == 1)
                        {

                            using (var transaction = _context.Database.BeginTransaction())
                            {
                                try
                                {
                                    Marca entity = _mapper.Map<Marca>(marca);
                                    entity.Ativo = 1;
                                    _context.Marca.Add(entity);
                                    _context.SaveChanges();

                                    log.LogWho = loggedUser.Id.ToString();
                                    log.LogHowMuch = entity.Id.ToString();
                                    LogTransaction(log);

                                    transaction.Commit();

                                    //ViewBag.SuccessAlertNew = "Cadastro Novo Obra Successo!";

                                    TempData["SuccessAlertMarca"] = "Cadastro Novo Marca Successo!";
                                    return RedirectToAction(nameof(Index));

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
                            return RedirectToAction("PreserveActionError", "Home", new { Error = $"No Permission for Page:{pagina}" });
                        }
                    }
                    else
                    {
                        log.LogWhy = "Permission is Empty";
                        return RedirectToAction("PreserveActionError", "Home", new { Error = "Permission is Empty" });
                    }
                }
                else
                {
                    log.LogWhy = "Session Expired";
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
        public IActionResult EditMarca([Bind("Id,Nome")] MarcaViewModel marca)
        {
            Log log = new Log();
            log.LogWhat = pagina + "/EditMarca";
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
                        if (checkPermission.Editar == 1)
                        {
                            using (var transaction = _context.Database.BeginTransaction())
                            {
                                try
                                {
                                    Marca? entityToEdit = _context.Marca.FirstOrDefault(i => i.Id == marca.Id);
                                    if (entityToEdit != null)
                                    {
                                        entityToEdit.Nome = marca.Nome;
                                        _context.Update(entityToEdit);
                                        _context.SaveChanges();

                                        log.LogWho = loggedUser.Id.ToString();
                                        log.LogHowMuch = entityToEdit.Id.ToString();
                                        LogTransaction(log);

                                        transaction.Commit();

                                        TempData["SuccessAlertMarca"] = $"Editar Obra {entityToEdit.Id} Successo!";
                                        return RedirectToAction(nameof(Index));
                                    }
                                    else
                                    {
                                        throw new Exception($"Entity with ID:{marca.Id} not found");
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






        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Edit(int id, [Bind("Id, Nome, DataRegistro, Ativo")] MarcaViewModel viewModel)
        //{
        //    Log log = new Log();
        //    log.LogWhat = pagina + "/Index";
        //    log.LogWhere = pagina;
        //    Auxiliar auxiliar = new Auxiliar(_context, _contextBS, httpContextAccessor, _configuration);

        //    try
        //    {


        //        LoggedUserData? loggedUser = HttpContext.Session.GetObject<LoggedUserData>(SessionKeyLoggedUserInformation) ?? new LoggedUserData();
        //        if (loggedUser != null)
        //        {
        //            PermissionAccessModel? checkPermission = loggedUser?.ListOfPermissionAccess?.FirstOrDefault(i => i.Pagina == pagina);
        //            if (checkPermission != null)
        //            {
        //                if (checkPermission.Editar == 1)
        //                {


        //                    if (ModelState.IsValid)
        //                    {
        //                        if (id != viewModel.Id)
        //                        {
        //                            log.LogWhy = "ID da marca não localizada no banco de dados!";
        //                            ErrorViewModel erro = new ErrorViewModel();
        //                            erro.Tela = log.LogWhere;
        //                            erro.Descricao = log.LogWhy;
        //                            erro.Mensagem = log.LogWhat;
        //                            erro.IdLog = auxiliar.GravaLogRetornoErro(log);
        //                            return NotFound();
        //                        }

        //                        var marcas = _context.Marca.AsNoTracking().Where(x => x.Id == id).FirstOrDefault();

        //                        if (marcas == null)
        //                        {
        //                            return NotFound();
        //                        }

        //                        if (id != marcas.Id)
        //                        {
        //                            return NotFound(marcas.Id);
        //                        }

        //                        viewModel.Id = marcas.Id;
        //                        viewModel.DataRegistro = marcas.DataRegistro;
        //                        viewModel.Ativo = marcas.Ativo;

        //                        bool isDuplicate = _context.Marca.Any(m => m.Nome == viewModel.Nome);

        //                        if (isDuplicate)
        //                        {
        //                            log.LogWhy = "Duplicado cadastrado!";
        //                            ErrorViewModel erro = new ErrorViewModel();
        //                            erro.Tela = log.LogWhere;
        //                            erro.Descricao = log.LogWhy;
        //                            erro.Mensagem = log.LogWhat;
        //                            erro.IdLog = auxiliar.GravaLogRetornoErro(log);
        //                            ViewBag.Error = "Duplicado cadastrado!";
        //                        }

        //                        var mapper = mapeamentoClasses.CreateMapper();
        //                        marcas = mapper.Map<Marca>(viewModel);

        //                        _context.Entry(marcas).State = EntityState.Modified;
        //                        _context.SaveChanges();

        //                        log.LogWhy = "Marca editada com sucesso!";
        //                        auxiliar.GravaLogSucesso(log);

        //                        TempData["ShowSuccessAlert"] = true;
        //                        httpContextAccessor.HttpContext?.Session.Remove(SessionKeyMarcaList);
        //                        //_ListMarca.Clear();
        //                        //GlobalValues.ClearList(GlobalValues.ListMarcaViewModel);


        //                        return RedirectToAction(nameof(Index));

        //                    }
        //                    else
        //                    {
        //                        log.LogWhy = "Erro na validação do modelo em editar empresa!";
        //                        auxiliar.GravaLogAlerta(log);
        //                        return View(viewModel);
        //                    }



        //                }
        //                else
        //                {
        //                    return RedirectToAction("PreserveActionError", "Home", new { Error = $"No Permission for Page:{pagina}" });
        //                }
        //            }
        //            else
        //            {
        //                log.LogWhy = "Permission is Empty";
        //                return RedirectToAction("PreserveActionError", "Home", new { Error = "Permission is Empty" });
        //            }
        //        }
        //        else
        //        {
        //            log.LogWhy = "Session Expired";
        //            return RedirectToAction("PreserveActionError", "Home", new { Error = "Session Expired" });
        //        }

        //        //string EditReciever = viewModel.Nome;
        //        //int EditId = id;
        //        //int ActiveReceiver = viewModel.Ativo;




        //            //return View(viewModel);
        //    }
        //    catch (Exception ex)
        //    {
        //        log.LogWhy = ex.Message;
        //        ErrorViewModel erro = new ErrorViewModel();
        //        erro.Tela = log.LogWhere;
        //        erro.Descricao = log.LogWhy;
        //        erro.Mensagem = log.LogWhat;
        //        erro.IdLog = auxiliar.GravaLogRetornoErro(log);
        //        ViewBag.Error = ex.Message;
        //        return View("Index");
        //    }

        //}


    }
}
