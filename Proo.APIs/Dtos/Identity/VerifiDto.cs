namespace Proo.APIs.Dtos.Identity
{
    public class VerifiDto
    {
        public string Otp { get; set; }
        public string PhoneNumber { get; set; }
    }
    public class SendOTPDto
    {
        public string PhoneNumber { get; set; }
    }


}
