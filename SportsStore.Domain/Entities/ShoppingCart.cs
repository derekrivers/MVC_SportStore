using System.Collections.Generic;
using System.Linq;

namespace SportsStore.Domain.Entities
{
    public class ShoppingCart
    {
        private List<CartLine> _lineCollection = new List<CartLine>();

        public void AddItem(Product product, int quantity)
        {
            CartLine line = _lineCollection.FirstOrDefault(p => p.Product.ProductId == product.ProductId);

            if (line == null)
            {
                _lineCollection.Add(new CartLine() {Product = product, Quantity = quantity});
            }
            else
            {
                line.Quantity += quantity;
            }

        }

        public void RemoveItem(Product product)
        {
            _lineCollection.RemoveAll(p => p.Product.ProductId == product.ProductId);
        }

        public decimal ComputeTotalValue()
        {
            return _lineCollection.Sum(e => e.Product.Price* e.Quantity);
        }

        public void Clear()
        {
            _lineCollection.Clear();
        }

        public IEnumerable<CartLine> Lines
        {
            get { return _lineCollection; }
        } 
    }
}
