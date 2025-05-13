namespace KOL1.Models.DTOs;

public class DeliverDTO
{
    public DateTime date { get; set; }
    public CustomerDTO customer { get; set; }
    public DriverDTO driver { get; set; }
    public List<ProductDTO> products { get; set; }
}

public class CustomerDTO
{
    public string firstName { get; set; }
    public string lastName { get; set; }
    public DateTime dateOfBirth { get; set; }
}

public class DriverDTO
{
    public string firstName { get; set; }
    public string lastName { get; set; }
    public string licenceNumber { get; set; }  
}