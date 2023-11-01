using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace GadgetStore.DATA.EF.Models//Metadata
{
    //internal class Partials
    //{
    //}
    [ModelMetadataType(typeof(CategoryMetadata))]
    public partial class Category { /*custom props here*/ }

    [ModelMetadataType(typeof(OrderMetadata))]
    public partial class Order { }

    [ModelMetadataType(typeof(ProductMetadata))]
    public partial class Product { }

    [ModelMetadataType(typeof(SupplierMetadata))]
    public partial class Supplier { }

    [ModelMetadataType(typeof(UserDetailMetadata))]
    public partial class UserDetail
    {
        //custom properties can be stored in the Partial/Buddy class.
        [Display(Name = "Full Name")]
        public string FullName => FirstName + " " + LastName;
    }
}

//Models -> Entity Models : Don't make changes here
//Metadata -> Metadata classes : Data Annotations only
//           Partial Classes : bind metadata to the entity models - also custom props