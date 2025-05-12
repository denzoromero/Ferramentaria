using FerramentariaTest.Models;
using FerramentariaTest.DAL;
using FerramentariaTest.Entities;
using UsuarioBS = FerramentariaTest.EntitiesBS.Usuario;
using FerramentariaTest.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System;

namespace FerramentariaTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ContextoBanco _context;
        private readonly ContextoBancoBS _contextBS;
        protected IHttpContextAccessor httpContextAccessor;
        private readonly IConfiguration _configuration;
        private static string pagina = "Home";

        private Auxiliar auxiliar;

        private const string SessionKeyLoggedUserInformation = "LoggedUserInformation";

        public HomeController(ContextoBanco context, ContextoBancoBS contextBS, IHttpContextAccessor httpCA, IConfiguration configuration)
        {
            _context = context;
            _contextBS = contextBS;
            httpContextAccessor = httpCA;
            _configuration = configuration;
            auxiliar = new Auxiliar(_context, _contextBS, httpContextAccessor, _configuration);
        }


        public IActionResult Index() 
        {
            Log log = new Log();
            log.LogWhat = pagina + "/Index";
            log.LogWhere = pagina;
            Auxiliar auxiliar = new Auxiliar(_context, _contextBS, httpContextAccessor, _configuration);
            try
            {
                string Page = "?codigo=FERRAMENTARIA";
                LoggedUserData? loggedUser = HttpContext.Session.GetObject<LoggedUserData>(SessionKeyLoggedUserInformation) ?? new LoggedUserData();

                if (loggedUser != null) 
                {
                    PermissionAccessModel? checkPermission = loggedUser?.ListOfPermissionAccess?.FirstOrDefault(i => i.Pagina == Page);
                    if (checkPermission != null)
                    {
                        if (checkPermission.Visualizar == 1)
                        {
                            var connectionString = _configuration.GetConnectionString("FerramentariaConnectionTest");
                            if (connectionString != null)
                            {
                                httpContextAccessor.HttpContext.Session.Remove(Sessao.Environment);
                                httpContextAccessor.HttpContext.Session.SetString(Sessao.Environment, "Test");
                            }
                            else
                            {
                                httpContextAccessor.HttpContext.Session.Remove(Sessao.NOME);
                                httpContextAccessor.HttpContext.Session.SetString(Sessao.NOME, "Producao");
                            }

                            // Pass it via ViewBag or ViewData
                            ViewBag.ConnectionString = connectionString;

                            return View();
                        }
                        else
                        {
                            log.LogWhy = $"No Permission for Page:{Page}";
                            ErrorViewModel erro = new ErrorViewModel();
                            erro.Tela = log.LogWhere;
                            erro.Descricao = log.LogWhy;
                            erro.Mensagem = log.LogWhat;
                            return RedirectToAction("Error", "Home", erro);
                        }
                    }
                    else
                    {
                        log.LogWhy = "Permission is Empty";
                        ErrorViewModel erro = new ErrorViewModel();
                        erro.Tela = log.LogWhere;
                        erro.Descricao = log.LogWhy;
                        erro.Mensagem = log.LogWhat;
                        return RedirectToAction("Error", "Home", erro);
                    }
                }
                else
                {
                    log.LogWhy = "Session Expired";
                    ErrorViewModel erro = new ErrorViewModel();
                    erro.Tela = log.LogWhere;
                    erro.Descricao = log.LogWhy;
                    erro.Mensagem = log.LogWhat;
                    return RedirectToAction("Error", "Home", erro);
                }




                //VW_Usuario_NewViewModel usuarioViewModel = auxiliar.retornaUsuario();
                ////usuario.Pagina = "Home/Index";
                //usuarioViewModel.Pagina = log.LogWhat;
                //usuarioViewModel.Pagina1 = log.LogWhat;
                //usuarioViewModel.Acesso = log.LogWhat;
                //usuarioViewModel = auxiliar.VerificaPermissao(usuarioViewModel);

           

            }
            catch (Exception ex)
            {
                log.LogWhy = ex.Message;
                ErrorViewModel erro = new ErrorViewModel();
                erro.Tela = log.LogWhere;
                erro.Descricao = log.LogWhy;
                erro.Mensagem = log.LogWhat;
                //erro.IdLog = auxiliar.GravaLogRetornoErro(log);
                return RedirectToAction("Error", "Home", erro);
            }
        }

        public IActionResult Login()
        {
            return View();
        }


		//public IActionResult PreserveActionError(VW_Usuario_NewViewModel usuario)
		//{
  //          return View(nameof(Index), usuario);				
		//}

        public IActionResult PreserveActionError(string? Error)
        {
            ViewBag.ErrorHandlerReturn = Error;
            return View(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        //[AllowAnonymous]
        public IActionResult Login([Bind("SAMAccountName,Senha")] VW_Usuario_NewViewModel usuario)
        {
            Log log = new Log();
            log.LogWhat = pagina + "/Login";
            log.LogWhere = pagina;

            try
            {
                VW_Usuario_NewViewModel uvm = new VW_Usuario_NewViewModel();

                if (ModelState.IsValid)
                {

                    usuario.Retorno = auxiliar.ValidaUsuarioAD(usuario.SAMAccountName, usuario.Senha);

                    if (usuario == null)
                    {
                        uvm.SAMAccountName = usuario.SAMAccountName;
                        uvm.Senha = usuario.Senha;
                        uvm.Retorno = "Usuário sem permissão!";
                        log.LogWhy = uvm.Retorno;
                        auxiliar.GravaLogAlerta(log);
                        return View(uvm);
                    }
                    else if (usuario.Retorno.Equals("Erro em conectar com o AD!"))
                    {
                        uvm.SAMAccountName = usuario.SAMAccountName;
                        uvm.Senha = usuario.Senha;
                        uvm.Retorno = "Usuário ou Senha Inválidos!";
                        log.LogWhy = uvm.Retorno;
                        auxiliar.GravaLogAlerta(log);
                        return View(uvm);
                    }
                    else
                    {
                        UsuarioBS? checkUser = _contextBS.Usuario.FirstOrDefault(i => i.Email.Contains(usuario.SAMAccountName) && i.Ativo == 1);
                        if (checkUser != null)
                        {
                            string Page = "thDefault.aspx";
                            Acesso? checkAccess = _contextBS.Acesso.FirstOrDefault(i => i.IdModulo == 6 && i.Pagina == Page && i.Ativo == 1);
                            if (checkAccess != null)
                            {
                                Permissao? checkPermission = _contextBS.Permissao.FirstOrDefault(i => i.IdUsuario == checkUser.Id && i.IdAcesso == checkAccess.Id);
                                if (checkPermission != null)
                                {
                                    List<PermissionAccessModel?>? ListOfPermissionAccess = (from permission in _contextBS.Permissao
                                                                                            join access in _contextBS.Acesso on permission.IdAcesso equals access.Id
                                                                                            where access.IdModulo == 6
                                                                                            && permission.IdUsuario == checkUser.Id
                                                                                            orderby permission.DataRegistro descending
                                                                                            select new PermissionAccessModel
                                                                                            {
                                                                                                IdUsuario = permission.IdUsuario,
                                                                                                IdAcesso = permission.IdAcesso,
                                                                                                Visualizar = permission.Visualizar,
                                                                                                Inserir = permission.Inserir,
                                                                                                Editar = permission.Editar,
                                                                                                Excluir = permission.Excluir,
                                                                                                Caminho = access.Caminho,
                                                                                                Pagina = access.Pagina,
                                                                                                Pagina1 = access.Pagina1,
                                                                                            }).ToList();

                                    Funcionario? employeeInformation = _contextBS.Funcionario.FirstOrDefault(i => i.CodColigada == checkUser.CodColigada && i.Chapa == checkUser.Chapa);

                                    LoggedUserData loggedUser = new LoggedUserData()
                                    {
                                        Id = checkUser.Id,
                                        IdTerceiro = checkUser.IdTerceiro,
                                        Chapa = checkUser.Chapa,
                                        CodColigada = checkUser.CodColigada,
                                        Nome = employeeInformation?.Nome,
                                        Email = checkUser.Email.Replace("@seatrium.com",""),
                                        ListOfPermissionAccess = ListOfPermissionAccess
                                    };

                                    HttpContext.Session.Clear();
                                    httpContextAccessor?.HttpContext?.Session.Clear();
                                    TempData.Clear();

                                    HttpContext.Session.SetObject(SessionKeyLoggedUserInformation, loggedUser);
                                    return RedirectToAction(nameof(Index));

                                }
                                else
                                {
                                    uvm.SAMAccountName = usuario.SAMAccountName;
                                    uvm.Senha = usuario.Senha;
                                    uvm.Retorno = $"User doesnt have access to  {Page}!";
                                    log.LogWhy = uvm.Retorno;
                                    auxiliar.GravaLogAlerta(log);
                                    return View(uvm);
                                }
                            }
                            else
                            {
                                uvm.SAMAccountName = usuario.SAMAccountName;
                                uvm.Senha = usuario.Senha;
                                uvm.Retorno = $"Page {Page} not found on Accesso!";
                                log.LogWhy = uvm.Retorno;
                                auxiliar.GravaLogAlerta(log);
                                return View(uvm);
                            }
                        }
                        else
                        {
                            uvm.SAMAccountName = usuario.SAMAccountName;
                            uvm.Senha = usuario.Senha;
                            uvm.Retorno = "User not found on the Usuario BS!";
                            log.LogWhy = uvm.Retorno;
                            auxiliar.GravaLogAlerta(log);
                            return View(uvm);
                        }


                        //Permissao? permissao = new Permissao();
                        //permissao = _contextBS.Permissao.OrderByDescending(p => p.DataRegistro).FirstOrDefault(p => p.SAMAccountName.Equals(usuario.SAMAccountName));

                        //if (permissao == null)
                        //{
                        //    //UsuarioViewModel uvm = new UsuarioViewModel();
                        //    uvm.SAMAccountName = usuario.SAMAccountName;
                        //    uvm.Senha = usuario.Senha;
                        //    uvm.Retorno = "Usuário sem permissão!";
                        //    log.LogWhy = uvm.Retorno;
                        //    //auxiliar.GravaLogAlerta(log);
                        //    return View(uvm);
                        //}
                        //else
                        //{

                        //    var usu = _contextBS.VW_Usuario_New.FirstOrDefault(u => u.Id == permissao.IdUsuario && u.Ativo == 1);

                        //    if (usu == null)
                        //    {
                        //        //UsuarioViewModel uvm = new UsuarioViewModel();
                        //        uvm.SAMAccountName = usuario.SAMAccountName;
                        //        uvm.Senha = usuario.Senha;
                        //        uvm.Retorno = "Usuário destivado no SIB!";
                        //        log.LogWhy = uvm.Retorno;
                        //        auxiliar.GravaLogAlerta(log);
                        //        return View(uvm);
                        //    }
                        //    else
                        //    {
                        //        var acessoList = _contextBS.Acesso.Where(a => a.IdModulo == 6).GroupJoin(_contextBS.Permissao.Where(p => p.IdUsuario == permissao.IdUsuario), a => a.Id, p => p.IdAcesso, (a, p) => new AcessoViewModel { Pagina = a.Pagina }).AsEnumerable();


                        //        if (acessoList == null)
                        //        {
                        //            //UsuarioViewModel uvm = new UsuarioViewModel();
                        //            uvm.SAMAccountName = usuario.SAMAccountName;
                        //            uvm.Senha = usuario.Senha;
                        //            uvm.Retorno = "Usuário sem Acesso!";
                        //            log.LogWhy = uvm.Retorno;
                        //            //auxiliar.GravaLogAlerta(log);
                        //            return View(uvm);
                        //        }
                        //        else
                        //        {
                        //            uvm.Id = usu.Id;
                        //            uvm.AcessoLista = acessoList;
                        //            uvm.Chapa = usu.Chapa;
                        //            uvm.Email = usu.Email;
                        //            uvm.Nome = usu.Nome;
                        //            uvm.Chapa = usu.Chapa;
                        //            uvm.SAMAccountName = usuario.SAMAccountName;

                        //            if (uvm.Id != null)
                        //            {
                        //                httpContextAccessor.HttpContext.Session.SetInt32(Sessao.ID, (int)uvm.Id);
                        //                if (uvm.Chapa != null)
                        //                {
                        //                    httpContextAccessor.HttpContext.Session.SetString(Sessao.CHAPA, uvm.Chapa);
                        //                }
                        //                if (uvm.Nome != null)
                        //                {
                        //                    httpContextAccessor.HttpContext.Session.SetString(Sessao.NOME, uvm.Nome);
                        //                }
                        //                if (uvm.SAMAccountName != null)
                        //                {
                        //                    httpContextAccessor.HttpContext.Session.SetString(Sessao.SAMACCOUNT, uvm.SAMAccountName);
                        //                }
                        //                httpContextAccessor.HttpContext.Session.SetString(Sessao.LOGADO, "S");
                        //            }

                        //            bool teste = verificaConnectionsStrins();
                        //            log.LogWhy = "Login Executado com Sucesso";
                        //            auxiliar.GravaLogSucesso(log);
                        //            //return RedirectToAction(nameof(Index), uvm);
                        //            return RedirectToAction(nameof(Index));

                        //        }


                        //    }



                        //}


                    }


                }
                else
                {
                    return View(usuario);
                }

            }
            catch (Exception ex)
            {
                //return View(usuario);

                log.LogWhy = ex.Message;
                ErrorViewModel erro = new ErrorViewModel();
                erro.Tela = log.LogWhere;
                erro.Descricao = log.LogWhy;
                erro.Mensagem = log.LogWhat;
                erro.IdLog = auxiliar.GravaLogRetornoErro(log);
				ViewBag.Error = ex.Message;
				return View();
            }

        }

		public IActionResult Logout()
        {
			Log log = new Log();
			log.LogWhat = pagina + "/Login";
			log.LogWhere = pagina;
			Auxiliar auxiliar = new Auxiliar(_context, _contextBS, httpContextAccessor, _configuration);

            try
            {
				#region Authenticate User
				VW_Usuario_NewViewModel usuariofer = auxiliar.retornaUsuario();
				//usuario.Pagina = "Home/Index";
				usuariofer.Pagina = pagina;
				usuariofer.Pagina1 = log.LogWhat;
				usuariofer.Acesso = log.LogWhat;
				usuariofer = auxiliar.VerificaPermissao(usuariofer);

				if (usuariofer.Permissao == null)
				{
					usuariofer.Retorno = "Usuário sem permissão na página!";
					log.LogWhy = usuariofer.Retorno;
					auxiliar.GravaLogAlerta(log);
					return RedirectToAction("Login", "Home", usuariofer);
				}
				else
				{
					if (usuariofer.Permissao.Visualizar != 1)
					{
						usuariofer.Retorno = "Usuário sem permissão de Editar a página de ferramentaria!";
						log.LogWhy = usuariofer.Retorno;
						auxiliar.GravaLogAlerta(log);
						return RedirectToAction("Login", "Home", usuariofer);
					}
				}
				#endregion

				//httpContextAccessor.HttpContext.Session.Remove(Sessao.Funcionario);
				//httpContextAccessor.HttpContext.Session.Remove(Sessao.LOGADO);
				//httpContextAccessor.HttpContext.Session.Remove(Sessao.NOME);
				//httpContextAccessor.HttpContext.Session.Remove(Sessao.CHAPA);
				//httpContextAccessor.HttpContext.Session.Remove(Sessao.ID);
				//httpContextAccessor.HttpContext.Session.Remove(Sessao.SAMACCOUNT);
                httpContextAccessor?.HttpContext?.Session.Clear();
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                TempData.Clear();

                return RedirectToAction(nameof(Login));
            }
			catch (Exception ex)
			{
				//return View(usuario);

				log.LogWhy = ex.Message;
				ErrorViewModel erro = new ErrorViewModel();
				erro.Tela = log.LogWhere;
				erro.Descricao = log.LogWhy;
				erro.Mensagem = log.LogWhat;
				erro.IdLog = auxiliar.GravaLogRetornoErro(log);
				return View();
			}		
		}

		//private readonly ILogger<HomeController> _logger;

		//public HomeController(ILogger<HomeController> logger)
		//{
		//    _logger = logger;
		//}


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}