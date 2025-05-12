using FerramentariaTest.DAL;
using FerramentariaTest.Entities;
using FerramentariaTest.EntitiesBS;
using FerramentariaTest.Helpers;
using FerramentariaTest.Models;
using Microsoft.AspNetCore.Mvc;

namespace FerramentariaTest.Controllers
{
    public class PrepareReservation : Controller
    {
        private readonly ContextoBanco _context;
        private readonly ContextoBancoBS _contextBS;
        private readonly ContextoBancoRM _contextRM;
        private readonly ContextoBancoSeek _contextSeek;
        protected IHttpContextAccessor httpContextAccessor;
        private readonly IConfiguration _configuration;

        private const string SessionKeyLoggedUserInformation = "LoggedUserInformation";

        private Auxiliar auxiliar;
        private Searches searches;

        public PrepareReservation(ContextoBanco context, ContextoBancoBS contextBS, ContextoBancoRM contextRM, ContextoBancoSeek contextSeek, IHttpContextAccessor httpCA, IConfiguration configuration)
        {
            _context = context;
            _contextBS = contextBS;
            _contextRM = contextRM;
            _contextSeek = contextSeek;
            httpContextAccessor = httpCA;
            _configuration = configuration;
            //mapeamentoClasses = new MapperConfiguration(cfg =>
            //{
            //    cfg.CreateMap<Funcionario, FuncionarioViewModel>();
            //    cfg.CreateMap<FuncionarioViewModel, Funcionario>();
            //});
            auxiliar = new Auxiliar(_context, _contextBS, httpContextAccessor, _configuration);
            searches = new Searches(_context, _contextBS, _contextRM, _contextSeek, httpContextAccessor, _configuration);
        }

        public IActionResult Index()
        {

            try
            {

                LoggedUserData? loggedUser = HttpContext.Session.GetObject<LoggedUserData>(SessionKeyLoggedUserInformation) ?? new LoggedUserData();

                int? FerramentariaValue = httpContextAccessor.HttpContext?.Session.GetInt32(Sessao.Ferramentaria);
                if (FerramentariaValue == null)
                {
                    var ferramentariaItems = (from ferramentaria in _context.Ferramentaria
                                              where ferramentaria.Ativo == 1 &&
                                                    !_context.VW_Ferramentaria_Ass_Solda.Select(s => s.Id).Contains(ferramentaria.Id) &&
                                                    _context.FerramentariaVsLiberador.Any(l => l.IdLogin == loggedUser.Id && l.IdFerramentaria == ferramentaria.Id)
                                              orderby ferramentaria.Nome
                                              select new
                                              {
                                                  Id = ferramentaria.Id,
                                                  Nome = ferramentaria.Nome
                                              }).ToList();

                    if (ferramentariaItems != null)
                    {
                        ViewBag.FerramentariaItems = ferramentariaItems;
                        ViewBag.ViewPage = "PreparationIndex";
                    }

                    return PartialView("_FerramentariaPartialView");

                }
                else
                {
                    httpContextAccessor.HttpContext.Session.SetInt32(Sessao.IdFerramentaria, (int)FerramentariaValue);
                }

                List<ReservationsModel>? groupReservation = getGroupReservation(FerramentariaValue);

                ViewBag.ReservationList = groupReservation;

                //ViewBag.TestJs = "test";

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }       
        }

        public IActionResult ProcessItem(int id)
        {
            try
            {
                if (TempData.ContainsKey("ShowSuccessAlertproceedPrepare"))
                {
                    ViewBag.SuccessAlertNew = TempData["ShowSuccessAlertproceedPrepare"]?.ToString();
                    TempData.Remove("ShowSuccessAlertproceedPrepare"); // Remove it from TempData to avoid displaying it again
                }

                ItemReservationDetailModel? completeItemDetail = getCompleteItemDetail(id);

                return View(completeItemDetail);

            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
          
        }

        public ActionResult proceedReservation(reserveSubmission? item)
        {
            try
            {
                LoggedUserData? loggedUser = HttpContext.Session.GetObject<LoggedUserData>(SessionKeyLoggedUserInformation) ?? new LoggedUserData();
                if (item != null)
                {
                    ProductReservation? checkReservation = _context.ProductReservation.FirstOrDefault(i => i.IdReservation == item.IdReservation);
                    if (checkReservation == null)
                    {
                        Reservations? reservation = _context.Reservations.FirstOrDefault(i => i.Id == item.IdReservation);
                        if (reservation != null)
                        {
                            using (var transaction = _context.Database.BeginTransaction())
                            {
                                try
                                {

                                    ProductReservation entity = new ProductReservation()
                                    {
                                        IdReservation = item.IdReservation,
                                        IdProduto = item.IdProduto,
                                        IdControleCA = item.IdControleCA,
                                        DataPrevistaDevolucao = item.dataPrevista,
                                        Observacao = item.observacao,
                                        PreparedBy = loggedUser.Id,
                                        Status = 0,
                                        DataRegistro = DateTime.Now,
                                    };

                                    _context.ProductReservation.Add(entity);

                                    reservation.Status = 2;

                                    _context.SaveChanges();
                                    transaction.Commit();

                                    TempData["ShowSuccessAlertproceedPrepare"] = "Sucesso!";
                                    return RedirectToAction(nameof(ProcessItem));

                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback();
                                    ViewBag.Error = ex.Message;
                                    return View(nameof(ProcessItem));
                                }
                            }
                        }
                        else
                        {
                            ViewBag.Error = "Cannot find reservation detail";
                            return View(nameof(ProcessItem));
                        }
                    }
                    else
                    {
                        ViewBag.Error = "This item is already prepared and waiting for pickup.";
                        return View(nameof(ProcessItem));
                    }
                }
                else
                {
                    ViewBag.Error = "Passed Data is null.";
                    return View(nameof(ProcessItem));
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(nameof(ProcessItem));
            }
        }

        public ActionResult proceedCancellation(cancellationSubmission? item)
        {
            try
            {
                LoggedUserData? loggedUser = HttpContext.Session.GetObject<LoggedUserData>(SessionKeyLoggedUserInformation) ?? new LoggedUserData();
                if (item != null)
                {
                    LogCancellationTransfers? checkItem = _context.LogCancellationTransfers.FirstOrDefault(i => i.IdReservation == item.IdReservation);
                    if (checkItem == null)
                    {
                        Reservations? reservation = _context.Reservations.FirstOrDefault(i => i.Id == item.IdReservation);
                        if (reservation != null)
                        {
                            using (var transaction = _context.Database.BeginTransaction())
                            {
                                try
                                {
                                    LogCancellationTransfers entity = new LogCancellationTransfers()
                                    {
                                        IdReservation = item.IdReservation,
                                        ActionType = 1,
                                        Obs = item.obsCancel,
                                        Status = 0,
                                        PostedBy = loggedUser.Id,
                                        DataRegistro = DateTime.Now,
                                    };
                                    _context.LogCancellationTransfers.Add(entity);

                                    reservation.Status = 5;

                                    _context.SaveChanges();
                                    transaction.Commit();

                                    TempData["ShowSuccessAlertproceedPrepare"] = "Sucesso para cancelar!";
                                    return RedirectToAction(nameof(ProcessItem));
                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback();
                                    ViewBag.Error = ex.Message;
                                    return View(nameof(ProcessItem));
                                }
                            }
                        }
                        else
                        {
                            ViewBag.Error = "Cannot find reservation detail";
                            return View(nameof(ProcessItem));
                        }
                    }
                    else
                    {
                        ViewBag.Error = "This reservation is already pending for Cancellation/Transfer";
                        return View(nameof(ProcessItem));
                    }                                 
                }
                else
                {
                    ViewBag.Error = "Passed Data is null.";
                    return View(nameof(ProcessItem));
                }               
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(nameof(ProcessItem));
            }
        }

        public ActionResult proceedTransfer(transferSubmission? item)
        {
            try
            {
                LoggedUserData? loggedUser = HttpContext.Session.GetObject<LoggedUserData>(SessionKeyLoggedUserInformation) ?? new LoggedUserData();
                if (item != null)
                {
                    LogCancellationTransfers? checkItem = _context.LogCancellationTransfers.FirstOrDefault(i => i.IdReservation == item.IdReservation);
                    if (checkItem == null)
                    {
                        Reservations? reservation = _context.Reservations.FirstOrDefault(i => i.Id == item.IdReservation);
                        if (reservation != null)
                        {
                            using (var transaction = _context.Database.BeginTransaction())
                            {
                                try
                                {
                                    LogCancellationTransfers entity = new LogCancellationTransfers()
                                    {
                                        IdReservation = item.IdReservation,
                                        ActionType = 2,
                                        IdFerramentariaFrom = item.IdFerramentariaOrigin,
                                        IdFerramentariaTo = item.IdFerramentariaDestination,
                                        Obs = item.obsTransfer,
                                        Status = 0,
                                        PostedBy = loggedUser.Id,
                                        DataRegistro = DateTime.Now,
                                    };
                                    _context.LogCancellationTransfers.Add(entity);

                                    reservation.Status = 5;

                                    _context.SaveChanges();
                                    transaction.Commit();

                                    TempData["ShowSuccessAlertproceedPrepare"] = "Sucesso para transferencia!";
                                    return RedirectToAction(nameof(ProcessItem));

                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback();
                                    ViewBag.Error = ex.Message;
                                    return View(nameof(ProcessItem));
                                }
                            }                           
                        }
                        else
                        {
                            ViewBag.Error = "Cannot find reservation detail";
                            return View(nameof(ProcessItem));
                        }
                    }
                    else
                    {
                        ViewBag.Error = "This reservation is already pending for Cancellation/Transfer";
                        return View(nameof(ProcessItem));
                    }             
                }
                else
                {
                    ViewBag.Error = "Passed Data is null.";
                    return View(nameof(ProcessItem));
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(nameof(ProcessItem));
            }
        }


        #region javascript

        [HttpGet]
        public IActionResult getItemDetail(int id)
        {
            try
            {
                ItemReservationDetailModel? itemdetail = getReservationDetail(id);

                if (itemdetail != null)
                {
                    int? FerramentariaValue = httpContextAccessor.HttpContext?.Session.GetInt32(Sessao.Ferramentaria);
                    if (itemdetail.IdFerramentaria == FerramentariaValue)
                    {
                        if (itemdetail.intStatus == 1)
                        {

                            if (itemdetail.intClasse == 2)
                            {
                                itemdetail.listCA = GetCAList(itemdetail.IdCatalogo);
                            }

                            itemdetail.MemberInfo = searches.searchEmployeeInformationUsingCodPessoa(itemdetail.MemberCodPessoa);
                            itemdetail.LeaderInfo = searches.searchEmployeeInformationUsingCodPessoa(itemdetail.LeaderCodPessoa);

                            itemdetail.ferramentariacount = countFerramentaria(itemdetail.IdCatalogo);

                            List<productDetail> product = checkProduct(itemdetail);

                            return Json(new { success = true, itemdetail, product });
                        }
                        else if (itemdetail.intStatus == 2)
                        {
                            return Json(new { success = false, message = "Item is already in prepared status." });
                        }
                        else if (itemdetail.intStatus == 0)
                        {
                            return Json(new { success = false, message = "Please prepare the Item first, Item is not in preparing status." });
                        }
                        else if (itemdetail.intStatus == 5)
                        {
                            return Json(new { success = false, message = "This reservation is pending for Cancellation/Transfer." });
                        }
                        else
                        {
                            return Json(new { success = false, message = "Item is not in any of the status" });
                        }
                    }
                    else
                    {
                        return Json(new { success = false, message = "Item is not on your section." });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "No data found." });
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"getItemDetail: {ex.Message}" });
            }
        }





        #endregion


        #region functions

        public List<ReservationsModel>? getGroupReservation(int? FerramentariaValue)
        {
            List<ReservationsControlModel>? reservations = (from reservationControl in _context.ReservationControl
                                                            join reserve in _context.Reservations on reservationControl.Id equals reserve.IdReservationControl
                                                            where reserve.IdFerramentaria == FerramentariaValue // Ensure this matches your SQL query
                                                            select new ReservationsControlModel
                                                            {
                                                                ControlId = reservationControl.Id,
                                                                Status = reserve.Status,
                                                            }).ToList();

            List<ReservationsModel>? groupReservation = reservations
                                                        .GroupBy(r => r.ControlId)
                                                        .Select(group => new ReservationsModel
                                                        {
                                                            ControlId = group.Key,
                                                            itemCount = group.Count(),
                                                            RegisteredCount = group.Count(r => r.Status == 0) > 0 ? group.Count(r => r.Status == 0) : 0,
                                                            PreparingCount = group.Count(r => r.Status == 1) > 0 ? group.Count(r => r.Status == 1) : 0,
                                                            PreparedCount = group.Count(r => r.Status == 2) > 0 ? group.Count(r => r.Status == 2) : 0,
                                                            //ActualStatus = group.All(r => r.Status == 0) ? "Pending" : group.All(r => r.Status == 1) ? "Prepared" : string.Empty
                                                            ActualStatus = group.All(r => r.Status == 2) ? "Concluded" : "Pending"
                                                        }).ToList();

            return groupReservation ?? new List<ReservationsModel>();
        }

        public List<ControleCA>? GetCAList(int? IdCatalogo)
        {
            List<ControleCA>? controleCAData = _context.ControleCA.Where(i => i.IdCatalogo == IdCatalogo && i.Ativo == 1 && i.Validade > DateTime.Now).OrderByDescending(i => i.Validade).ToList();

            return controleCAData ?? new List<ControleCA>();
        }

        public int countFerramentaria(int? IdCatalogo)
        {
            int FerramentariaCount = _context.Produto.Where(i => i.IdCatalogo == IdCatalogo
                                        && i.Ativo == 1
                                        && i.Quantidade > 0)
                                        .Select(i => i.IdFerramentaria)
                                        .Distinct().Count();

            return FerramentariaCount;
        }

        public List<productDetail> checkProduct(ItemReservationDetailModel itemdetail)
        {
            var subquery = from pa in _context.ProdutoAlocado
                           join catalogo in _context.Catalogo on pa.IdProduto equals catalogo.Id
                           where catalogo.PorAferido == 1 && catalogo.PorSerial == 1
                           select pa.IdProduto;

            List<productDetail>? productDetail = (from produto in _context.Produto
                                                  join catalogo in _context.Catalogo on produto.IdCatalogo equals catalogo.Id
                                                  join categoria in _context.Categoria on catalogo.IdCategoria equals categoria.Id
                                                  where produto.IdCatalogo == itemdetail.IdCatalogo
                                                  && produto.IdFerramentaria == itemdetail.IdFerramentaria
                                                  && produto.Quantidade > 0
                                                  && produto.Ativo == 1
                                                  && !subquery.Contains(produto.Id)
                                                  && categoria.Ativo == 1
                                                  && catalogo.Ativo == 1
                                                  select new productDetail
                                                  {
                                                      IdCatalogo = itemdetail.IdCatalogo,
                                                      Id = produto.Id,
                                                      Quantidade = produto.Quantidade,
                                                      AF = produto.AF,
                                                      PAT = produto.PAT,
                                                      DataVencimento = produto.DataVencimento,
                                                      allowedtoborrow = true,
                                                  }).ToList() ?? new List<productDetail>();

            if (itemdetail.Type == "PorAferido")
            {
                productDetail?.ForEach(p =>
                {
                    if (p.DataVencimento != null && p.DataVencimento < DateTime.Today)
                    {
                        p.allowedtoborrow = false;
                        p.reason = "Item fora do prazo de validade para uma data posterior a data de vencimento do item.";
                    }
                });
            }



            if (itemdetail.Classe == "EPI")
            {
                if (itemdetail.DataDeRetornoAutomatico != 0)
                {
                    productDetail.ForEach(r => r.DataPrevistaDevolucao = DateTime.Now.AddDays(itemdetail.DataDeRetornoAutomatico.Value));
                }


                List<ControleCA>? controleCAData = _context.ControleCA.Where(i => i.IdCatalogo == itemdetail.IdCatalogo
                                                                        && i.Ativo == 1
                                                                        && i.Validade > DateTime.Now).OrderByDescending(i => i.Validade).ToList();

                if (controleCAData.Count > 0)
                {
                    productDetail.ForEach(r => r.listCA = controleCAData);
                }
            }

            //productDetail?.ForEach(p =>
            //{
            //    p.allowedtoborrow = false;
            //    p.reason = "test";
            //});

            return productDetail ?? new List<productDetail>();

        }

        public ItemReservationDetailModel getReservationDetail(int id)
        {
            ItemReservationDetailModel? itemdetail = (from reserv in _context.Reservations
                                                      join catalogo in _context.Catalogo on reserv.IdCatalogo equals catalogo.Id
                                                      join categoria in _context.Categoria on catalogo.IdCategoria equals categoria.Id
                                                      join reservationControl in _context.ReservationControl on reserv.IdReservationControl equals reservationControl.Id
                                                      join member in _context.LeaderMemberRel on reserv.IdLeaderMemberRel equals member.Id
                                                      join leader in _context.LeaderData on member.IdLeader equals leader.Id
                                                      join obra in _context.Obra on reserv.IdObra equals obra.Id
                                                      where reserv.Id == id
                                                      //&& reserv.Status == 1
                                                      select new ItemReservationDetailModel
                                                      {
                                                          IdFerramentaria = reserv.IdFerramentaria,
                                                          IdCategoria = categoria.Id,
                                                          IdCatalogo = reserv.IdCatalogo,
                                                          IdReservation = reserv.Id,
                                                          intClasse = categoria.Classe,
                                                          Classe = categoria.ClassType,
                                                          Type = catalogo.PorType,
                                                          Codigo = catalogo.Codigo,
                                                          itemNome = catalogo.Nome,
                                                          QuantidadeResquested = reserv.Quantidade,
                                                          DataRegistro = reserv.DataRegistro.HasValue == true ? reserv.DataRegistro.Value.ToString("dd-MM-yyyy HH:mm") : string.Empty,
                                                          intStatus = reserv.Status,
                                                          Status = reserv.StatusString,
                                                          DataDeRetornoAutomatico = catalogo.DataDeRetornoAutomatico,
                                                          MemberCodPessoa = member.CodPessoa,
                                                          LeaderCodPessoa = leader.CodPessoa,
                                                          ExpiryDate = reservationControl.ExpirationDate,
                                                          IdObra = obra.Id,
                                                          ObraName = $"{obra.Codigo}-{obra.Nome}",
                                                      }).FirstOrDefault();


            return itemdetail ?? new ItemReservationDetailModel();

        }

        public ItemReservationDetailModel getCompleteItemDetail(int id)
        {
            ItemReservationDetailModel itemdetail = (from reserv in _context.Reservations
                                                      join catalogo in _context.Catalogo on reserv.IdCatalogo equals catalogo.Id
                                                      join categoria in _context.Categoria on catalogo.IdCategoria equals categoria.Id
                                                      join reservationControl in _context.ReservationControl on reserv.IdReservationControl equals reservationControl.Id
                                                      join member in _context.LeaderMemberRel on reserv.IdLeaderMemberRel equals member.Id
                                                      join leader in _context.LeaderData on member.IdLeader equals leader.Id
                                                      join obra in _context.Obra on reserv.IdObra equals obra.Id
                                                      where reserv.Id == id
                                                      //&& reserv.Status == 1
                                                      select new ItemReservationDetailModel
                                                      {
                                                          IdFerramentaria = reserv.IdFerramentaria,
                                                          IdCategoria = categoria.Id,
                                                          IdCatalogo = reserv.IdCatalogo,
                                                          IdReservation = reserv.Id,
                                                          intClasse = categoria.Classe,
                                                          Classe = categoria.ClassType,
                                                          Type = catalogo.PorType,
                                                          Codigo = catalogo.Codigo,
                                                          itemNome = catalogo.Nome,
                                                          QuantidadeResquested = reserv.Quantidade,
                                                          DataRegistro = reserv.DataRegistro.HasValue == true ? reserv.DataRegistro.Value.ToString("dd-MM-yyyy HH:mm") : string.Empty,
                                                          intStatus = reserv.Status,
                                                          Status = reserv.StatusString,
                                                          DataDeRetornoAutomatico = catalogo.DataDeRetornoAutomatico,
                                                          MemberCodPessoa = member.CodPessoa,
                                                          LeaderCodPessoa = leader.CodPessoa,
                                                          ExpiryDate = reservationControl.ExpirationDate,
                                                          IdObra = obra.Id,
                                                          ObraName = $"{obra.Codigo}-{obra.Nome}",
                                                          ExpiryDateString = reservationControl.ExpirationDate.HasValue == true ? reservationControl.ExpirationDate.Value.ToString("dd-MM-yyyy HH:mm") : string.Empty
                                                      }).FirstOrDefault() ?? new ItemReservationDetailModel();

            if (itemdetail.intClasse == 2)
            {
                itemdetail.listCA = GetCAList(itemdetail.IdCatalogo);
            }

            itemdetail.MemberInfo = searches.searchEmployeeInformationUsingCodPessoa(itemdetail.MemberCodPessoa);
            itemdetail.LeaderInfo = searches.searchEmployeeInformationUsingCodPessoa(itemdetail.LeaderCodPessoa);



            //itemdetail.ferramentariacount = countFerramentaria(itemdetail.IdCatalogo);

            itemdetail.listFerramentaria = getFerramentariaList(itemdetail.IdCatalogo,itemdetail.IdFerramentaria);

            itemdetail.listProduto = checkProduct(itemdetail);


            return itemdetail ?? new ItemReservationDetailModel();
        }

        public List<Ferramentaria>? getFerramentariaList(int? IdCatalogo,int? IdFerramentaria)
        {
            List<Ferramentaria>? ferramentariaList = (from produto in _context.Produto
                                                      join ferramentaria in _context.Ferramentaria on produto.IdFerramentaria equals ferramentaria.Id
                                                      where produto.Ativo == 1 
                                                      && produto.Quantidade > 0
                                                      && ferramentaria.Ativo == 1
                                                      && produto.IdFerramentaria != 17
                                                      && produto.IdFerramentaria != IdFerramentaria
                                                      && produto.IdCatalogo == IdCatalogo
                                                      select new Ferramentaria
                                                      {
                                                          Id = ferramentaria.Id,
                                                          Nome = ferramentaria.Nome
                                                      }).Distinct().ToList() ?? new List<Ferramentaria>();

            return ferramentariaList;
        }

        #endregion





        #region Ferramentaria Actions

        public ActionResult SetFerramentariaValue(int? Ferramentaria, string? SelectedNome)
        {
            var currentController = RouteData.Values["controller"]?.ToString();

            if (Ferramentaria != null)
            {
                httpContextAccessor.HttpContext.Session.SetInt32(Sessao.Ferramentaria, (int)Ferramentaria);
                httpContextAccessor.HttpContext.Session.SetString(Sessao.FerramentariaNome, SelectedNome);
            }

            return RedirectToAction(nameof(Index));
        }

        public ActionResult RefreshFerramentaria()
        {
            httpContextAccessor.HttpContext.Session.Remove(Sessao.Ferramentaria);
            httpContextAccessor.HttpContext.Session.Remove(Sessao.FerramentariaNome);
            return RedirectToAction(nameof(Index));
        }

        #endregion

    }
}
