//Step 1:
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//Step 2:
namespace GadgetStore.DATA.EF.Models//.Metadata
{
    //Step 3:
    //internal class Metadata
    //{
    //}
    /*
     * COMMON METADATA ATTRIBUTES:
     * Structural:
     * [Key] - for the primary key
     * [Unicode(true/false)] - nvarchar vs varchar
     * [Column(TypeName = "money")] - not usually used
     * [ForeignKey("ProductId")] - used with navigation properties
     * [InverseProperty("TableName")]
     * 
     * Display:
     * [Display(Name = "First Name")] - updates labels in our html code generation
     * [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true/false)]
     *  - if applyformatineditmode = true, must enter dates as 'MM/dd/yyyy'
     *  
     * Validation
     * [Required(ErrorMessage = "Message")]
     * [StringLength(##, ErrorMessage = "Message here")]
     * [DataType(DataType.SomeType)] - password, phone number, social security number
     * [Range(min#, max#)]
     */
    public class CategoryMetadata
    {
        [Key]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "* Required")]
        [StringLength(50)] //using the default error message
        [Display(Name = "Category")]
        [Unicode(false)]//from Microsoft.EntityFrameworkCore, not normally used.
        public string CategoryName { get; set; } = null!;

        [StringLength(500)]
        [Display(Name = "Description")]
        [Unicode(false)]
        [UIHint("MultilineText")]//generated a <textarea> instead of an <input>
        public string? CategoryDescription { get; set; }

        //navigation props will ALWAYS be virtual. Usually, we exclude these from the metadata.
        [InverseProperty("Category")]//self documenting, but so far, seems to serve no other purpose. 
        public virtual ICollection<Product> Products { get; set; }
    }

    public class OrderMetadata
    {
        [Key]
        [Display(Name = "Order ID")]
        public int OrderId { get; set; }

        [Display(Name = "User")]
        public string UserId { get; set; } = null!;

        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]

        [Display(Name ="Order Date")]
        public DateTime OrderDate { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Ship To")]
        public string ShipToName { get; set; } = null!;

        [Required]
        [StringLength(50)]
        [Display(Name = "City")]
        public string ShipCity { get; set; } = null!;

        [StringLength(2, MinimumLength = 2)]
        [Display(Name = "State")]
        [DisplayFormat(NullDisplayText = "[N/A]")]
        public string? ShipState { get; set; }

        [Required]
        [StringLength(5, MinimumLength = 5)]
        [Display(Name = "Zip")]
        [DataType(DataType.PostalCode)]
        public string ShipZip { get; set; } = null!;


        //no more navigation props in the metadata
        [ForeignKey("UserId")]
        [InverseProperty("Orders")]
        public virtual UserDetail User { get; set; } = null!;

        [InverseProperty("Order")]
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
    }

    public class ProductMetadata
    {
        //public int ProductId { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Gadget")]
        public string ProductName { get; set; } = null!;

        [Required]
        [Range(0, (double)decimal.MaxValue)]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = false)]
        [Display(Name = "Price")]
        [DataType(DataType.Currency)]
        [DefaultValue(0)]
        public decimal ProductPrice { get; set; }

        [DisplayFormat(NullDisplayText = "No description given")]
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]//generates a textarea instead of a text input
        [StringLength(500)]
        public string? ProductDescription { get; set; }

        [Required]
        [Range(0, short.MaxValue)]
        [Display(Name = "In Stock")]
        public short UnitsInStock { get; set; }
        [Required]
        [Range(0, short.MaxValue)]
        [Display(Name = "On Order")]
        public short UnitsOnOrder { get; set; }

        [Display(Name = "Discontinued?")]
        public bool IsDiscontinued { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        [Display(Name = "Supplier")]
        public int? SupplierId { get; set; }

        [StringLength(75)]
        [Display(Name = "Image")]
        public string? ProductImage { get; set; }
    }

    public class SupplierMetadata
    {
        //public int SupplierId { get; set; }
        [Display(Name = "Supplier")]
        [Required]
        [StringLength(200)]
        [DisplayFormat(NullDisplayText = "[No Supplier]")]
        public string SupplierName { get; set; } = null!;

        [Required]
        [StringLength(150)]
        public string Address { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string City { get; set; } = null!;

        [StringLength(2, MinimumLength = 2)]
        [DisplayFormat(NullDisplayText = "[None]")]
        public string? State { get; set; }


        [StringLength(5, MinimumLength = 5)]
        [DataType(DataType.PostalCode)]
        [DisplayFormat(NullDisplayText = "[None]")]
        public string? Zip { get; set; }

        [DataType(DataType.PhoneNumber)]
        [DisplayFormat(NullDisplayText = "[None]")]
        public string? Phone { get; set; }
    }

    public class UserDetailMetadata
    {
        //public string UserId { get; set; } = null!;
        [Required]
        [Display(Name = "First Name")]
        [StringLength(50)]
        public string FirstName { get; set; } = null!;

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(50)]
        public string LastName { get; set; } = null!;

        [StringLength(150)]
        [DisplayFormat(NullDisplayText = "[None]")]
        public string? Address { get; set; }

        [StringLength(50)]
        [DisplayFormat(NullDisplayText = "[None]")]
        public string? City { get; set; }

        [StringLength(2, MinimumLength = 2)]
        [DisplayFormat(NullDisplayText = "[None]")]
        public string? State { get; set; }

        [StringLength(5, MinimumLength = 5)]
        [DataType(DataType.PostalCode)]
        [DisplayFormat(NullDisplayText = "[None]")]
        public string? Zip { get; set; }
        [StringLength(24)]
        [DataType(DataType.PhoneNumber)]//does nothing
        [DisplayFormat(NullDisplayText = "[None]", ApplyFormatInEditMode = true, DataFormatString = "{0:(###) ###-####")]
        public string? Phone { get; set; }

    }
}
