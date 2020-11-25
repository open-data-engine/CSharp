using System;
using System.Collections.Generic;
using DotNetEnv;
using OpenDataEngine.Attribute;
using OpenDataEngine.Query;
using OpenDataEngine.Relation;
using OpenDataEngine.Source;
using OpenDataEngine.Strategy;

namespace Example.Model.Relation
{
    public enum Status
    {
        Active,
        Inactive,
        Blocked,
    }

    public enum Gender
    {
        None,
        M,
        F,
        Family,
        FF,
        MM,
        MF,
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
    [Relations(typeof(Relation))]
    public class Relation : Queryable<Relation>
    {
        #region Fields
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

        public Location Location { get; set; }

        public String Salutation => Gender switch
        {
            Gender.M => "Mr.",
            Gender.F => "Mrs.",
            Gender.Family => "Fam.",
            _ => "",
        };

        public String DisplayName => (FirstName, MiddleName, SurName) switch
        {
            ("", "", "") => throw new Exception("No names given"),
            ("", "", _) => $"{SurName}",
            ("", _, "") => $"{MiddleName}",
            ("", _, _) => $"{MiddleName} {SurName}",
            (_, "", "") => $"{FirstName}",
            (_, "", _) => $"{FirstName} {SurName}",
            (_, _, "") => $"{FirstName} {MiddleName}",
            (_, _, _) => $"{FirstName} {MiddleName} {SurName}",
        };

        public String FormalDisplayName => $"{Salutation} {DisplayName}";
        public Boolean IsUser => true;
        #endregion

        #region Relations
        public IEnumerable<Extra> Extras { get; set; }
        public IEnumerable<Relation> Friends { get; set; }
        #endregion

        public static readonly IDictionary<String, ISource> Sources = new Dictionary<String, ISource>
        {
            {
                "default",
                new Database(Env.GetString("DB_HOST"), Env.GetString("DB_USER"), Env.GetString("DB_PASS"),
                    new
                    {
                        ID = "Customer_ID",
                        FirstName = "First_Name",
                        MiddleName = "Middle_Name",
                        SurName = "Sur_Name"
                    }, "General", "Customer")
            },
            { "cache", new Cache() },
        };

        public static readonly IDictionary<String, IStrategy> Strategies = new Dictionary<String, IStrategy>
        {
            { "default", new CacheFirst("cache", "default") },
        };

        public static readonly IRelation?[] Relations =
        {
            new OwnsMany<Relation, Extra>(nameof(Extras), (r, b) => r.ID == b.RelationID),
            new HasMany<Relation, Relation>(nameof(Friends), (r, f) => r.ID == f.ID),
        };
    }
}