using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetaPoco;

namespace AddonTemplate.Web.Models
{
    [TableName("Purchases")]
    [PrimaryKey("PurchaseId")]
    public class Purchase
    {
        public int PurchaseId { get; set; }
        public string CreatedBy { get; set; }
        public string UniqueId { get; set; }
        public Plan Plan { get; set; }
        public string ProviderId { get; set; }
        public ProvisionStatus ProvisionStatus { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecretKey { get; set; }


        public static void Save(Database db, Purchase purchase)
        {
            var existing = db.SingleOrDefault<Purchase>("SELECT * FROM purchases WHERE UniqueId=@0 ", purchase.UniqueId);
            if (existing != null)
                db.Update("purchases", "PurchaseId", existing);
            else
                db.Insert("purchases", "PurchaseId", purchase);
        }

    }
}