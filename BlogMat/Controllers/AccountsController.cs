using BlogMat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BlogMat.Controllers
{
    public class AccountsController : Controller
    {
        private BlogDbEntities1 model = new BlogDbEntities1();
        public ActionResult Login(string name, string hash)
        {
            if (string.IsNullOrWhiteSpace(hash))
            {
                Random random = new Random();
                byte[] randomData = new byte[sizeof(long)];
                random.NextBytes(randomData);
                string newNonce = BitConverter.ToUInt64(randomData, 0).ToString("X16");
                Session["Nonce"] = newNonce;
                return View(model:newNonce);
            }

            Administrators administrator = model.Administrators.Where(x => x.Name == name).FirstOrDefault();
            string nonce = Session["Nonce"] as string;
            if (administrator == null || string.IsNullOrWhiteSpace(nonce))
            {
                return RedirectToAction("Index", "Post");
            }

            string computedHash;
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashInput = Encoding.ASCII.GetBytes(administrator.Password + nonce);
                byte[] hashData = sha256.ComputeHash(hashInput);
                StringBuilder stringBuilder = new StringBuilder();
                foreach (byte value in hashData)
                {
                    stringBuilder.AppendFormat("{0:X2}", value);
                }
                computedHash = stringBuilder.ToString();
            }
            Session["IsAdmin"] = (computedHash.ToLower() == hash.ToLower());
            return RedirectToAction("Index", "Post");
        }
        public ActionResult Logout()
        {
            Session["Nonce"] = null;
            Session["IsAdmin"] = false;
            return RedirectToAction("Index", "Post");
        }
    }
}