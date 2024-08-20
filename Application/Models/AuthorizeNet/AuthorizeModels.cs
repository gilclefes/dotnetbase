using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetbase.Application.Models.AuthorizeNet
{
    public class AuthorizeModels
    {

    }

    public class CreateCustomerProfileRequest
    {
        public required MerchantAuthentication merchantAuthentication { get; set; }
        public CreateProfile? profile { get; set; }
        public string? validationMode { get; set; }
    }

    public class CreateCustomerProfileResponse
    {

        public string? customerProfileId { get; set; }
        public string[]? customerPaymentProfileIdList { get; set; }
        public string[]? customerShippingAddressIdList { get; set; }
        public string[]? validationDirectResponseList { get; set; }
        public Messages? messages { get; set; }
    }

    public class GetCustomerProfileRequest
    {
        public required MerchantAuthentication merchantAuthentication { get; set; }
        public string? customerProfileId { get; set; }
        public string? includeIssuerInfo { get; set; }
    }

    public class GetCustomerProfileResponse
    {
        public GetProfile? profile { get; set; }
        public Messages? messages { get; set; }
        public string? includeIssuerInfo { get; set; }
    }

    public class UpdateCustomerProfileRequest
    {
        public required MerchantAuthentication merchantAuthentication { get; set; }
        public CreateProfile? profile { get; set; }
        public string? validationMode { get; set; }
    }

    public class UpdateCustomerProfileResponse
    {
        public required MerchantAuthentication merchantAuthentication { get; set; }
        public CreateProfile? profile { get; set; }
        public string? validationMode { get; set; }
    }

    public class GetCustomerPaymentProfileRequest
    {
        public required MerchantAuthentication merchantAuthentication { get; set; }
        public string? customerProfileId { get; set; }
        public string? customerPaymentProfileId { get; set; }

        public string? includeIssuerInfo { get; set; }


    }


    public class GetPaymentProfileResponse
    {
        public PaymentProfile? paymentProfile { get; set; }
        public Messages? messages { get; set; }
    }

    public class CreateCustomerPaymentProfileRequest
    {
        public required MerchantAuthentication merchantAuthentication { get; set; }
        public string? customerProfileId { get; set; }
        public PaymentProfile? paymentProfile { get; set; }
        public string? validationMode { get; set; }
    }

    public class CreateCustomerPaymentProfileResponse
    {
        public string? customerProfileId { get; set; }
        public string? customerPaymentProfileId { get; set; }
        public string? validationDirectResponse { get; set; }
        public Messages? messages { get; set; }
    }

    public class UpdateCustomerPaymentProfileRequest
    {
        public required MerchantAuthentication merchantAuthentication { get; set; }
        public string? customerProfileId { get; set; }
        public PaymentProfile? paymentProfile { get; set; }
        public string? validationMode { get; set; }
    }

    public class UpdateCustomerPaymentProfileResponse
    {
        public string? customerProfileId { get; set; }
        public string? customerPaymentProfileId { get; set; }
        public string? validationDirectResponse { get; set; }
        public Messages? messages { get; set; }
    }

    public class MerchantAuthentication
    {
        public required string name { get; set; }
        public required string transactionKey { get; set; }
    }

    public class CreateProfile
    {
        public string? merchantCustomerId { get; set; }
        public string? description { get; set; }
        public string? email { get; set; }

        public string? customerProfileId { get; set; }
        public PaymentProfiles? paymentProfiles { get; set; }
    }

    public class GetProfile
    {
        public string? merchantCustomerId { get; set; }
        public string? description { get; set; }
        public string? email { get; set; }

        public string? profileType { get; set; }
        public string? customerProfileId { get; set; }
        public PaymentProfiles[]? paymentProfiles { get; set; }


    }

    public class PaymentProfiles
    {
        public string? customerType { get; set; }
        public AuthorizePayment? payment { get; set; }
        public BillTo? billTo { get; set; }
    }

    public class AuthorizePayment
    {
        public CreditCard? creditCard { get; set; }
        public BankAccount? bankAccount { get; set; }
    }




    public class PaymentProfile
    {
        //  public string? customerProfileId { get; set; }
        public BillTo? billTo { get; set; }
        public AuthorizePayment? payment { get; set; }
        public bool defaultPaymentProfile { get; set; }
        public string? customerPaymentProfileId { get; set; }

        public string? originalNetworkTransId { get; set; }




    }








    public class CreditCard
    {
        public string? cardNumber { get; set; }
        public string? expirationDate { get; set; }
        public string? cardCode { get; set; }
    }

    public class BankAccount
    {
        public string? accountType { get; set; }
        public required string routingNumber { get; set; }
        public required string accountNumber { get; set; }
        public required string nameOnAccount { get; set; }
        // public required string bankName { get; set; }
        //public string? echeckType { get; set; }

    }



    public class BillTo
    {

        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? address { get; set; }
        public string? city { get; set; }
        public string? state { get; set; }
        public string? zip { get; set; }
        public string? country { get; set; }
        public string? phoneNumber { get; set; }
    }

    public class Messages
    {
        public string? resultCode { get; set; }
        public List<Message>? message { get; set; }
    }

    public class Message
    {
        public string? code { get; set; }
        public string? text { get; set; }
    }

    public class AuthorizeGenericResponse
    {
        public string? resultCode { get; set; }
        public string? errorCode { get; set; }
        public string? message { get; set; }

    }

    public class CreateTransactionRequest
    {
        public MerchantAuthentication merchantAuthentication { get; set; }
        public string refId { get; set; }
        public TransactionRequest transactionRequest { get; set; }
    }

    public class CreateRefundTransactionRequest
    {
        public MerchantAuthentication merchantAuthentication { get; set; }
        public string refId { get; set; }
        public RefundTransactionRequest transactionRequest { get; set; }

    }

    public class GetTransactionDetailsRequest
    {
        public MerchantAuthentication merchantAuthentication { get; set; }
        public string transId { get; set; }

    }



    public class TransactionRequest
    {
        public string transactionType { get; set; }
        public string amount { get; set; }
        public Profile profile { get; set; }
        public LineItems lineItems { get; set; }
        public ProcessingOptions processingOptions { get; set; }
        public SubsequentAuthInformation subsequentAuthInformation { get; set; }
        public AuthorizationIndicatorType authorizationIndicatorType { get; set; }
    }

    public class RefundTransactionRequest
    {
        public string transactionType { get; set; } = "refundTransaction";
        public string amount { get; set; }
        public string currencyCode { get; set; }
        public AuthorizePayment payment { get; set; }
        public string refTransId { get; set; }

    }

    public class Profile
    {
        public string customerProfileId { get; set; }
        public TransactionPaymentProfile paymentProfile { get; set; }
    }

    public class TransactionPaymentProfile
    {
        public string paymentProfileId { get; set; }
    }

    public class LineItems
    {
        public LineItem lineItem { get; set; }
    }

    public class LineItem
    {
        public string itemId { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string quantity { get; set; }
        public string unitPrice { get; set; }
    }

    public class ProcessingOptions
    {
        public string isSubsequentAuth { get; set; }
    }

    public class SubsequentAuthInformation
    {
        public string originalNetworkTransId { get; set; }
        public string originalAuthAmount { get; set; }
        public string reason { get; set; }
    }

    public class AuthorizationIndicatorType
    {
        public string authorizationIndicator { get; set; }
    }

    public class TransactionResponse
    {
        public string responseCode { get; set; }
        public string authCode { get; set; }
        public string avsResultCode { get; set; }
        public string cvvResultCode { get; set; }
        public string cavvResultCode { get; set; }
        public string transId { get; set; }
        public string refTransID { get; set; }
        public string transHash { get; set; }
        public string testRequest { get; set; }
        public string accountNumber { get; set; }
        public string accountType { get; set; }
        public List<Message> messages { get; set; }
        public string transHashSha2 { get; set; }
        public Profile profile { get; set; }
        public int SupplementalDataQualificationIndicator { get; set; }
        public string networkTransId { get; set; }

        public decimal? authAmount { get; set; }
        public decimal? settleAmount { get; set; }

        public string? responseReasonCode { get; set; }
        public string? responseReasonDescription { get; set; }
        public AuthorizePayment? payment { get; set; }
    }

    public class RootTransactionResponse
    {
        public TransactionResponse transactionResponse { get; set; }
        public string refId { get; set; }
        public Messages messages { get; set; }
    }

    public class GetTransactionDetailResponse
    {
        public TransactionDetailResponse transaction { get; set; }

        public Messages messages { get; set; }
    }



    public class TransactionDetailResponse
    {
        public string transId { get; set; }
        public string submitTimeUTC { get; set; }
        public string submitTimeLocal { get; set; }
        public string transactionType { get; set; }
        public string transactionStatus { get; set; }
        public string responseCode { get; set; }
        public string responseReasonCode { get; set; }
        public string responseReasonDescription { get; set; }
        public string AVSResponse { get; set; }

        public decimal authAmount { get; set; }
        public decimal settleAmount { get; set; }
        public List<LineItem> lineItems { get; set; }

        public AuthorizePayment payment { get; set; }

        public BillTo billTo { get; set; }

        public string customerIP { get; set; }
        public Profile profile { get; set; }
    }

}