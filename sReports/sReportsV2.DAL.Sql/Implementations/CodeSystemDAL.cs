﻿using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.CodeSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Implementations
{
    public class CodeSystemDAL : BaseDisposalDAL, ICodeSystemDAL
    {
        public CodeSystemDAL(SReportsContext context) : base(context)
        {
        }

        public List<CodeSystem> GetAll()
        {
            return context.CodeSystems.ToList();
        }

        public int GetAllCount()
        {
            return context.CodeSystems.Count();
        }

        public CodeSystem GetById(int id)
        {
            return context.CodeSystems.FirstOrDefault(c => c.CodeSystemId == id);
        }

        public CodeSystem GetBySAB(string SAB)
        {
            return context.CodeSystems.FirstOrDefault(s => s.SAB == SAB);
        }

        public CodeSystem GetByValue(string value)
        {
            return context.CodeSystems.FirstOrDefault(s => s.Value == value);
        }

        public void InsertMany(List<CodeSystem> codeSystems)
        {
            IEnumerable<string> codeSystemsInDb = GetAll().Select(x => x.Value);
            foreach (var codeSystem in codeSystems) 
            {
                if (!codeSystemsInDb.Contains(codeSystem.Value))
                {
                    context.CodeSystems.Add(codeSystem);
                }
            }
            context.SaveChanges();
        }

        public void InsertOrUpdate(CodeSystem codeSystem)
        {
            if (codeSystem.CodeSystemId == 0)
            {
                context.CodeSystems.Add(codeSystem);
            }

            context.SaveChanges();
        }
    }
}
