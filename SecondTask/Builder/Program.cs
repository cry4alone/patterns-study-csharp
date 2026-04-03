var builder = new OrderBuilder();

// Example 1: Standard Order
var standardOrder = builder
    .WithId(1)
    .WithCustomerName("John Doe")
    .WithItem("Laptop")
    .WithTotalAmount(1200.50m)
    .WithDeliveryAddress("123 Main St")
    .Build();
Console.WriteLine($"Standard Order: {standardOrder}");

// Example 2: Express Order
var expressOrder = builder
    .WithId(2)
    .WithCustomerName("Jane Smith")
    .WithItems(new List<string> { "Phone", "Charger" })
    .WithTotalAmount(800.00m)
    .WithDeliveryAddress("456 Elm St")
    .AsExpress()
    .Build();
Console.WriteLine($"Express Order: {expressOrder}");

// Example 3: Gift Order with Promo Code
var giftOrder = builder
    .WithId(3)
    .WithCustomerName("Alice Johnson")
    .WithItem("Book")
    .WithTotalAmount(25.00m)
    .WithGiftWrap(true)
    .WithPromoCode("DISCOUNT10")
    .Build();
Console.WriteLine($"Gift Order: {giftOrder}");

// Example 4: Express Order without Address (Throws Exception)
try
{
    var invalidOrder = builder
        .WithId(4)
        .WithCustomerName("Bob Brown")
        .WithItem("Tablet")
        .WithTotalAmount(300.00m)
        .AsExpress()
        .Build();
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}

public record Order(
    int Id,
    string CustomerName,
    List<string> Items,
    decimal TotalAmount,
    string? DeliveryAddress,
    bool IsExpress,
    bool GiftWrap,
    string? PromoCode
);

public class OrderBuilder
{
    private int _id; 
    private string _customerName = string.Empty; // Default to avoid null
    private List<string> _items = new List<string>(); // Initialize to avoid null
    private decimal _totalAmount;
    private string? _deliveryAddress;
    private bool _isExpress;
    private bool _giftWrap;
    private string? _promoCode;

    public Order Build()
    {
        if (_isExpress && string.IsNullOrEmpty(_deliveryAddress))
        {
            throw new Exception("Address can't be empty with express delivery.");
        }

        return new Order(
            _id,
            _customerName,
            _items,
            _totalAmount,
            _deliveryAddress,
            _isExpress,
            _giftWrap,
            _promoCode
        );
    }

    public OrderBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public OrderBuilder WithCustomerName(string customerName)
    {
        _customerName = customerName;
        return this;
    }

    public OrderBuilder WithItem(string item)
    {
        _items.Add(item);
        return this;
    }

    public OrderBuilder WithItems(List<string> items)
    {
        _items.AddRange(items);
        return this;
    }

    public OrderBuilder WithTotalAmount(decimal totalAmount)
    {
        _totalAmount = totalAmount;
        return this;
    }

    public OrderBuilder WithDeliveryAddress(string deliveryAddress)
    {
        _deliveryAddress = deliveryAddress;
        return this;
    }

    public OrderBuilder AsExpress()
    {
        _isExpress = true;
        return this;
    }

    public OrderBuilder WithGiftWrap(bool giftWrap)
    {
        _giftWrap = giftWrap;
        return this;
    }

    public OrderBuilder WithPromoCode(string promoCode)
    {
        _promoCode = promoCode;
        return this;
    }
}