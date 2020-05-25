using System;
using System.Linq;
using DotNetEnv;
using OpenDataEngine;
using OpenDataEngine.Attribute;
using OpenDataEngine.Query;
using OpenDataEngine.Source;
using OpenDataEngine.Strategy;

namespace Example.Model
{
    public enum Status
    {
        Active,
        Inactive,
        Blocked,
    }

    public enum Gender
    {
        M,
        F,
        Family,
    }

    public enum Entry
    {
        Retail,
        Webshop,
    }

    public enum Group
    {
        Companies,
        Private,
        Dispatch,
        Caregiver,
    }

    [Strategies(typeof(Relation))]
    [Sources(typeof(Relation))]
    public class Relation : Queryable<Relation>
    {
        public UInt32 ID { get; set; }
        public String Username { get; set; }

        public Status Status { get; set; }
        public UInt32 InvoiceDebtor { get; set; }
        public Boolean SaleCombineOrder { get; set; }
        public Gender Gender { get; set; }
        public String FirstName { get; set; }
        public String MiddleName { get; set; }
        public String SurName { get; set; }
        public String PhoneNumber { get; set; }
        public String MobilePhoneNumber { get; set; }
        public String FaxNumber { get; set; }
        public String Email { get; set; }
        public String Website { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime WeddingDate { get; set; }
        public String Street { get; set; }
        public UInt32 HouseNumber { get; set; }
        public String HouseNumberAddition { get; set; }
        public String City { get; set; }
        public String PostalCode { get; set; }
        public UInt32 CountryID { get; set; }
        public String Language { get; set; }
        public UInt64 EAN { get; set; }
        public String BSNRSIN { get; set; }
        public String CoC { get; set; }
        public String EstablishmentNumber { get; set; }
        public String VAT { get; set; }
        public String Password { get; set; }
        public String Memo { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
        public Entry Entry { get; set; }
        public UInt32 PackingSlips { get; set; }
        public UInt32 State { get; set; }
        public Boolean DisplayVAT { get; set; }
        public UInt32 DisplayTurnover { get; set; }
        public Boolean Deleted { get; set; }
        public String EDIAdres { get; set; }
        public Group CustomerGroup { get; set; }
        public String Image { get; set; }

        public String FullName => String.Join(" ", new[] { FirstName, MiddleName, SurName }.Where(n => !String.IsNullOrEmpty(n)));

        public Book Book { get; set; }

        public static new readonly (String Key, ISource Source)[] Sources =
        {
            ("default", new Database(Env.GetString("DB_HOST"), Env.GetString("DB_USER"), Env.GetString("DB_PASS"), new { ID = "Customer_ID", FirstName = "First_Name", MiddleName = "Middle_Name", SurName = "Sur_Name" }, "General", "Customer")),
            ("cache", new Cache()),
        };

        public static new readonly (String, IStrategy)[] Strategies =
        {
            ("default", new CacheFirst("cache", "default")),
        };
    }
}