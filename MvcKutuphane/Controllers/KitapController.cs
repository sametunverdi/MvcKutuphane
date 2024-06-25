using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcKutuphane.Models.Entity;
using System.Data.Entity;

namespace MvcKutuphane.Controllers
{
    public class KitapController : Controller
    {
        // GET: Kitap
        DBKUTUPHANEEntities1 db = new DBKUTUPHANEEntities1();
        public ActionResult Index(string p)
        {
            var kitaplar = from k in db.TBLKITAP select k;
            if (!string.IsNullOrEmpty(p))
            {
                kitaplar = kitaplar.Where(m => m.AD.Contains(p));
            }
            //var kitaplar = db.TBLKITAP.ToList();
            return View(kitaplar.ToList());
        }


        [HttpPost]
        public ActionResult KitapByBarcode(string barcode)
        {
            try
            {
                var kitap = db.TBLKITAP
                    .Include(k => k.TBLYAZAR)
                    .Include(k => k.TBLKATEGORI)
                    .FirstOrDefault(k => k.BARKOD == barcode);

                if (kitap != null)
                {
                    return Json(new
                    {
                        ID = kitap.ID,
                        AD = kitap.AD,
                        YAZAR = kitap.TBLYAZAR.AD + " " + kitap.TBLYAZAR.SOYAD,
                        KATEGORI = kitap.TBLKATEGORI.AD,
                        BASIMYIL = kitap.BASIMYIL,
                        YAYINEVI = kitap.YAYINEVI,
                        SAYFA = kitap.SAYFA,
                        DURUM = kitap.DURUM
                    });
                }
                else
                {
                    return Json(new { error = "Kitap bulunamadı." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = "Beklenmeyen bir hata oluştu: " + ex.Message });
            }
        }


        [HttpGet]
        public ActionResult KitapEkle()
        {
            List<SelectListItem> deger1 = (from i in db.TBLKATEGORI.ToList()
                                           select new SelectListItem
                                           {
                                               Text = i.AD,
                                               Value = i.ID.ToString()
                                           }).ToList();
            ViewBag.dgr1 = deger1;

            List<SelectListItem> deger2 = (from i in db.TBLYAZAR.ToList()
                                           select new SelectListItem
                                           {
                                               Text = i.AD + ' ' + i.SOYAD,
                                               Value = i.ID.ToString()
                                           }).ToList();
            ViewBag.dgr2 = deger2;
            return View();
        }
        [HttpPost]
        public ActionResult KitapEkle(TBLKITAP p, string barkod)
        {
            // Barkod alanını gelen değerle güncelle
            p.BARKOD = barkod;

            // Seçilen kategori ve yazarın veritabanından ilgili nesnelerini al
            var ktg = db.TBLKATEGORI.Where(k => k.ID == p.TBLKATEGORI.ID).FirstOrDefault();
            var yzr = db.TBLYAZAR.Where(y => y.ID == p.TBLYAZAR.ID).FirstOrDefault();

            // Alınan kategori ve yazar nesnelerini kitap nesnesine ata
            p.TBLKATEGORI = ktg;
            p.TBLYAZAR = yzr;

            // Kitabı veritabanına ekle
            db.TBLKITAP.Add(p);
            db.SaveChanges();

            // Index sayfasına yönlendir
            return RedirectToAction("Index");
        }

        public ActionResult KitapSil(int id)
        {
            var kitap = db.TBLKITAP.Find(id);
            db.TBLKITAP.Remove(kitap);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult KitapGetir(int id)
        {
            var ktp = db.TBLKITAP.Find(id);
            List<SelectListItem> deger1 = (from i in db.TBLKATEGORI.ToList()
                                           select new SelectListItem
                                           {
                                               Text = i.AD,
                                               Value = i.ID.ToString()
                                           }).ToList();
            ViewBag.dgr1 = deger1;

            List<SelectListItem> deger2 = (from i in db.TBLYAZAR.ToList()
                                           select new SelectListItem
                                           {
                                               Text = i.AD + ' ' + i.SOYAD,
                                               Value = i.ID.ToString()
                                           }).ToList();
            ViewBag.dgr2 = deger2;
            return View("KitapGetir", ktp);
        }
        public ActionResult KitapGuncelle(TBLKITAP p)
        {
            var kitap = db.TBLKITAP.Find(p.ID);
            kitap.AD = p.AD;
            kitap.BASIMYIL = p.BASIMYIL;
            kitap.SAYFA = p.SAYFA;
            kitap.YAYINEVI = p.YAYINEVI;
            var ktg = db.TBLKATEGORI.Where(k => k.ID == p.TBLKATEGORI.ID).FirstOrDefault();
            var yzr = db.TBLYAZAR.Where(y => y.ID == p.TBLYAZAR.ID).FirstOrDefault();
            kitap.KATEGORI = ktg.ID;
            kitap.YAZAR = yzr.ID;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}