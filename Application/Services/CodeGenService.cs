using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnetbase.Application.Database;
using dotnetbase.Application.Models;

namespace dotnetbase.Application.Services
{
    public class CodeGenService
    {
        private readonly DatabaseContext _db;
        private readonly IWebHostEnvironment _env;

        public CodeGenService(DatabaseContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;

        }

        private KeyValuePair<bool, string> GetUniqueIDFromDate()
        {
            KeyValuePair<bool, string> response = new KeyValuePair<bool, string>(false, "");
            DateTime thedate = DateTime.Now;
            try
            {
                response = GenerateUniqueIDFromDate(thedate);
                return response;
            }
            catch (Exception ex)
            {
                response = new KeyValuePair<bool, string>(false, ex.Message);
                return response;
            }

        }

        private KeyValuePair<bool, string> GetUniqueIDFromDate(DateTime thedate)
        {
            KeyValuePair<bool, string> response = new KeyValuePair<bool, string>(false, "");
            try
            {
                response = GenerateUniqueIDFromDate(thedate);
                return response;
            }
            catch (Exception ex)
            {
                response = new KeyValuePair<bool, string>(false, ex.Message);
                return response;
            }

        }

        private KeyValuePair<bool, string> GenerateUniqueIDFromDate(DateTime TheDate)
        {
            KeyValuePair<bool, string> response = new KeyValuePair<bool, string>(false, "Cannot generate code at the moment");
            try
            {
                string UniqueID = "";
                string StrDate = TheDate.ToString("dd-MM-yyyy");
                string[] DateSplit = StrDate.Split(new Char[] { '-' });
                string day = DateSplit[0];
                string month = DateSplit[1];
                string year = DateSplit[2];
                Dictionary<string, string> Months = new Dictionary<string, string>
            {
            {"01","25"}, {"02","68"}, {"03","69"},
            {"04","31"},
            {"05","42"},
            {"06","90"},
            {"07","38"},
            {"08","40"},
            {"09","56"},
            {"10","63"},
            {"11","45"},
            {"12","90"}
            };

                Dictionary<string, string> Days = new Dictionary<string, string>
            {
            {"01" , "50"},
            {"02" , "31"},
            {"03" , "23"},
            {"04" , "12"},
            {"05" , "54"},
            {"06" , "67"},
            {"07" , "87"},
            {"08" , "90"},
            {"09" , "11"},
            {"10" , "34"},
            {"11" , "22"},
            {"12" , "38"},
            {"13" , "88"},
            {"14" , "78"},
            {"15" , "33"},
            {"16" , "54"},
            {"17" , "67"},
            {"18" , "77"},
            {"19" , "29"},
            {"20" , "59"},
            {"21" , "17"},
            {"22" , "32"},
            {"23" , "44"},
            {"24" , "66"},
            {"25" , "00"},
            {"26" , "04"},
            {"27" , "05"},
            {"28" , "03"},
            {"29" , "08"},
            {"30" , "20"},
            {"31" , "45"}
            };

                Dictionary<string, string> Years = new Dictionary<string, string>
            {
            {"2013" , "33"},
            {"2014" , "44"},
            {"2015" , "55"},
            {"2016" , "66"},
            {"2017" , "77"},
            {"2018" , "88"},
            {"2019" , "99"},
            {"2020" , "31"},
            {"2021" , "52"},
            {"2022" , "14"},
            {"2023" , "24"},
            {"2024" , "57"},
            {"2025" , "68"},
            {"2026" , "30"},
            {"2027" , "70"},
            {"2028" , "73"},
            {"2029" , "87"},
            {"2030" , "62"},
            {"2031" , "91"},
            {"2032" , "83"},
            {"2033" , "34"},
            {"2034" , "45"},
            {"2035" , "48"}
            };
                string CodeDay = Days.First(x => x.Key == DateSplit[0]).Value;
                string CodeMonth = Months.First(x => x.Key == DateSplit[1]).Value;
                string CodeYear = Years.First(x => x.Key == DateSplit[2]).Value;
                string DateCode = CodeDay + CodeMonth + CodeYear;
                string nextid = PadZeroes(4, GetNextId(DateCode));
                UniqueID = DateCode + nextid;
                response = new KeyValuePair<bool, string>(true, UniqueID);
                return response;
            }
            catch (Exception ex)
            {
                response = new KeyValuePair<bool, string>(false, ex.Message);
                return response;
            }

        }

        private int GetNextId(string TableName)
        {

            var Tab = _db.CodeGenerators.Where(x => x.EntityName == TableName).FirstOrDefault();

            if (Tab != null)
            {
                Tab.EntityId += 1;
                _db.SaveChanges();
                return Tab.EntityId;
            }
            else
            {
                CodeGenerator datagen = new CodeGenerator { EntityName = TableName, EntityId = 1 };
                _db.CodeGenerators.Add(datagen);
                _db.SaveChanges();
                return 1;
            }
        }


        private string PadZeroes(int length, int number)
        {
            string format = "D" + length.ToString();
            string paddednumber = number.ToString(format);
            return paddednumber;
        }

        public KeyValuePair<bool, string> GenerateClientCode()
        {
            string applNo = this.PadZeroes(4, this.GetNextId(DateTime.Now.Year.ToString() + "Client"));
            applNo = "YC" + applNo;
            return new KeyValuePair<bool, string>(true, applNo);

        }

        public KeyValuePair<bool, string> GenerateProviderCode()
        {
            string applNo = this.PadZeroes(4, this.GetNextId(DateTime.Now.Year.ToString() + "Provider"));
            applNo = "YD" + applNo;
            return new KeyValuePair<bool, string>(true, applNo);

        }

        public KeyValuePair<bool, string> GeneratePartnerCode()
        {
            string applNo = this.PadZeroes(4, this.GetNextId(DateTime.Now.Year.ToString() + "Provider"));

            applNo = "YP" + applNo;
            return new KeyValuePair<bool, string>(true, applNo);
        }

        public KeyValuePair<bool, string> GenerateOrderCode()
        {
            string applNo = this.PadZeroes(4, this.GetNextId(DateTime.Now.Year.ToString() + "Order"));

            string year = DateTime.Now.Year.ToString();
            string month = DateTime.Now.Month.ToString("d2");


            applNo = "YO" + year + month + applNo;
            return new KeyValuePair<bool, string>(true, applNo);
        }
    }
}