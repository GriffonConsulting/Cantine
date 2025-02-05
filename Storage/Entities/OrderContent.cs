﻿using Domain.Product;

namespace EntityFramework.Entities
{
    public class OrderContent : IEfEntity
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public required string ProductName { get; set; }
        public ProductType ProductType { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public virtual Order? Order { get; set; }
    }
}
