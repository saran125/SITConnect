using System;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Configuration;
using SITConnect.Models;

namespace SITConnect.Repository
{
    public class AuditRepository
    {
        private SITConnectDBcontext _context;
        public AuditRepository(SITConnectDBcontext context)
        {
            _context = context;

        }

        public void InsertAuditLogs(AuditModel objauditmodel)
        {
            try
            {
                objauditmodel.Id = Guid.NewGuid().ToString();
                _context.Add(objauditmodel);
                _context.SaveChanges();

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}