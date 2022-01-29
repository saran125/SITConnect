using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SITConnect.Models;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;
using crypto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;


namespace SITConnect.Services
{
    public class UserService
    {
       

        private SITConnectDBcontext _context;
        public UserService(SITConnectDBcontext context)
        {
            _context = context;

        }


        public bool Register(User newuser)
        {
            if (UserExists(newuser.Email))
            {
                return false;
            }
            else
            {

                newuser.TwoFactorEnabled = false;
                string salt = BC.GenerateSalt(12);
                var password = BC.HashPassword(newuser.Password, salt);
                newuser.Password = password;
               
                RijndaelManaged cipher = new RijndaelManaged();
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();
         
                byte[] plainText = Encoding.UTF8.GetBytes(newuser.Card_number);
                //Encrypt
                byte[] cipherText = encryptTransform.TransformFinalBlock(plainText, 0,
                plainText.Length);
                newuser.Card_number = Convert.ToBase64String(cipherText);
                newuser.Key = cipherText;
                newuser.Password_Changed_Time = DateTime.Now;
                newuser.LockedoutEnabled = false;
                newuser.Email = newuser.Email.ToLower();
                newuser.Id = Guid.NewGuid().ToString();
                _context.Add(newuser);
                _context.SaveChanges();
                return true;
            }
        }

        public bool Login(User existuser)
        {
            try
            {
                if (UserExists(existuser.Email) == false)
                {
                    return false; 
                }
                else {
                  //  return true;
                   
                    User account = GetUserByEmail(existuser.Email);
                    if (BC.Verify(existuser.Password, account.Password))
                    {
                        return true;
                    }
                    else
                    {

                        account.AccessFailedCount += 1;
                        if( account.AccessFailedCount >= 3){
                            account.LockedoutEnabled = true;
                            account.LockoutEnd = DateTime.Now;
                        }
                        _context.Attach(account).State = EntityState.Modified;
                            _context.Update(account);
                            _context.SaveChanges();
                            return false;
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;

            }
            
            
        }

        public Boolean Lockout(string email)
        {

            if (UserExists(email))
            {
                User user = GetUserByEmail(email);
                var time = (DateTime.Now - user.LockoutEnd).Minutes;
                if (time > 1 || user.LockedoutEnabled == false)
                {
                    return true;
                }
                else
                {
                    user.AccessFailedCount = 0;
                    user.LockedoutEnabled = false;
                    _context.Attach(user).State = EntityState.Modified;
                    _context.Update(user);
                    _context.SaveChanges();
                    return false;
                }
            }
            else
                return true;
        }
        public Boolean Enable_twofactor(string email)
        {

            if (UserExists(email))
            {
                User user = GetUserByEmail(email);
                user.TwoFactorEnabled = true;
                _context.Attach(user).State = EntityState.Modified;
                _context.Update(user);
                _context.SaveChanges();
                return true;
            }
            else
                return false;
        }
        public Boolean Twofactor(string email)
        {
            if (UserExists(email))
            {
                User user = GetUserByEmail(email);
                return user.TwoFactorEnabled;
            }
            else
                return false;
        }
        public Boolean Min_password(string email)
        {

            if (UserExists(email))
            {
                User user = GetUserByEmail(email);
                var time = (DateTime.Now - user.Password_Changed_Time).Minutes;
                if (time > 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
                return false;
        }
        public Boolean Max_password(string email)
        {

            if (UserExists(email))
            {
                User user = GetUserByEmail(email);
                var time = (DateTime.Now - user.Password_Changed_Time).Minutes;
                if (time > 3)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
                return false;
        }

        public Boolean reset_password(User existuser)
        {
            if (UserExists(existuser.Email))
            {
                User user = GetUserByEmail(existuser.Email);
                string salt = BC.GenerateSalt(12);
                var password = BC.HashPassword(existuser.Password, salt);
                 if(BC.Verify(existuser.Password, user.Password))
                {
                    return false;
                }
                else if (BC.Verify(existuser.Password, user.Password1))
                {
                    return false;
                }
                else if (BC.Verify(existuser.Password, user.Password2))
                {
                    return false;
                }
                else
                {
                    user.Password2 = user.Password1;
                    user.Password1 = user.Password;
                    user.Password = password;
                    _context.Attach(user).State = EntityState.Modified;
                    _context.Update(user);
                    _context.SaveChanges();
                    return true;
                }
            }
            else
                return false;
        }
        public string Decrypt(byte[] cipherText)
        {
            RijndaelManaged cipher = new RijndaelManaged();
            ICryptoTransform decryptTransform = cipher.CreateDecryptor();
            byte[] decryptedText = decryptTransform.TransformFinalBlock(cipherText, 0, cipherText.Length);

            string decryptedString = Encoding.UTF8.GetString(decryptedText);
            return decryptedString;
        }

        public User Theuser(User existuser)
        {
            User account = GetUserByEmail(existuser.Email);
            return account;
        }

        public User GetUserByEmail(string Email)
        {
            User theuser = _context.Users.SingleOrDefault(o => o.Email == Email);
            return theuser;
        }
        private bool UserExists(string email)
        {
            return _context.Users.Any(e => e.Email == email);
        }


    }
}
