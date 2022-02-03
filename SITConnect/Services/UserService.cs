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
            try
            {
                newuser.Email.ToLower();
                if (UserExists(newuser.Email))
                {
                    return false;
                }
                else
                {

                    newuser.TwoFactorEnabled = false;
                    // hashing
                    string salt = BC.GenerateSalt(12);
                    var password = BC.HashPassword(newuser.Password, salt);
                    newuser.Password = password;
                    newuser.Password1 = password;
                    newuser.Password2 = password;
                    //Encrypt
                    byte[] plainText = Encoding.UTF8.GetBytes(newuser.Card_number);
                    RijndaelManaged cipher = new RijndaelManaged();
                    cipher.Padding = PaddingMode.Zeros;
                    newuser.Key = cipher.Key;
                    newuser.IV = cipher.IV;
                    ICryptoTransform encryptTransform = cipher.CreateEncryptor();
                    byte[] cipherText = encryptTransform.TransformFinalBlock(plainText, 0, plainText.Length);
                    string cipherString = Convert.ToBase64String(cipherText);
                    newuser.Card_number = cipherString;

                    newuser.Password_Changed_Time = DateTime.Now;
                    newuser.LockedoutEnabled = false;
                    newuser.Email = newuser.Email.ToLower();
                    newuser.Id = Guid.NewGuid().ToString();
                    _context.Add(newuser);
                    _context.SaveChanges();
                    return true;
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                
                return false;
                throw;

            }
}

        public bool pwdcheck(string Id, string pwd)
        {
            User account = GetUserById(Id);
            if (BC.Verify(pwd, account.Password))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool Login(User existuser)
        {

            if (UserExists(existuser.Email) == false)
            {
                return false;
            }

            else
            {

                User account = GetUserByEmail(existuser.Email);
                if (BC.Verify(existuser.Password, account.Password))
                {
                  
                    return true;
                }
                else 
                {

                    account.LoginFail += 1;
                    if (account.LoginFail >= 3)
                    {
                        account.LoginFail = 0;
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

        public Boolean Lockout(string email)
        {
            try { 
            if (UserExists(email))
            {
                User user = GetUserByEmail(email);
                var time = (DateTime.Now - user.LockoutEnd).Minutes;
                if (time >= 1 || user.LockedoutEnabled == false )
                {
                        user.LockedoutEnabled = false;
                        _context.Attach(user).State = EntityState.Modified;
                        _context.Update(user);
                        _context.SaveChanges();
                        return true;
                }
                else
                {
         
                    return false;
                }
            }
            else
                return true;
        }
            catch (DbUpdateConcurrencyException)
            {
                
                return false;
                throw;

            }

}
        public Boolean Enable_twofactor(string email)
        {
            try { 
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
            catch (DbUpdateConcurrencyException)
            {

                return false;
                throw;

            }
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
        public Boolean Min_password(string Id)
        {
            User user = GetUserById(Id);
            if (UserExists(user.Email))
            {
                
                var time = (DateTime.Now - user.Password_Changed_Time).Minutes;
                if (time >= 3)
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
                if (time > 120)
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
            try { 
            User theuser = GetUserById(existuser.Id);
            if (UserExists(theuser.Email))
            {
                User user = GetUserByEmail(theuser.Email);
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
                    user.Password_Changed_Time = DateTime.Now;
                    _context.Attach(user).State = EntityState.Modified;
                    _context.Update(user);
                    _context.SaveChanges();
                    return true;
                }
            }
            else
                return false;
                        }
            catch (DbUpdateConcurrencyException)
            {
                
                return false;
                throw;

            }
        }
        public string Decrypt(string Text, byte[] Key, byte[] IV)
        {
            byte[] cipherText = Convert.FromBase64String(Text);
            RijndaelManaged cipher = new RijndaelManaged();
            cipher.Key = Key;
            cipher.IV = IV;
            cipher.Padding = PaddingMode.Zeros;
            ICryptoTransform decryptTransform = cipher.CreateDecryptor();
            byte[] decryptedText = decryptTransform.TransformFinalBlock(cipherText, 0, cipherText.Length);
            string decryptedString = Encoding.UTF8.GetString(decryptedText);
            decryptedString.ToArray();
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
        public User GetUserById(string Id)
        {
            User theuser = _context.Users.SingleOrDefault(o => o.Id == Id);
            return theuser;
        }
        private bool UserExists(string email)
        {
            return _context.Users.Any(e => e.Email == email);
        }
    }
}
