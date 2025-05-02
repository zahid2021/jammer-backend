using System;
using System.Collections.Generic;

namespace Jammer.OrderModule.DTOs
{
    public class OrderDTOS
    {
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string Region { get; set; }

        public List<OrderItemDTO> Items { get; set; }
    }public class OrderUserIDDTOS
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string fullname { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string Region { get; set; }

        public List<OrderItemDTO> Items { get; set; }
    }

}
