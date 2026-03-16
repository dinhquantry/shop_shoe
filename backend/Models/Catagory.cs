using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set;}
        [Required,MaxLength(100)]
        public string Name{get;set;}=string.Empty;
        [Required,MaxLength(150)]
        public string Slug{get;set;}=string.Empty;
        public bool IsActive{get;set;}=true;
        public int? ParentId{get;set;}
        public Category? ParentCategory{get;set;}
        public ICollection<Category>SubCategories{get;set;}=new List<Category>();
        public ICollection<Product> Products{get;set;}=new List<Product>();
    }
}